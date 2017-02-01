using Axis.Luna;
using Axis.Pollux.Authentication;
using Axis.Pollux.Identity.Principal;
using BitDiamond.Core.Models;
using System.Collections.Generic;

namespace BitDiamond.Core.Services
{
    public interface IAccountManager
    {
        #region Account
        Operation RegisterUser(string targetUser, string referee, Credential[] secretCredentials);
        
        Operation RegisterAdminUser(string targetUser, Credential[] secretCredentials);


        Operation<User> DeactivateUser(string targetUser);

        Operation<User> BlockUser(string targetUser);

        Operation<ContextVerification> RequestUserActivation(string targetUser);

        Operation<User> VerifyUserActivation(string targetUser, string contextToken);
        #endregion
        
        #region Biodata
        Operation<BioData> ModifyBioData(BioData data);
        
        Operation<BioData> GetBioData();
        #endregion

        #region Contact data
        
        Operation<ContactData> ModifyContactData(ContactData data);
        
        Operation<IEnumerable<ContactData>> RemoveContactData(long[] ids);
        
        Operation<IEnumerable<ContactData>> GetContactData();
        #endregion

        #region User data
        Operation<IEnumerable<UserData>> AddData(UserData[] data);
        
        Operation<IEnumerable<UserData>> RemoveData(string[] names);
        
        Operation<IEnumerable<UserData>> GetUserData();
        
        Operation<UserData> GetUserData(string name);
        
        Operation<string> UpdateProfileImage(EncodedBinaryData image, string oldImageUrl);
        #endregion
    }
}