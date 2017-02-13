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
        ReferalNode GetReferalNode(string referenceCode);

        IEnumerable<ReferalNode> AllDownlines(ReferalNode node);

        IEnumerable<ReferalNode> DirectDownlines(ReferalNode node);

        IEnumerable<ReferalNode> Uplines(ReferalNode node);

        IEnumerable<string> GetAllReferenceCodes();

        User GetUserById(string userId);
    }
}
