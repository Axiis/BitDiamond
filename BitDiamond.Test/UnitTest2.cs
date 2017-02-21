using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UAParser;
using Castle.DynamicProxy;
using Axis.Luna.Extensions;

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


        [TestMethod]
        public void TestMethod2()
        {
            var gen = new ProxyGenerator();
            var somethingProxy = gen.CreateInterfaceProxyWithoutTarget<ISomething>(new Interceptor());

            var t = somethingProxy.Property;
            var u = somethingProxy.Method();
        }
    }

    public class SomeClass
    {
        public string Name { get; set; }
    }

    public interface ISomething
    {
        string Method();
        int Property { get; }
    }

    public class Interceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            invocation.ReturnValue = invocation.Method.ReturnType.DefaultValue();
            invocation.Proceed();
        }
    }
}
