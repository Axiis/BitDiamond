using Axis.Luna;
using BitDiamond.Core.Models;
using System.Collections.Generic;

namespace BitDiamond.Core.Services
{
    public interface IUserNotifier
    {
        Operation<Notification> NotifyUser(Notification notification);
        Operation<Notification> ClearNotification(long notificationId);
        Operation ClearAll();

        Operation<IEnumerable<Notification>> NotificationHistory();
        Operation<IEnumerable<Notification>> UnseenNotifications();
    }
}
