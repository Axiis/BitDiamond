
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
            return this.http.get<T>(url, config).then(args => {
                return args.data;
            }, err => {
                return this.treatError(err);
            });
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
            return this.http.delete<T>(url, config).then(args => args.data, err => {
                return this.treatError(err);
            });
        }
        
        head<T>(url: string, config?: angular.IRequestShortcutConfig): ng.IPromise<T> {
            return this.http.head<T>(url, config).then(args => args.data, err => {
                return this.treatError(err);
            });
        }
        
        jsonp<T>(url: string, data: any, config?: angular.IRequestShortcutConfig): ng.IPromise<T> {
            if (data) {
                data = this.removeSupportProperties(data);
                config = config || {};
                config.data = data;
            }         
            return this.http.jsonp<T>(url, config).then(args => args.data, err => {
                return this.treatError(err);
            });
        }
        
        post<T>(url: string, data: any, config?: angular.IRequestShortcutConfig): ng.IPromise<T> {
            data = this.removeSupportProperties(data);
            return this.http.post<T>(url, data, config).then(args => args.data, err => {
                return this.treatError(err);
            });
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
            return this.http.put<T>(url, data, config).then(args => args.data, err => {
                return this.treatError(err);
            });
        }
        
        patch<T>(url: string, data: any, config?: angular.IRequestShortcutConfig): ng.IPromise<T> {
            data = this.removeSupportProperties(data);
            return this.http.patch<T>(url, data, config).then(args => args.data, err => {
                return this.treatError(err);
            });
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

            if (!Object.isNullOrUndefined(arg.status)) {

                //access denied
                if (arg.status == 401) this.__notify.error("Access Denied.", "Oops!");

                //conflict
                else if (arg.status == 409) this.__notify.error("A Conflict was caused by your previous request.", "Oops!");

                //other errors...
            }
            else {
                this.__notify.error((arg as any).message || 'unknown error');
            }

            return this.$q.reject(arg) as ng.IPromise<ng.IHttpPromiseCallbackArg<T>>;
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
    

    export class UserContext {

        user: ng.IPromise<Pollux.Models.IUser>;
        userRoles: ng.IPromise<string[]>;
        userBio: ng.IPromise<Pollux.Models.IBioData>;
        userContact: ng.IPromise<Pollux.Models.IContactData>;
        profileImageRef: ng.IPromise<Pollux.Models.IUserData>;
        userRef: ng.IPromise<Models.IReferralNode>;


        __account: BitDiamond.Services.Account;
        __notify: BitDiamond.Utils.Services.NotifyService;

        $q: ng.IQService;

        constructor(__notify, __account, $q) {
            this.__notify = __notify;
            this.__account = __account;
            this.$q = $q;

            //load user object
            this.user = this.__account.getUser().then(opr => opr.Result);

            //load profile image
            this.profileImageRef = this.__account.getUserDataByName(Utils.Constants.UserData_ProfileImage).then(opr => {
                return this.$q.resolve(opr.Result);
            });

            //load user roles
            this.userRoles = this.__account.getUserRoles().then(opr => {
                return this.$q.resolve(opr.Result);
            });

            //load user biodata
            this.userBio = this.__account.getBiodata().then(opr => {
                return this.$q.resolve(opr.Result);
            });

            //load user contact data
            this.userContact = this.__account.getContactdata().then(opr => {
                return this.$q.resolve(opr.Result);
            });

            //load ref code
            this.userRef = this.__account.getCurrentUserRef().then(opr => {
                return this.$q.resolve(opr.Result);
            });

            //load any other needed data
        }
    }

}