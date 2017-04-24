using Axis.Luna;
using Axis.Luna.Extensions;
using Newtonsoft.Json;
using System.IO;
using System.Web.Hosting;
using System.Web.Http;

namespace BitDiamond.Web.Controllers.Api
{
    [RoutePrefix("pad")]
    public class PadController : ApiController
    {
        [HttpPut, Route("users/{value}")]
        public IHttpActionResult IncrementUsers(int value)
        {
            Operation.Try(() =>
            {
                Pad pad = null; ;
                new StreamReader(new FileStream(HostingEnvironment.MapPath("~/App_Data/pad.json"), FileMode.OpenOrCreate)).Using(_r =>
                {
                    pad = JsonConvert.DeserializeObject<Pad>(_r.ReadToEnd().Trim());
                });

                pad.users += value;
                new StreamWriter(new FileStream(HostingEnvironment.MapPath("~/App_Data/pad.json"), FileMode.OpenOrCreate)).Using(_w =>
                {
                    _w.Write(JsonConvert.SerializeObject(pad));
                    _w.Flush();
                });
            });

            return Json(@"{""succeeded"":true}");
        }

        [HttpPut, Route("btc")]
        public IHttpActionResult IncrementBtc(decimal value)
        {
            Operation.Try(() =>
            {
                Pad pad = null; ;
                new StreamReader(new FileStream(HostingEnvironment.MapPath("~/App_Data/pad.json"), FileMode.OpenOrCreate)).Using(_r =>
                {
                    pad = JsonConvert.DeserializeObject<Pad>(_r.ReadToEnd().Trim());
                });

                pad.btc += value;
                new StreamWriter(new FileStream(HostingEnvironment.MapPath("~/App_Data/pad.json"), FileMode.OpenOrCreate)).Using(_w =>
                {
                    _w.Write(JsonConvert.SerializeObject(pad));
                    _w.Flush();
                });
            });

            return Json(@"{""succeeded"":true}");
        }
    }


    public class Pad
    {
        public int users { get; set; }
        public decimal btc { get; set; }
    }
}
