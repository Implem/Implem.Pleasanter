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
    public static class HtmlBurnDown
    {
        public static HtmlBuilder BurnDown(
            this HtmlBuilder hb,
            SiteSettings ss,
            IEnumerable<DataRow> dataRows,
            string ownerLabelText,
            Column column)
        {
            var burnDown = new BurnDown(ss, dataRows);
            return hb
                .Body(burnDown: burnDown)
                .Details(
                    burnDown: burnDown,
                    ss: ss,
                    ownerLabelText: ownerLabelText,
                    column: column);
        }

        private static HtmlBuilder Body(this HtmlBuilder hb, BurnDown burnDown)
        {
            return hb.Div(action: () => hb
                .Svg(id: "BurnDown")
                .Hidden(
                    controlId: "BurnDownJson",
                    value: burnDown.Json()));
        }

        private static HtmlBuilder Details(
            this HtmlBuilder hb,
            BurnDown burnDown,
            SiteSettings ss,
            string ownerLabelText,
            Column column)
        {
            if (burnDown.Select(o => o.EarnedValueAdditions).Sum() > 0)
            {
                var updators = burnDown
                    .Where(o => o.EarnedValueAdditions > 0)
                    .Select(o => o.Updator)
                    .OrderByDescending(o => burnDown
                        .Where(p => p.Updator == o)
                        .Select(p => p.EarnedValueAdditions)
                        .Sum())
                    .Distinct()
                    .ToList();
                hb.Table(id: "BurnDownDetails", css: "grid", action: () => hb
                    .THead(action: () => hb.DetailsHeader(
                        burnDown: burnDown,
                        updators: updators,
                        ownerLabelText: ownerLabelText,
                        column: column))
                    .DetailsBody(
                        burnDown: burnDown,
                        ss: ss,
                        updators: updators,
                        column: column));
            }
            return hb;
        }

        private static HtmlBuilder DetailsBody(
            this HtmlBuilder hb,
            BurnDown burnDown,
            SiteSettings ss,
            IEnumerable<int> updators,
            Column column)
        {
            var colspan = updators.Count() + 1;
            var minTime = burnDown.MinTime;
            var updatedMaxTime = burnDown.LatestUpdatedTime;
            var count = Times.DateDiff(Times.Types.Days, minTime, updatedMaxTime);
            var first = true;
            return hb.TBody(action: () =>
            {
                for (var d = count; d >= 0; d--)
                {
                    var currentTime = minTime.AddDays(d);
                    if (burnDown.Any(o =>
                        o.UpdatedTime == currentTime &&
                        o.EarnedValueAdditions != 0))
                    {
                        hb.DetailsRow(
                            burnDown: burnDown,
                            updators: updators,
                            currentTime: currentTime,
                            column: column);
                        if (first)
                        {
                            hb.BurnDownRecordDetails(
                                elements: burnDown.Where(o => o.UpdatedTime == currentTime),
                                progressRateColumn: ss.GetColumn("ProgressRate"),
                                statusColumn: ss.GetColumn("Status"),
                                colspan: updators.Count() + 5,
                                unit: column.Unit);
                            first = false;
                        }
                    }
                }
            });
        }

        private static HtmlBuilder DetailsHeader(
            this HtmlBuilder hb,
            BurnDown burnDown,
            IEnumerable<int> updators,
            string ownerLabelText,
            Column column)
        {
            return hb.Tr(css: "ui-widget-header", action: () =>
            {
                hb
                    .Th(action: () => hb
                        .Text(text: Displays.Date()))
                    .Th(action: () => hb
                        .Text(text: Displays.PlannedValue()))
                    .Th(action: () => hb
                        .Text(text: Displays.EarnedValue()))
                    .Th(action: () => hb
                        .Text(text: Displays.Difference()))
                    .Th(action: () => hb
                        .Text(text: ownerLabelText + " " + Displays.Total()));
                updators.ForEach(updatorId => hb
                    .Th(action: () => hb
                        .Text(text: SiteInfo.User(updatorId).Name +
                            " ({0})".Params(column.Display(burnDown
                                .Where(p => p.Updator == updatorId)
                                .Select(p => p.EarnedValueAdditions)
                                .Sum()) + column.Unit))));
            });
        }

        private static HtmlBuilder DetailsRow(
            this HtmlBuilder hb,
            BurnDown burnDown,
            IEnumerable<int> updators,
            DateTime currentTime,
            Column column)
        {
            return hb.Tr(
                attributes: new HtmlAttributes()
                    .Class("grid-row not-link")
                    .Add("data-date", currentTime.ToShortDateString())
                    .DataAction("BurnDownRecordDetails")
                    .DataMethod("post"),
                action: () =>
                {
                    var targets = burnDown.Targets(currentTime);
                    var total = targets.Select(o => o.WorkValue).Sum();
                    var planned = targets.Select(o => o.PlannedValue(currentTime)).Sum();
                    var earned = total - targets.Select(o => o.EarnedValue).Sum();
                    var difference = planned - earned;
                    hb
                        .Td(action: () => hb
                            .Text(text: "{0} - {1}".Params(
                                currentTime.AddDays(-1).ToString(
                                    Displays.Get("YmdaFormat"),
                                    Sessions.CultureInfo()),
                                currentTime.ToString(
                                    Displays.Get("YmdaFormat"),
                                    Sessions.CultureInfo()))))
                        .Td(action: () => hb
                            .Text(text: column.Display(planned) + column.Unit))
                        .Td(action: () => hb
                            .Text(text: column.Display(earned) + column.Unit))
                        .Td(value: difference, unit: column.Unit, css: "difference")
                        .Td(value: burnDown
                                .Where(o => o.UpdatedTime == currentTime)
                                .Select(o => o.EarnedValueAdditions)
                                .Sum(),
                            unit: column.Unit);
                    updators.ForEach(updator => hb
                        .Td(
                            value: burnDown
                                .Where(o => o.UpdatedTime == currentTime)
                                .Where(o => o.Updator == updator)
                                .Select(o => o.EarnedValueAdditions)
                                .Sum(),
                            unit: column.Unit));
                });
        }

        private static HtmlBuilder Td(
            this HtmlBuilder hb, decimal value, string unit, string css = null)
        {
            return hb.Td(
                css: css + (value <= 0
                    ? " warning"
                    : string.Empty),
                action: () => hb
                    .Text(text: value.ToString(
                        intFormat: "+0;-0",
                        decimalFormat: "+0.0;-0.0") + unit));
        }

        public static HtmlBuilder BurnDownRecordDetails(
            this HtmlBuilder hb,
            IEnumerable<BurnDownElement> elements,
            Column progressRateColumn,
            Column statusColumn,
            int colspan,
            string unit)
        {
            return hb.Tr(
                attributes: new HtmlAttributes()
                    .Class("grid-row not-link items"),
                action: () => hb
                    .Td(attributes: new HtmlAttributes().Colspan(colspan),
                        action: () => elements
                            .Select(o => o.Updator)
                            .Distinct()
                            .ForEach(updatorId =>
                                hb.BurnDownRecordDetail(
                                    fullName: SiteInfo.User(updatorId).Name,
                                    updatorId: updatorId,
                                    earndValue: elements
                                        .Where(o => o.Updator == updatorId)
                                        .Select(o => o.EarnedValueAdditions)
                                        .Sum(),
                                    elements: elements,
                                    progressRateColumn: progressRateColumn,
                                    statusColumn: statusColumn,
                                    unit: unit))));
        }

        private static HtmlBuilder BurnDownRecordDetail(
            this HtmlBuilder hb,
            int updatorId,
            string fullName,
            decimal earndValue,
            IEnumerable<BurnDownElement> elements,
            Column progressRateColumn,
            Column statusColumn,
            string unit)
        {
            if (earndValue != 0)
            {
                hb.Div(css: "user-info", action: () => hb
                    .Text("{0} ({1}{2})".Params(
                        fullName,
                        progressRateColumn.Display(earndValue),
                        unit)));
                elements
                    .Where(o => o.Updator == updatorId)
                    .Where(o => o.ProgressRateAdditions != 0)
                    .OrderByDescending(o => o.EarnedValueAdditions)
                    .ForEach(element => hb
                        .Div(css: "items", action: () => hb
                            .BurnDownRecordDetailAnchor(
                                element: element,
                                progressRateColumn: progressRateColumn,
                                statusColumn: statusColumn,
                                unit: unit)));
            }
            return hb;
        }

        private static HtmlBuilder BurnDownRecordDetailAnchor(
            this HtmlBuilder hb,
            BurnDownElement element,
            Column progressRateColumn,
            Column statusColumn,
            string unit)
        {
            return hb.A(
                href: Locations.ItemEdit(element.Id),
                text: "{0}{1} * {2}% = {3}{1} : {4} - {5}".Params(
                    progressRateColumn.Display(element.WorkValue),
                    unit,
                    progressRateColumn.Display(element.ProgressRateAdditions),
                    progressRateColumn.Display(element.EarnedValueAdditions),
                    element.Title,
                    statusColumn.Choice(element.Status.ToString())
                        .Text));
        }
    }
}