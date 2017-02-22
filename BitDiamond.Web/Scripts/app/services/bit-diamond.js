var BitDiamond;
(function (BitDiamond) {
    var Services;
    (function (Services) {
        var Account = (function () {
            function Account(__transport, $q) {
                this.__transport = __transport;
                this.$q = $q;
            }
            Account.prototype.registerUser = function (targetUser, referrer, credential) {
                return this.__transport.post('/api/accounts/users', {
                    TargetUser: targetUser,
                    Referrer: referrer,
                    Credential: credential
                });
            };
            Account.prototype.registerAdmin = function (targetUser, credential) {
                return this.__transport.post('/api/accounts/admins', {
                    TargetUser: targetUser,
                    Credential: credential
                });
            };
            Account.prototype.deactivateUser = function (targetUser) {
                return this.__transport.put('/api/accounts/users/deactivate', {
                    TargetUser: targetUser
                });
            };
            Account.prototype.blockUser = function (targetUser) {
                return this.__transport.put('/api/accounts/users/block', {
                    TargetUser: targetUser
                });
            };
            Account.prototype.requestUserActivation = function (targetUser) {
                return this.__transport.put('/api/accounts/users/activations', {
                    TargetUser: targetUser
                });
            };
            Account.prototype.verifyUserActivation = function (targetUser, token) {
                return this.__transport.put('/api/accounts/users/activations/verify', {
                    TargetUser: targetUser,
                    Token: token
                });
            };
            Account.prototype.requestPasswordReset = function (targetUser) {
                return this.__transport.put('/api/accounts/users/credentials/reset-tokens', {
                    TargetUser: targetUser,
                    Metadata: {
                        Name: 'Password',
                        Access: Pollux.Models.Access.Secret
                    }
                });
            };
            Account.prototype.verifyPasswordReset = function (targetUser, token, $new) {
                return this.__transport.put('/api/accounts/users/credentials/reset-tokens/verify', {
                    TargetUser: targetUser,
                    Token: token,
                    New: {
                        Value: BitDiamond.Utils.ToBase64String(BitDiamond.Utils.ToUTF8EncodedArray($new)),
                        Metadata: {
                            Name: 'Password',
                            Access: Pollux.Models.Access.Secret
                        }
                    }
                });
            };
            Account.prototype.modifyBiodata = function (data) {
                return this.__transport.put('/api/accounts/biodata', data);
            };
            Account.prototype.getBiodata = function () {
                return this.__transport.get('/api/accounts/biodata');
            };
            Account.prototype.modifyContactdata = function (data) {
                return this.__transport.put('/api/accounts/biodata', data);
            };
            Account.prototype.getContactdata = function (data) {
                return this.__transport.get('/api/accounts/biodata');
            };
            Account.prototype.addData = function (data) {
                return this.__transport.post('/api/accounts/userdata', {
                    Data: data
                });
            };
            Account.prototype.modifyData = function (data) {
                return this.__transport.put('/api/accounts/userdata', {
                    Data: data
                });
            };
            Account.prototype.removeData = function (names) {
                return this.__transport.delete('/api/accounts/userdata', {
                    Names: names
                });
            };
            Account.prototype.getUserData = function () {
                return this.__transport.get('/api/accounts/userdata');
            };
            Account.prototype.getUserDataByName = function (name) {
                return this.__transport.get('/api/accounts/userdata/filter', {
                    Name: name
                });
            };
            Account.prototype.updateProfileImage = function (data, oldUrl) {
                return this.__transport.get('/api/accounts/userdata/filter', {
                    Image: data,
                    OldImageUrl: oldUrl
                });
            };
            Account.prototype.getUserRoles = function () {
                return this.__transport.get('/api/accounts/users/roles');
            };
            Account.prototype.getUser = function () {
                return this.__transport.get('/api/accounts/users/current');
            };
            Account.prototype.signin = function (email, password) {
                return this.__transport.postUrlEncoded('/tokens', {
                    grant_type: 'password',
                    username: email,
                    password: password
                }).then(function (opr) {
                    window.localStorage.setItem(BitDiamond.Utils.Constants.Misc_OAuthTokenKey, JSON.stringify(opr));
                    return {
                        Succeeded: true
                    };
                });
            };
            Account.prototype.signout = function () {
                var tokenObj = JSON.parse(window.localStorage.getItem(BitDiamond.Utils.Constants.Misc_OAuthTokenKey));
                if (!Object.isNullOrUndefined(tokenObj)) {
                    return this.__transport.post('/api/accounts/users/tokens/invalidate', {
                        Token: tokenObj.access_token
                    }).finally(function () {
                        window.localStorage.removeItem(BitDiamond.Utils.Constants.Misc_OAuthTokenKey);
                        window.location.href = '/account/index';
                    });
                }
            };
            return Account;
        }());
        Services.Account = Account;
        var Profile = (function () {
            function Profile() {
            }
            return Profile;
        }());
        Services.Profile = Profile;
        var Dashboard = (function () {
            function Dashboard() {
            }
            return Dashboard;
        }());
        Services.Dashboard = Dashboard;
    })(Services = BitDiamond.Services || (BitDiamond.Services = {}));
})(BitDiamond || (BitDiamond = {}));
