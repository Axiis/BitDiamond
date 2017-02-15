using Axis.Luna;
using BitDiamond.Core.Models;
using BitDiamond.Core.Utils;
using System;

namespace BitDiamond.Core.Services
{
    public interface IContextVerifier: IUserContextAware
    {
        Operation<ContextVerification> CreateVerificationObject(string userId, string verificationContext, DateTime? expiryDate);
        
        Operation VerifyContext(string userId, string verificationContext, string token);
    }
}
