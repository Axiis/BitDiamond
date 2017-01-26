using Axis.Pollux.Identity.Principal;

namespace BitDiamond.Core.Domain
{
    public class BitcoinAddress: BaseModel<long>
    {
        public string BlockChainAddress { get; set; }

        public User Owner { get; set; }
    }
}
