using Axis.Jupiter.Europa.Mappings;
using BitDiamond.Core.Models;
using Axis.Jupiter.Europa;

namespace BitDiamond.Data.EF.Mappings
{
    public class BlockChainTransactionMapping: BaseMap<BlockChainTransaction>
    {
        public BlockChainTransactionMapping()
        {
            this.HasRequired(e => e.Sender)
                .WithMany()
                .HasForeignKey(e => e.SenderId)
                .WillCascadeOnDelete(false);

            this.HasRequired(e => e.Receiver)
                .WithMany()
                .HasForeignKey(e => e.ReceiverId)
                .WillCascadeOnDelete(false);

            this.Property(e => e.ContextId)
                .HasMaxLength(400)
                .IsIndex(nameof(BlockChainTransaction.ContextId))
                .IsRequired();

            this.Property(e => e.ContextType)
                .HasMaxLength(400)
                .IsIndex(nameof(BlockChainTransaction.ContextType))
                .IsRequired();

            this.Property(e => e.Amount)
                .HasPrecision(12, 12);
        }
    }
}
