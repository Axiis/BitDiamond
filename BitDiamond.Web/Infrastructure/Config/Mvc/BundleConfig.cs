using System.Web;
using System.Web.Optimization;

namespace BitDiamond.Web.Infrastructure.Config.Mvc
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region Script bundles

            ////jquery
            //bundles.Add(new ScriptBundle("~/jquery/scrirpts").Include(
            //            "~/Scripts/jquery-{version}.js",
            //            "~/Scripts/jquery.validate*"));

            //// Use the development version of Modernizr to develop with and learn from. Then, when you're
            //// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/modernizr/scripts").Include(
            //            "~/Scripts/modernizr-*"));

            ////bootstrap
            ////bundles.Add(new ScriptBundle("~/bootstrap/styles").Include(
            ////          "~/Scripts/bootstrap.js"));

            ////londinium
            //bundles.Add(new ScriptBundle("~/themes/londinium/scirpts").Include(
            //    ""));

            ////app
            //bundles.Add(new ScriptBundle("~/app-content/scripts").Include(
            //    ));

            //#endregion


            //#region Css Bundles

            ////bootstrap
            ////bundles.Add(new StyleBundle("~/bootstrap/styles").Include(
            ////          "~/Content/bootstrap.css",
            ////          "~/Content/bootstrap-theme.css"));

            ////londinium
            //bundles.Add(new StyleBundle("~/themes/londinium/styles").Include(
            //          "/themes/londinium/css/bootstrap.min.css",
            //          "/themes/londinium/css/londinium-theme.css",
            //          "/themes/londinium/css/styles.css",
            //          "/themes/londinium/css/icons.css"));

            ////app
            //bundles.Add(new StyleBundle("~/app-content/css").Include(
            //          "~/Content/site.css"));

            #endregion
        }
    }
}
