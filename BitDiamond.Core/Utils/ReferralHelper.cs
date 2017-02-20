using Axis.Luna;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitDiamond.Core.Utils
{
    using RANG = RandomAlphaNumericGenerator;

    public static class ReferralHelper
    {
        public static string GenerateReferenceCode(IEnumerable<string> codes)
        {
            string newCode = null;
            do
            {
                newCode = $"{RANG.RandomAlphaNumeric(5)}-{RANG.RandomAlphaNumeric(5)}-{RANG.RandomAlphaNumeric(10)}";
                newCode = newCode.ToUpper()
                                 .Replace("I", "A").Replace("1", "A")
                                 .Replace("O", "B").Replace("0", "B");
            }
            while (codes.Contains(newCode));

            return newCode;
        }
    }
}
