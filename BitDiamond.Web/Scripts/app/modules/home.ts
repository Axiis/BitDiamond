
module BitDiamond.Modules {

    export const homeModlue = angular.module('home', ['ui.router', 'ngSanitize', 'ngAnimate']);

    //directives
    homeModlue.directive('boxLoader', () => new BitDiamond.Directives.BoxLoader());

    //services
    homeModlue.service('__transport', BitDiamond.Utils.Services.DomainTransport);
    homeModlue.service('__dom', BitDiamond.Utils.Services.DomModelService);
    homeModlue.service('__notify', BitDiamond.Utils.Services.NotifyService);
    homeModlue.service('__userContext', BitDiamond.Utils.Services.UserContext);

    homeModlue.service('__blockChain', BitDiamond.Services.BlockChain);
    homeModlue.service('__account', BitDiamond.Services.Account);


    //controllers
    homeModlue.controller('Landing', BitDiamond.Controllers.Home.Landing);
}