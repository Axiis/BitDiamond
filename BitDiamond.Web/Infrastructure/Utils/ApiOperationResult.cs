using Axis.Luna;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace BitDiamond.Web.Infrastructure.Utils
{
    public class ApiOperationResult<V> : IHttpActionResult
    {
        public Operation<V> Operation { get; private set; }
        public HttpStatusCode Code { get; private set; } = HttpStatusCode.InternalServerError;
        private HttpRequestMessage Request { get; set; }

        public ApiOperationResult(Operation<V> operation, HttpRequestMessage request, HttpStatusCode? code = HttpStatusCode.OK)
        {
            Operation = operation;
            Request = request;

            Code = code ?? (operation.Succeeded ? HttpStatusCode.OK : ExtractCode(operation));
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            => Task.FromResult(Request.CreateResponse(Code, Operation, Request.GetConfiguration().Formatters.JsonFormatter));

        /// <summary>
        /// Use whatever means necessary to decide on what code to export to the api-caller based on information present in the operation class
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        private HttpStatusCode ExtractCode(Operation<V> operation)
        {
            if (operation.GetException() is ValidationException) return HttpStatusCode.Conflict;
            return HttpStatusCode.InternalServerError;
        }
    }
}