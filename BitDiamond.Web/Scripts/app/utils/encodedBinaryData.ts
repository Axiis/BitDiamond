

module BitDiamond.Utils {


    export class EncodedBinaryData {

        private _mime: string = null;
        private _metadata: StringPairCollection = new StringPairCollection();
        private _data: Uint8Array;

        //Data
        get Data(): Uint8Array {
            return this._data || new Uint8Array(0);
        }
        set Data(value: Uint8Array) {
            this._data = value;
        }

        //Metadata
        get Metadata(): string {
            return this._metadata.toString();
        }
        set Metadata(value: string) {
            var name = this.Name;
            this._metadata = new StringPairCollection(value);
            if (!Object.isNullOrUndefined(name)) this._metadata.add("name", name);
        }

        //Name
        get Name(): string {
            var pair = this._metadata.pairs.firstOrDefault(_t => _t.Key.toLowerCase() == "name");
            if (Object.isNullOrUndefined(pair)) return null;
            else return pair.Value;
        }
        set Name(value: string) {
            this._metadata.remove("name").add("name", value);
        }


        //Mime
        get Mime(): string {
            return this._mime || this.MimeMap().MimeCode;
        }
        set Mime(value: string) {
            if (!Object.isNullOrUndefined(value)) this._mime = value.trim();
            else this._mime = null;
        }


        
        MimeMap(): MimeMap {
            return Object.isNullOrUndefined(this._mime) ?
                Constants.MimeCodes.toMimeMap(this.Extension()) : //get the default mime for the present (or absent) extension
                new MimeMap(this._mime, this.Extension()); //...or force the given mime on the present (or absent) extension
        }
        Extension(): string {
            if (!Object.isNullOrUndefined(this.Name) && this.Name.contains('.'))
                return this.Name.substr(this.Name.lastIndexOf('.')).trim().toLowerCase();
            else return null;
        }
        Base64(): string {
            return ToBase64String(this.Data);
        }
        DataUri(): string {
            return 'data:' + this.Mime + ';base64,' + this.Base64();
        }
        MetadataTags(): StringPair[] {
            return this._metadata.pairs;
        }

        RawObjectForm(): any {
            return {
                Data: Utils.ToBase64String(this.Data),
                Name: this.Name,
                Mime: this.Mime
            };
        }

        Serializable(): any {
            return this.RawObjectForm();
        }


        constructor(binary: Uint8Array, mime: string, name?: string, metadata?: string) {
            this.Data = binary;
            this.Mime = mime;
            this.Metadata = metadata;
            this.Name = name; //name comes last so it is appended to the metadata
        }

        static Create(data: any): EncodedBinaryData {
            return new EncodedBinaryData(data.Data, data.Mime, data.Name, data.Metadata);
        }
    }
}