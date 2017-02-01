using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitDiamond.Core.Models.Email
{
    public class AccountActivation: MailModel
    {
        public string Target { get; set; }

        public string Link { get; set; }
    }
}
