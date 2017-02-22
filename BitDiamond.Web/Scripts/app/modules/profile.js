var BitDiamond;
(function (BitDiamond) {
    var Modules;
    (function (Modules) {
        Modules.accountModule = angular.module('profile', ['ui.router', 'ngSanitize', 'ngAnimate']);
        //directives
        Modules.accountModule.directive('ringLoader', function () { return new BitDiamond.Directives.RingLoader(); });
        Modules.accountModule.directive('boxLoader', function () { return new BitDiamond.Directives.BoxLoader(); });
        //accountModule.directive('binaryData', Gaia.Directives.BinaryData);
        //accountModule.directive('tagsInput', () => new Gaia.Directives.TagsInput());
        //accountModule.directive('numberSpinner', () => new Gaia.Directives.NumberSpinner());
        //accountModule.directive('enumOptions', () => new Gaia.Directives.EnumOptions());
        //accountModule.directive('smallProductCard', ['#gaia.marketPlaceService', '#gaia.utils.notify', '$compile',
        //    (mp: Services.MarketPlaceService, n: Utils.Services.NotifyService, $compile: ng.ICompileService) => new Gaia.Directives.MarketPlace.SmallProductCard(mp, n, $compile)]);
        //accountModule.directive('largeProductCard', ['#gaia.marketPlaceService', '#gaia.utils.notify', '$compile',
        //    (mp: Services.MarketPlaceService, n: Utils.Services.NotifyService, $compile: ng.ICompileService) => new Gaia.Directives.MarketPlace.LargeProductCard(mp, n, $compile)]);
        //services
        Modules.accountModule.service('__transport', BitDiamond.Utils.Services.DomainTransport);
        Modules.accountModule.service('__dom', BitDiamond.Utils.Services.DomModelService);
        Modules.accountModule.service('__notify', BitDiamond.Utils.Services.NotifyService);
        Modules.accountModule.service('__account', BitDiamond.Services.Account);
        //controllers
        Modules.accountModule.controller('NavBar', BitDiamond.Controllers.Shared.NavBar);
        Modules.accountModule.controller('SideBar', BitDiamond.Controllers.Shared.SideBar);
        //accountModule.controller('Signin', BitDiamond.Controllers.Account.Signin);
        //configure states
        Modules.accountModule.config(function ($stateProvider, $urlRouterProvider) {
            $urlRouterProvider.otherwise('/dashboard/');
            $stateProvider
                .state('dashboard', {
                url: '/dashboard',
                templateUrl: '/profile/dashboard',
                controller: 'dashboard',
                controllerAs: 'vm'
            });
        });
    })(Modules = BitDiamond.Modules || (BitDiamond.Modules = {}));
})(BitDiamond || (BitDiamond = {}));
