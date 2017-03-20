
module BitDiamond.Controllers.Profile {

    export class Dashboard {


        displayTime(time: Apollo.Models.JsonDateTime): string {
            if (Object.isNullOrUndefined(time)) return '';
            else return time.toMoment().format('YYYY/M/D  H:m');
        }
        isLastNotification(index: number): boolean {
            return index == this.notifications.length - 1;
        }
        notifications: Models.INotification[] = [];
        notificationCount: number;
        isLoadingNotifications: boolean;

        isLoadingIncomingTransactions: boolean;
        isLoadingOutgoingTransactions: boolean;
        totalIncomingTransactions: number;
        totalOutgoingTransactions: number;
        user: Pollux.Models.IUser;

        isLoadingBitLevel: boolean;
        bitLevel: Models.IBitLevel;

        __notify: Utils.Services.NotifyService;
        __account: Services.Account;
        __userContext: Utils.Services.UserContext;
        __systemNotification: Services.Notification;
        __blockChain: Services.BlockChain;
        __bitLevel: Services.BitLevel;

        $q: ng.IQService;
        $scope: ng.IScope;

        constructor(__notify, __account, __userContext, __systemNotification, __blockChain, __bitLevel, $q, $scope) {
            this.__notify = __notify;
            this.__account = __account;
            this.__userContext = __userContext;
            this.__systemNotification = __systemNotification;
            this.__blockChain = __blockChain;
            this.__bitLevel = __bitLevel;
            this.$scope = $scope;

            this.__userContext.user.then(u => this.user = u);

            //load incoming transactions
            this.isLoadingIncomingTransactions = true;
            this.__blockChain.getIncomingUserTransactionTotal().then(opr => {
                this.totalIncomingTransactions = opr.Result;
            }).finally(() => {
                this.isLoadingIncomingTransactions = false;
            });

            //load outgoing transactions
            this.isLoadingOutgoingTransactions = true;
            this.__blockChain.getOutgoingUserTransactionTotal().then(opr => {
                this.totalOutgoingTransactions = opr.Result;
            }).finally(() => {
                this.isLoadingOutgoingTransactions = false;
            });

            //load notifications
            this.isLoadingNotifications = true;
            this.__systemNotification.getUnseenNotificaftions().then(opr => {
                this.notificationCount = opr.Result.length;
                this.notifications = opr.Result.slice(0, 5).map(_v => {
                    _v.CreatedOn = new Apollo.Models.JsonDateTime(_v.CreatedOn);
                    return _v;
                });
            }).finally(() => {
                this.isLoadingNotifications = false;
            });

            //load bitlevel
            this.isLoadingBitLevel = true;
            this.__bitLevel.currentLevel().then(opr => {
                this.bitLevel = opr.Result;
            }).finally(() => {
                this.isLoadingBitLevel = false;
            });
        }
    }


    export class Home {

        profileImage: Pollux.Models.IUserData;
        userBio: Pollux.Models.IBioData;
        userContact: Pollux.Models.IContactData;
        user: Pollux.Models.IUser;
        userRef: string;

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
            if (Object.isNullOrUndefined(this.userBio) ||
                Object.isNullOrUndefined(this.userBio.FirstName) && Object.isNullOrUndefined(this.userBio.LastName)) return '-N/A-';
            else return (this.userBio.FirstName || '') + ' ' + (this.userBio.LastName || '');
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
            return this.userBio.Dob.toMoment().format('MMM Do YYYY');
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
                this.tempBio.Dob = Apollo.Models.JsonDateTime.fromMoment(moment(value));
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
        refCopied() {
            this.__notify.success('User Reference Code copied to clipboard');
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

            //user ref
            this.__userContext.userRef.then(ref => {
                this.userRef = ref.ReferenceCode;
            });

            this.currentTab = 'profile';
        }
    }

}