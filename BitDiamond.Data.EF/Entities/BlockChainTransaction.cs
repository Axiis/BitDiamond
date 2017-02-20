using Axis.Luna.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace BitDiamond.Data.EF.Entities
{

    public class BlockChainTransaction : Core.Models.BlockChainTransaction
    {
        public long SenderId { get; set; }
        public new BitcoinAddress Sender
        {
            get { return base.Sender.As<BitcoinAddress>(); }
            set
            {
                SenderId = value?.Id ?? 0;
                base.Sender = value;
            }
        }

        public long ReceiverId { get; set; }
        public new BitcoinAddress Receiver
        {
            get { return base.Sender.As<BitcoinAddress>(); }
            set
            {
                ReceiverId = value?.Id ?? 0;
                base.Sender = value;
            }
        }
    }
}
