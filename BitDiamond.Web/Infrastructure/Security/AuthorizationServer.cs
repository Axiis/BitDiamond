using Axis.Jupiter;
using Axis.Luna;
using Axis.Luna.Extensions;
using Axis.Pollux.Authentication;
using Axis.Pollux.Authentication.Service;
using Axis.Pollux.Identity.Principal;
using BitDiamond.Core.Models;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using static Axis.Luna.Extensions.ExceptionExtensions;

namespace BitDiamond.Web.Infrastructure.Security
{
    public class AuthorizationServer : OAuthAuthorizationServerProvider, IDisposable//, IAuthenticationTokenProvider
    {
        private ICredentialAuthentication _credentialAuthority = null;
        private IDataContext _dataContext = null;

        public AuthorizationServer(ICredentialAuthentication credentialAuthority, IDataContext dataContext)
        {
            ThrowNullArguments(() => credentialAuthority, () => dataContext);

            _credentialAuthority = credentialAuthority;
            _dataContext = dataContext;
        }

        #region OAuthAuthrizationServerProvider
        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        => Task.Run(() =>
        {
            Operation.Try(() =>
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
                Value = Encoding.Unicode.GetBytes(context.Password)
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