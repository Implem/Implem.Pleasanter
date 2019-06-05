using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlHistoryCommands
    {
        public static HtmlBuilder HistoryCommands(
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
                        action: "RestoreFromHistory",
                        method: "post",
                        confirm: "ConfirmRestore",
                        _using: Parameters.History.Restore
                            && context.CanUpdate(ss: ss))
                    .Button(
                        text: Displays.DeleteHistory(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.send($(this));",
                        icon: "ui-icon-closethick",
                        action: "DeleteHistory",
                        method: "delete",
                        confirm: "ConfirmPhysicalDelete",
                        _using: Parameters.History.PhysicalDelete
                            && context.CanManageSite(ss: ss)),
                _using: (Parameters.History.Restore || Parameters.History.PhysicalDelete)
                    && context.Controller == "items"
                    && (context.CanUpdate(ss: ss) || context.CanManageSite(ss: ss))
                    && !ss.Locked());
        }
    }
}