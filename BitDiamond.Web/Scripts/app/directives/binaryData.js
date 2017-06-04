var BitDiamond;
(function (BitDiamond) {
    var Directives;
    (function (Directives) {
        var BinaryData = (function () {
            function BinaryData() {
                this.scope = {
                    binaryData: "="
                };
                this.restrict = 'A';
            }
            BinaryData.prototype.link = function (scope, element, attributes) {
                element.bind("change", function (changeEvent) {
                    var reader = new FileReader();
                    reader.onload = function (loadEvent) {
                        scope.$apply(function () {
                            scope.binaryData = BitDiamond.Utils.EncodedBinaryData.Create({
                                Size: changeEvent.target.files[0].size,
                                Data: new Uint8Array(reader.result),
                                Mime: changeEvent.target.files[0].type,
                                Name: changeEvent.target.files[0].name
                            });
                        });
                    };
                    if (changeEvent.target.files.length > 0 &&
                        changeEvent.target.files[0] instanceof Blob)
                        reader.readAsArrayBuffer(changeEvent.target.files[0]);
                });
            };
            ;
            return BinaryData;
        }());
        Directives.BinaryData = BinaryData;
    })(Directives = BitDiamond.Directives || (BitDiamond.Directives = {}));
})(BitDiamond || (BitDiamond = {}));
//# sourceMappingURL=binaryData.js.map