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
                        action: "Update",
                        method: "put",
                        confirm: process.ConfirmationMessage));
            return hb;
        }
    }
}