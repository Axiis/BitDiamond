using System;
using System.ComponentModel.DataAnnotations;

namespace BitDiamond.Core.Models
{

    public class BlockChainTransaction : BaseModel<long>
    {
        private BitcoinAddress _sender;
        private long _senderId;
        public virtual BitcoinAddress Sender
        {
            get { return _sender; }
            set
            {
                _sender = value;
                if (value != null) _senderId = _sender.Id;
                else _senderId = 0;
            }
        }
        [Required(ErrorMessage = "Sender Id is Required")]
        public long SenderId
        {
            get { return _senderId; }
            set
            {
                _senderId = value;
                if (value <= 0) _sender = null;
                else if (!value.Equals(_sender?.Id)) _sender = null;
            }
        }

        private BitcoinAddress _receiver;
        private long _receiverId;
        public virtual BitcoinAddress Receiver
        {
            get { return _receiver; }
            set
            {
                _receiver = value;
                if (value != null) _receiverId = _receiver.Id;
                else _receiverId = 0;
            }
        }
        [Required(ErrorMessage = "ReceiverId is Required")]
        public long ReceiverId
        {
            get { return _receiverId; }
            set
            {
                _receiverId = value;
                if (value <= 0) _receiver = null;
                else if (!value.Equals(_receiver?.Id)) _receiver = null;
            }
        }

        public string TransactionHash { get; set; }

        public int LedgerCount { get; set; }
        public decimal Amount { get; set; }

        public BlockChainTransactionStatus Status { get; set; } = BlockChainTransactionStatus.Unverified;

        public string ContextId { get; set; }
        public string ContextType { get; set; }
    }

    public enum BlockChainTransactionStatus
    {
        Unverified,
        Verified
    }
}
