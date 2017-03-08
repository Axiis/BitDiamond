using Newtonsoft.Json;
using System;
using System.Linq;
using static Axis.Luna.Extensions.EnumerableExtensions;

namespace BitDiamond.Core.Utils
{
    public static class Constants
    {
        #region Settings
        public static readonly string Settings_DefaultContextVerificationExpirationTime = "System.ContextVerification.ValidPeriod";
        public static readonly string Settings_DefaultPasswordExpirationTime = "System.Credential.Password.ValidPeriod";
        public static readonly string Settings_MaxBitLevel = "System.BitLevel.MaxLevel";
        public static readonly string Settings_UpgradeFeeVector = "system.BitLevel.UpgradeFeeVector";
        #endregion

        #region Verification Contexts
        public static readonly string VerificationContext_UserActivation = "Context.UserActivation";
        public static readonly string VerificationContext_CredentialReset = "Context.CredentialUpdate";
        #endregion

        #region Transaction Contexts
        public static readonly string TransactionContext_UpgradeBitLevel = "BlockChainTransactionContext.UpgradeBitLevel";
        #endregion

        #region Roles
        public static readonly string Roles_RootRole = "#root";
        public static readonly string Roles_AdminRole = "#admin";
        public static readonly string Roles_GuestRole = "#guest";
        public static readonly string Roles_BitMemberRole = "#bit-member";
        #endregion

        #region System Users
        public static readonly string SystemUsers_Root = "@root";
        public static readonly string SystemUsers_Guest = "@guest";
        public static readonly string SystemUsers_Apex = "@apex";
        #endregion

        #region UserData
        public static readonly string UserData_ProfileImage = "Profile.Image";
        #endregion

        #region Mail Origins
        public static readonly string MailOrigin_DoNotReply = "donotreply@bitdiamond.com";
        #endregion

        #region Misc
        public static readonly JsonSerializerSettings Misc_DefaultJsonSerializerSettings = new JsonSerializerSettings
        {
            Converters = Enumerate<JsonConverter>()
                .Append(new Axis.Apollo.Json.TimeSpanConverter())
                .Append(new Axis.Apollo.Json.DateTimeConverter())
                .ToList(),
            MissingMemberHandling = MissingMemberHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            ObjectCreationHandling = ObjectCreationHandling.Auto,
            FloatFormatHandling = FloatFormatHandling.DefaultValue,
            //PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            StringEscapeHandling = StringEscapeHandling.Default,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        #endregion

        #region Post Contexts
        public static readonly string PostContext_News = "PostContext.News";
        #endregion

    }
}
