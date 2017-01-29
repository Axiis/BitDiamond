using BitDiamond.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitDiamond.Core.Services
{
    public interface IBlockChainService
    {
        BlockChainTransaction GetTransactionDetails(string transactionStatus);
    }
}
