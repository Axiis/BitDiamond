using Axis.Luna.Extensions;
using BitDiamond.Core.Utils;
using BitDiamond.Web.Infrastructure.DI;
using BitDiamond.Web.Infrastructure.Services;
using BitDiamond.Web.Infrastructure.Utils;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using SimpleInjector.Integration.WebApi;
using System.Net.Http.Formatting;
using System.Web.Http;

[assembly: OwinStartup(typeof(BitDiamond.Web.Infrastructure.Config.WebApi.WebApiStartup))]

namespace BitDiamond.Web.Infrastructure.Config.WebApi
{
    public class WebApiStartup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            ConfigureDI(app);
            ConfigureAuth(app);
            ConfigureWebApi(app, config);
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
                
                //Authorization. In this case, it comes before authentication because bearer authentication 
                //expects a token to be created already
                app.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions
                {
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
                });

                //configure bearer authentication. This creates a claims-user from the info found in the bearer token
                //user logon is also implemented here
                app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions
                {
                    Provider = resolver.Resolve<IOAuthBearerAuthenticationProvider>()
                });

                //app.UseCors(CorsOptions.AllowAll); //<-- will configure this appropriately when it is needed
            });

        }

        private void ConfigureWebApi(IAppBuilder app, HttpConfiguration config)
        {
            //change the json formatter
            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter { SerializerSettings = Constants.DefaultJsonSerializerSettings });

            //conigure dependency injection
            config.DependencyResolver = app.GetSimpleInjectorResolver();

            //enable attribute routes
            config.MapHttpAttributeRoutes();

            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            app.UseOwinContextProvider();

            //apply the configuration
            app.UseWebApi(config);
        }

    }
}