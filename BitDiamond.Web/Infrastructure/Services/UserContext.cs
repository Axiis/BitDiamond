using BitDiamond.Core.Services;
using Axis.Pollux.Identity.Principal;
using static Axis.Luna.Extensions.ExceptionExtensions;
using BitDiamond.Core.Services.Query;
using BitDiamond.Core.Utils;

namespace BitDiamond.Web.Infrastructure.Services
{
    public class UserContext : IUserContext
    {
        private OwinContextProvider _owinProvider = null;
        private IUserContextQuery _query = null;

        public UserContext(OwinContextProvider owinProvider, IUserContextQuery query)
        {
            ThrowNullArguments(() => owinProvider,
                               () => query);

            this._owinProvider = owinProvider;
            this._query = query;
        }

        private User _user = null;
        public User CurrentUser()
        {
            if (_user != null) return _user;

            else if (_owinProvider.Owin.Request.User?.Identity?.Name == null)
                return _user = _query.GetUserById(Constants.SystemUsers_Guest);

            else
                return _user = _query.GetUserById(_owinProvider.Owin.Request.User.Identity.Name);
        }
    }
}