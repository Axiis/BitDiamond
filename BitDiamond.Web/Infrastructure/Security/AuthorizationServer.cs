using Axis.Jupiter;
using Axis.Luna;
using Axis.Luna.Extensions;
using Axis.Pollux.Authentication;
using Axis.Pollux.Authentication.Service;
using Axis.Pollux.Identity.Principal;
using BitDiamond.Core.Models;
using BitDiamond.Web.Infrastructure.Utils;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UAParser;
using static Axis.Luna.Extensions.ExceptionExtensions;

namespace BitDiamond.Web.Infrastructure.Security
{
    public class AuthorizationServer : OAuthAuthorizationServerProvider, IDisposable//, IAuthenticationTokenProvider
    {
        private WeakCache _cache = null;
        private Parser Parser = Parser.GetDefault();

        public AuthorizationServer(WeakCache cache)
        {
            ThrowNullArguments(() => cache);
            
            _cache = cache;
        }

        #region OAuthAuthrizationServerProvider
        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        => Task.Run(() =>
        {
            var _credentialAuthority = context.OwinContext.GetPerRequestValue<ICredentialAuthentication>(nameof(ICredentialAuthentication));
            var _dataContext = context.OwinContext.GetPerRequestValue<IDataContext>(nameof(IDataContext));

            //delete old logons if they exist
            Operation.Try(() =>
            {
                var oldToken = context.Request.Headers.GetValues(WebConstants.OAuthCustomHeaders_OldToken)?.FirstOrDefault() ?? null;
                if(oldToken != null)
                {
                    var logon = _cache.GetOrRefresh<UserLogon>(oldToken);
                    if (logon != null)
                    {
                        logon.Invalidated = true;
                        _dataContext.Store<UserLogon>().Modify(logon, true);
                        _cache.Invalidate(oldToken);
                    }
                }
            })

            .Then(opr =>
            {
                _dataContext.Store<User>().Query
                    .Where(_u => _u.EntityId == context.UserName)
                    .FirstOrDefault()
                    .ThrowIfNull("invalid user credential")
                    .ThrowIf(_u => _u.Status != (int)AccountStatus.Active, "inactive user account");
            })

            //authenticate the credential authority
            .Then(opr => _credentialAuthority.VerifyCredential(new Credential
            {
                OwnerId = context.UserName,
                Metadata = CredentialMetadata.Password,
                Value = Encoding.UTF8.GetBytes(context.Password)
            }))

            //aggregate the claims that makeup the token
            .Then(opr =>
            {
                var identity = new ClaimsIdentity(context.Options.AuthenticationType);

                identity.AddClaim(new Claim("user-name", context.UserName));
                identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));

                context.Validated(new Microsoft.Owin.Security.AuthenticationTicket(identity, null));
            })

            //if any of the above failed...
            .Error(opr =>
            {
                context.SetError("invalid_grant", opr.Message);
                context.Rejected();
            });
        });

        public override Task TokenEndpointResponse(OAuthTokenEndpointResponseContext context)
        => Task.Run(() =>
        {
            var _credentialAuthority = context.OwinContext.GetPerRequestValue<ICredentialAuthentication>(nameof(ICredentialAuthentication));
            var _dataContext = context.OwinContext.GetPerRequestValue<IDataContext>(nameof(IDataContext));

            _cache.GetOrAdd(context.AccessToken, _token =>
            {
                var agent = Parser.Parse(context.Request.Headers.Get("User-Agent"));

                var _l = _dataContext.Store<UserLogon>()
                    .QueryWith(_ul => _ul.User)
                    .Where(_ul => _ul.User.EntityId == context.Identity.Name)
                    .Where(_ul => _ul.OwinToken == _token) //get the bearer token from the header
                    .FirstOrDefault();

                if (_l != null) return _l;
                else
                {
                    _l = new UserLogon
                    {
                        UserId = context.Identity.Name,
                        Client = new Core.Models.UserAgent
                        {
                            OS = agent.OS.Family,
                            OSVersion = $"{agent.OS.Major}.{agent.OS.Minor}",

                            Browser = agent.UserAgent.Family,
                            BrowserVersion = $"{agent.UserAgent.Major}.{agent.UserAgent.Minor}",

                            Device = $"{agent.Device.Family}"
                        },
                        OwinToken = _token,
                        Location = null,

                        ModifiedOn = DateTime.Now
                    };

                    _dataContext.Store<UserLogon>().Add(_l).Context.CommitChanges();

                    return _l;
                }
            });
        });

        /// <summary>
        /// For custom authentication/authorizations
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task GrantCustomExtension(OAuthGrantCustomExtensionContext context) => base.GrantCustomExtension(context);
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context) => Task.Run(() => context.Validated());

        public void Dispose()
        {
        }
        #endregion
    }
}