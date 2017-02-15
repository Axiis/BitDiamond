
module BitDiamond.Utils {

    const lut = []; for (var i = 0; i < 256; i++) { lut[i] = (i < 16 ? '0' : '') + (i).toString(16); }
    export function NewGuid() {
        var d0 = Math.random() * 0xffffffff | 0;
        var d1 = Math.random() * 0xffffffff | 0;
        var d2 = Math.random() * 0xffffffff | 0;
        var d3 = Math.random() * 0xffffffff | 0;
        return lut[d0 & 0xff] + lut[d0 >> 8 & 0xff] + lut[d0 >> 16 & 0xff] + lut[d0 >> 24 & 0xff] + '-' +
               lut[d1 & 0xff] + lut[d1 >> 8 & 0xff] + '-' + lut[d1 >> 16 & 0x0f | 0x40] + lut[d1 >> 24 & 0xff] + '-' +
               lut[d2 & 0x3f | 0x80] + lut[d2 >> 8 & 0xff] + '-' + lut[d2 >> 16 & 0xff] + lut[d2 >> 24 & 0xff] +
               lut[d3 & 0xff] + lut[d3 >> 8 & 0xff] + lut[d3 >> 16 & 0xff] + lut[d3 >> 24 & 0xff];
    }

    //http://stackoverflow.com/a/18729931
    export function ToUTF8EncodedArray(str: string): number[] {
        var utf8: number[] = [];
        for (var i = 0; i < str.length; i++) {
            var charcode = str.charCodeAt(i);
            if (charcode < 0x80) utf8.push(charcode);
            else if (charcode < 0x800) {
                utf8.push(0xc0 | (charcode >> 6),
                    0x80 | (charcode & 0x3f));
            }
            else if (charcode < 0xd800 || charcode >= 0xe000) {
                utf8.push(0xe0 | (charcode >> 12),
                    0x80 | ((charcode >> 6) & 0x3f),
                    0x80 | (charcode & 0x3f));
            }
            else {
                i++;
                charcode = 0x10000 + (((charcode & 0x3ff) << 10) | (str.charCodeAt(i) & 0x3ff));
                utf8.push(0xf0 | (charcode >> 18),
                    0x80 | ((charcode >> 12) & 0x3f),
                    0x80 | ((charcode >> 6) & 0x3f),
                    0x80 | (charcode & 0x3f));
            }
        }
        return utf8;
    }

    //https://gist.github.com/jonleighton/958841
    const _b64Encodings: string = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/';
    export function ToBase64String(arrayBuffer: number[] | Uint8Array): string {
        var base64 = '';

        var bytes = new Uint8Array(arrayBuffer);
        var byteLength = bytes.byteLength;
        var byteRemainder = byteLength % 3;
        var mainLength = byteLength - byteRemainder;

        var a, b, c, d;
        var chunk;

        for (var i = 0; i < mainLength; i = i + 3) {
            // Combine the three bytes into a single integer
            chunk = (bytes[i] << 16) | (bytes[i + 1] << 8) | bytes[i + 2];
            
            a = (chunk & 16515072) >> 18; // 16515072 = (2^6 - 1) << 18
            b = (chunk & 258048) >> 12; // 258048   = (2^6 - 1) << 12
            c = (chunk & 4032) >> 6; // 4032     = (2^6 - 1) << 6
            d = chunk & 63;               // 63       = 2^6 - 1
            
            base64 += _b64Encodings[a] + _b64Encodings[b] + _b64Encodings[c] + _b64Encodings[d];
        }
        
        if (byteRemainder == 1) {
            chunk = bytes[mainLength];

            a = (chunk & 252) >> 2; // 252 = (2^6 - 1) << 2            
            b = (chunk & 3) << 4; // 3   = 2^2 - 1

            base64 += _b64Encodings[a] + _b64Encodings[b] + '==';
        } else if (byteRemainder == 2) {
            chunk = (bytes[mainLength] << 8) | bytes[mainLength + 1];

            a = (chunk & 64512) >> 10; // 64512 = (2^6 - 1) << 10
            b = (chunk & 1008) >> 4; // 1008  = (2^6 - 1) << 4
            
            c = (chunk & 15) << 2; // 15    = 2^4 - 1

            base64 += _b64Encodings[a] + _b64Encodings[b] + _b64Encodings[c] + '=';
        }

        return base64;
    }

    export function FromBase64String(base64: string): Uint8Array {
        var raw = window.atob(base64);
        var rawLength = raw.length;
        var array = new Uint8Array(new ArrayBuffer(rawLength));

        for (i = 0; i < rawLength; i++) {
            array[i] = raw.charCodeAt(i);
        }
        return array;
    }

    //Class that implements pagination
    export class SequencePage<Data>{

        PageIndex: number;
        SequenceLength: number;
        PageCount: number;
        PageSize: number;

        Page: Data[] = [];

        constructor(page: Data[], sequenceLength: number, pageSize?: number, pageIndex?: number) {

            if (page == null || pageIndex < 0 || sequenceLength < 0) throw "invalid page";
            this.PageIndex = pageIndex || 0;
            this.SequenceLength = sequenceLength;
            this.Page = page;
            this.PageSize = pageSize || page.length;
            this.PageCount = Math.floor(this.SequenceLength / this.PageSize) + (this.SequenceLength % this.PageSize > 0 ? 1 : 0);
        }

        /// <summary>
        /// Returns an array containing page indexes for pages immediately adjecent to the current page.
        /// The span indicates how many pages indexes to each side of the current page should be returned
        /// </summary>
        /// <param name="span"></param>
        /// <returns></returns>
        AdjacentIndexes(span: number): Array<number> {

            if (span < 0) throw 'invalid span: ' + span;
            var fullspan = (span * 2) + 1,
                start = 0,
                count = 0;

            if (fullspan >= this.PageCount) count = this.PageCount;

            else {
                start = this.PageIndex - span;
                count = fullspan;

                if (start < 0) start = 0;
                if ((this.PageIndex + span) >= this.PageCount) start = this.PageCount - fullspan;
            }

            var pages: Array<number> = [];
            for (var indx = 0; indx < count; indx++) pages.push(indx + start);

            return pages;
        }
    } 

    //http://stackoverflow.com/a/21294925/4137383
    //Enum helper
    export class EnumHelper {
        static getNamesAndValues<T extends number>(e: any) {
            return EnumHelper.getNames(e).map(n => ({ name: n, value: e[n] as T }));
        }

        static getNames(e: any) {
            return EnumHelper.getObjValues(e).filter(v => typeof v === "string") as string[];
        }

        static getValues<T extends number>(e: any) {
            return EnumHelper.getObjValues(e).filter(v => typeof v === "number") as T[];
        }

        private static getObjValues(e: any): (number | string)[] {
            return Object.keys(e).map(k => e[k]);
        }
    }


    export class MimeMap {
        MimeCode: string;
        Extensions: string;

        constructor(code?: string, extension?: string) {
            this.MimeCode = code;
            this.Extensions = Object.isNullOrUndefined(extension) ? null :
                extension.startsWith('.') ? extension : '.' + extension;
        }
    }
}