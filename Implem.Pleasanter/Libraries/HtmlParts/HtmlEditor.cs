using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using System;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlEditor
    {
        public static HtmlBuilder EditorDialog(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return ss.EditInDialog == true
                ? hb.Div(
                    attributes: new HtmlAttributes()
                        .Id("EditorDialog")
                        .Class("dialog")
                        .Title(Displays.Edit()),
                    action: () => hb
                        .Div(id: "EditInDialogBody")
                        .P(css: "message-dialog"))

                : hb;
        }

        public static HtmlBuilder DialogEditorForm(
            this HtmlBuilder hb, long siteId, long referenceId, Action action)
        {
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("DialogEditorForm")
                    .Action(Locations.ItemAction(referenceId != 0
                        ? referenceId
                        : siteId)),
                action: () =>
                {
                    action();
                    hb
                        .Hidden(
                            controlId: "IsDialogEditorForm",
                            css: "always-send",
                            value: "1")
                        .Div(css: "command-center", action: () => hb
                            .Button(
                                text: Displays.Update(),
                                controlCss: "button-icon",
                                onClick: "$p.send($(this));",
                                icon: "ui-icon-copy",
                                action: "Update",
                                method: "put")
                            .Button(
                                text: Displays.Cancel(),
                                controlCss: "button-icon",
                                onClick: "$p.closeDialog($(this));",
                                icon: "ui-icon-cancel"));
                });
        }
    }
}