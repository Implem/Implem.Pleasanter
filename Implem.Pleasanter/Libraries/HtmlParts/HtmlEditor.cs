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
                        .Title(Displays.Edit(context: context)),
                    action: () => hb
                        .Div(id: "EditInDialogBody"))

                : hb;
        }

        public static HtmlBuilder DialogEditorForm(
            this HtmlBuilder hb, Context context, long siteId, long referenceId, Action action)
        {
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("DialogEditorForm")
                    .Action(Locations.ItemAction(
                        context: context,
                        id: referenceId != 0
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
                        .Hidden(
                            controlId: "EditorLoading",
                            value: "1")
                        .P(css: "message-dialog")
                        .Div(css: "command-center", action: () => hb
                            .Button(
                                text: Displays.Update(context: context),
                                controlCss: "button-icon validate",
                                accessKey: "s",
                                onClick: "$p.send($(this));",
                                icon: "ui-icon-copy",
                                action: "Update",
                                method: "put")
                            .Button(
                                text: Displays.Cancel(context: context),
                                controlCss: "button-icon",
                                onClick: "$p.closeDialog($(this));",
                                icon: "ui-icon-cancel"));
                });
        }
    }
}