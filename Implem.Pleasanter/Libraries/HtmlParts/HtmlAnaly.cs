using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.ViewModes;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlAnaly
    {
        public static HtmlBuilder Analy(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            List<AnalyData> analyDataList,
            bool inRange)
        {
            return hb.Div(css: "both", action: () => hb
                .Button(
                    controlId: "OpenAnalyPartDialog",
                    text: Displays.Add(context: context),
                    controlCss: "button-icon button-positive",
                    onClick: "$p.openAnalyPartDialog($(this));",
                    icon: "ui-icon-plus",
                    action: "OpenAnalyPartDialog",
                    method: "post")
                .Div(id: "AnalyBody", action: () => hb
                    .AnalyBody(
                        context: context,
                        ss: ss,
                        analyDataList: analyDataList,
                        inRange: inRange)));
        }

        public static HtmlBuilder AnalyPartDialog(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
            int analyPartId)
        {
            var analyPart = view.AnalyPartSettings?.FirstOrDefault(o => o.Id == analyPartId)
                ?? new AnalyPartSetting()
                {
                    Id = 0,
                    GroupBy = ss.AnalyGroupByOptions(context: context).FirstOrDefault().Key,
                    TimePeriodValue = 0,
                    TimePeriod = "DaysAgoNoArgs",
                    AggregationType = "Count"
                };
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("AnalyPartDialogForm")
                    .Action(Locations.ItemAction(
                        context: context,
                        id: ss.SiteId)),
                action: () => hb
                    .FieldText(
                        controlId: "AnalyPartId",
                        controlCss: " always-send",
                        labelText: Displays.Id(context: context),
                        text: analyPart.Id.ToString())
                    .FieldDropDown(
                        context: context,
                        controlId: "AnalyPartGroupBy",
                        fieldCss: "field-normal",
                        controlCss: " always-send",
                        labelText: Displays.Column(context: context),
                        optionCollection: ss.AnalyGroupByOptions(context: context),
                        selectedValue: analyPart.GroupBy)
                    .FieldSpinner(
                        fieldId: "AnalyPartTimePeriodValueField",
                        controlId: "AnalyPartTimePeriodValue",
                        fieldCss: "field-normal",
                        controlCss: " always-send",
                        labelText: Displays.Value(context: context),
                        value: analyPart.TimePeriodValue,
                        min: Parameters.General.AnalyPartPeriodValueMin,
                        max: Parameters.General.AnalyPartPeriodValueMax,
                        step: 1,
                        width: 70)
                    .FieldDropDown(
                        context: context,
                        fieldId: "AnalyPartTimePeriodField",
                        controlId: "AnalyPartTimePeriod",
                        fieldCss: "field-normal",
                        controlCss: " always-send",
                        labelText: Displays.Period(context: context),
                        optionCollection: ss.AnalyPeriodOptions(context: context),
                        selectedValue: analyPart.TimePeriod)
                    .FieldDropDown(
                        context: context,
                        controlId: "AnalyPartAggregationType",
                        controlCss: " always-send",
                        labelText: Displays.AggregationType(context: context),
                        optionCollection: ss.AnalyAggregationTypeOptions(context: context),
                        selectedValue: analyPart.AggregationType)
                    .FieldDropDown(
                        context: context,
                        controlId: "AnalyPartAggregationTarget",
                        controlCss: " always-send",
                        labelText: Displays.AggregationTarget(context: context),
                        optionCollection: ss.AnalyAggregationTargetOptions(context: context),
                        selectedValue: analyPart.AggregationTarget,
                        insertBlank: true)
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            text: Displays.Add(context: context),
                            controlId: "AddAnalyPart",
                            controlCss: "button-icon button-positive",
                            onClick: "$p.send($(this));",
                            method: "post",
                            icon: "ui-icon-disk")
                        .Button(
                            text: Displays.Cancel(context: context),
                            controlCss: "button-icon button-neutral",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        public static HtmlBuilder AnalyBody(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            List<AnalyData> analyDataList,
            bool inRange)
        {
            if (inRange)
            {
                var analy = new Analy(
                    context: context,
                    ss: ss,
                    analyDataList: analyDataList);
                return hb
                    .Hidden(
                        controlId: "AnalyJson",
                        value: analy.ToJson());
            }
            else
            {
                return hb;
            }
        }
    }
}