
module BitDiamond.Modules {

    export const accountModule = angular.module('profile', ['ui.router', 'ngSanitize', 'ngAnimate']);

    //directives
    accountModule.directive('ringLoader', () => new BitDiamond.Directives.RingLoader());
    accountModule.directive('boxLoader', () => new BitDiamond.Directives.BoxLoader());
    //accountModule.directive('binaryData', Gaia.Directives.BinaryData);
    //accountModule.directive('tagsInput', () => new Gaia.Directives.TagsInput());
    //accountModule.directive('numberSpinner', () => new Gaia.Directives.NumberSpinner());
    //accountModule.directive('enumOptions', () => new Gaia.Directives.EnumOptions());
    //accountModule.directive('smallProductCard', ['#gaia.marketPlaceService', '#gaia.utils.notify', '$compile',
    //    (mp: Services.MarketPlaceService, n: Utils.Services.NotifyService, $compile: ng.ICompileService) => new Gaia.Directives.MarketPlace.SmallProductCard(mp, n, $compile)]);
    //accountModule.directive('largeProductCard', ['#gaia.marketPlaceService', '#gaia.utils.notify', '$compile',
    //    (mp: Services.MarketPlaceService, n: Utils.Services.NotifyService, $compile: ng.ICompileService) => new Gaia.Directives.MarketPlace.LargeProductCard(mp, n, $compile)]);


    //services
    accountModule.service('__transport', BitDiamond.Utils.Services.DomainTransport);
    accountModule.service('__dom', BitDiamond.Utils.Services.DomModelService);
    accountModule.service('__notify', BitDiamond.Utils.Services.NotifyService);

    accountModule.service('__account', BitDiamond.Services.Account);


    //controllers
    accountModule.controller('NavBar', BitDiamond.Controllers.Shared.NavBar);
    accountModule.controller('SideBar', BitDiamond.Controllers.Shared.SideBar);

    //accountModule.controller('Signin', BitDiamond.Controllers.Account.Signin);


    //configure states
    accountModule.config(($stateProvider: angular.ui.IStateProvider, $urlRouterProvider: angular.ui.IUrlRouterProvider) => {
        $urlRouterProvider.otherwise('/dashboard/')

        $stateProvider
            .state('dashboard', {
                url: '/dashboard',
                templateUrl: '/profile/dashboard', //<-- /profile/dashboard
                controller: 'dashboard',
                controllerAs: 'vm'
            });
    });
}