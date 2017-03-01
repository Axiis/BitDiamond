using System;
using System.Collections.Generic;
using Axis.Luna;
using BitDiamond.Core.Models;
using BitDiamond.Core.Services.Query;

using static Axis.Luna.Extensions.ExceptionExtensions;
using Axis.Pollux.RBAC.Services;
using BitDiamond.Core.Utils;
using Axis.Luna.Extensions;
using Newtonsoft.Json;
using Axis.Jupiter.Kore.Command;
using Axis.Pollux.Identity.Principal;
using System.Linq;

namespace BitDiamond.Core.Services
{
    public class BitLevelManager : IBitLevelManager, IUserContextAware
    {
        private IBitLevelQuery _query = null;
        private IReferralQuery _refQuery = null;
        private IPersistenceCommands _pcommand = null;
        private IUserAuthorization _authorizer = null;
        private IUserNotifier _notifier = null;
        private IBlockChainService _blockChain = null;
        private ISettingsManager _settingsManager = null;

        public IUserContext UserContext { get; private set; }

        public BitLevelManager(IUserAuthorization authorizer, IUserContext userContext, IBitLevelQuery query, 
                               IPersistenceCommands pcommand, IUserNotifier notifier, IBlockChainService blockChain,
                               ISettingsManager settingsManager, IReferralQuery refQuery)
        {
            ThrowNullArguments(() => userContext,
                               () => query,
                               () => pcommand,
                               () => notifier,
                               () => blockChain,
                               () => settingsManager,
                               () => refQuery);

            _query = query;
            _pcommand = pcommand;
            _authorizer = authorizer;
            _notifier = notifier;
            _blockChain = blockChain;
            _refQuery = refQuery;
            _settingsManager = settingsManager;

            UserContext = userContext;
        }
               

        /// <summary>
        /// Does both upgrading and recycling
        /// </summary>
        /// <returns></returns>
        public Operation<BitLevel> Upgrade()
        => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(), () =>
        {
            //attempt to confirm the upgrade donation first...
            //note that this method returns "null" for new users without bitlevels
            ConfirmUpgradeDonnation().Resolve();

            var maxLevelSettings = _settingsManager.GetSetting(Constants.Settings_MaxBitLevel).Resolve();
            var maxLevel = (int)maxLevelSettings.ParseData<long>();
            var targetUser = UserContext.CurrentUser();
            var currentLevel = _query.CurrentBitLevel(targetUser) ?? new BitLevel { Level = maxLevel, Cycle = 0, Donation = new BlockChainTransaction { Status = BlockChainTransactionStatus.Verified } };
            if (currentLevel.Donation.Status == BlockChainTransactionStatus.Unverified) throw new Exception("Current level donnation is still unverified");

            var myAddress = _query.GetActiveBitcoinAddress(targetUser)
                                  .ThrowIfNull("You do not have a valid block chain transaction address yet.");

            var nextLevel = (currentLevel.Level + 1) % (maxLevel + 1);
            var nextCycle = nextLevel == 0 ? currentLevel.Cycle + 1 : currentLevel.Cycle;

            var lastBeneficiaryAddressId = currentLevel.Donation.ReceiverId;
            var lastBeneficiaryAddress = _query.GetBitcoinAddressById(lastBeneficiaryAddressId);
            var nextUpgradeBeneficiary = NextUpgradeBeneficiary(lastBeneficiaryAddress?.Owner ?? targetUser, nextLevel, nextCycle).ThrowIfNull("Could not find a valid beneficiary");
            var beneficiaryAddress = _query.GetActiveBitcoinAddress(nextUpgradeBeneficiary);

            var bl = new BitLevel
            {
                Level = nextLevel,
                Cycle = nextCycle,
                DonationCount = 0,
                SkipCount = 0,
                User = targetUser
            };
            _pcommand.Add(bl).Resolve();

            var donation = new BlockChainTransaction
            {
                Amount = GetUpgradeAmount(nextLevel + 1),
                LedgerCount = 0,
                CreatedOn = DateTime.Now,
                Sender = myAddress,
                Receiver = beneficiaryAddress,
                Status = BlockChainTransactionStatus.Unverified,
                ContextType = Constants.TransactionContext_UpgradeBitLevel,
                ContextId = bl.Id.ToString()
            };
            _pcommand.Add(donation).Resolve();

            bl.DonationId = donation.Id;
            _pcommand.Update(bl);

            bl.Donation = donation;
            return bl;
        });

        public Operation<BlockChainTransaction> UpdateTransactionHash(string transactionHash)
        => _authorizer.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            var currentUser = UserContext.CurrentUser();
            var currentLevel = _query.CurrentBitLevel(currentUser);

            _query.GetTransactionWithHash(transactionHash)
                  .ThrowIfNotNull(new Exception("A transaction already exists in the system with the supplied hash"));

            currentLevel.Donation.TransactionHash = transactionHash;
            return _pcommand.Update(currentLevel.Donation);
        });

        public Operation<BlockChainTransaction> ConfirmUpgradeDonnation()
        => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(), () =>
        {
            var currentUser = UserContext.CurrentUser();
            var currentLevel = _query.CurrentBitLevel(currentUser);

            // for a new user...
            if (currentLevel == null) return null;

            //for a donation without receiver and sender
            else if (currentLevel.Donation.SenderId <= 0 ||
                    currentLevel.Donation.ReceiverId <= 0)
                throw new Exception("Invalid Donation transaction Receiver/Sender");

            var blockChainTransaction = _blockChain
                .GetTransactionDetails(currentLevel.Donation.TransactionHash)
                .Resolve()
                .ThrowIf(_bct => _bct.LedgerCount < 3, "Transaction ledger count is less than threshold");

            if (IsSameTransaction(currentLevel.Donation, blockChainTransaction) &&
                blockChainTransaction.Status == BlockChainTransactionStatus.Verified)
            {
                currentLevel.Donation.Status = BlockChainTransactionStatus.Verified;
                currentLevel.Donation.LedgerCount = blockChainTransaction.LedgerCount;
                _pcommand.Update(currentLevel.Donation);
                
                //increment receiver's donnation count
                var receiverLevel = _query.CurrentBitLevel(blockChainTransaction.Receiver.Owner);
                receiverLevel.DonationCount++;
                _pcommand.Update(receiverLevel);

                return currentLevel.Donation;
            }
            else throw new Exception("Invalid transaction hash");
        });

        public Operation ReceiverConfirmation(string transactionHash)
        => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(), () =>
        {
            var transaction = _query.GetTransactionWithHash(transactionHash)
                                    .ThrowIfNull("Transaction not found")
                                    .ThrowIf(_t => _t.Receiver.OwnerId != UserContext.CurrentUser().UserId, "Invalid transaction");

            transaction.Status = BlockChainTransactionStatus.Verified;
            _pcommand.Update(transaction).Resolve();
        });


        public Operation<BitLevel> CurrentUserLevel()
            => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(),
                                           () => _query.CurrentBitLevel(UserContext.CurrentUser()));

        public Operation<BitLevel> GetBitLevelById(long id)
            => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(),
                                           () => _query.GetBitLevelById(id));

        public Operation<IEnumerable<BitLevel>> UserUpgradeHistory()
            => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(),
                                           () => _query.GetBitLevelHistory(UserContext.CurrentUser()));

        public Operation<decimal> GetUpgradeFee(int level)
        => _authorizer.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            var feeVector = _settingsManager
                .GetSetting(Constants.Settings_UpgradeFeeVector)
                .Resolve()
                .ParseData(_json => JsonConvert.DeserializeObject<decimal[]>(_json, Constants.Misc_DefaultJsonSerializerSettings));

            return feeVector[level - 1];
        });

        public Operation<BitcoinAddress> GetUpgradeTransactionReceiver(long id)
        => _authorizer.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            var bl = _query.GetBitLevelById(id);
            var trnx = _query.GetBlockChainTransaction(bl.DonationId ?? 0);
            return _query.GetBitcoinAddressById(trnx.ReceiverId);
        });

        public Operation<BlockChainTransaction> GetCurrentUpgradeTransaction()
        => _authorizer.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            var bl = _query.CurrentBitLevel(UserContext.CurrentUser());
            return _query.GetBlockChainTransaction(bl.DonationId ?? 0);
        });

        public Operation<IEnumerable<BitcoinAddress>> GetAllBitcoinAddresses()
        => _authorizer.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            return _query.GetAllBitcoinAddresses(UserContext.CurrentUser())
                         .OrderByDescending(_bca => _bca.CreatedOn)
                         .AsEnumerable();
        });

        public Operation<BitcoinAddress> AddBitcoindAddress(BitcoinAddress address)
        => _authorizer.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            if (address == null || address.Id != 0) throw new Exception("Invalid bit address given");
            else return address.With(new {OwnerId = UserContext.CurrentUser().UserId }).Validate()
            .Then(opr =>
            {
                address.IsVerified = false;
                address.IsActive = false;

                return _pcommand.Add(address);
            });
        });

        public Operation<BitcoinAddress> ActivateAddress(long bitcoinAddressId)
        => _authorizer.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            var address = _query.GetBitcoinAddressById(bitcoinAddressId);
            if (address == null || address.OwnerId != UserContext.CurrentUser().UserId || !address.IsVerified) throw new Exception("Invalid bitcoin address");
            else if(!address.IsActive)
            {
                _query.GetActiveBitcoinAddress(UserContext.CurrentUser())
                      .Pipe(_bca => _bca != null ? DeactivateAddress(_bca.Id) : Operation.FromValue<BitcoinAddress>(null))
                      .Then(opr =>
                      {
                          address.IsActive = true;
                          return _pcommand.Update(address);
                      })
                      .Resolve();
            }

            return address;
        });

        public Operation<BitcoinAddress> DeactivateAddress(long bitcoinAddressId)
        => _authorizer.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            var address = _query.GetBitcoinAddressById(bitcoinAddressId);
            if (address == null || address.OwnerId != UserContext.CurrentUser().UserId) throw new Exception("Invalid bitcoin address");
            else if (address.IsActive)
            {
                address.IsActive = false;
                _pcommand.Update(address);
            }

            return address;
        });

        public Operation<ReferralNode> GetUserRef(long userId)
        => _authorizer.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            return _refQuery.
        });

        public Operation<BitcoinAddress> VerifyAddress(long bitcoinAddressId)
        => _authorizer.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            var address = _query.GetBitcoinAddressById(bitcoinAddressId)
                                .ThrowIfNull("Invalid bitcoin id specified");

            return _blockChain.VerifyBitcoinAddress(address)
                              .Then(opr => _pcommand.Update(opr.Result.With(new { IsVerified = true })));
        });

        public Operation<BitcoinAddress> GetActiveBitcoinAddress()
        => _authorizer.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            return _query.GetActiveBitcoinAddress(UserContext.CurrentUser());
        });


        private decimal GetUpgradeAmount(int level)
            => _settingsManager.GetSetting(Constants.Settings_UpgradeFeeVector)
                               .Resolve()
                               .ParseData(_d => JsonConvert.DeserializeObject<decimal[]>(_d))
                               [level-1];


        private bool IsSameTransaction(BlockChainTransaction tnx1, BlockChainTransaction tnx2)
        {
            if (tnx1 == null || tnx2 == null) return false;
            return tnx1.Amount == tnx2.Amount &&
                   (tnx1.Receiver?.Equals(tnx2.Receiver) ?? false) &&
                   (tnx1.Sender?.Equals(tnx2.Sender) ?? false);
        }

        private User NextUpgradeBeneficiary(User lastBeneficiary, int nextLvel, int nextCycle)
        => _query.Uplines(lastBeneficiary)
                 .FirstOrDefault(_rn =>
                 {
                     var bl = _query.CurrentBitLevel(_rn.User);
                     if (bl.Cycle < nextCycle ||
                        bl.Cycle == nextCycle && bl.Level <= nextLvel)
                     {
                         bl.SkipCount++;
                         _pcommand.Update(bl);
                 
                         return false;
                     }
                     else return true;
                 })?
                 .User;
    }
}
