
module BitDiamond.Modules {

    export const notificationModule = angular.module('notifications', ['ui.router', 'ngSanitize', 'ngAnimate']);

    //directives
    notificationModule.directive('ringLoader', () => new BitDiamond.Directives.RingLoader());
    notificationModule.directive('boxLoader', () => new BitDiamond.Directives.BoxLoader());


    //services
    notificationModule.service('__transport', BitDiamond.Utils.Services.DomainTransport);
    notificationModule.service('__notify', BitDiamond.Utils.Services.NotifyService);
    notificationModule.service('__userContext', BitDiamond.Utils.Services.UserContext);

    notificationModule.service('__systemNotification', BitDiamond.Services.Notification);
    notificationModule.service('__account', BitDiamond.Services.Account);


    //controllers
    notificationModule.controller('NavBar', BitDiamond.Controllers.Shared.NavBar);
    notificationModule.controller('SideBar', BitDiamond.Controllers.Shared.SideBar);

    notificationModule.controller('History', BitDiamond.Controllers.Notification.History);
    notificationModule.controller('Home', BitDiamond.Controllers.Notification.Details);


    //configure states
    notificationModule.config(($stateProvider: angular.ui.IStateProvider, $urlRouterProvider: angular.ui.IUrlRouterProvider) => {
        $urlRouterProvider.otherwise('/history')

        $stateProvider
            .state('history', {
                url: '/history',
                templateUrl: '/notifications/history',
                controller: 'History',
                controllerAs: 'vm'
            })
            .state('details', {
                url: '/details/',
                params: {
                    notification: {}
                },
                templateUrl: '/notifications/details',
                controller: 'Details',
                controllerAs: 'vm'
            });
    });
}