using BitDiamond.Web.Infrastructure.Config.Mvc;
using BitDiamond.Web.Infrastructure.DI;
using Hangfire;
using log4net.Config;
using SimpleInjector.Integration.Web;
using System;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace BitDiamond.Web
{
    public class Global : System.Web.HttpApplication
    {

        private BackgroundJobServer _jobServer;

        protected void Application_Start(object sender, EventArgs e)
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //DI
            DependencyResolver.SetResolver(new MvcResolutionContext(new WebRequestLifestyle(), DIRegistrations.RegisterTypes));

            //logging
            XmlConfigurator.Configure();

            //hangfire
            _jobServer = new BackgroundJobServer();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {
            //hangfire
            _jobServer.Dispose();
        }
    }
}