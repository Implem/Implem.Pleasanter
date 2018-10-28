using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.ViewModes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlCrosstab
    {
        public static HtmlBuilder Crosstab(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
            Column groupByX,
            Column groupByY,
            List<Column> columns,
            string aggregateType,
            Column value,
            string timePeriod,
            DateTime month,
            EnumerableRowCollection<DataRow> dataRows,
            bool inRange = true)
        {
            return hb.Div(id: "Crosstab", css: "both", action: () =>
            {
                hb
                    .FieldDropDown(
                        context: context,
                        controlId: "CrosstabGroupByX",
                        fieldCss: "field-auto-thin",
                        controlCss: " auto-postback",
                        labelText: Displays.GroupByX(context: context),
                        optionCollection: ss.CrosstabGroupByXOptions(context: context),
                        selectedValue: view.CrosstabGroupByX,
                        method: "post")
                    .FieldDropDown(
                        context: context,
                        controlId: "CrosstabGroupByY",
                        fieldCss: "field-auto-thin",
                        controlCss: " auto-postback",
                        labelText: Displays.GroupByY(context: context),
                        optionCollection: ss.CrosstabGroupByYOptions(context: context),
                        selectedValue: view.CrosstabGroupByY,
                        method: "post")
                    .FieldDropDown(
                        context: context,
                        fieldId: "CrosstabColumnsField",
                        controlId: "CrosstabColumns",
                        fieldCss: "field-auto-thin",
                        controlCss: " auto-postback",
                        labelText: Displays.NumericColumn(context: context),
                        optionCollection: ss.CrosstabColumnsOptions(),
                        selectedValue: view.CrosstabColumns,
                        multiple: true,
                        method: "post")
                    .FieldDropDown(
                        context: context,
                        controlId: "CrosstabAggregateType",
                        fieldCss: "field-auto-thin",
                        controlCss: " auto-postback",
                        labelText: Displays.AggregationType(context: context),
                        optionCollection: ss.CrosstabAggregationTypeOptions(context: context),
                        selectedValue: aggregateType,
                        method: "post")
                    .FieldDropDown(
                        context: context,
                        fieldId: "CrosstabValueField",
                        controlId: "CrosstabValue",
                        fieldCss: "field-auto-thin",
                        controlCss: " auto-postback",
                        labelText: Displays.AggregationTarget(context: context),
                        optionCollection: ss.CrosstabColumnsOptions(),
                        selectedValue: view.CrosstabValue,
                        addSelectedValue: false,
                        method: "post")
                    .FieldDropDown(
                        context: context,
                        fieldId: "CrosstabTimePeriodField",
                        controlId: "CrosstabTimePeriod",
                        fieldCss: "field-auto-thin",
                        controlCss: " auto-postback",
                        labelText: Displays.Period(context: context),
                        optionCollection: ss.CrosstabTimePeriodOptions(context: context),
                        selectedValue: view.CrosstabTimePeriod,
                        method: "post")
                    .DropDown(
                        context: context,
                        controlId: "CrosstabMonth",
                        controlCss: " w100 auto-postback",
                        optionCollection: CrosstabMonth(context: context),
                        selectedValue: new DateTime(month.Year, month.Month, 1).ToString(),
                        action: "Crosstab",
                        method: "post")
                    .Button(
                        controlId: "CrosstabPreviousButton",
                        text: Displays.Previous(context: context),
                        controlCss: "button-icon",
                        accessKey: "b",
                        onClick: "$p.moveCrosstab('Previous');",
                        icon: "ui-icon-seek-prev")
                    .Button(
                        controlId: "CrosstabNextButton",
                        text: Displays.Next(context: context),
                        controlCss: "button-icon",
                        accessKey: "n",
                        onClick: "$p.moveCrosstab('Next');",
                        icon: "ui-icon-seek-next")
                    .Button(
                        controlId: "CrosstabThisMonthButton",
                        text: Displays.ThisMonth(context: context),
                        controlCss: "button-icon",
                        accessKey: "n",
                        onClick: "$p.moveCrosstab('ThisMonth');",
                        icon: "ui-icon-Crosstab")
                    .Div(id: "CrosstabBody", action: () =>
                    {
                        if (inRange)
                        {
                            hb.CrosstabBody(
                                context: context,
                                ss: ss,
                                view: view,
                                groupByX: groupByX,
                                groupByY: groupByY,
                                columns: columns,
                                aggregateType: aggregateType,
                                value: value,
                                timePeriod: timePeriod,
                                month: month,
                                dataRows: dataRows);
                        }
                    });
            });
        }

        private static Dictionary<string, ControlData> CrosstabMonth(Context context)
        {
            var now = DateTime.Now;
            var month = new DateTime(
                year: now.ToLocal(context: context).Year,
                month: now.ToLocal(context: context).Month,
                day: 1);
            return Enumerable.Range(
                Parameters.General.CrosstabBegin,
                Parameters.General.CrosstabEnd - Parameters.General.CrosstabBegin)
                    .ToDictionary(
                        o => month.AddMonths(o).ToString(),
                        o => new ControlData(month.AddMonths(o).ToString(
                            "Y", context.CultureInfo())));
        }

        public static HtmlBuilder CrosstabBody(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
            Column groupByX,
            Column groupByY,
            List<Column> columns,
            string aggregateType,
            Column value,
            string timePeriod,
            DateTime month,
            EnumerableRowCollection<DataRow> dataRows,
            bool inRange = true)
        {
            if (!inRange) return hb;
            if (groupByY != null)
            {
                hb.Table(
                    context: context,
                    ss: ss,
                    choicesX: CrosstabUtilities.ChoicesX(
                        context: context,
                        groupByX: groupByX,
                        view: view,
                        timePeriod: timePeriod,
                        month: month),
                    choicesY: CrosstabUtilities.ChoicesY(
                        context: context,
                        groupByY: groupByY,
                        view: view),
                    aggregateType: aggregateType,
                    value: value,
                    daily: Daily(
                        xColumn: groupByX,
                        timePeriod: timePeriod),
                    data: CrosstabUtilities.Elements(
                        groupByX: groupByX,
                        groupByY: groupByY,
                        dataRows: dataRows));
            }
            else
            {
                var columnList = CrosstabUtilities.GetColumns(
                    context: context,
                    ss: ss,
                    columns: columns);
                hb.Table(
                    context: context,
                    ss: ss,
                    choicesX: CrosstabUtilities.ChoicesX(
                        context: context,
                        groupByX: groupByX,
                        view: view,
                        timePeriod: timePeriod,
                        month: month),
                    choicesY: CrosstabUtilities.ChoicesY(
                        columnList: columnList),
                    aggregateType: aggregateType,
                    value: value,
                    daily: Daily(
                        xColumn: groupByX,
                        timePeriod: timePeriod),
                    data: CrosstabUtilities.ColumnsElements(
                        groupByX: groupByX,
                        dataRows: dataRows,
                        columnList: columnList));
            }
            return hb
                .Hidden(controlId: "CrosstabXType", value: groupByX?.TypeName)
                .Hidden(
                    controlId: "CrosstabPrevious",
                    value: Times.PreviousMonth(
                        context: context,
                        month: month))
                .Hidden(
                    controlId: "CrosstabNext",
                    value: Times.NextMonth(
                        context: context,
                        month: month))
                .Hidden(
                    controlId: "CrosstabThisMonth",
                    value: Times.ThisMonth(context: context));
        }

        private static bool Daily(Column xColumn, string timePeriod)
        {
            return xColumn?.TypeName == "datetime" && timePeriod == "Daily";
        }

        private static Dictionary<string, ControlData> CorrectedChoices(
            Column groupBy, IEnumerable<KeyValuePair<string, ControlData>> choices)
        {
            return groupBy != null
                ? groupBy.TypeName.CsTypeSummary() != Types.CsNumeric
                    ? choices.ToDictionary(o => o.Key, o => o.Value)
                    : choices.ToDictionary(
                        o => o.Key != string.Empty ? o.Key : "0",
                        o => o.Value)
                : null;
        }

        private static HtmlBuilder Table(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Dictionary<string, ControlData> choicesX,
            Dictionary<string, ControlData> choicesY,
            string aggregateType,
            Column value,
            bool daily,
            IEnumerable<CrosstabElement> data,
            IEnumerable<Column> columns = null)
        {
            var max = data.Any() && columns == null
                ? data.Select(o => o.Value).Max()
                : 0;
            return hb.Table(
                css: "grid fixed",
                action: () => hb
                    .THead(action: () => hb
                        .Tr(css: "ui-widget-header", action: () =>
                        {
                            if (choicesY != null)
                            {
                                hb.Th();
                            }
                            choicesX.ForEach(choiceX => hb
                                .Th(action: () => hb
                                    .HeaderText(
                                        context: context,
                                        ss: ss,
                                        aggregateType: aggregateType,
                                        value: value,
                                        showValue: columns?.Any() != true,
                                        data: data.Where(o => o.GroupByX == choiceX.Key),
                                        choice: choiceX)));
                        }))
                    .TBody(action: () =>
                    {
                        choicesY?.ForEach(choiceY =>
                        {
                            var column = columns?.Any() != true
                                ? value
                                : ss.GetColumn(
                                    context: context,
                                    columnName: choiceY.Key);
                            hb.Tr(css: "crosstab-row", action: () =>
                            {
                                var row = data.Where(o => o.GroupByY == choiceY.Key).ToList();
                                hb.Th(action: () => hb
                                    .HeaderText(
                                        context: context,
                                        ss: ss,
                                        aggregateType: aggregateType,
                                        value: column,
                                        showValue: true,
                                        data: row,
                                        choice: choiceY));
                                if (columns != null)
                                {
                                    max = row.Any()
                                        ? row.Max(o => o.Value)
                                        : 0;
                                }
                                choicesX.ForEach(choiceX => hb
                                    .Td(
                                        context: context,
                                        ss: ss,
                                        aggregateType: aggregateType,
                                        daily: daily,
                                        value: column,
                                        x: choiceX.Key,
                                        max: max,
                                        data: CrosstabUtilities
                                            .CellValue(
                                                data: data,
                                                choiceX: choiceX,
                                                choiceY: choiceY)));
                            });
                        });
                    }));

        }

        private static HtmlBuilder Td(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Column value,
            string aggregateType,
            bool daily,
            string x,
            decimal max,
            decimal data)
        {
            return hb.Td(css: DayOfWeekCss(daily, x), action: () => hb
                .Text(text: CrosstabUtilities.CellText(
                    context: context,
                    value: value,
                    aggregateType: aggregateType,
                    data: data))
                .Svg(css: "svg-crosstab", action: () => hb
                    .Rect(
                        x: 0,
                        y: 0,
                        width: max > 0
                            ? (data / max * 100).ToString("0.0") + "%"
                            : "0",
                        height: "20px")));
        }

        private static string DayOfWeekCss(bool daily, string x)
        {
            if (daily)
            {
                switch (x.ToDateTime().DayOfWeek)
                {
                    case DayOfWeek.Saturday: return "saturday";
                    case DayOfWeek.Sunday: return "sunday";
                }
            }
            return null;
        }

        private static HtmlBuilder HeaderText(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            string aggregateType,
            Column value,
            bool showValue,
            IEnumerable<CrosstabElement> data,
            KeyValuePair<string, ControlData> choice)
        {
            var num = data.Summary(aggregateType);
            return hb.Text(text: "{0}{1}".Params(
                choice.Value.DisplayValue(context: context),
                showValue && num != null
                    ? " : " + (aggregateType != "Count"
                        ? value.Display(
                            context: context,
                            value: value != null && data.Any()
                                ? num.ToDecimal()
                                : 0,
                            unit: true)
                        : num.ToString())
                    : string.Empty));
        }

        private static decimal? Summary(
            this IEnumerable<CrosstabElement> data, string aggregateType)
        {
            switch (aggregateType)
            {
                case "Count": return data.Sum(o => o.Value);
                case "Total": return data.Sum(o => o.Value);
                case "Min":
                    return data.Any()
                        ? data.Min(o => o.Value)
                        : 0;
                case "Max":
                    return data.Any()
                        ? data.Max(o => o.Value)
                        : 0;
                default: return null;
            }
        }
    }
}