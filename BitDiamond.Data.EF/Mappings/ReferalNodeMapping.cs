using BitDiamond.Core.Models;

namespace BitDiamond.Data.EF.Mappings
{
    public class ReferalNodeMapping : BaseModelMap<ReferalNode, long>
    {
        public ReferalNodeMapping()
        {
            this.HasRequired(e => e.User)
                .WithMany();

            this.Ignore(e => e.DirectDownlines)
                .Ignore(e => e.Referals)
                .Ignore(e => e.Referee)
                .Ignore(e => e.Upline);
        }
    }
}
