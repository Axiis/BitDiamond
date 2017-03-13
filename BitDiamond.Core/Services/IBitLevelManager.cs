using Axis.Luna;
using Axis.Pollux.RBAC.Auth;
using BitDiamond.Core.Models;
using System.Collections.Generic;

namespace BitDiamond.Core.Services
{
    public interface IBitLevelManager
    {
        [Resource(":system/bit-levels/cycles/@upgrade")]
        Operation<BitLevel> Upgrade();

        [Resource(":system/bit-levels/cycles/@promote")]
        Operation<BitLevel> Promote(string userRef, int units, string securityHash);


        [Resource(":system/bit-levels/transactions/current/@update")]
        Operation<BlockChainTransaction> UpdateTransactionHash(string transactionHash);


        [Resource(":system/bit-levels/transactions/current/@confirm")]
        Operation<BlockChainTransaction> ConfirmUpgradeDonnation();


        [Resource(":system/bit-levels/transactions/current/@get")]
        Operation<BlockChainTransaction> GetCurrentUpgradeTransaction();


        [Resource(":system/bit-levels/cycles/current/@get")]
        Operation<BitLevel> CurrentUserLevel();


        [Resource(":system/bit-levels/cycles/@get")]
        Operation<BitLevel> GetBitLevelById(long id);


        Operation<IEnumerable<BitcoinAddress>> GetAllBitcoinAddresses();

        [Resource(":system/bit-levels/cycles/history/@get")]
        Operation<IEnumerable<BitLevel>> UserUpgradeHistory();

        [Resource(":system/bit-levels/cycles/history/@get-paged")]
        Operation<SequencePage<BitLevel>> PagedUserUpgradeHistory(int pageSize, long pageIndex = 0L);

        [Resource(":system/bit-levels/bitcoin-addresses/active/@get")]
        Operation<BitcoinAddress> GetActiveBitcoinAddress();

        [Resource(":system/bit-levels/bitcoin-addresses/@add")]
        Operation<BitcoinAddress> AddBitcoindAddress(BitcoinAddress address);


        [Resource(":system/bit-levels/upgrade-fees/@get")]
        Operation<decimal> GetUpgradeFee(int level);


        [Resource(":system/bit-levels/bitcoin-addresses/@activate")]
        Operation<BitcoinAddress> ActivateAddress(long v);


        [Resource(":system/bit-levels/bitcoin-addresses/@deactivate")]
        Operation<BitcoinAddress> DeactivateAddress(long v);


        [Resource(":system/bit-levels/bitcoin-addresses/@verify")]
        Operation<BitcoinAddress> VerifyAddress(long v);

        [Resource(":system/bit-levels/referrals/@get")]
        Operation<ReferralNode> GetUserRef(string userId);
    }
}
