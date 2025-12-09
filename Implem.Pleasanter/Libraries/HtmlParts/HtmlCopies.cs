using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using static Implem.DefinitionAccessor.Parameters;
using static Implem.ParameterAccessor.Parts.Types;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlCopies
    {
        public static HtmlBuilder CopyDialog(
            this HtmlBuilder hb,
            Context context,
            Settings.SiteSettings ss)
        {
            if (context.IsForm) return hb;

            return hb.Div(
                attributes: new HtmlAttributes()
                    .Id("CopyDialog")
                    .Class("dialog")
                    .Title(Displays.CopySettings(context: context)),
                action: () => hb
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id("CopyDialogForm")
                            .Action(Responses.Locations.Action(
                                context: context,
                                controller: context.Controller,
                                id: context.Id)),
                        action: () => hb
                            .FieldCheckBox(
                                controlId: "CopyWithComments",
                                labelText: Displays.CopyWithComments(context: context),
                                _checked: true,
                                fieldCss: "field-wide",
                                controlContainerCss: "m-l50",
                                controlCss: " always-send",
                                labelPositionIsRight: true)
                            .FieldCheckBox(
                                controlId: "CopyWithNotifications",
                                labelText: Displays.CopyWithNotifications(context: context),
                                _checked: Notification.CopyWithNotifications == OptionTypes.On,
                                fieldCss: "field-wide",
                                controlContainerCss: "m-l50",
                                controlCss: " always-send",
                                labelPositionIsRight: true,
                                _using: TableSettings(
                                    context: context,
                                    ss: ss)
                                        && Notification.CopyWithNotifications != OptionTypes.Disabled)
                            .FieldCheckBox(
                                controlId: "CopyWithReminders",
                                labelText: Displays.CopyWithReminders(context: context),
                                _checked: Reminder.CopyWithReminders == OptionTypes.On,
                                fieldCss: "field-wide",
                                controlContainerCss: "m-l50",
                                controlCss: " always-send",
                                labelPositionIsRight: true,
                                _using: TableSettings(
                                    context: context,
                                    ss: ss)
                                        && Reminder.CopyWithReminders != OptionTypes.Disabled)
                            .P(css: "message-dialog")
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    controlId: "CopyCommand",
                                    text: Displays.Copy(context: context),
                                    controlCss: "button-icon button-positive",
                                    onClick: "$p.copy($(this));",
                                    icon: "ui-icon-copy",
                                    action: "Copy",
                                    method: "post")
                                .Button(
                                    text: Displays.Cancel(context: context),
                                    controlCss: "button-icon button-neutral",
                                    onClick: "$p.closeDialog($(this));",
                                    icon: "ui-icon-cancel"))));
        }

        private static bool TableSettings(Context context, Settings.SiteSettings ss)
        {
            if (ss.IsSite(context: context))
            {
                switch (ss.ReferenceType)
                {
                    case "Issues":
                    case "Results":
                        return true;
                }
            }
            return false;
        }
    }
}