using System.Web.Mvc;
using System.Web.Routing;
namespace Implem.Pleasanter
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}",
                defaults: new
                {
                    Controller = "Items",
                    Action = "Index"
                },
                constraints: new
                {
                    Controller = "[A-Za-z][A-Za-z0-9_]*",
                    Action = "[A-Za-z][A-Za-z0-9_]*"
                }
            );
            routes.MapRoute(
                name: "Permissions",
                url: "{reference}/{id}/{controller}/{action}",
                defaults: new
                {
                    Action = "Index"
                },
                constraints: new
                {
                    Reference = "[A-Za-z][A-Za-z0-9_]*",
                    Id = "[0-9]+",
                    Controller = "ExportSettings|Images|OutgoingMails|Permissions",
                    Action = "[A-Za-z][A-Za-z0-9_]*"
                }
            );
            routes.MapRoute(
                name: "Item",
                url: "{controller}/{id}/{action}",
                defaults: new
                {
                    Controller = "Items",
                    Action = "Edit"
                },
                constraints: new
                {
                    Id = "[0-9]+",
                    Action = "[A-Za-z][A-Za-z0-9_]*"
                }
            );
        }
    }
}