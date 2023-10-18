using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.General;
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
            Column groupBy,
            Column fromColumn,
            Column toColumn,
            DateTime date,
            DateTime begin,
            DateTime end,
            string CalendarViewType,
            Dictionary<string, ControlData> choices,
            IEnumerable<DataRow> dataRows,
            bool showStatus,
            bool inRange,
            long changedItemId,
            string calendarType,
            string prefix,
            string calendarFromTo)
        {
            if (calendarType == "Standard")
            {
                return hb.Div(id: $"{prefix}Calendar", css: "both", action: () => hb
                .FieldDropDown(
                    context: context,
                    controlId: "CalendarGroupBy",
                    fieldCss: "field-auto-thin",
                    controlCss: " auto-postback",
                    labelText: Displays.GroupBy(context: context),
                    optionCollection: ss.CalendarGroupByOptions(context: context),
                    selectedValue: groupBy?.ColumnName,
                    insertBlank: true,
                    method: "post",
                    _using: prefix.IsNullOrEmpty())
                .FieldDropDown(
                    context: context,
                    controlId: "CalendarTimePeriod",
                    fieldCss: "field-auto-thin",
                    controlCss: " auto-postback",
                    labelText: Displays.Period(context: context),
                    optionCollection: ss.CalendarTimePeriodOptions(context: context),
                    selectedValue: timePeriod,
                    method: "post",
                    _using: prefix.IsNullOrEmpty())
                .FieldDropDown(
                    context: context,
                    controlId: "CalendarFromTo",
                    fieldCss: "field-auto-thin",
                    controlCss: " auto-postback",
                    labelText: Displays.Column(context: context),
                    optionCollection: ss.CalendarColumnOptions(context: context),
                    selectedValue: toColumn == null
                        ? fromColumn?.ColumnName
                        : $"{fromColumn?.ColumnName}-{toColumn?.ColumnName}",
                    action: "Calendar",
                    method: "post",
                    _using: prefix.IsNullOrEmpty())
                .FieldTextBox(
                    textType: HtmlTypes.TextTypes.DateTime,
                    fieldCss: "field-auto-thin",
                    controlId: $"{prefix}CalendarDate",
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
                    onClick: $"$p.moveCalendar('Previous','{prefix}');",
                    icon: "ui-icon-seek-prev")
                .Button(
                    text: Displays.Next(context: context),
                    controlCss: "button-icon",
                    accessKey: "n",
                    onClick: $"$p.moveCalendar('Next','{prefix}');",
                    icon: "ui-icon-seek-next")
                .Button(
                    text: Displays.Today(context: context),
                    controlCss: "button-icon",
                    onClick: $"$p.moveCalendar('Today','{prefix}');",
                    icon: "ui-icon-calendar")
                .FieldCheckBox(
                    controlId: "CalendarShowStatus",
                    fieldCss: "field-auto-thin",
                    controlCss: " auto-postback",
                    labelText: Displays.ShowStatus(context: context),
                    _checked: showStatus,
                    method: "post",
                    _using: prefix.IsNullOrEmpty())
                .Div(
                    attributes: new HtmlAttributes()
                        .Id($"{prefix}CalendarBody")
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
                            end: end,
                            CalendarViewType: CalendarViewType,
                            choices: choices,
                            dataRows: dataRows,
                            showStatus: showStatus,
                            inRange: inRange,
                            changedItemId: changedItemId,
                            calendarType: calendarType,
                            prefix: prefix,
                            calendarFromTo: calendarFromTo)));
            } else {
                return hb.Div(id: $"{prefix}Full", css: "both", action: () => hb
                .FieldDropDown(
                    context: context,
                    controlId: "CalendarFromTo",
                    fieldCss: "field-auto-thin",
                    controlCss: " auto-postback",
                    labelText: Displays.Column(context: context),
                    optionCollection: ss.CalendarColumnOptions(context: context),
                    selectedValue: toColumn == null
                        ? fromColumn?.ColumnName
                        : $"{fromColumn?.ColumnName}-{toColumn?.ColumnName}",
                    action: "Calendar",
                    method: "post",
                    _using: prefix.IsNullOrEmpty())
                .FieldCheckBox(
                    controlId: "CalendarShowStatus",
                    fieldCss: "field-auto-thin",
                    controlCss: " auto-postback",
                    labelText: Displays.ShowStatus(context: context),
                    _checked: showStatus,
                    method: "post",
                    _using: prefix.IsNullOrEmpty())
                .Div(
                    attributes: new HtmlAttributes()
                        .Id($"{prefix}FullCalendar")
                    )
                .Div(
                    attributes: new HtmlAttributes()
                        .Id($"{prefix}FullCalendarBody")
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
                            end: end,
                            CalendarViewType: CalendarViewType,
                            choices: choices,
                            dataRows: dataRows,
                            showStatus: showStatus,
                            inRange: inRange,
                            changedItemId: changedItemId,
                            calendarType: calendarType,
                            prefix: prefix,
                            calendarFromTo: calendarFromTo)));
            }
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
            DateTime end,
            string CalendarViewType,
            Dictionary<string, ControlData> choices,
            IEnumerable<DataRow> dataRows,
            bool showStatus,
            bool inRange,
            long changedItemId,
            string calendarType,
            string prefix,
            string calendarFromTo)
        {
            hb
                .Hidden(
                    controlId: $"{prefix}CalendarFromTo",
                    value: calendarFromTo?.ToString(),
                    _using: !prefix.IsNullOrEmpty())
                .Hidden(
                    controlId: $"{prefix}CalendarCanUpdate",
                    value: (fromColumn?.RecordedTime != true
                        && fromColumn?.GetEditorReadOnly() != true
                        && fromColumn?.CanUpdate(
                            context: context,
                            ss: ss,
                            mine: null) == true
                        && timePeriod != "Yearly"
                        && groupBy == null).ToOneOrZeroString())
                .Hidden(
                    controlId: $"{prefix}CalendarPrevious",
                    value: Times.PreviousCalendar(
                        context: context,
                        month: date,
                        timePeriod: timePeriod))
                .Hidden(
                    controlId: $"{prefix}CalendarNext",
                    value: Times.NextCalendar(
                        context: context,
                        month: date,
                        timePeriod: timePeriod))
                .Hidden(
                    controlId: $"{prefix}CalendarToday",
                    value: Times.Today(
                        context: context))
                .Hidden(
                    controlId: $"{prefix}CalendarFromDefaultInput",
                    value: fromColumn?.DefaultInput)
                .Hidden(
                    controlId: $"{prefix}CalendarToDefaultInput",
                    value: toColumn?.DefaultInput)
                .Hidden(
                    controlId: $"{prefix}CalendarType",
                    value: calendarType);
            if (calendarType == "Standard") { 
                return inRange
                    ? hb
                        .Hidden(
                            controlId: $"{prefix}CalendarTimePeriod",
                            value: timePeriod)
                        .Hidden(
                            controlId: $"{prefix}CalendarJson",
                            value: choices == null
                                ? Json(
                                    context: context,
                                    ss: ss,
                                    from: fromColumn,
                                    to: toColumn,
                                    dataRows: dataRows,
                                    changedItemId: changedItemId,
                                    showStatus: showStatus,
                                    calendarType: calendarType)
                                : GroupingJson(
                                    context: context,
                                    ss: ss,
                                    from: fromColumn,
                                    to: toColumn,
                                    groupBy: groupBy,
                                    dataRows: dataRows,
                                    changedItemId: changedItemId,
                                    showStatus: showStatus,
                                    calendarType: calendarType))
                        .CalendarBodyTable(
                            context: context,
                            timePeriod: timePeriod,
                            date: date,
                            begin: begin,
                            groupBy: groupBy,
                            choices: choices)
                    : hb;
            } else {
                return inRange
                    ? hb
                        .Hidden(
                            controlId: $"{prefix}CalendarStart",
                            value: begin.ToString()
                        )
                        .Hidden(
                            controlId: $"{prefix}CalendarEnd",
                            value: end.ToString()
                        )
                        .Hidden(
                            controlId: $"{prefix}CalendarViewType",
                            value: CalendarViewType
                        )
                        .Hidden(
                            controlId:$"{prefix}IsInit",
                            value: "True"
                        )
                        .Hidden(
                            controlId: $"{prefix}CalendarJson",
                            value: choices == null
                                ? Json(
                                    context: context,
                                    ss: ss,
                                    from: fromColumn,
                                    to: toColumn,
                                    dataRows: dataRows,
                                    changedItemId: changedItemId,
                                    showStatus: showStatus,
                                    calendarType: calendarType)
                                : GroupingJson(
                                    context: context,
                                    ss: ss,
                                    from: fromColumn,
                                    to: toColumn,
                                    groupBy: groupBy,
                                    dataRows: dataRows,
                                    changedItemId: changedItemId,
                                    showStatus: showStatus,
                                    calendarType: calendarType))
                    : hb;
            }
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
                                action: () => hb.Text(groupBy?.LabelText),
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
                                action: () => hb.Text(groupBy?.LabelText),
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
                                action: () => hb.Text(groupBy?.LabelText),
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
            SiteSettings ss,
            Column from,
            Column to,
            Column groupBy,
            IEnumerable<DataRow> dataRows,
            long changedItemId,
            bool showStatus,
            string calendarType)
        {
            if (calendarType == "Standard")
            {
                return dataRows
                    .GroupBy(
                        dataRow => dataRow.String(groupBy?.ColumnName),
                        dataRow =>
                            CreateCalendarElement(
                                context: context,
                                ss: ss,
                                from: from,
                                to: to,
                                changedItemId: changedItemId,
                                dataRow: dataRow,
                                showStatus: showStatus))
                    .Select(group => new
                    {
                        group = group.Key,
                        items = group
                            .OrderBy(o => o.From)
                            .ThenBy(o => o.To)
                            .ThenBy(o => o.UpdatedTime)
                    })
                    .ToJson();
            } else {
                return dataRows
                    .GroupBy(
                        dataRow => dataRow.String(groupBy?.ColumnName),
                        dataRow =>
                            CreateFullCalendarElement(
                                context: context,
                                ss: ss,
                                from: from,
                                to: to,
                                changedItemId: changedItemId,
                                dataRow: dataRow,
                                showStatus: showStatus))
                    .Select(group => new
                    {
                        group = group.Key,
                        items = group
                            .OrderBy(o => o.start)
                            .ThenBy(o => o.end)
                            .ThenBy(o => o.UpdatedTime)
                    })
                    .ToJson();
            }
        }

        private static string Json(
            Context context,
            SiteSettings ss,
            Column from,
            Column to,
            IEnumerable<DataRow> dataRows,
            long changedItemId,
            bool showStatus,
            string calendarType)
        {
            if (calendarType == "Standard") {
                return new[] { new
                {
                    group = (string)null,
                    items = dataRows.Select(dataRow =>
                        CreateCalendarElement(
                            context: context,
                            ss: ss,
                            from: from,
                            to: to,
                            dataRow: dataRow,
                            changedItemId: changedItemId,
                            showStatus: showStatus))
                                .OrderBy(o => o.From)
                                .ThenBy(o => o.To)
                                .ThenBy(o => o.UpdatedTime)
                }}
                .ToJson();
            } else {
                return new[] { new
                {
                    group = (string)null,
                    items = dataRows.Select(dataRow =>
                        CreateFullCalendarElement(
                            context: context,
                            ss: ss,
                            from: from,
                            to: to,
                            dataRow: dataRow,
                            changedItemId: changedItemId,
                            showStatus: showStatus))
                                .OrderBy(o => o.start)
                                .ThenBy(o => o.end)
                                .ThenBy(o => o.UpdatedTime)
                }}
                .ToJson();
            }
        }

        private static CalendarElement CreateCalendarElement(
            Context context,
            SiteSettings ss,
            Column from,
            Column to,
            DataRow dataRow,
            long changedItemId,
            bool showStatus)
        {
            return new CalendarElement(
                id: dataRow.Long("Id"),
                title: dataRow.String("ItemTitle"),
                time: (from?.EditorFormat == "Ymdhm"
                    ? dataRow.DateTime("From").ToLocal(context: context).ToString("HH:mm") + " "
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
                updatedTime: dataRow.DateTime("UpdatedTime"),
                statusHtml: ElementStatus(
                    context: context,
                    ss: ss,
                    status: new Status(dataRow.Int("Status")),
                    showStatus: showStatus));
        }

        private static FullCalendarElement CreateFullCalendarElement(
            Context context,
            SiteSettings ss,
            Column from,
            Column to,
            DataRow dataRow,
            long changedItemId,
            bool showStatus)
        {
            return new FullCalendarElement(
                Id: dataRow.Long("Id"),
                Title: dataRow.String("ItemTitle"),
                Time: (from?.EditorFormat == "Ymdhm"
                    ? dataRow.DateTime("From").ToLocal(context: context).ToString("HH:mm") + " "
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
                updatedTime: dataRow.DateTime("UpdatedTime"),
                statusHtml: ElementStatus(
                    context: context,
                    ss: ss,
                    status: new Status(dataRow.Int("Status")),
                    showStatus: showStatus));
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

        private static string ElementStatus(
            Context context,
            SiteSettings ss,
            Status status,
            bool showStatus)
        {
            if (!showStatus) return null;
            var column = ss.GetColumn(
                context: context,
                columnName: "Status");
            return status?.StyleBody(
                hb: new HtmlBuilder(),
                column: column,
                tag: "SPAN").ToString();
        }
    }
}