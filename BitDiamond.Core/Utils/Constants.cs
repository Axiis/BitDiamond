using System;

namespace BitDiamond.Core.Utils
{
    public static class Constants
    {
        #region Settings
        public static readonly TimeSpan DefaultContextVerificationExpirationTime = TimeSpan.FromDays(2);
        #endregion



        public static readonly int BitLevelCount = 3;
        public static readonly decimal UpgradeCostLevel1 = 0.11m;
        public static readonly decimal UpgradeCostLevel2 = 0.1826m;
        public static readonly decimal UpgradeCostLevel3 = 0.3652m;
    }
}
