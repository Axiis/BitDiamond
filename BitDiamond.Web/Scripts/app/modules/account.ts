
module BitDiamond.Modules {

    export const module = angular.module('account', ['ui.router', 'ngSanitize', 'ngAnimate']);

    //directives
    module.directive('ringLoader', () => new BitDiamond.Directives.RingLoader());
    module.directive('boxLoader', () => new BitDiamond.Directives.BoxLoader());
    //module.directive('binaryData', Gaia.Directives.BinaryData);
    //module.directive('tagsInput', () => new Gaia.Directives.TagsInput());
    //module.directive('numberSpinner', () => new Gaia.Directives.NumberSpinner());
    //module.directive('enumOptions', () => new Gaia.Directives.EnumOptions());
    //module.directive('smallProductCard', ['#gaia.marketPlaceService', '#gaia.utils.notify', '$compile',
    //    (mp: Services.MarketPlaceService, n: Utils.Services.NotifyService, $compile: ng.ICompileService) => new Gaia.Directives.MarketPlace.SmallProductCard(mp, n, $compile)]);
    //module.directive('largeProductCard', ['#gaia.marketPlaceService', '#gaia.utils.notify', '$compile',
    //    (mp: Services.MarketPlaceService, n: Utils.Services.NotifyService, $compile: ng.ICompileService) => new Gaia.Directives.MarketPlace.LargeProductCard(mp, n, $compile)]);


    //services
    module.service('__transport', BitDiamond.Utils.Services.DomainTransport);
    module.service('__dom', BitDiamond.Utils.Services.DomModelService);
    module.service('__notify', BitDiamond.Utils.Services.NotifyService);

    module.service('__account', BitDiamond.Services.Account);


    //controllers
    module.controller('Signin', BitDiamond.Controllers.Account.Signin);
    module.controller('Signup', BitDiamond.Controllers.Account.Signup);
    module.controller('RecoveryRequest', BitDiamond.Controllers.Account.RecoveryRequest);
    module.controller('RecoverPassword', BitDiamond.Controllers.Account.RecoverPassword);
    module.controller('Message', BitDiamond.Controllers.Shared.Message);
    module.controller('VerifyRegistration', BitDiamond.Controllers.Account.VerifyRegistration);
    module.controller('Terms', BitDiamond.Controllers.Account.Terms);


    //configure states
    module.config(($stateProvider: angular.ui.IStateProvider, $urlRouterProvider: angular.ui.IUrlRouterProvider) => {
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
                templateUrl: '/account/terms-conditions', //<-- /account/verify-registration
                controller: 'Terms',
                controllerAs: 'vm'
            });
    });
}