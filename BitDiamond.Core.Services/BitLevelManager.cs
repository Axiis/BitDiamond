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

                //get the ideal upgrade receiver
                var receiver = _query.Upline(currentLevel.Level + 1);

                while(DonationShouldSkip(receiver))
                    receiver = _query.UserRef(receiver).Referee.User;

                return new BitLevel
                {

                };
            });

        public Operation<BitLevel> LatestUserDonation()
        {
            throw new NotImplementedException();
        }

        public Operation ManualDonationVerification()
        {
            throw new NotImplementedException();
        }

        public Operation<BitLevel> UserDonationById(long id)
        {
            throw new NotImplementedException();
        }

        public Operation<IEnumerable<BitLevel>> UserDonations()
        {
            throw new NotImplementedException();
        }

        public Operation VerifyDonation(string transactionHash)
        {
            throw new NotImplementedException();
        }

        private bool DonationShouldSkip(User user)
        {

        }
    }
}
