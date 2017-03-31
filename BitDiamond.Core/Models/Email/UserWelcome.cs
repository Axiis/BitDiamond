using System;
using System.Linq;

namespace BitDiamond.Core.Models.Email
{
    [Serializable]
    public class UserWelcome: MailModel
    {
        public string Target
        {
            get { return Recipients.FirstOrDefault(); }
            set { Recipients = new[] { value }; }
        }

        public string Link { get; set; }
    }
}
