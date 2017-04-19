using BitDiamond.Core.Services.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Axis.Pollux.Identity.Principal;
using Axis.Jupiter;

using static Axis.Luna.Extensions.ExceptionExtensions;
using Axis.Pollux.RBAC.Auth;

namespace BitDiamond.Data.EF.Query
{
    public class UserContextQuery : IUserContextQuery
    {
        private IDataContext _europa = null;

        public UserContextQuery(IDataContext context)
        {
            ThrowNullArguments(() => context);

            _europa = context;
        }

        public User GetUserById(string userId)
        => _europa.Store<User>().Query.FirstOrDefault(_user => _user.EntityId == userId);

        public IEnumerable<string> GetUserRoles(string userId)
        => _europa
            .Store<UserRole>().Query
            .Where(_ur => _ur.UserId == userId)
            .Select(_ur => _ur.RoleName)
            .ToArray();
    }
}
