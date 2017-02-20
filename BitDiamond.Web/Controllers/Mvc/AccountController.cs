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
        [Route("signin")]
        public ActionResult Signin()
        {
            return View();
        }

        [HttpGet]
        [Route("recovery-request")]
        public ActionResult RecoveryRequest()
        {
            return View();
        }

        [HttpGet]
        [Route("signup")]
        public ActionResult Signup()
        {
            return View();
        }
    }
}