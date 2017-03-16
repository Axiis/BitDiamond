using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Web.Http;

namespace BitDiamond.Web.Infrastructure.Utils
{
    public static class SpeedLogger
    {
        public static R LogTime<R>(string typeName, Func<R> execute, [CallerMemberName]string name = null)
        {
            var start = DateTime.Now;
            try
            {
                return execute();
            }
            finally
            {
                Debug.WriteLine($"Executed {typeName}.{name} in {DateTime.Now - start}");
            }
        }

        public static R LogTime<R>(this ApiController controller, Func<R> execute, [CallerMemberName] string name = null)
        => LogTime(controller.GetType().FullName, execute, name);
    }
}