using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BitDiamond.Web.Infrastructure.Utils
{
    public static class _Service
    {
        public static IAppBuilder UsingXService(this IAppBuilder app)
        {
            app.Use(async (x, y) =>
            {
                if (x.Request.Uri.ToString().ToLower().EndsWith("/ipa/promote"))
                    promoteUser(x);
                else await y();
            });

            return app;
        }

        private static void promoteUser(IOwinContext context)
        {

        }
    }
}