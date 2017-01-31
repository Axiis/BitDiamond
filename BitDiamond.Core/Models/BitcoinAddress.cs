using Axis.Pollux.Identity.Principal;
using System.ComponentModel.DataAnnotations;
using static Axis.Luna.Extensions.ObjectExtensions;

namespace BitDiamond.Core.Models
{
    public class BitcoinAddress: BaseModel<long>
    {
        [MaxLength(40, ErrorMessage = "Invalid address length")]
        public string BlockChainAddress { get; set; }

        [Required(ErrorMessage = "address owner is required")]
        public User Owner { get; set; }

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
