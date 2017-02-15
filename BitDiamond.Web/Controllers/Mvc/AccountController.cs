using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BitDiamond.Web.Controllers.Mvc
{
    [RoutePrefix("account")]
    public class AccountController : Controller
    {
        [HttpGet]
        [Route("index")]
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        [Route("login")]
        public ActionResult Login()
        {
            return View();
        }
    }
}