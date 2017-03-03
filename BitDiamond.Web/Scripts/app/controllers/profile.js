var BitDiamond;
(function (BitDiamond) {
    var Controllers;
    (function (Controllers) {
        var Profile;
        (function (Profile) {
            var Dashboard = (function () {
                function Dashboard(__notify, __account, __userContext, $q) {
                    this.__notify = __notify;
                    this.__account = __account;
                    this.__userContext = __userContext;
                    //after loading timeline data...
                    Libs.HorizontalTimeline.initTimeline($('.cd-horizontal-timeline'));
                }
                return Dashboard;
            }());
            Profile.Dashboard = Dashboard;
            var Home = (function () {
                function Home(__notify, __account, __userContext) {
                    var _this = this;
                    this.tempBio = {};
                    this.tempContact = {};
                    this.__account = __account;
                    this.__notify = __notify;
                    this.__userContext = __userContext;
                    this.__userContext.profileImageRef.then(function (pimg) {
                        if (!Object.isNullOrUndefined(pimg)) {
                            _this.profileImage = pimg;
                        }
                    }).finally(function () {
                        if (Object.isNullOrUndefined(_this.profileImage))
                            _this.profileImage = {};
                    });
                    this.__userContext.userBio.then(function (bio) {
                        if (!Object.isNullOrUndefined(bio)) {
                            _this.userBio = bio;
                            _this.userBio.Dob = new Apollo.Models.JsonDateTime(_this.userBio.Dob);
                            _this.userBio.copyTo(_this.tempBio);
                            _this._dobField = _this.userBio.Dob.toMoment().toDate();
                        }
                    }).finally(function () {
                        if (Object.isNullOrUndefined(_this.userBio)) {
                            _this.userBio = {};
                        }
                    });
                    this.__userContext.userContact.then(function (contact) {
                        if (!Object.isNullOrUndefined(contact)) {
                            _this.userContact = contact;
                            _this.userContact.copyTo(_this.tempContact);
                        }
                    }).finally(function () {
                        if (Object.isNullOrUndefined(_this.userContact)) {
                            _this.userContact = {};
                        }
                    });
                    this.__userContext.user.then(function (u) {
                        _this.user = u;
                        _this.tempBio.OwnerId = _this.user.UserId;
                        _this.tempContact.OwnerId = _this.user.UserId;
                    });
                    this.currentTab = 'profile';
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
                        return '-';
                    else
                        return (this.userBio.FirstName || '') + ' ' +
                            (this.userBio.MiddleName || '') + ' ' +
                            (this.userBio.LastName || '');
                };
                Home.prototype.getGender = function () {
                    if (Object.isNullOrUndefined(this.userBio))
                        return '-';
                    return Pollux.Models.Gender[this.userBio.Gender];
                };
                Home.prototype.getDateOfBirth = function () {
                    if (Object.isNullOrUndefined(this.userBio))
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
                Object.defineProperty(Home.prototype, "dobBinding", {
                    get: function () {
                        return this._dobField;
                    },
                    set: function (value) {
                        if (this.tempBio) {
                            this.tempBio.Dob = new Apollo.Models.JsonDateTime().fromMoment(moment.utc(value));
                            this._dobField = value;
                        }
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(Home.prototype, "previewImage", {
                    set: function (value) {
                        var _this = this;
                        this.isUpdatingProfileImage = true;
                        var oldUrl = Object.isNullOrUndefined(this.profileImage) ? '' : (this.profileImage.Data || '');
                        this.__account.updateProfileImage(value, oldUrl).then(function (opr) {
                            _this.profileImage.Data = opr.Result;
                        }, function (err) {
                            _this.__notify.error('Couldn\'t update your profile image');
                        }).finally(function () {
                            _this.isUpdatingProfileImage = false;
                            $('#profileImageSelector')[0].reset();
                        });
                    },
                    enumerable: true,
                    configurable: true
                });
                Home.prototype.updateProfile = function () {
                    var _this = this;
                    //validate
                    this.isUpdatingProfile = true;
                    var count = 2;
                    //save biodata
                    this.__account.modifyBiodata(this.tempBio).then(function (opr) {
                        _this.__notify.success('Saved successfully');
                        _this.tempBio.copyTo(_this.userBio);
                        count--;
                        if (count == 0)
                            _this.isUpdatingProfile = false;
                    }, function (err) {
                        _this.__notify.error('An error occured while saving your changes. Try again later...', 'Oops!');
                        count--;
                        if (count == 0)
                            _this.isUpdatingProfile = false;
                    });
                    //save contact data
                    this.__account.modifyContactdata(this.tempContact).then(function (opr) {
                        _this.__notify.success('Saved successfully');
                        _this.tempContact.copyTo(_this.userContact);
                        count--;
                        if (count == 0)
                            _this.isUpdatingProfile = false;
                    }, function (err) {
                        _this.__notify.error('An error occured while saving your changes. Try again later...', 'Oops!');
                        count--;
                        if (count == 0)
                            _this.isUpdatingProfile = false;
                    });
                };
                Home.prototype.requestPasswordUpdate = function () {
                    var _this = this;
                    this.__account.requestPasswordReset(this.user.UserId).then(function (opr) {
                        swal({
                            text: 'An email has been sent to you via ' + _this.user.UserId + '. Follow the instructions therein to proceed.',
                            title: 'Success',
                            type: 'success'
                        });
                    }, function (err) {
                        swal({
                            text: 'An error occured, so we are sorry but you cannot change your password at the moment.',
                            title: 'Error',
                            type: 'error'
                        });
                    });
                };
                return Home;
            }());
            Profile.Home = Home;
        })(Profile = Controllers.Profile || (Controllers.Profile = {}));
    })(Controllers = BitDiamond.Controllers || (BitDiamond.Controllers = {}));
})(BitDiamond || (BitDiamond = {}));
//# sourceMappingURL=profile.js.map