var BitDiamond;
(function (BitDiamond) {
    var Directives;
    (function (Directives) {
        ///An attribute directive that accepts as args, a css-encoded key-value-pair that contains the following
        /// min: minimum value accepted by the number spinner
        /// max: maximum value accepted by the number spinner
        /// start: start value
        /// step: value to increment with
        /// format: format for the number to display
        /// model: angular expression that produces a variable to accept the value of the spinner
        var NumberSpinner = (function () {
            function NumberSpinner() {
                this.restrict = 'A';
            }
            NumberSpinner.prototype.link = function (scope, element, attributes) {
                var binding = attributes.numberSpinner;
                var map = BitDiamond.Utils.StringPair
                    .ParseStringPairs(binding)
                    .reduce({}, function (_map, _pair) {
                    _map[_pair.Key.toLowerCase()] = _pair.Value;
                    return _map;
                });
                element.spinner({
                    min: scope.$eval(map.min || '0'),
                    step: scope.$eval(map.step || '100'),
                    start: scope.$eval(map.start || '0'),
                    numberFormat: scope.$eval('"' + map.format + '"' || '"C"'),
                    change: function (_v) {
                        try {
                            scope.$eval(map.model + ' = ' + element.val());
                        }
                        catch (e) { }
                    }
                });
            };
            return NumberSpinner;
        }());
        Directives.NumberSpinner = NumberSpinner;
    })(Directives = BitDiamond.Directives || (BitDiamond.Directives = {}));
})(BitDiamond || (BitDiamond = {}));
//# sourceMappingURL=numberSpinner.js.map