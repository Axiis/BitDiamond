using BitDiamond.Core.Services.Query;
using System.Collections.Generic;
using System.Linq;
using Axis.Pollux.Identity.Principal;
using BitDiamond.Core.Models;
using Axis.Jupiter;

using static Axis.Luna.Extensions.ExceptionExtensions;
using Axis.Pollux.Authentication;
using System;
using Axis.Pollux.RBAC.Auth;
using System.Diagnostics;

namespace BitDiamond.Data.EF.Query
{
    public class AccountQuery : IAccountQuery
    {
        private IDataContext _europa = null;

        public AccountQuery(IDataContext context)
        {
            ThrowNullArguments(() => context);

            _europa = context;
        }

        public BioData GetBioData(User user)
        {
            var start = DateTime.Now;
            var r = _europa.Store<BioData>().Query
                  .Where(_bd => _bd.OwnerId == user.UserId)
                  .FirstOrDefault();
            Debug.WriteLine($"[AccountQuery.GetBioData] query executed in: {DateTime.Now - start}");
            return r;
        }

        public ContactData GetContactData(User user)
        => _europa.Store<ContactData>().Query
                  .Where(_cd => _cd.OwnerId == user.UserId)
                  .FirstOrDefault();

        public ContextVerification GetContextVerification(User user, string context, string token)
        => _europa.Store<ContextVerification>()
                  .QueryWith(_cv => _cv.Target)
                  .Where(_cv => _cv.Target.EntityId == user.UserId)
                  .Where(_cv => _cv.VerificationToken == token)
                  .Where(_cv => _cv.Context == context)
                  .FirstOrDefault();

        public IEnumerable<ContextVerification> GetContextVerifications(User user)
        => _europa.Store<ContextVerification>()
                  .QueryWith(_cv => _cv.Target)
                  .Where(_cv => _cv.Target.EntityId == user.EntityId)
                  .ToArray();

        public Credential GetCredential(User user, string name, Access credentialVisibility)
        => _europa.Store<Credential>()
                  .QueryWith(_cr => _cr.Owner)
                  .Where(_cr => _cr.OwnerId == user.EntityId)
                  .Where(_cr => _cr.Metadata.Name == name)
                  .Where(_cr => _cr.Metadata.Access == credentialVisibility)
                  .FirstOrDefault();

        public ContextVerification GetLatestContextVerification(User user, string context)
        => _europa.Store<ContextVerification>()
                  .QueryWith(_cv => _cv.Target)
                  .Where(_cv => _cv.Target.EntityId == user.UserId)
                  .Where(_cv => _cv.Context == context)
                  .OrderByDescending(_cv => _cv.CreatedOn)
                  .FirstOrDefault();

        public ReferralNode GetRefNode(string code)
        => _europa.Store<ReferralNode>()
                  .QueryWith()
                  .Where(_rn => _rn.ReferenceCode == code)
                  .FirstOrDefault();

        public User GetUserById(string userId)
        => _europa.Store<User>().Query
                  .Where(_u => _u.EntityId == userId)
                  .FirstOrDefault();

        public long GetUserCount()
        => _europa.Store<User>().Query.Count();

        public IEnumerable<UserData> GetUserData(User user)
        => _europa.Store<UserData>()
                  .QueryWith(_ud => _ud.Owner)
                  .Where(_ud => _ud.OwnerId == user.EntityId)
                  .ToArray();

        public UserData GetUserData(User user, string name)
        => _europa.Store<UserData>()
                  .QueryWith(_ud => _ud.Owner)
                  .Where(_ud => _ud.OwnerId == user.EntityId)
                  .Where(_ud => _ud.Name == name)
                  .FirstOrDefault();

        public UserLogon GetUserLogin(string token)
        => _europa.Store<UserLogon>()
                  .QueryWith(_ul => _ul.User)
                  .Where(_ul => _ul.OwinToken == token)
                  .FirstOrDefault();

        public IEnumerable<UserLogon> GetUserLogins(string userId)
        => _europa.Store<UserLogon>()
                  .QueryWith(_ul => _ul.User)
                  .Where(_ul => _ul.User.EntityId == userId)
                  .ToArray();

        public IEnumerable<string> GetUserRoles(User user)
        => _europa.Store<UserRole>().Query
                  .Where(_ur => _ur.UserId == user.UserId)
                  .Select(_ur => _ur.RoleName)
                  .ToArray();
    }
}
