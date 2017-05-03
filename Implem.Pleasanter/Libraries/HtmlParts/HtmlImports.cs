using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Responses;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlImports
    {
        public static HtmlBuilder ImportSettingsDialog(this HtmlBuilder hb)
        {
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Id("ImportSettingsDialog")
                    .Class("dialog")
                    .Title(Displays.Import()),
                action: () => hb
                    .FieldTextBox(
                        textType: HtmlTypes.TextTypes.File,
                        controlId: "Import",
                        fieldCss: "field-wide",
                        labelText: Displays.CsvFile())
                    .FieldDropDown(
                        controlId: "Encoding",
                        fieldCss: "field-wide",
                        labelText: Displays.CharacterCode(),
                        optionCollection: new Dictionary<string, ControlData>
                        {
                            { "Shift-JIS", new ControlData("Shift-JIS") },
                            { "UTF-8", new ControlData("UTF-8") },
                        })
                    .FieldCheckBox(
                        controlId: "UpdatableImport",
                        fieldCss: "field-wide",
                        labelText: Displays.UpdatableImport(),
                        _checked: false,
                        controlCss: " always-send")
                    .P(css: "message-dialog")
                    .Div(
                        css: "command-center",
                        action: () => hb
                            .Button(
                                text: Displays.Import(),
                                controlCss: "button-icon",
                                onClick: "$p.import($(this));",
                                icon: "ui-icon-arrowreturnthick-1-e",
                                action: "Import",
                                method: "post")
                            .Button(
                                text: Displays.Cancel(),
                                controlCss: "button-icon",
                                onClick: "$p.closeDialog($(this));",
                                icon: "ui-icon-cancel")));
        }
    }
}