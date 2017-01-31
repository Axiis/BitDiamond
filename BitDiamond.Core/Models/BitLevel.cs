using Axis.Pollux.Identity.Principal;
using System.ComponentModel.DataAnnotations;

namespace BitDiamond.Core.Models
{
    /// <summary>
    /// </summary>
    public class BitLevel: BaseModel<long>
    {
        public BlockChainTransaction Donation { get; set; } = new BlockChainTransaction();


        [Range(0, int.MaxValue, ErrorMessage = "level is invalid")]
        public int Level { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "skip-count is invalid")]
        public int SkipCount { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "donation-count is invalid")]
        public int DonationCount { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "cycle is invalid")]
        public int Cycle { get; set; }

        [Required(ErrorMessage = "User is required")]
        public User User { get; set; }
    }
}
