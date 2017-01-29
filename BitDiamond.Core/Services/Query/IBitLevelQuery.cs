using Axis.Pollux.Identity.Principal;
using BitDiamond.Core.Models;
using System.Collections.Generic;

namespace BitDiamond.Core.Services.Query
{
    public interface IBitLevelQuery
    {
        IEnumerable<ReferalNode> Uplines(User user);
        ReferalNode Upline(int uplineOffset);
        IEnumerable<ReferalNode> Downlines(User user);
        ReferalNode UserRef(User user);

        IEnumerable<BitLevel> GetBitLevelHistory(User user);
        BitLevel CurrentBitLevel(User user);
        BitLevel GetClosestValidAncestorLevel(int level);
        BitLevel GetBitLevelById(long id);

        BitcoinAddress GetBitcoinAddress(User user);
    }
}
