using Axis.Luna;
using Axis.Luna.Extensions;
using BitDiamond.Core.Services;
using BitDiamond.Core.Utils;
using BitDiamond.Web.Controllers.Api.BitLevelControllerModels;
using BitDiamond.Web.Controllers.Api.NotificationModels;
using BitDiamond.Web.Infrastructure.Exceptions;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Web.Http;

using static Axis.Luna.Extensions.ExceptionExtensions;

namespace BitDiamond.Web.Controllers.Api
{
    public class NotificationController : ApiController
    {
        private IUserNotifier _notifier = null;

        #region init
        public NotificationController(IUserNotifier userNotifier)
        {
            ThrowNullArguments(() => userNotifier);

            _notifier = userNotifier;
        }
        #endregion


        [HttpPut, Route("api/notifications/single")]
        public IHttpActionResult ClearNotification([FromBody] NotificationData data)
        => Operation.Try(() => data.ThrowIfNull(new MalformedApiArgumentsException()))
            .Then(opr => _notifier.ClearNotification(data.Id))
            .OperationResult(Request);

        [HttpPut, Route("api/notifications")]
        public IHttpActionResult ClearAll()
        => _notifier.ClearAll().OperationResult(Request);

        [HttpGet, Route("api/notifications")]
        public IHttpActionResult NotificationHistory()
        => _notifier.NotificationHistory().OperationResult(Request);

        [HttpGet, Route("api/notifications/unseen")]
        public IHttpActionResult UnseenNotifications()
        => _notifier.UnseenNotifications().OperationResult(Request);

        [HttpGet, Route("api/notifications/paged")]
        public IHttpActionResult PagedNotificationHistory(string data)
        => Operation.Try(() => ThrowIfFail(() => Encoding.UTF8.GetString(Convert.FromBase64String(data)), ex => new MalformedApiArgumentsException()))
            .Then(_jopr => ThrowIfFail(() => JsonConvert.DeserializeObject<SequencePageArgs>(_jopr.Result, Constants.Misc_DefaultJsonSerializerSettings), ex => new MalformedApiArgumentsException()))
            .Then(argopr => _notifier.PagedNotificationHistory(argopr.Result.PageSize, argopr.Result.PageIndex))
            .OperationResult(Request);

    }

    namespace NotificationModels
    {
        public class NotificationData
        {
            public long Id { get; set; }
        }

        public class PageData
        {
            public int PageSize { get; set; }
            public int PageIndex { get; set; }
        }
    }
}
