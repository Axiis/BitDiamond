using Axis.Pollux.Identity.Principal;

namespace BitDiamond.Core.Models
{
    /// <summary>
    /// </summary>
    public class UserLogon: BaseModel<long>
    {
        public string Device { get; set; }
        public string Location { get; set; }
        public string OwinToken { get; set; }
        public bool Invalidated { get; set; }

        //note: implement "LastActive" using the "ModifiedOn" property

        public User User { get; set; }
    }
}
