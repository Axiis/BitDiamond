
module BitDiamond.Directives {

    ///An attribute directive that accepts as args, a css-encoded key-value-pair that contains the following
    /// min: minimum value accepted by the number spinner
    /// max: maximum value accepted by the number spinner
    /// start: start value
    /// step: value to increment with
    /// format: format for the number to display
    /// model: angular expression that produces a variable to accept the value of the spinner
    export class NumberSpinner {

        restrict: string = 'A';

        link(scope: ng.IScope, element: JQuery, attributes): void {
            var binding = attributes.numberSpinner;
            var map: any = Utils.StringPair
                .ParseStringPairs(binding)
                .aggregate({}, (_map, _pair) => {
                    _map[_pair.Key.toLowerCase()] = _pair.Value;
                    return _map;
                });
            
            (element as any).spinner({
                min: scope.$eval(map.min || '0'),
                step: scope.$eval(map.step || '100'),
                start: scope.$eval(map.start || '0'),
                numberFormat: scope.$eval('"' + map.format + '"' || '"C"'),
                change: _v => {
                    try {
                        scope.$eval(map.model + ' = ' + element.val());
                    }
                    catch (e) { }
                }
            });
        }
    }
}