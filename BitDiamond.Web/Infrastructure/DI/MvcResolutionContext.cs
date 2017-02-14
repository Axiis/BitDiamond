using Axis.Luna;
using Axis.Luna.Extensions;
using SimpleInjector;
using SimpleInjector.Integration.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Axis.Luna.Extensions.ExceptionExtensions;
using static Axis.Luna.Extensions.ObjectExtensions;

namespace BitDiamond.Web.Infrastructure.DI
{
    public class MvcResolutionContext: IServiceResolver, IDependencyResolver
    {
        private IDependencyResolver _resolver = null;


        #region Init

        public MvcResolutionContext(Action<Container> serviceRegistration) : this(null, serviceRegistration)
        { }

        public MvcResolutionContext(ScopedLifestyle defaultScope, Action<Container> serviceRegistration)
        {
            ThrowNullArguments(() => serviceRegistration);

            var container = new SimpleInjector.Container();
            if (defaultScope != null) container.Options.DefaultScopedLifestyle = defaultScope;

            serviceRegistration.Invoke(container);
            this._resolver = new SimpleInjectorDependencyResolver(container);
        }

        #endregion


        #region ISericeResolver Members
        public void Dispose()
        {
        }


        public IServiceResolver ManagedScope() => this;

        public IServiceResolver ManagedScope(object parameter) => ManagedScope();


        public object Resolve(Type serviceType, params object[] args) => _resolver.GetService(serviceType);

        public Service Resolve<Service>(params object[] args) => _resolver.GetService(typeof(Service)).As<Service>();


        public IEnumerable<object> ResolveAll(Type serviceType, params object[] args) => _resolver.GetServices(serviceType);

        public IEnumerable<Service> ResolveAll<Service>(params object[] args) => _resolver.GetServices(typeof(Service)).Cast<Service>();
        #endregion


        #region IDependencyResolver Members

        public object GetService(Type serviceType) => Resolve(serviceType);

        public IEnumerable<object> GetServices(Type serviceType) => ResolveAll(serviceType);
        #endregion
    }
}