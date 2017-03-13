using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BitDiamond.Core.Utils;

namespace BitDiamond.Test
{
    [TestClass]
    public class BitLevelTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var r = BitCycle.Create(1, 0);
            var r2 = r.Increment(4);

            Console.WriteLine(r2);
        }
    }
}
