using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web.Http;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace BitDiamond.Web.Infrastructure.Utils
{
    public static class Logger
    {
        public static log4net.ILog _Log = log4net.LogManager.GetLogger("X_Logger");
        public static ApiOperationResult<R> Log<R>(string typeName, Func<ApiOperationResult<R>> execute, [CallerMemberName]string name = null)
        {
            var x = execute();
            if (!x.Operation.Succeeded) Task.Run(() => _Log.Error(x.Operation.Message, x.Operation.GetException()));

            return x;
        }

        public static ApiOperationResult<R> Log<R>(this ApiController controller, Func<ApiOperationResult<R>> execute, [CallerMemberName] string name = null)
        => Log(controller.GetType().FullName, execute, name);
    }
}