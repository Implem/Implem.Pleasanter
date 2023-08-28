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
            Dictionary<string, ControlData> choices,
            IEnumerable<DataRow> dataRows,
            bool showStatus,
            bool inRange,
            long changedItemId)
        {
            if (ss.CalendarType.ToString() == "Standard")
            {
                return hb.Div(id: "Calendar", css: "both", action: () => hb
                .FieldDropDown(
                    context: context,
                    controlId: "CalendarGroupBy",
                    fieldCss: "field-auto-thin",
                    controlCss: " auto-postback",
                    labelText: Displays.GroupBy(context: context),
                    optionCollection: ss.CalendarGroupByOptions(context: context),
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
                        ? fromColumn?.ColumnName
                        : $"{fromColumn?.ColumnName}-{toColumn?.ColumnName}",
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
                .FieldCheckBox(
                    controlId: "CalendarShowStatus",
                    fieldCss: "field-auto-thin",
                    controlCss: " auto-postback",
                    labelText: Displays.ShowStatus(context: context),
                    _checked: showStatus,
                    method: "post")
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
                            showStatus: showStatus,
                            inRange: inRange,
                            changedItemId: changedItemId)));
            } else {
                return hb.Div(id: "Calendar", css: "both", action: () => hb
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
                    method: "post")
                .FieldCheckBox(
                    controlId: "CalendarShowStatus",
                    fieldCss: "field-auto-thin",
                    controlCss: " auto-postback",
                    labelText: Displays.ShowStatus(context: context),
                    _checked: showStatus,
                    method: "post")
                .Div(
                    attributes: new HtmlAttributes()
                        .Id("FullCalendar")
                    )
                .Div(
                    attributes: new HtmlAttributes()
                        .Id("FullCalendarBody")
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
                            showStatus: showStatus,
                            inRange: inRange,
                            changedItemId: changedItemId)));
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
            Dictionary<string, ControlData> choices,
            IEnumerable<DataRow> dataRows,
            bool showStatus,
            bool inRange,
            long changedItemId)
        {
            hb
                .Hidden(
                    controlId: "CalendarCanUpdate",
                    value: (fromColumn?.RecordedTime != true
                        && fromColumn?.GetEditorReadOnly() != true
                        && fromColumn?.CanUpdate(
                            context: context,
                            ss: ss,
                            mine: null) == true
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
                    value: fromColumn?.DefaultInput)
                .Hidden(
                    controlId: "CalendarToDefaultInput",
                    value: toColumn?.DefaultInput)
                .Hidden(
                    controlId: "CalendarType",
                    value: ss.CalendarType.ToString());
            if (ss.CalendarType.ToString() == "Standard") { 
                return inRange
                    ? hb
                        .Hidden(
                            controlId: "CalendarJson",
                            value: choices == null
                                ? Json(
                                    context: context,
                                    ss: ss,
                                    from: fromColumn,
                                    to: toColumn,
                                    dataRows: dataRows,
                                    changedItemId: changedItemId,
                                    showStatus: showStatus)
                                : GroupingJson(
                                    context: context,
                                    ss: ss,
                                    from: fromColumn,
                                    to: toColumn,
                                    groupBy: groupBy,
                                    dataRows: dataRows,
                                    changedItemId: changedItemId,
                                    showStatus: showStatus))
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
                            controlId: "CalendarStart",
                            value: context.Forms.ContainsKey("CalendarStart") ? context.Forms["CalendarStart"].ToString() : ""
                        )
                        .Hidden(
                            controlId: "CalendarEnd",
                            value: context.Forms.ContainsKey("CalendarEnd") ? context.Forms["CalendarEnd"].ToString() : ""
                        )
                        .Hidden(
                            controlId: "CalendarDate",
                            value: context.Forms.ContainsKey("CalendarDate") ? context.Forms["CalendarDate"].ToString() : ""
                        )
                        .Hidden(
                            controlId: "CalendarViewType",
                            value: context.Forms.ContainsKey("CalendarViewType") ? context.Forms["CalendarViewType"].ToString() : ""
                        )
                        .Hidden(
                            controlId: "CalendarTimePeriod",
                            value: context.Forms.ContainsKey("CalendarTimePeriod") ? context.Forms["CalendarTimePeriod"].ToString() : ""
                        )
                        .Hidden(
                            controlId:"IsInit",
                            value: "True"
                        )
                        .Hidden(
                            controlId: "CalendarJson",
                            value: choices == null
                                ? Json(
                                    context: context,
                                    ss: ss,
                                    from: fromColumn,
                                    to: toColumn,
                                    dataRows: dataRows,
                                    changedItemId: changedItemId,
                                    showStatus: showStatus)
                                : GroupingJson(
                                    context: context,
                                    ss: ss,
                                    from: fromColumn,
                                    to: toColumn,
                                    groupBy: groupBy,
                                    dataRows: dataRows,
                                    changedItemId: changedItemId,
                                    showStatus: showStatus))
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
            bool showStatus)
        {
            if (ss.CalendarType.ToString() == "Standard")
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
            bool showStatus)
        {
            if (ss.CalendarType.ToString() == "Standard") {
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