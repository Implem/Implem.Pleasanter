using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Analysis;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.Utilities;
using System.Collections.Generic;
using System.Data;
namespace Implem.Pleasanter.Libraries.Views
{
    public static class HtmlGantts
    {
        public static HtmlBuilder Gantt(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            IEnumerable<DataRow> dataRows,
            string unit)
        {
            var gantt = new Gantt(siteSettings, dataRows);
            return hb
                .Graph(gantt: gantt)
                .MainCommands(
                    siteId: siteSettings.SiteId,
                    permissionType: permissionType,
                    verType: Versions.VerTypes.Latest,
                    backUrl: Navigations.ItemIndex(siteSettings.ParentId),
                    importButton: true,
                    exportButton: true);
        }

        private static HtmlBuilder Graph(this HtmlBuilder hb, Gantt gantt)
        {
            return hb.Div(css: "gantt-graph", action: () => hb
                .Svg(
                    attributes: Html.Attributes()
                        .Id_Css("Gantt", "gantt")
                        .Height(gantt.Height.ToInt()))
                .Svg(
                    attributes: Html.Attributes()
                        .Id_Css("GanttAxis", "gantt-axis"))
                .Hidden(
                    controlId: "GanttJson",
                    value: gantt.GanttGraphJson()));
        }
    }
}