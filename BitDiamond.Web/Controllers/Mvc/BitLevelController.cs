using System.Web.Mvc;

namespace BitDiamond.Web.Controllers.Mvc
{
    [RoutePrefix("bit-level")]
    public class BitLevelController : Controller
    {
        [HttpGet, Route("Index")]
        // GET: BitLevel
        public ActionResult Index() => View();

        [HttpGet, Route("Home")]
        public ActionResult Home() => View();

        [HttpGet, Route("History")]
        public ActionResult History() => View();

        [HttpGet, Route("Bitcoin-Addresses")]
        public ActionResult CoinAddresses() => View();
    }
}