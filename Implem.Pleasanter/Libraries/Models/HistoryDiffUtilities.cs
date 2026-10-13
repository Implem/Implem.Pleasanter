using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Models
{
    public static class HistoryDiffUtilities
    {
        public static string HistoryDiff<TModel>(
            Context context,
            SiteSettings ss,
            List<TModel> versions,
            Func<int, TModel> getModel,
            Func<TModel, Column, List<string>, string> toDisplay)
            where TModel : BaseModel
        {
            var hb = new HtmlBuilder();
            var invalidVerOrder = false;
            if (versions.Count <= 1)
            {
                hb.HistoryDiffNoPreviousVersion(context: context);
            }
            else
            {
                var vers = versions.Select(o => o.Ver).ToList();
                var baseVer = Ver(
                    context: context,
                    vers: vers,
                    controlId: "HistoryDiffBaseVer",
                    defaultVer: vers.Max());
                var newVer = Ver(
                    context: context,
                    vers: vers,
                    controlId: "HistoryDiffNewVer",
                    defaultVer: vers.Any(o => o < baseVer)
                        ? baseVer
                        : vers
                            .Where(o => o > baseVer)
                            .DefaultIfEmpty(baseVer)
                            .Min());
                var oldVer = Ver(
                    context: context,
                    vers: vers,
                    controlId: "HistoryDiffOldVer",
                    defaultVer: vers
                        .Where(o => o < newVer)
                        .DefaultIfEmpty(vers.Min())
                        .Max());
                invalidVerOrder = newVer <= oldVer;
                var oldModel = getModel(oldVer);
                var newModel = getModel(newVer);
                hb.HistoryDiff(
                    context: context,
                    versionOptions: versions.ToDictionary(
                        o => o.Ver.ToString(),
                        o => new ControlData(VerAndUpdatedTimeText(
                            context: context,
                            ss: ss,
                            model: o))),
                    oldVer: oldVer,
                    newVer: newVer,
                    oldVerLabelText: VerLabelText(
                        context: context,
                        model: oldModel),
                    newVerLabelText: VerLabelText(
                        context: context,
                        model: newModel),
                    tabs: Tabs(
                        context: context,
                        ss: ss,
                        oldModel: oldModel,
                        newModel: newModel,
                        toDisplay: toDisplay),
                    invalidVerOrder: invalidVerOrder);
            }
            return new ResponseCollection(context: context)
                .Html("#FieldSetHistoryDiff", hb)
                .Invoke("initHistoryDiff")
                .Message(
                    message: Messages.NewVersionMustBeNewer(context: context),
                    _using: invalidVerOrder)
                .Messages(context.Messages)
                .ToJson();
        }

        private static int Ver(
            Context context,
            List<int> vers,
            string controlId,
            int defaultVer)
        {
            var ver = context.Forms.Int(controlId);
            return vers.Contains(ver)
                ? ver
                : defaultVer;
        }

        private static string VerLabelText(
            Context context,
            BaseModel model)
        {
            return Displays.Ver(context: context)
                + " " + model.Ver
                + " - " + SiteInfo.UserName(
                    context: context,
                    userId: model.Updator.Id);
        }

        private static string VerAndUpdatedTimeText(
            Context context,
            SiteSettings ss,
            BaseModel model)
        {
            var column = ss.GetColumn(context: context, columnName: "UpdatedTime");
            var updatedTime = column != null
                ? model.UpdatedTime.ToDisplay(
                    context: context,
                    ss: ss,
                    column: column)
                : model.UpdatedTime.DisplayValue.ToString();
            return model.Ver + ": " + updatedTime;
        }

        private static List<HistoryDiffTab> Tabs<TModel>(
            Context context,
            SiteSettings ss,
            TModel oldModel,
            TModel newModel,
            Func<TModel, Column, List<string>, string> toDisplay)
            where TModel : BaseModel
        {
            var tabs = new List<HistoryDiffTab>
            {
                Tab(
                    context: context,
                    ss: ss,
                    tab: new Tab { Id = 0 },
                    labelText: ss.GeneralTabLabelText,
                    oldModel: oldModel,
                    newModel: newModel,
                    toDisplay: toDisplay)
            };
            ss.Tabs?.ForEach(tab => tabs.Add(Tab(
                context: context,
                ss: ss,
                tab: tab,
                labelText: tab.LabelText,
                oldModel: oldModel,
                newModel: newModel,
                toDisplay: toDisplay)));
            return tabs;
        }

        private static HistoryDiffTab Tab<TModel>(
            Context context,
            SiteSettings ss,
            Tab tab,
            string labelText,
            TModel oldModel,
            TModel newModel,
            Func<TModel, Column, List<string>, string> toDisplay)
            where TModel : BaseModel
        {
            var oldMine = oldModel.Mine(context: context);
            var newMine = newModel.Mine(context: context);
            var historyDiffTab = new HistoryDiffTab
            {
                Id = tab.Id.ToString(),
                LabelText = labelText
            };
            ss.GetEditorColumns(context: context, tab: tab)
                ?.AllowedColumns(context: context, ss: ss, checkPermission: true)
                .Where(column => context.ContractSettings.Attachments()
                    || column.ControlType != "Attachments")
                .Where(column => !HtmlHistoryDiff.ExcludedColumnNames.Contains(column.ColumnName))
                .ForEach(column =>
                {
                    var oldValue = toDisplay(oldModel, column, oldMine);
                    var newValue = toDisplay(newModel, column, newMine);
                    historyDiffTab.Fields.Add(new HistoryDiffField
                    {
                        ColumnName = column.ColumnName,
                        LabelText = column.LabelText,
                        OldValue = HtmlHistoryDiff.ToDiffText(
                            value: oldValue,
                            column: column,
                            rendered: true),
                        NewValue = HtmlHistoryDiff.ToDiffText(
                            value: newValue,
                            column: column,
                            rendered: true),
                        OldSourceValue = HtmlHistoryDiff.ToDiffText(
                            value: oldValue,
                            column: column,
                            rendered: false),
                        NewSourceValue = HtmlHistoryDiff.ToDiffText(
                            value: newValue,
                            column: column,
                            rendered: false),
                        Switchable = HtmlHistoryDiff.Switchable(column: column)
                    });
                });
            return historyDiffTab;
        }
    }
}
