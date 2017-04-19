using Axis.Luna;
using Axis.Luna.Extensions;
using Axis.Luna.MetaTypes;
using BitDiamond.Core.Utils;
using BitDiamond.Web.Infrastructure.Exceptions;
using BitDiamond.Web.Infrastructure.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using static Axis.Luna.Extensions.ExceptionExtensions;

namespace BitDiamond.Web
{
    public static class Extensions
    {
        public static ApiOperationResult<V> OperationResult<V>(this Operation<V> operation, HttpRequestMessage request, HttpStatusCode? code = null)
            => new ApiOperationResult<V>(operation, request, code);

        public static Operation<Data> DecodeUrlData<Data>(this string data) => data.DecodeUrlData<Data>(Encoding.UTF8);

        public static Operation<Data> DecodeUrlData<Data>(this string data, Encoding encoding)
        => Operation.Try(() => ThrowIfFail(() => encoding.GetString(Convert.FromBase64String(data)), ex => new MalformedApiArgumentsException()))
            .Then(_jopr => ThrowIfFail(() => JsonConvert.DeserializeObject<Data>(_jopr.Result, Constants.Misc_DefaultJsonSerializerSettings), ex => new MalformedApiArgumentsException()));

        
    }
}