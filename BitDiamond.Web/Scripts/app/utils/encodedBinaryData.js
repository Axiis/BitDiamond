var BitDiamond;
(function (BitDiamond) {
    var Utils;
    (function (Utils) {
        var EncodedBinaryData = (function () {
            function EncodedBinaryData(binary, mime, name, metadata) {
                this._mime = null;
                this._metadata = new Utils.StringPairCollection();
                this.Data = binary;
                this.Mime = mime;
                this.Metadata = metadata;
                this.Name = name; //name comes last so it is appended to the metadata
            }
            Object.defineProperty(EncodedBinaryData.prototype, "Data", {
                //Data
                get: function () {
                    return this._data || new Uint8Array(0);
                },
                set: function (value) {
                    this._data = value;
                },
                enumerable: true,
                configurable: true
            });
            Object.defineProperty(EncodedBinaryData.prototype, "Metadata", {
                //Metadata
                get: function () {
                    return this._metadata.toString();
                },
                set: function (value) {
                    var name = this.Name;
                    this._metadata = new Utils.StringPairCollection(value);
                    if (!Object.isNullOrUndefined(name))
                        this._metadata.add("name", name);
                },
                enumerable: true,
                configurable: true
            });
            Object.defineProperty(EncodedBinaryData.prototype, "Name", {
                //Name
                get: function () {
                    var pair = this._metadata.pairs.firstOrDefault(function (_t) { return _t.Key.toLowerCase() == "name"; });
                    if (Object.isNullOrUndefined(pair))
                        return null;
                    else
                        return pair.Value;
                },
                set: function (value) {
                    this._metadata.remove("name").add("name", value);
                },
                enumerable: true,
                configurable: true
            });
            Object.defineProperty(EncodedBinaryData.prototype, "Mime", {
                //Mime
                get: function () {
                    return this._mime || this.MimeMap().MimeCode;
                },
                set: function (value) {
                    if (!Object.isNullOrUndefined(value))
                        this._mime = value.trim();
                    else
                        this._mime = null;
                },
                enumerable: true,
                configurable: true
            });
            EncodedBinaryData.prototype.MimeMap = function () {
                return Object.isNullOrUndefined(this._mime) ?
                    Utils.Constants.MimeCodes.toMimeMap(this.Extension()) :
                    new Utils.MimeMap(this._mime, this.Extension()); //...or force the given mime on the present (or absent) extension
            };
            EncodedBinaryData.prototype.Extension = function () {
                if (!Object.isNullOrUndefined(this.Name) && this.Name.contains('.'))
                    return this.Name.substr(this.Name.lastIndexOf('.')).trim().toLowerCase();
                else
                    return null;
            };
            EncodedBinaryData.prototype.Base64 = function () {
                return Utils.ToBase64String(this.Data);
            };
            EncodedBinaryData.prototype.DataUri = function () {
                return 'data:' + this.Mime + ';base64,' + this.Base64();
            };
            EncodedBinaryData.prototype.MetadataTags = function () {
                return this._metadata.pairs;
            };
            EncodedBinaryData.prototype.RawObjectForm = function () {
                return {
                    Data: Utils.ToBase64String(this.Data),
                    Name: this.Name,
                    Mime: this.Mime
                };
            };
            EncodedBinaryData.prototype.Serializable = function () {
                return this.RawObjectForm();
            };
            EncodedBinaryData.Create = function (data) {
                return new EncodedBinaryData(data.Data, data.Mime, data.Name, data.Metadata);
            };
            return EncodedBinaryData;
        }());
        Utils.EncodedBinaryData = EncodedBinaryData;
    })(Utils = BitDiamond.Utils || (BitDiamond.Utils = {}));
})(BitDiamond || (BitDiamond = {}));
//# sourceMappingURL=encodedBinaryData.js.map