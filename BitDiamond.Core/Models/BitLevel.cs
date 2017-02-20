using Axis.Pollux.Identity.Principal;
using System.ComponentModel.DataAnnotations;

namespace BitDiamond.Core.Models
{
    /// <summary>
    /// </summary>
    public class BitLevel: BaseModel<long>
    {
        private BlockChainTransaction _transaction;
        private long _transactionId;
        public virtual BlockChainTransaction Donation
        {
            get { return _transaction; }
            set
            {
                _transaction = value;
                if (value != null) _transactionId = _transaction.Id;
                else _transactionId = 0;
            }
        }
        public long DonationId
        {
            get { return _transactionId; }
            set
            {
                _transactionId = value;
                if (value <= 0) _transaction = null;
                else if (!value.Equals(_transaction?.Id)) _transaction = null;
            }
        }


        [Range(0, int.MaxValue, ErrorMessage = "level is invalid")]
        public int Level { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "skip-count is invalid")]
        public int SkipCount { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "donation-count is invalid")]
        public int DonationCount { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "cycle is invalid")]
        public int Cycle { get; set; }

        private User _user;
        private string _userId;
        [Required(ErrorMessage = "address owner is required")]
        public virtual User User
        {
            get { return _user; }
            set
            {
                _user = value;
                if (value != null) _userId = _user.EntityId;
                else _userId = null;
            }
        }
        public string UserId
        {
            get { return _userId; }
            set
            {
                _userId = value;
                if (value == null) _user = null;
                else if (!value.Equals(_user?.EntityId)) _user = null;
            }
        }
    }
}
