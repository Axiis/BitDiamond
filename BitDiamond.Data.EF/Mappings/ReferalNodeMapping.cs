using BitDiamond.Core.Models;

namespace BitDiamond.Data.EF.Mappings
{
    public class ReferalNodeMapping : BaseModelMap<ReferralNode, long>
    {
        public ReferalNodeMapping()
        {
            this.Ignore(e => e.UserBio);

            this.HasRequired(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId);

            this.Ignore(e => e.DirectDownlines)
                .Ignore(e => e.Referrals)
                .Ignore(e => e.Referrer)
                .Ignore(e => e.Upline);
        }
    }
}
