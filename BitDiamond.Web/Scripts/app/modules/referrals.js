var BitDiamond;
(function (BitDiamond) {
    var Modules;
    (function (Modules) {
        Modules.referralsModule = angular.module('referrals', ['ui.router', 'ngSanitize', 'ngAnimate']);
        //directives
        Modules.referralsModule.directive('ringLoader', function () { return new BitDiamond.Directives.RingLoader(); });
        Modules.referralsModule.directive('boxLoader', function () { return new BitDiamond.Directives.BoxLoader(); });
        //services
        Modules.referralsModule.service('__transport', BitDiamond.Utils.Services.DomainTransport);
        Modules.referralsModule.service('__dom', BitDiamond.Utils.Services.DomModelService);
        Modules.referralsModule.service('__notify', BitDiamond.Utils.Services.NotifyService);
        Modules.referralsModule.service('__userContext', BitDiamond.Utils.Services.UserContext);
        Modules.referralsModule.service('__account', BitDiamond.Services.Account);
        Modules.referralsModule.service('__referrals', BitDiamond.Services.Referrals);
        Modules.referralsModule.service('__systemNotification', BitDiamond.Services.Notification);
        //controllers
        Modules.referralsModule.controller('NavBar', BitDiamond.Controllers.Shared.NavBar);
        Modules.referralsModule.controller('SideBar', BitDiamond.Controllers.Shared.SideBar);
        Modules.referralsModule.controller('Downlines', BitDiamond.Controllers.Referrals.Downlines);
        //configure states
        Modules.referralsModule.config(function ($stateProvider, $urlRouterProvider) {
            $urlRouterProvider.otherwise('/downlines');
            $stateProvider
                .state('downlines', {
                url: '/downlines',
                templateUrl: '/referrals/downlines',
                controller: 'Downlines',
                controllerAs: 'vm'
            });
        });
    })(Modules = BitDiamond.Modules || (BitDiamond.Modules = {}));
})(BitDiamond || (BitDiamond = {}));
