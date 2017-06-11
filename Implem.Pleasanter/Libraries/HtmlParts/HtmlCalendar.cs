using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
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
            string columnName,
            DateTime month,
            DateTime begin,
            IEnumerable<DataRow> dataRows)
        {
            return hb.Div(id: "Calendar", css: "both", action: () => hb
                .FieldDropDown(
                    controlId: "CalendarColumn",
                    fieldCss: "field-auto-thin",
                    controlCss: " auto-postback",
                    labelText: Displays.Column(),
                    optionCollection: ss.CalendarColumnOptions(),
                    selectedValue: columnName,
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
                .CalendarBody(
                    ss: ss,
                    column: ss.GetColumn(columnName),
                    month: month,
                    begin: begin,
                    dataRows: dataRows));
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
            Column column,
            DateTime month,
            DateTime begin,
            IEnumerable<DataRow> dataRows)
        {
            var data = dataRows
                .Select(o => new CalendarElement
                (
                    o["Id"].ToLong(),
                    (ss.GetColumn(column.ColumnName).EditorFormat == "Ymdhm"
                        ? o["Date"].ToDateTime().ToLocal().ToString("t") + " "
                        : string.Empty) + new Title(ss, o, "Id").DisplayValue,
                    column.ColumnName == "CompletionTime"
                        ? o["Date"].ToDateTime().ToLocal().AddDays(-1)
                        : o["Date"].ToDateTime().ToLocal()
                ))
                .OrderBy(o => o.Time)
                .ThenBy(o => o.Title)
                .GroupBy(o => o.Date)
                .ToDictionary(
                    o => o.First().Date,
                    o => o.Select(p => p));
            return hb
                .Table(
                    attributes: new HtmlAttributes()
                        .Id("CalendarBody")
                        .DataAction("UpdateByCalendar")
                        .DataMethod("post"), 
                    action: () =>
                    {
                        hb.THead(action: () => hb
                            .Tr(action: () =>
                            {
                                for (var x = 0; x < 7; x++)
                                {
                                    hb.Th(css: DayOfWeekCss(x), action: () => hb
                                        .Text(text: DayOfWeekString(x)));
                                }
                            }));
                        hb.TBody(action: () =>
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
                                                .DataId(date.ToString()),
                                            action: () => hb
                                                .Items(
                                                    month: month,
                                                    date: date,
                                                    data: data));
                                    }
                                });
                            }
                        });
                    })
                .Hidden(
                    controlId: "CalendarCanUpdate",
                    value: (
                        !column.RecordedTime &&
                        column.EditorReadOnly != true &&
                        column.CanUpdate).ToOneOrZeroString())
                .Hidden(controlId: "CalendarPrevious", value: Times.PreviousMonth(month))
                .Hidden(controlId: "CalendarNext", value: Times.NextMonth(month))
                .Hidden(controlId: "CalendarThisMonth", value: Times.ThisMonth());
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

        private static HtmlBuilder Items(
            this HtmlBuilder hb,
            DateTime month,
            DateTime date,
            Dictionary<DateTime, IEnumerable<CalendarElement>> data)
        {
            hb.Div(css: "day", action: () =>
            {
                hb.Div(action: () => hb
                    .Text(date.Day.ToString()));
                data
                    .FirstOrDefault(o => o.Key == date)
                    .Value?
                    .ForEach(element => hb
                    .Div(
                        attributes: new HtmlAttributes()
                            .Class( "item")
                            .Title(element.Title)
                            .DataId(element.Id.ToString())
                            .DataValue(element.Time.ToString()),
                        action: () => hb
                            .Span(css: "ui-icon ui-icon-pencil")
                            .Text(text: element.Title)));
            });
            return hb;
        }
    }
}