using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlRecordSwitchers
    {
        public static HtmlBuilder RecordSwitchers(
            this HtmlBuilder hb, Context context, SiteSettings ss, bool switcher = true)
        {
            var onClick = $"$p.get($(this),{ss.SwitchRecordWithAjax.ToBool().ToString().ToLower()});";
            return hb
                .Switcher(
                    context: context,
                    onClick: onClick,
                    switcher: switcher)
                .Button(
                    controlId: "Reload",
                    text: Displays.Reload(context: context),
                    controlCss: "button-icon confirm-unload",
                    onClick: context.Controller == "tenants"
                        ? "location.reload();"
                        : onClick,
                    icon: "ui-icon-refresh",
                    action: "Edit",
                    method: "post");
        }

        private static HtmlBuilder Switcher(
            this HtmlBuilder hb, Context context, string onClick, bool switcher)
        {
            return switcher
                ? hb
                    .Button(
                        controlId: "Previous",
                        text: Displays.Previous(context: context),
                        controlCss: "button-icon confirm-unload",
                        accessKey: "b",
                        onClick: onClick,
                        icon: "ui-icon-seek-prev",
                        action: "Edit",
                        method: "post")
                    .Div(id: "CurrentIndex", css: "current ui-widget-header")
                    .Button(
                        controlId: "Next",
                        text: Displays.Next(context: context),
                        controlCss: "button-icon confirm-unload",
                        accessKey: "n",
                        onClick: onClick,
                        icon: "ui-icon-seek-next",
                        action: "Edit",
                        method: "post")
                : hb;
        }
    }
}