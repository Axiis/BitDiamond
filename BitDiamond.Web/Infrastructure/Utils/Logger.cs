using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Web.Http;

namespace BitDiamond.Web.Infrastructure.Utils
{
    public static class Logger
    {
        public static R Log<R>(string typeName, Func<R> execute, [CallerMemberName]string name = null)
        {
            var start = DateTime.Now;
            try
            {
                return execute();
            }
            finally
            {
                //Debug.WriteLine($"Executed {typeName}.{name} in {DateTime.Now - start}");
            }
        }

        public static R Log<R>(this ApiController controller, Func<R> execute, [CallerMemberName] string name = null)
        => Log(controller.GetType().FullName, execute, name);
    }
}