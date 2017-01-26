using Axis.Luna;
using BitDiamond.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitDiamond.Core.Services
{
    public interface IDonationManager
    {
        Operation<Donation> GenerateUpgradeDonation();
        Operation VerifyDonation(string transactionHash);
        Operation ManualDonationVerification();

        Operation<IEnumerable<Donation>> UserDonations();
        Operation<Donation> UserDonationById(long id);
        Operation<Donation> LatestUserDonation();
    }
}
