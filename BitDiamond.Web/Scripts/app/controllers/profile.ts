
module BitDiamond.Controllers.Profile {

    export class Dashboard {


        constructor() {

        }
    }


    export class Home {

        profileImage: Pollux.Models.IUserData;
        userBio: Pollux.Models.IBioData;
        userContact: Pollux.Models.IContactData;
        user: Pollux.Models.IUser;

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
            if (Object.isNullOrUndefined(this.userBio)) return '-N/A-';
            else return (this.userBio.FirstName || '') + ' ' +
                (this.userBio.MiddleName || '') + ' ' +
                (this.userBio.LastName || '');
        }
        getGender() {
            if (Object.isNullOrUndefined(this.userBio.Gender)) return '-';
            return Pollux.Models.Gender[this.userBio.Gender];
        }
        getDateOfBirth() {
            if (Object.isNullOrUndefined(this.userBio.Gender)) return '-';
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


        constructor(__notify, __account, __userContext) {
            this.__account = __account;
            this.__notify = __notify;
            this.__userContext = __userContext;

            this.__userContext.profileImageRef.then(pimg => this.profileImage = pimg);

            this.__userContext.userBio.then(bio => this.userBio = bio || <Pollux.Models.IBioData>{
                Dob: new Apollo.Models.JsonDateTime(new Date()),
                FirstName: 'Jon',
                Gender: Pollux.Models.Gender.Male,
                LastName: 'Doe',
                Nationality: 'Nigerian',
                StateOfOrigin: 'Poly'
            });

            this.__userContext.userContact.then(contact => this.userContact = contact);

            this.__userContext.user.then(u => this.user = u);
        }
    }

}