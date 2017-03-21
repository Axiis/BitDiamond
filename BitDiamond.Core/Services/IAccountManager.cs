using Axis.Pollux.Authentication;
using Axis.Pollux.Identity.Principal;
using System.Collections.Generic;
using Axis.Pollux.RBAC.Auth;
using Axis.Luna;

namespace BitDiamond.Core.Services
{
    public interface IAccountManager
    {
        #region Account
        [Resource(":system/accounts/users/@register")]
        Operation<User> RegisterUser(string targetUser, string referrer, Credential secretCredential);

        [Resource(":system/accounts/admins/@register")]
        Operation<User> RegisterAdminUser(string targetUser, Credential secretCredential);

        [Resource(":system/accounts/users/@deactivate")]
        Operation<User> DeactivateUser(string targetUser);

        [Resource(":system/accounts/users/@block")]
        Operation<User> BlockUser(string targetUser);

        [Resource(":system/accounts/users/activations/@request")]
        Operation RequestUserActivation(string targetUser);

        Operation ValidateUserLogon();

        [Resource(":system/accounts/users/activations/@verify")]
        Operation<User> VerifyUserActivation(string targetUser, string contextToken);

        [Resource(":system/accounts/users/@count")]
        Operation<long> UserCount();

        [Resource(":system/accounts/users/credentials/reset-tokens/@verify")]
        Operation ResetCredential(Credential newCredential, string verificationToken, string targetUser);

        [Resource(":system/accounts/users/credentials/reset-tokens/@request")]
        Operation RequestCredentialReset(CredentialMetadata credentialMetadata, string targetUser);

        #endregion

        #region Biodata
        [Resource(":system/accounts/biodata/@update")]
        Operation<BioData> UpdateBioData(BioData data);
        
        [Resource(":system/accounts/biodata/@get")]
        Operation<BioData> GetBioData();
        #endregion

        #region Contact data
        [Resource(":system/accounts/contacts/@update")]
        Operation<ContactData> UpdateContactData(ContactData data);

        [Resource(":system/accounts/contacts/@get")]
        Operation<ContactData> GetContactData();
        #endregion

        #region User data
        [Resource(":system/accounts/userdata/@add")]
        Operation<IEnumerable<UserData>> AddData(UserData[] data);

        [Resource(":system/accounts/userdata/@update")]
        Operation<IEnumerable<UserData>> UpdateData(UserData[] data);        

        [Resource(":system/accounts/userdata/@delete")]
        Operation<IEnumerable<UserData>> RemoveData(string[] names);
        
        [Resource(":system/accounts/userdata/@get-all")]
        Operation<IEnumerable<UserData>> GetUserData();

        [Resource(":system/accounts/userdata/@get-named")]
        Operation<UserData> GetUserData(string name);

        [Resource(":system/accounts/userdata/profile-images/@update")]
        Operation<string> UpdateProfileImage(EncodedBinaryData image, string oldImageUrl);

        [Resource(":system/accounts/logons/@invalidate")]
        Operation InvalidateLogon(string token);

        [Resource(":system/accounts/users/roles/@get")]
        Operation<IEnumerable<string>> GetRoles();
        
        //Leaving out the Resource-Perission is on purpose: One does not need permission to get access to his/her 
        //User object
        Operation<User> CurrentUser();
        #endregion
    }
}