namespace BitDiamond.Core.Models
{
    public class BitLevel: BaseModel<long>
    {
        #region Donnation
        public BitcoinAddress Sender { get; set; }
        public BitcoinAddress Reciever { get; set; }
        public string TransactionHash { get; set; }

        public double Amount { get; set; }

        public DonationStatus Status { get; set; }
        #endregion

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
