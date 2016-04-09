using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Responses;
namespace Implem.Pleasanter.Libraries.Views
{
    public static class HtmlSeparates
    {
        public static HtmlBuilder Dialog_SeparateSettings(this HtmlBuilder hb)
        {
            return hb.Div(attributes: Html.Attributes()
                .Id_Css("Dialog_SeparateSettings", "dialog")
                .Title(Displays.SeparateSettings()));
        }

        public static HtmlBuilder SeparateSettings(
            this HtmlBuilder hb, string title, decimal workValue, string unit)
        {
            var max = Def.Parameters.SeparateMax;
            var min = Def.Parameters.SeparateMin;
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
                        text: workValue.ToString(),
                        dataValue: workValue.ToString(),
                        unit: unit))
                .Items(
                    title: title,
                    workValue: workValue,
                    unit: unit,
                    max: max,
                    min: min)
                .P(css: "message-dialog")
                .Div(css: "command-center", action: () => hb
                    .Button(
                        controlId: "Separate",
                        text: Displays.Separate(),
                        controlCss: "button-separate",
                        onClick: Def.JavaScript.Submit,
                        action: "Separate",
                        method: "put",
                        confirm: "Displays_ConfirmSeparate")
                    .Button(
                        text: Displays.Cancel(),
                        controlCss: "button-cancel",
                        onClick: Def.JavaScript.CancelDialog)));
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