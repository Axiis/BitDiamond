using System;
using System.ComponentModel.DataAnnotations;

namespace BitDiamond.Core.Models
{

    public class BlockChainTransaction : BaseModel
    {
        [Required(ErrorMessage = "Sender is Required")]
        public BitcoinAddress Sender { get; set; }

        [Required(ErrorMessage = "Receiver is Required")]
        public BitcoinAddress Receiver { get; set; }
        
        public string TransactionHash { get; set; }

        public int LedgerCount { get; set; }
        public decimal Amount { get; set; }

        public DateTime CreatedOn { get; set; }

        public BlockChainTransactionStatus Status { get; set; } = BlockChainTransactionStatus.Unverified;

        public string ContextId { get; set; }
        public string ContextType { get; set; }
    }

    public enum BlockChainTransactionStatus
    {
        Unverified,
        Invalid,
        Valid
    }
}
