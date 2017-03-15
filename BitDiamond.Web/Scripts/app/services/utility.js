var BitDiamond;
(function (BitDiamond) {
    var Utils;
    (function (Utils) {
        var Services;
        (function (Services) {
            var DomainTransport = (function () {
                function DomainTransport($http, $q, __notify) {
                    this.$http = $http;
                    this.$q = $q;
                    this.__notify = __notify;
                    this.http = null;
                    var oauthtoken = window.localStorage.getItem(BitDiamond.Utils.Constants.Misc_OAuthTokenKey);
                    if (!Object.isNullOrUndefined(oauthtoken))
                        this.$http.defaults.headers.common.Authorization = 'Bearer ' + JSON.parse(oauthtoken).access_token;
                    this.http = $http;
                }
                DomainTransport.prototype.get = function (url, data, config) {
                    if (data) {
                        data = this.removeSupportProperties(data);
                        config = config || {};
                        config.params = { data: Utils.ToBase64String(Utils.ToUTF8EncodedArray(JSON.stringify(data))) };
                    }
                    return this.http.get(url, config).then(function (args) { return args.data; }, this.treatError);
                };
                DomainTransport.prototype.getUrlEncoded = function (url, data, config) {
                    if (Object.isNullOrUndefined(config))
                        config = {
                            headers: {}
                        };
                    config.headers['Content-Type'] = 'application/x-www-form-urlencoded';
                    config.headers['Accept'] = 'application/json';
                    config.data = data;
                    config.transformRequest = function (obj) {
                        var str = [];
                        for (var p in obj)
                            str.push(encodeURIComponent(p) + "=" + encodeURIComponent(obj[p]));
                        return str.join("&");
                    };
                    return this.http.get(url, config).then(function (args) { return args.data; });
                };
                DomainTransport.prototype.delete = function (url, data, config) {
                    if (data) {
                        data = this.removeSupportProperties(data);
                        config = config || {};
                        config.params = { data: Utils.ToBase64String(Utils.ToUTF8EncodedArray(JSON.stringify(data))) };
                    }
                    return this.http.delete(url, config).then(function (args) { return args.data; }, this.treatError);
                };
                DomainTransport.prototype.head = function (url, config) {
                    return this.http.head(url, config).then(function (args) { return args.data; }, this.treatError);
                };
                DomainTransport.prototype.jsonp = function (url, data, config) {
                    if (data) {
                        data = this.removeSupportProperties(data);
                        config = config || {};
                        config.data = data;
                    }
                    return this.http.jsonp(url, config).then(function (args) { return args.data; }, this.treatError);
                };
                DomainTransport.prototype.post = function (url, data, config) {
                    data = this.removeSupportProperties(data);
                    return this.http.post(url, data, config).then(function (args) { return args.data; }, this.treatError);
                };
                DomainTransport.prototype.postUrlEncoded = function (url, data, config) {
                    if (Object.isNullOrUndefined(config))
                        config = {
                            headers: {}
                        };
                    config.headers['Content-Type'] = 'application/x-www-form-urlencoded';
                    config.headers['Accept'] = 'application/json';
                    config.transformRequest = function (obj) {
                        var str = [];
                        for (var p in obj)
                            str.push(encodeURIComponent(p) + "=" + encodeURIComponent(obj[p]));
                        return str.join("&");
                    };
                    return this.http.post(url, data, config).then(function (args) { return args.data; });
                };
                DomainTransport.prototype.put = function (url, data, config) {
                    data = this.removeSupportProperties(data);
                    return this.http.put(url, data, config).then(function (args) { return args.data; }, this.treatError);
                };
                DomainTransport.prototype.patch = function (url, data, config) {
                    data = this.removeSupportProperties(data);
                    return this.http.patch(url, data, config).then(function (args) { return args.data; }, this.treatError);
                };
                DomainTransport.prototype.removeSupportProperties = function (data) {
                    var _this = this;
                    if (Object.isNullOrUndefined(data))
                        return data;
                    var _data = {};
                    for (var key in data) {
                        var _val = data[key];
                        var _type = typeof _val;
                        if (key.startsWith('$'))
                            continue;
                        else if (Array.isArray(_val))
                            _val = _val.map(function (_next) { return _this.removeSupportProperties(_next); });
                        else if (_type == 'object')
                            _val = this.removeSupportProperties(_val);
                        _data[key] = _val;
                    }
                    return _data;
                };
                DomainTransport.prototype.removeRecurrsion = function (data, _cache) {
                    if (Object.isNullOrUndefined(data))
                        return data;
                    var cache = _cache || [];
                    for (var key in data) {
                        var val = data[key];
                        if (typeof val == 'object') {
                            if (cache.contains(val))
                                return null;
                            else {
                                cache.push(val);
                                this.removeRecurrsion(val);
                            }
                        }
                    }
                };
                DomainTransport.prototype.treatError = function (arg) {
                    //access denied
                    if (arg.status == 401)
                        window.location.href = Utils.Constants.URL_Login;
                    else if (arg.status == 409)
                        this.__notify.error("A Conflict was caused by your previous request.", "Oops!");
                    //other errors...
                    return this.$q.reject(arg);
                };
                return DomainTransport;
            }());
            DomainTransport.inject = ['$http', '$q', '__notify'];
            Services.DomainTransport = DomainTransport;
            var DomModelService = (function () {
                function DomModelService() {
                    var _this = this;
                    this.simpleModel = {};
                    this.complexModel = null;
                    var $element = angular.element('#local-models');
                    //simple model
                    $element.attr('simple-models')
                        .project(function (v) { return BitDiamond.Utils.StringPair.ParseStringPairs(v); })
                        .forEach(function (v) {
                        _this.simpleModel[v.Key] = v.Value;
                    });
                    //complex model
                    try {
                        this.complexModel = JSON.parse($element.text());
                    }
                    catch (e) {
                        this.complexModel = {};
                    }
                }
                return DomModelService;
            }());
            Services.DomModelService = DomModelService;
            var NotifyService = (function () {
                function NotifyService() {
                    toastr.options['closeButton'] = true;
                }
                NotifyService.prototype.success = function (message, title) {
                    console.log(message);
                    toastr.success(this.parse(message), title);
                };
                NotifyService.prototype.error = function (message, title) {
                    console.error(message);
                    toastr.error(this.parse(message), title);
                };
                NotifyService.prototype.info = function (message, title) {
                    console.info(message);
                    toastr.info(this.parse(message), title);
                };
                NotifyService.prototype.warning = function (message, title) {
                    console.warn(message);
                    toastr.warning(this.parse(message), title);
                };
                NotifyService.prototype.option = function (setting, value) {
                    toastr.options[setting] = value;
                };
                NotifyService.prototype.parse = function (message) {
                    if (!message || message.length <= 0)
                        return " &nbsp; ";
                    else
                        return message;
                };
                return NotifyService;
            }());
            Services.NotifyService = NotifyService;
            var UserContext = (function () {
                function UserContext(__notify, __account, $q) {
                    var _this = this;
                    this.__notify = __notify;
                    this.__account = __account;
                    this.$q = $q;
                    //load user object
                    this.user = this.__account.getUser().then(function (opr) {
                        return _this.$q.resolve(opr.Result);
                    });
                    //load profile image
                    this.profileImageRef = this.__account.getUserDataByName(Utils.Constants.UserData_ProfileImage).then(function (opr) {
                        return _this.$q.resolve(opr.Result);
                    });
                    //load user roles
                    this.userRoles = this.__account.getUserRoles().then(function (opr) {
                        return _this.$q.resolve(opr.Result);
                    });
                    //load user biodata
                    this.userBio = this.__account.getBiodata().then(function (opr) {
                        return _this.$q.resolve(opr.Result);
                    });
                    //load user contact data
                    this.userContact = this.__account.getContactdata().then(function (opr) {
                        return _this.$q.resolve(opr.Result);
                    });
                    //load ref code
                    this.userRef = this.__account.getCurrentUserRef().then(function (opr) {
                        return _this.$q.resolve(opr.Result);
                    });
                    //load any other needed data
                }
                return UserContext;
            }());
            Services.UserContext = UserContext;
        })(Services = Utils.Services || (Utils.Services = {}));
    })(Utils = BitDiamond.Utils || (BitDiamond.Utils = {}));
})(BitDiamond || (BitDiamond = {}));
