using Axis.Luna;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitDiamond.Core.Services
{
    public interface IApiLinkProvider
    {
        Operation<string> GenerateContextVerificationLink(string verificationToken);
    }
}
