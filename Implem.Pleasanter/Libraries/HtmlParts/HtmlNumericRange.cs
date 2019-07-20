using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlNumericRange
    {
        public static HtmlBuilder SetNumericRangeDialog(
            this HtmlBuilder hb, Context context, SiteSettings ss, Column column)
        {
            var textval = context.Forms.Data(context.Forms.ControlId())?.Split(' ');
            var satartval = textval?[0]?.ToString().Trim();
            var endval = string.Empty;
            if (textval.Length > 2)
            {
                endval = textval[2].ToString().Trim();
            }
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("numericRangeForm")
                    .Action(Locations.ItemAction(
                        context: context,
                        id: ss.SiteId)),
                action: () => hb
                    .FieldTextBox(
                        textType: HtmlTypes.TextTypes.Normal,
                        fieldId: "numericRangeStartField",
                        controlId: "numericRangeStart",
                        fieldDescription: column.Description,
                        labelText: Displays.Start(context: context),
                        labelRequired: false,
                        controlOnly: false,
                        unit: column.Unit,
                        text: satartval,
                        alwaysSend: false,
                        validateRequired: false,
                        validateNumber: column.ValidateNumber ?? false,
                        validateMinNumber: column.MinNumber(),
                        validateMaxNumber: column.MaxNumber(),
                        validateMaxLength: column.ValidateMaxLength ?? 0)
                    .FieldTextBox(
                        textType: HtmlTypes.TextTypes.Normal,
                        fieldId: "numericRangeEndField",
                        controlId: "numericRangeEnd",
                        fieldDescription: column.Description,
                        labelText: Displays.End(context: context),
                        labelRequired: false,
                        controlOnly: false,
                        unit: column.Unit,
                        text: endval,
                        alwaysSend: false,
                        validateRequired: false,
                        validateNumber: column.ValidateNumber ?? false,
                        validateMinNumber: column.MinNumber(),
                        validateMaxNumber: column.MaxNumber(),
                        validateMaxLength: column.ValidateMaxLength ?? 0)
                    .P(css: "message-dialog")
                    .Div(
                        css: "command-center",
                        action: () => hb
                            .Button(
                                text: Displays.OK(context: context),
                                controlCss: "button-icon validate",
                                onClick: "$p.openSetNumericRangeOK('" + context.Forms.ControlId() + "');",
                                icon: "ui-icon-arrowreturnthick-1-e",
                                method: "post")
                            .Button(
                                text: Displays.Cancel(context: context),
                                controlCss: "button-icon",
                                onClick: "$p.closeDialog($(this));",
                                icon: "ui-icon-cancel")
                            .Button(
                                text: Displays.Clear(context: context),
                                controlCss: "button-icon",
                                onClick: "$p.openSetNumericRangeClear($(this));",
                                icon: "ui-icon-cancel"))
                );
        }
    }
}