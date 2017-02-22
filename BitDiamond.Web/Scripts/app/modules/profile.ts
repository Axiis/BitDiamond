
module BitDiamond.Modules {

    export const profileModule = angular.module('profile', ['ui.router', 'ngSanitize', 'ngAnimate']);

    //directives
    profileModule.directive('ringLoader', () => new BitDiamond.Directives.RingLoader());
    profileModule.directive('boxLoader', () => new BitDiamond.Directives.BoxLoader());
    //accountModule.directive('binaryData', Gaia.Directives.BinaryData);
    //accountModule.directive('tagsInput', () => new Gaia.Directives.TagsInput());
    //accountModule.directive('numberSpinner', () => new Gaia.Directives.NumberSpinner());
    //accountModule.directive('enumOptions', () => new Gaia.Directives.EnumOptions());
    //accountModule.directive('smallProductCard', ['#gaia.marketPlaceService', '#gaia.utils.notify', '$compile',
    //    (mp: Services.MarketPlaceService, n: Utils.Services.NotifyService, $compile: ng.ICompileService) => new Gaia.Directives.MarketPlace.SmallProductCard(mp, n, $compile)]);
    //accountModule.directive('largeProductCard', ['#gaia.marketPlaceService', '#gaia.utils.notify', '$compile',
    //    (mp: Services.MarketPlaceService, n: Utils.Services.NotifyService, $compile: ng.ICompileService) => new Gaia.Directives.MarketPlace.LargeProductCard(mp, n, $compile)]);


    //services
    profileModule.service('__transport', BitDiamond.Utils.Services.DomainTransport);
    profileModule.service('__dom', BitDiamond.Utils.Services.DomModelService);
    profileModule.service('__notify', BitDiamond.Utils.Services.NotifyService);

    profileModule.service('__account', BitDiamond.Services.Account);


    //controllers
    profileModule.controller('NavBar', BitDiamond.Controllers.Shared.NavBar);
    profileModule.controller('SideBar', BitDiamond.Controllers.Shared.SideBar);

    profileModule.controller('Dashboard', BitDiamond.Controllers.Profile.Dashboard);


    //configure states
    profileModule.config(($stateProvider: angular.ui.IStateProvider, $urlRouterProvider: angular.ui.IUrlRouterProvider) => {
        $urlRouterProvider.otherwise('/dashboard')

        $stateProvider
            .state('dashboard', {
                url: '/dashboard',
                templateUrl: '/profile/dashboard', //<-- /profile/dashboard
                controller: 'Dashboard',
                controllerAs: 'vm'
            });
    });
}