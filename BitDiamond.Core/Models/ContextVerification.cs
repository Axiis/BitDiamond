using Axis.Pollux.Identity.Principal;

using System;

namespace BitDiamond.Core.Domain
{
    public class ContextVerification: BaseModel<long>
    {
        public virtual User Target { get; set; }
        public virtual string UserId { get; set; }

        public virtual string VerificationToken { get; set; }

        public virtual bool Verified { get; set; }

        public virtual string Context { get; set; }

        public DateTime ExpiresOn { get; set; }
    }
}
