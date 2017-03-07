using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BitDiamond.Web.Controllers.Mvc
{
    public class ReferralsController : Controller
    {
        // GET: FamilyTree
        [HttpGet, Route("referrals/index")]
        public ActionResult Index() => View();


        // GET: FamilyTree
        [HttpGet, Route("referrals/downlines")]
        public ActionResult Home() => View();
    }
}