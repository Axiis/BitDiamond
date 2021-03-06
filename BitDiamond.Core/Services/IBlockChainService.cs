﻿using Axis.Luna;
using Axis.Pollux.RBAC.Auth;
using BitDiamond.Core.Models;
using System.Collections.Generic;

namespace BitDiamond.Core.Services
{
    public interface IBlockChainService
    {
        [Resource(":system/block-chain/transactions/@getByHash")]
        Operation<BlockChainTransaction> AcquireTransactionDetails(string transactionHash);

        [Resource(":system/block-chain/addresses/@verify")]
        Operation<BitcoinAddress> VerifyBitcoinAddress(BitcoinAddress bitcoinAddress);

        [Resource(":system/block-chain/transactions/@getAll")]
        Operation<IEnumerable<BlockChainTransaction>> GetAllUserTransactions();

        [Resource(":system/block-chain/transactions/@getIncoming")]
        Operation<SequencePage<BlockChainTransaction>> GetIncomingUserTransactions(int pageSize, int pageIndex);

        [Resource(":system/block-chain/transactions/@getOutgoing")]
        Operation<SequencePage<BlockChainTransaction>> GetOutgoingUserTransactions(int pageSize, int pageIndex);

        [Resource(":system/block-chain/transactions/@verifyManually")]
        Operation<BlockChainTransaction> VerifyManually(long transactionId);

        [Resource(":system/block-chain/transactions/@verifyManually")]
        Operation<BlockChainTransaction> VerifyManually(string transactionHash);


        [Resource(":system/block-chain/transactions/incoming/@userTotal")]
        Operation<decimal> GetIncomingUserTransactionsTotal();

        [Resource(":system/block-chain/transactions/outgoing/@userTotal")]
        Operation<decimal> GetOutgoingUserTransactionsTotal();

        [Resource(":system/block-chain/transactions/@systemTotal")]
        Operation<decimal> GetSystemTransactionsTotal();
        
        [Resource(":system/block-chain/transactions/@getByHash")]
        Operation VerifyTransaction(string transactionHash, BitLevel currentLevel);
    }
}
