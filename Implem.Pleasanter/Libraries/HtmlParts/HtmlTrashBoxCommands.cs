using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Responses;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlTrashBoxCommands
    {
        public static HtmlBuilder TrashBoxCommands(this HtmlBuilder hb)
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
                        confirm: "ConfirmRestore")
                    .Button(
                        text: Displays.DeleteFromTrashBox(),
                        controlCss: "button-icon",
                        onClick: "$p.send($(this));",
                        icon: "ui-icon-arrowreturnthick-1-n",
                        action: "PhysicalDelete",
                        method: "delete",
                        confirm: "ConfirmPhysicalDelete"));
        }
    }
}