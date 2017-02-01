using Axis.Pollux.Identity.Principal;
using BitDiamond.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitDiamond.Core.Services.Query
{
    public interface IAccountQuery
    {
        User GetUserById(string userId);
        IEnumerable<UserLogon> GetUserLogins(string userId);
        IEnumerable<ContextVerification> GetContextVerifications(User user);
        ContextVerification GetLatestContextVerification(User user, string context);
        ContextVerification GetContextVerification(User user, string token);
    }
}
