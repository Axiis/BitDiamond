using Axis.Jupiter;
using BitDiamond.Core.Services.Query;
using System.Collections.Generic;
using System.Linq;
using Axis.Pollux.Identity.Principal;
using BitDiamond.Core.Models;

using static Axis.Luna.Extensions.ExceptionExtensions;

namespace BitDiamond.Data.EF.Query
{
    public class UserNotifierQuery : IUserNotifierQuery
    {
        private IDataContext _europa = null;

        public UserNotifierQuery(IDataContext context)
        {
            ThrowNullArguments(() => context);

            _europa = context;
        }

        public Notification GetNotificationById(long id)
        => _europa.Store<Notification>().Query.FirstOrDefault(_n => _n.Id == id);

        public IEnumerable<Notification> NotificationHistory(User user)
        => _europa.Store<Notification>()
                  .QueryWith(_n => _n.Target)
                  .Where(_n => _n.Target.EntityId == user.EntityId)
                  .OrderByDescending(_n => _n.CreatedOn)
                  .AsEnumerable();

        public IEnumerable<Notification> UnseenNotifications(User user)
        => _europa.Store<Notification>()
                  .QueryWith(_n => _n.Target)
                  .Where(_n => _n.Target.EntityId == user.EntityId)
                  .Where(_n => _n.Seen == false)
                  .OrderByDescending(_n => _n.CreatedOn)
                  .AsEnumerable();
    }
}
