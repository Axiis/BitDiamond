using Axis.Luna;
using BitDiamond.Core.Models;
using BitDiamond.Web.Infrastructure.Utils;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
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

                //in future, a realtime event will notify the bearer-provider of changes to a logon, so we dont need to keep quering the database
                var logon = _cache.GetOrRefresh<UserLogon>(token);

                //Note that if "logon"' is null, it means that it was not found either in the cache or in the db - but the fact that this method was called
                //means that the token was verified by the authorization server: this is an anomaly, as the source of the token is in question. What we do
                //is reject this request.
                if (logon?.Invalidated ?? true) context.Rejected();

                //implement traditional session timeout after "WebConstants.Misc_SessionTimeoutMinutes" minutes
                else if ((DateTime.Now - logon.ModifiedOn) >= TimeSpan.FromMinutes(WebConstants.Misc_SessionTimeoutMinutes))
                {
                    logon.ModifiedOn = DateTime.Now;
                    context.Rejected();
                }
                else
                {
                    context.OwinContext.Environment[WebConstants.Misc_UserLogonOwinContextKey] = logon;
                    context.Validated();
                }
            });
        }
    }
}