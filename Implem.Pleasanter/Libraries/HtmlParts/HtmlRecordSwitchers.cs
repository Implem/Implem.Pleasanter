using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlRecordSwitchers
    {
        public static HtmlBuilder RecordSwitchers(
            this HtmlBuilder hb, SiteSettings ss, bool switcher = true)
        {
            return hb
                .Switcher(ajax: !ss.Scripts.Any(o => o.Edit == true), switcher: switcher)
                .Button(
                    controlId: "Reload",
                    text: Displays.Reload(),
                    controlCss: "button-icon confirm-reload",
                    onClick: "$p.get($(this));",
                    icon: "ui-icon-refresh",
                    action: "Edit",
                    method: "post");
        }

        private static HtmlBuilder Switcher(this HtmlBuilder hb, bool ajax, bool switcher)
        {
            var onClick = $"$p.get($(this), {ajax.ToString().ToLower()});";
            return switcher
                ? hb
                    .Button(
                        controlId: "Previous",
                        text: Displays.Previous(),
                        controlCss: "button-icon confirm-reload",
                        accessKey: "b",
                        onClick: onClick,
                        icon: "ui-icon-seek-prev",
                        action: "Edit",
                        method: "post")
                    .Div(id: "CurrentIndex", css: "current ui-widget-header")
                    .Button(
                        controlId: "Next",
                        text: Displays.Next(),
                        controlCss: "button-icon confirm-reload",
                        accessKey: "n",
                        onClick: onClick,
                        icon: "ui-icon-seek-next",
                        action: "Edit",
                        method: "post")
                : hb;
        }
    }
}