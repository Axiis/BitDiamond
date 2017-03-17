var BitDiamond;
(function (BitDiamond) {
    var Controllers;
    (function (Controllers) {
        var Account;
        (function (Account) {
            var Signin = (function () {
                function Signin(__account, __notify, $stateParams) {
                    this.__account = __account;
                    this.__notify = __notify;
                    this.$stateParams = $stateParams;
                }
                Signin.prototype.signin = function () {
                    var _this = this;
                    if (this.isSigningIn)
                        return;
                    //validate
                    if (!Object.isNullOrUndefined(this.password) &&
                        !Object.isNullOrUndefined(this.email)) {
                        this.isSigningIn = true;
                        this.__account.signin(this.email, this.password)
                            .then(function (tokenObj) {
                            var _rurl = _this.$stateParams['returnUrl'];
                            var _nurl = (Object.isNullOrUndefined(_rurl) || _rurl.trim() == "") ? window.location.protocol + '//' + window.location.host + '/profile/index' : _rurl;
                            window.location.href = _nurl;
                            _this.isSigningIn = false;
                        }, function (err) {
                            _this.__notify.error('Something went wrong...', 'Oops!');
                            _this.isSigningIn = false;
                        });
                    }
                };
                return Signin;
            }());
            Account.Signin = Signin;
        })(Account = Controllers.Account || (Controllers.Account = {}));
    })(Controllers = BitDiamond.Controllers || (BitDiamond.Controllers = {}));
})(BitDiamond || (BitDiamond = {}));
(function (BitDiamond) {
    var Controllers;
    (function (Controllers) {
        var Account;
        (function (Account) {
            var Signup = (function () {
                function Signup(__account, __notify, $stateParams, $state) {
                    this.__account = __account;
                    this.__notify = __notify;
                    this.$stateParams = $stateParams;
                    this.$state = $state;
                }
                Signup.prototype.passwordStrength = function () {
                    if (!Object.isNullOrUndefined(this.password))
                        return zxcvbn(this.password).score;
                    else
                        return -1;
                };
                Signup.prototype.strengthPercent = function () {
                    var score = this.passwordStrength() + 1;
                    return {
                        width: (score * 20) + '%'
                    };
                };
                Signup.prototype.strengthClass = function () {
                    switch (this.passwordStrength()) {
                        case 2:
                        case 3: return { 'progress-bar-warning': true };
                        case 4: return { 'progress-bar-success': true };
                        case 0:
                        case 1: return { 'progress-bar-danger': true };
                        case -1:
                        default: return {};
                    }
                };
                Signup.prototype.strengthMessage = function () {
                    switch (this.passwordStrength()) {
                        case 0: return 'easily guessable';
                        case 1: return 'very weak';
                        case 2: return 'weak';
                        case 3: return 'strong';
                        case 4: return 'very strong! Recommended.';
                        case -1:
                        default: return '';
                    }
                };
                Signup.prototype.signup = function () {
                    var _this = this;
                    if (this.isSigningUp)
                        return;
                    //validate
                    if (Object.isNullOrUndefined(this.email) || this.email.trim() == '') {
                        //alert the user that the emails must match
                        swal({
                            title: 'Oops!',
                            text: 'Your must enter an Email.',
                            type: 'warning'
                        });
                    }
                    else if (Object.isNullOrUndefined(this.password) || Object.isNullOrUndefined(this.password) || this.password.trim() == '') {
                        //alert the user that the emails must match
                        swal({
                            title: 'Oops!',
                            text: 'Your must enter a Password.',
                            type: 'warning'
                        });
                    }
                    else if (this.password != this.confirmPassword) {
                        swal({
                            title: 'Oops!',
                            text: 'Your passwords do not match.',
                            type: 'warning'
                        });
                    }
                    else if (!this.agreedToTerms) {
                        //alert to say you must agree to terms and conditions
                        swal({
                            title: 'Oops!',
                            text: 'Your must agree to the terms to proceed.',
                            type: 'warning'
                        });
                    }
                    else if (this.hasReferrerCode && Object.isNullOrUndefined(this.referrerCode)) {
                        //alert the user that s/he must supply a referrer code, or check the "do not have referrer code" link
                        swal({
                            title: 'Oops!',
                            text: 'Your must supply a VALID referrer code.',
                            type: 'warning'
                        });
                    }
                    else {
                        this.isSigningUp = true;
                        this.__account.registerUser(this.email, this.referrerCode, {
                            Value: BitDiamond.Utils.ToBase64String(BitDiamond.Utils.ToUTF8EncodedArray(this.password)),
                            Metadata: {
                                Name: 'Password',
                                Access: 1
                            }
                        }).then(function (opr) {
                            _this.isSigningUp = false;
                            _this.$state.go('message', { actionState: 'signin', actionTitle: 'Sign in', title: 'Congratulations!', message: 'An email has been sent to you with further instructions.' });
                        }, function (err) {
                            _this.isSigningUp = false;
                            _this.__notify.error('Something went wrong...', 'Oops!');
                        });
                    }
                };
                return Signup;
            }());
            Account.Signup = Signup;
            var VerifyRegistration = (function () {
                function VerifyRegistration(__account, __notify, $state, $stateParams) {
                    var _this = this;
                    this.__account = __account;
                    this.__notify = __notify;
                    this.$state = $state;
                    this.$stateParams = $stateParams;
                    //get the email and tokens
                    try {
                        var data = JSON.parse(BitDiamond.Utils.FromUTF8EncodedArray(BitDiamond.Utils.FromBase64String($stateParams['data'])));
                        this.email = data.Email;
                        this.token = data.Token;
                    }
                    catch (_e) {
                        //couldnt parse the data
                        swal({
                            title: 'Oops!',
                            text: 'An error occured while processing your request. Please contact the system administrator.',
                            type: 'error'
                        });
                    }
                    if (!Object.isNullOrUndefined(data)) {
                        this.isVerifying = true;
                        this.isSuccessfull = this.isError = false;
                        this.__account
                            .verifyUserActivation(this.email, this.token)
                            .then(function (opr) {
                            _this.isVerifying = false;
                            _this.isSuccessfull = true;
                        }, function (err) {
                            _this.isVerifying = false;
                            _this.isError = true;
                        });
                    }
                }
                return VerifyRegistration;
            }());
            Account.VerifyRegistration = VerifyRegistration;
        })(Account = Controllers.Account || (Controllers.Account = {}));
    })(Controllers = BitDiamond.Controllers || (BitDiamond.Controllers = {}));
})(BitDiamond || (BitDiamond = {}));
(function (BitDiamond) {
    var Controllers;
    (function (Controllers) {
        var Account;
        (function (Account) {
            var RecoveryRequest = (function () {
                function RecoveryRequest(__account, __notify, $state) {
                    this.__account = __account;
                    this.__notify = __notify;
                    this.$state = $state;
                }
                RecoveryRequest.prototype.requestRecovery = function () {
                    var _this = this;
                    if (this.isRecovering)
                        return;
                    //validate
                    if (Object.isNullOrUndefined(this.email)) {
                        //alert the user that s/he must supply a referrer code, or check the "do not have referrer code" link
                        swal({
                            title: 'Oops!',
                            text: 'You must enter a VALID email address to proceed.',
                            type: 'warning'
                        });
                    }
                    else {
                        this.isRecovering = true;
                        this.__account
                            .requestPasswordReset(this.email)
                            .then(function (opr) {
                            _this.isRecovering = false;
                            _this.$state.go('message', { actionState: 'signin', actionTitle: 'Sign in', title: 'Done!', message: 'An email has been sent to you with further instructions.' });
                        }, function (err) {
                            _this.isRecovering = false;
                            _this.__notify.error('Something went wrong...', 'Oops!');
                        });
                    }
                };
                return RecoveryRequest;
            }());
            Account.RecoveryRequest = RecoveryRequest;
            var RecoverPassword = (function () {
                function RecoverPassword(__account, __notify, $stateParams, $state) {
                    this.__account = __account;
                    this.__notify = __notify;
                    this.$stateParams = $stateParams;
                    this.$state = $state;
                    try {
                        var data = JSON.parse(BitDiamond.Utils.FromUTF8EncodedArray(BitDiamond.Utils.FromBase64String($stateParams['data'])));
                        this.email = data.Email;
                        this.token = data.Token;
                    }
                    catch (_e) {
                        //couldnt parse the data
                        this.hasInvalidData = true;
                    }
                }
                RecoverPassword.prototype.passwordStrength = function () {
                    if (!Object.isNullOrUndefined(this.password))
                        return zxcvbn(this.password).score;
                    else
                        return -1;
                };
                RecoverPassword.prototype.strengthPercent = function () {
                    var score = this.passwordStrength() + 1;
                    return {
                        width: (score * 20) + '%'
                    };
                };
                RecoverPassword.prototype.strengthClass = function () {
                    switch (this.passwordStrength()) {
                        case 2:
                        case 3: return { 'progress-bar-warning': true };
                        case 4: return { 'progress-bar-success': true };
                        case 0:
                        case 1: return { 'progress-bar-danger': true };
                        case -1:
                        default: return {};
                    }
                };
                RecoverPassword.prototype.strengthMessage = function () {
                    switch (this.passwordStrength()) {
                        case 0: return 'easily guessable';
                        case 1: return 'very weak';
                        case 2: return 'weak';
                        case 3: return 'strong';
                        case 4: return 'very strong (recommended)!';
                        case -1:
                        default: return '';
                    }
                };
                RecoverPassword.prototype.recover = function () {
                    var _this = this;
                    if (this.isRecovering)
                        return;
                    //validate
                    if (Object.isNullOrUndefined(this.password) || Object.isNullOrUndefined(this.password) || this.password.trim() == '') {
                        //alert the user that the emails must match
                        swal({
                            title: 'Oops!',
                            text: 'Your must enter a Password.',
                            type: 'warning'
                        });
                    }
                    else if (this.password != this.confirmPassword) {
                        swal({
                            title: 'Oops!',
                            text: 'Your passwords do not match.',
                            type: 'warning'
                        });
                    }
                    else {
                        this.isRecovering = true;
                        this.__account.verifyPasswordReset(this.email, this.token, this.password).then(function (opr) {
                            _this.isRecovering = false;
                            _this.$state.go('message', { actionState: 'signin', actionTitle: 'Sign in', title: 'Congratulations!', message: 'Your password has been reset.' });
                        }, function (err) {
                            _this.isRecovering = false;
                            _this.__notify.error('Something went wrong...', 'Oops!');
                        });
                    }
                };
                return RecoverPassword;
            }());
            Account.RecoverPassword = RecoverPassword;
        })(Account = Controllers.Account || (Controllers.Account = {}));
    })(Controllers = BitDiamond.Controllers || (BitDiamond.Controllers = {}));
})(BitDiamond || (BitDiamond = {}));
(function (BitDiamond) {
    var Controllers;
    (function (Controllers) {
        var Account;
        (function (Account) {
            var Terms = (function () {
                function Terms($state) {
                    this.$state = $state;
                }
                Terms.prototype.ok = function () {
                    this.$state.go('signup');
                };
                return Terms;
            }());
            Account.Terms = Terms;
        })(Account = Controllers.Account || (Controllers.Account = {}));
    })(Controllers = BitDiamond.Controllers || (BitDiamond.Controllers = {}));
})(BitDiamond || (BitDiamond = {}));
