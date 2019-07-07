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
            return ss.GridEditorType == SiteSettings.GridEditorTypes.Dialog
                ? hb.Div(
                    attributes: new HtmlAttributes()
                        .Id("EditorDialog")
                        .Class("dialog")
                        .Title(Displays.Edit(context: context)),
                    action: () => hb
                        .Div(id: "EditInDialogBody")
                        .Hidden(
                            controlId: "TriggerRelatingColumns",
                            value: Implem.Libraries.Utilities.Jsons.ToJson(ss.RelatingColumns)))
                : hb;
        }

        public static HtmlBuilder DialogEditorForm(
            this HtmlBuilder hb, Context context, SiteSettings ss, long siteId, long referenceId, bool isHistory, Action action)
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
                        .P(css: "message-dialog",action: ()=> hb
                            .Notes(
                                context:context,
                                ss:ss,
                                verType: isHistory? Models.Versions.VerTypes.History: Models.Versions.VerTypes.Latest))
                        .Div(css: "command-center", action: () => hb
                            .Button(
                                text: Displays.Update(context: context),
                                controlCss: "button-icon validate",
                                accessKey: "s",
                                onClick: "$p.send($(this));",
                                icon: "ui-icon-copy",
                                action: "Update",
                                method: "put",
                                _using: !isHistory)
                            .Button(
                                text: Displays.Cancel(context: context),
                                controlCss: "button-icon",
                                onClick: "$p.closeDialog($(this));",
                                icon: "ui-icon-cancel"));
                });
        }
    }
}