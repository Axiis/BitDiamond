using Axis.Luna;
using BitDiamond.Web.Infrastructure.Utils;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BitDiamond.Web
{
    public static class Extensions
    {
        public static ApiOperationResult<V> OperationResult<V>(this Operation<V> operation, HttpRequestMessage request, HttpStatusCode? code = null)
            => new ApiOperationResult<V>(operation, request, code);
    }
}