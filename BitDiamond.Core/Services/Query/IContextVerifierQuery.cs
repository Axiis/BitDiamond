using Axis.Pollux.Identity.Principal;
using BitDiamond.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitDiamond.Core.Services.Query
{
    public interface IContextVerifierQuery
    {
        User GetUserById(string userId);
        ContextVerification GetContextVerification(string userId, string context, string token);
    }
}
