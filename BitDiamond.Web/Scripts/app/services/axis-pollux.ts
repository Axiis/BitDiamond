
module Pollux.Services {

    export class Authentication {


        static $inject = ["$http"];
        constructor(private $http: ng.IHttpService) {
        }
    }
}