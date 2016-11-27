using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Libraries.Responses
{
    public static class ResponseViews
    {
        public static ResponseCollection View(
            this ResponseCollection res, SiteSettings ss, View view)
        {
            return res
                .ViewFilters(ss, view)
                .ClearFormData("View", "startsWith");
        }

        private static ResponseCollection ViewFilters(
            this ResponseCollection res, SiteSettings ss, View view)
        {
            var controlId = Forms.ControlId();
            switch (Forms.ControlId())
            {
                case "ViewSelector":
                case "ReduceViewFilters":
                case "ExpandViewFilters":
                    return res.ReplaceAll("#ViewFilters", new HtmlBuilder().ViewFilters(ss, view));
                default:
                    return res;
            }
        }
    }
}