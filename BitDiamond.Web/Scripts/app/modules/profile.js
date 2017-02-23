var BitDiamond;
(function (BitDiamond) {
    var Modules;
    (function (Modules) {
        Modules.profileModule = angular.module('profile', ['ui.router', 'ngSanitize', 'ngAnimate']);
        //directives
        Modules.profileModule.directive('ringLoader', function () { return new BitDiamond.Directives.RingLoader(); });
        Modules.profileModule.directive('boxLoader', function () { return new BitDiamond.Directives.BoxLoader(); });
        //accountModule.directive('binaryData', Gaia.Directives.BinaryData);
        //accountModule.directive('tagsInput', () => new Gaia.Directives.TagsInput());
        //accountModule.directive('numberSpinner', () => new Gaia.Directives.NumberSpinner());
        //accountModule.directive('enumOptions', () => new Gaia.Directives.EnumOptions());
        //accountModule.directive('smallProductCard', ['#gaia.marketPlaceService', '#gaia.utils.notify', '$compile',
        //    (mp: Services.MarketPlaceService, n: Utils.Services.NotifyService, $compile: ng.ICompileService) => new Gaia.Directives.MarketPlace.SmallProductCard(mp, n, $compile)]);
        //accountModule.directive('largeProductCard', ['#gaia.marketPlaceService', '#gaia.utils.notify', '$compile',
        //    (mp: Services.MarketPlaceService, n: Utils.Services.NotifyService, $compile: ng.ICompileService) => new Gaia.Directives.MarketPlace.LargeProductCard(mp, n, $compile)]);
        //services
        Modules.profileModule.service('__transport', BitDiamond.Utils.Services.DomainTransport);
        Modules.profileModule.service('__dom', BitDiamond.Utils.Services.DomModelService);
        Modules.profileModule.service('__notify', BitDiamond.Utils.Services.NotifyService);
        Modules.profileModule.service('__account', BitDiamond.Services.Account);
        //controllers
        Modules.profileModule.controller('NavBar', BitDiamond.Controllers.Shared.NavBar);
        Modules.profileModule.controller('SideBar', BitDiamond.Controllers.Shared.SideBar);
        Modules.profileModule.controller('Dashboard', BitDiamond.Controllers.Profile.Dashboard);
        //configure states
        Modules.profileModule.config(function ($stateProvider, $urlRouterProvider) {
            $urlRouterProvider.otherwise('/dashboard');
            $stateProvider
                .state('dashboard', {
                url: '/dashboard',
                templateUrl: '/profile/dashboard',
                controller: 'Dashboard',
                controllerAs: 'vm'
            });
        });
    })(Modules = BitDiamond.Modules || (BitDiamond.Modules = {}));
})(BitDiamond || (BitDiamond = {}));
//# sourceMappingURL=profile.js.map