using Axis.Luna;
using Axis.Luna.Extensions;
using Axis.Pollux.Identity.Principal;
using BitDiamond.Core.Models;
using BitDiamond.Core.Services;
using BitDiamond.Core.Utils;
using BitDiamond.Web.Controllers.Api.ReferralsModels;
using BitDiamond.Web.Infrastructure.Exceptions;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Web.Http;
using static Axis.Luna.Extensions.ExceptionExtensions;

namespace BitDiamond.Web.Controllers.Api
{
    public class ReferralsController : ApiController
    {
        private IReferralManager _referrals = null;

        #region init
        public ReferralsController(IReferralManager referralManager)
        {
            ThrowNullArguments(() => referralManager);

            _referrals = referralManager;
        }
        #endregion


        [HttpGet, Route("api/referrals")]
        public IHttpActionResult UserRef(string data)
        => Operation.Try(() => ThrowIfFail(() => Encoding.UTF8.GetString(Convert.FromBase64String(data)), ex => new MalformedApiArgumentsException()))
            .Then(_jopr => ThrowIfFail(() => JsonConvert.DeserializeObject<UserArgs>(_jopr.Result, Constants.Misc_DefaultJsonSerializerSettings), ex => new MalformedApiArgumentsException()))
            .Then(argopr => _referrals.GetUserRef(argopr.Result.UserId))
            .OperationResult(Request);


        [HttpGet, Route("api/referrals/current")]
        public IHttpActionResult CurrentUserRef()
        => this._referrals.CurrentUserRef().OperationResult(Request);


        [HttpGet, Route("api/referrals/downlines/direct")]
        public IHttpActionResult DirectDownlines(string data)
        => Operation.Try(() => ThrowIfFail(() => Encoding.UTF8.GetString(Convert.FromBase64String(data)), ex => new MalformedApiArgumentsException()))
            .Then(_jopr => ThrowIfFail(() => JsonConvert.DeserializeObject<ReferralNode>(_jopr.Result, Constants.Misc_DefaultJsonSerializerSettings), ex => new MalformedApiArgumentsException()))
            .Then(argopr => _referrals.DirectDownlines(argopr.Result))
            .OperationResult(Request);


        [HttpGet, Route("api/referrals/downlines")]
        public IHttpActionResult AllDownlines(string data)
        => Operation.Try(() => ThrowIfFail(() => Encoding.UTF8.GetString(Convert.FromBase64String(data)), ex => new MalformedApiArgumentsException()))
            .Then(_jopr => ThrowIfFail(() => JsonConvert.DeserializeObject<ReferralNode>(_jopr.Result, Constants.Misc_DefaultJsonSerializerSettings), ex => new MalformedApiArgumentsException()))
            .Then(argopr => _referrals.AllDownlines(argopr.Result))
            .OperationResult(Request);


        [HttpGet, Route("api/referrals/uplines")]
        public IHttpActionResult Uplines(string data)
        => Operation.Try(() => ThrowIfFail(() => Encoding.UTF8.GetString(Convert.FromBase64String(data)), ex => new MalformedApiArgumentsException()))
            .Then(_jopr => ThrowIfFail(() => JsonConvert.DeserializeObject<ReferralNode>(_jopr.Result, Constants.Misc_DefaultJsonSerializerSettings), ex => new MalformedApiArgumentsException()))
            .Then(argopr => _referrals.AllDownlines(argopr.Result))
            .OperationResult(Request);
    }

    namespace ReferralsModels
    {
        public class UserArgs
        {
            public string UserId { get; set; }
        }
    }
}
