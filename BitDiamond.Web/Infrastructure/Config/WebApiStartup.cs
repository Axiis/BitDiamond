using Axis.Luna.Extensions;
using BitDiamond.Web.Infrastructure.DI;
using BitDiamond.Web.Infrastructure.Utils;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using SimpleInjector.Integration.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

[assembly: OwinStartup(typeof(BitDiamond.Web.Infrastructure.Config.WebApiStartup))]

namespace BitDiamond.Web.Infrastructure.Config
{
    public class WebApiStartup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureDI(app);
            ConfigureAuth(app);
            ConfigureWebApi(app);
        }

        private void ConfigureDI(IAppBuilder app)
        {
            app.UseSimpleInjectorResolver(new WebApiRequestLifestyle(), DIRegistrations.RegisterTypes);
        }

        private void ConfigureAuth(IAppBuilder app)
        {
            app.GetSimpleInjectorResolver().ResolutionScope().UsingValue(resolver =>
            {
                app.Properties["$_AuthorizationResolutionScoppe"] = resolver; //<-- so it doesnt get garbage collected

                var oauthAuthorizeOptions = new OAuthAuthorizationServerOptions
                {
                    //AuthorizeEndpointPath = new PathString(OAuthPaths.CredentialAuthorizationPath),
                    TokenEndpointPath = new PathString(WebConstants.OAuthPath_TokenPath),
                    ApplicationCanDisplayErrors = true,
                    AccessTokenExpireTimeSpan = WebConstants.Misc_TokenValidityDuration,
                    AuthenticationType = OAuthDefaults.AuthenticationType,
                    AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active,
                    //AuthorizationCodeProvider = ...,

#if DEBUG
                    AllowInsecureHttp = true,
#endif

                    // Authorization server provider which controls the lifecycle of Authorization Server
                    Provider = resolver.Resolve<IOAuthAuthorizationServerProvider>()
                };

                //app.UseOAuthBearerTokens(oauthAuthorizeOptions);
                app.UseOAuthAuthorizationServer(oauthAuthorizeOptions);
                app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions
                {
                    Provider = new OAuthBearerAuthenticationProvider
                    {
                        OnRequestToken = context => Task.Run(() =>
                        {

                        }), 

                        OnValidateIdentity = context => Task.Run(() =>
                        {

                        })
                    }
                });

                //app.UseCors(CorsOptions.AllowAll); //<-- will configure this appropriately when it is needed
            });

        }

        private void ConfigureWebApi(IAppBuilder app)
        {

        }

    }
}