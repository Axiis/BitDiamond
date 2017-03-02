using Axis.Luna;
using Axis.Luna.Extensions;
using BitDiamond.Core.Services;
using BitDiamond.Core.Utils;
using BitDiamond.Web.Controllers.Api.SettingsModels;
using BitDiamond.Web.Infrastructure.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

using static Axis.Luna.Extensions.ExceptionExtensions;

namespace BitDiamond.Web.Controllers.Api
{
    public class SettingsController : ApiController
    {
        private ISettingsManager _settings;

        public SettingsController(ISettingsManager settingsManager)
        {
            ThrowNullArguments(() => settingsManager);

            _settings = settingsManager;
        }

        [HttpPut, Route("api/system-settings")]
        public IHttpActionResult ModifySetting([FromBody] SettingsArgs args)
        => _settings.ModifySetting(args?.Name, args?.Value).OperationResult(Request);


        [HttpGet, Route("api/system-settings/all")]
        public IHttpActionResult GetSettings()
        => _settings.GetSettings().OperationResult(Request);


        [HttpGet, Route("api/system-settings/filter")]
        public IHttpActionResult GetSetting(string data)
        => Operation.Try(() => ThrowIfFail(() => Encoding.UTF8.GetString(Convert.FromBase64String(data)), ex => new MalformedApiArgumentsException()))
            .Then(_jopr => ThrowIfFail(() => JsonConvert.DeserializeObject<SettingsArgs>(_jopr.Result, Constants.Misc_DefaultJsonSerializerSettings), ex => new MalformedApiArgumentsException()))
            .Then(argopr => _settings.GetSetting(argopr.Result.Name))
            .OperationResult(Request);
    }

    namespace SettingsModels
    {
        public class SettingsArgs
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }
    }
}
