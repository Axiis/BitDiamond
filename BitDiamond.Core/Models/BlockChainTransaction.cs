using System;

namespace BitDiamond.Core.Models
{

    public class BlockChainTransaction : BaseModel
    {
        public BitcoinAddress Sender { get; set; }
        public BitcoinAddress Reciever { get; set; }
        public string TransactionHash { get; set; }

        public int LedgerCount { get; set; }
        public decimal Amount { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
