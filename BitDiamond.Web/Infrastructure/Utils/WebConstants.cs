using Axis.Jupiter.Europa;
using Axis.Pollux.Authentication.OAModule;
using Axis.Pollux.Identity.OAModule;
using Axis.Pollux.RBAC.OAModule;
using BitDiamond.Data.EF;
using System;
using System.Configuration;

namespace BitDiamond.Web.Infrastructure.Utils
{
    public static class WebConstants
    {
        #region OAuthPaths
        public static readonly string OAuthPath_TokenPath = "/tokens";
        #endregion

        #region OAuthCustomHeaders
        public static readonly string OAuthCustomHeaders_OldToken = "OAuthOldToken";
        #endregion

        #region Misc
        public static readonly string Misc_UserLogonOwinContextKey = "$__BitDiamond.UserLogon";
        public static readonly TimeSpan Misc_TokenValidityDuration = TimeSpan.FromDays(365);
        public static readonly ContextConfiguration<EuropaContext> Misc_UniversalEuropaConfig = new ContextConfiguration<EuropaContext>()
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

        public static readonly int Misc_SessionTimeoutMinutes = 15;
        #endregion
    }
}