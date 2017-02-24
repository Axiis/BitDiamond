var BitDiamond;
(function (BitDiamond) {
    var Directives;
    (function (Directives) {
        var RingLoader = (function () {
            function RingLoader() {
                this.restrict = 'E';
                this.scope = {
                    size: '=?',
                    //color: '=?', get this from the 'attributes' in the 'link' function
                    isBlockLoader: '=?blockLoader',
                    overlayOpacity: '=?',
                    show: '=?'
                };
                this.template = '<div class="inline-center-pseudo" ng-style="overlayStyle()" ng-show="show">' +
                    '<div class="ring-loader" ng-style="containerStyle()">' +
                    '<div ng-style="rotorStyle()"></div>' +
                    '</div>' +
                    '</div>';
            }
            //note that we should do all of these inside the link function instead so that the color attribute can be carried from the attributes object.
            RingLoader.prototype.controller = function ($scope) {
            };
            ;
            RingLoader.prototype.link = function (scope, element, attributes) {
                var $scope = scope;
                //default values
                if (!$scope.size)
                    $scope.size = '0px';
                if (Object.isNullOrUndefined($scope.isBlockLoader))
                    $scope.isBlockLoader = false;
                if (!$scope.overlayOpacity)
                    $scope.overlayOpacity = 0;
                if (!$scope.show)
                    $scope.show = false;
                if (Object.isNullOrUndefined(attributes['color']))
                    $scope.color = 'rgba(0,0,0,0)';
                else
                    $scope.color = attributes['color'];
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
                };
            };
            return RingLoader;
        }());
        Directives.RingLoader = RingLoader;
        var WheelLoader = (function () {
            function WheelLoader() {
                this.restrict = 'E';
                this.scope = {
                    size: '=?',
                    //color: '=?', get this from the 'attributes' in the 'link' function
                    isBlockLoader: '=?blockLoader',
                    overlayOpacity: '=?',
                    show: '=?'
                };
                this.template = '<div class="inline-center-pseudo" ng-style="overlayStyle()" ng-show="show">' +
                    '<div ng-style="rotorStyle()"></div>' +
                    '</div>';
            }
            WheelLoader.prototype.controller = function ($scope) {
            };
            ;
            WheelLoader.prototype.link = function (scope, element, attributes) {
                var $scope = scope;
                //default values
                if (!$scope.size)
                    $scope.size = '0px';
                if (Object.isNullOrUndefined($scope.isBlockLoader))
                    $scope.isBlockLoader = false;
                if (!$scope.overlayOpacity)
                    $scope.overlayOpacity = 0;
                if (!$scope.show)
                    $scope.show = false;
                if (Object.isNullOrUndefined(attributes['color']))
                    $scope.color = 'rgba(0,0,0,0.98)';
                else
                    $scope.color = attributes['color'];
                $scope.containerStyle = function () {
                    return {
                        height: $scope.size + 'px',
                        width: $scope.size + 'px'
                    };
                };
                $scope.rotorStyle = function () {
                    return {
                        'border': '2px solid ' + $scope.color,
                        'border-radius': '50%',
                        'border-left-color': 'transparent',
                        'border-right-color': 'transparent',
                        'animation': 'cssload-spin 1s infinite linear',
                        '-o-animation': 'cssload-spin 1s infinite linear',
                        '-ms-animation': 'cssload-spin 1s infinite linear',
                        '-webkit-animation': 'cssload-spin 1s infinite linear',
                        '-moz-animation': 'cssload-spin 1s infinite linear'
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
                };
            };
            return WheelLoader;
        }());
        Directives.WheelLoader = WheelLoader;
        var BoxLoader = (function () {
            function BoxLoader() {
                this.restrict = 'E';
                this.scope = {
                    size: '=?',
                    isBlockLoader: '=?blockLoader',
                    overlayOpacity: '=?',
                    show: '=?'
                };
                this.template = '<div class="inline-center-pseudo" ng-style="overlayStyle()" ng-show="show">' +
                    '<div class="cube-loader" ng-style="containerStyle()">' +
                    '<div ng-style="boxStyle()"></div>' +
                    '<div ng-style="boxStyle()"></div>' +
                    '<div ng-style="boxStyle()"></div>' +
                    '<div ng-style="boxStyle()"></div>' +
                    '</div>' +
                    '</div>';
            }
            BoxLoader.prototype.link = function (scope, element, attributes) {
                var $scope = scope;
                //default values
                if (!$scope.size)
                    $scope.size = '0px';
                if (Object.isNullOrUndefined($scope.isBlockLoader))
                    $scope.isBlockLoader = false;
                if (!$scope.overlayOpacity)
                    $scope.overlayOpacity = 0;
                if (!$scope.show)
                    $scope.show = false;
                if (Object.isNullOrUndefined(attributes['color']))
                    $scope.color = 'rgba(0,0,0,0)';
                else
                    $scope.color = attributes['color'];
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
                };
            };
            ;
            return BoxLoader;
        }());
        Directives.BoxLoader = BoxLoader;
    })(Directives = BitDiamond.Directives || (BitDiamond.Directives = {}));
})(BitDiamond || (BitDiamond = {}));
