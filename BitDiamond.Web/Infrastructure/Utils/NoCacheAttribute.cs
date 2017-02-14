using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http.Filters;

namespace BitDiamond.Web.Infrastructure.Utils
{
    /// <summary>
    /// Disables Caching of this request
    /// </summary>
    public class NoCacheAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// After the Action has been Executed
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuted(HttpActionExecutedContext context)
        {
            context.Response.Headers.CacheControl = new CacheControlHeaderValue
            {
                Public = true,
                MaxAge = TimeSpan.FromSeconds(0),
                NoCache = true,
                MustRevalidate = true
            };
            base.OnActionExecuted(context);
        }
    }
}