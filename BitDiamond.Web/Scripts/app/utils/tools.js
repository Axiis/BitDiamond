var BitDiamond;
(function (BitDiamond) {
    var Utils;
    (function (Utils) {
        var lut = [];
        for (var i = 0; i < 256; i++) {
            lut[i] = (i < 16 ? '0' : '') + (i).toString(16);
        }
        function NewGuid() {
            var d0 = Math.random() * 0xffffffff | 0;
            var d1 = Math.random() * 0xffffffff | 0;
            var d2 = Math.random() * 0xffffffff | 0;
            var d3 = Math.random() * 0xffffffff | 0;
            return lut[d0 & 0xff] + lut[d0 >> 8 & 0xff] + lut[d0 >> 16 & 0xff] + lut[d0 >> 24 & 0xff] + '-' +
                lut[d1 & 0xff] + lut[d1 >> 8 & 0xff] + '-' + lut[d1 >> 16 & 0x0f | 0x40] + lut[d1 >> 24 & 0xff] + '-' +
                lut[d2 & 0x3f | 0x80] + lut[d2 >> 8 & 0xff] + '-' + lut[d2 >> 16 & 0xff] + lut[d2 >> 24 & 0xff] +
                lut[d3 & 0xff] + lut[d3 >> 8 & 0xff] + lut[d3 >> 16 & 0xff] + lut[d3 >> 24 & 0xff];
        }
        Utils.NewGuid = NewGuid;
        //http://stackoverflow.com/a/18729931
        function ToUTF8EncodedArray(str) {
            var utf8 = [];
            for (var i = 0; i < str.length; i++) {
                var charcode = str.charCodeAt(i);
                if (charcode < 0x80)
                    utf8.push(charcode);
                else if (charcode < 0x800) {
                    utf8.push(0xc0 | (charcode >> 6), 0x80 | (charcode & 0x3f));
                }
                else if (charcode < 0xd800 || charcode >= 0xe000) {
                    utf8.push(0xe0 | (charcode >> 12), 0x80 | ((charcode >> 6) & 0x3f), 0x80 | (charcode & 0x3f));
                }
                else {
                    i++;
                    charcode = 0x10000 + (((charcode & 0x3ff) << 10) | (str.charCodeAt(i) & 0x3ff));
                    utf8.push(0xf0 | (charcode >> 18), 0x80 | ((charcode >> 12) & 0x3f), 0x80 | ((charcode >> 6) & 0x3f), 0x80 | (charcode & 0x3f));
                }
            }
            return utf8;
        }
        Utils.ToUTF8EncodedArray = ToUTF8EncodedArray;
        //http://stackoverflow.com/a/22373135/4137383
        function FromUTF8EncodedArray(array) {
            var out, i, len, c;
            var char2, char3;
            out = "";
            len = array.length;
            i = 0;
            while (i < len) {
                c = array[i++];
                switch (c >> 4) {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                        // 0xxxxxxx
                        out += String.fromCharCode(c);
                        break;
                    case 12:
                    case 13:
                        // 110x xxxx   10xx xxxx
                        char2 = array[i++];
                        out += String.fromCharCode(((c & 0x1F) << 6) | (char2 & 0x3F));
                        break;
                    case 14:
                        // 1110 xxxx  10xx xxxx  10xx xxxx
                        char2 = array[i++];
                        char3 = array[i++];
                        out += String.fromCharCode(((c & 0x0F) << 12) |
                            ((char2 & 0x3F) << 6) |
                            ((char3 & 0x3F) << 0));
                        break;
                }
            }
            return out;
        }
        Utils.FromUTF8EncodedArray = FromUTF8EncodedArray;
        //https://gist.github.com/jonleighton/958841
        var _b64Encodings = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/';
        function ToBase64String(arrayBuffer) {
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
                d = chunk & 63; // 63       = 2^6 - 1
                base64 += _b64Encodings[a] + _b64Encodings[b] + _b64Encodings[c] + _b64Encodings[d];
            }
            if (byteRemainder == 1) {
                chunk = bytes[mainLength];
                a = (chunk & 252) >> 2; // 252 = (2^6 - 1) << 2            
                b = (chunk & 3) << 4; // 3   = 2^2 - 1
                base64 += _b64Encodings[a] + _b64Encodings[b] + '==';
            }
            else if (byteRemainder == 2) {
                chunk = (bytes[mainLength] << 8) | bytes[mainLength + 1];
                a = (chunk & 64512) >> 10; // 64512 = (2^6 - 1) << 10
                b = (chunk & 1008) >> 4; // 1008  = (2^6 - 1) << 4
                c = (chunk & 15) << 2; // 15    = 2^4 - 1
                base64 += _b64Encodings[a] + _b64Encodings[b] + _b64Encodings[c] + '=';
            }
            return base64;
        }
        Utils.ToBase64String = ToBase64String;
        function FromBase64String(base64) {
            var raw = window.atob(base64);
            var rawLength = raw.length;
            var array = new Uint8Array(new ArrayBuffer(rawLength));
            for (i = 0; i < rawLength; i++) {
                array[i] = raw.charCodeAt(i);
            }
            return array;
        }
        Utils.FromBase64String = FromBase64String;
        //Class that implements pagination
        var SequencePage = (function () {
            function SequencePage(page, sequenceLength, pageSize, pageIndex) {
                this.Page = [];
                if (page == null || pageIndex < 0 || sequenceLength < 0)
                    throw "invalid page";
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
            SequencePage.prototype.AdjacentIndexes = function (span) {
                if (span < 0)
                    throw 'invalid span: ' + span;
                var fullspan = (span * 2) + 1, start = 0, count = 0;
                if (fullspan >= this.PageCount)
                    count = this.PageCount;
                else {
                    start = this.PageIndex - span;
                    count = fullspan;
                    if (start < 0)
                        start = 0;
                    if ((this.PageIndex + span) >= this.PageCount)
                        start = this.PageCount - fullspan;
                }
                var pages = [];
                for (var indx = 0; indx < count; indx++)
                    pages.push(indx + start);
                return pages;
            };
            return SequencePage;
        }());
        Utils.SequencePage = SequencePage;
        //http://stackoverflow.com/a/21294925/4137383
        //Enum helper
        var EnumHelper = (function () {
            function EnumHelper() {
            }
            EnumHelper.getNamesAndValues = function (e) {
                return EnumHelper.getNames(e).map(function (n) { return ({ name: n, value: e[n] }); });
            };
            EnumHelper.getNames = function (e) {
                return EnumHelper.getObjValues(e).filter(function (v) { return typeof v === "string"; });
            };
            EnumHelper.getValues = function (e) {
                return EnumHelper.getObjValues(e).filter(function (v) { return typeof v === "number"; });
            };
            EnumHelper.getObjValues = function (e) {
                return Object.keys(e).map(function (k) { return e[k]; });
            };
            return EnumHelper;
        }());
        Utils.EnumHelper = EnumHelper;
        var MimeMap = (function () {
            function MimeMap(code, extension) {
                this.MimeCode = code;
                this.Extensions = Object.isNullOrUndefined(extension) ? null :
                    extension.startsWith('.') ? extension : '.' + extension;
            }
            return MimeMap;
        }());
        Utils.MimeMap = MimeMap;
    })(Utils = BitDiamond.Utils || (BitDiamond.Utils = {}));
})(BitDiamond || (BitDiamond = {}));
//# sourceMappingURL=tools.js.map