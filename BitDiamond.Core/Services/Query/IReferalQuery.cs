using Axis.Pollux.Identity.Principal;
using BitDiamond.Core.Models;
using System.Collections.Generic;

namespace BitDiamond.Core.Services.Query
{
    public interface IReferralQuery
    {
        ReferralNode GetReferalNode(string referenceCode);

        ReferralNode GetUserReferalNode(User user);

        IEnumerable<ReferralNode> AllDownlines(ReferralNode node);

        IEnumerable<ReferralNode> DirectDownlines(ReferralNode node);

        IEnumerable<ReferralNode> Uplines(ReferralNode node);

        IEnumerable<string> GetAllReferenceCodes();

        User GetUserById(string userId);
    }
}
