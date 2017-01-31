using Axis.Pollux.Identity.Principal;
using System.ComponentModel.DataAnnotations;

namespace BitDiamond.Core.Models
{
    public class Notification: BaseModel<long>
    {
        [MaxLength(500, ErrorMessage = "Title is too long")]
        public string Title { get; set; }
        public string Message { get; set; }

        [MaxLength(500, ErrorMessage = "ContextType is too long")]
        public string ContextType { get; set; }

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
