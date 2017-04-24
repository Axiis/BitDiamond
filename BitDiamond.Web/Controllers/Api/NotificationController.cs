using Axis.Luna;
using Axis.Luna.Extensions;
using BitDiamond.Core.Models.Email;
using BitDiamond.Core.Services;
using BitDiamond.Core.Utils;
using BitDiamond.Web.Controllers.Api.BitLevelControllerModels;
using BitDiamond.Web.Controllers.Api.NotificationModels;
using BitDiamond.Web.Infrastructure.Exceptions;
using BitDiamond.Web.Infrastructure.Utils;
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
        private IEmailPush _messagePush = null;

        #region init
        public NotificationController(IUserNotifier userNotifier, IEmailPush messagePush)
        {
            ThrowNullArguments(() => userNotifier, 
                               () => messagePush);

            _notifier = userNotifier;
            _messagePush = messagePush;
        }
        #endregion


        [HttpPost, Route("api/notifications/support")]
        public IHttpActionResult NotifySupport([FromBody] MessageData data)
        => this.Log(() => Operation.Try(() => data.ThrowIfNull(new MalformedApiArgumentsException()))
        .Then(opr =>
        {
            return _messagePush.SendMail(new SupportMessage
            {
                FirstName = data.FirstName,
                LastName = data.LastName,
                Subject = data.Subject,
                Message = data.Message,
                From = data.Email,
                Recipients = new[] { "support@bitdiamond.biz" }
            });
        })
        .OperationResult(Request));


        [HttpPut, Route("api/notifications/single")]
        public IHttpActionResult ClearNotification([FromBody] NotificationData data)
        => this.Log(() => Operation.Try(() => data.ThrowIfNull(new MalformedApiArgumentsException()))
            .Then(opr => _notifier.ClearNotification(data.Id))
            .OperationResult(Request));

        [HttpPut, Route("api/notifications")]
        public IHttpActionResult ClearAll()
        => this.Log(() => _notifier.ClearAll().OperationResult(Request));

        [HttpGet, Route("api/notifications")]
        public IHttpActionResult NotificationHistory()
        => this.Log(() => _notifier.NotificationHistory().OperationResult(Request));

        [HttpGet, Route("api/notifications/unseen")]
        public IHttpActionResult UnseenNotifications()
        => this.Log(() => _notifier.UnseenNotifications().OperationResult(Request));

        [HttpGet, Route("api/notifications/paged")]
        public IHttpActionResult PagedNotificationHistory(string data)
        => this.Log(() => Operation.Try(() => ThrowIfFail(() => Encoding.UTF8.GetString(Convert.FromBase64String(data)), ex => new MalformedApiArgumentsException()))
            .Then(_jopr => ThrowIfFail(() => JsonConvert.DeserializeObject<SequencePageArgs>(_jopr.Result, Constants.Misc_DefaultJsonSerializerSettings), ex => new MalformedApiArgumentsException()))
            .Then(argopr => _notifier.PagedNotificationHistory(argopr.Result.PageSize, argopr.Result.PageIndex))
            .OperationResult(Request));

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

        public class MessageData
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Subject { get; set; }
            public string Email { get; set; }
            public string Message { get; set; }
        }
    }
}
