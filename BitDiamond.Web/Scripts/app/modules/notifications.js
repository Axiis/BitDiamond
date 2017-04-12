var BitDiamond;
(function (BitDiamond) {
    var Modules;
    (function (Modules) {
        Modules.notificationModule = angular.module('notifications', ['ui.router', 'ngSanitize', 'ngAnimate']);
        //directives
        Modules.notificationModule.directive('ringLoader', function () { return new BitDiamond.Directives.RingLoader(); });
        Modules.notificationModule.directive('boxLoader', function () { return new BitDiamond.Directives.BoxLoader(); });
        //services
        Modules.notificationModule.service('__transport', BitDiamond.Utils.Services.DomainTransport);
        Modules.notificationModule.service('__notify', BitDiamond.Utils.Services.NotifyService);
        Modules.notificationModule.service('__userContext', BitDiamond.Utils.Services.UserContext);
        Modules.notificationModule.service('__systemNotification', BitDiamond.Services.Notification);
        Modules.notificationModule.service('__account', BitDiamond.Services.Account);
        //controllers
        Modules.notificationModule.controller('NavBar', BitDiamond.Controllers.Shared.NavBar);
        Modules.notificationModule.controller('SideBar', BitDiamond.Controllers.Shared.SideBar);
        Modules.notificationModule.controller('History', BitDiamond.Controllers.Notification.History);
        Modules.notificationModule.controller('Home', BitDiamond.Controllers.Notification.Details);
        //configure states
        Modules.notificationModule.config(function ($stateProvider, $urlRouterProvider) {
            $urlRouterProvider.otherwise('/history');
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
                .state('base.history', {
                url: '/history',
                templateUrl: '/notifications/history',
                controller: 'History',
                controllerAs: 'vm'
            })
                .state('base.details', {
                url: '/details/',
                params: {
                    notification: {}
                },
                templateUrl: '/notifications/details',
                controller: 'Details',
                controllerAs: 'vm'
            });
        });
    })(Modules = BitDiamond.Modules || (BitDiamond.Modules = {}));
})(BitDiamond || (BitDiamond = {}));
