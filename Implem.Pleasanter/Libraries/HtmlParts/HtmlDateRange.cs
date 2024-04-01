using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlDateRange
    {
        public static HtmlBuilder SetDateRangeDialog(
            this HtmlBuilder hb, Context context, SiteSettings ss, Column column, bool itemfilter = false)
        {
            var satartval = string.Empty;
            var endval = string.Empty;
            if (itemfilter)
            {
                var textval = context.Forms.Data(context.Forms.ControlId())?.Split('-');
                satartval = DateTimeString(textval?[0]);
                endval = string.Empty;
                if (textval.Length > 1)
                {
                    endval = DateTimeString(textval?[1]);
                }
            }
            return
                hb.Form(
                    attributes: new HtmlAttributes()
                        .Id("DateRangeForm")
                        .Action(Locations.ItemAction(
                            context: context,
                            id: ss.SiteId)),
                    action: () => hb
                        .FieldTextBox(
                            textType: HtmlTypes.TextTypes.DateTime,
                            fieldId: "DateRangeStartFieldField",
                            controlId: "DateRangeStart",
                            fieldDescription: column.Description,
                            labelText: Displays.Start(context: context),
                            controlOnly: false,
                            text: satartval,
                            format: column.DateTimeFormat(context: context),
                            timepiker: column.DateTimepicker(),
                            alwaysSend: false,
                            validateRequired: false,
                            validateNumber: column.ValidateNumber ?? false,
                            validateDate: column.ValidateDate ?? false,
                            validateEmail: column.ValidateEmail ?? false,
                            validateEqualTo: column.ValidateEqualTo,
                            validateMaxLength: column.ValidateMaxLength ?? 0,
                            attributes: column.DateTimeStep == null
                                ? null
                                : new Dictionary<string, string>() {
                                    { "data-step", column.DateTimeStep?.ToString() }
                                },
                            controlOption: () => hb
                                .Div(
                                    css: "ui-icon ui-icon-clock current-time",
                                    _using: !Parameters.General.HideCurrentTimeIcon))
                        .FieldTextBox(
                            textType: HtmlTypes.TextTypes.DateTime,
                            fieldId: "DateRangeEndField",
                            controlId: "DateRangeEnd",
                            fieldDescription: column.Description,
                            labelText: Displays.End(context: context),
                            controlOnly: false,
                            text: endval,
                            format: column.DateTimeFormat(context: context),
                            timepiker: column.DateTimepicker(),
                            alwaysSend: false,
                            validateRequired: false,
                            validateNumber: column.ValidateNumber ?? false,
                            validateDate: column.ValidateDate ?? false,
                            validateEmail: column.ValidateEmail ?? false,
                            validateEqualTo: column.ValidateEqualTo,
                            validateMaxLength: column.ValidateMaxLength ?? 0,
                            attributes: column.DateTimeStep == null
                                ? null
                                : new Dictionary<string, string>() {
                                    { "data-step", column.DateTimeStep?.ToString() }
                                },
                            controlOption: () => hb
                                .Div(
                                    css: "ui-icon ui-icon-clock current-time",
                                    _using: !Parameters.General.HideCurrentTimeIcon))
                        .P(css: "message-dialog")
                        .Div(
                            css: "command-center",
                            action: () => hb
                                .Button(
                                    text: Displays.OK(context: context),
                                    controlId: "DateRangeOK",
                                    controlCss: "button-icon validate button-positive",
                                    onClick: $"$p.openSetDateRangeOK('{context.Forms.ControlId()}','{(column.DateTimepicker() ? "DateTimepicker" : string.Empty)}');",
                                    icon: "ui-icon-arrowreturnthick-1-e",
                                    method: "post")
                                .Button(
                                    text: Displays.Cancel(context: context),
                                    controlId: "DateRangeCancel",
                                    controlCss: "button-icon button-neutral",
                                    onClick: $"$p.closeSiteSetDateRangeDialog('{context.Forms.ControlId()}')",
                                    icon: "ui-icon-cancel")
                                .Button(
                                    text: Displays.Clear(context: context),
                                    controlId: "DateRangeClear",
                                    controlCss: "button-icon",
                                    onClick: "$p.openSetDateRangeClear($(this));",
                                    icon: "ui-icon-cancel")
                                .Button(
                                    text: Displays.Today(context: context),
                                    controlId: "DateRangeToday",
                                    controlCss: "button-icon",
                                    onClick: $"$p.openSetDateRangeOK('{context.Forms.ControlId()}','Today');",
                                    icon: "ui-icon-clock")
                                .Button(
                                    text: Displays.ThisMonth(context: context),
                                    controlId: "DateRangeThisMonth",
                                    controlCss: "button-icon",
                                    onClick: $"$p.openSetDateRangeOK('{context.Forms.ControlId()}','ThisMonth');",
                                    icon: "ui-icon-clock")
                                .Button(
                                    text: Displays.ThisYear(context: context),
                                    controlId: "DateRangeThisYear",
                                    controlCss: "button-icon",
                                    onClick: $"$p.openSetDateRangeOK('{context.Forms.ControlId()}','ThisYear');",
                                    icon: "ui-icon-clock")));
        }

        private static string DateTimeString(string value)
        {
            return DateTime.TryParse(value, out _)
                ? value?.Trim() ?? string.Empty
                : string.Empty;
        }
    }
}