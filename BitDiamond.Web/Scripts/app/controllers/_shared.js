var BitDiamond;
(function (BitDiamond) {
    var Controllers;
    (function (Controllers) {
        var Shared;
        (function (Shared) {
            var Message = (function () {
                function Message($state, $stateParams) {
                    this.$state = $state;
                    this.message = $stateParams['message'];
                    this.title = $stateParams['title'];
                    this.actionState = $stateParams['actionState'];
                    this.actionTitle = $stateParams['actionTitle'];
                    this.hasNoActionTitle = Object.isNullOrUndefined(this.actionTitle);
                }
                Message.prototype.action = function () {
                    if (Object.isNullOrUndefined(this.actionState)) {
                        window.location.href = "/index.html";
                    }
                    else {
                        this.$state.go(this.actionState);
                    }
                };
                return Message;
            }());
            Shared.Message = Message;
            var NavBar = (function () {
                function NavBar() {
                }
                return NavBar;
            }());
            Shared.NavBar = NavBar;
        })(Shared = Controllers.Shared || (Controllers.Shared = {}));
    })(Controllers = BitDiamond.Controllers || (BitDiamond.Controllers = {}));
})(BitDiamond || (BitDiamond = {}));
