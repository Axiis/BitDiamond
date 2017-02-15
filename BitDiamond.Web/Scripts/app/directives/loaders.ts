

module BitDiamond.Directives {

    export class RingLoader {

        restrict: string = 'E';
        scope: any = {
            size: '=?',
            //color: '=?', get this from the 'attributes' in the 'link' function
            isBlockLoader: '=?blockLoader',
            overlayOpacity: '=?',
            show: '=?'
        };

        //note that we should do all of these inside the link function instead so that the color attribute can be carried from the attributes object.
        controller($scope) {
        };


        link(scope: ng.IScope, element: JQuery, attributes: ng.IAttributes): void {

            var $scope: any = scope;

            //default values
            if (!$scope.size) $scope.size = '0px';
            if (Object.isNullOrUndefined($scope.isBlockLoader)) $scope.isBlockLoader = false;
            if (!$scope.overlayOpacity) $scope.overlayOpacity = 0;
            if (!$scope.show) $scope.show = false;
            if (Object.isNullOrUndefined(attributes['color'])) $scope.color = 'rgba(0,0,0,0)';
            else $scope.color = attributes['color'];

            $scope.containerStyle = function () {
                return {
                    height: $scope.size + 'px',
                    width: $scope.size + 'px'
                };
            };

            $scope.rotorStyle = function () {
                return {
                    'box-shadow': '0 2px 0 0 ' + $scope.color
                };
            };

            $scope.overlayStyle = function () {
                return {
                    'background-color': 'rgba(0,0,0,' + ($scope.overlayOpacity) + ')',
                    display: !$scope.isBlockLoader ? 'inline-block' : 'block',
                    width: !$scope.isBlockLoader ? 'initial' : '100%',
                    height: !$scope.isBlockLoader ? 'initial' : '100%',
                    'text-align': !$scope.isBlockLoader ? 'initial' : 'center'
                };
            }
        }

        template: string = '<div class="inline-center-pseudo" ng-style="overlayStyle()" ng-show="show">' +
                               '<div class="ring-loader" ng-style="containerStyle()">' +
                                   '<div ng-style="rotorStyle()"></div>' +
                               '</div>' +
                           '</div>';
    }

    export class BoxLoader {

        restrict: string = 'E';
        scope: any = {
            size: '=?',
            isBlockLoader: '=?blockLoader',
            overlayOpacity: '=?',
            show: '=?'
        };

        link(scope: ng.IScope, element: JQuery, attributes: ng.IAttributes) {

            var $scope: any = scope;

            //default values
            if (!$scope.size) $scope.size = '0px';
            if (Object.isNullOrUndefined($scope.isBlockLoader)) $scope.isBlockLoader = false;
            if (!$scope.overlayOpacity) $scope.overlayOpacity = 0;
            if (!$scope.show) $scope.show = false;
            if (Object.isNullOrUndefined(attributes['color'])) $scope.color = 'rgba(0,0,0,0)';
            else $scope.color = attributes['color'];

            $scope.containerStyle = function () {
                return {
                    height: $scope.size + 'px',
                    width: $scope.size + 'px'
                };
            };

            $scope.boxStyle = function () {
                return {
                    'background': $scope.color
                };
            };

            $scope.overlayStyle = function () {
                return {
                    'background-color': 'rgba(0,0,0,' + ($scope.overlayOpacity) + ')',
                    display: !$scope.isBlockLoader ? 'inline-block' : 'block',
                    width: !$scope.isBlockLoader ? 'initial' : '100%',
                    height: !$scope.isBlockLoader ? 'initial' : '100%',
                    'text-align': !$scope.isBlockLoader ? 'initial' : 'center'
                };
            }
        };

        template: string = '<div class="inline-center-pseudo" ng-style="overlayStyle()" ng-show="show">' +
                               '<div class="cube-loader" ng-style="containerStyle()">' +
                                   '<div ng-style="boxStyle()"></div>' +
                                   '<div ng-style="boxStyle()"></div>' +
                                   '<div ng-style="boxStyle()"></div>' +
                                   '<div ng-style="boxStyle()"></div>' +
                               '</div>' +
                           '</div>';

    }

}