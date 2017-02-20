using Axis.Jupiter;
using Axis.Jupiter.Europa;
using Axis.Jupiter.Kore.Command;
using Axis.Luna.Extensions;
using Axis.Pollux.Authentication;
using Axis.Pollux.Authentication.OAModule;
using Axis.Pollux.Authentication.Service;
using Axis.Pollux.CoreAuthentication;
using Axis.Pollux.CoreAuthentication.Services;
using Axis.Pollux.Identity.OAModule;
using Axis.Pollux.Identity.Principal;
using Axis.Pollux.RBAC.Auth;
using Axis.Pollux.RBAC.OAModule;
using Axis.Pollux.RBAC.Services;
using Axis.Pollux.RoleAuthorization.Services;
using BitDiamond.Core.Models;
using BitDiamond.Core.Services;
using BitDiamond.Core.Services.Services;
using BitDiamond.Data.EF;
using BitDiamond.Data.EF.Command;
using BitDiamond.Web.Infrastructure.Security;
using BitDiamond.Web.Infrastructure.Services;
using Microsoft.Owin.Security.OAuth;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace BitDiamond.Web.Infrastructure.DI
{
    public static class DIRegistrations
    {
        public static void RegisterTypes(Container c)
        {
            var coreAssembly = typeof(BaseModel<>).Assembly;

            //register the container
            c.Register(() => c, Lifestyle.Singleton);


            #region infrastructure service registration

            //authorization server is a special case where it is created in the singleton scope, but relies on some services that are registered in ScopedLifeStyles...
            //thus we explicitly create the instance of the authorization server.
            c.Register<IOAuthAuthorizationServerProvider>(() => c.GetInstance<AuthorizationServer>());
            c.Register(() =>
            {
                var europa = new EuropaContext(c.GetInstance<ContextConfiguration<EuropaContext>>());
                var credentialAuthenticator = new CredentialAuthentication(europa, new DefaultHasher());
                return new AuthorizationServer(credentialAuthenticator, europa);
            }, Lifestyle.Singleton);

            //bearer authentication provider is a special case where it is created in the singleton scope, but relies on some services that are registered in ScopedLifeStyles...
            //thus we explicitly create the instance of the authentication provider.
            c.Register<IOAuthBearerAuthenticationProvider>(() => c.GetInstance<BearerAuthenticationProvider>());
            c.Register(() =>
            {
                var europa = new EuropaContext(c.GetInstance<ContextConfiguration<EuropaContext>>());
                return new BearerAuthenticationProvider(europa);
            }, Lifestyle.Singleton);


            c.Register<ICredentialHasher, DefaultHasher>(Lifestyle.Scoped);            
            c.Register<OwinContextProvider, OwinContextProvider>(Lifestyle.Scoped);            
            c.Register<IBlobStore, FileSystemBlobStore>(Lifestyle.Scoped);
            c.Register<IEmailPush, ElasticMailPushService>(Lifestyle.Singleton);
            c.Register<IAppUrlProvider, UrlProvider>(Lifestyle.Scoped);
            c.Register<IPersistenceCommands, SimplePersistenceCommands>(Lifestyle.Scoped);
            c.Register<IUserContext, UserContext>(Lifestyle.Scoped);
            #endregion


            #region domain/domain-service registration

            #region Jupiter

            //shared context configuration.
            var config = new ContextConfiguration<EuropaContext>()
                .WithConnection(ConfigurationManager.ConnectionStrings["EuropaContext"].ConnectionString)
                .WithEFConfiguraton(_efc =>
                {
                    _efc.LazyLoadingEnabled = false;
                    _efc.ProxyCreationEnabled = false;
                })
                .WithInitializer(new System.Data.Entity.DropCreateDatabaseIfModelChanges<EuropaContext>())
                .UsingModule(new IdentityAccessModuleConfig())
                .UsingModule(new AuthenticationAccessModuleConfig())
                .UsingModule(new RBACAccessModuleConfig())
                .UsingModule(new BitDiamondModuleConfig());
            c.Register(() => config, Lifestyle.Singleton);

            //scoped europa context
            c.Register<IDataContext, EuropaContext>(Lifestyle.Scoped);

            #endregion

            #region queries
            c.Register<Core.Services.Query.IAccountQuery, Data.EF.Query.AccountQuery>(Lifestyle.Scoped);
            c.Register<Core.Services.Query.IBitLevelQuery, Data.EF.Query.BitLevelQuery>(Lifestyle.Scoped);
            c.Register<Core.Services.Query.IContextVerifierQuery, Data.EF.Query.ContextVerifierQuery>(Lifestyle.Scoped);
            c.Register<Core.Services.Query.IReferralQuery, Data.EF.Query.ReferralQuery>(Lifestyle.Scoped);
            c.Register<Core.Services.Query.ISettingsQuery, Data.EF.Query.SettingsQuery>(Lifestyle.Scoped);
            c.Register<Core.Services.Query.IUserContextQuery, Data.EF.Query.UserContextQuery>(Lifestyle.Scoped);
            c.Register<Core.Services.Query.IUserNotifierQuery, Data.EF.Query.UserNotifierQuery>(Lifestyle.Scoped);
            #endregion

            #region Axis.Pollux.Identity

            #endregion

            #region Axis.Pollux.Authentication

            c.Register<ICredentialAuthentication, CredentialAuthentication>(Lifestyle.Scoped);
            #endregion

            #region Axis.Pollux.RBAC

            c.Register<IUserAuthorization>(() =>
            {
                var europa = new EuropaContext(c.GetInstance<ContextConfiguration<EuropaContext>>());
                return new CachableRoleAuthority(europa);
            }, Lifestyle.Singleton);
            #endregion

            #region BitDiamond.Core.Models/BitDiamond.Core.Services

            c.Register<IAccountManager, AccountManager>(Lifestyle.Scoped);
            c.Register<IBitLevelManager, BitLevelManager>(Lifestyle.Scoped);
            c.Register<IBlockChainService, BlockChainService>(Lifestyle.Scoped);
            c.Register<IContextVerifier, ContextVerifier>(Lifestyle.Scoped);
            c.Register<IReferralManager, ReferralManager>(Lifestyle.Scoped);
            c.Register<ISettingsManager, SettingsManager>(Lifestyle.Scoped);
            c.Register<IUserNotifier, UserNotifier>(Lifestyle.Scoped);
            #endregion
            
            #endregion


            #region webapi controller registration
            var wapict = typeof(ApiController);
            typeof(DIRegistrations).Assembly.GetTypes()
                                   .Where(_t => _t.BaseTypes().Contains(wapict))
                                   .ForAll((_cnt, _t) => c.Register(_t, _t, Lifestyle.Scoped));
            #endregion

            #region mvc controller registration
            var webct = typeof(Controller);
            typeof(DIRegistrations).Assembly.GetTypes()
                                   .Where(_t => _t.BaseTypes().Contains(webct))
                                   .ForAll((_cnt, _t) => c.Register(_t, _t, Lifestyle.Scoped));
            #endregion

        }
    }
}