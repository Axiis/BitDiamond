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
                        //lazily run and store all generators in the owin context. Note that the order of adding these things matters, as subsequent services have access to previous services
                        var list = new Dictionary<string, Lazy<object>>();
                        cxt.Environment[PerRequestValues] = list;
                        _generators.ForAll((_cnt, _gen) => list.Add(_gen.Key, new Lazy<object>(() => _gen.Value.Invoke(cxt))));

                        await next();
                    }
                    finally
                    {
                        //dispose all disposable generators
                        cxt.Environment[PerRequestValues]?
                           .As<Dictionary<string, Lazy<object>>>()
                           .Where(_kvp => _kvp.Value.IsValueCreated)
                           .ForAll((_cnt, _kvp) => _kvp.Value.Value.As<IDisposable>()?.Dispose());

                        cxt.Environment[PerRequestValues] = null;
                    }
                });
            }

            _generators[key] = generator;
            return app;
        }

        public static Value GetPerRequestValue<Value>(this IOwinContext context, string key)
        => context.Environment[PerRequestValues].As<Dictionary<string, Lazy<object>>>()[key].Value.As<Value>();
    }
}