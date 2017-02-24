
module BitDiamond.Controllers.Shared {


    export class Message {
        message: string;
        title: string;
        actionState: string;
        actionTitle: string;
        hasNoActionTitle: boolean;

        $state: ng.ui.IStateService;

        action() {
            if (Object.isNullOrUndefined(this.actionState)) {
                window.location.href = "/index.html";
            }
            else {
                this.$state.go(this.actionState);
            }
        }

        constructor($state, $stateParams) {
            this.$state = $state;

            this.message = $stateParams['message'];
            this.title = $stateParams['title'];
            this.actionState = $stateParams['actionState'];
            this.actionTitle = $stateParams['actionTitle'];
            this.hasNoActionTitle = Object.isNullOrUndefined(this.actionTitle);
        }
    }



    export class NavBar {

    }



    export class SideBar {

        user: Pollux.Models.IUser;
        userRoles: string[];
        userBio: Pollux.Models.IBioData;
        profileImageRef: Pollux.Models.IUserData;

        __account: Services.Account;
        __notify: Utils.Services.NotifyService;
        __userContext: Utils.Services.UserContext;

        $location: ng.ILocationService;

        profileImage() {
            //for now
            if (Object.isNullOrUndefined(this.profileImageRef)) return '/content/images/default-user.png';
            else return this.profileImageRef.Data;
        }
        userNames() {
            if (Object.isNullOrUndefined(this.userBio) && !Object.isNullOrUndefined(this.user)) return this.user.UserId;
            else if (!Object.isNullOrUndefined(this.userBio)) {
                return (this.userBio.FirstName || '') + ' ' + (this.userBio.LastName || '');
            }
            else return "Member";
        }

        getActiveClass(moduleName: string): any {
            return { active: this.isCurrentModule(moduleName) };
        }
        getModuleUrl(moduleName: string): string {
            if (moduleName == 'dashboard') return '/profiles/index';
            else return '/' + moduleName + '/index';
        }

        private isCurrentModule(moduleName: string): boolean {
            var m = window.location.pathname.split('/')[1];
            switch (m) {
                case 'profile': return 'dashboard' == moduleName;
                default: return m == moduleName;
            }
        }



        logout() {
            this.__account.signout();
        }


        constructor(__account, __notify, __userContext, $location) {

            this.__account = __account;
            this.__notify = __notify;
            this.__userContext = __userContext;
            this.$location = $location;

            //load user object
            this.__userContext.user.then(opr => {
                this.user = opr;
            }, err => {
                swal(<ISweetAlertConfig>{
                    text: 'Your information could not be retrieved from the server. You will be logged out so you can try loggin in again',
                    title: 'Error',
                    type: 'error'
                });
            });

            //load profile image
            this.__userContext.profileImageRef.then(opr => {
                this.profileImageRef = opr;
            }, err => {
                this.__notify.warning('we couldn\'t load your profile image...', 'Hey');
            });

            //load user roles
            this.__userContext.userRoles.then(opr => {
                this.userRoles = opr;
            }, err => {
                swal(<ISweetAlertConfig>{
                    text: 'Your information could not be retrieved from the server. You will be logged out so you can try loggin in again',
                    title: 'Error',
                    type: 'error'
                });
                this.logout();
            });

            //load user biodata
            this.__userContext.userBio.then(opr => {
                this.userBio = opr;
            }, err => {
                this.__notify.warning('we couldn\'t load your bio...', 'Hey');
            });
            
        }
    }
}