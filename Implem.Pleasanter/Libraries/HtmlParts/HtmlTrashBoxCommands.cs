using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlTrashBoxCommands
    {
        public static HtmlBuilder TrashBoxCommands(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.Div(
                css: "command-left",
                action: () => hb
                    .Button(
                        text: Displays.Restore(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.send($(this));",
                        icon: "ui-icon-arrowreturnthick-1-n",
                        action: "Restore",
                        method: "post",
                        confirm: "ConfirmRestore",
                        _using: Parameters.Deleted.Restore)
                    .Button(
                        text: Displays.DeleteFromTrashBox(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.send($(this));",
                        icon: "ui-icon-closethick",
                        action: "PhysicalDelete",
                        method: "delete",
                        confirm: "ConfirmPhysicalDelete",
                        _using: Parameters.Deleted.PhysicalDelete),
                _using: (Parameters.Deleted.Restore || Parameters.Deleted.PhysicalDelete)
                    && context.Controller == "items"
                    && context.CanManageSite(ss: ss));
        }
    }
}