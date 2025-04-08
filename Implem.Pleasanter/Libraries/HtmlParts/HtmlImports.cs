using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
using System.Linq;

namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlImports
    {
        public static HtmlBuilder ImportSettingsDialog(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss)
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
                        },
                        selectedValue: ss.ImportEncoding)
                    .FieldCheckBox(
                        controlId: "UpdatableImport",
                        fieldCss: "field-wide",
                        labelText: Displays.UpdatableImport(context: context),
                        _checked: ss.UpdatableImport == true,
                        controlCss: " always-send",
                        _using: context.Controller == "items")
                    .FieldDropDown(
                        context: context,
                        fieldId: "KeyField",
                        controlId: "Key",
                        fieldCss: "field-wide" + (ss.UpdatableImport == true
                            ? string.Empty
                            : " hidden"),
                        labelText: Displays.Key(context: context),
                        optionCollection: ss.Columns
                            ?.Where(o => o.ImportKey == true)
                            .Where(o => !o.Joined)
                            .OrderBy(o => o.No)
                            .ToDictionary(
                                o => o.ColumnName,
                                o => o.LabelText),
                        selectedValue: ss.DefaultImportKey)
                    .FieldCheckBox(
                        controlId: "ReplaceAllGroupMembers",
                        fieldCss: "field-wide",
                        labelText: Displays.ReplaceAllGroupMembers(context: context),
                        _checked: false,
                        controlCss: " always-send",
                        _using: context.Controller == "groups")
                    .P(css: "message-dialog")
                    .Div(
                        css: "command-center",
                        action: () => hb
                            .Button(
                                text: Displays.Import(context: context),
                                controlId: "DoImport",
                                controlCss: "button-icon button-positive",
                                onClick: "$p.import($(this));",
                                icon: "ui-icon-arrowreturnthick-1-e",
                                action: "Import",
                                method: "post")
                            .Button(
                                text: Displays.Cancel(context: context),
                                controlCss: "button-icon button-neutral",
                                onClick: "$p.closeDialog($(this));",
                                icon: "ui-icon-cancel")));
        }
    }
}