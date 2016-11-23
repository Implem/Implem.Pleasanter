using System.Web;
namespace Implem.Pleasanter.Libraries.Requests
{
    public static class ViewModes
    {
        public static string GetBySession(long siteId)
        {
            return HttpContext.Current.Session["ViewMode" + siteId] != null
                ? HttpContext.Current.Session["ViewMode" + siteId].ToString()
                : "index";
        }

        public static void Set(long siteId)
        {
            HttpContext.Current.Session["ViewMode" + siteId] = Routes.Action().ToLower();
        }
    }
}