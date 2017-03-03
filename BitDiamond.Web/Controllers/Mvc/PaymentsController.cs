using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BitDiamond.Web.Controllers.Mvc
{
    [RoutePrefix("payments")]
    public class PaymentsController : Controller
    {
        // GET: Payments
        [Route("index")]
        public ActionResult Index() => View();

        // GET: Payments
        [Route("incoming")]
        public ActionResult Incoming() => View();

        // GET: Payments
        [Route("outgoing")]
        public ActionResult Outgoing() => View();
    }
}