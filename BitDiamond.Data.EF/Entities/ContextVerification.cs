using Axis.Pollux.Identity.Principal;

using System;

namespace BitDiamond.Data.EF.Entities
{
    public class ContextVerification: Core.Models.ContextVerification
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
