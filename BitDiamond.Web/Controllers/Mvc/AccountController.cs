﻿using System;
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
        public ActionResult Index() => View();

        [HttpGet]
        [Route("signin")]
        public ActionResult Signin() => View();

        [HttpGet]
        [Route("recovery-request")]
        public ActionResult RecoveryRequest() => View();

        [HttpGet]
        [Route("signup")]
        public ActionResult Signup() => View();

        [HttpGet]
        [Route("verify-registration")]
        public ActionResult VerifyRegistration() => View();
    }
}