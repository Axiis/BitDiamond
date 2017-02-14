using Axis.Jupiter;
using BitDiamond.Core.Models;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

using static Axis.Luna.Extensions.ExceptionExtensions;

namespace BitDiamond.Web.Infrastructure.Security
{
    public class BearerAuthenticationProvider: OAuthBearerAuthenticationProvider
    {
        private IDataContext _dataContext = null;

        public BearerAuthenticationProvider(IDataContext dataContext)
        {
            ThrowNullArguments(() => dataContext);

            _dataContext = dataContext;

            //make sure a logon hasnt been invalidated!
            OnValidateIdentity = context => Task.Run(() =>
            {
                //in future, a realtime event will notify the bearer-provider of changes to a logon, so we dont need to keep quering the database
                var logon = _dataContext.Store<UserLogon>()
                                        .QueryWith(_ul => _ul.User)
                                        .Where(_ul => _ul.User.EntityId == context.Ticket.Identity.Name)
                                        .Where(_ul => _ul.OwinToken == context.Request.Headers.Get("")) //get the bearer token from the header
                                        .FirstOrDefault();

                if (logon == null)
                {
                    //create a new logon

                    context.Validated();
                }
                else if (logon.Invalidated) context.Rejected();
                else context.Validated();
            });
        }
    }
}