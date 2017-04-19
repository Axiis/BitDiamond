using Axis.Pollux.Identity.Principal;
using System.Collections.Generic;

namespace BitDiamond.Core.Services.Query
{
    public interface IUserContextQuery
    {
        User GetUserById(string userId);
        IEnumerable<string> GetUserRoles(string userId);
    }
}
