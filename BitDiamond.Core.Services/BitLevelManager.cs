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
using Haxh;

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
            var currentLevel = _query.CurrentBitLevel(targetUser) ??
                               new BitLevel
                               {
                                   Level = maxLevel,
                                   Cycle = 0,
                                   Donation = new BlockChainTransaction { Status = BlockChainTransactionStatus.Verified }
                               };
            if (currentLevel.Donation.Status == BlockChainTransactionStatus.Unverified)
                throw new Exception("Current level donnation is still unverified");

            var myAddress = _query.GetActiveBitcoinAddress(targetUser)
                                  .ThrowIfNull("You do not have a valid block chain transaction address yet.");

            if (currentLevel.Cycle == int.MaxValue && currentLevel.Level == maxLevel)
                throw new Exception("You cannot upgrade past the maximum cycle");

            var nextLevel = (currentLevel.Level + 1) % (maxLevel + 1);
            var nextCycle = nextLevel == 0 ? currentLevel.Cycle + 1 : currentLevel.Cycle;

            var nextUpgradeBeneficiary = NextUpgradeBeneficiary(nextLevel == 0 ? targetUser : currentLevel.Donation.Receiver.Owner, nextLevel, nextCycle).ThrowIfNull("Could not find a valid beneficiary");
            var beneficiaryAddress = _query.GetActiveBitcoinAddress(nextUpgradeBeneficiary);

            var bl = new BitLevel
            {
                Level = nextLevel,
                Cycle = nextCycle,
                DonationCount = 0,
                SkipCount = 0,
                UserId = targetUser.UserId
            };
            _pcommand.Add(bl).Resolve();

            var donation = new BlockChainTransaction
            {
                Amount = nextLevel == maxLevel ? 0 : GetUpgradeAmount(nextLevel + 1),
                LedgerCount = nextLevel == maxLevel ? int.MaxValue : 0,
                CreatedOn = DateTime.Now,
                Sender = myAddress,
                Receiver = beneficiaryAddress,
                ContextType = Constants.TransactionContext_UpgradeBitLevel,
                ContextId = bl.Id.ToString(),
                Status = nextLevel == maxLevel ?
                         BlockChainTransactionStatus.Verified : 
                         BlockChainTransactionStatus.Unverified
            };
            _pcommand.Add(donation).Resolve();

            bl.DonationId = donation.Id;
            _pcommand.Update(bl);

            bl.Donation = donation;

            //notify user
            _notifier.NotifyUser(new Notification
            {
                Type = NotificationType.Success,
                TargetId = targetUser.UserId,
                Title = "Congratulations!",
                Message = $"Well done, {targetUser.UserId}!! You are now <strong>proudly</strong> at Cycle-{nextCycle} / Level-{nextLevel}, no easy feat!<br/>"+
                @"Now you can receive even more donations from your downlines. 
                  <p>Remember though, that this is a race to the <strong class='text-primary'>top</strong>, and as such
                  you may miss the donations of any of your downlines who upgrades to levels higher than yours. So dont waste much time here, Upgrade as soon as you can!</p>"
            })
            .Resolve();

            return bl;
        });

        public Operation<BlockChainTransaction> VerifyAndSaveTransactionHash(string transactionHash)
        => _authorizer.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            var currentUser = UserContext.CurrentUser();
            var currentLevel = _query.CurrentBitLevel(currentUser);

            _query.GetTransactionWithHash(transactionHash)
                  .ThrowIfNotNull(new Exception("A transaction already exists in the system with the supplied hash"));

            _blockChain.VerifyTransaction(transactionHash, currentLevel)
                       .Resolve();             

            currentLevel.Donation.TransactionHash = transactionHash;
            return _pcommand.Update(currentLevel.Donation);
        });


        public Operation<BitLevel> Promote(string userRef, int units, string securityHash)
        => _authorizer.AuthorizeAccess(this.PermissionProfile(UserContext.CurrentUser()), () =>
        {
            //verify the securityHash
            Haxher.IsValidHash(securityHash).ThrowIf(_v => !_v, "Access Denied");

            //do promotion logic here
            var @ref = _refQuery.GetReferalNode(userRef);
            var targetUser = new User { EntityId = @ref.UserId };
            var currentLevel = _query.CurrentBitLevel(targetUser);
            var newLevel = BitCycle.Create(currentLevel.Cycle, currentLevel.Level).Increment(units);
            var beneficiary = NextUpgradeBeneficiary(new User { UserId = currentLevel.Donation.Receiver.OwnerId }, newLevel.Level, newLevel.Cycle)
                .ThrowIfNull("could not find a suitable beneficiary");

            var beneficiaryAddress = _query.GetActiveBitcoinAddress(beneficiary);
            var targetUserAddress = _query.GetActiveBitcoinAddress(targetUser);

            //close off the old level
            currentLevel.Donation.Amount = 0;
            currentLevel.Donation.Status = BlockChainTransactionStatus.Verified;
            _pcommand.Update(currentLevel.Donation);

            var bl = new BitLevel
            {
                Level = newLevel.Level,
                Cycle = newLevel.Cycle,
                DonationCount = 0,
                SkipCount = 0,
                UserId = @ref.UserId
            };
            _pcommand.Add(bl).Resolve();

            var maxLevelSettings = _settingsManager.GetSetting(Constants.Settings_MaxBitLevel).Resolve();
            var maxLevel = (int)maxLevelSettings.ParseData<long>();

            var donation = new BlockChainTransaction
            {
                Amount = newLevel.Level == maxLevel ? 0 : GetUpgradeAmount(newLevel.Level + 1),
                LedgerCount = newLevel.Level == maxLevel ? int.MaxValue : 0,
                CreatedOn = DateTime.Now,
                Sender = targetUserAddress,
                Receiver = beneficiaryAddress,
                ContextType = Constants.TransactionContext_UpgradeBitLevel,
                ContextId = bl.Id.ToString(),
                Status = newLevel.Level == maxLevel ?
                         BlockChainTransactionStatus.Verified :
                         BlockChainTransactionStatus.Unverified
            };
            _pcommand.Add(donation).Resolve();

            bl.DonationId = donation.Id;
            _pcommand.Update(bl);

            bl.Donation = donation;

            //notify user
            _notifier.NotifyUser(new Notification
            {
                Type = NotificationType.Success,
                TargetId = targetUser.UserId,
                Title = "Congratulations!",
                Message = $"Well done, {targetUser.UserId}!! You are now <strong>proudly</strong> at Cycle-{newLevel.Cycle} / Level-{newLevel.Level}, no easy feat!<br/>" +
                @"Now you can receive even more donations from your downlines. 
                  <p>Remember though, that this is a race to the <strong class='text-primary'>top</strong>, and as such
                  you may miss the donations of any of your downlines who upgrades to levels higher than yours. So dont waste much time here, Upgrade as soon as you can!</p>"
            })
            .Resolve();

            return bl;
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


        public Operation<BitLevel> CurrentUserLevel()
            => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(),() =>
            {
                var level =_query.CurrentBitLevel(UserContext.CurrentUser());
                return level;
            });

        public Operation<BitLevel> GetBitLevelById(long id)
            => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(),
                                           () => _query.GetBitLevelById(id));

        public Operation<IEnumerable<BitLevel>> UserUpgradeHistory()
        => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(),
                                       () => _query.GetBitLevelHistory(UserContext.CurrentUser()));

        public Operation<decimal> GetUpgradeFee(int level)
        => _authorizer.AuthorizeAccess(UserContext.CurrentPPP(),
                                       () => GetUpgradeAmount(level));

        public Operation<BlockChainTransaction> GetCurrentUpgradeTransaction()
        => _authorizer.AuthorizeAccess(UserContext.CurrentPPP(), 
                                       () => _query.CurrentBitLevel(UserContext.CurrentUser()).Donation);

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

        public Operation<ReferralNode> GetUserRef(string userId)
        => _authorizer.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            return _refQuery.GetUserReferalNode(_query.GetUser(userId));
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

        public Operation<SequencePage<BitLevel>> PagedUserUpgradeHistory(int pageSize, long pageIndex = 0)
        => _authorizer.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            return _query.GetPagedBitLevelHistory(UserContext.CurrentUser(), pageSize, pageIndex);
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
                   (tnx1.ReceiverId.Equals(tnx2.ReceiverId)) &&
                   (tnx1.SenderId.Equals(tnx2.SenderId));
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

                         //notify user
                         _notifier.NotifyUser(new Notification
                         {
                             Type = NotificationType.Info,
                             TargetId = bl.UserId,
                             Title = "Missed donation",
                             Message = @"
<strong>Ouch!</strong> You just missed a donation...
<p>
    <span class='text-muted'>Your level</span><br />
    {0}
</p>
<p>
    <span class='text-muted'>Downline</span><br />
    {1}
</p>
<p>
    <span class='text-muted'>Downline Level</span><br />
    {2}
</p>
".ResolveParameters(new BitCycle { Level = nextLvel, Cycle = nextCycle }, _rn.UserId, new BitCycle { Level = bl.Level, Cycle = bl.Cycle })
                         })
                         .Resolve();

                         return false;
                     }
                     else return true;
                 })?.User
                 ?? lastBeneficiary;
    }
}
