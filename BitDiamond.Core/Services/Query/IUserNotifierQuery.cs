using Axis.Pollux.Identity.Principal;
using BitDiamond.Core.Models;
using System.Collections.Generic;

namespace BitDiamond.Core.Services.Query
{
    public interface IUserNotifierQuery
    {
        Notification GetNotificationById(long id);

        IEnumerable<Notification> NotificationHistory(User user);

        IEnumerable<Notification> UnseenNotifications(User user);
    }
}
