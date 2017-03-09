var BitDiamond;
(function (BitDiamond) {
    var Utils;
    (function (Utils) {
        var Encoding;
        (function (Encoding) {
            Encoding[Encoding["InlineCss"] = 0] = "InlineCss";
        })(Encoding = Utils.Encoding || (Utils.Encoding = {}));
        var StringPair = (function () {
            function StringPair(key, value) {
                this.Key = StringPair.DecodeDelimiters(key);
                this.Value = StringPair.DecodeDelimiters(value);
            }
            StringPair.prototype.toString = function (encoding) {
                //if(encoding == ...) //encode
                //else //default encoding
                return StringPair.EncodeDelimiters(this.Key) + ':' + StringPair.EncodeDelimiters(this.Value) + ';';
            };
            StringPair.ParseStringPairs = function (value, encoding) {
                encoding = encoding || Encoding.InlineCss;
                if (encoding == Encoding.InlineCss)
                    return value.split(';')
                        .filter(function (_part) { return !Object.isNullOrUndefined(_part) && _part.length > 0; })
                        .map(function (v) {
                        var parts = v.split(':').filter(function (_part) { return !Object.isNullOrUndefined(_part) && _part.length > 0; });
                        var sp = new StringPair(parts[0].trim(), parts.length > 1 ? parts[1].trim() : null);
                        return sp;
                    })
                        .filter(function (pair) { return pair.Key != null && pair.Key != ''; });
                else
                    return [];
            };
            StringPair.EncodeDelimiters = function (value) {
                return value.replace(':', '##col').replace(';', '##scol');
            };
            StringPair.DecodeDelimiters = function (value) {
                return value.replace('##col', ':').replace('##scol', ';');
            };
            return StringPair;
        }());
        Utils.StringPair = StringPair;
        var StringPairCollection = (function () {
            function StringPairCollection(pairs) {
                this._pairs = [];
                if (!Object.isNullOrUndefined(pairs))
                    (_a = this._pairs).push.apply(_a, StringPair.ParseStringPairs(pairs));
                var _a;
            }
            Object.defineProperty(StringPairCollection.prototype, "pairs", {
                get: function () {
                    return this._pairs;
                },
                enumerable: true,
                configurable: true
            });
            StringPairCollection.prototype.getOrAdd = function (key, generator) {
                var pair = this._pairs.firstOrDefault(function (_p) { return _p.Key == key; });
                if (Object.isNullOrUndefined(pair))
                    this._pairs.push(pair = new StringPair(key, generator(key)));
                return pair;
            };
            StringPairCollection.prototype.add = function (key, value) {
                this.getOrAdd(key, function (_k) { return value; }).Value = value;
                return this;
            };
            StringPairCollection.prototype.remove = function (key) {
                var indx = 0;
                for (; indx < this._pairs.length; indx++) {
                    if (this._pairs[indx].Key == key)
                        break;
                }
                if (indx < this._pairs.length)
                    this._pairs.removeAt(indx);
                return this;
            };
            StringPairCollection.prototype.toString = function () {
                return this._pairs.aggregate('', function (sbuff, next) { return sbuff + ' ' + next.toString(); });
            };
            return StringPairCollection;
        }());
        Utils.StringPairCollection = StringPairCollection;
    })(Utils = BitDiamond.Utils || (BitDiamond.Utils = {}));
})(BitDiamond || (BitDiamond = {}));
