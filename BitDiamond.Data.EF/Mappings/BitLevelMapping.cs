using BitDiamond.Core.Models;

namespace BitDiamond.Data.EF.Mappings
{
    public class BitLevelMapping: BaseModelMap<BitLevel, long>
    {
        public BitLevelMapping()
        {
            this.Ignore(e => e.Donation);

            this.HasRequired(e => e.User)
                .WithMany();
        }
    }
}
