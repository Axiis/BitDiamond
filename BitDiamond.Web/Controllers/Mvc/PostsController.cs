using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BitDiamond.Web.Controllers.Mvc
{
    public class PostsController : Controller
    {
        // GET: Posts
        [HttpGet, Route("posts"), Route("posts/index")]
        public ActionResult Index() => View();

        // GET: Posts
        [HttpGet, Route("posts/details")]
        public ActionResult Details() => View();

        // GET: Posts
        [HttpGet, Route("posts/edit")]
        public ActionResult Edit() => View();

        [HttpGet, Route("posts/list")]
        public ActionResult List() => View();
    }
}