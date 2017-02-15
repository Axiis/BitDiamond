using System.Web;
using System.Web.Mvc;

namespace BitDiamond.Web.Infrastructure.Config.Mvc
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
