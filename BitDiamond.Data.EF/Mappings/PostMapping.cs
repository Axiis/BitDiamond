using Axis.Jupiter.Europa;
using BitDiamond.Core.Models;

namespace BitDiamond.Data.EF.Mappings
{
    public class PostMapping : BaseModelMap<Post, long>
    {
        public PostMapping()
        {
            this.HasRequired(e => e.Owner)
                .WithMany()
                .HasForeignKey(e => e.OwnerId);

            this.Property(e => e.ContextId)
                .IsIndex("PostContext_Index");
        }
    }
}
