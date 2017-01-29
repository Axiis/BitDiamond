using System;
using System.Collections.Generic;
using Axis.Luna;
using BitDiamond.Core.Models;
using BitDiamond.Core.Services.Query;
using BitDiamond.Core.Services.Command;

using static Axis.Luna.Extensions.ExceptionExtensions;
using Axis.Pollux.RBAC.Services;
using BitDiamond.Core.Utils;
using Axis.Pollux.Identity.Principal;
using Axis.Luna.Extensions;

namespace BitDiamond.Core.Services.Services
{
    public class BitLevelManager : IBitLevelManager, IUserContextAware
    {

        private IBitLevelQuery _query = null;
        private IPersistenceCommand _pcommand = null;
        private IUserAuthorization _authorizer = null;
        private IUserNotifier _notifier = null;
        private IBlockChainService _blockChain = null;

        public IUserContext UserContext { get; private set; }

        public BitLevelManager(IUserAuthorization authorizer, IUserContext userContext, IBitLevelQuery query, 
                               IPersistenceCommand pcommand, IUserNotifier notifier, IBlockChainService blockChain)
        {
            ThrowNullArguments(() => userContext,
                               () => query,
                               () => pcommand,
                               () => notifier,
                               () => blockChain);

            _query = query;
            _pcommand = pcommand;
            _authorizer = authorizer;
            UserContext = userContext;
            _notifier = notifier;
            _blockChain = blockChain;
        }

        public Operation<BitLevel> ConfirmUpgrade(string transactionHash)
            => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(), () =>
            {
                var currentUser = UserContext.CurrentUser();
                var currentLevel = _query.CurrentBitLevel(currentUser);

                var blockChainTransaction = _blockChain.GetTransactionDetails(transactionHash);
                if (IsSameTransaction(currentLevel.Donation, blockChainTransaction))
                {
                    currentLevel.Donation.Status = blockChainTransaction.Status;
                    currentLevel.Donation.LedgerCount = blockChainTransaction.LedgerCount;
                    //if the ledger count is  < 3, queue this transaction for verification polling till the ledger
                    //count is greater than 3
                    _pcommand.Persist(currentLevel);

                    //increment receiver's donnation count
                    var receiverLevel = _query.CurrentBitLevel(blockChainTransaction.Reciever.Owner);
                    receiverLevel.DonationCount++;
                    _pcommand.Persist(receiverLevel);

                    return currentLevel;
                }
                else throw new Exception("invalid transaction hash");
            });

        public Operation<BitLevel> RequestUpgrade()
            => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(), () =>
            {
                var currentUser = UserContext.CurrentUser();
                var currentLevel = _query.CurrentBitLevel(currentUser);

                if (currentLevel.Level == Constants.MaxBitLevel) throw new Exception("cannot upgrade from max level");

                //get the ideal upgrade receiver
                var receiver = _query.Upline(currentLevel.Level + 1);
                var receiverLevel = _query.CurrentBitLevel(receiver.User);
                if (receiverLevel.Donation.Status != BlockChainTransactionStatus.Valid ||
                    receiverLevel.Cycle < currentLevel.Cycle ||
                    receiverLevel.Level <= currentLevel.Level)
                {
                    //increment the skip count and persist
                    receiverLevel.SkipCount++;
                    _pcommand.Persist(receiverLevel);

                    _notifier.NotifyUser(new Notification
                    {
                        Message = "Due to your delay in confirming your upgrade, your downline's donnation has skipped you.",
                        Type = NotificationType.Warning,
                        Target = receiverLevel.User
                    });

                    //find the next best receiver
                    receiverLevel = _query.GetClosestValidAncestorLevel(currentLevel.Level + 1);
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
                        Reciever = _query.GetBitcoinAddress(receiverLevel.User),
                        Sender = _query.GetBitcoinAddress(currentUser),
                        CreatedOn = DateTime.Now
                    }
                }
                .Pipe(_bl => _pcommand.Persist(_bl));
            });

        public Operation<BitLevel> RecycleAccount()
            => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(), () =>
            {

                var currentUser = UserContext.CurrentUser();
                var currentLevel = _query.CurrentBitLevel(currentUser);

                if (currentLevel != null &&
                    currentLevel.Level < Constants.MaxBitLevel)
                    throw new Exception($"only level {Constants.MaxBitLevel} accounts can be recycled");

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
                        Reciever = btcAddress,
                        Sender = btcAddress,
                        Status = BlockChainTransactionStatus.Valid
                    }
                }
                .Pipe(_bl => _pcommand.Persist(_bl));
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
        {
            switch(level)
            {
                case 1: return Constants.UpgradeCostLevel1;
                case 2: return Constants.UpgradeCostLevel2;
                case 3: return Constants.UpgradeCostLevel3;
                default: throw new Exception("invalid level");
            }
        }
        private bool IsSameTransaction(BlockChainTransaction tnx1, BlockChainTransaction tnx2)
        {
            if (tnx1 == null || tnx2 == null) return false;
            return tnx1.Amount == tnx2.Amount &&
                   (tnx1.Reciever?.Equals(tnx2.Reciever) ?? false) &&
                   (tnx1.Sender?.Equals(tnx2.Sender) ?? false);
        }
    }
}
