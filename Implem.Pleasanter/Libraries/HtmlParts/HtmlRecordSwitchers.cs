using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Responses;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlRecordSwitchers
    {
        public static HtmlBuilder RecordSwitchers(this HtmlBuilder hb, bool switcher = true)
        {
            return hb
                .Switcher(switcher: switcher)
                .Button(
                    controlId: "Reload",
                    text: Displays.Reload(),
                    controlCss: "button-icon",
                    onClick: "$p.get($(this));",
                    icon: "ui-icon-refresh",
                    action: "Edit",
                    method: "post");
        }

        private static HtmlBuilder Switcher(this HtmlBuilder hb, bool switcher)
        {
            return switcher
                ? hb
                    .Button(
                        controlId: "Previous",
                        text: Displays.Previous(),
                        controlCss: "button-icon",
                        accessKey: "b",
                        onClick: "$p.get($(this));",
                        icon: "ui-icon-seek-prev",
                        action: "Edit",
                        method: "post")
                    .Div(id: "CurrentIndex", css: "current ui-widget-header")
                    .Button(
                        controlId: "Next",
                        text: Displays.Next(),
                        controlCss: "button-icon",
                        accessKey: "n",
                        onClick: "$p.get($(this));",
                        icon: "ui-icon-seek-next",
                        action: "Edit",
                        method: "post")
                : hb;
        }
    }
}