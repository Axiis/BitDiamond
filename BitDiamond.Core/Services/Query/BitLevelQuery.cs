using Axis.Pollux.Identity.Principal;
using BitDiamond.Core.Models;
using System.Collections.Generic;

namespace BitDiamond.Core.Services.Query
{
    public interface BitLevelQuery
    {
        IEnumerable<ReferalNode> Uplines(User user);
        IEnumerable<ReferalNode> Downlines(User user);
        ReferalNode UserRef(User user);

        IEnumerable<BitLevel> BitLevelHistory(User user);
        BitLevel CurrentBitLevel(User user);
    }
}
