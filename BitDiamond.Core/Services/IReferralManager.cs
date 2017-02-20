using Axis.Luna;
using BitDiamond.Core.Models;
using System.Collections.Generic;

namespace BitDiamond.Core.Services
{
    public interface IReferralManager
    {
        Operation<ReferralNode> AffixNewUser(string userId, string refereeCode);

        Operation<IEnumerable<ReferralNode>> DirectDownlines(ReferralNode node);
        Operation<IEnumerable<ReferralNode>> AllDownlines(ReferralNode node);
        Operation<IEnumerable<ReferralNode>> Uplines(ReferralNode node);
    }
}
