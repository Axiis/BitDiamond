
module BitDiamond.Controllers.Profile {

    export class Dashboard {

        __notify: Utils.Services.NotifyService;
        __account: Services.Account;
        __userContext: Utils.Services.UserContext;

        $q: ng.IQService;

        constructor(__notify, __account, __userContext, $q) {
            this.__notify = __notify;
            this.__account = __account;
            this.__userContext = __userContext;

            //after loading timeline data...
            Libs.HorizontalTimeline.initTimeline($('.cd-horizontal-timeline'));
        }
    }


    export class Home {

        profileImage: Pollux.Models.IUserData;
        userBio: Pollux.Models.IBioData;
        userContact: Pollux.Models.IContactData;
        user: Pollux.Models.IUser;

        tempBio: any = {};
        tempContact: any = {};
        isUpdatingProfile: boolean;
        isRequestingPasswordUpdate: boolean;
        isUpdatingProfileImage: boolean;

        currentTab: string;

        __notify: Utils.Services.NotifyService;
        __account: Services.Account;
        __userContext: Utils.Services.UserContext;

        getProfileImage() {
            var imgUrl = Object.isNullOrUndefined(this.profileImage) ?
                Utils.Constants.URL_DefaultProfileImage :
                this.profileImage.Data || Utils.Constants.URL_DefaultProfileImage;

            return {
                'background-image': 'url(' + imgUrl + ')',
                'background-size': 'contain',
                'background-repeat': 'no-repeat',
                'background-position': 'center center'
            };
        }

        getNames() {
            if (Object.isNullOrUndefined(this.userBio)) return '-N/A-';
            else return this.userBio.FirstName + ' ' + this.userBio.LastName;
        }
        getFullNames() {
            if (Object.isNullOrUndefined(this.userBio)) return '-';
            else return (this.userBio.FirstName || '') + ' ' +
                (this.userBio.MiddleName || '') + ' ' +
                (this.userBio.LastName || '');
        }
        getGender() {
            if (Object.isNullOrUndefined(this.userBio)) return '-';
            return Pollux.Models.Gender[this.userBio.Gender];
        }
        getDateOfBirth() {
            if (Object.isNullOrUndefined(this.userBio)) return '-';
            return moment(this.userBio.Dob).format('MMM Do YYYY');
        }


        activateTab(name: string) {
            this.currentTab = name;
        }
        getActiveClassFor(name: string): any {
            return {
                active: name == this.currentTab
            };
        }

        private _dobField: Date;
        set dobBinding(value: Date) {
            if (this.tempBio) {
                this.tempBio.Dob = new Apollo.Models.JsonDateTime().fromMoment(moment.utc(value));
                this._dobField = value;
            }
        }
        get dobBinding(): Date {
            return this._dobField;
        }
        
        set previewImage(value: Utils.EncodedBinaryData) {
            this.isUpdatingProfileImage = true;
            var oldUrl: string = Object.isNullOrUndefined(this.profileImage) ? '' : (this.profileImage.Data || '');

            this.__account.updateProfileImage(value, oldUrl).then(opr => {                
                this.profileImage.Data = opr.Result;
            }, err => {
                this.__notify.error('Couldn\'t update your profile image');
            }).finally(() => {
                this.isUpdatingProfileImage = false;
                ($('#profileImageSelector')[0] as any).reset();
            });
        }

        updateProfile() {
            //validate
            this.isUpdatingProfile = true;
            var count = 2;

            //save biodata
            this.__account.modifyBiodata(this.tempBio).then(opr => {
                this.__notify.success('Saved successfully');
                (<Object>this.tempBio).copyTo(this.userBio);

                count--;
                if (count == 0) this.isUpdatingProfile = false;
            }, err => {
                this.__notify.error('An error occured while saving your changes. Try again later...', 'Oops!');

                count--;
                if (count == 0) this.isUpdatingProfile = false;
            });

            //save contact data
            this.__account.modifyContactdata(this.tempContact).then(opr => {
                this.__notify.success('Saved successfully');
                (<Object>this.tempContact).copyTo(this.userContact);

                count--;
                if (count == 0) this.isUpdatingProfile = false;
            }, err => {
                this.__notify.error('An error occured while saving your changes. Try again later...', 'Oops!');

                count--;
                if (count == 0) this.isUpdatingProfile = false;
            });
        }
        requestPasswordUpdate() {

            this.__account.requestPasswordReset(this.user.UserId).then(opr => {
                swal({
                    text: 'An email has been sent to you via ' + this.user.UserId+'. Follow the instructions therein to proceed.',
                    title: 'Success',
                    type: 'success'
                });
            }, err => {
                swal({
                    text: 'An error occured, so we are sorry but you cannot change your password at the moment.',
                    title: 'Error',
                    type: 'error'
                });
            });
        }


        constructor(__notify, __account, __userContext) {
            this.__account = __account;
            this.__notify = __notify;
            this.__userContext = __userContext;

            this.__userContext.profileImageRef.then(pimg => {
                if (!Object.isNullOrUndefined(pimg)) {
                    this.profileImage = pimg;
                }
            }).finally(() => {
                if (Object.isNullOrUndefined(this.profileImage)) this.profileImage = {} as Pollux.Models.IUserData;
            });

            this.__userContext.userBio.then(bio => {
                if (!Object.isNullOrUndefined(bio)) {
                    this.userBio = bio;
                    this.userBio.Dob = new Apollo.Models.JsonDateTime(this.userBio.Dob);
                    (<Object>this.userBio).copyTo(this.tempBio);
                    this._dobField = this.userBio.Dob.toMoment().toDate();
                }
            }).finally(() => {
                if (Object.isNullOrUndefined(this.userBio)) {
                    this.userBio = {} as Pollux.Models.IBioData;
                }
            });

            this.__userContext.userContact.then(contact => {
                if (!Object.isNullOrUndefined(contact)) {
                    this.userContact = contact;
                    (<Object>this.userContact).copyTo(this.tempContact);
                }
            }).finally(() => {
                if (Object.isNullOrUndefined(this.userContact)) {
                    this.userContact = {} as Pollux.Models.IContactData;
                }
            });

            this.__userContext.user.then(u => {
                this.user = u;
                this.tempBio.OwnerId = this.user.UserId;
                this.tempContact.OwnerId = this.user.UserId;
            });

            this.currentTab = 'profile';
        }
    }

}