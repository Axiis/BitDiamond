using BitDiamond.Core.Models;

namespace BitDiamond.Data.EF.Mappings
{
    public class ReferalNodeMapping : BaseModelMap<ReferalNode, long>
    {
        public ReferalNodeMapping()
        {
            this.HasRequired(e => e.User)
                .WithMany();

            this.HasRequired(e => e.Referee)
                .WithMany(e => e.Referals);

            this.HasRequired(e => e.Upline)
                .WithMany(e => e.DirectDownlines);
        }
    }
}
