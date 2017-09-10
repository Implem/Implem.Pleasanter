using Implem.DefinitionAccessor;
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
    public static class HtmlGantts
    {
        public static HtmlBuilder Gantt(
            this HtmlBuilder hb,
            SiteSettings ss,
            string groupBy,
            string sortBy,
            int period,
            DateTime startDate,
            GanttRange range,
            EnumerableRowCollection<DataRow> dataRows,
            bool inRange = true)
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
                    optionCollection: ss.GanttSortByOptions(),
                    selectedValue: sortBy,
                    insertBlank: true,
                    method: "post")
                .Div(css: "field-auto-thin", action: () => hb
                    .FieldTextBox(
                        textType: HtmlTypes.TextTypes.DateTime,
                        fieldCss: "field-auto-thin",
                        controlId: "GanttStartDate",
                        controlCss: " w100 auto-postback",
                        labelText: Displays.StartDate(),
                        text: startDate.ToLocal().InRange()
                            ? startDate.ToLocal().ToString(
                                "d", Sessions.CultureInfo())
                            : string.Empty,
                        format: Displays.YmdDatePickerFormat(),
                        method: "post")
                    .Button(
                        controlId: "GanttPreviousButton",
                        text: Displays.Previous(),
                        controlCss: "button-icon",
                        accessKey: "b",
                        onClick: "$p.moveGantt('Previous');",
                        icon: "ui-icon-seek-prev")
                    .Button(
                        controlId: "GanttNextButton",
                        text: Displays.Next(),
                        controlCss: "button-icon",
                        accessKey: "n",
                        onClick: "$p.moveGantt('Next');",
                        icon: "ui-icon-seek-next")
                    .Button(
                        controlId: "GanttFirstDayButton",
                        text: Displays.FirstDay(),
                        controlCss: "button-icon",
                        onClick: "$p.moveGantt('FirstDay');",
                        icon: "ui-icon-calendar")
                    .Button(
                        controlId: "GanttTodayButton",
                        text: Displays.Today(),
                        controlCss: "button-icon",
                        onClick: "$p.moveGantt('Today');",
                        icon: "ui-icon-calendar"))
                .FieldSlider(
                    controlId: "GanttPeriod",
                    fieldCss: "field-auto-thin",
                    controlCss: " auto-postback",
                    labelText: Displays.Period(),
                    min: Parameters.General.GanttPeriodMin,
                    max: range.Period,
                    step: 1,
                    value: period,
                    method: "post")
                .Div(id: "GanttBody", action: () => hb
                    .GanttBody(
                        ss: ss,
                        groupBy: groupBy,
                        sortBy: sortBy,
                        period: period,
                        startDate: startDate,
                        range: range,
                        dataRows: dataRows,
                        inRange: inRange)));
        }

        private static Dictionary<string, ControlData> GanttDays(
            EnumerableRowCollection<DataRow> dataRows)
        {
            var now = DateTime.Now;
            var month = new DateTime(now.ToLocal().Year, now.ToLocal().Month, 1);
            return Enumerable.Range(
                1,
                100)
                    .ToDictionary(
                        o => month.AddDays(o).ToString(),
                        o => new ControlData(month.AddDays(o).ToString(
                            "Y", Sessions.CultureInfo())));
        }

        public static HtmlBuilder GanttBody(
            this HtmlBuilder hb,
            SiteSettings ss,
            string groupBy,
            string sortBy,
            int period,
            DateTime startDate,
            GanttRange range,
            EnumerableRowCollection<DataRow> dataRows,
            bool inRange)
        {
            hb
                .Hidden(
                    controlId: "GanttMinDate",
                    value: startDate.ToLocal().ToString(
                        "d", Sessions.CultureInfo()))
                .Hidden(
                    controlId: "GanttMaxDate",
                    value: startDate.AddDays(period).ToLocal().ToString(
                        "d", Sessions.CultureInfo()))
                .Hidden(
                    controlId: "GanttPrevious",
                    value: startDate.AddDays(-7).ToLocal().ToString(
                        "d", Sessions.CultureInfo()))
                .Hidden(
                    controlId: "GanttNext",
                    value: startDate.AddDays(7).ToLocal().ToString(
                        "d", Sessions.CultureInfo()))
                .Hidden(
                    controlId: "GanttFirstDay",
                    value: range.Min.ToLocal().ToString(
                        "d", Sessions.CultureInfo()))
                .Hidden(
                    controlId: "GanttToday",
                    value: DateTime.Today.ToLocal().ToString(
                        "d", Sessions.CultureInfo()))
                .Hidden(
                    controlId: "ShowGanttProgressRate",
                    value: ss.ShowGanttProgressRate.ToBool().ToOneOrZeroString());
            if (!inRange) return hb;
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