using Axis.Pollux.Identity.Principal;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BitDiamond.Core.Models
{
    public class ReferralNode: BaseModel<long>
    {
        private User _user;
        private string _userId;
        [Required(ErrorMessage = "User is Required")]
        public virtual User User
        {
            get { return _user; }
            set
            {
                _user = value;
                if (value != null) _userId = _user.EntityId;
                else _userId = null;
            }
        }
        public string UserId
        {
            get { return _userId; }
            set
            {
                _userId = value;
                if (value == null) _user = null;
                else if (!value.Equals(_user?.EntityId)) _user = null;
            }
        }

        [MaxLength(50, ErrorMessage="Reference code is too long")]
        public string ReferenceCode { get; set; }

        public string ReferrerCode { get; set; }
        public virtual ReferralNode Referrer { get; set; }
        public virtual List<ReferralNode> Referrals { get; private set; } = new List<ReferralNode>();

        public string UplineCode { get; set; }
        public virtual ReferralNode Upline { get; set; }

        public virtual List<ReferralNode> DirectDownlines { get; private set; } = new List<ReferralNode>();
    }
}
