using System.Web.Mvc;
using System.Web.Routing;

namespace SimpleFiles
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.LowercaseUrls = true;
            routes.MapMvcAttributeRoutes();

            routes.MapRoute("Default", "{controller}/{action}/{id}",
                new {controller = "Entry", action = "Index", id = UrlParameter.Optional}
                );
        }
    }
}