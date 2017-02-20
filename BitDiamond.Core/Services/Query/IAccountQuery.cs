using Axis.Pollux.Authentication;
using Axis.Pollux.Identity.Principal;
using BitDiamond.Core.Models;
using System.Collections.Generic;

namespace BitDiamond.Core.Services.Query
{
    public interface IAccountQuery
    {
        User GetUserById(string userId);
        IEnumerable<UserLogon> GetUserLogins(string userId);
        IEnumerable<ContextVerification> GetContextVerifications(User user);
        ContextVerification GetLatestContextVerification(User user, string context);
        ContextVerification GetContextVerification(User user, string context, string token);
        BioData GetBioData(User user);
        ContactData GetContactData(User user);
        IEnumerable<UserData> GetUserData(User user);
        UserData GetUserData(User user, string name);
        ReferralNode GetRefNode(string refereeCode);
        Credential GetCredential(User user, string name, Access credentialVisibility);
    }
}
