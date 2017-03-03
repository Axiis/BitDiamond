var BitDiamond;
(function (BitDiamond) {
    var Modules;
    (function (Modules) {
        Modules.bitlevelModules = angular.module('bitlevel', ['ui.router', 'ngSanitize', 'ngAnimate']);
        //directives
        Modules.bitlevelModules.directive('ringLoader', function () { return new BitDiamond.Directives.RingLoader(); });
        Modules.bitlevelModules.directive('boxLoader', function () { return new BitDiamond.Directives.BoxLoader(); });
        Modules.bitlevelModules.directive('binaryData', function () { return new BitDiamond.Directives.BinaryData(); });
        Modules.bitlevelModules.directive('enumOptions', function () { return new BitDiamond.Directives.EnumOptions(); });
        //bitlevelModules.directive('tagsInput', () => new Gaia.Directives.TagsInput());
        //bitlevelModules.directive('numberSpinner', () => new Gaia.Directives.NumberSpinner());
        //bitlevelModules.directive('smallProductCard', ['#gaia.marketPlaceService', '#gaia.utils.notify', '$compile',
        //    (mp: Services.MarketPlaceService, n: Utils.Services.NotifyService, $compile: ng.ICompileService) => new Gaia.Directives.MarketPlace.SmallProductCard(mp, n, $compile)]);
        //bitlevelModules.directive('largeProductCard', ['#gaia.marketPlaceService', '#gaia.utils.notify', '$compile',
        //    (mp: Services.MarketPlaceService, n: Utils.Services.NotifyService, $compile: ng.ICompileService) => new Gaia.Directives.MarketPlace.LargeProductCard(mp, n, $compile)]);
        //services
        Modules.bitlevelModules.service('__transport', BitDiamond.Utils.Services.DomainTransport);
        Modules.bitlevelModules.service('__dom', BitDiamond.Utils.Services.DomModelService);
        Modules.bitlevelModules.service('__notify', BitDiamond.Utils.Services.NotifyService);
        Modules.bitlevelModules.service('__userContext', BitDiamond.Utils.Services.UserContext);
        Modules.bitlevelModules.service('__bitlevel', BitDiamond.Services.BitLevel);
        Modules.bitlevelModules.service('__account', BitDiamond.Services.Account);
        //controllers
        Modules.bitlevelModules.controller('NavBar', BitDiamond.Controllers.Shared.NavBar);
        Modules.bitlevelModules.controller('SideBar', BitDiamond.Controllers.Shared.SideBar);
        Modules.bitlevelModules.controller('History', BitDiamond.Controllers.BitLevel.History);
        Modules.bitlevelModules.controller('Home', BitDiamond.Controllers.BitLevel.Home);
        Modules.bitlevelModules.controller('BitcoinAddresses', BitDiamond.Controllers.BitLevel.BitcoinAddresses);
        //configure states
        Modules.bitlevelModules.config(function ($stateProvider, $urlRouterProvider) {
            $urlRouterProvider.otherwise('/home');
            $stateProvider
                .state('home', {
                url: '/home',
                templateUrl: '/bit-level/home',
                controller: 'Home',
                controllerAs: 'vm'
            })
                .state('bitcoinAddresses', {
                url: '/bitcoin-addresses',
                templateUrl: '/bit-level/bitcoin-addresses',
                controller: 'BitcoinAddresses',
                controllerAs: 'vm'
            })
                .state('history', {
                url: '/history',
                templateUrl: '/bit-level/history',
                controller: 'History',
                controllerAs: 'vm'
            });
        });
    })(Modules = BitDiamond.Modules || (BitDiamond.Modules = {}));
})(BitDiamond || (BitDiamond = {}));
