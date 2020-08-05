using System.Web.Http;
namespace Implem.Pleasanter
{
    public static class ApiRouteConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "BinariesApi",
                routeTemplate: "api/binaries/{guid}/{action}",
                defaults: new
                {
                    Controller = "Binaries",
                    Action = "Get",
                    Id = RouteParameter.Optional
                });
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}/{action}",
                defaults: new
                {
                    Id = RouteParameter.Optional
                });
            config.Routes.MapHttpRoute(
                name: "WithoutId",
                routeTemplate: "api/{controller}/{action}",
                defaults: new
                {
                    Id = RouteParameter.Optional
                });
            config.Routes.MapHttpRoute(
                name: "OutgoingMails",
                routeTemplate: "api/{reference}/{id}/{controller}/{action}",
                defaults: new
                {
                    Id = RouteParameter.Optional
                });
        }
    }
}