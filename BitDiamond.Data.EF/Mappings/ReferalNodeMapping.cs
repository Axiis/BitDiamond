using BitDiamond.Core.Models;

namespace BitDiamond.Data.EF.Mappings
{
    public class ReferalNodeMapping : BaseModelMap<ReferalNode, long>
    {
        public ReferalNodeMapping()
        {
            this.HasRequired(e => e.User)
                .WithMany()
                .Map(m =>
                {
                    m.MapKey($"{nameof(ReferalNode.User)}Id");
                });

            this.Ignore(e => e.DirectDownlines)
                .Ignore(e => e.Referals)
                .Ignore(e => e.Referrer)
                .Ignore(e => e.Upline);
        }
    }
}
