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
        //services
        Modules.profileModule.service('__transport', BitDiamond.Utils.Services.DomainTransport);
        Modules.profileModule.service('__notify', BitDiamond.Utils.Services.NotifyService);
        Modules.profileModule.service('__userContext', BitDiamond.Utils.Services.UserContext);
        Modules.profileModule.service('__systemNotification', BitDiamond.Services.Notification);
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
                .state('dashboard', {
                url: '/dashboard',
                templateUrl: '/profile/dashboard',
                controller: 'Dashboard',
                controllerAs: 'vm'
            })
                .state('home', {
                url: '/home',
                templateUrl: '/profile/home',
                controller: 'Home',
                controllerAs: 'vm'
            });
        });
    })(Modules = BitDiamond.Modules || (BitDiamond.Modules = {}));
})(BitDiamond || (BitDiamond = {}));
//# sourceMappingURL=profile.js.map