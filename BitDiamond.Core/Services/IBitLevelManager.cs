using Axis.Luna;
using BitDiamond.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitDiamond.Core.Services
{
    public interface IBitLevelManager
    {
        Operation<BitLevel> GenerateUpgradeDonation();
        Operation VerifyDonation(string transactionHash);
        Operation ManualDonationVerification();

        Operation<IEnumerable<BitLevel>> UserDonations();
        Operation<BitLevel> UserDonationById(long id);
        Operation<BitLevel> LatestUserDonation();
    }
}
