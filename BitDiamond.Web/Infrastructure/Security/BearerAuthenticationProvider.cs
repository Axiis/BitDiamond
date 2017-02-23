using Axis.Jupiter;
using Axis.Luna;
using Axis.Pollux.Identity.Principal;
using BitDiamond.Core.Models;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Linq;
using System.Threading.Tasks;
using UAParser;
using static Axis.Luna.Extensions.ExceptionExtensions;
using static Axis.Luna.Extensions.ObjectExtensions;

namespace BitDiamond.Web.Infrastructure.Security
{
    public class BearerAuthenticationProvider: OAuthBearerAuthenticationProvider
    {
        private WeakCache _cache = null;

        public BearerAuthenticationProvider(WeakCache cache)
        {
            ThrowNullArguments(() => cache);
            
            _cache = cache;

            //make sure a logon hasnt been invalidated!
            OnValidateIdentity = context => Task.Run(() =>
            {
                var token = context.Request.Headers.Get("Authorization").TrimStart("Bearer").Trim();

                //invalidate old logons if the request payload has any

                //in future, a realtime event will notify the bearer-provider of changes to a logon, so we dont need to keep quering the database
                var logon = _cache.GetOrRefresh<UserLogon>(token);
                
                if (logon.Invalidated) context.Rejected();
                else context.Validated();
            });
        }
    }
}