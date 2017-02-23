using Axis.Luna.Extensions;
using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BitDiamond.Web.Infrastructure.Utils
{
    public static class RunPerRequestHelper
    {
        private static Dictionary<string, Func<IOwinContext, object>> _generators = new Dictionary<string, Func<IOwinContext, object>>();
        private static readonly string PerRequestValues = "$__BitDiamond.PerRequestValues";

        public static IAppBuilder RunPerRequest(this IAppBuilder app, string key, Func<IOwinContext, object> generator)
        {
            if(_generators.Count == 0)
            {
                app.Use(async (cxt, next) =>
                {
                    try
                    {
                        //run and store all generators in the owin context. Note that the order of adding these things matters, as subsequent services have access to previous services
                        //
                        _generators.ForAll((_cnt, _gen) => cxt.Environment[$"{PerRequestValues}{_gen.Key}"] = _gen.Value.Invoke(cxt));

                        await next();

                    }
                    finally
                    {
                        //dispose all disposable generators
                        var keys = _generators.Keys.Select(_k => $"{PerRequestValues}{_k}").ToArray();
                        cxt.Environment
                           .Where(_kvp => keys.Contains(_kvp.Key))
                           .Where(_kvp => _kvp.Value is IDisposable)
                           .ForAll((_cnt, _v) => _v.As<IDisposable>().Dispose());
                    }
                });
            }

            _generators[key] = generator;
            return app;
        }

        public static Value GetPerRequestValue<Value>(this IOwinContext context, string key)
        => context.Environment[$"{PerRequestValues}{key}"].As<Value>();
    }
}