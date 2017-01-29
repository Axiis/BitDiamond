using Axis.Luna;
using BitDiamond.Core.Models;
using System.Collections.Generic;

namespace BitDiamond.Core.Services
{
    public interface IBitLevelManager
    {
        Operation<BitLevel> RequestUpgrade();
        Operation<BitLevel> ConfirmUpgrade(string transactionHash);
        Operation ManualDonationVerification();
        Operation<BitLevel> RecycleAccount();

        Operation<IEnumerable<BitLevel>> UserUpgradeHistory();
        Operation<BitLevel> GetBitLevelById(long id);
        Operation<BitLevel> CurrentUserLevel();
    }
}
