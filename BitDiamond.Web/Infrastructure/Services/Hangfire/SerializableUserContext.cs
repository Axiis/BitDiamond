using Axis.Pollux.Identity.Principal;
using BitDiamond.Core.Models;
using BitDiamond.Core.Services;
using System;
using System.Collections.Generic;

namespace BitDiamond.Web.Infrastructure.Services.Hangfire
{

    [Serializable]
    public class SerializableUserContext : IUserContext
    {
        public User _currentUser { get; set; }
        public UserLogon _currentUserLogon { get; set; }
        public List<string> _currentUserRoles { get; set; }

        public User CurrentUser() => _currentUser;
        public UserLogon CurrentUserLogon() => _currentUserLogon;
        public IEnumerable<string> CurrentUserRoles() => _currentUserRoles;
    }
}