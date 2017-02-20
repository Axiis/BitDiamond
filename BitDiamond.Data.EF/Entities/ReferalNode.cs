using Axis.Pollux.Identity.Principal;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BitDiamond.Data.EF.Entities
{
    public class ReferralNode: Core.Models.ReferralNode
    {
        public string UserId { get; set; }
        public override User User
        {
            get { return base.User; }
            set
            {
                UserId = value?.EntityId;
                base.User = value;
            }
        }
    }
}
