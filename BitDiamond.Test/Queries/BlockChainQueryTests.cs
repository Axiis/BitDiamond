using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BitDiamond.Web.Infrastructure.Utils;
using BitDiamond.Data.EF;
using Axis.Pollux.RBAC.OAModule;
using Axis.Pollux.Authentication.OAModule;
using Axis.Pollux.Identity.OAModule;
using Axis.Jupiter.Europa;
using BitDiamond.Data.EF.Query;
using System.Linq;
using BitDiamond.Core.Models;
using BitDiamond.Web.Infrastructure.DI;
using SimpleInjector.Extensions.ExecutionContextScoping;
using BitDiamond.Web.Controllers.Api;
using BitDiamond.Core.Services;
using SimpleInjector;
using Castle.DynamicProxy;
using BitDiamond.Web.Infrastructure.Services;
using Axis.Pollux.Authentication;
using Axis.Pollux.CoreAuthentication;
using Axis.Jupiter.Kore.Command;
using BitDiamond.Data.EF.Command;
using Axis.Jupiter;
using Axis.Pollux.Authentication.Service;
using Axis.Pollux.CoreAuthentication.Services;
using Axis.Pollux.RoleAuthorization.Services;
using Axis.Pollux.RBAC.Services;
using Axis.Luna;
using BitDiamond.Core.Services.Services;
using System.Web.Http;
using Axis.Luna.Extensions;
using System.Web.Mvc;
using Axis.Pollux.Identity.Principal;
using System.Text;
using System.Collections.Generic;

namespace BitDiamond.Test.Queries
{
    [TestClass]
    public class BlockChainQueryTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var config = new ContextConfiguration<EuropaContext>()
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

            var cxt = new EuropaContext(config);

            var bcQuery = new BlockChainQuery(cxt);

            var start = DateTime.Now;
            var r = bcQuery.GetPagedOutgoingUserTransactions(new User { EntityId = "dev.bankai@gmail.com" }, 10, 0);
            Console.WriteLine(DateTime.Now - start);
        }

        [TestMethod]
        public void DISpeedTest()
        {
            var start = DateTime.Now;
            var resolver = new SimpleInjectorOwinResolutionContext(new ExecutionContextScopeLifestyle(), RegisterTypes);
            Console.WriteLine($"Registered dependencies in: {DateTime.Now - start}\n\n\n");

            using (var scope = resolver.NewResolutionScope())
            {
                start = DateTime.Now;
                var bcManager = scope.Resolve<IBlockChainService>();
                Console.WriteLine($"BlockChainService resolved in: {DateTime.Now - start}");

                start = DateTime.Now;
                var bcController = scope.Resolve<BlockChainController>();
                Console.WriteLine($"BlockChainController resolved in: {DateTime.Now - start}");

                var data = Convert.ToBase64String(Encoding.UTF8.GetBytes("{\"PageSize\":20, \"PageIndex\":0}"));
                start = DateTime.Now;
                var opr = bcController.GetOutgoingUserTransactions(data);//.GetOutgoingUserTransactions(20, 0);
                Console.WriteLine($"1st run bcController.GetOutgoingUserTransactions executed in: {DateTime.Now - start}");


                start = DateTime.Now;
                opr = bcController.GetOutgoingUserTransactions(data);//.GetOutgoingUserTransactions(20, 0);
                Console.WriteLine($"2nd run bcController.GetOutgoingUserTransactions executed in: {DateTime.Now - start}");


                start = DateTime.Now;
                opr = bcController.GetOutgoingUserTransactions(data);
                Console.WriteLine($"3rd run bcController.GetOutgoingUserTransactions executed in: {DateTime.Now - start}");
            }


            Console.WriteLine("\n\n\n");
            using (var scope = resolver.NewResolutionScope())
            {
                start = DateTime.Now;
                var bcManager = scope.Resolve<IBlockChainService>();
                Console.WriteLine($"BlockChainService resolved in: {DateTime.Now - start}");

                start = DateTime.Now;
                var bcController = scope.Resolve<BlockChainController>();
                Console.WriteLine($"BlockChainController resolved in: {DateTime.Now - start}");

                var data = Convert.ToBase64String(Encoding.UTF8.GetBytes("{\"PageSize\":20, \"PageIndex\":0}"));
                start = DateTime.Now;
                var opr = bcController.GetOutgoingUserTransactions(data);//.GetOutgoingUserTransactions(20, 0);
                Console.WriteLine($"1st run bcController.GetOutgoingUserTransactions executed in: {DateTime.Now - start}");


                start = DateTime.Now;
                opr = bcController.GetOutgoingUserTransactions(data);//.GetOutgoingUserTransactions(20, 0);
                Console.WriteLine($"2nd run bcController.GetOutgoingUserTransactions executed in: {DateTime.Now - start}");


                start = DateTime.Now;
                opr = bcController.GetOutgoingUserTransactions(data);
                Console.WriteLine($"3rd run bcController.GetOutgoingUserTransactions executed in: {DateTime.Now - start}");
            }


            Console.WriteLine("\n\n\n");
            using (var scope = resolver.NewResolutionScope())
            {
                start = DateTime.Now;
                var bcManager = scope.Resolve<IBlockChainService>();
                Console.WriteLine($"BlockChainService resolved in: {DateTime.Now - start}");

                start = DateTime.Now;
                var bcController = scope.Resolve<BlockChainController>();
                Console.WriteLine($"BlockChainController resolved in: {DateTime.Now - start}");

                var data = Convert.ToBase64String(Encoding.UTF8.GetBytes("{\"PageSize\":20, \"PageIndex\":0}"));
                start = DateTime.Now;
                var opr = bcController.GetOutgoingUserTransactions(data);//.GetOutgoingUserTransactions(20, 0);
                Console.WriteLine($"1st run bcController.GetOutgoingUserTransactions executed in: {DateTime.Now - start}");


                start = DateTime.Now;
                opr = bcController.GetOutgoingUserTransactions(data);//.GetOutgoingUserTransactions(20, 0);
                Console.WriteLine($"2nd run bcController.GetOutgoingUserTransactions executed in: {DateTime.Now - start}");


                start = DateTime.Now;
                opr = bcController.GetOutgoingUserTransactions(data);
                Console.WriteLine($"3rd run bcController.GetOutgoingUserTransactions executed in: {DateTime.Now - start}");
            }


            Console.WriteLine("\n\n\n");
            using (var scope = resolver.NewResolutionScope())
            {
                start = DateTime.Now;
                var bcManager = scope.Resolve<IBlockChainService>();
                Console.WriteLine($"BlockChainService resolved in: {DateTime.Now - start}");

                start = DateTime.Now;
                var bcController = scope.Resolve<BlockChainController>();
                Console.WriteLine($"BlockChainController resolved in: {DateTime.Now - start}");

                var data = Convert.ToBase64String(Encoding.UTF8.GetBytes("{\"PageSize\":20, \"PageIndex\":0}"));
                start = DateTime.Now;
                var opr = bcController.GetOutgoingUserTransactions(data);//.GetOutgoingUserTransactions(20, 0);
                Console.WriteLine($"1st run bcController.GetOutgoingUserTransactions executed in: {DateTime.Now - start}");


                start = DateTime.Now;
                opr = bcController.GetOutgoingUserTransactions(data);//.GetOutgoingUserTransactions(20, 0);
                Console.WriteLine($"2nd run bcController.GetOutgoingUserTransactions executed in: {DateTime.Now - start}");


                start = DateTime.Now;
                opr = bcController.GetOutgoingUserTransactions(data);
                Console.WriteLine($"3rd run bcController.GetOutgoingUserTransactions executed in: {DateTime.Now - start}");
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

        private User _user = new User
        {
            CreatedOn = DateTime.Now,
            Status = (int)AccountStatus.Active,
            EntityId = "dev.bankai@gmail.comm",
            UId = Guid.NewGuid()
        };
    }

}
