using Axis.Pollux.Identity.Principal;
using System.ComponentModel.DataAnnotations;

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

        [MaxLength(20)]
        public string Locale { get; set; }

        /// <summary>
        /// This should, in future, be an integer (positive or negative) denoting the number of minutes to be added (or subtracted pending on the polarity of the offset)
        /// to client datetime objects to get UTC date. Thus, West-Central-Africa will have a value of 60, meaning add 60 minutes to UTC time to get current time.
        /// </summary>
        [MaxLength(20)]
        public string TimeZone { get; set; }
        public int TimeZoneOffset() => int.Parse(TimeZone);

        //note: implement "LastActive" using the "ModifiedOn" property

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
