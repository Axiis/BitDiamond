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
        Operation<string> GenerateContextVerificationApiUrl(string verificationToken, string user);
        Operation<string> GenerateBlobUrl(string blobName);
    }
}
