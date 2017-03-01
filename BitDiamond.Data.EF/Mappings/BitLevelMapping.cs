using BitDiamond.Core.Models;

namespace BitDiamond.Data.EF.Mappings
{
    public class BitLevelMapping: BaseModelMap<BitLevel, long>
    {
        public BitLevelMapping()
        {
            this.HasRequired(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);

            this.HasOptional(e => e.Donation)
                .WithMany()
                .HasForeignKey(e => e.DonationId)
                .WillCascadeOnDelete(false);
        }
    }
}
