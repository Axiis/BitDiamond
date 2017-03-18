using Axis.Jupiter;
using Axis.Jupiter.Europa;
using Axis.Luna;
using Axis.Luna.Extensions;
using Axis.Pollux.Authentication.Service;
using Axis.Pollux.CoreAuthentication;
using Axis.Pollux.CoreAuthentication.Services;
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

            ConfigureAuth(app);
            ConfigureRequestDI(app);
            ConfigureWebApi(app, config);
        }

        private void ConfigureAuth(IAppBuilder app)
        {
            ///add the run-per-request generators for the services needed for authentication and authorization
            //1. Europa Context
            app.RunPerRequest(nameof(IDataContext), cxt => new EuropaContext(WebConstants.Misc_UniversalEuropaConfig))

            //2. Credential Authentication
               .RunPerRequest(nameof(ICredentialAuthentication), cxt => new CredentialAuthentication(cxt.GetPerRequestValue<IDataContext>(nameof(IDataContext)), new DefaultHasher()));


            //weak cache used for logon processing
            var cache = new WeakCache();

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

//#if DEBUG
                AllowInsecureHttp = true,
//#endif

                // Authorization server provider which controls the lifecycle of Authorization Server
                Provider = new Security.AuthorizationServer(cache)
            });

            //configure bearer authentication. This creates a claims-user from the info found in the bearer token
            //user logon is also implemented here
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions
            {
                Provider = new Security.BearerAuthenticationProvider(cache)
            });

            //app.UseCors(CorsOptions.AllowAll); //<-- will configure this appropriately when it is needed

        }

        private void ConfigureRequestDI(IAppBuilder app)
        {
            app.UseSimpleInjectorResolver(new WebApiRequestLifestyle(), DIRegistrations.RegisterTypes);
        }

        private void ConfigureWebApi(IAppBuilder app, HttpConfiguration config)
        {
            //change the json formatter
            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter { SerializerSettings = Constants.Misc_DefaultJsonSerializerSettings });

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