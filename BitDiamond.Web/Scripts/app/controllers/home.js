var BitDiamond;
(function (BitDiamond) {
    var Controllers;
    (function (Controllers) {
        var Home;
        (function (Home) {
            var Landing = (function () {
                function Landing(__blockChain, __account, __notification, $q) {
                    var _this = this;
                    this.__blockChain = __blockChain;
                    this.__account = __account;
                    this.$q = $q;
                    this.currentYear = moment().format('YYYY');
                    //note that after the initial loading, set a 1minute timer to refresh the user count, but without the boxloader
                    this.isLoadingUsers = true;
                    this.__account.getUserCount().then(function (opr) {
                        _this.totalUsers = opr.Result.toString();
                    }, function (err) {
                        _this.totalUsers = '-';
                    }).finally(function () { return _this.isLoadingUsers = false; });
                    //note that after the initial loading, set a 1 minute timer to refresh the transaction total, but without the boxloader
                    this.isLoadingTransactions = true;
                    this.__blockChain.getSystemTransactionTotal().then(function (opr) {
                        _this.totalTransactions = opr.Result.toString();
                    }, function (err) {
                        _this.totalTransactions = '-';
                    }).finally(function () { return _this.isLoadingTransactions = false; });
                }
                Landing.prototype.sendMessage = function () {
                    //validate message...
                    this.__notification.notifySupport(this.firstName, this.lastName, this.email, this.subject, this.message).then(function (opr) {
                    }, function (err) {
                    });
                };
                return Landing;
            }());
            Home.Landing = Landing;
        })(Home = Controllers.Home || (Controllers.Home = {}));
    })(Controllers = BitDiamond.Controllers || (BitDiamond.Controllers = {}));
})(BitDiamond || (BitDiamond = {}));
