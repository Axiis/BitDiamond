namespace BitDiamond.Core.Models
{
    public class BitLevel: BaseModel<long>
    {
        public BlockChainTransaction Donation { get; set; }

        public int Level { get; set; }
        public int SkipCount { get; set; }
        public int DonationCount { get; set; }
        public int Cycle { get; set; }
    }
}
