using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlTrashBoxCommands
    {
        public static HtmlBuilder TrashBoxCommands(this HtmlBuilder hb, SiteSettings ss)
        {
            return hb.Div(
                css: "command-left",
                action: () => hb
                    .Button(
                        text: Displays.Restore(),
                        controlCss: "button-icon",
                        onClick: "$p.send($(this));",
                        icon: "ui-icon-arrowreturnthick-1-n",
                        action: "Restore",
                        method: "post",
                        confirm: "ConfirmRestore",
                        _using: Parameters.Deleted.Restore)
                    .Button(
                        text: Displays.DeleteFromTrashBox(),
                        controlCss: "button-icon",
                        onClick: "$p.send($(this));",
                        icon: "ui-icon-arrowreturnthick-1-n",
                        action: "PhysicalDelete",
                        method: "delete",
                        confirm: "ConfirmPhysicalDelete",
                        _using: Parameters.Deleted.PhysicalDelete),
                _using: (Parameters.Deleted.Restore || Parameters.Deleted.PhysicalDelete)
                    && ss.Context.Controller == "items"
                    && ss.CanManageSite());
        }
    }
}