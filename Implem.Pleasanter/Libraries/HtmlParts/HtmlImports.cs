using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlImports
    {
        public static HtmlBuilder ImportSettingsDialog(
            this HtmlBuilder hb, Context context)
        {
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Id("ImportSettingsDialog")
                    .Class("dialog")
                    .Title(Displays.Import(context: context)),
                action: () => hb
                    .FieldTextBox(
                        textType: HtmlTypes.TextTypes.File,
                        controlId: "Import",
                        fieldCss: "field-wide",
                        labelText: Displays.CsvFile(context: context))
                    .FieldDropDown(
                        context: context,
                        controlId: "Encoding",
                        fieldCss: "field-wide",
                        labelText: Displays.CharacterCode(context: context),
                        optionCollection: new Dictionary<string, ControlData>
                        {
                            { "Shift-JIS", new ControlData("Shift-JIS") },
                            { "UTF-8", new ControlData("UTF-8") },
                        })
                    .FieldCheckBox(
                        controlId: "UpdatableImport",
                        fieldCss: "field-wide",
                        labelText: Displays.UpdatableImport(context: context),
                        _checked: false,
                        controlCss: " always-send",
                        _using: context.Controller == "items")
                    .P(css: "message-dialog")
                    .Div(
                        css: "command-center",
                        action: () => hb
                            .Button(
                                text: Displays.Import(context: context),
                                controlCss: "button-icon",
                                onClick: "$p.import($(this));",
                                icon: "ui-icon-arrowreturnthick-1-e",
                                action: "Import",
                                method: "post")
                            .Button(
                                text: Displays.Cancel(context: context),
                                controlCss: "button-icon",
                                onClick: "$p.closeDialog($(this));",
                                icon: "ui-icon-cancel")));
        }
    }
}