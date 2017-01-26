using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Axis.Luna;
using BitDiamond.Core.Domain;
using Axis.Pollux.RBAC.Services;

using static Axis.Luna.Extensions.ExceptionExtensions;
using BitDiamond.Core.Utils;
using BitDiamond.Core.Services.Query;
using BitDiamond.Core.Services.Command;
using Axis.Luna.Extensions;

namespace BitDiamond.Core.Services.Services
{
    public class ContextVerifier : IContextVerifier
    {
        private IUserAuthorization _authorizer = null;
        private IContextVerifierQuery _query = null;
        private IPersistenceCommand _pcommand = null;

        public IUserContext UserContext { get; private set; }

        public ContextVerifier(IUserContext userContext, IUserAuthorization authorizer, IContextVerifierQuery query, IPersistenceCommand pcommand)
        {
            ThrowNullArguments(() => authorizer,
                               () => userContext,
                               () => query, 
                               () => pcommand);

            _authorizer = authorizer;
            _query = query;
            _pcommand = pcommand;
            UserContext = userContext;
        }

        public Operation<ContextVerification> CreateVerificationObject(string userId, string verificationContext, DateTime? expiryDate)
            => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(), () =>
            {
                var user = _query.GetUserById(userId).ThrowIfNull("user not found");
                return new ContextVerification().UsingValue(_cv =>
                {
                    _cv.Context = verificationContext;
                    _cv.ExpiresOn = expiryDate ?? (DateTime.Now + Constants.DefaultContextVerificationExpirationTime);
                    _cv.UserId = userId;
                    _cv.VerificationToken = GenerateToken();
                    _cv.Verified = false;


                });
            });

        public Operation VerifyContext(string userId, string verificationContext, string token)
            => FeatureAccess.Guard(UserContext, () =>
            {
                var cvstore = DataContext.Store<ContextVerification>();
                cvstore.Query
                       .Where(_cv => _cv.UserId == userId)
                       .Where(_cv => _cv.Context == verificationContext)
                       .Where(_cv => _cv.VerificationToken == token)
                       .Where(_cv => _cv.Verified == false)
                       .FirstOrDefault()
                       .ThrowIfNull("verification token is invalid")
                       .Do(_cv =>
                       {
                           _cv.Verified = true;
                           cvstore.Modify(_cv, true);
                       });
            });

        private string GenerateToken() => RandomAlphaNumericGenerator.RandomAlphaNumeric(50);
    }
}
