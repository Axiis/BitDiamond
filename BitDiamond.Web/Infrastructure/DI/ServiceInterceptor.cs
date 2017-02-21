using Castle.DynamicProxy;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using static Axis.Luna.Extensions.ExceptionExtensions;

namespace BitDiamond.Web.Infrastructure.DI
{
    public class ServiceInterceptor<Service> : IInterceptor
    where Service: class
    {
        private Service _service = null;
        private Container _container = null;

        public ServiceInterceptor(Container container)
        {
            ThrowNullArguments(() => container);

            _container = container;
        }

        public void Intercept(IInvocation invocation)
        {
            throw new NotImplementedException();
        }
    }
}