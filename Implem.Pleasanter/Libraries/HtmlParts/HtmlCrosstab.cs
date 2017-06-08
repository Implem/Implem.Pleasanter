using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
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
            SiteSettings ss,
            View view,
            string groupByX,
            string groupByY,
            string columns,
            string aggregateType,
            string value,
            string timePeriod,
            DateTime month,
            EnumerableRowCollection<DataRow> dataRows)
        {
            return hb.Div(id: "Crosstab", css: "both", action: () =>
            {
                hb
                    .FieldDropDown(
                        controlId: "CrosstabGroupByX",
                        fieldCss: "field-auto-thin",
                        controlCss: " auto-postback",
                        labelText: Displays.GroupByX(),
                        optionCollection: ss.CrosstabGroupByXOptions(),
                        selectedValue: groupByX,
                        method: "post")
                    .FieldDropDown(
                        controlId: "CrosstabGroupByY",
                        fieldCss: "field-auto-thin",
                        controlCss: " auto-postback",
                        labelText: Displays.GroupByY(),
                        optionCollection: ss.CrosstabGroupByYOptions(),
                        selectedValue: groupByY,
                        method: "post")
                    .FieldDropDown(
                        fieldId: "CrosstabColumnsField",
                        controlId: "CrosstabColumns",
                        fieldCss: "field-auto-thin",
                        controlCss: " auto-postback",
                        labelText: Displays.NumericColumn(),
                        optionCollection: ss.CrosstabColumnsOptions(),
                        selectedValue: columns,
                        multiple: true,
                        method: "post")
                    .FieldDropDown(
                        controlId: "CrosstabAggregateType",
                        fieldCss: "field-auto-thin",
                        controlCss: " auto-postback",
                        labelText: Displays.AggregationType(),
                        optionCollection: ss.CrosstabAggregationTypeOptions(),
                        selectedValue: aggregateType,
                        method: "post")
                    .FieldDropDown(
                        fieldId: "CrosstabValueField",
                        controlId: "CrosstabValue",
                        fieldCss: "field-auto-thin",
                        controlCss: " auto-postback",
                        labelText: Displays.AggregationTarget(),
                        optionCollection: ss.CrosstabValueOptions(),
                        selectedValue: value,
                        method: "post")
                    .FieldDropDown(
                        fieldId: "CrosstabTimePeriodField",
                        controlId: "CrosstabTimePeriod",
                        fieldCss: "field-auto-thin",
                        controlCss: " auto-postback",
                        labelText: Displays.Period(),
                        optionCollection: ss.CrosstabTimePeriodOptions(),
                        selectedValue: timePeriod,
                        method: "post")
                    .DropDown(
                        controlId: "CrosstabMonth",
                        controlCss: " w100 auto-postback",
                        optionCollection: CrosstabMonth(),
                        selectedValue: new DateTime(month.Year, month.Month, 1).ToString(),
                        action: "Crosstab",
                        method: "post")
                    .Button(
                        controlId: "CrosstabPreviousButton",
                        text: Displays.Previous(),
                        controlCss: "button-icon",
                        accessKey: "b",
                        onClick: "$p.moveCrosstab('Previous');",
                        icon: "ui-icon-seek-prev")
                    .Button(
                        controlId: "CrosstabNextButton",
                        text: Displays.Next(),
                        controlCss: "button-icon",
                        accessKey: "n",
                        onClick: "$p.moveCrosstab('Next');",
                        icon: "ui-icon-seek-next")
                    .Button(
                        controlId: "CrosstabThisMonthButton",
                        text: Displays.ThisMonth(),
                        controlCss: "button-icon",
                        accessKey: "n",
                        onClick: "$p.moveCrosstab('ThisMonth');",
                        icon: "ui-icon-Crosstab")
                    .CrosstabBody(
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
            });
        }

        private static Dictionary<string, ControlData> CrosstabMonth()
        {
            var now = DateTime.Now;
            var month = new DateTime(now.ToLocal().Year, now.ToLocal().Month, 1);
            return Enumerable.Range(
                Parameters.General.CrosstabBegin,
                Parameters.General.CrosstabEnd - Parameters.General.CrosstabBegin)
                    .ToDictionary(
                        o => month.AddMonths(o).ToString(),
                        o => new ControlData(month.AddMonths(o).ToString(
                            "Y", Sessions.CultureInfo())));
        }

        public static HtmlBuilder CrosstabBody(
            this HtmlBuilder hb,
            SiteSettings ss,
            View view,
            string groupByX,
            string groupByY,
            string columns,
            string aggregateType,
            string value,
            string timePeriod,
            DateTime month,
            EnumerableRowCollection<DataRow> dataRows)
        {
            if (groupByY != "Columns")
            {
                hb.CrosstabBody(
                    ss: ss,
                    view: view,
                    groupByX: groupByX,
                    groupByY: groupByY,
                    aggregateType: aggregateType,
                    value: value,
                    timePeriod: timePeriod,
                    month: month,
                    dataRows: dataRows);
            }
            else
            {
                hb.CrosstabColumnsBody(
                    ss: ss,
                    view: view,
                    groupByX: groupByX,
                    columns: columns,
                    aggregateType: aggregateType,
                    value: value,
                    timePeriod: timePeriod,
                    month: month,
                    dataRows: dataRows);
            }
            return hb
                .Hidden(controlId: "CrosstabXType", value: ss.GetColumn(groupByX)?.TypeName)
                .Hidden(controlId: "CrosstabPrevious", value: Times.PreviousMonth(month))
                .Hidden(controlId: "CrosstabNext", value: Times.NextMonth(month))
                .Hidden(controlId: "CrosstabThisMonth", value: Times.ThisMonth());
        }

        private static HtmlBuilder CrosstabBody(
            this HtmlBuilder hb,
            SiteSettings ss,
            View view,
            string groupByX,
            string groupByY,
            string aggregateType,
            string value,
            string timePeriod,
            DateTime month,
            EnumerableRowCollection<DataRow> dataRows)
        {
            var data = dataRows.Select(o => new CrosstabElement(
                o["GroupByX"].ToString(),
                o["GroupByY"].ToString(),
                o["Value"].ToDecimal()));
            var xColumn = ss.GetColumn(groupByX);
            var yColumn = ss.GetColumn(groupByY);
            var choicesX = xColumn?.TypeName == "datetime"
                ? CorrectedChoices(xColumn, timePeriod, month)
                : CorrectedChoices(
                    xColumn, xColumn?.Choices(data.Select(o => o.GroupByX)));
            var choicesY = CorrectedChoices(
                yColumn, yColumn?.Choices(data.Select(o => o.GroupByY)));
            return hb
                .Div(
                    attributes: new HtmlAttributes()
                        .Id("CrosstabBody")
                        .DataAction("UpdateByCrosstab")
                        .DataMethod("post"),
                    action: () => hb
                        .Table(
                            ss: ss,
                            choicesX: choicesX,
                            choicesY: choicesY,
                            aggregateType: aggregateType,
                            value: ss.GetColumn(value),
                            columns: null,
                            data: data));
        }

        private static HtmlBuilder CrosstabColumnsBody(
            this HtmlBuilder hb,
            SiteSettings ss,
            View view,
            string groupByX,
            string columns,
            string aggregateType,
            string value,
            string timePeriod,
            DateTime month,
            EnumerableRowCollection<DataRow> dataRows)
        {
            var columnList = CrosstabUtilities.GetColumns(
                ss, columns.Deserialize<IEnumerable<string>>());
            var data = new List<CrosstabElement>();
            dataRows.ForEach(o =>
                columnList.ForEach(column =>
                    data.Add(new CrosstabElement(
                        o["GroupByX"].ToString(),
                        column,
                        o[column].ToDecimal()))));
            var xColumn = ss.GetColumn(groupByX);
            var choicesX = xColumn?.TypeName == "datetime"
                ? CorrectedChoices(xColumn, timePeriod, month)
                : CorrectedChoices(
                    xColumn, xColumn?.Choices(data.Select(o => o.GroupByX)));
            var choicesY = columnList.ToDictionary(
                o => o,
                o => new ControlData(ss.GetColumn(o)?.LabelText));
            return hb
                .Div(
                    attributes: new HtmlAttributes()
                        .Id("CrosstabBody")
                        .DataAction("UpdateByCrosstab")
                        .DataMethod("post"),
                    action: () => hb
                        .Table(
                            ss: ss,
                            choicesX: choicesX,
                            choicesY: choicesY,
                            aggregateType: aggregateType,
                            value: ss.GetColumn(value),
                            columns: columnList,
                            data: data));
        }

        private static Dictionary<string, ControlData> CorrectedChoices(
            Column groupBy, string timePeriod, DateTime date)
        {
            switch (timePeriod)
            {
                case "Monthly": return Monthly(date);
                case "Weekly": return Weekly(date);
                case "Daily": return Daily(date);
                default: return null;
            }
        }

        private static Dictionary<string, ControlData> Monthly(DateTime date)
        {
            var hash = new Dictionary<string, ControlData>();
            for (var i = -11; i <= 0; i++)
            {
                var day = date.AddMonths(i);
                hash.Add(day.ToString("yyyy/MM"), new ControlData(day.ToString("yyyy/MM")));
            }
            return hash;
        }

        private static Dictionary<string, ControlData> Weekly(DateTime date)
        {
            var hash = new Dictionary<string, ControlData>();
            var end = CrosstabUtilities.WeeklyEndDate(date);
            for (var i = -77; i <= 0; i += 7)
            {
                var day = end.AddDays(i);
                var append = (int)(new DateTime(day.Year, 1, 1).DayOfWeek) > 1
                    ? 8 - (int)(new DateTime(day.Year, 1, 1).DayOfWeek)
                    : 0;
                var key = day.Year * 100 + ((day.DayOfYear + append) / 7) + 1;
                hash.Add(key.ToString(), new ControlData(day.ToString("MM/dd")));
            }
            return hash;
        }

        private static Dictionary<string, ControlData> Daily(DateTime date)
        {
            var hash = new Dictionary<string, ControlData>();
            var month = date.Month;
            while (month == date.Month)
            {
                hash.Add(date.ToString("yyyy/MM/dd"), new ControlData(date.ToString("dd")));
                date = date.AddDays(1);
            }
            return hash;
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

        private static Dictionary<string, ControlData> Choices(
            this Column column, IEnumerable<string> data)
        {
            return column
                .EditChoices(insertBlank: true)
                .Where(o => data.Contains(o.Key))
                .ToDictionary(o => o.Key, o => o.Value);
        }

        private static HtmlBuilder Table(
            this HtmlBuilder hb,
            SiteSettings ss,
            Dictionary<string, ControlData> choicesX,
            Dictionary<string, ControlData> choicesY,
            string aggregateType,
            Column value,
            IEnumerable<string> columns,
            IEnumerable<CrosstabElement> data)
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
                            choicesX.ForEach(choice => hb
                                .Th(action: () => hb
                                    .HeaderText(
                                        ss: ss,
                                        aggregateType: aggregateType,
                                        value: value,
                                        showValue: columns?.Any() != true,
                                        data: data.Where(o => o.GroupByX == choice.Key),
                                        choice: choice)));
                        }))
                    .TBody(action: () =>
                    {
                        choicesY?.ForEach(choiceY =>
                        {
                            var column = columns?.Any() != true
                                ? value
                                : ss.GetColumn(choiceY.Key);
                            hb.Tr(css: "crosstab-row", action: () =>
                            {
                                var row = data.Where(o => o.GroupByY == choiceY.Key).ToList();
                                hb.Th(action: () => hb
                                    .HeaderText(
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
                                    .Td(ss: ss,
                                        aggregateType: aggregateType,
                                        value: column,
                                        max: max,
                                        data: data.FirstOrDefault(o =>
                                            o.GroupByX == choiceX.Key &&
                                            o.GroupByY == choiceY.Key)?.Value ?? 0));
                            });
                        });
                    }));

        }

        private static HtmlBuilder Td(
            this HtmlBuilder hb,
            SiteSettings ss,
            Column value,
            string aggregateType,
            decimal max,
            decimal data)
        {
            return hb.Td(action: () => hb
                .Text(text: value?.Display(
                    data, unit: aggregateType != "Count", format: aggregateType != "Count") ??
                        data.ToString())
                .Svg(css: "svg-crosstab", action: () => hb
                    .Rect(
                        x: 0,
                        y: 0,
                        width: max > 0
                            ? (data / max * 100).ToString("0.0") + "%"
                            : "0",
                        height: "20px")));
        }

        private static HtmlBuilder HeaderText(
            this HtmlBuilder hb,
            SiteSettings ss,
            string aggregateType,
            Column value,
            bool showValue,
            IEnumerable<CrosstabElement> data,
            KeyValuePair<string, ControlData> choice)
        {
            var num = data.Summary(aggregateType);
            return hb.Text(text: "{0}{1}".Params(
                choice.Value.Text != string.Empty
                    ? choice.Value.Text
                    : Displays.NotSet(),
                showValue && num != null
                    ? " : " + (aggregateType != "Count"
                        ? value.Display(
                            value != null && data.Any()
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