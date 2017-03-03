using Axis.Luna;
using Axis.Pollux.Identity.Principal;
using BitDiamond.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitDiamond.Core.Services.Query
{
    public interface IBlockChainQuery
    {

        IEnumerable<BlockChainTransaction> GetAllUserTransactions(User user);
        SequencePage<BlockChainTransaction> GetPagedIncomingUserTransactions(User user, int pageSize, int pageIndex = 0);
        SequencePage<BlockChainTransaction> GetPagedOutgoingUserTransactions(User user, int pageSize, int pageIndex = 0);
    }
}
