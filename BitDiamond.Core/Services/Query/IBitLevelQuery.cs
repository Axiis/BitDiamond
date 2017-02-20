using Axis.Pollux.Identity.Principal;
using BitDiamond.Core.Models;
using System.Collections.Generic;

namespace BitDiamond.Core.Services.Query
{
    public interface IBitLevelQuery
    {
        IEnumerable<ReferralNode> Uplines(User user);
        IEnumerable<ReferralNode> Downlines(User user);
        IEnumerable<ReferralNode> Referrals(User user);
        ReferralNode Upline(User user, int uplineOffset);
        ReferralNode UserRef(User user);

        IEnumerable<BitLevel> GetBitLevelHistory(User user);
        BitLevel CurrentBitLevel(User user);
        BitLevel GetClosestValidBeneficiary(User user);
        BitLevel GetBitLevelById(long id);

        BitcoinAddress GetBitcoinAddress(User user);
    }
}
