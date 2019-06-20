using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Extensions;
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
    public static class HtmlCalendar
    {
        public static HtmlBuilder Calendar(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            string timePeriod,
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
                    context: context,
                    controlId: "CalendarTimePeriod",
                    fieldCss: "field-auto-thin",
                    controlCss: " auto-postback",
                    labelText: Displays.Period(context: context),
                    optionCollection: ss.CalendarTimePeriodOptions(context: context),
                    selectedValue: timePeriod,
                    method: "post")
                .FieldDropDown(
                    context: context,
                    controlId: "CalendarFromTo",
                    fieldCss: "field-auto-thin",
                    controlCss: " auto-postback",
                    labelText: Displays.Column(context: context),
                    optionCollection: ss.CalendarColumnOptions(context: context),
                    selectedValue: toColumn == null
                        ? fromColumn.ColumnName
                        : $"{fromColumn.ColumnName}-{toColumn.ColumnName}",
                    action: "Calendar",
                    method: "post")
                .DropDown(
                    context: context,
                    controlId: "CalendarMonth",
                    controlCss: " w100 auto-postback",
                    optionCollection: CalendarMonth(context: context),
                    selectedValue: new DateTime(month.Year, month.Month, 1).ToString(),
                    action: "Calendar",
                    method: "post")
                .Button(
                    text: Displays.Previous(context: context),
                    controlCss: "button-icon",
                    accessKey: "b",
                    onClick: "$p.moveCalendar('Previous');",
                    icon: "ui-icon-seek-prev")
                .Button(
                    text: Displays.Next(context: context),
                    controlCss: "button-icon",
                    accessKey: "n",
                    onClick: "$p.moveCalendar('Next');",
                    icon: "ui-icon-seek-next")
                .Button(
                    text: Displays.ThisMonth(context: context),
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
                            context: context,
                            ss: ss,
                            timePeriod: timePeriod,
                            fromColumn: fromColumn,
                            toColumn: toColumn,
                            month: month,
                            begin: begin,
                            dataRows: dataRows,
                            inRange: inRange,
                            changedItemId: changedItemId)));
        }

        private static Dictionary<string, ControlData> CalendarMonth(Context context)
        {
            var now = DateTime.Now;
            var month = new DateTime(
                year: now.ToLocal(context: context).Year,
                month: now.ToLocal(context: context).Month,
                day: 1);
            return Enumerable.Range(
                Parameters.General.CalendarBegin,
                Parameters.General.CalendarEnd - Parameters.General.CalendarBegin)
                    .ToDictionary(
                        o => month.AddMonths(o).ToString(),
                        o => new ControlData(month.AddMonths(o).ToString(
                            "Y", context.CultureInfo())));
        }

        public static HtmlBuilder CalendarBody(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            string timePeriod,
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
                        !fromColumn.RecordedTime
                        && fromColumn.EditorReadOnly != true
                        && fromColumn.CanUpdate
                        && timePeriod == "Monthly").ToOneOrZeroString())
                .Hidden(
                    controlId: "CalendarPrevious",
                    value: Times.PreviousMonth(
                        context: context,
                        month: month))
                .Hidden(
                    controlId: "CalendarNext",
                    value: Times.NextMonth(
                        context: context,
                        month: month))
                .Hidden(
                    controlId: "CalendarThisMonth",
                    value: Times.ThisMonth(context: context))
                .Hidden(
                    controlId: "CalendarFromDefaultInput",
                    value: fromColumn.DefaultInput)
                .Hidden(
                    controlId: "CalendarToDefaultInput",
                    value: toColumn?.DefaultInput);
            return inRange
                ? hb
                    .Hidden(
                        controlId: "CalendarJson",
                        value: Json(
                            context: context,
                            ss: ss,
                            from: fromColumn,
                            to: toColumn,
                            dataRows: dataRows,
                            changedItemId: changedItemId))
                    .CalendarBodyTable(
                        context: context,
                        timePeriod: timePeriod,
                        month: month,
                        begin: begin)
                : hb;
        }

        private static HtmlBuilder CalendarBodyTable(
            this HtmlBuilder hb,
            Context context,
            string timePeriod,
            DateTime month,
            DateTime begin)
        {
            switch (timePeriod)
            {
                case "Yearly":
                    return hb.YearlyTable(
                        context: context,
                        month: month,
                        begin: begin);
                case "Monthly":
                    return hb.MonthlyTable(
                        context: context,
                        month: month,
                        begin: begin);
                default:
                    return hb;
            }
        }

        private static HtmlBuilder YearlyTable(
            this HtmlBuilder hb, Context context, DateTime month, DateTime begin)
        {
            return hb.Table(id: "Grid", action: () => hb
                .THead(action: () => hb
                    .Tr(action: () =>
                    {
                        for (var x = 0; x < 12; x++)
                        {
                            var date = begin.AddMonths(x);
                            hb.Th(action: () => hb
                                .A(
                                    css: "calendar-to-monthly",
                                    href: "javascript:void(0);",
                                    attributes: new HtmlAttributes()
                                        .DataId(date.ToString()),
                                    action: () => hb
                                        .Text(text: date.ToString(
                                            "Y", context.CultureInfo()))));
                        }
                    }))
                .TBody(action: () =>
                {
                    for (var x = 0; x < 12; x++)
                    {
                        var date = begin.AddMonths(x);
                        hb.Td(
                            attributes: new HtmlAttributes()
                                .Class("container")
                                .DataId(date.ToString("yyyy/M/d")),
                            action: () => hb
                                .Div());
                    }
                }));
        }

        private static HtmlBuilder MonthlyTable(
            this HtmlBuilder hb, Context context, DateTime month, DateTime begin)
        {
            return hb.Table(id: "Grid", action: () => hb
                .THead(action: () => hb
                    .Tr(action: () =>
                    {
                        for (var x = 0; x < 7; x++)
                        {
                            hb.Th(css: DayOfWeekCss(x), action: () => hb
                                .Text(text: DayOfWeekString(
                                    context: context,
                                    x: x)));
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
                                var date = begin.ToLocal(context: context).AddDays(y * 7 + x);
                                hb.Td(
                                    attributes: new HtmlAttributes()
                                        .Class("container" +
                                            (date == DateTime.Now.ToLocal(context: context).Date
                                                ? " today"
                                                : string.Empty) +
                                            (month.ToLocal(context: context).Month
                                                != date.ToLocal(context: context).Month
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
            Context context,
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
                    ? dataRow.DateTime("From").ToLocal(context: context).ToString("t") + " "
                    : null),
                from: ConvertIfCompletionTime(
                    context: context,
                    column: from,
                    dateTime: dataRow.DateTime("from")),
                to: ConvertIfCompletionTime(
                    context: context,
                    column: to,
                    dateTime: dataRow.DateTime("to")),
                changedItemId: changedItemId,
                updatedTime: dataRow.DateTime("UpdatedTime")
            ))
                .OrderBy(o => o.From)
                .ThenBy(o => o.To)
                .ThenBy(o => o.UpdatedTime)
                .ToJson();
        }

        private static DateTime ConvertIfCompletionTime(
            Context context, Column column, DateTime dateTime)
        {
            switch (column?.ColumnName)
            {
                case "CompletionTime":
                    return dateTime
                        .ToLocal(context: context)
                        .AddDifferenceOfDates(column.EditorFormat, minus: true);
                default:
                    return dateTime.ToLocal(context: context);
            }
        }

        private static string DayOfWeekCss(int x)
        {
            return Parameters.General.FirstDayOfWeek + x > 6
                ? ((DayOfWeek)(Parameters.General.FirstDayOfWeek + x - 7)).ToString().ToLower()
                : ((DayOfWeek)(Parameters.General.FirstDayOfWeek + x)).ToString().ToLower();
        }

        private static string DayOfWeekString(Context context, int x)
        {
            return Displays.Get(
                context: context,
                id: Parameters.General.FirstDayOfWeek + x > 6
                    ? ((DayOfWeek)(Parameters.General.FirstDayOfWeek + x - 7)).ToString()
                    : ((DayOfWeek)(Parameters.General.FirstDayOfWeek + x)).ToString());
        }
    }
}