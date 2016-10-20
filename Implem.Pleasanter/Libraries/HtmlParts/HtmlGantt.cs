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
            SiteSettings ss,
            string groupByColumn,
            Permissions.Types permissionType,
            IEnumerable<DataRow> dataRows)
        {
            return hb.Div(css: "both", action: () => hb
                .FieldDropDown(
                    controlId: "GanttGroupByColumn",
                    fieldCss: "field-auto-thin",
                    controlCss: " auto-postback",
                    labelText: Displays.GroupBy(),
                    optionCollection: ss.ColumnCollection
                        .Where(o => o.HasChoices())
                        .ToDictionary(o => o.ColumnName, o => o.LabelText),
                    selectedValue: groupByColumn,
                    insertBlank: true,
                    method: "post")
                .Div(id: "GanttBody", action: () => hb
                    .GanttBody(
                        ss: ss,
                        groupByColumn: groupByColumn,
                        dataRows: dataRows))
                .MainCommands(
                    siteId: ss.SiteId,
                    permissionType: permissionType,
                    verType: Versions.VerTypes.Latest,
                    importButton: true,
                    exportButton: true));
        }

        public static HtmlBuilder GanttBody(
            this HtmlBuilder hb,
            SiteSettings ss,
            string groupByColumn,
            IEnumerable<DataRow> dataRows)
        {
            var gantt = new Gantt(ss, dataRows, groupByColumn);
            return hb
                .Svg(id: "Gantt")
                .Svg(id: "GanttAxis")
                .Hidden(
                    controlId: "GanttJson",
                    value: gantt.Json());
        }
    }
}