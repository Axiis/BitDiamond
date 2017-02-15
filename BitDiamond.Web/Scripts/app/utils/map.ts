

module BitDiamond.Utils {

    //key value pair
    export interface Map<K, V> {
        Key: K;
        Value: V;
    }

    export enum Encoding {
        InlineCss
    }   

    export class StringPair implements Map<string, string> {
        Key: string;
        Value: string;
        
        constructor(key: string, value: string) {
            this.Key = StringPair.DecodeDelimiters(key);
            this.Value = StringPair.DecodeDelimiters(value);
        }

        toString(encoding?: Encoding): string {

            //if(encoding == ...) //encode

            //else //default encoding
            return StringPair.EncodeDelimiters(this.Key) + ':' + StringPair.EncodeDelimiters(this.Value) + ';';
        }



        static ParseStringPairs(value: string, encoding?: Encoding): Array<StringPair> {
            encoding = encoding || Encoding.InlineCss;
            if (encoding == Encoding.InlineCss) return value.split(';')
                .filter(_part => !Object.isNullOrUndefined(_part) && _part.length > 0)
                .map(v => {
                    var parts = v.split(':').filter(_part => !Object.isNullOrUndefined(_part) && _part.length > 0);
                    var sp = new StringPair(parts[0].trim(), parts.length > 1 ? parts[1].trim() : null);
                    return sp;
                })
                .filter(pair => pair.Key != null && pair.Key != '');

            //unknown encoding
            else return [];
        }

        static EncodeDelimiters(value: string): string {
            return value.replace(':', '##col').replace(';', '##scol');
        }

        static DecodeDelimiters(value: string): string {
            return value.replace('##col', ':').replace('##scol', ';');
        }
    } 

    export class StringPairCollection {
        private _pairs: StringPair[] = [];

        get pairs(): StringPair[] {
            return this._pairs;
        }

        getOrAdd(key: string, generator: (k: string) => string): StringPair {
            var pair = this._pairs.firstOrDefault(_p => _p.Key == key);
            if (Object.isNullOrUndefined(pair)) this._pairs.push(pair = new StringPair(key, generator(key)));

            return pair;
        }

        add(key: string, value: string): StringPairCollection {
            this.getOrAdd(key, _k => value).Value = value;
            return this;
        }
        remove(key: string): StringPairCollection {
            var indx = 0;
            for (; indx < this._pairs.length; indx++) {
                if (this._pairs[indx].Key == key) break;
            }

            if (indx < this._pairs.length) this._pairs.removeAt(indx);
            return this;
        }

        toString(): string {
            return this._pairs.reduce('', (sbuff, next) => sbuff + ' ' + next.toString());
        }

        constructor(pairs?: string) {
            if (!Object.isNullOrUndefined(pairs))
                this._pairs.push(...StringPair.ParseStringPairs(pairs));
        }

    }
}