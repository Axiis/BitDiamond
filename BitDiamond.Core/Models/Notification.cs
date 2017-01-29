using Axis.Pollux.Identity.Principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitDiamond.Core.Models
{
    public class Notification: BaseModel<long>
    {
        public string Message { get; set; }
        public NotificationType Type { get; set; }
        public bool Seen { get; set; }

        public User Target { get; set; }
    }

    public enum NotificationType
    {
        Info,
        Error,
        Warning,
        Success
    }
}
