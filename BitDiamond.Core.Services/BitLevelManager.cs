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

        public IUserContext UserContext { get; private set; }

        public BitLevelManager(IUserAuthorization authorizer, IUserContext userContext, IBitLevelQuery query, IPersistenceCommand pcommand)
        {
            ThrowNullArguments(() => userContext,
                               () => query,
                               () => pcommand);

            _query = query;
            _pcommand = pcommand;
            _authorizer = authorizer;
            UserContext = userContext;
        }

        public Operation<BitLevel> ConfirmUpgrade(string transactionHash)
        {
        
        }

        public Operation<BitLevel> RequestUpgrade()
            => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(), () =>
            {
                var currentUser = UserContext.CurrentUser();
                var currentLevel = _query.CurrentBitLevel(currentUser);

                if (currentLevel.Level == Constants.MaxBitLevel) throw new Exception("cannot upgrade from max level");

                //get the ideal upgrade receiver
                var receiver = _query.Upline(currentLevel.Level + 1);
                var receiverLevel = _query.CurrentBitLevel(receiver.User);
                if (receiverLevel.Donation.Status != BlockChainTransactionStatus.Valid)
                {
                    //increment the skip count and persist
                    receiverLevel.SkipCount++;
                    _pcommand.Persist(receiverLevel);

                    //find the next best receiver
                    receiverLevel = _query.GetClosestValidAncestorLevel(currentLevel.Level + 1);
                }

                return new BitLevel
                {
                    Cycle = currentLevel.Cycle,
                    Donation = new BlockChainTransaction
                    {
                        Amount = GetUpgradeAmount(currentLevel.Level + 1),
                        LedgerCount = 0,
                        Reciever = _query.GetBitcoinAddress(receiverLevel.User),
                        Sender = _query.GetBitcoinAddress(currentUser),
                        CreatedOn = DateTime.Now
                    },
                    DonationCount = 0,
                    Level = currentLevel.Level + 1,
                    SkipCount = 0,
                    User = currentUser
                }
                .Pipe(_bl => _pcommand.Persist(_bl));
            });

        public Operation<BitLevel> CurrentUserLevel()
        {
            throw new NotImplementedException();
        }

        public Operation ManualDonationVerification()
        {
            throw new NotImplementedException();
        }

        public Operation<BitLevel> GetBitLevelById(long id)
        {
            throw new NotImplementedException();
        }

        public Operation<IEnumerable<BitLevel>> UserUpgradeHistory()
        {
            throw new NotImplementedException();
        }

        public Operation VerifyDonation(string transactionHash)
        {
            throw new NotImplementedException();
        }

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
    }
}
