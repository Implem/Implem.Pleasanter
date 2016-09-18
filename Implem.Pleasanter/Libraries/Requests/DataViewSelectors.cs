using System.Web;
namespace Implem.Pleasanter.Libraries.Requests
{
    public static class DataViewSelectors
    {
        public static string Get(long siteId)
        {
            return HttpContext.Current.Session["DataViewSelector" + siteId] != null
                ? HttpContext.Current.Session["DataViewSelector" + siteId].ToString()
                : "index";
        }

        public static void Set(long siteId)
        {
            HttpContext.Current.Session["DataViewSelector" + siteId] = Routes.Action().ToLower();
        }
    }
}