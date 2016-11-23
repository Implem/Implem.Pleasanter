using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Libraries.Responses
{
    public static class ResponseDataViews
    {
        public static ResponseCollection DataView(
            this ResponseCollection res, SiteSettings ss, DataView dataView)
        {
            return res
                .DataViewFilters(ss, dataView)
                .ClearFormData("DataView", "startsWith");
        }

        private static ResponseCollection DataViewFilters(
            this ResponseCollection res, SiteSettings ss, DataView dataView)
        {
            var controlId = Forms.ControlId();
            switch (Forms.ControlId())
            {
                case "DataViewSelector":
                case "ReduceDataViewFilters":
                case "ExpandDataViewFilters":
                    return res.ReplaceAll("#DataViewFilters", new HtmlBuilder()
                        .DataViewFilters(ss, dataView));
                default:
                    return res;
            }
        }
    }
}