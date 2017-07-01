using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlExport
    {
        public static HtmlBuilder ExportSelectorDialog(this HtmlBuilder hb, SiteSettings ss)
        {
            var optionCollection = ss.Exports.ToDictionary(o => o.Id.ToString(), o => o.Name);
            optionCollection.Add("0", Displays.Standard());
            return hb
                .FieldDropDown(
                    controlId: "ExportId",
                    controlCss: " always-send",
                    labelText: Displays.Format(),
                    optionCollection: optionCollection)
                .P(css: "message-dialog")
                .Div(css: "command-center", action: () => hb
                    .Button(
                        text: Displays.Export(),
                        controlCss: "button-icon",
                        onClick: "$p.export();",
                        icon: "ui-icon-arrowreturnthick-1-w")
                    .Button(
                        text: Displays.Cancel(),
                        controlCss: "button-icon",
                        onClick: "$p.closeDialog($(this));",
                        icon: "ui-icon-cancel"));
        }
    }
}