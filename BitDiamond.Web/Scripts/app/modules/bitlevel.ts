
module BitDiamond.Modules {

    export const bitlevelModules = angular.module('bitlevel', ['ui.router', 'ngSanitize', 'ngAnimate']);

    //directives
    bitlevelModules.directive('ringLoader', () => new BitDiamond.Directives.RingLoader());
    bitlevelModules.directive('boxLoader', () => new BitDiamond.Directives.BoxLoader());
    bitlevelModules.directive('binaryData', () => new BitDiamond.Directives.BinaryData());
    bitlevelModules.directive('enumOptions', () => new BitDiamond.Directives.EnumOptions());


    //services
    bitlevelModules.service('__transport', BitDiamond.Utils.Services.DomainTransport);
    bitlevelModules.service('__dom', BitDiamond.Utils.Services.DomModelService);
    bitlevelModules.service('__notify', BitDiamond.Utils.Services.NotifyService);
    bitlevelModules.service('__userContext', BitDiamond.Utils.Services.UserContext);

    bitlevelModules.service('__bitlevel', BitDiamond.Services.BitLevel);
    bitlevelModules.service('__account', BitDiamond.Services.Account);
    bitlevelModules.service('__systemNotification', BitDiamond.Services.Notification);


    //controllers
    bitlevelModules.controller('NavBar', BitDiamond.Controllers.Shared.NavBar);
    bitlevelModules.controller('SideBar', BitDiamond.Controllers.Shared.SideBar);

    bitlevelModules.controller('History', BitDiamond.Controllers.BitLevel.History);
    bitlevelModules.controller('Home', BitDiamond.Controllers.BitLevel.Home);
    bitlevelModules.controller('BitcoinAddresses', BitDiamond.Controllers.BitLevel.BitcoinAddresses);


    //configure states
    bitlevelModules.config(($stateProvider: angular.ui.IStateProvider, $urlRouterProvider: angular.ui.IUrlRouterProvider) => {
        $urlRouterProvider.otherwise('/home')

        $stateProvider
            .state('base', {
                abstract: true,
                views: {
                    'sidebar': {
                        templateUrl: '/templates/common/sidebar.html',
                        controller: 'SideBar',
                        controllerAs: 'vm'
                    },
                    'navbar': {
                        templateUrl: '/templates/common/navbar.html',
                        controller: 'NavBar',
                        controllerAs: 'vm'
                    },
                    'content': {
                        template: '<ui-view/>'
                    }
                }
            })
            .state('base.home', {
                url: '/home',
                templateUrl: '/bit-level/home', //<-- /bit-level/home
                controller: 'Home',
                controllerAs: 'vm'
            })
            .state('base.bitcoinAddresses', {
                url: '/bitcoin-addresses',
                templateUrl: '/bit-level/bitcoin-addresses', //<-- /bit-level/bitcoin-addresses
                controller: 'BitcoinAddresses',
                controllerAs: 'vm'
            })
            .state('base.history', {
                url: '/history',
                templateUrl: '/bit-level/history', //<-- /bit-level/history
                controller: 'History',
                controllerAs: 'vm'
            });
    });
}