
module BitDiamond.Utils.Services {

    export class DomainTransport {

        http: angular.IHttpService = null;

        static inject = ['$http', '$q', '__notify'];
        constructor(private $http: angular.IHttpService, private $q: angular.IQService, private __notify: BitDiamond.Utils.Services.NotifyService) {
            var oauthtoken = window.localStorage.getItem(BitDiamond.Utils.Constants.Misc_OAuthTokenKey);
            if (!Object.isNullOrUndefined(oauthtoken))
                this.$http.defaults.headers.common.Authorization = 'Bearer ' + JSON.parse(oauthtoken).access_token;
            this.http = $http;
        }
        
        get<T>(url: string, data?: any, config?: angular.IRequestShortcutConfig): ng.IPromise<T> {    
            if (data) {
                data = this.removeSupportProperties(data);
                config = config || {};
                config.params = { data: Utils.ToBase64String(Utils.ToUTF8EncodedArray(JSON.stringify(data))) };
            }
            return this.http.get<T>(url, config).then(args => args.data, this.treatError);
        }
        getUrlEncoded<T>(url: string, data: any, config?: ng.IRequestShortcutConfig): ng.IPromise<T> {

            if (Object.isNullOrUndefined(config)) config = {
                headers: {}
            };

            config.headers['Content-Type'] = 'application/x-www-form-urlencoded';
            config.headers['Accept'] = 'application/json';
            config.data = data;
            config.transformRequest = obj => {
                var str = [];
                for (var p in obj)
                    str.push(encodeURIComponent(p) + "=" + encodeURIComponent(obj[p]));
                return str.join("&");
            }

            return this.http.get<T>(url, config).then(args => args.data);
        }
        
        delete<T>(url: string, data: any, config?: angular.IRequestShortcutConfig): ng.IPromise<T> {
            if (data) {
                data = this.removeSupportProperties(data);
                config = config || {};
                config.params = { data: Utils.ToBase64String(Utils.ToUTF8EncodedArray(JSON.stringify(data))) };
            }
            return this.http.delete<T>(url, config).then(args => args.data, this.treatError);
        }
        
        head<T>(url: string, config?: angular.IRequestShortcutConfig): ng.IPromise<T> {
            return this.http.head<T>(url, config).then(args => args.data, this.treatError);
        }
        
        jsonp<T>(url: string, data: any, config?: angular.IRequestShortcutConfig): ng.IPromise<T> {
            if (data) {
                data = this.removeSupportProperties(data);
                config = config || {};
                config.data = data;
            }         
            return this.http.jsonp<T>(url, config).then(args => args.data, this.treatError);
        }
        
        post<T>(url: string, data: any, config?: angular.IRequestShortcutConfig): ng.IPromise<T> {
            data = this.removeSupportProperties(data);
            return this.http.post<T>(url, data, config).then(args => args.data, this.treatError);
        }
        postUrlEncoded<T>(url: string, data: any, config?: ng.IRequestShortcutConfig): ng.IPromise<T> {

            if (Object.isNullOrUndefined(config)) config = {
                headers: {}
            };

            config.headers['Content-Type'] = 'application/x-www-form-urlencoded';
            config.headers['Accept'] = 'application/json';
            config.transformRequest = obj => {
                var str = [];
                for (var p in obj)
                    str.push(encodeURIComponent(p) + "=" + encodeURIComponent(obj[p]));
                return str.join("&");
            }

            return this.http.post<T>(url, data, config).then(args => args.data);
        }
        
        put<T>(url: string, data: any, config?: angular.IRequestShortcutConfig): ng.IPromise<T> {
            data = this.removeSupportProperties(data);
            return this.http.put<T>(url, data, config).then(args => args.data, this.treatError);
        }
        
        patch<T>(url: string, data: any, config?: angular.IRequestShortcutConfig): ng.IPromise<T> {
            data = this.removeSupportProperties(data);
            return this.http.patch<T>(url, data, config).then(args => args.data, this.treatError);
        }

        private removeSupportProperties(data: any): any {
            if (Object.isNullOrUndefined(data)) return data;
            
            var _data = {};

            for (var key in data) {
                var _val = data[key];
                var _type = typeof _val;

                if (key.startsWith('$')) continue;
                else if (Array.isArray(_val)) _val = (<any[]>_val).map(_next => this.removeSupportProperties(_next));
                else if (_type == 'object') _val = this.removeSupportProperties(_val);

                _data[key] = _val;
            }
            return _data;
        }        

        private removeRecurrsion(data: any, _cache?: any[]) {
            if (Object.isNullOrUndefined(data)) return data;

            var cache = _cache || [];

            for (var key in data) {
                var val = data[key];
                if (typeof val == 'object') {
                    if (cache.contains(val)) return null;
                    else {
                        cache.push(val);
                        this.removeRecurrsion(val);
                    }
                }
            }
        }

        private treatError<T>(arg: ng.IHttpPromiseCallbackArg<T>): ng.IPromise<ng.IHttpPromiseCallbackArg<T>> {

            //access denied
            if (arg.status == 401) window.location.href = Constants.URL_Login;

            //conflict
            else if (arg.status == 409) this.__notify.error("A Conflict was caused by your previous request.", "Oops!");

            //other errors...


            return this.$q.reject(arg) as ng.IPromise<ng.IHttpPromiseCallbackArg<T>>;
        }
    }


    export class DomModelService {

        public simpleModel: any = {};
        public complexModel: any = null;

        constructor() {
            var $element = angular.element('#local-models');

            //simple model
            $element.attr('simple-models')
                .project((v: string) => BitDiamond.Utils.StringPair.ParseStringPairs(v))
                .forEach(v => {
                    this.simpleModel[v.Key] = v.Value;
                });

            //complex model
            try {
                this.complexModel = JSON.parse($element.text());
            }
            catch (e) {
                this.complexModel = {};
            }
        }
    }


    export class NotifyService {

        constructor() {
            toastr.options['closeButton'] = true;
        }

        success(message: string, title?: string) {
            console.log(message);
            toastr.success(this.parse(message), title);
        }
        error(message: string, title?: string) {            
            console.error(message);
            toastr.error(this.parse(message), title);
        }
        info(message: string, title?: string) {
            console.info(message);
            toastr.info(this.parse(message), title);
        }
        warning(message: string, title?: string) {
            console.warn(message);
            toastr.warning(this.parse(message), title);
        }
        option(setting: string, value: any) {
            toastr.options[setting] = value;
        }

        parse(message: string) {
            if (!message || message.length <= 0) return " &nbsp; ";
            else return message;
        }
    }

}