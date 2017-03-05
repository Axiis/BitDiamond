using BitDiamond.Core.Models;

using static Axis.Luna.Extensions.ExceptionExtensions;
using Axis.Luna;
using BitDiamond.Core.Utils;
using BitDiamond.Core.Services.Query;
using Axis.Jupiter.Kore.Command;
using System;
using System.Collections.Generic;
using Axis.Pollux.RBAC.Services;

namespace BitDiamond.Core.Services
{
    public class BlockChainService : IBlockChainService, IUserContextAware
    {
        public IUserContext UserContext { get; private set; }
        private IBlockChainQuery _blQuery;
        private IBitLevelQuery _levelQuery;
        private IPersistenceCommands _pcommand;
        private IUserAuthorization _authorizer;

        public BlockChainService(IBlockChainQuery blQuery, IUserContext context, 
                                 IPersistenceCommands pcommand,
                                 IBitLevelQuery levelQuery,
                                 IUserAuthorization authorizer)
        {
            ThrowNullArguments(() => blQuery,
                               () => context,
                               () => pcommand,
                               () => authorizer,
                               () => levelQuery);

            this._blQuery = blQuery;
            this.UserContext = context;
            this._pcommand = pcommand;
            this._authorizer = authorizer;
            this._levelQuery = levelQuery;
        }


        public Operation<BlockChainTransaction> GetTransactionDetails(string transactionHash)
        => _authorizer.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            //use web client to call the webservice that validates the transaction hash...

            //but for the interim...
            var bl = this._levelQuery.CurrentBitLevel(UserContext.CurrentUser());
            bl.Donation.LedgerCount = 4;
            bl.Donation.Status = BlockChainTransactionStatus.Verified;

            return _pcommand.Update(bl.Donation);
        });

        public Operation<BitcoinAddress> VerifyBitcoinAddress(BitcoinAddress bitcoinAddress)
        => _authorizer.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            //use web client object to make a call to the web api service to verify the bitcoin addresses existence
            return bitcoinAddress;
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

            return transaction;
        });
    }
}
