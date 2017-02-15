
module BitDiamond.Directives {

    export class EnumOptions {

        restrict: string = 'A';

        link(scope: ng.IScope, element: JQuery, attributes: ng.IAttributes): void {

            var config = (<string>attributes['enumOptions']).split(':').map(_str => _str.trim());
            var modelExp = config[0];
            var modelValue: number;
            scope.$eval('modelValue = ' + modelExp, modelValue);
            var _enum = eval(config[1]);

            var options = '';
            Utils.EnumHelper.getNames(_enum).forEach(_prop => {
                var enumVal: number = _enum[_prop];
                options += '<option value="' + enumVal + '" ' + (modelValue == enumVal ? "selected " : "") + '>' + _prop + '</option>';
            });

            element.html(options);

            //bind changes on the UI to the model
            element.bind('change', (evt) => {
                scope.$eval(modelExp + ' = ' + element.get(0)['value']);
            });

            //setup a watch to bind changes on the model to the UI
            scope.$watch(modelExp, (_new, _old) => {
                element.get(0)['value'] = _new;
            });
        }

    }

}