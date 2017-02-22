using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BitDiamond.Web.Controllers.Mvc
{
    [RoutePrefix("profile")]
    public class ProfileController : Controller
    {
        [HttpGet, Route("index")]
        public ActionResult Index() => View();


        [HttpGet, Route("dashboard")]
        public ActionResult Dashboard() => View();

        /// <summary>
        /// shows the conventional profile information like user bio, and stuff.
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("home")]
        public ActionResult Home() => View();
    }
}