using Axis.Pollux.Identity.Principal;
using System.ComponentModel.DataAnnotations;

namespace BitDiamond.Data.EF.Entities
{
    public class Notification: Core.Models.Notification
    {
        public string TargetId { get; set; }
        public override User Target
        {
            get { return base.Target; }
            set
            {
                TargetId = value?.EntityId;
                base.Target = value;
            }
        }
    }
}
