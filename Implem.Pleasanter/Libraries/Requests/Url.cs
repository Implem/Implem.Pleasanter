using System.Web;
using System.Web.Routing;
namespace Implem.Pleasanter.Libraries.Requests
{
    public static class Url
    {
        public static string Server()
        {
            return
                HttpContext.Current.Request.Url.Scheme + "://" +
                HttpContext.Current.Request.Url.Authority;
        }

        public static string ApplicationPath()
        {
            var path = HttpContext.Current.Request.ApplicationPath;
            return path.EndsWith("/")
                ? path 
                : path + "/";
        }

        public static string AbsoluteUri()
        {
            return HttpContext.Current.Request.Url.AbsoluteUri;
        }

        public static string LocalPath()
        {
            return HttpContext.Current.Request.Url.LocalPath;
        }

        public static string AbsolutePath()
        {
            return HttpContext.Current.Request.Url.AbsolutePath;
        }

        public static string UrlReferrer()
        {
            return HttpContext.Current.Request.UrlReferrer?.ToString() ?? string.Empty;
        }

        public static string RouteData(string name)
        {
            return RouteTable.Routes
                .GetRouteData(new HttpContextWrapper(HttpContext.Current))
                .Values[name]?
                .ToString();
        }

        public static string Encode(string url)
        {
            return HttpUtility.UrlEncode(url);
        }
    }
}