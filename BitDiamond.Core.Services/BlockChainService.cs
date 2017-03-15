using BitDiamond.Core.Models;

using static Axis.Luna.Extensions.ExceptionExtensions;
using Axis.Luna;
using BitDiamond.Core.Utils;
using BitDiamond.Core.Services.Query;
using Axis.Jupiter.Kore.Command;
using System;
using System.Collections.Generic;
using Axis.Pollux.RBAC.Services;
using System.Net;
using Newtonsoft.Json;
using System.Linq;

namespace BitDiamond.Core.Services
{
    public class BlockChainService : IBlockChainService, IUserContextAware
    {
        public IUserContext UserContext { get; private set; }
        private IBlockChainQuery _blQuery;
        private IBitLevelQuery _levelQuery;
        private IPersistenceCommands _pcommand;
        private IUserAuthorization _authorizer;
        private IUserNotifier _notifier;

        public BlockChainService(IBlockChainQuery blQuery, IUserContext context, 
                                 IPersistenceCommands pcommand,
                                 IBitLevelQuery levelQuery,
                                 IUserAuthorization authorizer,
                                 IUserNotifier notifier)
        {
            ThrowNullArguments(() => blQuery,
                               () => context,
                               () => pcommand,
                               () => authorizer,
                               () => levelQuery,
                               () => notifier);

            this._blQuery = blQuery;
            this.UserContext = context;
            this._pcommand = pcommand;
            this._authorizer = authorizer;
            this._levelQuery = levelQuery;
            this._notifier = notifier;
        }


        public Operation<BlockChainTransaction> GetTransactionDetails(string transactionHash)
        => _authorizer.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            var bl = this._levelQuery.CurrentBitLevel(UserContext.CurrentUser());

            //use web client to call the webservice that validates the transaction hash...
            using (var client = new WebClient())
            {
                var trnx = client.DownloadString($"https://blockchain.info/tx/{transactionHash}?format=json");
                var trnxContainer = JsonConvert.DeserializeObject<TransactionContainer>(trnx);

                var currentBlockCount = long.Parse(client.DownloadString("https://blockchain.info/q/getblockcount"));

                //has any confirmations?
                if (trnxContainer.block_height == 0) throw new Exception();

                //receiver matches?
                else if (!trnxContainer.@out.Any(_tx => _tx.addr == bl.Donation.Receiver.BlockChainAddress)) throw new Exception("invalid receiver");

                //sender matches?
                else if (!trnxContainer.inputs.Any(_tx => _tx.prev_out.addr == bl.Donation.Sender.BlockChainAddress)) throw new Exception("invalid sender");

                //amount matches?
                else if ((trnxContainer.@out.Where(_tx => _tx.addr == bl.Donation.Receiver.BlockChainAddress).Sum(_tx => _tx.value) / 100000000) < bl.Donation.Amount)
                    throw new Exception("invalid amount");


                bl.Donation.LedgerCount = (int)(currentBlockCount - trnxContainer.block_height + 1);
                if (bl.Donation.LedgerCount >= 3) bl.Donation.Status = BlockChainTransactionStatus.Verified;

                return _pcommand.Update(bl.Donation);
            }
        });

        public Operation<BitcoinAddress> VerifyBitcoinAddress(BitcoinAddress bitcoinAddress)
        => _authorizer.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            using (var client = new WebClient())
            {
                //any errors will throw an exception
                var result = client.DownloadString($"https://blockchain.info/address/{bitcoinAddress.BlockChainAddress}?format=json");
                return bitcoinAddress;
            }
        });

        public Operation<IEnumerable<BlockChainTransaction>> GetAllUserTransactions()
        => _authorizer.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            return _blQuery.GetAllUserTransactions(UserContext.CurrentUser());
        });

        public Operation<SequencePage<BlockChainTransaction>> GetIncomingUserTransactions(int pageSize, int pageIndex)
        => _authorizer.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            return _blQuery.GetPagedIncomingUserTransactions(UserContext.CurrentUser(), pageSize, pageIndex);
        });

        public Operation<SequencePage<BlockChainTransaction>> GetOutgoingUserTransactions(int pageSize, int pageIndex)
        => _authorizer.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            return _blQuery.GetPagedOutgoingUserTransactions(UserContext.CurrentUser(), pageSize, pageIndex);
        });

        public Operation<BlockChainTransaction> VerifyManually(long transactionId)
        => _authorizer.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            var transaction = _blQuery.GetTransactionById(transactionId);
            if (transaction.Receiver.OwnerId != UserContext.CurrentUser().UserId)
                throw new Exception("Invalid transaction");

            transaction.Status = BlockChainTransactionStatus.Verified;
            _pcommand.Update(transaction);

            //notify user
            _notifier.NotifyUser(new Notification
            {
                Type = NotificationType.Info,
                TargetId = transaction.Sender.OwnerId,
                Title = "Transaction Confirmation",
                Message = $"Your transaction was <strong class='text-warning'>manually</strong> verified by the receiver."
            })
            .Resolve();

            return transaction;
        });

        public Operation<BlockChainTransaction> VerifyManually(string transactionHash)
        => _authorizer.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            var transaction = _blQuery.GetTransactionByHash(transactionHash);
            if (transaction.Receiver.OwnerId != UserContext.CurrentUser().UserId)
                throw new Exception("Invalid transaction");

            transaction.Status = BlockChainTransactionStatus.Verified;
            _pcommand.Update(transaction);

            //notify user
            _notifier.NotifyUser(new Notification
            {
                Type = NotificationType.Info,
                TargetId = transaction.Sender.OwnerId,
                Title = "Transaction Confirmation",
                Message = $"Your transaction was <strong class='text-warning'>manually</strong> verified by the receiver."
            })
            .Resolve();

            return transaction;
        });

        public Operation<decimal> GetIncomingUserTransactionsTotal()
        => _authorizer.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            throw new NotImplementedException();
        });

        public Operation<decimal> GetOutgoingUserTransactionsTotal()
        => _authorizer.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            throw new NotImplementedException();
        });

        public Operation<decimal> GetIncomingSystemTransactionsTotal()
        => _authorizer.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            throw new NotImplementedException();
        });

        public Operation<decimal> GetOutgoingSystemTransactionsTotal()
        => _authorizer.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            throw new NotImplementedException();
        });

        public class TransactionDescriptor
        {
            public bool spent { get; set; }
            public long tx_index { get; set; }
            public int type { get; set; }
            public string addr { get; set; }
            public decimal value { get; set; }
            public int n { get; set; }
            public string script { get; set; }
        }
        public class TransactionInput
        {
            public string sequence { get; set; }
            public TransactionDescriptor prev_out { get; set; }
            public string script { get; set; }
        }
        public class TransactionContainer
        {
            public int ver { get; set; }
            public TransactionInput[] inputs { get; set; }
            public long block_height { get; set; }
            public string relayed_by { get; set; }
            public TransactionDescriptor[] @out { get; set; }
            public int lock_time { get; set; }
            public int size { get; set; }
            public bool double_spend { get; set; }
            public long time { get; set; }
            public int vin_sz { get; set; }
            public string hash { get; set; }
            public int vout_sz { get; set; }
        }
    }
}
