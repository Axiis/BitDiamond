using System.Web.Mvc;

namespace BitDiamond.Web.Controllers.Mvc
{
    public class NotificationController : Controller
    {
        // GET: Notification
        [HttpGet, Route("notifications/index"), Route("notifications")]
        public ActionResult Index() => View();

        // GET: Notification
        [HttpGet, Route("notifications/details")]
        public ActionResult Details() => View();

        // GET: Notification
        [HttpGet, Route("notifications/history")]
        public ActionResult History() => View();
    }
}