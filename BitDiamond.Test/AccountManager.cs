using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Axis.Jupiter.Europa;
using Axis.Pollux.Identity.OAModule;
using Axis.Pollux.Authentication.OAModule;
using Axis.Pollux.RBAC.OAModule;
using BitDiamond.Data.EF;
using BitDiamond.Data.EF.Query;
using BitDiamond.Web.Infrastructure.DI;
using SimpleInjector.Extensions.ExecutionContextScoping;
using BitDiamond.Core.Services;
using Axis.Pollux.Identity.Principal;
using System.Web.Mvc;
using System.Web.Http;
using BitDiamond.Core.Services.Services;
using SimpleInjector;
using System.Linq;
using Axis.Luna.Extensions;
using Axis.Pollux.RoleAuthorization.Services;
using Axis.Pollux.RBAC.Services;
using Axis.Pollux.Authentication.Service;
using Axis.Pollux.CoreAuthentication.Services;
using BitDiamond.Core.Models;
using Axis.Luna;
using BitDiamond.Web.Infrastructure.Services;
using Axis.Pollux.CoreAuthentication;
using Axis.Pollux.Authentication;
using Axis.Jupiter.Kore.Command;
using BitDiamond.Data.EF.Command;
using BitDiamond.Web.Infrastructure.Utils;
using Axis.Jupiter;
using Castle.DynamicProxy;

namespace BitDiamond.Test
{
    /// <summary>
    /// Summary description for AccountManager
    /// </summary>
    [TestClass]
    public class AccountManager
    {
        public static ContextConfiguration<EuropaContext> Config;
        static AccountManager()
        {
            Config = new ContextConfiguration<EuropaContext>()
                .WithConnection("server=(local);database=BitDiamondDb;user id=sa;password=developer;Max Pool Size=1000;Min Pool Size=10;pooling=true;multipleactiveresultsets=True;")
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
        }

        [TestMethod]
        public void BioData()
        {
            var start = DateTime.Now;
            var resolver = new SimpleInjectorOwinResolutionContext(new ExecutionContextScopeLifestyle(), RegisterTypes);
            Console.WriteLine($"Registered dependencies in: {DateTime.Now - start}\n\n\n");

            using (var scope = resolver.NewResolutionScope())
            {
                start = DateTime.Now;
                var accManager = scope.Resolve<IAccountManager>();
                Console.WriteLine($"IAccountManager resolved in: {DateTime.Now - start}");

                start = DateTime.Now;
                var bio = accManager.GetBioData();
                Console.WriteLine($"IAccountManager.GetBioData({bio.Succeeded}) executed in: {DateTime.Now - start}\n\n");
            }

            using (var scope = resolver.NewResolutionScope())
            {
                start = DateTime.Now;
                var accManager = scope.Resolve<IAccountManager>();
                Console.WriteLine($"IAccountManager resolved in: {DateTime.Now - start}");

                start = DateTime.Now;
                var bio = accManager.GetBioData();
                Console.WriteLine($"IAccountManager.GetBioData({bio.Succeeded}) executed in: {DateTime.Now - start}\n\n");
            }

            using (var scope = resolver.NewResolutionScope())
            {
                start = DateTime.Now;
                var accManager = scope.Resolve<IAccountManager>();
                Console.WriteLine($"IAccountManager resolved in: {DateTime.Now - start}");

                start = DateTime.Now;
                var bio = accManager.GetBioData();
                Console.WriteLine($"IAccountManager.GetBioData({bio.Succeeded}) executed in: {DateTime.Now - start}\n\n");
            }

            using (var scope = resolver.NewResolutionScope())
            {
                start = DateTime.Now;
                var accManager = scope.Resolve<IAccountManager>();
                Console.WriteLine($"IAccountManager resolved in: {DateTime.Now - start}");

                start = DateTime.Now;
                var bio = accManager.GetBioData();
                Console.WriteLine($"IAccountManager.GetBioData({bio.Succeeded}) executed in: {DateTime.Now - start}\n\n");
            }

            using (var scope = resolver.NewResolutionScope())
            {
                start = DateTime.Now;
                var accManager = scope.Resolve<IAccountManager>();
                Console.WriteLine($"IAccountManager resolved in: {DateTime.Now - start}");

                start = DateTime.Now;
                var bio = accManager.GetBioData();
                Console.WriteLine($"IAccountManager.GetBioData({bio.Succeeded}) executed in: {DateTime.Now - start}\n\n");
            }
        }

        [TestMethod]
        public void UserData()
        {
            var start = DateTime.Now;
            var resolver = new SimpleInjectorOwinResolutionContext(new ExecutionContextScopeLifestyle(), RegisterTypes);
            Console.WriteLine($"Registered dependencies in: {DateTime.Now - start}\n\n\n");

            using (var scope = resolver.NewResolutionScope())
            {
                start = DateTime.Now;
                var accManager = scope.Resolve<IAccountManager>();
                Console.WriteLine($"IAccountManager resolved in: {DateTime.Now - start}");

                start = DateTime.Now;
                var udata = accManager.GetUserData();
                Console.WriteLine($"IAccountManager.GetUserData({udata.Succeeded}) executed in: {DateTime.Now - start}\n\n");
            }

            using (var scope = resolver.NewResolutionScope())
            {
                start = DateTime.Now;
                var accManager = scope.Resolve<IAccountManager>();
                Console.WriteLine($"IAccountManager resolved in: {DateTime.Now - start}");

                start = DateTime.Now;
                var udata = accManager.GetUserData();
                Console.WriteLine($"IAccountManager.GetUserData({udata.Succeeded}) executed in: {DateTime.Now - start}\n\n");
            }

            using (var scope = resolver.NewResolutionScope())
            {
                start = DateTime.Now;
                var accManager = scope.Resolve<IAccountManager>();
                Console.WriteLine($"IAccountManager resolved in: {DateTime.Now - start}");

                start = DateTime.Now;
                var udata = accManager.GetUserData();
                Console.WriteLine($"IAccountManager.GetUserData({udata.Succeeded}) executed in: {DateTime.Now - start}\n\n");
            }

            using (var scope = resolver.NewResolutionScope())
            {
                start = DateTime.Now;
                var accManager = scope.Resolve<IAccountManager>();
                Console.WriteLine($"IAccountManager resolved in: {DateTime.Now - start}");

                start = DateTime.Now;
                var udata = accManager.GetUserData();
                Console.WriteLine($"IAccountManager.GetUserData({udata.Succeeded}) executed in: {DateTime.Now - start}\n\n");
            }

            using (var scope = resolver.NewResolutionScope())
            {
                start = DateTime.Now;
                var accManager = scope.Resolve<IAccountManager>();
                Console.WriteLine($"IAccountManager resolved in: {DateTime.Now - start}");

                start = DateTime.Now;
                var udata = accManager.GetUserData();
                Console.WriteLine($"IAccountManager.GetUserData({udata.Succeeded}) executed in: {DateTime.Now - start}\n\n");
            }
        }


        public static void RegisterTypes(Container c)
        {
            var gen = new ProxyGenerator(); //proxy generator that enables lazy service loading

            var coreAssembly = typeof(BaseModel<>).Assembly;

            //register the container
            c.Register(() => c, Lifestyle.Singleton);


            #region infrastructure service registration

            var cache = new WeakCache();

            //authorization server is a special case where it is created in the singleton scope, but relies on some services that are registered in ScopedLifeStyles...
            //thus we explicitly create the instance of the authorization server.
            //c.Register<IOAuthAuthorizationServerProvider>(() => c.GetInstance<AuthorizationServer>());
            //c.Register(() =>
            //{
            //    var europa = new EuropaContext(c.GetInstance<ContextConfiguration<EuropaContext>>());
            //    var credentialAuthenticator = new CredentialAuthentication(europa, new DefaultHasher());
            //    return new AuthorizationServer(credentialAuthenticator, europa, cache);
            //}, Lifestyle.Singleton);

            ////bearer authentication provider is a special case where it is created in the singleton scope, but relies on some services that are registered in ScopedLifeStyles...
            ////thus we explicitly create the instance of the authentication provider.
            //c.Register<IOAuthBearerAuthenticationProvider>(() => c.GetInstance<BearerAuthenticationProvider>());
            //c.Register(() =>
            //{
            //    return new BearerAuthenticationProvider(cache);
            //}, Lifestyle.Singleton);


            c.Register<OwinContextProvider, OwinContextProvider>(Lifestyle.Scoped);
            c.RegisterLazyService<ICredentialHasher, DefaultHasher>(gen, Lifestyle.Scoped);
            c.RegisterLazyService<IBlobStore, FileSystemBlobStore>(gen, Lifestyle.Scoped);
            c.RegisterLazyService<IEmailPush, ElasticMailPushService>(gen, Lifestyle.Singleton);
            c.RegisterLazyService<IAppUrlProvider, UrlProvider>(gen, Lifestyle.Scoped);
            c.RegisterLazyService<IPersistenceCommands, SimplePersistenceCommands>(gen, Lifestyle.Scoped);
            c.RegisterLazyService<IUserContext, _UserContext>(gen, Lifestyle.Scoped);
            #endregion


            #region domain/domain-service registration

            #region Jupiter

            //shared context configuration.
            var config = WebConstants.Misc_UniversalEuropaConfig;
            c.Register(() => config, Lifestyle.Singleton);

            //scoped europa context
            c.Register<IDataContext, EuropaContext>(Lifestyle.Scoped); //doesnt need lazy initialization

            #endregion

            #region queries
            c.RegisterLazyService<Core.Services.Query.IAccountQuery, Data.EF.Query.AccountQuery>(gen, Lifestyle.Scoped);
            c.RegisterLazyService<Core.Services.Query.IBlockChainQuery, Data.EF.Query.BlockChainQuery>(gen, Lifestyle.Scoped);
            c.RegisterLazyService<Core.Services.Query.IBitLevelQuery, Data.EF.Query.BitLevelQuery>(gen, Lifestyle.Scoped);
            c.RegisterLazyService<Core.Services.Query.IContextVerifierQuery, Data.EF.Query.ContextVerifierQuery>(gen, Lifestyle.Scoped);
            c.RegisterLazyService<Core.Services.Query.IReferralQuery, Data.EF.Query.ReferralQuery>(gen, Lifestyle.Scoped);
            c.RegisterLazyService<Core.Services.Query.ISettingsQuery, Data.EF.Query.SettingsQuery>(gen, Lifestyle.Scoped);
            c.RegisterLazyService<Core.Services.Query.IUserContextQuery, Data.EF.Query.UserContextQuery>(gen, Lifestyle.Scoped);
            c.RegisterLazyService<Core.Services.Query.IUserNotifierQuery, Data.EF.Query.UserNotifierQuery>(gen, Lifestyle.Scoped);
            #endregion

            #region Axis.Pollux.Identity

            #endregion

            #region Axis.Pollux.Authentication

            c.RegisterLazyService<ICredentialAuthentication, CredentialAuthentication>(gen, Lifestyle.Scoped);
            #endregion

            #region Axis.Pollux.RBAC

            c.Register<IUserAuthorization>(() =>
            {
                var europa = new EuropaContext(c.GetInstance<ContextConfiguration<EuropaContext>>());
                return new CachableRoleAuthority(europa);
            }, Lifestyle.Singleton);
            #endregion

            #region BitDiamond.Core.Models/BitDiamond.Core.Services

            c.RegisterLazyService<IAccountManager, Core.Services.AccountManager>(gen, Lifestyle.Scoped);
            c.RegisterLazyService<IBitLevelManager, BitLevelManager>(gen, Lifestyle.Scoped);
            c.RegisterLazyService<IBlockChainService, BlockChainService>(gen, Lifestyle.Scoped);
            c.RegisterLazyService<IContextVerifier, ContextVerifier>(gen, Lifestyle.Scoped);
            c.RegisterLazyService<IReferralManager, ReferralManager>(gen, Lifestyle.Scoped);
            c.RegisterLazyService<ISettingsManager, SettingsManager>(gen, Lifestyle.Scoped);
            c.RegisterLazyService<IUserNotifier, UserNotifier>(gen, Lifestyle.Scoped);
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

    public class _UserContext : IUserContext
    {
        public User CurrentUser() => _user;

        public IEnumerable<string> CurrentUserRoles() => new[] { "#bit-member" };

        public UserLogon CurrentUserLogon()
        {
            throw new NotImplementedException();
        }

        private User _user = new User
        {
            CreatedOn = DateTime.Now,
            Status = (int)AccountStatus.Active,
            EntityId = "dev.bankai@gmail.com",
            UId = Guid.NewGuid()
        };
    }
}
