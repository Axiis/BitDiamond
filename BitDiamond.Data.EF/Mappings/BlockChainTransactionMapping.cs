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
                .WillCascadeOnDelete(false);

            this.HasRequired(e => e.Receiver)
                .WithMany()
                .WillCascadeOnDelete(false);

            this.Property(e => e.ContextId)
                .IsIndex(nameof(BlockChainTransaction.ContextId))
                .IsRequired();

            this.Property(e => e.ContextType)
                .IsIndex(nameof(BlockChainTransaction.ContextType))
                .IsRequired();
        }
    }
}
