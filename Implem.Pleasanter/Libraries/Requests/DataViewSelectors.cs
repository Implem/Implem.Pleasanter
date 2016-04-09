using System.Web;
namespace Implem.Pleasanter.Libraries.Requests
{
    public static class DataViewSelectors
    {
        public static string Get(long siteId)
        {
            return HttpContext.Current.Session["DataViewSelector" + siteId] != null
                ? HttpContext.Current.Session["DataViewSelector" + siteId].ToString()
                : string.Empty;
        }

        public static void Set(long siteId)
        {
            if (Forms.Data("ControlId") == "DataViewSelector")
            {
                HttpContext.Current.Session["DataViewSelector" + siteId] =
                    Forms.Data("DataViewSelector");
            }
        }
    }
}