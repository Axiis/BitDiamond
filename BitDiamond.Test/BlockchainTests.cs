using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using Newtonsoft.Json;
using static BitDiamond.Core.Services.BlockChainService;

namespace BitDiamond.Test
{
    /// <summary>
    /// Summary description for BlockchainTests
    /// </summary>
    [TestClass]
    public class BlockchainTests
    {
        [TestMethod]
        public void TestVerifyAddress()
        {
            var webClient = new WebClient();
            var result = webClient.DownloadString("https://blockchain.info/tx/30dbb3a9c0c5b16b5bf247d7d4baef796ac4fb8d0b0a0097ca78d954e67b627f?show_adv=false&format=json");
            Console.WriteLine(result);
            var tc = JsonConvert.DeserializeObject<TransactionContainer>(result);
        }
    }
}
