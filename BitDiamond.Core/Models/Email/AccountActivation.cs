using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitDiamond.Core.Models.Email
{
    [Serializable]
    public class AccountActivation: MailModel
    {
        public string Target
        {
            get { return Recipients.FirstOrDefault(); }
            set { Recipients = new[] { value }; }
        }

        public string Link { get; set; }
    }
}
