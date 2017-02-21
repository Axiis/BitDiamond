
module BitDiamond.Controllers.Account {

    export class Signin {

        isSigningIn: boolean;

        password: string;
        email: string;

        __account: BitDiamond.Services.Account;
        __notify: BitDiamond.Utils.Services.NotifyService;
        $stateParams: ng.ui.IStateParamsService;


        signin() {
            if (this.isSigningIn) return;

            //validate
            if (!Object.isNullOrUndefined(this.password) &&
                !Object.isNullOrUndefined(this.email)) {
                this.isSigningIn = true;

                this.__account.signin(this.email, this.password)
                    .then(tokenObj => {
                        window.localStorage.setItem(BitDiamond.Utils.Constants.Misc_OAuthTokenKey, JSON.stringify(tokenObj));
                        window.location.href = Object.isNullOrUndefined(this.$stateParams['returnUrl']) ? '/profile/index' : this.$stateParams['returnUrl'];

                        this.isSigningIn = false;
                    }, err => {
                        this.__notify.error('Something went wrong...', 'Oops!');

                        this.isSigningIn = false;
                    });
            }
        }

        constructor(__account, __notify, $stateParams) {
            this.__account = __account;
            this.__notify = __notify;
            this.$stateParams = $stateParams;
        }
    }
}

module BitDiamond.Controllers.Account {

    export class Signup {

        isSigningUp: boolean;

        email: string;
        password: string;
        confirmPassword: string;
        referrerCode: string;
        agreedToTerms: boolean;
        hasReferrerCode: boolean;

        passwordStrength(): number {
            if (!Object.isNullOrUndefined(this.password))
                return zxcvbn(this.password).score;

            else return -1;
        }
        strengthPercent(): any {
            var score = this.passwordStrength() + 1;
            return {
                width: (score * 20) + '%'
            };
        }
        strengthClass(): any {
            switch (this.passwordStrength()) {
                case 2: case 3: return { 'progress-bar-warning': true };
                case 4: return { 'progress-bar-success': true };
                case 0: case 1: return { 'progress-bar-danger': true };
                case -1: default: return {};
            }
        }
        strengthMessage(): string {
            switch (this.passwordStrength()) {
                case 0: return 'easily guessable';
                case 1: return 'very weak';
                case 2: return 'weak';
                case 3: return 'strong';
                case 4: return 'very strong! Recommended.';
                case -1: default: return '';
            }
        }

        __account: BitDiamond.Services.Account;
        __notify: BitDiamond.Utils.Services.NotifyService;

        $stateParams: ng.ui.IStateParamsService;
        $state: ng.ui.IStateService;

        signup() {

            if (this.isSigningUp) return;

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
                this.__account.registerUser(this.email, this.referrerCode, <any>{
                    Value: Utils.ToBase64String(Utils.ToUTF8EncodedArray(this.password)),
                    Metadata: {
                        Name: 'Password',
                        Access: 1
                    }
                }).then(opr => {
                    this.isSigningUp = false;
                    this.$state.go('message', { back: 'signin', title: 'Congratulations!', message: 'An email has been sent to you with further instructions.' });
                }, err => {
                    this.isSigningUp = false;
                    this.__notify.error('Something went wrong...', 'Oops!');
                });
            }
        }


        constructor(__account, __notify, $stateParams, $state) {
            this.__account = __account;
            this.__notify = __notify;
            this.$stateParams = $stateParams;
            this.$state = $state;
        }

    }

    export class VerifyRegistration {

        isVerifying: boolean;
        isSuccessfull: boolean;
        isError: boolean;
        email: string;
        token: string;

        __account: BitDiamond.Services.Account;
        __notify: BitDiamond.Utils.Services.NotifyService;
        $state: ng.ui.IStateService;
        $stateParams: ng.ui.IStateParamsService;

        constructor(__account, __notify, $state, $stateParams) {
            this.__account = __account;
            this.__notify = __notify;
            this.$state = $state;
            this.$stateParams = $stateParams;

            //get the email and tokens
            var data = JSON.parse(Utils.FromUTF8EncodedArray(Utils.FromBase64String($stateParams['data'])));
            this.email = data.Email;
            this.token = data.Token;

            this.isVerifying = true;
            this.isSuccessfull = this.isError = false;
            this.__account
                .verifyUserActivation(this.email, this.token)
                .then(opr => {
                    this.isVerifying = false;
                    this.isSuccessfull = true;
                }, err => {
                    this.isVerifying = false;
                    this.isError = true;
                });
        }
    }
}


module BitDiamond.Controllers.Account {

    export class RecoveryRequest {

        isRecovering: boolean;

        email: string;

        __account: BitDiamond.Services.Account;
        __notify: BitDiamond.Utils.Services.NotifyService;
        $state: ng.ui.IStateService;

        recoverPassword() {
            if (this.isRecovering) return;

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
                    .then(opr => {
                        this.isRecovering = false;
                        this.$state.go('message', { action: 'signin', actionTitle: 'Signin', title: 'Done!', message: 'An email has been sent to you with further instructions.' })
                    }, err => {
                        this.isRecovering = false;
                        this.__notify.error('Something went wrong...', 'Oops!');
                    });
            }
        }


        constructor(__account, __notify, $state) {
            this.__account = __account;
            this.__notify = __notify;
            this.$state = $state;
        }
    }

    export class RecoverPassword {

        isVerifying: boolean;
        password: string;
        email: string;
        token: string;

        __account: BitDiamond.Services.Account;
        __notify: BitDiamond.Utils.Services.NotifyService;
        $state: ng.ui.IStateService;
        $stateParams: ng.ui.IStateParamsService;

        passwordStrength(): number {
            return zxcvbn(this.password).score;
        }

        recover() {

            //validate
            if (Object.isNullOrUndefined(this.password)) {
                //alert the user that s/he must supply a referrer code, or check the "do not have referrer code" link
                swal({
                    title: 'Oops!',
                    text: 'You must enter a VALID password to proceed.',
                    type: 'Warning'
                });
            }
            else {
                this.isVerifying = true;
                this.__account
                    .verifyPasswordReset(this.email, this.token, this.password)
                    .then(opr => {
                        this.isVerifying = false;
                        this.$state.go('message', { action: 'signin', actionTitle: 'Signin', title: 'Done!', message: 'Your password has been reset.' })
                    }, err => {
                        this.isVerifying = false;
                        this.__notify.error('Something went wrong...', 'Oops!');
                    });
            }
        }

        constructor(__account, __notify, $state, $stateParams) {
            this.__account = __account;
            this.__notify = __notify;
            this.$state = $state;
            this.$stateParams = $stateParams;

            //get the email and tokens
            var data = JSON.parse(Utils.FromUTF8EncodedArray(Utils.FromBase64String($stateParams['data'])));
            this.email = data.Email;
            this.token = data.Token;
        }
    }
}


module BitDiamond.Controllers.Account {

    export class Terms {
        message: "[Terms and conditions here...]";

        $state: ng.ui.IStateService;

        ok() {
            this.$state.go('signup');
        }

        constructor($state) {
            this.$state = $state;
        }
    }

}