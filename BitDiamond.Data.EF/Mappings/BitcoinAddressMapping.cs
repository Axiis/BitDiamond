using BitDiamond.Core.Models;

namespace BitDiamond.Data.EF.Mappings
{
    public class BitcoinAddressMapping: BaseModelMap<BitcoinAddress, long>
    {
        public BitcoinAddressMapping()
        {
            Ignore(e => e.OwnerRef);

            Property(e => e.OwnerId).HasMaxLength(400);

            this.HasRequired(e => e.Owner)
                .WithMany()
                .HasForeignKey(e => e.OwnerId)
                .WillCascadeOnDelete(false);
        }
    }
}
