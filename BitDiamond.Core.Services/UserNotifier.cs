using System.Linq;
using System.Collections.Generic;
using Axis.Luna;
using Axis.Pollux.RBAC.Services;
using BitDiamond.Core.Models;
using BitDiamond.Core.Services.Command;
using BitDiamond.Core.Services.Query;

using static Axis.Luna.Extensions.ExceptionExtensions;
using Axis.Luna.Extensions;
using BitDiamond.Core.Utils;

namespace BitDiamond.Core.Services
{
    public class UserNotifier: IUserNotifier, IUserContextAware
    {
        public IUserContext UserContext { get; private set; }
        private IUserNotifierQuery _query;
        private IUserAuthorization _auth;
        private IPersistenceCommands _pcommand;
        private ISettingsManager _settingsManager;
        private IAppUrlProvider _apiProvider;


        #region Init
        public UserNotifier(IUserContext userContext, 
                            IUserNotifierQuery dataContext,
                            ISettingsManager settingsManager,
                            IUserAuthorization accessManager,
                            IPersistenceCommands pcommands,
                            IAppUrlProvider apiProvider)
        {
            ThrowNullArguments(() => userContext,
                               () => dataContext,
                               () => accessManager,
                               () => pcommands,
                               () => settingsManager,
                               () => apiProvider);

            UserContext = userContext;
            _query = dataContext;
            _auth = accessManager;
            _settingsManager = settingsManager;
            _apiProvider = apiProvider;
            _pcommand = pcommands;
        }
        #endregion

        public Operation<Notification> NotifyUser(Notification notification)
        => _auth.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            return notification.Validate()
                               .Then(opr => _pcommand.Add(notification));
        });

        public Operation<Notification> ClearNotification(long notificationId)
        => _auth.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            var notification = _query.GetNotificationById(notificationId)
                                     .ThrowIfNull("Notification not found")
                                     .ThrowIf(_n => _n.Target.UserId != UserContext.CurrentUser().UserId, "Invalid notification found");

            notification.Seen = true;
            return _pcommand.Update(notification);
        });

        public Operation ClearAll()
        => _auth.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            _query.UnseenNotifications(UserContext.CurrentUser())
                  .Select(_n => _pcommand.Update(_n.With(new { Seen = true })))
                  .Any(_n => !_n.Succeeded)
                  .ThrowIf(_any => _any, "Some notifications were not cleared");
        });

        public Operation<IEnumerable<Notification>> NotificationHistory()
        => _auth.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            return _query.NotificationHistory(UserContext.CurrentUser());
        });

        public Operation<IEnumerable<Notification>> UnseenNotifications()
        => _auth.AuthorizeAccess(UserContext.CurrentPPP(), () =>
        {
            return _query.UnseenNotifications(UserContext.CurrentUser());
        });
    }
}
