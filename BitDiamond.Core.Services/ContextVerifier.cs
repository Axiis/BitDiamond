using System;
using System.Linq;
using Axis.Luna;
using BitDiamond.Core.Models;
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
        private IPersistenceCommands _pcommand = null;

        public IUserContext UserContext { get; private set; }

        public ContextVerifier(IUserContext userContext, IUserAuthorization authorizer, IContextVerifierQuery query, IPersistenceCommands pcommand)
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
                var _cv = new ContextVerification();
                _cv.Context = verificationContext;
                _cv.ExpiresOn = expiryDate ?? (DateTime.Now + Constants.Settings_DefaultContextVerificationExpirationTime);
                _cv.UserId = userId;
                _cv.VerificationToken = GenerateToken();
                _cv.Verified = false;

                return _pcommand.Persist(_cv);
            });

        public Operation VerifyContext(string userId, string verificationContext, string token)
            => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(), () =>
            {
                var cv = _query.GetContextVerification(userId, verificationContext, token)
                               .ThrowIf(_cv => _cv == null || _cv.Verified, "verification token is invalid");

                cv.Verified = true;
                _pcommand.Persist(cv).Resolve();
            });

        private string GenerateToken() => RandomAlphaNumericGenerator.RandomAlphaNumeric(50);
    }
}
