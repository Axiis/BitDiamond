using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BitDiamond.Web.Infrastructure.Services;

namespace BitDiamond.Test
{
    [TestClass]
    public class EmailPushTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var pushService = new ElasticMailPushService();
            var x = pushService.SendMail(null);
        }
    }
}
