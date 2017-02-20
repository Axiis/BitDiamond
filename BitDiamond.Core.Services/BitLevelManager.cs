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

namespace BitDiamond.Core.Services
{
    public class BitLevelManager : IBitLevelManager, IUserContextAware
    {
        private IBitLevelQuery _query = null;
        private IPersistenceCommands _pcommand = null;
        private IUserAuthorization _authorizer = null;
        private IUserNotifier _notifier = null;
        private IBlockChainService _blockChain = null;
        private ISettingsManager _settingsManager = null;

        public IUserContext UserContext { get; private set; }

        public BitLevelManager(IUserAuthorization authorizer, IUserContext userContext, IBitLevelQuery query, 
                               IPersistenceCommands pcommand, IUserNotifier notifier, IBlockChainService blockChain,
                               ISettingsManager settingsManager)
        {
            ThrowNullArguments(() => userContext,
                               () => query,
                               () => pcommand,
                               () => notifier,
                               () => blockChain,
                               () => settingsManager);

            _query = query;
            _pcommand = pcommand;
            _authorizer = authorizer;
            _notifier = notifier;
            _blockChain = blockChain;

            UserContext = userContext;
        }

        public Operation<BitLevel> ConfirmUpgrade(string transactionHash)
            => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(), () =>
            {
                var currentUser = UserContext.CurrentUser();
                var currentLevel = _query.CurrentBitLevel(currentUser);

                var blockChainTransaction = _blockChain.GetTransactionDetails(transactionHash).Resolve();
                if (IsSameTransaction(currentLevel.Donation, blockChainTransaction))
                {
                    currentLevel.Donation.Status = blockChainTransaction.Status;
                    currentLevel.Donation.LedgerCount = blockChainTransaction.LedgerCount;
                    //if the ledger count is  < 3, queue this transaction for verification polling till the ledger
                    //count is greater than 3
                    _pcommand.Update(currentLevel);

                    //increment receiver's donnation count
                    var receiverLevel = _query.CurrentBitLevel(blockChainTransaction.Receiver.Owner);
                    receiverLevel.DonationCount++;
                    _pcommand.Update(receiverLevel);

                    return currentLevel;
                }
                else throw new Exception("invalid transaction hash");
            });

        public Operation<BitLevel> RequestUpgrade()
            => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(), () =>
            {
                var currentUser = UserContext.CurrentUser();
                var currentLevel = _query.CurrentBitLevel(currentUser);                
                var maxLevel = _settingsManager.GetSetting(Constants.Settings_MaxBitLevel)
                                               .Resolve()
                                               .ParseData<int>();

                if (currentLevel.Level == maxLevel) throw new Exception("cannot upgrade from max level");

                //get the ideal upgrade receiver
                var receiver = _query.Upline(currentUser, currentLevel.Level + 1);
                var receiverLevel = _query.CurrentBitLevel(receiver.User);
                if (receiverLevel.Donation.Status != BlockChainTransactionStatus.Valid ||
                    receiverLevel.Cycle < currentLevel.Cycle ||
                    receiverLevel.Level <= currentLevel.Level)
                {
                    //increment the skip count and persist
                    receiverLevel.SkipCount++;
                    _pcommand.Update(receiverLevel);

                    _notifier.NotifyUser(new Notification
                    {
                        Message = "Due to your delay in confirming your upgrade, your downline's donnation has skipped you.",
                        Type = NotificationType.Warning,
                        Target = receiverLevel.User
                    });

                    //find the next best receiver
                    receiverLevel = _query.GetClosestValidBeneficiary(currentUser);
                }

                return new BitLevel
                {
                    Cycle = currentLevel.Cycle,
                    Level = currentLevel.Level + 1,
                    DonationCount = 0,
                    SkipCount = 0,
                    User = currentUser,
                    Donation = new BlockChainTransaction
                    {
                        Amount = GetUpgradeAmount(currentLevel.Level + 1),
                        LedgerCount = 0,
                        Receiver = _query.GetBitcoinAddress(receiverLevel.User),
                        Sender = _query.GetBitcoinAddress(currentUser),
                        CreatedOn = DateTime.Now
                    }
                }
                .Pipe(_bl => _pcommand.Add(_bl));
            });

        public Operation<BitLevel> RecycleAccount()
            => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(), () =>
            {

                var currentUser = UserContext.CurrentUser();
                var currentLevel = _query.CurrentBitLevel(currentUser);
                var maxLevel = _settingsManager.GetSetting(Constants.Settings_MaxBitLevel)
                                               .Resolve()
                                               .ParseData<int>();

                if (currentLevel != null &&
                    currentLevel.Level < maxLevel)
                    throw new Exception($"only level {Constants.Settings_MaxBitLevel} accounts can be recycled");

                var btcAddress = _query.GetBitcoinAddress(currentUser);
                return new BitLevel
                {
                    Cycle = (currentLevel?.Cycle ?? 0) + 1,
                    Level = 0,
                    DonationCount = 0,
                    SkipCount = 0,
                    User = currentUser,
                    Donation = new BlockChainTransaction
                    {
                        Amount = 0,
                        LedgerCount = int.MaxValue,
                        CreatedOn = DateTime.Now,
                        Receiver = btcAddress,
                        Sender = btcAddress,
                        Status = BlockChainTransactionStatus.Valid
                    }
                }
                .Pipe(_bl => _pcommand.Add(_bl));
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

        private decimal GetUpgradeAmount(int level)
            => _settingsManager.GetSetting(Constants.Settings_UpgradeCostVector)
                               .Resolve()
                               .ParseData(_d => JsonConvert.DeserializeObject<decimal[]>(_d))
                               [level];

        private bool IsSameTransaction(BlockChainTransaction tnx1, BlockChainTransaction tnx2)
        {
            if (tnx1 == null || tnx2 == null) return false;
            return tnx1.Amount == tnx2.Amount &&
                   (tnx1.Receiver?.Equals(tnx2.Receiver) ?? false) &&
                   (tnx1.Sender?.Equals(tnx2.Sender) ?? false);
        }
    }
}
