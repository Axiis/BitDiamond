using Axis.Luna;
using BitDiamond.Core.Models;
using System.Collections.Generic;

namespace BitDiamond.Core.Services
{
    public interface IReferralManager
    {
        Operation<ReferalNode> AffixNewUser(string userId, string refereeCode);

        Operation<IEnumerable<ReferalNode>> DirectDownlines(ReferalNode node);
        Operation<IEnumerable<ReferalNode>> AllDownlines(ReferalNode node);
        Operation<IEnumerable<ReferalNode>> Uplines(ReferalNode node);
    }
}
