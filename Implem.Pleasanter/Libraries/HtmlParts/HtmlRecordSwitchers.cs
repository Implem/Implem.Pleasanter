using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Responses;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlRecordSwitchers
    {
        public static HtmlBuilder RecordSwitchers(
            this HtmlBuilder hb, long id, List<long> switchTargets, bool switcher = true)
        {
            return hb
                .Switcher(id: id, switchTargets: switchTargets, switcher: switcher)
                .Button(
                    text: Displays.Reload(),
                    controlCss: "button-icon",
                    onClick: "$p.send($(this));",
                    icon: "ui-icon-refresh",
                    action: "Reload",
                    method: "post");
        }

        private static HtmlBuilder Switcher(
            this HtmlBuilder hb, long id, List<long> switchTargets, bool switcher)
        {
            return switchTargets?.Count >= 2 && switcher
                ? hb
                    .Button(
                        text: Displays.Previous(),
                        controlCss: "button-icon",
                        accessKey: "b",
                        onClick: "$p.send($(this));",
                        icon: "ui-icon-seek-prev",
                        action: "Previous",
                        method: "post")
                    .Current(id: id, switchTargets: switchTargets)
                    .Button(
                        text: Displays.Next(),
                        controlCss: "button-icon",
                        accessKey: "n",
                        onClick: "$p.send($(this));",
                        icon: "ui-icon-seek-next",
                        action: "Next",
                        method: "post")
                : hb;
        }

        private static HtmlBuilder Current(
            this HtmlBuilder hb, long id, List<long> switchTargets)
        {
            return switchTargets != null
                ? hb.Div(css: "current ui-widget-header", action: () => hb
                    .Text(text: "{0}/{1}".Params(
                        switchTargets.IndexOf(id) + 1,
                        switchTargets.Count)))
                : hb;
        }
    }
}