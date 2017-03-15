using Axis.Jupiter;
using BitDiamond.Core.Services.Query;
using System.Collections.Generic;
using System.Linq;

using static Axis.Luna.Extensions.ExceptionExtensions;
using Axis.Luna;
using Axis.Pollux.Identity.Principal;
using BitDiamond.Core.Models;
using System;

namespace BitDiamond.Data.EF.Query
{
    public class BlockChainQuery: IBlockChainQuery
    {
        private IDataContext _europa = null;

        public BlockChainQuery(IDataContext context)
        {
            ThrowNullArguments(() => context);

            _europa = context;
        }


        #region Base Queries
        public IQueryable<TransactionJoiner> BaseBlockChainQuery()
        => from btc in _europa.Store<BlockChainTransaction>().Query
           join rad in _europa.Store<BitcoinAddress>().Query on btc.ReceiverId equals rad.Id
           join sad in _europa.Store<BitcoinAddress>().Query on btc.SenderId equals sad.Id
           join r in _europa.Store<User>().Query on rad.OwnerId equals r.EntityId
           join s in _europa.Store<User>().Query on sad.OwnerId equals s.EntityId
           join rref in _europa.Store<ReferralNode>().Query on rad.OwnerId equals rref.UserId
           join sref in _europa.Store<ReferralNode>().Query on sad.OwnerId equals sref.UserId
           join rb in _europa.Store<BioData>().Query on rad.OwnerId equals rb.OwnerId into _rb
           join sb in _europa.Store<BioData>().Query on sad.OwnerId equals sb.OwnerId into _sb
           from __rb in _rb.DefaultIfEmpty()
           from __sb in _sb.DefaultIfEmpty()
           select new TransactionJoiner
           {
               Transaction = btc,
               ReceiverAddress = rad,
               SenderAddress = sad,
               ReceiverNode = rref,
               SenderNode = sref,
               SenderBio = __sb,
               ReceiverBio = __rb,
               Receiver = r,
               Sender = s
           };
        #endregion

        public IEnumerable<BlockChainTransaction> GetAllUserTransactions(User user)
        {
            var addresses = _europa.Store<BitcoinAddress>().Query
                .Where(_bcad => _bcad.OwnerId == user.UserId)
                .Select(_bcad => _bcad.Id)
                .ToArray();

            return BaseBlockChainQuery()
                .Where(btc => addresses.Contains(btc.Transaction.ReceiverId) || addresses.Contains(btc.Transaction.SenderId))
                .AsEnumerable()
                .Select(_q => _q.ToBlockChainTransaction())
                .ToArray();
        }

        public decimal GetSystemTransactionsTotal()
        => _europa.Store<BlockChainTransaction>()
            .Query
            .Sum(_bc => _bc.Amount);

        public decimal GetIncomingUserTransactionsTotal(User user)
        => _europa.Store<BlockChainTransaction>()
            .QueryWith(_bc => _bc.Receiver)
            .Where(_bc => _bc.Receiver.OwnerId == user.UserId)
            .Sum(_bc => _bc.Amount);

        public decimal GetOutgoingUserTransactionsTotal(User user)
        => _europa.Store<BlockChainTransaction>()
            .QueryWith(_bc => _bc.Sender)
            .Where(_bc => _bc.Sender.OwnerId == user.UserId)
            .Sum(_bc => _bc.Amount);

        public SequencePage<BlockChainTransaction> GetPagedIncomingUserTransactions(User user, int pageSize, int pageIndex = 0)
        {
            var addresses = _europa.Store<BitcoinAddress>().Query
                .Where(_bcad => _bcad.OwnerId == user.UserId)
                .Select(_bcad => _bcad.Id)
                .ToArray();

            var q = BaseBlockChainQuery().Where(btc => addresses.Contains(btc.Transaction.ReceiverId));

            return new SequencePage<BlockChainTransaction>(q
                .OrderBy(_x => _x.Transaction.CreatedOn)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .AsEnumerable()
                .Select(_btc => _btc.ToBlockChainTransaction())
                .ToArray(), q.Count(), pageSize, pageIndex);
        }

        public SequencePage<BlockChainTransaction> GetPagedOutgoingUserTransactions(User user, int pageSize, int pageIndex = 0)
        {
            var addresses = _europa.Store<BitcoinAddress>().Query
                .Where(_bcad => _bcad.OwnerId == user.UserId)
                .Select(_bcad => _bcad.Id)
                .ToArray();

            var q = BaseBlockChainQuery().Where(btc => addresses.Contains(btc.Transaction.SenderId));

            return new SequencePage<BlockChainTransaction>(q
                .OrderBy(_x => _x.Transaction.CreatedOn)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .AsEnumerable()
                .Select(_btc => _btc.ToBlockChainTransaction())
                .ToArray(), q.Count(), pageSize, pageIndex);
        }

        public BlockChainTransaction GetTransactionByHash(string transactionHash)
        => BaseBlockChainQuery()
            .Where(_trnx => _trnx.Transaction.TransactionHash == transactionHash)
            .FirstOrDefault()
            .ToBlockChainTransaction();

        public BlockChainTransaction GetTransactionById(long transactionId)
        => BaseBlockChainQuery()
            .Where(_trnx => _trnx.Transaction.Id == transactionId)
            .FirstOrDefault()
            .ToBlockChainTransaction();


        #region joiner helpers

        public class TransactionJoiner
        {
            public BlockChainTransaction Transaction { get; set; }
            public BitcoinAddress SenderAddress { get; set; }
            public BitcoinAddress ReceiverAddress { get; set; }
            public ReferralNode SenderNode { get; set; }
            public ReferralNode ReceiverNode { get; set; }
            public BioData SenderBio { get; set; }
            public BioData ReceiverBio { get; set; }
            public User Receiver { get; set; }
            public User Sender { get; set; }

            public BlockChainTransaction ToBlockChainTransaction()
            {
                if (SenderBio != null) SenderBio.Owner = Sender;
                if (ReceiverBio != null) ReceiverBio.Owner = Receiver;

                SenderNode.UserBio = SenderBio;
                SenderNode.User = Sender;

                ReceiverNode.UserBio = ReceiverBio;
                ReceiverNode.User = Receiver;

                SenderAddress.OwnerRef = SenderNode;
                ReceiverAddress.OwnerRef = ReceiverNode;

                Transaction.Sender = SenderAddress;
                Transaction.Receiver = ReceiverAddress;

                return Transaction;
            }
        }
        #endregion
    }
}
