using Axis.Pollux.Identity.Principal;
using BitDiamond.Core.Models;
using System.Collections.Generic;

namespace BitDiamond.Core.Services.Query
{
    public interface IBitLevelQuery
    {
        IEnumerable<ReferalNode> Uplines(User user);
        IEnumerable<ReferalNode> Downlines(User user);
        IEnumerable<ReferalNode> Referrals(User user);
        ReferalNode Upline(User user, int uplineOffset);
        ReferalNode UserRef(User user);

        IEnumerable<BitLevel> GetBitLevelHistory(User user);
        BitLevel CurrentBitLevel(User user);
        BitLevel GetClosestValidBeneficiary(User user);
        BitLevel GetBitLevelById(long id);

        BitcoinAddress GetBitcoinAddress(User user);
    }
}
