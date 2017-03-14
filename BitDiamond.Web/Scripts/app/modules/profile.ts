
module BitDiamond.Modules {

    export const profileModule = angular.module('profile', ['ui.router', 'ngSanitize', 'ngAnimate']);

    //directives
    profileModule.directive('ringLoader', () => new BitDiamond.Directives.RingLoader());
    profileModule.directive('boxLoader', () => new BitDiamond.Directives.BoxLoader());
    profileModule.directive('binaryData', () => new BitDiamond.Directives.BinaryData());
    profileModule.directive('enumOptions', () => new BitDiamond.Directives.EnumOptions());
    profileModule.directive('transactionTimeline', ($compile: ng.ICompileService) => new BitDiamond.Directives.TransactionTimeline($compile));

    //services
    profileModule.service('__transport', BitDiamond.Utils.Services.DomainTransport);
    profileModule.service('__notify', BitDiamond.Utils.Services.NotifyService);
    profileModule.service('__userContext', BitDiamond.Utils.Services.UserContext);
    profileModule.service('__systemNotification', BitDiamond.Services.Notification);
    profileModule.service('__blockChain', BitDiamond.Services.BlockChain);

    profileModule.service('__account', BitDiamond.Services.Account);


    //controllers
    profileModule.controller('NavBar', BitDiamond.Controllers.Shared.NavBar);
    profileModule.controller('SideBar', BitDiamond.Controllers.Shared.SideBar);

    profileModule.controller('Dashboard', BitDiamond.Controllers.Profile.Dashboard);
    profileModule.controller('Home', BitDiamond.Controllers.Profile.Home);


    //configure states
    profileModule.config(($stateProvider: angular.ui.IStateProvider, $urlRouterProvider: angular.ui.IUrlRouterProvider) => {
        $urlRouterProvider.otherwise('/dashboard')

        $stateProvider
            .state('dashboard', {
                url: '/dashboard',
                templateUrl: '/profile/dashboard', //<-- /profile/dashboard
                controller: 'Dashboard',
                controllerAs: 'vm'
            })
            .state('home', {
                url: '/home',
                templateUrl: '/profile/home', //<-- /profile/home
                controller: 'Home',
                controllerAs: 'vm'
            });
    });
}