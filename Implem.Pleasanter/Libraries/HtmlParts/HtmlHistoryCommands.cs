using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Responses;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlHistoryCommands
    {
        public static HtmlBuilder HistoryCommands(this HtmlBuilder hb)
        {
            return hb.Div(
                css: "command-left",
                action: () => hb
                    .Button(
                        text: Displays.Restore(),
                        controlCss: "button-icon",
                        onClick: "$p.send($(this));",
                        icon: "ui-icon-arrowreturnthick-1-n",
                        action: "RestoreFromHistory",
                        method: "post",
                        confirm: "ConfirmRestore")
                    .Button(
                        text: Displays.DeleteHistory(),
                        controlCss: "button-icon",
                        onClick: "$p.send($(this));",
                        icon: "ui-icon-arrowreturnthick-1-n",
                        action: "DeleteHistory",
                        method: "delete",
                        confirm: "ConfirmPhysicalDelete"));
        }
    }
}