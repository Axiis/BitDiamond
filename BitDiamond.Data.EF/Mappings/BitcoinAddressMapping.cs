using BitDiamond.Core.Models;

namespace BitDiamond.Data.EF.Mappings
{
    public class BitcoinAddressMapping: BaseModelMap<BitcoinAddress, long>
    {
        public BitcoinAddressMapping()
        {
            this.HasRequired(e => e.Owner)
                .WithRequiredDependent()
                .WillCascadeOnDelete(false);
        }
    }
}
