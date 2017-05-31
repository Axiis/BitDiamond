var BitDiamond;
(function (BitDiamond) {
    var Directives;
    (function (Directives) {
        var RingLoader = (function () {
            function RingLoader() {
                this.restrict = 'E';
                this.scope = {};
                this.template = '<div class="inline-center-pseudo" ng-style="__ldrz___overlayStyle()" ng-show="__ldrz___show()">' +
                    '<div class="ring-loader" ng-style="__ldrz___containerStyle()">' +
                    '<div ng-style="__ldrz___rotorStyle()"></div>' +
                    '</div>' +
                    '</div>';
            }
            RingLoader.prototype.link = function (scope, element, attributes) {
                var $scope = scope;
                //default values
                var size = scope.$eval(attributes['size'] || '0');
                var isBlockLoader = scope.$eval(attributes['blockLoader'] || 'false');
                var overlayOpacity = scope.$eval(attributes['overlayOpacity'] || '0');
                var color = attributes['color'] || 'rgba(0,0,0,0.98)';
                var show = attributes['show'] || 'false'; //<-- show isnt evaluated on purpose
                $scope.__ldrz___show = function () {
                    return scope.$parent.$eval(show);
                };
                $scope.__ldrz___containerStyle = function () {
                    return {
                        height: size + 'px',
                        width: size + 'px'
                    };
                };
                $scope.__ldrz___rotorStyle = function () {
                    return {
                        'box-shadow': '0 2px 0 0 ' + color
                    };
                };
                $scope.__ldrz___overlayStyle = function () {
                    return {
                        'background-color': 'rgba(0,0,0,' + (overlayOpacity) + ')',
                        display: !isBlockLoader ? 'inline-block' : 'block',
                        width: !isBlockLoader ? 'initial' : '100%',
                        height: !isBlockLoader ? 'initial' : '100%',
                        'text-align': !isBlockLoader ? 'initial' : 'center'
                    };
                };
            };
            return RingLoader;
        }());
        Directives.RingLoader = RingLoader;
        var WheelLoader = (function () {
            function WheelLoader() {
                this.restrict = 'E';
                this.scope = {};
                this.template = '<div class="inline-center-pseudo" ng-style="__ldrz___overlayStyle()" ng-show="__ldrz___show()">' +
                    '<div ng-style="__ldrz___rotorStyle()"></div>' +
                    '</div>';
            }
            WheelLoader.prototype.link = function (scope, element, attributes) {
                var $scope = scope;
                //default values
                var size = scope.$eval(attributes['size'] || '0');
                var isBlockLoader = scope.$eval(attributes['blockLoader'] || 'false');
                var overlayOpacity = scope.$eval(attributes['overlayOpacity'] || '0');
                var color = scope.$eval(attributes['color'] || 'rgba(0,0,0,0.98)');
                var show = attributes['show'] || 'false'; //<-- show isnt evaluated on purpose
                $scope.__ldrz___show = function () {
                    return scope.$parent.$eval(show);
                };
                $scope.__ldrz___containerStyle = function () {
                    return {
                        height: size + 'px',
                        width: size + 'px'
                    };
                };
                $scope.__ldrz___rotorStyle = function () {
                    return {
                        'border': '2px solid ' + color,
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
                $scope.__ldrz___overlayStyle = function () {
                    return {
                        'background-color': 'rgba(0,0,0,' + (overlayOpacity) + ')',
                        display: !isBlockLoader ? 'inline-block' : 'block',
                        width: !isBlockLoader ? 'initial' : '100%',
                        height: !isBlockLoader ? 'initial' : '100%',
                        'text-align': !isBlockLoader ? 'initial' : 'center'
                    };
                };
            };
            return WheelLoader;
        }());
        Directives.WheelLoader = WheelLoader;
        var BoxLoader = (function () {
            function BoxLoader() {
                this.restrict = 'E';
                this.scope = {};
                this.template = '<div class="inline-center-pseudo" ng-style="__ldrz___overlayStyle()" ng-show="__ldrz___show()">' +
                    '<div class="cube-loader" ng-style="__ldrz___containerStyle()">' +
                    '<div ng-style="__ldrz___boxStyle()"></div>' +
                    '<div ng-style="__ldrz___boxStyle()"></div>' +
                    '<div ng-style="__ldrz___boxStyle()"></div>' +
                    '<div ng-style="__ldrz___boxStyle()"></div>' +
                    '</div>' +
                    '</div>';
            }
            BoxLoader.prototype.link = function (scope, element, attributes) {
                var $scope = scope;
                //default values
                var size = scope.$eval(attributes['size'] || '0');
                var isBlockLoader = scope.$eval(attributes['blockLoader'] || 'false');
                var overlayOpacity = scope.$eval(attributes['overlayOpacity'] || '0');
                var color = attributes['color'] || 'rgba(0,0,0,0.98)';
                var show = attributes['show'] || 'false'; //<-- show isnt evaluated on purpose
                $scope.__ldrz___show = function () {
                    return scope.$parent.$eval(show);
                };
                $scope.__ldrz___containerStyle = function () {
                    return {
                        height: size + 'px',
                        width: size + 'px'
                    };
                };
                $scope.__ldrz___boxStyle = function () {
                    return {
                        'background': color
                    };
                };
                $scope.__ldrz___overlayStyle = function () {
                    return {
                        'background-color': 'rgba(0,0,0,' + (overlayOpacity) + ')',
                        display: !isBlockLoader ? 'inline-block' : 'block',
                        width: !isBlockLoader ? 'initial' : '100%',
                        height: !isBlockLoader ? 'initial' : '100%',
                        'text-align': !isBlockLoader ? 'initial' : 'center'
                    };
                };
            };
            ;
            return BoxLoader;
        }());
        Directives.BoxLoader = BoxLoader;
    })(Directives = BitDiamond.Directives || (BitDiamond.Directives = {}));
})(BitDiamond || (BitDiamond = {}));
