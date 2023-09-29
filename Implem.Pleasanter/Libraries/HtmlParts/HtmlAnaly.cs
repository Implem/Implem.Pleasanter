using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.ViewModes;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlAnaly
    {
        public static HtmlBuilder AnalyPartDialog(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("AnalyPartDialogForm")
                    .Action(Locations.ItemAction(
                        context: context,
                        id: ss.SiteId)),
                action: () => hb
                    .FieldDropDown(
                        context: context,
                        controlId: "AnalyPartGroupBy",
                        fieldCss: "field-normal",
                        controlCss: " always-send",
                        labelText: Displays.Column(context: context),
                        optionCollection: ss.AnalyGroupByOptions(context: context))
                    .FieldSpinner(
                        controlId: "AnalyPartPeriodValue",
                        fieldCss: "field-normal",
                        controlCss: " always-send",
                        labelText: Displays.Value(context: context),
                        value: 1,
                        min: Parameters.General.AnalyPartPeriodValueMin,
                        max: Parameters.General.AnalyPartPeriodValueMax,
                        step: 1,
                        width: 70)
                    .FieldDropDown(
                        context: context,
                        fieldId: "AnalyPartPeriodField",
                        controlId: "AnalyPartPeriod",
                        fieldCss: "field-normal",
                        controlCss: " always-send",
                        labelText: Displays.Period(context: context),
                        optionCollection: ss.PeriodOptions(context: context),
                        selectedValue: "")
                    .FieldDropDown(
                        context: context,
                        controlId: "AnalyPartAggregationType",
                        controlCss: " always-send",
                        labelText: Displays.AggregationType(context: context),
                        optionCollection: ss.AnalyAggregationTypeOptions(context: context))
                    .FieldDropDown(
                        context: context,
                        controlId: "AnalyPartAggregationTarget",
                        controlCss: " always-send",
                        labelText: Displays.AggregationTarget(context: context),
                        optionCollection: ss.AnalyAggregationTargetOptions(context: context),
                        insertBlank: true)
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            text: Displays.Add(context: context),
                            controlId: "AddAnalyPart",
                            controlCss: "button-icon" + (ss.SaveViewType == SiteSettings.SaveViewTypes.None
                                 ? " save-view-types-none"
                                 : string.Empty),
                            onClick: "$p.send($(this));",
                            method: "post",
                            icon: "ui-icon-disk")
                        .Button(
                            text: Displays.Cancel(context: context),
                            controlCss: "button-icon",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        public static HtmlBuilder Analy(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            List<EnumerableRowCollection<DataRow>> dataRowsSet,
            bool inRange)
        {
            return hb.Div(css: "both", action: () => hb
                .Button(
                    controlId: "AddAnalyPart",
                    text: Displays.Add(context: context),
                    controlCss: "button-icon",
                    onClick: "$p.openAnalyPartDialog($(this));",
                    icon: "ui-icon-plus",
                    action: "OpenAnalyPartDialog",
                    method: "post")
                .Div(id: "TimeSeriesBody", action: () => hb
                    .AnalyBody(
                        context: context,
                        ss: ss,
                        dataRowsSet: dataRowsSet,
                        inRange: inRange)));
        }

        public static HtmlBuilder AnalyBody(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            List<EnumerableRowCollection<DataRow>> dataRowsSet,
            bool inRange)
        {
            if (inRange && dataRowsSet?.Any() == true)
            {
                var analy = new Analy(
                    context: context,
                    ss: ss,
                    dataRowsSet: dataRowsSet);
                return hb
                    .Svg(id: "Analy")
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