using Axis.Pollux.Identity.Principal;
using System.ComponentModel.DataAnnotations;
using static Axis.Luna.Extensions.ObjectExtensions;

namespace BitDiamond.Core.Models
{
    public class BitcoinAddress: BaseModel<long>
    {
        [MaxLength(40, ErrorMessage = "Invalid address length")]
        public string BlockChainAddress { get; set; }


        private User _owner;
        private string _ownerId;
        [Required(ErrorMessage = "address owner is required")]
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

        public override int GetHashCode()
            => ValueHash(new object[]{ BlockChainAddress, Owner?.EntityId });

        public override bool Equals(object obj)
        {
            var val = obj.As<BitcoinAddress>();
            return GetHashCode() == val?.GetHashCode() &&
                   BlockChainAddress == val?.BlockChainAddress &&
                   Owner?.EntityId == val?.Owner?.EntityId;
        }
    }
}
