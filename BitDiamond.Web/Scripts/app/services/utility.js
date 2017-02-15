var Gaia;
(function (Gaia) {
    var Utils;
    (function (Utils) {
        var Services;
        (function (Services) {
            var DomainTransport = (function () {
                function DomainTransport($http, $q) {
                    this.$http = $http;
                    this.$q = $q;
                    this.http = null;
                    var oauthtoken = window.localStorage.getItem(Gaia.Utils.OAuthTokenKey);
                    this.$http.defaults.headers.common.Authorization = 'Bearer ' + (oauthtoken ? JSON.parse(oauthtoken).access_token : '');
                    this.http = $http;
                }
                DomainTransport.prototype.accessDenied = function (callbackParam) {
                    if (callbackParam.Message.startsWith('Access Denied'))
                        return true;
                    else
                        return false;
                };
                DomainTransport.prototype.get = function (url, data, config) {
                    var _this = this;
                    if (data) {
                        data = this.removeSupportProperties(data);
                        config = config || {};
                        config.params = { data: Utils.ToBase64String(Utils.ToUTF8EncodedArray(JSON.stringify(data))) };
                    }
                    return this.http.get(url, config)
                        .error(function (r) {
                        if (_this.accessDenied(r))
                            window.location.href = '/view-server/login/shell';
                    });
                };
                DomainTransport.prototype.delete = function (url, data, config) {
                    var _this = this;
                    if (data) {
                        data = this.removeSupportProperties(data);
                        config = config || {};
                        config.params = { data: Utils.ToBase64String(Utils.ToUTF8EncodedArray(JSON.stringify(data))) };
                    }
                    return this.http.delete(url, config)
                        .error(function (r) {
                        if (_this.accessDenied(r))
                            window.location.href = '/view-server/login/shell';
                    });
                };
                DomainTransport.prototype.head = function (url, config) {
                    var _this = this;
                    return this.http.head(url, config)
                        .error(function (r) {
                        if (_this.accessDenied(r))
                            window.location.href = '/view-server/login/shell';
                    });
                };
                DomainTransport.prototype.jsonp = function (url, data, config) {
                    var _this = this;
                    if (data) {
                        data = this.removeSupportProperties(data);
                        config = config || {};
                        config.data = data;
                    }
                    return this.http.jsonp(url, config)
                        .error(function (r) {
                        if (_this.accessDenied(r))
                            window.location.href = '/view-server/login/shell';
                    });
                };
                DomainTransport.prototype.post = function (url, data, config) {
                    var _this = this;
                    data = this.removeSupportProperties(data);
                    return this.http.post(url, data, config)
                        .error(function (r) {
                        if (_this.accessDenied(r))
                            window.location.href = '/view-server/login/shell';
                    });
                };
                DomainTransport.prototype.put = function (url, data, config) {
                    var _this = this;
                    data = this.removeSupportProperties(data);
                    return this.http.put(url, data, config)
                        .error(function (r) {
                        if (_this.accessDenied(r))
                            window.location.href = '/view-server/login/shell';
                    });
                };
                DomainTransport.prototype.patch = function (url, data, config) {
                    var _this = this;
                    data = this.removeSupportProperties(data);
                    return this.http.patch(url, data, config)
                        .error(function (r) {
                        if (_this.accessDenied(r))
                            window.location.href = '/view-server/login/shell';
                    });
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
                return DomainTransport;
            }());
            DomainTransport.inject = ['$http', '$q'];
            Services.DomainTransport = DomainTransport;
            var DomModelService = (function () {
                function DomModelService() {
                    var _this = this;
                    this.simpleModel = {};
                    this.complexModel = null;
                    var $element = angular.element('#local-models');
                    //simple model
                    $element.attr('simple-models')
                        .project(function (v) { return Gaia.Utils.StringPair.ParseStringPairs(v); })
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
        })(Services = Utils.Services || (Utils.Services = {}));
    })(Utils = Gaia.Utils || (Gaia.Utils = {}));
})(Gaia || (Gaia = {}));
