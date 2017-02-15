using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UAParser;

namespace BitDiamond.Test
{
    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void TestMethod1()
        {
            var parser = Parser.GetDefault();

            DateTime start;

            start = DateTime.Now;
            var result = parser.Parse("User-Agent: Mozilla/5.0 (iPad; U; CPU OS 3_2_1 like Mac OS X; en-us) AppleWebKit/531.21.10 (KHTML, like Gecko) Mobile/7B405");
            Console.WriteLine($"parsed in: {DateTime.Now - start}");

            start = DateTime.Now;
            result = parser.Parse("User-Agent: Mozilla/5.0 (iPad; U; CPU OS 3_2_1 like Mac OS X; en-us) AppleWebKit/531.21.10 (KHTML, like Gecko) Mobile/7B405");
            Console.WriteLine($"parsed in: {DateTime.Now - start}");

            start = DateTime.Now;
            result = parser.Parse("User-Agent: Mozilla/5.0 (iPad; U; CPU OS 3_2_1 like Mac OS X; en-us) AppleWebKit/531.21.10 (KHTML, like Gecko) Mobile/7B405");
            Console.WriteLine($"parsed in: {DateTime.Now - start}");

            start = DateTime.Now;
            result = parser.Parse("User-Agent: Mozilla/5.0 (iPad; U; CPU OS 3_2_1 like Mac OS X; en-us) AppleWebKit/531.21.10 (KHTML, like Gecko) Mobile/7B405");
            Console.WriteLine($"parsed in: {DateTime.Now - start}");

            start = DateTime.Now;
            result = parser.Parse("User-Agent: Mozilla/5.0 (iPad; U; CPU OS 3_2_1 like Mac OS X; en-us) AppleWebKit/531.21.10 (KHTML, like Gecko) Mobile/7B405");
            Console.WriteLine($"parsed in: {DateTime.Now - start}");

            Console.WriteLine(result);
        }
    }
}
