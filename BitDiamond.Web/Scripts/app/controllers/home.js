var BitDiamond;
(function (BitDiamond) {
    var Controllers;
    (function (Controllers) {
        var Home;
        (function (Home) {
            var Landing = (function () {
                function Landing(__blockChain, __account, __notification, __notify, $q, $interval) {
                    var _this = this;
                    this.totalUsers = '-';
                    this.totalTransactions = '-';
                    this.__blockChain = __blockChain;
                    this.__account = __account;
                    this.$q = $q;
                    this.$interval = $interval;
                    this.currentYear = moment().format('YYYY');
                    //note that after the initial loading, set a 1 minute timer to refresh the transaction total, but without the boxloader
                    this.isLoadingTransactions = true;
                    this.isLoadingUsers = true;
                    this.$interval(function () {
                        _this.__blockChain.getSystemTransactionTotal().then(function (opr) {
                            _this.totalTransactions = opr.Result.toString();
                            _this.__account.getUserCount().then(function (opr) {
                                _this.totalUsers = opr.Result.toString();
                            }).finally(function () {
                                _this.isLoadingUsers = false;
                            });
                        }).finally(function () {
                            _this.isLoadingTransactions = false;
                        });
                    }, 30000);
                }
                Landing.prototype.sendMessage = function () {
                    var _this = this;
                    //validate message...
                    var hasError = false;
                    if (Object.isNullOrUndefined(this.email) && (hasError = true))
                        this.__notify.error('Your must provide your email addres', 'Oops!');
                    if (Object.isNullOrUndefined(this.subject) && (hasError = true))
                        this.__notify.error('Your must provide a subject', 'Oops!');
                    if (Object.isNullOrUndefined(this.message) && (hasError = true))
                        this.__notify.error('Your must provide a message', 'Oops!');
                    if (hasError)
                        return;
                    this.__notification.notifySupport(this.firstName, this.lastName, this.email, this.subject, this.message).then(function (opr) {
                        _this.__notify.success('Your message was sent to Support.');
                    }, function (err) {
                        _this.__notify.error('Something happened', 'Oops');
                    });
                };
                return Landing;
            }());
            Home.Landing = Landing;
        })(Home = Controllers.Home || (Controllers.Home = {}));
    })(Controllers = BitDiamond.Controllers || (BitDiamond.Controllers = {}));
})(BitDiamond || (BitDiamond = {}));
//# sourceMappingURL=home.js.map