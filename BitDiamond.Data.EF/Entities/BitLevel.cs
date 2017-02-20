using Axis.Luna.Extensions;
using Axis.Pollux.Identity.Principal;
using System.ComponentModel.DataAnnotations;


namespace BitDiamond.Data.EF.Entities
{
    /// <summary>
    /// </summary>
    public class BitLevel: Core.Models.BitLevel
    {
        public long DonationId { get; set; }
        public new BlockChainTransaction Donation
        {
            get { return base.Donation.As<BlockChainTransaction>(); }
            set
            {
                DonationId = value?.Id ?? 0L;
                base.Donation = value;
            }
        }

        public string UserId { get; set; }
        public override User User
        {
            get { return base.User; }
            set
            {
                UserId = value?.EntityId;
                base.User = value;
            }
        }
    }
}
