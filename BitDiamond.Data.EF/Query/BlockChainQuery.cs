using Axis.Jupiter;
using BitDiamond.Core.Services.Query;
using System.Collections.Generic;
using System.Linq;

using static Axis.Luna.Extensions.ExceptionExtensions;
using Axis.Luna;
using Axis.Pollux.Identity.Principal;
using BitDiamond.Core.Models;

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

        public IEnumerable<BlockChainTransaction> GetAllUserTransactions(User user)
        {
            var addresses = _europa.Store<BitcoinAddress>().Query
                .Where(_bcad => _bcad.OwnerId == user.UserId)
                .Select(_bcad => _bcad.Id)
                .ToArray();

            var q = from btc in _europa.Store<BlockChainTransaction>().Query
            join rad in _europa.Store<BitcoinAddress>().Query on btc.ReceiverId equals rad.Id
            join sad in _europa.Store<BitcoinAddress>().Query on btc.SenderId equals sad.Id
            join rref in _europa.Store<ReferralNode>().Query on rad.OwnerId equals rref.UserId
            join sref in _europa.Store<ReferralNode>().Query on sad.OwnerId equals sref.UserId
            where addresses.Contains(btc.ReceiverId) || addresses.Contains(btc.SenderId)
            select new { btc, rad, sad, rref, sref };

            return q.ToArray().Select(_q =>
            {
                _q.rad.OwnerRef = _q.rref;
                _q.sad.OwnerRef = _q.sref;
                _q.btc.Receiver = _q.rad;
                _q.btc.Sender = _q.sad;
                return _q.btc;
            });
        }

        public SequencePage<BlockChainTransaction> GetPagedIncomingUserTransactions(User user, int pageSize, int pageIndex = 0)
        {
            var addresses = _europa.Store<BitcoinAddress>().Query
                .Where(_bcad => _bcad.OwnerId == user.UserId)
                .Select(_bcad => _bcad.Id)
                .ToArray();

            var q = from btc in _europa.Store<BlockChainTransaction>().Query
                    join rad in _europa.Store<BitcoinAddress>().Query on btc.ReceiverId equals rad.Id
                    join sad in _europa.Store<BitcoinAddress>().Query on btc.SenderId equals sad.Id
                    join rref in _europa.Store<ReferralNode>().Query on rad.OwnerId equals rref.UserId
                    join sref in _europa.Store<ReferralNode>().Query on sad.OwnerId equals sref.UserId
                    where addresses.Contains(btc.ReceiverId)
                    select new { btc, rad, sad, rref, sref };

            return new SequencePage<BlockChainTransaction>(q
            .OrderBy(_x => _x.btc.CreatedOn)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .AsEnumerable()
            //.ToArray()
            .Select(_q =>
            {
                _q.rad.OwnerRef = _q.rref;
                _q.sad.OwnerRef = _q.sref;
                _q.btc.Receiver = _q.rad;
                _q.btc.Sender = _q.sad;
                return _q.btc;
            })
            .ToArray(),
            q.Count(),
            pageSize,
            pageIndex);
        }

        public SequencePage<BlockChainTransaction> GetPagedOutgoingUserTransactions(User user, int pageSize, int pageIndex = 0)
        {
            var addresses = _europa.Store<BitcoinAddress>().Query
                .Where(_bcad => _bcad.OwnerId == user.UserId)
                .Select(_bcad => _bcad.Id)
                .ToArray();

            var q = from btc in _europa.Store<BlockChainTransaction>().Query
                    join rad in _europa.Store<BitcoinAddress>().Query on btc.ReceiverId equals rad.Id
                    join sad in _europa.Store<BitcoinAddress>().Query on btc.SenderId equals sad.Id
                    join rref in _europa.Store<ReferralNode>().Query on rad.OwnerId equals rref.UserId
                    join sref in _europa.Store<ReferralNode>().Query on sad.OwnerId equals sref.UserId
                    where addresses.Contains(btc.SenderId)
                    select new { btc, rad, sad, rref, sref };

            return new SequencePage<BlockChainTransaction>(q
            .OrderBy(_x => _x.btc.CreatedOn)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .AsEnumerable()
            //.ToArray()
            .Select(_q =>
            {
                _q.rad.OwnerRef = _q.rref;
                _q.sad.OwnerRef = _q.sref;
                _q.btc.Receiver = _q.rad;
                _q.btc.Sender = _q.sad;
                return _q.btc;
            })
            .ToArray(),
            q.Count(),
            pageSize,
            pageIndex);
        }
    }
}
