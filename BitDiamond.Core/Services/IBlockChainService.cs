using Axis.Luna;
using BitDiamond.Core.Models;

namespace BitDiamond.Core.Services
{
    public interface IBlockChainService
    {
        Operation<BlockChainTransaction> GetTransactionDetails(string transactionHash);

        Operation<BitcoinAddress> VerifyBitcoinAddress(BitcoinAddress bitcoinAddress);
    }
}
