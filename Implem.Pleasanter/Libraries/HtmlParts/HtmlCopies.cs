using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlCopies
    {
        public static HtmlBuilder CopyDialog(
            this HtmlBuilder hb, Context context, string referenceType, long id)
        {
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Id("CopyDialog")
                    .Class("dialog")
                    .Title(Displays.CopySettings(context: context)),
                action: () => hb
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id("CopyDialogForm")
                            .Action(Locations.Action(referenceType, id)),
                        action: () => hb
                            .FieldCheckBox(
                                controlId: "CopyWithComments",
                                labelText: Displays.CopyWithComments(context: context),
                                _checked: true,
                                fieldCss: "field-wide",
                                controlContainerCss: "m-l50",
                                controlCss: " always-send",
                                labelPositionIsRight: true)
                            .P(css: "message-dialog")
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    text: Displays.Copy(context: context),
                                    controlCss: "button-icon",
                                    onClick: "$p.copy($(this));",
                                    icon: "ui-icon-copy",
                                    action: "Copy",
                                    method: "post")
                                .Button(
                                    text: Displays.Cancel(context: context),
                                    controlCss: "button-icon",
                                    onClick: "$p.closeDialog($(this));",
                                    icon: "ui-icon-cancel"))));
        }
    }
}