using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlDateRange
    {
        public static HtmlBuilder SetDateRangeDialog(
            this HtmlBuilder hb, Context context, SiteSettings ss, Column column, bool itemfilter = false)
        {
            var satartval = "";
            var endval = "";
            if (itemfilter)
            {
                var textval = context.Forms.Data(context.Forms.ControlId())?.Split('-');
                satartval = textval?[0]?.ToString().Trim();
                endval = string.Empty;
                if (textval.Length > 1)
                {
                    endval = textval[1].ToString().Trim();
                }
            }
            return
                hb.Form(
                    attributes: new HtmlAttributes()
                        .Id("dateRangeForm")
                        .Action(Locations.ItemAction(
                            context: context,
                            id: ss.SiteId)),
                    action: () => hb
                        .FieldTextBox(
                            textType: HtmlTypes.TextTypes.DateTime,
                            fieldId: "dateRangeStartFieldField",
                            controlId: "dateRangeStart",
                            fieldDescription: column.Description,
                            labelText: Displays.Start(context: context),
                            labelRequired: false,
                            controlOnly: false,
                            text: satartval,
                            format: column.DateTimepicker() ? "Y/m/d H:i" : "Y/m/d",
                            timepiker: column.DateTimepicker(),
                            alwaysSend: false,
                            validateRequired: false,
                            validateNumber: column.ValidateNumber ?? false,
                            validateDate: column.ValidateDate ?? false,
                            validateEmail: column.ValidateEmail ?? false,
                            validateEqualTo: column.ValidateEqualTo,
                            validateMaxLength: column.ValidateMaxLength ?? 0)
                        .FieldTextBox(
                            textType: HtmlTypes.TextTypes.DateTime,
                            fieldId: "dateRangeEndField",
                            controlId: "dateRangeEnd",
                            fieldDescription: column.Description,
                            labelText: Displays.End(context: context),
                            labelRequired: false,
                            controlOnly: false,
                            text: endval,
                            format: column.DateTimepicker() ? "Y/m/d H:i" : "Y/m/d",
                            timepiker: column.DateTimepicker(),
                            alwaysSend: false,
                            validateRequired: false,
                            validateNumber: column.ValidateNumber ?? false,
                            validateDate: column.ValidateDate ?? false,
                            validateEmail: column.ValidateEmail ?? false,
                            validateEqualTo: column.ValidateEqualTo,
                            validateMaxLength: column.ValidateMaxLength ?? 0)
                        .P(css: "message-dialog")
                        .Div(
                            css: "command-center",
                            action: () => hb
                                .Button(
                                    text: Displays.OK(context: context),
                                    controlId: "dateRangeOK",
                                    controlCss: "button-icon validate",
                                    onClick: "$p.openSetDateRangeOK('" + context.Forms.ControlId() + "'," + column.DateTimepicker().ToString().ToLower() + ");",
                                    icon: "ui-icon-arrowreturnthick-1-e",
                                    method: "post")
                                .Button(
                                    text: Displays.Cancel(context: context),
                                    controlId: "dateRangeCancel",
                                    controlCss: "button-icon",
                                    onClick: "$p.closeDialog($(this));",
                                    icon: "ui-icon-cancel")
                                .Button(
                                    text: Displays.Clear(context: context),
                                    controlId: "dateRangeClear",
                                    controlCss: "button-icon",
                                    onClick: "$p.openSetDateRangeClear($(this));",
                                    icon: "ui-icon-cancel")));
        }
    }
}