var BitDiamond;
(function (BitDiamond) {
    var Directives;
    (function (Directives) {
        var Summernote = (function () {
            function Summernote($compile) {
                this.restrict = 'E';
                this.scope = {};
                this.$compile = $compile;
            }
            Summernote.prototype.link = function (scope, element, attributes) {
                var id = attributes['id'];
                var $class = attributes['class'];
                var style = attributes['style'];
                var bind = attributes['bind'];
                var $div = $(document.createElement('div'))
                    .attr('id', id)
                    .addClass($class);
                style.split(';').forEach(function (style) {
                    var parts = style.split(':');
                    if (parts.length < 2)
                        return;
                    else
                        $div.css(parts[0].trim(), parts[1].trim());
                });
                var _link = this.$compile($div);
                var activatedDom = _link(scope);
                element.html(''); //clear the dom
                element.append(activatedDom);
                //now initialize summernote
                scope.parent = scope.$parent; //<--hack to enable '$eval' accross scope hierarchy
                activatedDom.summernote({
                    airMode: true,
                    airPopover: [
                        ['font', ['bold', 'italic', 'underline', 'clear']],
                        ['style', ['strikethrough', 'superscript', 'subscript']],
                        ['fontsize', ['fontsize']],
                        ['color', ['color']],
                        ['para', ['ul', 'ol', 'paragraph']],
                        ['table', ['table']],
                        ['insert', ['link', 'picture']]
                    ]
                }).on('summernote.change', function (content, $editable) {
                    if (!Object.isNullOrUndefined(bind)) {
                        scope.content = $editable;
                        scope.$eval('parent.' + bind + ' = content');
                    }
                });
            };
            return Summernote;
        }());
        Directives.Summernote = Summernote;
    })(Directives = BitDiamond.Directives || (BitDiamond.Directives = {}));
})(BitDiamond || (BitDiamond = {}));
