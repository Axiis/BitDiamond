using Axis.Jupiter.Europa;
using BitDiamond.Core.Models;

namespace BitDiamond.Data.EF.Mappings
{
    public class ContextVerificationMapping : BaseModelMap<ContextVerification, long>
    {
        public ContextVerificationMapping()
        {
            this.HasRequired(e => e.Target)
                .WithMany()
                .HasForeignKey(e => e.TargetId);


            this.Property(e => e.VerificationToken)
                .IsIndex(nameof(ContextVerification.VerificationToken))
                .HasMaxLength(100);

            this.Property(e => e.Context)
                .IsIndex(nameof(ContextVerification.Context))
                .HasMaxLength(250);
        }
    }
}
