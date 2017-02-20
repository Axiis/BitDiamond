using Axis.Pollux.Identity.Principal;
using System.ComponentModel.DataAnnotations;

namespace BitDiamond.Core.Models
{
    public class Notification: BaseModel<long>
    {
        [MaxLength(500, ErrorMessage = "Title is too long")]
        public string Title { get; set; }

        [Required]
        public string Message { get; set; }

        public NotificationType Type { get; set; }
        public bool Seen { get; set; }

        private User _target;
        private string _targetId;
        [Required(ErrorMessage = "Target is Required")]
        public virtual User Target
        {
            get { return _target; }
            set
            {
                _target = value;
                if (value != null) _targetId = _target.EntityId;
                else _targetId = null;
            }
        }
        public string TargetId
        {
            get { return _targetId; }
            set
            {
                _targetId = value;
                if (value == null) _target = null;
                else if (!value.Equals(_target?.EntityId)) _target = null;
            }
        }
    }

    public enum NotificationType
    {
        Info,
        Error,
        Warning,
        Success
    }
}
