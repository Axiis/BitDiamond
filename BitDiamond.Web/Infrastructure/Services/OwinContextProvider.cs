using Microsoft.Owin;
using Owin;
using System.Runtime.Remoting.Messaging;

namespace BitDiamond.Web.Infrastructure.Services
{
    public class OwinContextProvider
    {
        public virtual IOwinContext Owin => CallContext.LogicalGetData(OwinContextProviderExtension.CallContextOwinKey) as IOwinContext;
    }
    
    public static class OwinContextProviderExtension
    {
        public static readonly string CallContextOwinKey = "BitDiamond.CallContext.OwinContext";
        
        public static IAppBuilder UseOwinContextProvider(this IAppBuilder app)
        => app.Use(async (context, next) =>
        {
            try
            {
                CallContext.LogicalSetData(CallContextOwinKey, context);
                await next();
            }
            finally
            {
                CallContext.FreeNamedDataSlot(CallContextOwinKey);
            }
        });
    }
}