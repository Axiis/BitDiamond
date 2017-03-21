var BitDiamond;
(function (BitDiamond) {
    var Controllers;
    (function (Controllers) {
        var Notification;
        (function (Notification) {
            var Details = (function () {
                function Details(__systemNotifications, __userContext, __notify, $q) {
                    this.__systemNotifications = __systemNotifications;
                    this.__usercontext = __userContext;
                    this.__notify = __notify;
                    this.$q = $q;
                }
                return Details;
            }());
            Notification.Details = Details;
            var History = (function () {
                function History(__systemNotification, __userContext, __notify, $q) {
                    this.pageSize = 20;
                    this.__systemNotifications = __systemNotification;
                    this.__usercontext = __userContext;
                    this.__notify = __notify;
                    this.$q = $q;
                    //load the initial view
                    this.loadHistory(0, this.pageSize);
                }
                History.prototype.loadHistory = function (index, size) {
                    var _this = this;
                    this.isLoadingView = true;
                    return this.__systemNotifications.getPagedNotificationHistory(index || 0, size || this.pageSize || 30).then(function (opr) {
                        if (!Object.isNullOrUndefined(opr.Result)) {
                            opr.Result.Page = opr.Result.Page.map(function (notif) {
                                notif.CreatedOn = new Apollo.Models.JsonDateTime(notif.CreatedOn);
                                notif.ModifiedOn = new Apollo.Models.JsonDateTime(notif.ModifiedOn);
                                return notif;
                            });
                            _this.notifications = new BitDiamond.Utils.SequencePage(opr.Result.Page, opr.Result.SequenceLength, opr.Result.PageSize, opr.Result.PageIndex);
                        }
                        else {
                            _this.notifications = new BitDiamond.Utils.SequencePage([], 0, 0, 0);
                        }
                        _this.pageLinks = _this.notifications.AdjacentIndexes(2);
                        return _this.$q.resolve(opr.Result);
                    }, function (err) {
                        _this.__notify.error('Something went wrong - couldn\'t load your history.', 'Oops!');
                    }).finally(function () {
                        _this.isLoadingView = false;
                    });
                };
                History.prototype.loadLastPage = function () {
                    this.loadHistory(this.notifications.PageCount - 1, this.pageSize);
                };
                History.prototype.loadFirstPage = function () {
                    this.loadHistory(0, this.pageSize);
                };
                History.prototype.loadLinkPage = function (pageIndex) {
                    this.loadHistory(pageIndex, this.pageSize);
                };
                History.prototype.linkButtonClass = function (page) {
                    return {
                        'btn-outline': page != this.notifications.PageIndex,
                        'btn-default': page != this.notifications.PageIndex,
                        'btn-info': page == this.notifications.PageIndex,
                    };
                };
                History.prototype.displayDate = function (date) {
                    if (Object.isNullOrUndefined(date))
                        return null;
                    else
                        return date.toMoment().format('YYYY/M/D  H:m');
                };
                History.prototype.markerClasses = function (notification) {
                    return {
                        'bd-marker-info': notification.Type == BitDiamond.Models.NotificationType.Info,
                        'bd-marker-success': notification.Type == BitDiamond.Models.NotificationType.Success,
                        'bd-marker-warning': notification.Type == BitDiamond.Models.NotificationType.Warning,
                        'bd-marker-danger': notification.Type == BitDiamond.Models.NotificationType.Error,
                        'active': !notification.Seen
                    };
                };
                History.prototype.iconClasses = function (notification) {
                    return {
                        'text-info': notification.Type == BitDiamond.Models.NotificationType.Info,
                        'text-success': notification.Type == BitDiamond.Models.NotificationType.Success,
                        'text-warning': notification.Type == BitDiamond.Models.NotificationType.Warning,
                        'text-danger': notification.Type == BitDiamond.Models.NotificationType.Error
                    };
                };
                History.prototype.clearAll = function () {
                    var _this = this;
                    this.isClearingAll = true;
                    this.__systemNotifications.clearAll().then(function (opr) {
                        _this.__notify.success('Notifications have been cleared.');
                        _this.loadHistory(_this.notifications.PageIndex, _this.pageSize);
                    }, function (err) {
                        _this.__notify.success('Something went wrong: ' + err.Message, 'Oops!');
                    }).finally(function () {
                        _this.isClearingAll = false;
                    });
                };
                History.prototype.clear = function (notification) {
                    var _this = this;
                    notification['$__isClearing'] = true;
                    this.__systemNotifications.clearNotification(notification.Id).then(function (opr) {
                        _this.__notify.success('Notification was cleared.');
                        notification.Seen = true;
                    }, function (err) {
                        _this.__notify.error('Something went wrong: ' + (err.Message), 'Oops!');
                    }).finally(function () {
                        delete notification['$__isClearing'];
                    });
                };
                return History;
            }());
            Notification.History = History;
        })(Notification = Controllers.Notification || (Controllers.Notification = {}));
    })(Controllers = BitDiamond.Controllers || (BitDiamond.Controllers = {}));
})(BitDiamond || (BitDiamond = {}));
//# sourceMappingURL=notifications.js.map