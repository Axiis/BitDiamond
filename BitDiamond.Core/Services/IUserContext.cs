using Axis.Pollux.Identity.Principal;
using BitDiamond.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitDiamond.Core.Services
{
    public interface IUserContext
    {
        User CurrentUser();
        IEnumerable<string> CurrentUserRoles();
        UserLogon CurrentUserLogon();

        IUserContext Impersonate(string userId);
    }
}
