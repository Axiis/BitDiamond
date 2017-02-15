
interface Object {
    copyTo(target: any, properties?: string[]): any;
    with<Obj>(value: any): Obj; 
    project<I, O>(f: (in1: I) => O): O;
    properties(): Array<string>;
    keys(): Array<string>;
    keyValuePairs(): Array<BitDiamond.Utils.Map<string, any>>;
    propertyMaps(): Array<BitDiamond.Utils.Map<string, any>>;
}


interface ObjectConstructor {
    isNullOrUndefined(value: any): boolean;
}

interface String {
    trimLeft(str: string): string;
    trimRight(str: string): string;
    trimChars(str: string|string[]): string;

    startsWith(str: string): boolean;
    endsWith(str: string): boolean;
    contains(str: string): boolean;
}


interface Array<T> {
    paginate(sequence: T[], pageIndex: number, pageSize: number): BitDiamond.Utils.SequencePage<T>;
    first(predicate?: (in1: T) => boolean): T;
    firstOrDefault(predicate?: (in1: T) => boolean): T;
    group<K>(keySelector: (in1: T) => K): Array<BitDiamond.Utils.Map<K, Array<T>>>;
    remove(value: T): T[];
    removeAt(index: number): T[];
    clear();
    reduce<U>(seed: U, transformFunc: (in1: U, in2: T) => U): U;
    contains(value: T): boolean;
}

module BitDiamond.Extensions {
    
    ///object extension

    Object.defineProperty(Object.prototype, 'copyTo', {
        value: function (target: any, properties?: string[]): any {
            //'use strict';
            // We must check against these specific cases.
            if (target === undefined || target === null)
                throw new TypeError('Cannot convert undefined or null to object');

            if (properties) {
                properties.forEach(nextKey => {
                    if (this.hasOwnProperty(nextKey))
                        target[nextKey] = this[nextKey];
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
        value: function <Obj>(obj: Object): Obj {

            if (obj) obj.copyTo(this);

            return this as Obj;
        },
        writable: false,
        configurable: false,
        enumerable: false
    });

    Object.defineProperty(Object.prototype, 'project', {
        value: function <I, O>(f: (in1: I) => O): O {
            if (typeof f === 'function') return f(this);
            else return null;
        },
        writable: false,
        configurable: false,
        enumerable: false
    });

    Object.defineProperty(Object.prototype, 'properties', {
        value: function (): Array<string> {
            return Object.getOwnPropertyNames(this);
        },
        writable: false,
        configurable: false,
        enumerable: false
    });

    Object.defineProperty(Object.prototype, 'keys', {
        value: function (): Array<string> {
            return Object.keys(this);
        },
        writable: false,
        configurable: false,
        enumerable: false
    });

    Object.defineProperty(Object.prototype, 'keyValuePairs', {
        value: function (): Array<Utils.Map<string, any>> {
            return Object.keys(this).map(_k => {
                return {
                    Key: _k,
                    Value: this[_k]
                } as Utils.Map<string, any>
            });
        },
        writable: false,
        configurable: false,
        enumerable: false
    });

    Object.defineProperty(Object.prototype, 'propertyMaps', {
        value: function (): Array<BitDiamond.Utils.Map<string, any>> {
            return (this as Object)
                .properties()
                .map(_p => {
                    return {
                        Key: _p,
                        Value: this[_p]
                    } as BitDiamond.Utils.Map<string, any>;
                });
        },
        writable: false,
        configurable: false,
        enumerable: false
    });

    Object.isNullOrUndefined = function (value: any): boolean {
        if (typeof value === 'undefined') return true;
        else if (value === null) return true;
        else return false;
    };


    ///string extension

    Object.defineProperty(String.prototype, 'trimLeft', {
        value: function (str: string): string {
            var _this = this as string;
            var indx = _this.indexOf(str)
            if (indx == 0) return _this.substr(indx, str.length);
            else return str;
        },
        writable: false,
        configurable: false,
        enumerable: false
    });
    Object.defineProperty(String.prototype, 'trimRight', {
        value: function (str: string): string {
            var _this = this as string;
            var lindx = _this.lastIndexOf(str)
            if (lindx == _this.length - str.length) return _this.substr(lindx, str.length);
            else return str;
        },
        writable: false,
        configurable: false,
        enumerable: false
    });
    Object.defineProperty(String.prototype, 'trimChars', {
        value: function (str: string | string[]): string {

            var sar: string[];
            if (typeof str === 'string') sar = [str];
            else sar = str;

            var localString = this as string;
            sar.forEach(v => {
                localString = localString.trimLeft(v).trimRight(v);
            });

            return localString;
        },
        writable: false,
        configurable: false,
        enumerable: false
    });
    Object.defineProperty(String.prototype, 'contains', {
        value: function (str: string): boolean {
            
            var localString = this as string;
            return localString.indexOf(str) >= 0;
        },
        writable: false,
        configurable: false,
        enumerable: false
    });


    Object.defineProperty(String.prototype, 'startsWith', {
        value: function (str: string): boolean {
            return (this as string).indexOf(str) == 0;
        },
        writable: false,
        configurable: false,
        enumerable: false
    });
    Object.defineProperty(String.prototype, 'endsWith', {
        value: function (str: string): boolean {
            var originalString = this as string;
            return originalString.lastIndexOf(str) == originalString.length - str.length;
        },
        writable: false,
        configurable: false,
        enumerable: false
    });

    ///number extension


    ///array extensions    

    Array.prototype.paginate = function<Data>(sequence: Array<Data>, pageIndex: number, pageSize: number): BitDiamond.Utils.SequencePage<Data> {

        if (pageIndex < 0 || pageSize < 1) throw 'invalid pagination arguments';

        var start = pageSize * pageIndex;
        return new BitDiamond.Utils.SequencePage<Data>(
            sequence.slice(start, (start + pageSize)),
            pageIndex,
            pageSize,
            sequence.length);
    }

    Array.prototype.remove = function <Data>(value: Data): Data[] {
        var arr = this as Array<Data>;
        var index = arr.indexOf(value);
        if (index >= 0) return arr.splice(index, 1);
        else return arr;
    }
    Array.prototype.removeAt = function <Data>(index: number): Data[] {
        var arr = this as Array<Data>;
        return arr.splice(index, 1);
    }

    Array.prototype.first = function <Data>(predicate?: (in1: Data) => boolean): Data {
        var arr = this as Array<Data>;
        if (predicate) arr = arr.filter(predicate);
        return arr[0]; //intentionally throw an exception if the array is empty
    }

    Array.prototype.firstOrDefault = function <Data>(predicate?: (in1: Data) => boolean): Data {
        try {
            return (this as Array<Data>).first(predicate);
        }
        catch (e) {
            return null;
        }
    }

    Array.prototype.clear = function () {
        var _this = this as Array<any>;

        if (_this.length <= 0) return;
        else {
            _this.splice(0, _this.length);
        }
    }

    Array.prototype.group = function <K, V>(keySelector: (in1: V) => K): Array<BitDiamond.Utils.Map<K, Array<V>>> {

        var arr = this as Array<V>;
        var map = {};
        arr.forEach(_v => {
            var key = keySelector(_v);
            var cache: Array<V> = map[key.toString()] || (map[key.toString()] = []);
            cache.push(_v);
        });

        return map.propertyMaps().map(_map => {
            return {
                Key: (_map.Key as any) as K,
                Value: _map.Value as Array<V>
            } as BitDiamond.Utils.Map<K, Array<V>>;
        });
    }

    Array.prototype.reduce = function <T, U>(seed: U, transformFunc: (in1: U, in2: T) => U): U {
        var arr = this as Array<T>;

        var v = seed;
        for (var cnt = 0; cnt < arr.length; cnt++) {
            v = transformFunc(v, arr[cnt]);
        }
        return v;
    }

    Array.prototype.contains = function <T>(value: T): boolean {
        var arr = this as Array<T>;
        return arr.indexOf(value) >= 0;
    }

}