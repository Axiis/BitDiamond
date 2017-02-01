using Axis.Luna;
using Axis.Pollux.Identity.Principal;
using BitDiamond.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitDiamond.Core.Services
{
    public interface IReferalManager
    {
        Operation<ReferalNode> AffixNewUser(User user, string refereeCode);


        Operation<IEnumerable<ReferalNode>> DirectDownlines(ReferalNode node);
        Operation<IEnumerable<ReferalNode>> AllDownlines(ReferalNode node);
        Operation<IEnumerable<ReferalNode>> Uplines(ReferalNode node);
    }
}
