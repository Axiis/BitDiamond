var BitDiamond;
(function (BitDiamond) {
    var Directives;
    (function (Directives) {
        var TransactionTimeline = (function () {
            function TransactionTimeline($compile, $timeout) {
                this.$compile = $compile;
                this.$timeout = $timeout;
                this.scope = {
                    transactions: '=',
                    user: '='
                };
                this._template = '' +
                    '<section class= "cd-horizontal-timeline" ng-hide="transactionGroups.length <=0">' +
                    '<div class="timeline">' +
                    '<div class="events-wrapper">' +
                    '<div class="events">' +
                    '<ol>' +
                    '<li ng-repeat="g in transactionGroups"><a data-date="{{timelineDataString(g.Key)}}" ng-class="{selected:($index==0)}" href>{{shortDate(g.Key)}}</a></li>' +
                    '</ol>' +
                    '<span class="filling-line" aria-hidden="true"></span>' +
                    '</div>' +
                    '</div>' +
                    '<ul class="cd-timeline-navigation">' +
                    '<li><a href class="prev inactive"> Prev </a></li>' +
                    '<li><a href class="next"> Next </a></li>' +
                    '</ul>' +
                    '</div>' +
                    '<div class="events-content">' +
                    '<ol>' +
                    '<li ng-repeat="g in transactionGroups" ng-class="{selected:($index==0)}" data-date="{{timelineDataString(g.Key)}}">' +
                    '<h4 class="box-title"> Transactions </h4>' +
                    '<ul class="feeds">' +
                    '<li ng-repeat="tnx in g.Value">' +
                    '<div class="bg-info">' +
                    '<i ng-class="transactionIconClass(tnx)"> </i>' +
                    '</div> ' +
                    '{{transactionText(tnx)}}' +
                    '<span class="text-muted"> {{transactionTime(tnx) }}</span>' +
                    '</li>' +
                    '</ul>' +
                    '</li>' +
                    '</ol>' +
                    '</div>' +
                    '</section>';
            }
            TransactionTimeline.prototype.link = function (scope, element, attributes) {
                this.setupScope(scope);
                var _link = this.$compile(this._template);
                var activatedDom = _link(scope);
                element.empty().append(activatedDom);
            };
            TransactionTimeline.prototype.setupScope = function (__scope) {
                var _this = this;
                var hasRegisteredDigestListener = false;
                var _initializer = function () {
                    __scope.transactionGroups = (__scope.transactions || []).map(function (_trnx) {
                        _trnx.CreatedOn = new Apollo.Models.JsonDateTime(_trnx.CreatedOn);
                        return _trnx;
                    }).group(function (_trnx) {
                        return _trnx.CreatedOn.toMoment().format('YYYY-M-D');
                    }).sort(function (_a, _b) {
                        var _adate = new Date(_a.Key);
                        var _bdate = new Date(_b.Key);
                        return _adate > _bdate ? 1 : _adate < _bdate ? -1 : 0;
                    });
                    if (hasRegisteredDigestListener)
                        return;
                    else {
                        hasRegisteredDigestListener = true;
                        _this.$timeout(function () {
                            hasRegisteredDigestListener = false;
                            try {
                                Libs.HorizontalTimeline.initTimeline($('.cd-horizontal-timeline'));
                            }
                            catch (e) { }
                        }, 0, false);
                    }
                };
                //Directive Initializers
                __scope.$watch('transactions.length', _initializer);
                __scope.$watch('user', _initializer);
                __scope.shortDate = function (date) {
                    return moment(date).format('MMM D');
                };
                __scope.transactionIconClass = function (tnx) {
                    return {
                        'ti-arrow-right': __scope.isIncomingTransaction(tnx),
                        'text-success': __scope.isIncomingTransaction(tnx),
                        'ti-arrow-left': __scope.isOutgoingTransaction(tnx),
                        'text-danger': __scope.isOutgoingTransaction(tnx)
                    };
                };
                __scope.transactionText = function (tnx) {
                    return (__scope.isIncomingTransaction(tnx) ? 'Received' : 'Sent')
                        + ' <span class="text-' + (__scope.isIncomingTransaction(tnx) ? 'success' : 'danger') + tnx.Amount + '</span>'
                        + (__scope.isIncomingTransaction(tnx) ? ' from ' : ' to ')
                        + (__scope.isIncomingTransaction(tnx) ? tnx.Sender.OwnerId : tnx.Receiver.OwnerId);
                };
                __scope.transactionTime = function (tnx) {
                    return tnx.CreatedOn.toMoment().format('HH:mm');
                };
                __scope.isIncomingTransaction = function (tnx) {
                    return tnx.Receiver.OwnerId == __scope.user.UserId;
                };
                __scope.isOutgoingTransaction = function (tnx) {
                    return tnx.Sender.OwnerId == __scope.user.UserId;
                };
                __scope.timelineDataString = function (strng) {
                    var nstr = strng.replace(/-/g, '/');
                    return nstr;
                };
            };
            return TransactionTimeline;
        }());
        Directives.TransactionTimeline = TransactionTimeline;
    })(Directives = BitDiamond.Directives || (BitDiamond.Directives = {}));
})(BitDiamond || (BitDiamond = {}));
//# sourceMappingURL=transactionTimeline.js.map