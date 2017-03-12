
module BitDiamond.Modules {

    export const accountModule = angular.module('account', ['ui.router', 'ngSanitize', 'ngAnimate']);

    //directives
    accountModule.directive('ringLoader', () => new BitDiamond.Directives.RingLoader());
    accountModule.directive('boxLoader', () => new BitDiamond.Directives.BoxLoader());

    //services
    accountModule.service('__transport', BitDiamond.Utils.Services.DomainTransport);
    accountModule.service('__dom', BitDiamond.Utils.Services.DomModelService);
    accountModule.service('__notify', BitDiamond.Utils.Services.NotifyService);
    accountModule.service('__userContext', BitDiamond.Utils.Services.UserContext);

    accountModule.service('__account', BitDiamond.Services.Account);


    //controllers
    accountModule.controller('Signin', BitDiamond.Controllers.Account.Signin);
    accountModule.controller('Signup', BitDiamond.Controllers.Account.Signup);
    accountModule.controller('RecoveryRequest', BitDiamond.Controllers.Account.RecoveryRequest);
    accountModule.controller('RecoverPassword', BitDiamond.Controllers.Account.RecoverPassword);
    accountModule.controller('Message', BitDiamond.Controllers.Shared.Message);
    accountModule.controller('VerifyRegistration', BitDiamond.Controllers.Account.VerifyRegistration);
    accountModule.controller('Terms', BitDiamond.Controllers.Account.Terms);


    //configure states
    accountModule.config(($stateProvider: angular.ui.IStateProvider, $urlRouterProvider: angular.ui.IUrlRouterProvider) => {
        $urlRouterProvider.otherwise('/signin/')

        $stateProvider
            .state('signin', {
                url: '/signin/:returnUrl',
                templateUrl: '/account/signin', //<-- /account/signin
                controller: 'Signin',
                controllerAs: 'vm'
            })
            .state('signup', {
                url: '/signup',
                templateUrl: '/account/signup', //<-- /account/signup
                controller: 'Signup',
                controllerAs: 'vm'
            })
            .state('passwordRecoveryRequest', {
                url: '/recovery-request',
                templateUrl: '/account/recovery-request', //<-- /account/recovery-request
                controller: 'RecoveryRequest',
                controllerAs: 'vm'
            })
            .state('recoverPassword', {
                url: '/recover-password/:data',
                templateUrl: '/account/recover-password', //<-- /account/recover-password
                controller: 'RecoverPassword',
                controllerAs: 'vm'
            })
            .state('message', {
                url: '/message',
                params: {
                    message: null,
                    title: null,
                    actionState: null,
                    actionTitle: null
                },
                templateUrl: '/account/message', //<-- /account/login-message
                controller: 'Message',
                controllerAs: 'vm'
            })
            .state('verifyRegistration', {
                url: '/verify-registration/:data',
                templateUrl: '/account/verify-registration', //<-- /account/verify-registration
                controller: 'VerifyRegistration',
                controllerAs: 'vm'
            })
            .state('termsAndConditions', {
                url: '/terms-conditions',
                templateUrl: '/account/terms',
                controller: 'Terms',
                controllerAs: 'vm'
            });
    });
}