using Axis.Pollux.Identity.Principal;

namespace BitDiamond.Data.EF.Entities
{
    /// <summary>
    /// </summary>
    public class UserLogon: Core.Models.UserLogon
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
