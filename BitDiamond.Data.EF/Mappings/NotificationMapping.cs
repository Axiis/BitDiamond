using BitDiamond.Core.Models;

namespace BitDiamond.Data.EF.Mappings
{
    public class NotificationMapping : BaseModelMap<Notification, long>
    {
        public NotificationMapping()
        {
            this.HasRequired(e => e.Target)
                .WithMany();

            this.Property(e => e.Title).HasMaxLength(500);
            this.Property(e => e.Message).IsMaxLength();
            this.Property(e => e.ContextType).HasMaxLength(500);
        }
    }
}
