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
    public static class HtmlGantts
    {
        public static HtmlBuilder Gantt(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Column groupBy,
            Column sortBy,
            int period,
            DateTime startDate,
            GanttRange range,
            EnumerableRowCollection<DataRow> dataRows,
            bool inRange = true)
        {
            return hb.Div(css: "both", action: () => hb
                .FieldDropDown(
                    context: context,
                    controlId: "GanttGroupBy",
                    fieldCss: "field-auto-thin",
                    controlCss: " auto-postback",
                    labelText: Displays.GroupBy(context: context),
                    optionCollection: ss.GanttGroupByOptions(),
                    selectedValue: groupBy?.ColumnName,
                    insertBlank: true,
                    method: "post")
                .FieldDropDown(
                    context: context,
                    controlId: "GanttSortBy",
                    fieldCss: "field-auto-thin",
                    controlCss: " auto-postback",
                    labelText: Displays.SortBy(context: context),
                    optionCollection: ss.GanttSortByOptions(context: context),
                    selectedValue: sortBy?.ColumnName,
                    insertBlank: true,
                    method: "post")
                .Div(css: "field-auto-thin", action: () => hb
                    .FieldTextBox(
                        textType: HtmlTypes.TextTypes.DateTime,
                        fieldCss: "field-auto-thin",
                        controlId: "GanttStartDate",
                        controlCss: " w100 auto-postback",
                        labelText: Displays.StartDate(context: context),
                        text: startDate.ToLocal(context: context).InRange()
                            ? startDate.ToLocal(context: context).ToString(
                                "d", context.CultureInfo())
                            : string.Empty,
                        format: Displays.YmdDatePickerFormat(context: context),
                        method: "post")
                    .Button(
                        controlId: "GanttPreviousButton",
                        text: Displays.Previous(context: context),
                        controlCss: "button-icon",
                        accessKey: "b",
                        onClick: "$p.moveGantt('Previous');",
                        icon: "ui-icon-seek-prev")
                    .Button(
                        controlId: "GanttNextButton",
                        text: Displays.Next(context: context),
                        controlCss: "button-icon",
                        accessKey: "n",
                        onClick: "$p.moveGantt('Next');",
                        icon: "ui-icon-seek-next")
                    .Button(
                        controlId: "GanttFirstDayButton",
                        text: Displays.FirstDay(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.moveGantt('FirstDay');",
                        icon: "ui-icon-calendar")
                    .Button(
                        controlId: "GanttTodayButton",
                        text: Displays.Today(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.moveGantt('Today');",
                        icon: "ui-icon-calendar"))
                .FieldSlider(
                    controlId: "GanttPeriod",
                    fieldCss: "field-auto-thin",
                    controlCss: " auto-postback",
                    labelText: Displays.Period(context: context),
                    min: Parameters.General.GanttPeriodMin,
                    max: range.Period,
                    step: 1,
                    value: period,
                    method: "post")
                .Div(id: "GanttBody", action: () => hb
                    .GanttBody(
                        context: context,
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
            Context context, EnumerableRowCollection<DataRow> dataRows)
        {
            var now = DateTime.Now;
            var month = new DateTime(
                year: now.ToLocal(context: context).Year,
                month: now.ToLocal(context: context).Month,
                day: 1);
            return Enumerable.Range(
                1,
                100)
                    .ToDictionary(
                        o => month.AddDays(o).ToString(),
                        o => new ControlData(month.AddDays(o).ToString(
                            "Y", context.CultureInfo())));
        }

        public static HtmlBuilder GanttBody(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Column groupBy,
            Column sortBy,
            int period,
            DateTime startDate,
            GanttRange range,
            EnumerableRowCollection<DataRow> dataRows,
            bool inRange)
        {
            hb
                .Hidden(
                    controlId: "GanttMinDate",
                    value: startDate.ToLocal(context: context).ToString(
                        "d", context.CultureInfo()))
                .Hidden(
                    controlId: "GanttMaxDate",
                    value: startDate.AddDays(period).ToLocal(context: context).ToString(
                        "d", context.CultureInfo()))
                .Hidden(
                    controlId: "GanttPrevious",
                    value: startDate.AddDays(-7).ToLocal(context: context).ToString(
                        "d", context.CultureInfo()))
                .Hidden(
                    controlId: "GanttNext",
                    value: startDate.AddDays(7).ToLocal(context: context).ToString(
                        "d", context.CultureInfo()))
                .Hidden(
                    controlId: "GanttFirstDay",
                    value: range.Min.ToLocal(context: context).ToString(
                        "d", context.CultureInfo()))
                .Hidden(
                    controlId: "GanttToday",
                    value: DateTime.Today.ToLocal(context: context).ToString(
                        "d", context.CultureInfo()))
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
                        value: new Gantt(
                            context: context,
                            ss: ss,
                            dataRows: dataRows,
                            groupBy: groupBy,
                            sortBy: sortBy)
                                .Json(groupBy, sortBy))
                : hb;
        }
    }
}