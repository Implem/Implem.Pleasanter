using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataViews;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
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
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            IEnumerable<DataRow> dataRows,
            string ownerLabelText,
            Column column)
        {
            var burnDown = new BurnDown(siteSettings, dataRows);
            return hb
                .Body(burnDown: burnDown)
                .Details(
                    burnDown: burnDown,
                    siteSettings: siteSettings,
                    ownerLabelText: ownerLabelText,
                    column: column)
                .MainCommands(
                    siteId: siteSettings.SiteId,
                    permissionType: permissionType,
                    verType: Versions.VerTypes.Latest,
                    backUrl: Navigations.ItemIndex(siteSettings.ParentId),
                    importButton: true,
                    exportButton: true);
        }

        private static HtmlBuilder Body(this HtmlBuilder hb, BurnDown burnDown)
        {
            return hb.Div(css: "burn-down-body", action: () => hb
                .Svg(
                    id: "BurnDown",
                    css: "burn-down",
                    action: () => { })
                .Hidden(
                    controlId: "BurnDownJson",
                    value: burnDown.Json()));
        }

        private static HtmlBuilder Details(
            this HtmlBuilder hb,
            BurnDown burnDown,
            SiteSettings siteSettings,
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
                    .Distinct();
                var colspan = updators.Count() + 1;
                var minTime = burnDown.MinTime;
                var updatedMaxTime = burnDown.LatestUpdatedTime;
                var count = Times.DateDiff(Times.Types.Days, minTime, updatedMaxTime);
                hb.Table(css: "grid", action: () =>
                {
                    hb.THead(action: () => hb
                        .DetailsHeader(
                            burnDown: burnDown,
                            updators: updators,
                            ownerLabelText: ownerLabelText,
                            column: column));
                    hb.TBody(action: () =>
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
                            }
                            if (d == count)
                            {
                                hb.BurnDownRecordDetails(
                                    elements: burnDown
                                        .Where(o => o.UpdatedTime == updatedMaxTime),
                                    column: siteSettings.GetColumn("ProgressRate"),
                                    colspan: updators.Count() + 5,
                                    unit: column.Unit);
                            }
                        }
                    });
                });
            }
            return hb;
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
                            .Text(text: SiteInfo.User(updatorId).FullName() +
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
                    .Class("grid-row not-link burn-down-details-row")
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
            this HtmlBuilder hb, decimal value, string unit, string css = "")
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
            Column column,
            int colspan,
            string unit)
        {
            return hb.Tr(
                attributes: new HtmlAttributes()
                    .Class("grid-row not-link burn-down-record-details"),
                action: () => hb
                    .Td(attributes: new HtmlAttributes().Colspan(colspan),
                        action: () => elements
                            .Select(o => o.Updator)
                            .Distinct()
                            .ForEach(updatorId =>
                                hb.BurnDownRecordDetail(
                                    fullName: SiteInfo.User(updatorId).FullName(),
                                    updatorId: updatorId,
                                    earndValue: elements
                                        .Where(o => o.Updator == updatorId)
                                        .Select(o => o.EarnedValueAdditions)
                                        .Sum(),
                                    elements: elements,
                                    column: column,
                                    unit: unit))));
        }

        private static HtmlBuilder BurnDownRecordDetail(
            this HtmlBuilder hb,
            int updatorId,
            string fullName,
            decimal earndValue,
            IEnumerable<BurnDownElement> elements,
            Column column,
            string unit)
        {
            if (earndValue != 0)
            {
                hb.Div(css: "user-info", action: () => hb
                    .Text("{0} ({1}{2})".Params(
                        fullName,
                        column.Display(earndValue),
                        unit)));
                elements
                    .Where(o => o.Updator == updatorId)
                    .Where(o => o.ProgressRateAdditions != 0)
                    .OrderByDescending(o => o.EarnedValueAdditions)
                    .ForEach(element => hb
                        .Div(css: "items", action: () => hb
                            .A(
                                href: Navigations.ItemEdit(element.Id),
                                text: "{0}{1} * {2}% = {3}{1} : {4} - {5}".Params(
                                    column.Display(element.WorkValue),
                                    unit,
                                    column.Display(element.ProgressRateAdditions),
                                    column.Display(element.EarnedValueAdditions),
                                    element.Title,
                                    column.Choice(element.Status.ToString())
                                        .Text()))));
            }
            return hb;
        }
    }
}