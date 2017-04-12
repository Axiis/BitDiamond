using Axis.Jupiter;
using Axis.Jupiter.Europa;
using Axis.Jupiter.Kore.Command;
using Axis.Luna;
using Axis.Luna.Extensions;
using Axis.Pollux.Authentication;
using Axis.Pollux.Authentication.Service;
using Axis.Pollux.CoreAuthentication;
using Axis.Pollux.CoreAuthentication.Services;
using Axis.Pollux.RBAC.Services;
using Axis.Pollux.RoleAuthorization.Services;
using BitDiamond.Core.Models;
using BitDiamond.Core.Services;
using BitDiamond.Core.Services.Services;
using BitDiamond.Data.EF.Command;
using BitDiamond.Web.Infrastructure.Config.Hangfire;
using BitDiamond.Web.Infrastructure.Services;
using BitDiamond.Web.Infrastructure.Utils;
using Castle.DynamicProxy;
using SimpleInjector;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web.Http;
using System.Web.Mvc;

namespace BitDiamond.Web.Infrastructure.DI
{
    public static class DIRegistrations
    {
        public static Container RegisterTypes(Container c)
        {
            var gen = new ProxyGenerator(); //proxy generator that enables lazy service loading

            var coreAssembly = typeof(BaseModel<>).Assembly;

            //register the container
            c.Register(() => c, Lifestyle.Singleton);


            #region infrastructure service registration

            var cache = new WeakCache();

            c.Register<OwinContextProvider, OwinContextProvider>(Lifestyle.Scoped);
            c.RegisterLazyService<ICredentialHasher, DefaultHasher>(Lifestyle.Scoped);                   
            c.RegisterLazyService<IBlobStore, FileSystemBlobStore>(Lifestyle.Scoped);
            c.RegisterLazyService<IEmailPush, ElasticMailPushService>(Lifestyle.Singleton);
            c.RegisterLazyService<IAppUrlProvider, UrlProvider>(Lifestyle.Scoped);
            c.RegisterLazyService<IPersistenceCommands, SimplePersistenceCommands>(Lifestyle.Scoped);
            c.RegisterLazyService<IBackgroundOperationScheduler, Services.Hangfire.HangfireJobScheduler>(Lifestyle.Scoped);
            c.RegisterLazyService<IUserContext, UserContext>(Lifestyle.Scoped);
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
            c.RegisterLazyService<Core.Services.Query.IAccountQuery, Data.EF.Query.AccountQuery>(Lifestyle.Scoped);
            c.RegisterLazyService<Core.Services.Query.IBlockChainQuery, Data.EF.Query.BlockChainQuery>(Lifestyle.Scoped);
            c.RegisterLazyService<Core.Services.Query.IBitLevelQuery, Data.EF.Query.BitLevelQuery>(Lifestyle.Scoped);
            c.RegisterLazyService<Core.Services.Query.IContextVerifierQuery, Data.EF.Query.ContextVerifierQuery>(Lifestyle.Scoped);
            c.RegisterLazyService<Core.Services.Query.IReferralQuery, Data.EF.Query.ReferralQuery>(Lifestyle.Scoped);
            c.RegisterLazyService<Core.Services.Query.ISettingsQuery, Data.EF.Query.SettingsQuery>(Lifestyle.Scoped);
            c.RegisterLazyService<Core.Services.Query.IUserContextQuery, Data.EF.Query.UserContextQuery>(Lifestyle.Scoped);
            c.RegisterLazyService<Core.Services.Query.IUserNotifierQuery, Data.EF.Query.UserNotifierQuery>(Lifestyle.Scoped);
            c.RegisterLazyService<Core.Services.Query.IPostQuery, Data.EF.Query.PostQuery>(Lifestyle.Scoped);
            #endregion

            #region Axis.Pollux.Identity

            #endregion

            #region Axis.Pollux.Authentication

            c.RegisterLazyService<ICredentialAuthentication, CredentialAuthentication>(Lifestyle.Scoped);
            #endregion

            #region Axis.Pollux.RBAC
            //5 stars for this, bruh!!!
            c.Register<IUserAuthorization>(() =>
            {
                var europa = new EuropaContext(c.GetInstance<ContextConfiguration<EuropaContext>>());
                return new CachableRoleAuthority(europa);
            }, Lifestyle.Singleton);
            #endregion

            #region BitDiamond.Core.Models/BitDiamond.Core.Services
            c.RegisterLazyService<IAccountManager, AccountManager>(Lifestyle.Scoped);
            c.RegisterLazyService<IBitLevelManager, BitLevelManager>(Lifestyle.Scoped);
            c.RegisterLazyService<IBlockChainService, BlockChainService>(Lifestyle.Scoped);
            c.RegisterLazyService<IContextVerifier, ContextVerifier>(Lifestyle.Scoped);
            c.RegisterLazyService<IReferralManager, ReferralManager>(Lifestyle.Scoped);
            c.RegisterLazyService<ISettingsManager, SettingsManager>(Lifestyle.Scoped);
            c.RegisterLazyService<IPostService, PostService>(Lifestyle.Scoped);
            c.RegisterLazyService<IUserNotifier, UserNotifier>(Lifestyle.Scoped);
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

            return c;

        }

        public static Container RegisterHangfireTypes(Container container) => container.UsingValue(c =>
        {
            //register all possible types
            c = RegisterTypes(c);

            //override necessary registrations
            //1. IUserContext
            c.Register(() => CallContext.LogicalGetData(Interceptor.CallContextParameters).As<IUserContext>(), Lifestyle.Scoped);
        });
    }
}