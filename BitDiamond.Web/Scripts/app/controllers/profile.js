var BitDiamond;
(function (BitDiamond) {
    var Controllers;
    (function (Controllers) {
        var Profile;
        (function (Profile) {
            var Dashboard = (function () {
                function Dashboard() {
                }
                return Dashboard;
            }());
            Profile.Dashboard = Dashboard;
            var Home = (function () {
                function Home(__notify, __account, __userContext) {
                    var _this = this;
                    this.__account = __account;
                    this.__notify = __notify;
                    this.__userContext = __userContext;
                    this.__userContext.profileImageRef.then(function (pimg) { return _this.profileImage = pimg; });
                    this.__userContext.userBio.then(function (bio) { return _this.userBio = bio || {
                        Dob: new Apollo.Models.JsonDateTime(new Date()),
                        FirstName: 'Jon',
                        Gender: Pollux.Models.Gender.Male,
                        LastName: 'Doe',
                        Nationality: 'Nigerian',
                        StateOfOrigin: 'Poly'
                    }; });
                    this.__userContext.userContact.then(function (contact) { return _this.userContact = contact; });
                    this.__userContext.user.then(function (u) { return _this.user = u; });
                }
                Home.prototype.getProfileImage = function () {
                    var imgUrl = Object.isNullOrUndefined(this.profileImage) ?
                        BitDiamond.Utils.Constants.URL_DefaultProfileImage :
                        this.profileImage.Data || BitDiamond.Utils.Constants.URL_DefaultProfileImage;
                    return {
                        'background-image': 'url(' + imgUrl + ')',
                        'background-size': 'contain',
                        'background-repeat': 'no-repeat',
                        'background-position': 'center center'
                    };
                };
                Home.prototype.getNames = function () {
                    if (Object.isNullOrUndefined(this.userBio))
                        return '-N/A-';
                    else
                        return this.userBio.FirstName + ' ' + this.userBio.LastName;
                };
                Home.prototype.getFullNames = function () {
                    if (Object.isNullOrUndefined(this.userBio))
                        return '-N/A-';
                    else
                        return (this.userBio.FirstName || '') + ' ' +
                            (this.userBio.MiddleName || '') + ' ' +
                            (this.userBio.LastName || '');
                };
                Home.prototype.getGender = function () {
                    if (Object.isNullOrUndefined(this.userBio.Gender))
                        return '-';
                    return Pollux.Models.Gender[this.userBio.Gender];
                };
                Home.prototype.getDateOfBirth = function () {
                    if (Object.isNullOrUndefined(this.userBio.Gender))
                        return '-';
                    return moment(this.userBio.Dob).format('MMM Do YYYY');
                };
                Home.prototype.activateTab = function (name) {
                    this.currentTab = name;
                };
                Home.prototype.getActiveClassFor = function (name) {
                    return {
                        active: name == this.currentTab
                    };
                };
                return Home;
            }());
            Profile.Home = Home;
        })(Profile = Controllers.Profile || (Controllers.Profile = {}));
    })(Controllers = BitDiamond.Controllers || (BitDiamond.Controllers = {}));
})(BitDiamond || (BitDiamond = {}));
