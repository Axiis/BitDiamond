using BitDiamond.Core.Models;

using static Axis.Luna.Extensions.ExceptionExtensions;
using Axis.Luna;
using BitDiamond.Core.Utils;
using BitDiamond.Core.Services.Query;
using Axis.Jupiter.Kore.Command;

namespace BitDiamond.Core.Services
{
    public class BlockChainService : IBlockChainService, IUserContextAware
    {
        public IUserContext UserContext { get; private set; }
        private IBitLevelQuery _blQuery;
        private IPersistenceCommands _pcommand;

        public BlockChainService(IBitLevelQuery blQuery, IUserContext context, IPersistenceCommands pcommand)
        {
            ThrowNullArguments(() => blQuery,
                               () => context,
                               () => pcommand);

            this._blQuery = blQuery;
            this.UserContext = context;
            this._pcommand = pcommand;
        }


        public Operation<BlockChainTransaction> GetTransactionDetails(string transactionHash)
        => Operation.Try(() =>
        {
            //use web client to call the webservice that validates the transaction hash...

            //but for the interim...
            var bl = this._blQuery.CurrentBitLevel(UserContext.CurrentUser());
            bl.Donation.LedgerCount = 4;
            bl.Donation.Status = BlockChainTransactionStatus.Verified;

            return _pcommand.Update(bl.Donation);
        });

        public Operation<BitcoinAddress> VerifyBitcoinAddress(BitcoinAddress bitcoinAddress)
        => Operation.Try(() =>
        {
            return bitcoinAddress;
        });
    }
}
