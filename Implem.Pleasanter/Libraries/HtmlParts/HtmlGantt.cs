using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.ViewModes;
using System.Collections.Generic;
using System.Data;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlGantts
    {
        public static HtmlBuilder Gantt(
            this HtmlBuilder hb,
            SiteSettings ss,
            string groupBy,
            string sortBy,
            IEnumerable<DataRow> dataRows)
        {
            return hb.Div(css: "both", action: () => hb
                .FieldDropDown(
                    controlId: "GanttGroupBy",
                    fieldCss: "field-auto-thin",
                    controlCss: " auto-postback",
                    labelText: Displays.GroupBy(),
                    optionCollection: ss.GanttGroupByOptions(),
                    selectedValue: groupBy,
                    insertBlank: true,
                    method: "post")
                .FieldDropDown(
                    controlId: "GanttSortBy",
                    fieldCss: "field-auto-thin",
                    controlCss: " auto-postback",
                    labelText: Displays.SortBy(),
                    optionCollection: ss.GanttSortOptions(),
                    selectedValue: sortBy,
                    insertBlank: true,
                    method: "post")
                .Div(id: "GanttBody", action: () => hb
                    .GanttBody(
                        ss: ss,
                        groupBy: groupBy,
                        sortBy: sortBy,
                        dataRows: dataRows)));
        }

        public static HtmlBuilder GanttBody(
            this HtmlBuilder hb,
            SiteSettings ss,
            string groupBy,
            string sortBy,
            IEnumerable<DataRow> dataRows)
        {
            return dataRows != null
                ? hb
                    .Svg(id: "Gantt")
                    .Svg(id: "GanttAxis")
                    .Hidden(
                        controlId: "GanttJson",
                        value: new Gantt(ss, dataRows, groupBy, sortBy).Json())
                : hb;
        }
    }
}