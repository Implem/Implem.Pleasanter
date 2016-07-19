using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataViews;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlGantts
    {
        public static HtmlBuilder Gantt(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            string groupByColumn,
            Permissions.Types permissionType,
            IEnumerable<DataRow> dataRows)
        {
            return hb.Div(css: "both", action: () => hb
                .FieldDropDown(
                    controlId: "TimeSeriesGroupByColumn",
                    fieldCss: "field-auto-thin hidden",
                    controlCss: " auto-postback",
                    labelText: Displays.GroupBy(),
                    optionCollection: siteSettings.ColumnCollection.Where(o => o.HasChoices())
                        .ToDictionary(o => o.ColumnName, o => o.LabelText),
                    selectedValue: groupByColumn,
                    action: "DataView",
                    method: "post")
                .Div(id: "GanttChart", action: () => hb
                    .GanttChart(
                        siteSettings: siteSettings,
                        groupByColumn: groupByColumn,
                        dataRows: dataRows))
                .MainCommands(
                    siteId: siteSettings.SiteId,
                    permissionType: permissionType,
                    verType: Versions.VerTypes.Latest,
                    backUrl: Navigations.ItemIndex(siteSettings.ParentId),
                    importButton: true,
                    exportButton: true));
        }

        public static HtmlBuilder GanttChart(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            string groupByColumn,
            IEnumerable<DataRow> dataRows)
        {
            var gantt = new Gantt(siteSettings, dataRows);
            return hb
                .Svg(attributes: new HtmlAttributes()
                    .Id_Css("Gantt", "gantt")
                    .Height(gantt.Height.ToInt()))
                .Svg(attributes: new HtmlAttributes()
                    .Id_Css("GanttAxis", "gantt-axis"))
                .Hidden(
                    controlId: "GanttJson",
                    value: gantt.ChartJson());
        }
    }
}