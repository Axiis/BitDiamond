using BitDiamond.Core.Models;

namespace BitDiamond.Data.EF.Mappings
{
    public class ReferalNodeMapping : BaseModelMap<ReferralNode, long>
    {
        public ReferalNodeMapping()
        {
            this.HasRequired(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId);

            this.Ignore(e => e.DirectDownlines)
                .Ignore(e => e.Referrals)
                .Ignore(e => e.Referrer)
                .Ignore(e => e.Upline)
                .Ignore(e => e.UserBio)
                .Ignore(e => e.UserContact)
                .Ignore(e => e.ProfileImageUrl);
        }
    }
}
