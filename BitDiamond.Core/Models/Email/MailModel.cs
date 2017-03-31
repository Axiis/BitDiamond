using System;
using System.Collections.Generic;

namespace BitDiamond.Core.Models.Email
{
    [Serializable]
    public abstract class MailModel
    {
        public string From { get; set; }
        public string Subject { get; set; }

        private List<string> _recipients = new List<string>();
        public IEnumerable<string> Recipients
        {
            get { return _recipients; }
            set
            {
                _recipients.Clear();
                if (value != null) _recipients.AddRange(value);
            }
        }

        public string LogoUrl { get; set; }
        public string LogoTextUrl { get; set; }
    }
}
