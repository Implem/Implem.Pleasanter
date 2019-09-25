using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Libraries.Responses
{
    public static class ResponseViews
    {
        public static ResponseCollection View(
            this ResponseCollection res, Context context, SiteSettings ss, View view)
        {
            return res
                .ViewFilters(context: context, ss: ss, view: view)
                .ClearFormData("View", "startsWith");
        }

        private static ResponseCollection ViewFilters(
            this ResponseCollection res, Context context, SiteSettings ss, View view)
        {
            switch (context.Forms.ControlId())
            {
                case "ViewSelector":
                case "ReduceViewFilters":
                case "ExpandViewFilters":
                    return res.ReplaceAll("#ViewFilters", new HtmlBuilder()
                        .ViewFilters(context: context, ss: ss, view: view))
                        .ReplaceAll("#ShowHistoryField", 
                            new HtmlBuilder().FieldCheckBox(
                                fieldId: "ShowHistoryField",
                                fieldCss: "field-auto-thin",
                                controlId: "ViewFilters_ShowHistory",
                                controlCss: " auto-postback",
                                method: "post",
                                _checked: view.ShowHistory == true,
                                labelText: Displays.ShowHistory(context: context),
                                _using: ss.HistoryOnGrid == true));
                default:
                    return res;
            }
        }
    }
}