var BitDiamond;
(function (BitDiamond) {
    var Modules;
    (function (Modules) {
        Modules.paymentsModules = angular.module('payments', ['ui.router', 'ngSanitize', 'ngAnimate']);
        //directives
        Modules.paymentsModules.directive('ringLoader', function () { return new BitDiamond.Directives.RingLoader(); });
        Modules.paymentsModules.directive('boxLoader', function () { return new BitDiamond.Directives.BoxLoader(); });
        Modules.paymentsModules.directive('binaryData', function () { return new BitDiamond.Directives.BinaryData(); });
        Modules.paymentsModules.directive('enumOptions', function () { return new BitDiamond.Directives.EnumOptions(); });
        //services
        Modules.paymentsModules.service('__transport', BitDiamond.Utils.Services.DomainTransport);
        Modules.paymentsModules.service('__dom', BitDiamond.Utils.Services.DomModelService);
        Modules.paymentsModules.service('__notify', BitDiamond.Utils.Services.NotifyService);
        Modules.paymentsModules.service('__userContext', BitDiamond.Utils.Services.UserContext);
        Modules.paymentsModules.service('__bitlevel', BitDiamond.Services.BitLevel);
        Modules.paymentsModules.service('__blockChain', BitDiamond.Services.BlockChain);
        Modules.paymentsModules.service('__account', BitDiamond.Services.Account);
        //controllers
        Modules.paymentsModules.controller('NavBar', BitDiamond.Controllers.Shared.NavBar);
        Modules.paymentsModules.controller('SideBar', BitDiamond.Controllers.Shared.SideBar);
        Modules.paymentsModules.controller('Incoming', BitDiamond.Controllers.Payments.Incoming);
        Modules.paymentsModules.controller('Outgoing', BitDiamond.Controllers.Payments.Outgoing);
        //configure states
        Modules.paymentsModules.config(function ($stateProvider, $urlRouterProvider) {
            $urlRouterProvider.otherwise('/incoming');
            $stateProvider
                .state('incoming', {
                url: '/incoming',
                templateUrl: '/payments/incoming',
                controller: 'Incoming',
                controllerAs: 'vm'
            })
                .state('outgoing', {
                url: '/outgoing',
                templateUrl: '/payments/outgoing',
                controller: 'Outgoing',
                controllerAs: 'vm'
            });
        });
    })(Modules = BitDiamond.Modules || (BitDiamond.Modules = {}));
})(BitDiamond || (BitDiamond = {}));
//# sourceMappingURL=payments.js.map