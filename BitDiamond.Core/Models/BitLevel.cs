using Axis.Pollux.Identity.Principal;

namespace BitDiamond.Core.Models
{
    public class BitLevel: BaseModel<long>
    {
        public BlockChainTransaction Donation { get; set; } = new BlockChainTransaction();

        public int Level { get; set; }
        public int SkipCount { get; set; }
        public int DonationCount { get; set; }
        public int Cycle { get; set; }

        public User User { get; set; }
    }
}
