var BitDiamond;
(function (BitDiamond) {
    var Modules;
    (function (Modules) {
        Modules.module = angular.module('account', ['ui.router', 'ngSanitize', 'ngAnimate']);
        //directives
        Modules.module.directive('ringLoader', function () { return new BitDiamond.Directives.RingLoader(); });
        Modules.module.directive('boxLoader', function () { return new BitDiamond.Directives.BoxLoader(); });
        //module.directive('binaryData', Gaia.Directives.BinaryData);
        //module.directive('tagsInput', () => new Gaia.Directives.TagsInput());
        //module.directive('numberSpinner', () => new Gaia.Directives.NumberSpinner());
        //module.directive('enumOptions', () => new Gaia.Directives.EnumOptions());
        //module.directive('smallProductCard', ['#gaia.marketPlaceService', '#gaia.utils.notify', '$compile',
        //    (mp: Services.MarketPlaceService, n: Utils.Services.NotifyService, $compile: ng.ICompileService) => new Gaia.Directives.MarketPlace.SmallProductCard(mp, n, $compile)]);
        //module.directive('largeProductCard', ['#gaia.marketPlaceService', '#gaia.utils.notify', '$compile',
        //    (mp: Services.MarketPlaceService, n: Utils.Services.NotifyService, $compile: ng.ICompileService) => new Gaia.Directives.MarketPlace.LargeProductCard(mp, n, $compile)]);
        //services
        Modules.module.service('__transport', BitDiamond.Utils.Services.DomainTransport);
        Modules.module.service('__dom', BitDiamond.Utils.Services.DomModelService);
        Modules.module.service('__notify', BitDiamond.Utils.Services.NotifyService);
        Modules.module.service('__account', BitDiamond.Services.Account);
        //controllers
        Modules.module.controller('Signin', BitDiamond.Controllers.Account.Signin);
        Modules.module.controller('Signup', BitDiamond.Controllers.Account.Signup);
        Modules.module.controller('RecoveryRequest', BitDiamond.Controllers.Account.RecoveryRequest);
        Modules.module.controller('RecoverPassword', BitDiamond.Controllers.Account.RecoverPassword);
        Modules.module.controller('Message', BitDiamond.Controllers.Shared.Message);
        Modules.module.controller('VerifyRegistration', BitDiamond.Controllers.Account.VerifyRegistration);
        Modules.module.controller('Terms', BitDiamond.Controllers.Account.Terms);
        //configure states
        Modules.module.config(function ($stateProvider, $urlRouterProvider) {
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
                url: '/recover/:data',
                templateUrl: '/account/recover-password',
                controller: 'RecoverPassword',
                controllerAs: 'vm'
            })
                .state('message', {
                url: '/message',
                params: {
                    message: null,
                    title: null,
                    action: null,
                    actionTitle: null
                },
                templateUrl: '/account/login-message',
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
