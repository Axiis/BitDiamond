using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleInjector;
using BitDiamond.Web.Infrastructure.DI;
using SimpleInjector.Extensions.ExecutionContextScoping;
using BitDiamond.Core.Services;
using Axis.Luna.Extensions;
using Axis.Pollux.Authentication;

namespace BitDiamond.Test
{
    [TestClass]
    public class DIRegistrationTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new ExecutionContextScopeLifestyle();

            DIRegistrations.RegisterTypes(container);
            container.BeginExecutionContextScope().Using(cxt =>
            {
                var x = cxt.GetInstance<ICredentialHasher>();

                var start = DateTime.Now;
                x.CalculateHash("abcdefghijkl");
                Console.WriteLine($"First call: {DateTime.Now - start}");

                start = DateTime.Now;
                x.CalculateHash("abcdefghijkl");
                Console.WriteLine($"Second call: {DateTime.Now - start}");

                start = DateTime.Now;
                x.CalculateHash("abcdefghijkl");
                Console.WriteLine($"Third call: {DateTime.Now - start}");
            });
        }
    }
}
