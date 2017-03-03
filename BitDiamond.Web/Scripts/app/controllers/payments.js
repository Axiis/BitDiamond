var BitDiamond;
(function (BitDiamond) {
    var Controllers;
    (function (Controllers) {
        var Payments;
        (function (Payments) {
            var Incoming = (function () {
                function Incoming() {
                }
                return Incoming;
            }());
            Payments.Incoming = Incoming;
            var Outgoing = (function () {
                function Outgoing() {
                }
                return Outgoing;
            }());
            Payments.Outgoing = Outgoing;
        })(Payments = Controllers.Payments || (Controllers.Payments = {}));
    })(Controllers = BitDiamond.Controllers || (BitDiamond.Controllers = {}));
})(BitDiamond || (BitDiamond = {}));
