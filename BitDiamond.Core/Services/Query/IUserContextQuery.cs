using Axis.Pollux.Identity.Principal;

namespace BitDiamond.Core.Services.Query
{
    public interface IUserContextQuery
    {
        User GetUserById(string userId);
    }
}
