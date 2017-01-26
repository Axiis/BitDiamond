using Axis.Luna;
using BitDiamond.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitDiamond.Core.Services
{
    public interface IContextVerifier: IUserContextAware
    {
        Operation<ContextVerification> CreateVerificationObject(string userId, string verificationContext, DateTime? expiryDate);
        
        Operation VerifyContext(string userId, string verificationContext, string token);
    }
}
