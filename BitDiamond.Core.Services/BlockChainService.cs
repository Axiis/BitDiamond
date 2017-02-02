using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitDiamond.Core.Models;

using static Axis.Luna.Extensions.ExceptionExtensions;
using Axis.Luna;

namespace BitDiamond.Core.Services
{
    public class BlockChainService : IBlockChainService, IUserContextAware
    {
        public IUserContext UserContext { get; private set; }


        public BlockChainService(IUserContext userContext)
        {
            ThrowNullArguments(() => userContext);

            UserContext = userContext;
        }


        public Operation<BlockChainTransaction> GetTransactionDetails(string transactionHash)
            => Operation.Try(() =>
            {
                //use web client to call the webservice that validates the transaction hash...

                return ((BlockChainTransaction)null).ThrowIf(_t => true, "");
            });
    }
}
