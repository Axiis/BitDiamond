using BitDiamond.Core.Services.Query;
using System;
using System.Linq;
using Axis.Pollux.Identity.Principal;
using BitDiamond.Core.Models;
using Axis.Jupiter;

using static Axis.Luna.Extensions.ExceptionExtensions;

namespace BitDiamond.Data.EF.Query
{
    public class ContextVerifierQuery: IContextVerifierQuery
    {
        private IDataContext _europa = null;

        public ContextVerifierQuery(IDataContext context)
        {
            ThrowNullArguments(() => context);

            _europa = context;
        }

        public ContextVerification GetContextVerification(string userId, string context, string token)
        => _europa.Store<ContextVerification>()
                  .QueryWith(_cv => _cv.Target)
                  .Where(_cv => _cv.Target.EntityId == userId)
                  .Where(_cv => _cv.Context == context)
                  .Where(_cv => _cv.VerificationToken == token)
                  .FirstOrDefault();

        public User GetUserById(string userId)
        => _europa.Store<User>().Query.FirstOrDefault(_u => _u.EntityId == userId);
    }
}
