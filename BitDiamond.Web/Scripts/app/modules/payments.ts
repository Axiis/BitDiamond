
module BitDiamond.Modules {

    export const paymentsModules = angular.module('payments', ['ui.router', 'ngSanitize', 'ngAnimate']);

    //directives
    paymentsModules.directive('ringLoader', () => new BitDiamond.Directives.RingLoader());
    paymentsModules.directive('boxLoader', () => new BitDiamond.Directives.BoxLoader());
    paymentsModules.directive('binaryData', () => new BitDiamond.Directives.BinaryData());
    paymentsModules.directive('enumOptions', () => new BitDiamond.Directives.EnumOptions());


    //services
    paymentsModules.service('__transport', BitDiamond.Utils.Services.DomainTransport);
    paymentsModules.service('__dom', BitDiamond.Utils.Services.DomModelService);
    paymentsModules.service('__notify', BitDiamond.Utils.Services.NotifyService);
    paymentsModules.service('__userContext', BitDiamond.Utils.Services.UserContext);

    paymentsModules.service('__bitlevel', BitDiamond.Services.BitLevel);
    paymentsModules.service('__blockChain', BitDiamond.Services.BlockChain);
    paymentsModules.service('__account', BitDiamond.Services.Account);


    //controllers
    paymentsModules.controller('NavBar', BitDiamond.Controllers.Shared.NavBar);
    paymentsModules.controller('SideBar', BitDiamond.Controllers.Shared.SideBar);

    paymentsModules.controller('Incoming', BitDiamond.Controllers.Payments.Incoming);
    paymentsModules.controller('Outgoing', BitDiamond.Controllers.Payments.Outgoing);


    //configure states
    paymentsModules.config(($stateProvider: angular.ui.IStateProvider, $urlRouterProvider: angular.ui.IUrlRouterProvider) => {
        $urlRouterProvider.otherwise('/incoming')

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
}