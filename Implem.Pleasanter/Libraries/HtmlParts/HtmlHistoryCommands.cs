using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlHistoryCommands
    {
        public static HtmlBuilder HistoryCommands(this HtmlBuilder hb, SiteSettings ss)
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
                        confirm: "ConfirmRestore",
                        _using: Parameters.History.Restore && ss.CanUpdate())
                    .Button(
                        text: Displays.DeleteHistory(),
                        controlCss: "button-icon",
                        onClick: "$p.send($(this));",
                        icon: "ui-icon-closethick",
                        action: "DeleteHistory",
                        method: "delete",
                        confirm: "ConfirmPhysicalDelete",
                        _using: Parameters.History.PhysicalDelete && ss.CanManageSite()),
                _using: (Parameters.History.Restore || Parameters.History.PhysicalDelete)
                    && ss.Context.Controller == "items"
                    && (ss.CanUpdate() || ss.CanManageSite()));
        }
    }
}