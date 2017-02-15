using Axis.Pollux.Identity.Principal;

namespace BitDiamond.Core.Models
{
    /// <summary>
    /// </summary>
    public class UserLogon: BaseModel<long>
    {
        public UserAgent Client { get; set; } = new UserAgent();
        public string Location { get; set; }
        public string OwinToken { get; set; }
        public bool Invalidated { get; set; }

        //note: implement "LastActive" using the "ModifiedOn" property

        public User User { get; set; }
    }

    public class UserAgent
    {
        public string OS { get; set; }
        public string OSVersion { get; set; }

        public string Browser { get; set; }
        public string BrowserVersion { get; set; }

        public string Device { get; set; }
    }
}
