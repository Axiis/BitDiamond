using System;

namespace BitDiamond.Web.Infrastructure.Utils
{
    public static class WebConstants
    {
        #region OAuthPaths
        public static readonly string OAuthPath_CredentialAuthorizationPath = "/login";
        public static readonly string OAuthPath_TokenPath = "/tokens";
        #endregion

        #region Misc
        public static readonly TimeSpan Misc_TokenValidityDuration = TimeSpan.FromDays(double.MaxValue);
        #endregion
    }
}