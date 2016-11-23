using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlDataView
    {
        public static HtmlBuilder DataViewSelector(
            this HtmlBuilder hb, SiteSettings ss, DataView dataView)
        {
            return hb.FieldDropDown(
                fieldId: "DataViewSelectorField",
                controlId: "DataViewSelector",
                fieldCss: "field-auto-thin",
                controlCss: " auto-postback",
                labelText: Displays.DataView(),
                optionCollection: ss.DataViews?.ToDictionary(o =>
                    o.Id.ToString(), o => o.Name),
                selectedValue: ss.DataViews?.FirstOrDefault(o =>
                    o.ToJson() == dataView.ToJson())?.Id.ToString(),
                addSelectedValue: false,
                insertBlank: true,
                method: "post",
                _using: ss.DataViews?.Any() == true);
        }
    }
}