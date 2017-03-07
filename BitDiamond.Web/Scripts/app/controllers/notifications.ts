
module BitDiamond.Controllers.Notification {

    export class Details {

                
        __systemNotifications: Services.Notification;
        __usercontext: Utils.Services.UserContext;
        __notify: Utils.Services.NotifyService;
        $q: ng.IQService;

        constructor(__systemNotifications, __userContext, __notify, $q) {
            this.__systemNotifications = __systemNotifications;
            this.__usercontext = __userContext;
            this.__notify = __notify;
            this.$q = $q;
        }
    }


    export class History {

        loadHistory(index: number, size: number): ng.IPromise<Utils.SequencePage<Models.INotification>> {
            this.isLoadingView = true;
            return this.__systemNotifications.getPagedNotificationHistory(index || 0, size || this.pageSize || 30).then(opr => {

                if (!Object.isNullOrUndefined(opr.Result)) {
                    opr.Result.Page = opr.Result.Page.map(notif => {
                        notif.CreatedOn = new Apollo.Models.JsonDateTime(notif.CreatedOn);
                        notif.ModifiedOn = new Apollo.Models.JsonDateTime(notif.ModifiedOn);
                        return notif;
                    });
                    this.notifications = new Utils.SequencePage<Models.INotification>(opr.Result.Page, opr.Result.SequenceLength, opr.Result.PageSize, opr.Result.PageIndex);
                }
                else {
                    this.notifications = new Utils.SequencePage<Models.INotification>([], 0, 0, 0);
                }

                this.pageLinks = this.notifications.AdjacentIndexes(2);

                return this.$q.resolve(opr.Result);
            }, err => {
                this.__notify.error('Something went wrong - couldn\'t load your history.', 'Oops!');
            }).finally(() => {
                this.isLoadingView = false;
            });
        }
        loadLastPage() {
            this.loadHistory(this.notifications.PageCount - 1, this.pageSize);
        }
        loadFirstPage() {
            this.loadHistory(0, this.pageSize);
        }
        loadLinkPage(pageIndex: number) {
            this.loadHistory(pageIndex, this.pageSize);
        }
        linkButtonClass(page: number) {
            return {
                'btn-outline': page != this.notifications.PageIndex,
                'btn-default': page != this.notifications.PageIndex,
                'btn-info': page == this.notifications.PageIndex,
            };
        }
        displayDate(date: Apollo.Models.JsonDateTime): string {
            if (Object.isNullOrUndefined(date)) return null;
            else return date.toMoment().format('YYYY/M/D  H:m');
        }
        markerClasses(notification: Models.INotification) {
            return {
                'bd-marker-info': notification.Type == Models.NotificationType.Info,
                'bd-marker-success': notification.Type == Models.NotificationType.Success,
                'bd-marker-warning': notification.Type == Models.NotificationType.Warning,
                'bd-marker-danger': notification.Type == Models.NotificationType.Error,
                'active': !notification.Seen
            };
        }
        iconClasses(notification: Models.INotification) {
            return {
                'text-info': notification.Type == Models.NotificationType.Info,
                'text-success': notification.Type == Models.NotificationType.Success,
                'text-warning': notification.Type == Models.NotificationType.Warning,
                'text-danger': notification.Type == Models.NotificationType.Error
            };
        }

        clearAll() {
            this.isClearingAll = true;
            this.__systemNotifications.clearAll().then(opr => {
                this.__notify.success('Notifications have been cleared.');
                this.loadHistory(this.notifications.PageIndex, this.pageSize);
            }, err => {
                this.__notify.success('Something went wrong: '+err.Message, 'Oops!');
            }).finally(() => {
                this.isClearingAll = false;
            });
        }
        clear(notification: Models.INotification) {
            notification['$__isClearing'] = true;
            this.__systemNotifications.clearNotification(notification.Id).then(opr => {
                this.__notify.success('Notification was cleared.');
                notification.Seen = true;
            }, err => {
                this.__notify.success('Something went wrong: ' + err.Message, 'Oops!');
            }).finally(() => {
                delete notification['$__isClearing'];
            });
        }

        pageLinks: number[];
        pageSize: number = 20;

        isLoadingView: boolean;
        isClearingAll: boolean;
        notifications: Utils.SequencePage<Models.INotification>;

        __systemNotifications: Services.Notification;
        __usercontext: Utils.Services.UserContext;
        __notify: Utils.Services.NotifyService;
        $q: ng.IQService;

        constructor(__systemNotification, __userContext, __notify, $q) {
            this.__systemNotifications = __systemNotification;
            this.__usercontext = __userContext;
            this.__notify = __notify;
            this.$q = $q;

            //load the initial view
            this.loadHistory(0, this.pageSize);
        }
    }

}