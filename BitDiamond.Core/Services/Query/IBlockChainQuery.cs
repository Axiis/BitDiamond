using Axis.Luna;
using Axis.Pollux.Identity.Principal;
using BitDiamond.Core.Models;
using System.Collections.Generic;

namespace BitDiamond.Core.Services.Query
{
    public interface IBlockChainQuery
    {
        BlockChainTransaction GetTransactionByHash(string transactionHash);
        BlockChainTransaction GetTransactionById(long transactionId);
        IEnumerable<BlockChainTransaction> GetAllUserTransactions(User user);
        SequencePage<BlockChainTransaction> GetPagedIncomingUserTransactions(User user, int pageSize, int pageIndex = 0);
        SequencePage<BlockChainTransaction> GetPagedOutgoingUserTransactions(User user, int pageSize, int pageIndex = 0);
    }
}
