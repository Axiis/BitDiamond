using Axis.Pollux.Identity.Principal;
using BitDiamond.Core.Models;
using System.Collections.Generic;
using Axis.Luna;

namespace BitDiamond.Core.Services.Query
{
    public interface IBitLevelQuery
    {
        IEnumerable<ReferralNode> Uplines(User user);
        IEnumerable<ReferralNode> Downlines(User user);
        IEnumerable<ReferralNode> Referrals(User user);
        ReferralNode Upline(User user, int uplineOffset);
        ReferralNode UserRef(User user);

        IEnumerable<BitLevel> GetBitLevelHistory(User user);
        SequencePage<BitLevel> GetPagedBitLevelHistory(User user, int pageSize, long pageIndex);
        BitLevel CurrentBitLevel(User user);
        BitLevel PreviousBitLevel(User targetUser);
        BitLevel GetNextUpgradeBeneficiary(User user);
        BitLevel GetBitLevelById(long id);

        BitcoinAddress GetActiveBitcoinAddress(User user);
        BitcoinAddress GetBitcoinAddressById(long id);
        IEnumerable<BitcoinAddress> GetBitcoinAddresses(User user);
        User GetUser(string targetUser);
        BlockChainTransaction GetTransactionWithHash(string transactionHash);
        BitLevel GetBitLevelHavingTransaction(long id);
        BlockChainTransaction GetBlockChainTransaction(long transactionId);
        IEnumerable<BitcoinAddress> GetAllBitcoinAddresses(User user);
        bool AddressExists(string blockChainAddress);
    }
}
