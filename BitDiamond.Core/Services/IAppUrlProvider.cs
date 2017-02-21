using Axis.Luna;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitDiamond.Core.Services
{
    public interface IAppUrlProvider
    {
        Operation<string> GeneratePasswordUpdateVerificationUrl(string verificationToken, string user);
        Operation<string> GenerateUserActivationVerificationUrl(string verificationToken, string user);
        Operation<string> GenerateBlobUrl(string blobName);
    }
}
