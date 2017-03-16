var BitDiamond;
(function (BitDiamond) {
    var Directives;
    (function (Directives) {
        var EnumOptions = (function () {
            function EnumOptions() {
                this.restrict = 'A';
            }
            EnumOptions.prototype.link = function (scope, element, attributes) {
                var config = attributes['enumOptions'].split(':').map(function (_str) { return _str.trim(); });
                var modelExp = config[0];
                var modelValue;
                scope.$eval('modelValue = ' + modelExp, modelValue);
                var _enum = eval(config[1]);
                var options = '';
                BitDiamond.Utils.EnumHelper.getNames(_enum).forEach(function (_prop) {
                    var enumVal = _enum[_prop];
                    options += '<option value="' + enumVal + '" ' + (modelValue == enumVal ? "selected " : "") + '>' + _prop + '</option>';
                });
                element.html(options);
                //bind changes on the UI to the model
                element.bind('change', function (evt) {
                    scope.$eval(modelExp + ' = ' + element.get(0)['value']);
                });
                //setup a watch to bind changes on the model to the UI
                scope.$watch(modelExp, function (_new, _old) {
                    element.get(0)['value'] = _new;
                });
            };
            return EnumOptions;
        }());
        Directives.EnumOptions = EnumOptions;
    })(Directives = BitDiamond.Directives || (BitDiamond.Directives = {}));
})(BitDiamond || (BitDiamond = {}));
//# sourceMappingURL=enumOptions.js.map