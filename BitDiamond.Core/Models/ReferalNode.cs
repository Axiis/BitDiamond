using Axis.Pollux.Identity.Principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitDiamond.Core.Models
{
    public class ReferalNode: BaseModel<long>
    {
        public User User { get; set; }
        public string ReferenceCode { get; set; }

        public ReferalNode Referee { get; set; }
        public List<ReferalNode> Referals { get; private set; } = new List<ReferalNode>();

        public ReferalNode Upline { get; set; }
        public List<ReferalNode> DirectDownlines { get; private set; } = new List<ReferalNode>();
    }
}
