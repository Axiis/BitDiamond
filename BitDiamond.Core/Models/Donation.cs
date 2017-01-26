namespace BitDiamond.Core.Domain
{
    public class Donation: BaseModel<long>
    {
        public BitcoinAddress Sender { get; set; }
        public BitcoinAddress Reciever { get; set; }
        public string TransactionHash { get; set; }

        public DonationStatus Status { get; set; }

        public int Level { get; set; }
        public int SkipCount { get; set; }
        public int DonationCount { get; set; }

    }

    public enum DonationStatus
    {
        AwaitingVerification,
        Verifying,
        Invalid,
        Valid
    }
}
