
module BitDiamond.Modules {

    export const postsModule = angular.module('posts', ['ui.router', 'ngSanitize', 'ngAnimate']);

    //directives
    postsModule.directive('ringLoader', () => new BitDiamond.Directives.RingLoader());
    postsModule.directive('boxLoader', () => new BitDiamond.Directives.BoxLoader());
    postsModule.directive('summernote', ($compile) => new BitDiamond.Directives.Summernote($compile));


    //services
    postsModule.service('__transport', BitDiamond.Utils.Services.DomainTransport);
    postsModule.service('__notify', BitDiamond.Utils.Services.NotifyService);
    postsModule.service('__userContext', BitDiamond.Utils.Services.UserContext);

    postsModule.service('__posts', BitDiamond.Services.Posts);
    postsModule.service('__account', BitDiamond.Services.Account);
    postsModule.service('__systemNotification', BitDiamond.Services.Notification);


    //controllers
    postsModule.controller('NavBar', BitDiamond.Controllers.Shared.NavBar);
    postsModule.controller('SideBar', BitDiamond.Controllers.Shared.SideBar);

    postsModule.controller('List', BitDiamond.Controllers.Posts.List);
    postsModule.controller('Edit', BitDiamond.Controllers.Posts.Edit);
    postsModule.controller('Details', BitDiamond.Controllers.Posts.Details);


    //configure states
    postsModule.config(($stateProvider: angular.ui.IStateProvider, $urlRouterProvider: angular.ui.IUrlRouterProvider) => {
        $urlRouterProvider.otherwise('/list');

        $stateProvider
            .state('list', {
                url: '/list',
                templateUrl: '/posts/list',
                controller: 'List',
                controllerAs: 'vm'
            })
            .state('edit', {
                url: '/edit/:id',
                params: {
                    post: null,
                    id: null
                },
                templateUrl: '/posts/edit',
                controller: 'Edit',
                controllerAs: 'vm'
            })
            .state('details', {
                url: '/details/:id',
                params: {
                    post: null,
                    id: null
                },
                templateUrl: '/posts/details',
                controller: 'Details',
                controllerAs: 'vm'
            });
    });
}