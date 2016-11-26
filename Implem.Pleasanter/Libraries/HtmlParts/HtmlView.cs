using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlView
    {
        public static HtmlBuilder ViewSelector(this HtmlBuilder hb, SiteSettings ss, View view)
        {
            return hb.FieldDropDown(
                fieldId: "ViewSelectorField",
                controlId: "ViewSelector",
                controlCss: " auto-postback",
                labelText: Displays.DataView(),
                optionCollection: ss.Views?.ToDictionary(o =>
                    o.Id.ToString(), o => o.Name),
                selectedValue: ss.Views?.FirstOrDefault(o =>
                    o.ToJson() == view.ToJson())?.Id.ToString(),
                addSelectedValue: false,
                insertBlank: true,
                method: "post",
                _using: ss.Views?.Any() == true);
        }
    }
}