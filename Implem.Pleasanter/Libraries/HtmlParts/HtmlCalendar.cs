using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Extensions;
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
    public static class HtmlCalendar
    {
        public static HtmlBuilder Calendar(
            this HtmlBuilder hb,
            SiteSettings ss,
            Column fromColumn,
            Column toColumn,
            DateTime month,
            DateTime begin,
            IEnumerable<DataRow> dataRows,
            bool inRange,
            long changedItemId)
        {
            return hb.Div(id: "Calendar", css: "both", action: () => hb
                .FieldDropDown(
                    controlId: "CalendarFromTo",
                    fieldCss: "field-auto-thin",
                    controlCss: " auto-postback",
                    labelText: Displays.Column(),
                    optionCollection: ss.CalendarColumnOptions(),
                    selectedValue: toColumn == null
                        ? fromColumn.ColumnName
                        : $"{fromColumn.ColumnName}-{toColumn.ColumnName}",
                    action: "Calendar",
                    method: "post")
                .DropDown(
                    controlId: "CalendarMonth",
                    controlCss: " w100 auto-postback",
                    optionCollection: CalendarMonth(),
                    selectedValue: new DateTime(month.Year, month.Month, 1).ToString(),
                    action: "Calendar",
                    method: "post")
                .Button(
                    text: Displays.Previous(),
                    controlCss: "button-icon",
                    accessKey: "b",
                    onClick: "$p.moveCalendar('Previous');",
                    icon: "ui-icon-seek-prev")
                .Button(
                    text: Displays.Next(),
                    controlCss: "button-icon",
                    accessKey: "n",
                    onClick: "$p.moveCalendar('Next');",
                    icon: "ui-icon-seek-next")
                .Button(
                    text: Displays.ThisMonth(),
                    controlCss: "button-icon",
                    onClick: "$p.moveCalendar('ThisMonth');",
                    icon: "ui-icon-calendar")
                .Div(
                    attributes: new HtmlAttributes()
                        .Id("CalendarBody")
                        .DataAction("UpdateByCalendar")
                        .DataMethod("post"),
                    action: () => hb
                        .CalendarBody(
                            ss: ss,
                            fromColumn: fromColumn,
                            toColumn: toColumn,
                            month: month,
                            begin: begin,
                            dataRows: dataRows,
                            inRange: inRange,
                            changedItemId: changedItemId)));
        }

        private static Dictionary<string, ControlData> CalendarMonth()
        {
            var now = DateTime.Now;
            var month = new DateTime(now.ToLocal().Year, now.ToLocal().Month, 1);
            return Enumerable.Range(
                Parameters.General.CalendarBegin,
                Parameters.General.CalendarEnd - Parameters.General.CalendarBegin)
                    .ToDictionary(
                        o => month.AddMonths(o).ToString(),
                        o => new ControlData(month.AddMonths(o).ToString(
                            "Y", Sessions.CultureInfo())));
        }

        public static HtmlBuilder CalendarBody(
            this HtmlBuilder hb,
            SiteSettings ss,
            Column fromColumn,
            Column toColumn,
            DateTime month,
            DateTime begin,
            IEnumerable<DataRow> dataRows,
            bool inRange,
            long changedItemId)
        {
            hb
                .Hidden(
                    controlId: "CalendarCanUpdate",
                    value: (
                        !fromColumn.RecordedTime &&
                        fromColumn.EditorReadOnly != true &&
                        fromColumn.CanUpdate).ToOneOrZeroString())
                .Hidden(controlId: "CalendarPrevious", value: Times.PreviousMonth(month))
                .Hidden(controlId: "CalendarNext", value: Times.NextMonth(month))
                .Hidden(controlId: "CalendarThisMonth", value: Times.ThisMonth());
            return inRange
                ? hb
                    .Hidden(
                        controlId: "CalendarJson",
                        value: Json(ss, fromColumn, toColumn, dataRows, changedItemId))
                    .CalendarBodyTable(month, begin)
                : hb;
        }

        private static HtmlBuilder CalendarBodyTable(
            this HtmlBuilder hb, DateTime month, DateTime begin)
        {
            return hb.Table(action: () => hb
                .THead(action: () => hb
                    .Tr(action: () =>
                    {
                        for (var x = 0; x < 7; x++)
                        {
                            hb.Th(css: DayOfWeekCss(x), action: () => hb
                                .Text(text: DayOfWeekString(x)));
                        }
                    }))
                .TBody(action: () =>
                {
                    for (var y = 0; y < 6; y++)
                    {
                        hb.Tr(action: () =>
                        {
                            for (var x = 0; x < 7; x++)
                            {
                                var date = begin.ToLocal().AddDays(y * 7 + x);
                                hb.Td(
                                    attributes: new HtmlAttributes()
                                        .Class("container" +
                                            (date == DateTime.Now.ToLocal().Date
                                                ? " today"
                                                : string.Empty) +
                                            (month.ToLocal().Month != date.ToLocal().Month
                                                ? " other-month"
                                                : string.Empty))
                                        .DataId(date.ToString("yyyy/M/d")),
                                    action: () => hb
                                        .Div(action: () => hb
                                            .Div(
                                                css: "day",
                                                action: () => hb
                                                    .Text(date.Day.ToString()))));
                            }
                        });
                    }
                }));
        }

        private static string Json(
            SiteSettings ss,
            Column from,
            Column to,
            IEnumerable<DataRow> dataRows,
            long changedItemId)
        {
            return dataRows.Select(dataRow => new CalendarElement
            (
                id: dataRow.Long("Id"),
                title: dataRow.String("ItemTitle"),
                time: (from.EditorFormat == "Ymdhm"
                    ? dataRow.DateTime("From").ToLocal().ToString("t") + " "
                    : null),
                from: ConvertIfCompletionTime(from, dataRow.DateTime("from")),
                to: ConvertIfCompletionTime(to, dataRow.DateTime("to")),
                changedItemId: changedItemId,
                updatedTime: dataRow.DateTime("UpdatedTime")
            ))
                .OrderBy(o => o.From)
                .ThenBy(o => o.To)
                .ThenBy(o => o.UpdatedTime)
                .ToJson();
        }

        private static DateTime ConvertIfCompletionTime(Column column, DateTime dateTime)
        {
            switch (column?.ColumnName)
            {
                case "CompletionTime":
                    return dateTime
                        .ToLocal()
                        .AddDifferenceOfDates(column.EditorFormat, minus: true);
                default:
                    return dateTime.ToLocal();
            }
        }

        private static string DayOfWeekCss(int x)
        {
            return Parameters.General.FirstDayOfWeek + x > 6
                ? ((DayOfWeek)(Parameters.General.FirstDayOfWeek + x - 7)).ToString().ToLower()
                : ((DayOfWeek)(Parameters.General.FirstDayOfWeek + x)).ToString().ToLower();
        }

        private static string DayOfWeekString(int x)
        {
            return Displays.Get(Parameters.General.FirstDayOfWeek + x > 6
                ? ((DayOfWeek)(Parameters.General.FirstDayOfWeek + x - 7)).ToString()
                : ((DayOfWeek)(Parameters.General.FirstDayOfWeek + x)).ToString());
        }
    }
}