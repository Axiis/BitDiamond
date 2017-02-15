var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var Pollux;
(function (Pollux) {
    var Models;
    (function (Models) {
        var Access;
        (function (Access) {
            Access[Access["Public"] = 0] = "Public";
            Access[Access["Secret"] = 1] = "Secret";
        })(Access = Models.Access || (Models.Access = {}));
        var Gender;
        (function (Gender) {
            Gender[Gender["Female"] = 0] = "Female";
            Gender[Gender["Male"] = 1] = "Male";
            Gender[Gender["Other"] = 2] = "Other";
        })(Gender = Models.Gender || (Models.Gender = {}));
        var PolluxEntity = (function () {
            function PolluxEntity(data) {
                if (data) {
                    data.copyTo(this);
                    this.CreatedOn = data['CreatedOn'] ? new Axis.Apollo.Domain.JsonDateTime(data['CreatedOn']) : null;
                    this.ModifiedOn = data['ModifiedOn'] ? new Axis.Apollo.Domain.JsonDateTime(data['ModifiedOn']) : null;
                }
            }
            return PolluxEntity;
        }());
        Models.PolluxEntity = PolluxEntity;
        var AddressData = (function (_super) {
            __extends(AddressData, _super);
            function AddressData(data) {
                var _this = _super.call(this, data) || this;
                if (data) {
                    _this.Owner = data['Owner'] ? new User(data['Owner']) : null;
                }
                return _this;
            }
            return AddressData;
        }(PolluxEntity));
        Models.AddressData = AddressData;
        var BioData = (function (_super) {
            __extends(BioData, _super);
            function BioData(data) {
                var _this = _super.call(this, data) || this;
                if (data) {
                    _this.Dob = data['Dob'] ? new Axis.Apollo.Domain.JsonDateTime(data['Dob']) : null;
                    _this.Owner = data['Owner'] ? new User(data['Owner']) : null;
                }
                return _this;
            }
            return BioData;
        }(PolluxEntity));
        Models.BioData = BioData;
        var ContactData = (function (_super) {
            __extends(ContactData, _super);
            function ContactData(data) {
                var _this = _super.call(this, data) || this;
                if (data) {
                    _this.Owner = data['Owner'] ? new User(data['Owner']) : null;
                }
                return _this;
            }
            return ContactData;
        }(PolluxEntity));
        Models.ContactData = ContactData;
        var CorporateData = (function (_super) {
            __extends(CorporateData, _super);
            function CorporateData(data) {
                var _this = _super.call(this, data) || this;
                _this.Status = Gaia.Utils.BusinessStatus_Draft;
                if (data) {
                    _this.IncorporationDate = data['IncorporationDate'] ? new Axis.Apollo.Domain.JsonDateTime(data['IncorporationDate']) : null;
                    _this.Owner = data['Owner'] ? new User(data['Owner']) : null;
                }
                return _this;
            }
            return CorporateData;
        }(PolluxEntity));
        Models.CorporateData = CorporateData;
        var User = (function (_super) {
            __extends(User, _super);
            function User(data) {
                var _this = _super.call(this, data) || this;
                if (data) {
                }
                return _this;
            }
            return User;
        }(PolluxEntity));
        Models.User = User;
        var UserData = (function (_super) {
            __extends(UserData, _super);
            function UserData(data) {
                var _this = _super.call(this, data) || this;
                if (data) {
                    _this.Owner = data['Owner'] ? new User(data['Owner']) : null;
                }
                return _this;
            }
            return UserData;
        }(PolluxEntity));
        Models.UserData = UserData;
        var CredentialMetadata = (function () {
            function CredentialMetadata(Name, Access) {
                this.Name = Name;
                this.Access = Access;
            }
            return CredentialMetadata;
        }());
        Models.CredentialMetadata = CredentialMetadata;
        var Credential = (function (_super) {
            __extends(Credential, _super);
            function Credential(data) {
                var _this = _super.call(this, data) || this;
                if (data) {
                    _this.Owner = data['Owner'] ? new User(data['Owner']) : null;
                    _this.Expires = data['Expires'] ? new Axis.Apollo.Domain.JsonTimeSpan(data['Expires']) : null;
                }
                return _this;
            }
            return Credential;
        }(PolluxEntity));
        Models.Credential = Credential;
    })(Models = Pollux.Models || (Pollux.Models = {}));
})(Pollux || (Pollux = {}));
