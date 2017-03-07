using Axis.Luna;
using Axis.Pollux.RBAC.Auth;
using BitDiamond.Core.Models;
using System.Collections.Generic;

namespace BitDiamond.Core.Services
{
    public interface IUserNotifier
    {
        [Resource(":system/notifications/@notifyUser")]
        Operation<Notification> NotifyUser(Notification notification);

        [Resource(":system/notifications/@clear")]
        Operation<Notification> ClearNotification(long notificationId);

        [Resource(":system/notifications/@clearAll")]
        Operation ClearAll();

        [Resource(":system/notifications/@getHistory")]
        Operation<IEnumerable<Notification>> NotificationHistory();

        [Resource(":system/notifications/@getPagedHistory")]
        Operation<SequencePage<Notification>> PagedNotificationHistory(int pageSize, int pageIndex = 0);

        [Resource(":system/notifications/@getUnseen")]
        Operation<IEnumerable<Notification>> UnseenNotifications();
    }
}
