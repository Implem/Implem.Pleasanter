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
using System.Web.WebPages;

namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlCalendar
    {
        public static HtmlBuilder Calendar(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            string timePeriod,
            Column groupBy,
            Column fromColumn,
            Column toColumn,
            DateTime date,
            DateTime begin,
            Dictionary<string, ControlData> choices,
            IEnumerable<DataRow> dataRows,
            bool inRange,
            long changedItemId)
        {
            return hb.Div(id: "Calendar", css: "both", action: () => hb
                .FieldDropDown(
                    context: context,
                    controlId: "CalendarGroupBy",
                    fieldCss: "field-auto-thin",
                    controlCss: " auto-postback",
                    labelText: Displays.GroupBy(context: context),
                    optionCollection: ss.CalendarGroupByOptions(),
                    selectedValue: groupBy?.ColumnName,
                    insertBlank: true,
                    method: "post")
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
                .FieldTextBox(
                    textType: HtmlTypes.TextTypes.DateTime,
                    fieldCss: "field-auto-thin",
                    controlId: "CalendarDate",
                    controlCss: " w100 auto-postback always-send",
                    labelText: "",
                    text: date
                        .ToLocal(context: context)
                        .ToString(Displays.YmdFormat(context: context)),
                    format: Displays.YmdDatePickerFormat(context: context),
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
                    text: Displays.Today(context: context),
                    controlCss: "button-icon",
                    onClick: "$p.moveCalendar('Today');",
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
                            groupBy: groupBy,
                            fromColumn: fromColumn,
                            toColumn: toColumn,
                            date: date,
                            begin: begin,
                            choices: choices,
                            dataRows: dataRows,
                            inRange: inRange,
                            changedItemId: changedItemId)));
        }

        public static HtmlBuilder CalendarBody(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            string timePeriod,
            Column groupBy,
            Column fromColumn,
            Column toColumn,
            DateTime date,
            DateTime begin,
            Dictionary<string, ControlData> choices,
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
                        && timePeriod != "Yearly"
                        && groupBy == null).ToOneOrZeroString())
                .Hidden(
                    controlId: "CalendarPrevious",
                    value: Times.PreviousCalendar(
                        context: context,
                        month: date,
                        timePeriod: timePeriod))
                .Hidden(
                    controlId: "CalendarNext",
                    value: Times.NextCalendar(
                        context: context,
                        month: date,
                        timePeriod: timePeriod))
                .Hidden(
                    controlId: "CalendarToday",
                    value: Times.Today(
                        context: context))
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
                        value: choices == null
                            ? Json(
                                context: context,
                                from: fromColumn,
                                to: toColumn,
                                dataRows: dataRows,
                                changedItemId: changedItemId)
                            : GroupingJson(
                                context: context,
                                from: fromColumn,
                                to: toColumn,
                                groupBy: groupBy,
                                dataRows: dataRows,
                                changedItemId: changedItemId))
                    .CalendarBodyTable(
                        context: context,
                        timePeriod: timePeriod,
                        date: date,
                        begin: begin,
                        groupBy: groupBy,
                        choices: choices)
                : hb;
        }

        private static HtmlBuilder CalendarBodyTable(
            this HtmlBuilder hb,
            Context context,
            string timePeriod,
            Column groupBy,
            DateTime date,
            DateTime begin,
            Dictionary<string, ControlData> choices)
        {
            switch (timePeriod)
            {
                case "Yearly":
                    return hb.YearlyTable(
                        context: context,
                        begin: begin,
                        groupBy:groupBy,
                        choices: choices);
                case "Monthly":
                    return hb.MonthlyTable(
                        context: context,
                        date: date,
                        begin: begin,
                        groupBy: groupBy,
                        choices: choices);
                case "Weekly":
                    return hb.WeeklyTable(
                        context: context,
                        date: date,
                        begin: begin,
                        groupBy: groupBy,
                        choices: choices);
                default:
                    return hb;
            }
        }

        private static HtmlBuilder YearlyTable(
            this HtmlBuilder hb,
            Context context,
            Column groupBy,
            DateTime begin,
            Dictionary<string, ControlData> choices)
        {
            return hb.Table(
                id: "Grid",
                css: "grid fixed",
                action: () => hb
                    .THead(action: () => hb
                        .Tr(action: () =>
                        {
                            hb.Th(
                                css: "ui-widget-header",
                                action: () => hb.Text(groupBy.LabelText),
                                _using: groupBy != null);
                            for (var x = 0; x < 12; x++)
                            {
                                var currentDate = begin.ToLocal(context: context).AddMonths(x);
                                hb.Th(action: () => hb
                                    .A(
                                        css: "calendar-to-monthly",
                                        href: "javascript:void(0);",
                                        attributes: new HtmlAttributes()
                                            .DataId(currentDate.ToString()),
                                        action: () => hb
                                            .Text(text: currentDate.ToString(
                                                "Y", context.CultureInfo()))));
                            }
                        }))
                    .TBody(action: () =>
                    {
                        choices = choices ?? new Dictionary<string, ControlData>() { [string.Empty] = null };
                        choices?.ForEach(choice =>
                        {
                            hb.Tr(css: "calendar-row", action: () =>
                            {
                                hb.Th(action: () => hb.Text(
                                    choice.Value.DisplayValue(context: context)),
                                    _using: choice.Value != null);
                                for (var x = 0; x < 12; x++)
                                {
                                    var date = begin.ToLocal(context: context).AddMonths(x);
                                    hb.Td(
                                        attributes: new HtmlAttributes()
                                            .Class("container")
                                            .DataValue(value: choice.Key, _using: choice.Value != null)
                                            .DataId(date.ToString("yyyy/M/d")),
                                        action: () => hb
                                            .Div());
                                }
                            });
                        });
                    }));
        }

        private static HtmlBuilder MonthlyTable(
            this HtmlBuilder hb,
            Context context,
            Column groupBy,
            DateTime date,
            DateTime begin,
            Dictionary<string, ControlData> choices)
        {
            return hb.Table(
                id: "Grid",
                css: "grid fixed",
                action: () => hb
                    .THead(action: () => hb
                        .Tr(action: () =>
                        {
                            hb.Th(
                                css: "ui-widget-header",
                                action: () => hb.Text(groupBy.LabelText),
                                _using: groupBy != null);
                            for (var x = 0; x < 7; x++)
                            {
                                hb.Th(css: DayOfWeekCss(x) + " calendar-header", action: () => hb
                                    .Text(text: DayOfWeekString(
                                        context: context,
                                        x: x)));
                            }
                        }))
                    .TBody(action: () =>
                    {
                        choices = choices ?? new Dictionary<string, ControlData>() { [string.Empty] = null };
                        choices.ForEach(choice =>
                        {
                            hb.Tr(css: "calendar-row", action: () =>
                            {
                                hb.Th(
                                    attributes: new HtmlAttributes().Add("rowspan", "7"),
                                    action: () => hb.Text(choice.Value.DisplayValue(context: context)),
                                    _using: choice.Value != null);
                                for (var y = 0; y < 6; y++)
                                {
                                    hb.Tr(action: () =>
                                    {
                                        for (var x = 0; x < 7; x++)
                                        {
                                            var currentDate = begin.ToLocal(context: context).AddDays(y * 7 + x);
                                            hb.Td(
                                                attributes: new HtmlAttributes()
                                                    .Class("container" +
                                                        (currentDate == DateTime.Now.ToLocal(context: context).Date
                                                            ? " today"
                                                            : string.Empty) +
                                                        (date.ToLocal(context: context).Month
                                                            != currentDate.ToLocal(context: context).Month
                                                                ? " other-month"
                                                                : string.Empty))
                                                    .DataValue(value: choice.Key, _using: choice.Value != null)
                                                    .DataId(currentDate.ToString("yyyy/M/d")),
                                                action: () => hb
                                                    .Div(action: () => hb
                                                        .Div(
                                                            css: "day",
                                                            action: () => hb
                                                                .Text(currentDate.Day.ToString()))));
                                        }
                                    });
                                }
                            });
                        });
                    }));
        }

        private static HtmlBuilder WeeklyTable(
            this HtmlBuilder hb,
            Context context,
            Column groupBy,
            DateTime date,
            DateTime begin,
            Dictionary<string, ControlData> choices)
        {
            return hb.Table(
                id: "Grid",
                css: "grid fixed",
                action: () => hb
                    .THead(action: () => hb
                        .Tr(action: () =>
                        {
                            hb.Th(
                                css: "ui-widget-header",
                                action: () => hb.Text(groupBy.LabelText),
                                _using: groupBy != null);
                            for (var x = 0; x < 7; x++)
                            {
                                hb.Th(css: DayOfWeekCss(x) + " calendar-header", action: () => hb
                                    .Text(text: DayOfWeekString(
                                        context: context,
                                        x: x)));
                            }
                        }))
                    .TBody(action: () =>
                    {
                        choices = choices ?? new Dictionary<string, ControlData>() { [string.Empty] = null };
                        choices.ForEach(choice =>
                        {
                            hb.Tr(css: "calendar-row", action: () =>
                            {
                                hb.Th(action: () =>
                                    hb.Text(choice.Value.DisplayValue(context: context)),
                                    _using: choice.Value != null);
                                for (var x = 0; x < 7; x++)
                                {
                                    var currentDate = begin.ToLocal(context: context).AddDays(x);
                                    hb.Td(
                                        attributes: new HtmlAttributes()
                                            .Class("container" +
                                                (currentDate == DateTime.Now.ToLocal(context: context).Date
                                                    ? " today"
                                                    : string.Empty) +
                                                (date.ToLocal(context: context).Month
                                                    != currentDate.ToLocal(context: context).Month
                                                        ? " other-month"
                                                        : string.Empty))
                                            .DataValue(value: choice.Key, _using: choice.Value != null)
                                            .DataId(currentDate.ToString("yyyy/M/d")),
                                        action: () => hb
                                            .Div(action: () => hb
                                                .Div(
                                                    css: "day",
                                                    action: () => hb
                                                        .Text(currentDate.Day.ToString()))));
                                }
                            });

                        });
                    }));
        }

        private static string GroupingJson(
            Context context,
            Column from,
            Column to,
            Column groupBy,
            IEnumerable<DataRow> dataRows,
            long changedItemId)
        {
            return dataRows
                .GroupBy(
                    dataRow => dataRow.String(groupBy.ColumnName),
                    dataRow =>
                        CreateCalendarElement(
                            context: context,
                            from: from,
                            to: to,
                            changedItemId: changedItemId,
                            dataRow: dataRow))
                .Select(group => new
                {
                    group = group.Key,
                    items = group
                        .OrderBy(o => o.From)
                        .ThenBy(o => o.To)
                        .ThenBy(o => o.UpdatedTime)
                })
                .ToJson();
        }

        private static string Json(
            Context context,
            Column from,
            Column to,
            IEnumerable<DataRow> dataRows,
            long changedItemId)
        {
            return new [] { new
            {
                group = (string)null,
                items = dataRows.Select(dataRow =>
                    CreateCalendarElement(
                        context: context,
                        from: from,
                        to: to,
                        changedItemId: changedItemId,
                        dataRow: dataRow))
                            .OrderBy(o => o.From)
                            .ThenBy(o => o.To)
                            .ThenBy(o => o.UpdatedTime)
            }}
            .ToJson();
        }

        private static CalendarElement CreateCalendarElement(
            Context context,
            Column from,
            Column to,
            long changedItemId,
            DataRow dataRow)
        {
            return new CalendarElement(
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
            );
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