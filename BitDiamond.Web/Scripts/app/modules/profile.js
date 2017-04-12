var BitDiamond;
(function (BitDiamond) {
    var Modules;
    (function (Modules) {
        Modules.profileModule = angular.module('profile', ['ui.router', 'ngSanitize', 'ngAnimate']);
        //directives
        Modules.profileModule.directive('ringLoader', function () { return new BitDiamond.Directives.RingLoader(); });
        Modules.profileModule.directive('boxLoader', function () { return new BitDiamond.Directives.BoxLoader(); });
        Modules.profileModule.directive('binaryData', function () { return new BitDiamond.Directives.BinaryData(); });
        Modules.profileModule.directive('enumOptions', function () { return new BitDiamond.Directives.EnumOptions(); });
        Modules.profileModule.directive('transactionTimeline', function ($compile, $timeout) { return new BitDiamond.Directives.TransactionTimeline($compile, $timeout); });
        //services
        Modules.profileModule.service('__transport', BitDiamond.Utils.Services.DomainTransport);
        Modules.profileModule.service('__notify', BitDiamond.Utils.Services.NotifyService);
        Modules.profileModule.service('__userContext', BitDiamond.Utils.Services.UserContext);
        Modules.profileModule.service('__systemNotification', BitDiamond.Services.Notification);
        Modules.profileModule.service('__posts', BitDiamond.Services.Posts);
        Modules.profileModule.service('__blockChain', BitDiamond.Services.BlockChain);
        Modules.profileModule.service('__bitLevel', BitDiamond.Services.BitLevel);
        Modules.profileModule.service('__account', BitDiamond.Services.Account);
        //controllers
        Modules.profileModule.controller('NavBar', BitDiamond.Controllers.Shared.NavBar);
        Modules.profileModule.controller('SideBar', BitDiamond.Controllers.Shared.SideBar);
        Modules.profileModule.controller('Dashboard', BitDiamond.Controllers.Profile.Dashboard);
        Modules.profileModule.controller('Home', BitDiamond.Controllers.Profile.Home);
        //configure states
        Modules.profileModule.config(function ($stateProvider, $urlRouterProvider) {
            $urlRouterProvider.otherwise('/dashboard');
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
                .state('base.dashboard', {
                url: '/dashboard',
                templateUrl: '/profile/dashboard',
                controller: 'Dashboard',
                controllerAs: 'vm'
            })
                .state('base.home', {
                url: '/home',
                templateUrl: '/profile/home',
                controller: 'Home',
                controllerAs: 'vm'
            });
        }).run(function ($rootScope, __account) {
            $rootScope.$on('$stateChangeStart', function (e, toState, toParams, fromState, fromParams) {
                __account.validateUserLogon().catch(function (err) {
                    window.location.href = BitDiamond.Utils.Constants.URL_Login;
                });
            });
        });
    })(Modules = BitDiamond.Modules || (BitDiamond.Modules = {}));
})(BitDiamond || (BitDiamond = {}));
