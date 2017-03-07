using Axis.Luna;
using System;
using System.Linq;
using System.Text;

namespace BitDiamond.Core.Utils
{
    using Axis.Luna.Extensions;
    using System.Net.Mail;

    public static class ReferralHelper
    {
        public static string GenerateCode(string userId)
        {
            if (userId == Constants.SystemUsers_Apex) return userId + "-001";
            else if (!userId.IsEmail()) throw new Exception("invalid userid");
            else
            {
                var parts = userId.Split(new[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                return parts[1]
                    .Pipe(_chars => Encoding.ASCII.GetBytes(_chars.ToArray()))
                    .Aggregate(17, (hash, next) => hash * 283 + next)
                    .Pipe(_hash => $"@{parts[0]}-{Math.Abs(_hash)}");
            }
        }

        public static bool IsEmail(this string email)
        {
            try
            {
                var m = new MailAddress(email);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
