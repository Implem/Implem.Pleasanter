using Implem.Libraries.Utilities;
using System.Web;
namespace Implem.Pleasanter.Libraries.Requests
{
    public class QueryStrings
    {
        public static int Int(string key)
        {
            return Data(key).ToInt();
        }

        public static string Data(string key)
        {
            return HttpContext.Current.Request.QueryString[key] != null
                ? HttpContext.Current.Request.QueryString[key]
                : string.Empty;
        }
    }
}