using Axis.Pollux.Identity.Principal;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BitDiamond.Core.Models
{
    public class ReferalNode: BaseModel<long>
    {
        public User User { get; set; }

        [MaxLength(50, ErrorMessage="Reference code is too long")]
        public string ReferenceCode { get; set; }

        public ReferalNode Referee { get; set; }
        public List<ReferalNode> Referals { get; private set; } = new List<ReferalNode>();

        public ReferalNode Upline { get; set; }
        public List<ReferalNode> DirectDownlines { get; private set; } = new List<ReferalNode>();
    }
}
