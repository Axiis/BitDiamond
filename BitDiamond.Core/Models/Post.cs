using Axis.Pollux.Identity.Principal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitDiamond.Core.Models
{
    public class Post: BaseModel<long>
    {
        [MaxLength(400, ErrorMessage = "Title is too long")]
        public string Title { get; set; }
        
        [Column(TypeName = "ntext")]
        public string Message { get; set; }


        private User _owner;
        private string _ownerId;
        public virtual User Owner
        {
            get { return _owner; }
            set
            {
                _owner = value;
                if (value != null) _ownerId = _owner.EntityId;
                else _ownerId = null;
            }
        }
        [Required(ErrorMessage = "address owner id is required")]
        public string OwnerId
        {
            get { return _ownerId; }
            set
            {
                _ownerId = value;
                if (value == null) _owner = null;
                else if (!value.Equals(_owner?.EntityId)) _owner = null;
            }
        }

        
        public string Context { get; set; }

        [MaxLength(400, ErrorMessage = "Context Id is too long")]
        public string ContextId { get; set; }

        public PostStatus Status { get; set; }
    }

    public enum PostStatus
    {
        Draft,
        Published,
        Archived
    }
}
