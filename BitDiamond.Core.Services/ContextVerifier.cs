using System;
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
    public class ContextVerifier : IContextVerifier, IUserContextAware
    {
        private IUserAuthorization _authorizer = null;
        private IContextVerifierQuery _query = null;
        private IPersistenceCommands _pcommand = null;
        private ISettingsManager _settings = null;

        public IUserContext UserContext { get; private set; }

        public ContextVerifier(IUserContext userContext, 
                               IUserAuthorization authorizer,
                               ISettingsManager settings, 
                               IContextVerifierQuery query, 
                               IPersistenceCommands pcommand)
        {
            ThrowNullArguments(() => authorizer,
                               () => userContext,
                               () => query, 
                               () => pcommand,
                               () => settings);

            _authorizer = authorizer;
            _query = query;
            _pcommand = pcommand;
            _settings = settings;
            UserContext = userContext;
        }

        public Operation<ContextVerification> CreateVerificationObject(string userId, string verificationContext, DateTime? expiryDate)
            => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(), () =>
            {
                var user = _query.GetUserById(userId).ThrowIfNull("user not found");
                var _cv = new ContextVerification();
                var cvexpiration = _settings.GetSetting(Constants.Settings_DefaultContextVerificationExpirationTime)
                                            .Resolve()
                                            .ParseData<TimeSpan>();
                _cv.Context = verificationContext;
                _cv.ExpiresOn = expiryDate ?? (DateTime.Now + cvexpiration);
                _cv.Target = user;
                _cv.VerificationToken = GenerateToken();
                _cv.Verified = false;

                return _pcommand.Add(_cv);
            });

        public Operation VerifyContext(string userId, string verificationContext, string token)
            => _authorizer.AuthorizeAccess(UserContext.CurrentProcessPermissionProfile(), () =>
            {
                var cv = _query.GetContextVerification(userId, verificationContext, token)
                               .ThrowIf(_cv => _cv == null || _cv.Verified, "verification token is invalid");

                cv.Verified = true;
                _pcommand.Update(cv).Resolve();
            });

        private string GenerateToken() => RandomAlphaNumericGenerator.RandomAlphaNumeric(50);
    }
}
