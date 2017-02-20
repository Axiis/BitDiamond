using Axis.Pollux.Identity.Principal;
using BitDiamond.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitDiamond.Core.Services.Query
{
    public interface IReferralQuery
    {
        ReferralNode GetReferalNode(string referenceCode);

        IEnumerable<ReferralNode> AllDownlines(ReferralNode node);

        IEnumerable<ReferralNode> DirectDownlines(ReferralNode node);

        IEnumerable<ReferralNode> Uplines(ReferralNode node);

        IEnumerable<string> GetAllReferenceCodes();

        User GetUserById(string userId);
    }
}
