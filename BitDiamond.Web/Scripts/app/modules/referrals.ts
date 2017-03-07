
module BitDiamond.Modules {


    export const referralsModule = angular.module('referrals', ['ui.router', 'ngSanitize', 'ngAnimate']);

    //directives
    referralsModule.directive('ringLoader', () => new BitDiamond.Directives.RingLoader());
    referralsModule.directive('boxLoader', () => new BitDiamond.Directives.BoxLoader());

    //services
    referralsModule.service('__transport', BitDiamond.Utils.Services.DomainTransport);
    referralsModule.service('__dom', BitDiamond.Utils.Services.DomModelService);
    referralsModule.service('__notify', BitDiamond.Utils.Services.NotifyService);
    referralsModule.service('__userContext', BitDiamond.Utils.Services.UserContext);

    referralsModule.service('__account', BitDiamond.Services.Account);
    referralsModule.service('__referrals', BitDiamond.Services.Referrals);
    referralsModule.service('__systemNotification', BitDiamond.Services.Notification);


    //controllers
    referralsModule.controller('NavBar', BitDiamond.Controllers.Shared.NavBar);
    referralsModule.controller('SideBar', BitDiamond.Controllers.Shared.SideBar);

    referralsModule.controller('Downlines', BitDiamond.Controllers.Referrals.Downlines);


    //configure states
    referralsModule.config(($stateProvider: angular.ui.IStateProvider, $urlRouterProvider: angular.ui.IUrlRouterProvider) => {
        $urlRouterProvider.otherwise('/downlines')

        $stateProvider
            .state('downlines', {
                url: '/downlines',
                templateUrl: '/referrals/downlines',
                controller: 'Downlines',
                controllerAs: 'vm'
            });
    });
}