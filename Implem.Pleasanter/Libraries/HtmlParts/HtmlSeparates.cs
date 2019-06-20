using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlSeparates
    {
        public static HtmlBuilder SeparateSettingsDialog(this HtmlBuilder hb, Context context)
        {
            return hb.Div(attributes: new HtmlAttributes()
                .Id("SeparateSettingsDialog")
                .Class("dialog")
                .Title(Displays.SeparateSettings(context: context)));
        }

        public static HtmlBuilder SeparateSettings(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            string title,
            decimal workValue,
            List<string> mine)
        {
            var max = Parameters.General.SeparateMax;
            var min = Parameters.General.SeparateMin;
            var column = ss.GetColumn(
                context: context,
                columnName: "WorkValue");
            column = new Column()
            {
                EditorReadOnly = true,
                Unit = column.Unit
            };
            return hb.Div(id: "SeparateSettings", action: () => hb
                .FieldSpinner(
                    controlId: "SeparateNumber",
                    fieldCss: "field-auto",
                    controlCss: " always-send",
                    labelText: Displays.SeparateNumber(context: context),
                    value: min,
                    min: min,
                    max: max,
                    step: 1)
                .FieldCheckBox(
                    controlId: "SeparateCopyWithComments",
                    fieldCss: "field-auto-thin",
                    controlCss: " always-send",
                    labelText: Displays.CopyWithComments(context: context),
                    _checked: true)
                .Div(css: "item both", action: () => hb
                    .FieldTextBox(
                        controlId: "SeparateTitle_1",
                        fieldCss: " w500",
                        controlCss: " always-send",
                        labelText: Displays.Title(context: context) + "-1",
                        text: title + "-1")
                    .FieldText(
                        controlId: "SourceWorkValue",
                        fieldCss: "field-auto-thin",
                        controlCss: " w100",
                        labelText: Displays.WorkValue(context: context) + "-1",
                        text: workValue.ToControl(
                            context: context,
                            ss: ss,
                            column: column),
                        dataValue: workValue.ToString())
                    .Hidden(
                        controlId: "WorkValueUnit",
                        value: column.Unit))
                .Items(
                    context: context,
                    title: title,
                    workValue: workValue,
                    unit: column.Unit,
                    max: max,
                    min: min)
                .P(css: "message-dialog")
                .Div(css: "command-center", action: () => hb
                    .Button(
                        controlId: "Separate",
                        text: Displays.Separate(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.send($(this));",
                        icon: "ui-icon-extlink",
                        action: "Separate",
                        method: "put",
                        confirm: "ConfirmSeparate")
                    .Button(
                        text: Displays.Cancel(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.closeDialog($(this));",
                        icon: "ui-icon-cancel")));
        }

        private static HtmlBuilder Items(
            this HtmlBuilder hb,
            Context context,
            string title,
            decimal workValue,
            string unit,
            int max,
            int min)
        {
            for (var number = min; number <= max; number++)
            {
                hb.Item(
                    context: context,
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
            Context context,
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
                        controlCss: " always-send",
                        labelText: Displays.Title(context: context) + "-" + number,
                        text: title + "-" + number)
                    .FieldSpinner(
                        controlId: "SeparateWorkValue_" + number,
                        fieldCss: "field-auto-thin",
                        controlCss: " always-send",
                        labelText: Displays.WorkValue(context: context) + "-" + number,
                        value: 0,
                        min: 0,
                        max: workValue,
                        step: 0.1.ToDecimal(),
                        unit: unit));
        }
    }
}