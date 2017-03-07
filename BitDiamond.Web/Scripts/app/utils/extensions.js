var BitDiamond;
(function (BitDiamond) {
    var Extensions;
    (function (Extensions) {
        ///object extension
        Object.defineProperty(Object.prototype, 'copyTo', {
            value: function (target, properties) {
                var _this = this;
                //'use strict';
                // We must check against these specific cases.
                if (target === undefined || target === null)
                    throw new TypeError('Cannot convert undefined or null to object');
                if (properties) {
                    properties.forEach(function (nextKey) {
                        if (_this.hasOwnProperty(nextKey))
                            target[nextKey] = _this[nextKey];
                    });
                }
                else {
                    for (var nextKey in this) {
                        if (this.hasOwnProperty(nextKey))
                            target[nextKey] = this[nextKey];
                    }
                }
                return target;
            },
            writable: false,
            configurable: false,
            enumerable: false
        });
        Object.defineProperty(Object.prototype, 'with', {
            value: function (obj) {
                if (obj)
                    obj.copyTo(this);
                return this;
            },
            writable: false,
            configurable: false,
            enumerable: false
        });
        Object.defineProperty(Object.prototype, 'project', {
            value: function (f) {
                if (typeof f === 'function')
                    return f(this);
                else
                    return null;
            },
            writable: false,
            configurable: false,
            enumerable: false
        });
        Object.defineProperty(Object.prototype, 'properties', {
            value: function () {
                return Object.getOwnPropertyNames(this);
            },
            writable: false,
            configurable: false,
            enumerable: false
        });
        Object.defineProperty(Object.prototype, 'keys', {
            value: function () {
                return Object.keys(this);
            },
            writable: false,
            configurable: false,
            enumerable: false
        });
        Object.defineProperty(Object.prototype, 'keyValuePairs', {
            value: function () {
                var _this = this;
                return Object.keys(this).map(function (_k) {
                    return {
                        Key: _k,
                        Value: _this[_k]
                    };
                });
            },
            writable: false,
            configurable: false,
            enumerable: false
        });
        Object.defineProperty(Object.prototype, 'propertyMaps', {
            value: function () {
                var _this = this;
                return this
                    .properties()
                    .map(function (_p) {
                    return {
                        Key: _p,
                        Value: _this[_p]
                    };
                });
            },
            writable: false,
            configurable: false,
            enumerable: false
        });
        Object.isNullOrUndefined = function (value) {
            if (typeof value === 'undefined')
                return true;
            else if (value === null)
                return true;
            else
                return false;
        };
        ///string extension
        Object.defineProperty(String.prototype, 'asChars', {
            value: function () {
                var _this = this;
                var arr = [];
                for (var cnt = 0; cnt < _this.length; cnt++)
                    arr.push(_this.charAt(cnt));
                return arr;
            },
            writable: false,
            configurable: false,
            enumerable: false
        });
        Object.defineProperty(String.prototype, 'trimLeft', {
            value: function (str) {
                var _this = this;
                var indx = _this.indexOf(str);
                if (indx == 0)
                    return _this.substr(indx, str.length);
                else
                    return str;
            },
            writable: false,
            configurable: false,
            enumerable: false
        });
        Object.defineProperty(String.prototype, 'trimRight', {
            value: function (str) {
                var _this = this;
                var lindx = _this.lastIndexOf(str);
                if (lindx == _this.length - str.length)
                    return _this.substr(lindx, str.length);
                else
                    return str;
            },
            writable: false,
            configurable: false,
            enumerable: false
        });
        Object.defineProperty(String.prototype, 'trimChars', {
            value: function (str) {
                var sar;
                if (typeof str === 'string')
                    sar = [str];
                else
                    sar = str;
                var localString = this;
                sar.forEach(function (v) {
                    localString = localString.trimLeft(v).trimRight(v);
                });
                return localString;
            },
            writable: false,
            configurable: false,
            enumerable: false
        });
        Object.defineProperty(String.prototype, 'contains', {
            value: function (str) {
                var localString = this;
                return localString.indexOf(str) >= 0;
            },
            writable: false,
            configurable: false,
            enumerable: false
        });
        Object.defineProperty(String.prototype, 'startsWith', {
            value: function (str) {
                return this.indexOf(str) == 0;
            },
            writable: false,
            configurable: false,
            enumerable: false
        });
        Object.defineProperty(String.prototype, 'endsWith', {
            value: function (str) {
                var originalString = this;
                return originalString.lastIndexOf(str) == originalString.length - str.length;
            },
            writable: false,
            configurable: false,
            enumerable: false
        });
        ///number extension
        ///array extensions    
        Array.prototype.insert = function (index, item) {
            var _this = this;
            return _this.splice(index, 0, item);
        };
        Array.prototype.paginate = function (sequence, pageIndex, pageSize) {
            if (pageIndex < 0 || pageSize < 1)
                throw 'invalid pagination arguments';
            var start = pageSize * pageIndex;
            return new BitDiamond.Utils.SequencePage(sequence.slice(start, (start + pageSize)), pageIndex, pageSize, sequence.length);
        };
        Array.prototype.remove = function (value) {
            var arr = this;
            var index = arr.indexOf(value);
            if (index >= 0)
                return arr.splice(index, 1);
            else
                return arr;
        };
        Array.prototype.removeAt = function (index) {
            var arr = this;
            return arr.splice(index, 1);
        };
        Array.prototype.first = function (predicate) {
            var arr = this;
            if (predicate)
                arr = arr.filter(predicate);
            return arr[0]; //intentionally throw an exception if the array is empty
        };
        Array.prototype.firstOrDefault = function (predicate) {
            try {
                return this.first(predicate);
            }
            catch (e) {
                return null;
            }
        };
        Array.prototype.clear = function () {
            var _this = this;
            if (_this.length <= 0)
                return;
            else {
                _this.splice(0, _this.length);
            }
        };
        Array.prototype.group = function (keySelector) {
            var arr = this;
            var map = {};
            arr.forEach(function (_v) {
                var key = keySelector(_v);
                var cache = map[key.toString()] || (map[key.toString()] = []);
                cache.push(_v);
            });
            return map.propertyMaps().map(function (_map) {
                return {
                    Key: _map.Key,
                    Value: _map.Value
                };
            });
        };
        Array.prototype.reduce = function (seed, transformFunc) {
            var arr = this;
            var v = seed;
            for (var cnt = 0; cnt < arr.length; cnt++) {
                v = transformFunc(v, arr[cnt]);
            }
            return v;
        };
        Array.prototype.contains = function (value) {
            var arr = this;
            return arr.indexOf(value) >= 0;
        };
    })(Extensions = BitDiamond.Extensions || (BitDiamond.Extensions = {}));
})(BitDiamond || (BitDiamond = {}));
