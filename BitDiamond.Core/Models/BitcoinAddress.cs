using Axis.Pollux.Identity.Principal;

using static Axis.Luna.Extensions.ObjectExtensions;

namespace BitDiamond.Core.Models
{
    public class BitcoinAddress: BaseModel<long>
    {
        public string BlockChainAddress { get; set; }

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
