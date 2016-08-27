using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlSeparates
    {
        public static HtmlBuilder SeparateSettingsDialog(this HtmlBuilder hb)
        {
            return hb.Div(attributes: new HtmlAttributes()
                .Id("SeparateSettingsDialog")
                .Class("dialog")
                .Title(Displays.SeparateSettings()));
        }

        public static HtmlBuilder SeparateSettings(
            this HtmlBuilder hb,
            string title,
            decimal workValue,
            Column column, Permissions.Types permissionType)
        {
            var max = Parameters.General.SeparateMax;
            var min = Parameters.General.SeparateMin;
            return hb.Div(id: "SeparateSettings", action: () => hb
                .FieldSpinner(
                    controlId: "SeparateNumber",
                    fieldCss: "field-auto",
                    controlCss: " must-transport",
                    labelText: Displays.SeparateNumber(),
                    value: min,
                    min: min,
                    max: max,
                    step: 1)
                .FieldCheckBox(
                    controlId: "SeparateCopyWithComments",
                    fieldCss: "field-auto-thin",
                    controlCss: " must-transport",
                    labelText: Displays.CopyWithComments(),
                    _checked: true)
                .Div(css: "item both", action: () => hb
                    .FieldTextBox(
                        controlId: "SeparateTitle_1",
                        fieldCss: " w500",
                        controlCss: " must-transport",
                        labelText: Displays.Title() + "-1",
                        text: title + "-1")
                    .FieldText(
                        controlId: "SourceWorkValue",
                        fieldCss: "field-auto-thin",
                        controlCss: " w100",
                        labelText: Displays.WorkValue() + "-1",
                        text: workValue.ToControl(column, permissionType),
                        dataValue: workValue.ToString())
                    .Hidden(
                        controlId: "WorkValueUnit",
                        value: column.Unit))
                .Items(
                    title: title,
                    workValue: workValue,
                    unit: column.Unit,
                    max: max,
                    min: min)
                .P(css: "message-dialog")
                .Div(css: "command-center", action: () => hb
                    .Button(
                        controlId: "Separate",
                        text: Displays.Separate(),
                        controlCss: "button-separate",
                        onClick: "$p.send($(this));",
                        action: "Separate",
                        method: "put",
                        confirm: "Displays_ConfirmSeparate")
                    .Button(
                        text: Displays.Cancel(),
                        controlCss: "button-cancel",
                        onClick: "$p.closeDialog($(this));")));
        }

        private static HtmlBuilder Items(
            this HtmlBuilder hb,
            string title,
            decimal workValue,
            string unit,
            int max,
            int min)
        {
            for (var number = min; number <= max; number++)
            {
                hb.Item(
                    title: title,
                    workValue: workValue,
                    unit: unit,
                    number: number,
                    hide: number > min);
            }
            return hb;
        }

        private static HtmlBuilder Item(
            this HtmlBuilder hb,
            string title,
            decimal workValue,
            string unit,
            int number,
            bool hide)
        {
            return hb.Div(
                css: "item both" + (hide
                    ? " hidden"
                    : string.Empty),
                action: () => hb
                    .FieldTextBox(
                        controlId: "SeparateTitle_" + number,
                        fieldCss: " both w500",
                        controlCss: " must-transport",
                        labelText: Displays.Title() + "-" + number,
                        text: title + "-" + number)
                    .FieldSpinner(
                        controlId: "SeparateWorkValue_" + number,
                        fieldCss: "field-auto-thin",
                        controlCss: " must-transport",
                        labelText: Displays.WorkValue() + "-" + number,
                        value: 0,
                        min: 0,
                        max: workValue,
                        step: 0.1.ToDecimal(),
                        unit: unit));
        }
    }
}