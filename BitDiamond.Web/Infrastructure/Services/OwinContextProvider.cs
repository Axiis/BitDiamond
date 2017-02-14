using Microsoft.Owin;
using Owin;
using System.Runtime.Remoting.Messaging;

namespace BitDiamond.Web.Infrastructure.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class OwinContextProvider
    {
        /// <summary>
        /// 
        /// </summary>
        public virtual IOwinContext Owin => CallContext.LogicalGetData(OwinContextProviderExtension.CallContextOwinKey) as IOwinContext;
    }

    /// <summary>
    /// 
    /// </summary>
    public static class OwinContextProviderExtension
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly string CallContextOwinKey = "BitDiamond.CallContext.OwinContext";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IAppBuilder UseOwinContextProvider(this IAppBuilder app)
        => app.Use(async (context, next) =>
        {
            CallContext.LogicalSetData(CallContextOwinKey, context);
            await next();
        });
    }
}