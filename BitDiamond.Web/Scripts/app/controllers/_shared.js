var BitDiamond;
(function (BitDiamond) {
    var Controllers;
    (function (Controllers) {
        var Shared;
        (function (Shared) {
            var Message = (function () {
                function Message($state, $stateParams) {
                    this.$state = $state;
                    this.$stateParams = $stateParams;
                    this.title = this.$stateParams['title'];
                    this.message = this.$stateParams['message'];
                    this.actionTitle = this.$stateParams['actionTitle'];
                }
                Message.prototype.back = function () {
                    this.$state.go(this.$stateParams['action']);
                };
                return Message;
            }());
            Shared.Message = Message;
        })(Shared = Controllers.Shared || (Controllers.Shared = {}));
    })(Controllers = BitDiamond.Controllers || (BitDiamond.Controllers = {}));
})(BitDiamond || (BitDiamond = {}));
//# sourceMappingURL=_shared.js.map