using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlExport
    {
        public static HtmlBuilder ExportSelectorDialog(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            var optionCollection = ExportUtilities.GetAccessibleTemplates(
                context:context,
                ss:ss);
            return hb
                .FieldDropDown(
                    context: context,
                    controlId: "ExportId",
                    controlCss: " always-send",
                    labelText: Displays.Format(context: context),
                    optionCollection: optionCollection)
                .FieldDropDown(
                    context: context,
                    controlId: "ExportEncoding",
                    controlCss: " always-send",
                    labelText: Displays.CharacterCode(context: context),
                    optionCollection: new Dictionary<string, ControlData>
                    {
                        { "Shift-JIS", new ControlData("Shift-JIS") },
                        { "UTF-8", new ControlData("UTF-8") },
                    })
                .P(css: "message-dialog")
                .Div(css: "command-center", action: () => hb
                    .Button(
                        text: Displays.Export(context: context),
                        controlId: "DoExport",
                        controlCss: "button-icon button-positive" + (ss.SaveViewType == SiteSettings.SaveViewTypes.None
                             ? " save-view-types-none"
                             : string.Empty),
                        action: "ExportAndMailNotify",
                        method: "post",
                        onClick: "$p.export();",
                        icon: "ui-icon-arrowreturnthick-1-w")
                    .Button(
                        text: Displays.Cancel(context: context),
                        controlCss: "button-icon button-neutral",
                        onClick: "$p.closeDialog($(this));",
                        icon: "ui-icon-cancel"));
        }
    }
}