using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using System.Web.Mvc;
namespace Implem.Pleasanter.Libraries.Resources
{
    public static class Css
    {
        public static string Class(string _default, string additional)
        {
            if (additional.IsNullOrEmpty())
            {
                return _default?.Trim();
            }
            else if (_default?.EndsWith(" ") == true)
            {
                return (_default + additional).Trim();
            }
            else if (additional.Substring(0, 1) == " ")
            {
                return (_default?.Trim() + additional).Trim();
            }
            else
            {
                return additional;
            }
        }

        public static ContentResult Get(Context context)
        {
            return new ContentResult
            {
                ContentType = "text/css",
                Content = HtmlStyles.ExtendedStyles(
                    siteId: context.QueryStrings.Long("site-id"),
                    id: context.QueryStrings.Long("id"),
                    controller: context.QueryStrings.Data("controller"),
                    action: context.QueryStrings.Data("action"))
            };
        }
    }
}