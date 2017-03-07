var BitDiamond;
(function (BitDiamond) {
    var Modules;
    (function (Modules) {
        Modules.accountModule = angular.module('account', ['ui.router', 'ngSanitize', 'ngAnimate']);
        //directives
        Modules.accountModule.directive('ringLoader', function () { return new BitDiamond.Directives.RingLoader(); });
        Modules.accountModule.directive('boxLoader', function () { return new BitDiamond.Directives.BoxLoader(); });
        //services
        Modules.accountModule.service('__transport', BitDiamond.Utils.Services.DomainTransport);
        Modules.accountModule.service('__dom', BitDiamond.Utils.Services.DomModelService);
        Modules.accountModule.service('__notify', BitDiamond.Utils.Services.NotifyService);
        Modules.accountModule.service('__userContext', BitDiamond.Utils.Services.UserContext);
        Modules.accountModule.service('__account', BitDiamond.Services.Account);
        //controllers
        Modules.accountModule.controller('Signin', BitDiamond.Controllers.Account.Signin);
        Modules.accountModule.controller('Signup', BitDiamond.Controllers.Account.Signup);
        Modules.accountModule.controller('RecoveryRequest', BitDiamond.Controllers.Account.RecoveryRequest);
        Modules.accountModule.controller('RecoverPassword', BitDiamond.Controllers.Account.RecoverPassword);
        Modules.accountModule.controller('Message', BitDiamond.Controllers.Shared.Message);
        Modules.accountModule.controller('VerifyRegistration', BitDiamond.Controllers.Account.VerifyRegistration);
        Modules.accountModule.controller('Terms', BitDiamond.Controllers.Account.Terms);
        //configure states
        Modules.accountModule.config(function ($stateProvider, $urlRouterProvider) {
            $urlRouterProvider.otherwise('/signin/');
            $stateProvider
                .state('signin', {
                url: '/signin/:returnUrl',
                templateUrl: '/account/signin',
                controller: 'Signin',
                controllerAs: 'vm'
            })
                .state('signup', {
                url: '/signup',
                templateUrl: '/account/signup',
                controller: 'Signup',
                controllerAs: 'vm'
            })
                .state('passwordRecoveryRequest', {
                url: '/recovery-request',
                templateUrl: '/account/recovery-request',
                controller: 'RecoveryRequest',
                controllerAs: 'vm'
            })
                .state('recoverPassword', {
                url: '/recover-password/:data',
                templateUrl: '/account/recover-password',
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
                templateUrl: '/account/message',
                controller: 'Message',
                controllerAs: 'vm'
            })
                .state('verifyRegistration', {
                url: '/verify-registration/:data',
                templateUrl: '/account/verify-registration',
                controller: 'VerifyRegistration',
                controllerAs: 'vm'
            })
                .state('termsAndConditions', {
                url: '/terms-conditions',
                templateUrl: '/account/terms-conditions',
                controller: 'Terms',
                controllerAs: 'vm'
            });
        });
    })(Modules = BitDiamond.Modules || (BitDiamond.Modules = {}));
})(BitDiamond || (BitDiamond = {}));
