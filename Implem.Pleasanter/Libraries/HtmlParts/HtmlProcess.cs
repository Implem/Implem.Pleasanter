using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlProcess
    {
        public static HtmlBuilder ProcessCommands(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss)
        {
            ss.Processes
                ?.Where(process => process.Accessable(context: context))
                .Where(process => process.MatchConditions)
                .Where(process => (context.IsNew && process.ScreenType == Process.ScreenTypes.New)
                    || (!context.IsNew && process.ScreenType != Process.ScreenTypes.New))
                .ForEach(process =>
                    hb.Button(
                        controlId: $"Process_{process.Id}",
                        text: Strings.CoalesceEmpty(
                            process.DisplayName,
                            process.Name),
                        title: process.Tooltip,
                        onClick: !process.OnClick.IsNullOrEmpty()
                            ? process.OnClick
                            : "$p.execProcess($(this));",
                        controlCss: "button-icon validate",
                        icon: "ui-icon-disk",
                        validations: process.ValidateInputs?.ToJson() ?? "[]",
                        action: Action(
                            context: context,
                            process: process),
                        method: Method(
                            context: context,
                            process: process),
                        confirm: process.ConfirmationMessage));
            return hb;
        }

        private static string Action(Context context, Process process)
        {
            switch (process.ActionType ?? Process.ActionTypes.Save)
            {
                case Process.ActionTypes.Save:
                    return context.IsNew
                        ? "Create"
                        : "Update";
                case Process.ActionTypes.PostBack:
                    return "Edit";
                default:
                    return string.Empty;
            }
        }

        private static string Method(Context context, Process process)
        {
            switch (process.ActionType ?? Process.ActionTypes.Save)
            {
                case Process.ActionTypes.Save:
                    return context.IsNew
                        ? "post"
                        : "put";
                case Process.ActionTypes.PostBack:
                    return "post";
                default:
                    return string.Empty;
            }
        }
    }
}