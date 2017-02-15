var Pollux;
(function (Pollux) {
    var Services;
    (function (Services) {
        var Authentication = (function () {
            function Authentication($http) {
                this.$http = $http;
            }
            return Authentication;
        }());
        Authentication.$inject = ["$http"];
        Services.Authentication = Authentication;
    })(Services = Pollux.Services || (Pollux.Services = {}));
})(Pollux || (Pollux = {}));
