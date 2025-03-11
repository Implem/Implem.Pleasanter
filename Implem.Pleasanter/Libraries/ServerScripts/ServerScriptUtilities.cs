using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.Interfaces;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using static Implem.Pleasanter.Libraries.Security.Permissions;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
using Types = Implem.Libraries.Utilities.Types;

namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public static class ServerScriptUtilities
    {
        private static object Value(ExpandoObject data, string name)
        {
            if (data == null)
            {
                return null;
            }
            if (!((IDictionary<string, object>)data).TryGetValue(name, out var value))
            {
                return null;
            }
            return value;
        }

        private static string String(ExpandoObject data, string columnName)
        {
            object value;
            switch (Def.ExtendedColumnTypes.Get(columnName ?? string.Empty))
            {
                case "Date":
                    value = Date(
                        data: data,
                        name: columnName);
                    break;
                default:
                    value = Value(
                        data: data,
                        name: columnName);
                    break;
            }
            return value?.ToString() ?? string.Empty;
        }

        private static decimal Decimal(ExpandoObject data, string name)
        {
            return decimal.TryParse(Value(data, name)?.ToString(), out var value)
                ? value
                : default(decimal);
        }

        private static DateTime Date(ExpandoObject data, string name)
        {
            var value = Value(data, name);
            return value is DateTime dateTime
                ? TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo.Local)
                : Types.ToDateTime(0);
        }

        private static int Int(ExpandoObject data, string name)
        {
            return (int)Decimal(data, name);
        }

        private static bool Bool(ExpandoObject data, string name)
        {
            return Value(data, name).ToBool();
        }

        private static (string, object) ReadNameValue(string columnName, object value)
        {
            return (columnName, value);
        }

        private static (string, object) ReadNameValue(
            Context context, SiteSettings ss, string columnName, object value, List<string> mine)
        {
            return (
                columnName,
                ss?.ColumnHash.Get(columnName)?.CanRead(
                    context: context,
                    ss: ss,
                    mine: mine,
                    noCache: true) == true
                        ? value
                        : null);
        }

        public static IEnumerable<(string Name, object Value)> Values(
            Context context, SiteSettings ss, BaseItemModel model, bool isFormulaServerScript = false)
        {
            var mine = model?.Mine(context: context);
            var values = new List<(string, object)>
            {
                ReadNameValue(
                    columnName: nameof(model.ReadOnly),
                    value: model.ReadOnly),
                ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(model.SiteId),
                    value: model.SiteId,
                    mine: mine),
                ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(model.Title),
                    value: model.Title?.Value,
                    mine: mine),
                ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(model.Body),
                    value: model.Body,
                    mine: mine),
                ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(model.Ver),
                    value: model.Ver,
                    mine: mine),
                ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(model.Creator),
                    value: model.Creator.Id,
                    mine: mine),
                ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(model.Updator),
                    value: model.Updator.Id,
                    mine: mine),
                ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(model.CreatedTime),
                    value: isFormulaServerScript
                        ? model.CreatedTime?.Value.ToClientTimeZone(context: context)
                        : model.CreatedTime?.Value,
                    mine: mine),
                ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(model.UpdatedTime),
                    value: isFormulaServerScript
                        ? model.UpdatedTime?.Value.ToClientTimeZone(context: context)
                        : model.UpdatedTime?.Value,
                    mine: mine)
            };
            values.AddRange(model
                .ClassHash
                .Select(element => ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: element.Key,
                    value: element.Value,
                    mine: mine)));
            values.AddRange(model
                .NumHash
                .Select(element => ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: element.Key,
                    value: element.Value.Value
                        ?? (ss?.GetColumn(
                            context: context,
                            columnName: element.Key)
                                ?.Nullable == true
                                    ? (decimal?)null
                                    : 0),
                    mine: mine)));
            values.AddRange(model
                .DateHash
                .Select(element => ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: element.Key,
                    value: isFormulaServerScript
                        ? element.Value.ToClientTimeZone(context: context)
                        : element.Value,
                    mine: mine)));
            values.AddRange(model
                .DescriptionHash
                .Select(element => ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: element.Key,
                    value: element.Value,
                    mine: mine)));
            values.AddRange(model
                .CheckHash
                .Select(element => ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: element.Key,
                    value: element.Value,
                    mine: mine)));
            values.AddRange(model
                .AttachmentsHash
                .Select(element => ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: element.Key,
                    value: element.Value.ToJson(),
                    mine: mine)));
            values.Add(ReadNameValue(
                context: context,
                ss: ss,
                columnName: "Comments",
                value: model.Comments?.ToJson(),
                mine: mine));
            if (model is IssueModel issueModel)
            {
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(IssueModel.IssueId),
                    value: issueModel.IssueId,
                    mine: mine));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(IssueModel.StartTime),
                    value: isFormulaServerScript
                        ? issueModel.StartTime.ToClientTimeZone(context: context)
                        : issueModel.StartTime,
                    mine: mine));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(IssueModel.CompletionTime),
                    value: isFormulaServerScript
                        ? issueModel.CompletionTime.Value
                            .AddDifferenceOfDates(
                                format: ss.GetColumn(
                                    context: context,
                                    columnName: nameof(IssueModel.CompletionTime))?.EditorFormat,
                                minus: true)
                            .ToClientTimeZone(context: context)
                        : issueModel.CompletionTime.Value,
                    mine: mine));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(IssueModel.WorkValue),
                    value: issueModel.WorkValue.Value,
                    mine: mine));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(IssueModel.ProgressRate),
                    value: issueModel.ProgressRate.Value,
                    mine: mine));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(IssueModel.RemainingWorkValue),
                    value: issueModel.RemainingWorkValue.Value,
                    mine: mine));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(IssueModel.Status),
                    value: issueModel.Status?.Value,
                    mine: mine));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(IssueModel.Manager),
                    value: issueModel.Manager.Id,
                    mine: mine));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(IssueModel.Owner),
                    value: issueModel.Owner.Id,
                    mine: mine));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(IssueModel.Locked),
                    value: issueModel.Locked,
                    mine: mine));
            }
            if (model is ResultModel resultModel)
            {
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(ResultModel.ResultId),
                    value: resultModel.ResultId,
                    mine: mine));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(ResultModel.Status),
                    value: resultModel.Status?.Value,
                    mine: mine));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(ResultModel.Manager),
                    value: resultModel.Manager.Id,
                    mine: mine));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(ResultModel.Owner),
                    value: resultModel.Owner.Id,
                    mine: mine));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(ResultModel.Locked),
                    value: resultModel.Locked,
                    mine: mine));
            }
            return values.ToArray();
        }

        private static string ToClientTimeZone(this DateTime self, Context context)
        {
            return self.InRange()
                ? self.ToLocal(context).ToString("yyyy/MM/dd HH:mm:ss")
                : string.Empty;
        }

        public static IEnumerable<(string Name, object Value)> SavedValues(
            Context context,
            SiteSettings ss,
            BaseItemModel model)
        {
            var mine = model?.Mine(context: context);
            var values = new List<(string, object)>();
            values.Add(ReadNameValue(
                context: context,
                ss: ss,
                columnName: nameof(model.SiteId),
                value: model.SavedSiteId,
                mine: mine));
            values.Add(ReadNameValue(
                context: context,
                ss: ss,
                columnName: nameof(model.Title),
                value: model.SavedTitle,
                mine: mine));
            values.Add(ReadNameValue(
                context: context,
                ss: ss,
                columnName: nameof(model.Body),
                value: model.SavedBody,
                mine: mine));
            values.Add(ReadNameValue(
                context: context,
                ss: ss,
                columnName: nameof(model.Ver),
                value: model.SavedVer,
                mine: mine));
            values.Add(ReadNameValue(
                context: context,
                ss: ss,
                columnName: nameof(model.Creator),
                value: model.SavedCreator,
                mine: mine));
            values.Add(ReadNameValue(
                context: context,
                ss: ss,
                columnName: nameof(model.Updator),
                value: model.SavedUpdator,
                mine: mine));
            values.Add(ReadNameValue(
                context: context,
                ss: ss,
                columnName: nameof(model.CreatedTime),
                value: model.SavedCreatedTime,
                mine: mine));
            values.Add(ReadNameValue(
                context: context,
                ss: ss,
                columnName: nameof(model.UpdatedTime),
                value: model.SavedUpdatedTime,
                mine: mine));
            values.AddRange(model
                .SavedClassHash
                .Select(element => ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: element.Key,
                    value: element.Value,
                    mine: mine)));
            values.AddRange(model
                .SavedNumHash
                .Select(element => ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: element.Key,
                    value: element.Value
                        ?? (ss?.GetColumn(
                            context: context,
                            columnName: element.Key)
                                ?.Nullable == true
                                    ? (decimal?)null
                                    : 0),
                    mine: mine)));
            values.AddRange(model
                .SavedDateHash
                .Select(element => ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: element.Key,
                    value: element.Value,
                    mine: mine)));
            values.AddRange(model
                .SavedDescriptionHash
                .Select(element => ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: element.Key,
                    value: element.Value,
                    mine: mine)));
            values.AddRange(model
                .SavedCheckHash
                .Select(element => ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: element.Key,
                    value: element.Value,
                    mine: mine)));
            values.AddRange(model
                .SavedAttachmentsHash
                .Select(element => ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: element.Key,
                    value: element.Value,
                    mine: mine)));
            values.Add(ReadNameValue(
                context: context,
                ss: ss,
                columnName: "Comments",
                value: model.SavedComments,
                mine: mine));
            if (model is IssueModel issueModel)
            {
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(IssueModel.IssueId),
                    value: issueModel.SavedIssueId,
                    mine: mine));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(IssueModel.StartTime),
                    value: issueModel.SavedStartTime,
                    mine: mine));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(IssueModel.CompletionTime),
                    value: issueModel.SavedCompletionTime,
                    mine: mine));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(IssueModel.WorkValue),
                    value: issueModel.SavedWorkValue,
                    mine: mine));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(IssueModel.ProgressRate),
                    value: issueModel.SavedProgressRate,
                    mine: mine));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(IssueModel.RemainingWorkValue),
                    value: issueModel.SavedRemainingWorkValue,
                    mine: mine));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(IssueModel.Status),
                    value: issueModel.SavedStatus,
                    mine: mine));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(IssueModel.Manager),
                    value: issueModel.SavedManager,
                    mine: mine));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(IssueModel.Owner),
                    value: issueModel.SavedOwner,
                    mine: mine));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(IssueModel.Locked),
                    value: issueModel.SavedLocked,
                    mine: mine));
            }
            if (model is ResultModel resultModel)
            {
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(ResultModel.ResultId),
                    value: resultModel.SavedResultId,
                    mine: mine));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(ResultModel.Status),
                    value: resultModel.SavedStatus,
                    mine: mine));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(ResultModel.Manager),
                    value: resultModel.SavedManager,
                    mine: mine));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(ResultModel.Owner),
                    value: resultModel.SavedOwner,
                    mine: mine));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(ResultModel.Locked),
                    value: resultModel.SavedLocked,
                    mine: mine));
            }
            return values.ToArray();
        }

        public static IEnumerable<(string Name, ServerScriptModelColumn Value)> Columns(
            Context context, SiteSettings ss, BaseItemModel model)
        {
            var columns = Def.ColumnDefinitionCollection
                .Where(definition => definition.TableName == ss?.ReferenceType)
                .Select(definition => definition.ColumnName)
                .Concat(ss.GridColumns)
                .Distinct()
                .Select(columnName =>
                {
                    var column = ss.GetColumn(
                        context: context,
                        columnName: columnName);
                    return (
                        columnName,
                        model.ServerScriptModelRow.Columns?.Get(column?.ColumnName)
                            ?? new ServerScriptModelColumn(
                                labelText: column?.LabelText,
                                labelRaw: string.Empty,
                                rawText: string.Empty,
                                readOnly: column?.EditorReadOnly == true,
                                hide: column?.Hide == true,
                                validateRequired: column?.ValidateRequired == true,
                                extendedFieldCss: column?.ExtendedFieldCss,
                                extendedControlCss: column?.ExtendedControlCss,
                                extendedCellCss: column?.ExtendedCellCss,
                                extendedHtmlBeforeField: column?.ExtendedHtmlBeforeField,
                                extendedHtmlBeforeLabel: column?.ExtendedHtmlBeforeLabel,
                                extendedHtmlBetweenLabelAndControl: column?.ExtendedHtmlBetweenLabelAndControl,
                                extendedHtmlAfterControl: column?.ExtendedHtmlAfterControl,
                                extendedHtmlAfterField: column?.ExtendedHtmlAfterField));
                }).ToArray();
            return columns;
        }

        private static Column[] FilterCanUpdateColumns(
            Context context,
            SiteSettings ss,
            BaseItemModel model,
            IEnumerable<string> columnNames)
        {
            var mine = model?.Mine(context: context);
            var columns = columnNames
                .Distinct()
                .Select(columnName => ss.ColumnHash.TryGetValue(columnName, out var column)
                    ? column
                    : null)
                .Where(column => column?.CanEdit(
                    context: context,
                    ss: ss,
                    mine: mine,
                    skipCanReadCheck: true) == true)
                .ToArray();
            return columns;
        }

        private static Dictionary<string, ServerScriptModelColumn> SetColumns(
            Context context,
            SiteSettings ss,
            ExpandoObject columns,
            BaseItemModel model)
        {
            var mine = model?.Mine(context: context);
            var scriptValues = new Dictionary<string, ServerScriptModelColumn>();
            columns?.ForEach(datam =>
            {
                if (!ss.ColumnHash.TryGetValue(datam.Key, out var column))
                {
                    return;
                }
                var serverScriptColumn = datam.Value as ServerScriptModelColumn;
                if (serverScriptColumn.Changed())
                {
                    scriptValues[datam.Key] = serverScriptColumn;
                }
            });
            return scriptValues;
        }

        private static ServerScriptModelRow SetRow(
            Context context,
            SiteSettings ss,
            ExpandoObject model,
            ExpandoObject columns,
            ServerScriptModelHidden hidden,
            ServerScriptModelResponses responses,
            ServerScriptElements elements,
            BaseItemModel itemModel)
        {
            var row = new ServerScriptModelRow
            {
                ExtendedRowCss = String(model, nameof(ServerScriptModelRow.ExtendedRowCss)),
                ExtendedRowData = String(model, nameof(ServerScriptModelRow.ExtendedRowData)),
                Columns = SetColumns(
                    context: context,
                    ss: ss,
                    columns: columns,
                    model: itemModel),
                Hidden = hidden.GetAll(),
                Responses = responses,
                Elements = elements
            };
            return row;
        }

        private static void SetExtendedColumnValues(
            Context context,
            BaseItemModel model,
            ExpandoObject data,
            Column[] columns)
        {
            columns?.ForEach(column => model?.SetValue(
                context: context,
                column: column,
                value: String(
                    data: data,
                    columnName: column.ColumnName)));
        }

        private static void SetColumnFilterHash(
            View view,
            ServerScriptModel data)
        {
            var columnFilterHash = data.View.Filters;
            var noMerge = data.View.FiltersCleared;
            if (noMerge)
            {
                view.Incomplete = false;
                view.Own = false;
                view.NearCompletionTime = false;
                view.Delay = false;
                view.Overdue = false;
                view.Search = string.Empty;
                view.ColumnFilterHash?.Clear();
            }
            columnFilterHash?.ForEach(columnFilter =>
            {
                if (view.ColumnFilterHash == null)
                {
                    view.ColumnFilterHash = new Dictionary<string, string>();
                }
                view.ColumnFilterHash[columnFilter.Key] = Value(columnFilterHash, columnFilter.Key).ToString();
                data.View.ClearColumnFilterNegatives(view: view);
            });
        }

        private static void SetColumnFilterNegatives(
            View view,
            ServerScriptModel data)
        {
            data.View.FilterNegatives?.ForEach(filterNegative =>
            {
                if (filterNegative.Value)
                {
                    if (view.ColumnFilterNegatives?.Contains(filterNegative.Key) != true)
                    {
                        if (view.ColumnFilterNegatives == null)
                        {
                            view.ColumnFilterNegatives = new List<string>();
                        }
                        view.ColumnFilterNegatives.Add(filterNegative.Key);
                    }
                }
                else
                {
                    view.ColumnFilterNegatives?.RemoveAll(o => o == filterNegative.Key);
                }
            });
        }

        private static void SetColumnSearchTypeHash(
            View view,
            ExpandoObject columnSearchTypeHash)
        {
            columnSearchTypeHash?.ForEach(columnFilterSearchType =>
            {
                if (view.ColumnFilterSearchTypes == null)
                {
                    view.ColumnFilterSearchTypes = new Dictionary<string, Column.SearchTypes>();
                }
                view.ColumnFilterSearchTypes[columnFilterSearchType.Key] = Value(columnSearchTypeHash, columnFilterSearchType.Key).ToString().ToEnum<Column.SearchTypes>();
            });
        }

        private static void SetColumnSorterHash(
            View view,
            ExpandoObject columnSorterHash)
        {
            columnSorterHash?.ForEach(columnFilter =>
            {
                if (view.ColumnSorterHash == null)
                {
                    view.ColumnSorterHash = new Dictionary<string, SqlOrderBy.Types>();
                }
                if (Enum.TryParse<SqlOrderBy.Types>(Value(columnSorterHash, columnFilter.Key).ToString(), out var value))
                {
                    view.ColumnSorterHash[columnFilter.Key] = value;
                }
            });
        }

        private static void SetValue<T>(
            string columnName,
            Dictionary<string, Column> columns,
            Action<T> setter,
            Func<Column, T> getter)
        {
            if (!columns.TryGetValue(columnName, out var column))
            {
                return;
            }
            var value = getter(column);
            setter(value);
        }

        private static void SetIssueModelValues(
            Context context,
            SiteSettings ss,
            IssueModel issueModel,
            ExpandoObject data,
            Dictionary<string, Column> columns,
            ServerScriptConditions condition)
        {
            SetValue(
                columnName: nameof(IssueModel.Title),
                columns: columns,
                setter: value => issueModel.Title.Value = value,
                getter: column => String(
                    data: data,
                    columnName: column.ColumnName));
            SetValue(
                columnName: nameof(IssueModel.Body),
                columns: columns,
                setter: value => issueModel.Body = value,
                getter: column => String(
                    data: data,
                    columnName: column.ColumnName));
            SetValue(
                columnName: nameof(IssueModel.StartTime),
                columns: columns,
                setter: value => issueModel.StartTime = value,
                getter: column => Date(
                    data: data,
                    name: column.Name));
            SetValue(
                columnName: nameof(IssueModel.CompletionTime),
                columns: columns,
                setter: value => issueModel.CompletionTime = new CompletionTime(
                    context: context,
                    ss: ss,
                    value: value,
                    status: issueModel.Status,
                    byForm: true),
                getter: column => Date(
                    data: data,
                    name: column.Name));
            SetValue(
                columnName: nameof(IssueModel.WorkValue),
                columns: columns,
                setter: value => issueModel.WorkValue.Value = value,
                getter: column => Decimal(
                    data: data,
                    name: column.Name));
            SetValue(
                columnName: nameof(IssueModel.ProgressRate),
                columns: columns,
                setter: value => issueModel.ProgressRate.Value = value,
                getter: column => Decimal(
                    data: data,
                    name: column.Name));
            SetValue(
                columnName: nameof(IssueModel.Status),
                columns: columns,
                setter: value => issueModel.Status.Value = value,
                getter: column => Int(
                    data: data,
                    name: column.Name));
            SetValue(
                columnName: nameof(IssueModel.Manager),
                columns: columns,
                setter: value => issueModel.Manager = SiteInfo.User(
                    context: context,
                    userId: value),
                getter: column => Int(
                    data: data,
                    name: column.Name));
            SetValue(
                columnName: nameof(IssueModel.Owner),
                columns: columns,
                setter: value => issueModel.Owner = SiteInfo.User(
                    context: context,
                    userId: value),
                getter: column => Int(
                    data: data,
                    name: column.Name));
            SetValue(
                columnName: nameof(IssueModel.Locked),
                columns: columns,
                setter: value => issueModel.Locked = value,
                getter: column => Bool(
                    data: data,
                    name: column.Name));
            SetValue(
                columnName: nameof(IssueModel.Comments),
                columns: columns,
                setter: value => issueModel.Comments.Prepend(
                    context: context,
                    ss: ss,
                    body: value),
                getter: column => String(
                    data: data,
                    columnName: column.ColumnName));
            issueModel.SetTitle(
                context: context,
                ss: ss);
            if (Bool(data: data, name: "UpdateOnExit"))
            {
                issueModel.VerUp = Versions.MustVerUp(
                    context: context,
                    ss: ss,
                    baseModel: issueModel);
                issueModel.Update(
                    context: CreateContext(
                        context: context,
                        controller: "Items",
                        action: "Update",
                        id: issueModel.IssueId,
                        apiRequestBody: string.Empty),
                    ss: ss,
                    notice: true);
            }
        }

        private static void SetResultModelValues(
            Context context,
            SiteSettings ss,
            ResultModel resultModel,
            ExpandoObject data,
            Dictionary<string, Column> columns,
            ServerScriptConditions condition)
        {
            SetValue(
                columnName: nameof(ResultModel.Title),
                columns: columns,
                setter: value => resultModel.Title.Value = value,
                getter: column => String(
                    data: data,
                    columnName: column.ColumnName));
            SetValue(
                columnName: nameof(ResultModel.Body),
                columns: columns,
                setter: value => resultModel.Body = value,
                getter: column => String(
                    data: data,
                    columnName: column.ColumnName));
            SetValue(
                columnName: nameof(ResultModel.Status),
                columns: columns,
                setter: value => resultModel.Status.Value = value,
                getter: column => Int(
                    data: data,
                    name: column.ColumnName));
            SetValue(
                columnName: nameof(ResultModel.Manager),
                columns: columns,
                setter: value => resultModel.Manager = SiteInfo.User(
                    context: context,
                    userId: value),
                getter: column => Int(
                    data: data,
                    name: column.ColumnName));
            SetValue(
                columnName: nameof(ResultModel.Owner),
                columns: columns,
                setter: value => resultModel.Owner = SiteInfo.User(
                    context: context,
                    userId: value),
                getter: column => Int(
                    data: data,
                    name: column.ColumnName));
            SetValue(
                columnName: nameof(ResultModel.Locked),
                columns: columns,
                setter: value => resultModel.Locked = value,
                getter: column => Bool(
                    data: data,
                    name: column.Name));
            SetValue(
                columnName: nameof(ResultModel.Comments),
                columns: columns,
                setter: value => resultModel.Comments.Prepend(
                    context: context,
                    ss: ss,
                    body: value),
                getter: column => String(
                    data: data,
                    columnName: column.ColumnName));
            resultModel.SetTitle(
                context: context,
                ss: ss);
            if (Bool(data: data, name: "UpdateOnExit"))
            {
                resultModel.VerUp = Versions.MustVerUp(
                    context: context,
                    ss: ss,
                    baseModel: resultModel);
                resultModel.Update(
                    context: CreateContext(
                        context: context,
                        controller: "Items",
                        action: "Update",
                        id: resultModel.ResultId,
                        apiRequestBody: string.Empty),
                    ss: ss,
                    notice: true);
            }
        }

        private static void SetViewValues(
            SiteSettings ss,
            ServerScriptModelSiteSettings data)
        {
            if (ss == null)
            {
                return;
            }
            var viewId = data?.DefaultViewId ?? default;
            ss.GridView = ss?.Views?.Any(v => v.Id == viewId) == true ? viewId : default;
        }

        public static ServerScriptModelRow SetValues(
            Context context,
            SiteSettings ss,
            BaseItemModel model,
            View view,
            ServerScriptModel data)
        {
            var valueColumns = FilterCanUpdateColumns(
                context: context,
                ss: ss,
                model: model,
                columnNames: data.GetChangeItemNames());
            var valueColumnDictionary = valueColumns
                .ToDictionary(
                    column => column.ColumnName,
                    column => column);
            var scriptValues = SetRow(
                context: context,
                ss: ss,
                model: data.Model,
                columns: data.Columns,
                hidden: data.Hidden,
                elements: data.Elements,
                responses: data.Responses,
                itemModel: model);
            SetExtendedColumnValues(
                context: context,
                model: model,
                data: data.Model,
                columns: valueColumns);
            if (view != null)
            {
                view.AlwaysGetColumns = data.View.AlwaysGetColumns;
                view.OnSelectingWhere = data.View.OnSelectingWhere;
                view.OnSelectingOrderBy = data.View.OnSelectingOrderBy;
                view.ColumnPlaceholders = data.View.ColumnPlaceholders;
                SetColumnFilterHash(
                    view: view,
                    data: data);
                SetColumnFilterNegatives(
                    view: view,
                    data: data);
                SetColumnSearchTypeHash(
                    view: view,
                    columnSearchTypeHash: data.View.SearchTypes);
                SetColumnSorterHash(
                    view: view,
                    columnSorterHash: data.View.Sorters);
            }
            model.ReadOnly = Bool(
                data: data.Model,
                name: "ReadOnly");
            switch (ss?.ReferenceType)
            {
                case "Issues":
                    if (model is IssueModel issueModel)
                    {
                        SetIssueModelValues(
                            context: context,
                            ss: ss,
                            issueModel: issueModel,
                            data: data.Model,
                            columns: valueColumnDictionary,
                            ParseServerScriptCondition(data.Context.Condition));
                    }
                    break;
                case "Results":
                    if (model is ResultModel resultModel)
                    {
                        SetResultModelValues(
                            context: context,
                            ss: ss,
                            resultModel: resultModel,
                            data: data.Model,
                            columns: valueColumnDictionary,
                            ParseServerScriptCondition(data.Context.Condition));
                    }
                    break;
            }
            SetViewValues(
                ss: ss,
                data: data.SiteSettings);
            return scriptValues;
        }

        private static ServerScriptConditions ParseServerScriptCondition(string s)
        {
            return Enum.TryParse<ServerScriptConditions>(s, out var condition)
                ? condition
                : ServerScriptConditions.None;
        }

        public static ServerScriptModelRow Execute(
            Context context,
            SiteSettings ss,
            GridData gridData,
            BaseItemModel itemModel,
            View view,
            ServerScript[] scripts,
            ServerScriptConditions condition,
            bool debug,
            bool onTesting = false)
        {
            if (ss?.ServerScriptsAllDisabled == true)
            {
                return null;
            }
            if (!(Parameters.Script.ServerScript != false
                && context.ContractSettings.ServerScript != false
                && context.ServerScriptDisabled == false))
            {
                return null;
            }
            if (!(context?.ServerScriptDepth < 10))
            {
                return null;
            }
            itemModel = itemModel ?? new BaseItemModel();
            ServerScriptModelRow scriptValues = null;
            using (var model = new ServerScriptModel(
                context: context,
                ss: ss,
                gridData: gridData,
                data: Values(
                    context: context,
                    ss: ss,
                    model: itemModel),
                saved: SavedValues(
                    context: context,
                    ss: ss,
                    model: itemModel),
                columns: Columns(
                    context: context,
                    ss: ss,
                    model: itemModel),
                view: view,
                condition: condition,
                timeOut: GetTimeOut(scripts: scripts),
                debug: debug,
                onTesting: onTesting))
            {
                using (var engine = new ScriptEngine(debug: debug))
                {
                    try
                    {
                        engine.ContinuationCallback = model.ContinuationCallback;
                        engine.Execute(ServerScriptJsLibraries.ScriptInit(), debug: false);
                        engine.AddHostType(typeof(Newtonsoft.Json.JsonConvert));
                        engine.AddHostObject("context", model.Context);
                        engine.AddHostObject("grid", model.Grid);
                        engine.AddHostObject("model", model.Model);
                        engine.AddHostObject("saved", model.Saved);
                        engine.AddHostObject("depts", model.Depts);
                        engine.AddHostObject("groups", model.Groups);
                        engine.AddHostObject("users", model.Users);
                        engine.AddHostObject("columns", model.Columns);
                        engine.AddHostObject("siteSettings", model.SiteSettings);
                        engine.AddHostObject("view", model.View);
                        engine.AddHostObject("items", model.Items);
                        engine.AddHostObject("hidden", model.Hidden);
                        engine.AddHostObject("responses", model.Responses);
                        engine.AddHostObject("elements", model.Elements);
                        engine.AddHostObject("extendedSql", model.ExtendedSql);
                        engine.AddHostObject("notifications", model.Notification);
                        if (!Parameters.Script.DisableServerScriptHttpClient)
                        {
                            engine.AddHostObject("httpClient", model.HttpClient);
                        }
                        engine.AddHostObject("utilities", model.Utilities);
                        engine.AddHostObject("logs", model.Logs);
                        if (!Parameters.Script.DisableServerScriptFile)
                        {
                            engine.AddHostObject("_file_cs", model.File);
                            engine.Execute(model.File.Script(), debug: false);
                        }
                        engine.AddHostObject("_csv_cs", model.Csv);
                        engine.Execute(model.Csv.Script(), debug: false);
                        engine.Execute(ServerScriptJsLibraries.Scripts(), debug: false);
                        engine.Execute(
                            code: scripts.Select(script =>
                                ProcessedBody(
                                    ss: ss,
                                    script: script)).Join("\n"),
                            debug: debug);
                    }
                    finally
                    {
                        engine.ContinuationCallback = null;
                    }
                }
                scriptValues = SetValues(
                    context: context,
                    ss: ss,
                    model: itemModel,
                    view: condition == ServerScriptConditions.WhenViewProcessing
                        ? view
                        : null,
                    data: model);
            }
            return scriptValues;
        }

        private static string ProcessedBody(SiteSettings ss, ServerScript script)
        {
            var body = script.Body;
            if (script.Functionalize == true)
            {
                body = $"(()=>{{\n{script.Body}\n}})();";
            }
            if (script.TryCatch == true)
            {
                var description = System.Web.HttpUtility.JavaScriptStringEncode(new List<string>()
                {
                    script.Id.ToString(),
                    script.Title,
                    script.Name
                }.Where(o => o?.Trim().IsNullOrEmpty() == false).Join("_"));
                body = $"try{{\n{body}\n}}catch(e){{\nlogs.LogException('{description}\\n' + e.stack);\n}}";
            }
            return body;
        }

        private static DateTime GetTimeOut(ServerScript[] scripts)
        {
            if (scripts.Any(o => o.TimeOut == 0))
            {
                return Parameters.Script.ServerScriptTimeOutChangeable
                    ? DateTime.MaxValue
                    : Parameters.Script.ServerScriptTimeOut == 0
                        ? DateTime.MaxValue
                        : DateTime.Now.AddMilliseconds(Parameters.Script.ServerScriptTimeOut);
            }
            else
            {
                var max = scripts.Max(o => o.TimeOut ?? Parameters.Script.ServerScriptTimeOut);
                return max == 0
                    ? DateTime.MaxValue
                    : DateTime.Now.AddMilliseconds(max);
            }
        }

        public static ServerScriptModelRow Execute(
            Context context,
            SiteSettings ss,
            GridData gridData,
            BaseItemModel itemModel,
            View view,
            Func<ServerScript, bool> where,
            ServerScriptConditions condition)
        {
            if (ss?.ServerScriptsAllDisabled == true)
            {
                return null;
            }
            if (!(Parameters.Script.ServerScript != false
                && context.ContractSettings.ServerScript != false
                && context.ServerScriptDisabled == false))
            {
                return null;
            }
            var scripts = ss
                ?.GetServerScripts(context: context)
                ?.Where(where)
                .ToArray();
            if (scripts?.Any() != true)
            {
                return null;
            }
            var scriptValues = Execute(
                context: context,
                ss: ss,
                gridData: gridData,
                itemModel: itemModel,
                view: view,
                scripts: scripts,
                condition: condition,
                debug: scripts.Any(o => o.Debug));
            return scriptValues;
        }

        public static bool CanEdit(
            this Column column,
            Context context,
            SiteSettings ss,
            BaseModel baseModel)
        {
            if (column == null)
            {
                return false;
            }
            if (baseModel?.ServerScriptModelRow == null)
            {
                return column.CanEdit(
                    context: context,
                    ss: ss,
                    mine: baseModel?.Mine(context: context));
            }
            var serverScriptReadOnly = column.ServerScriptModelColumn?.GetReadOnly();
            var canUpdate = serverScriptReadOnly != null
                ? serverScriptReadOnly != true
                : column.CanEdit(
                    context: context,
                    ss: ss,
                    mine: baseModel?.Mine(context: context));
            return canUpdate;
        }

        public static Context CreateContext(
            Context context,
            string controller,
            string action,
            long id,
            string apiRequestBody)
        {
            var createdContext = context.BackgroundServerScript
                ? new Context(
                    userId: context.UserId,
                    deptId: context.DeptId,
                    tenantId: context.TenantId,
                    request: false,
                    setAuthenticated: true)
                : new Context(apiRequestBody: MergedApiRequestBody(
                    context: context,
                    apiRequestBody: apiRequestBody));
            createdContext.LogBuilder = context.LogBuilder;
            createdContext.UserData = context.UserData;
            createdContext.Messages = context.Messages;
            createdContext.Controller = controller.ToLower();
            createdContext.Action = action.ToLower();
            createdContext.Id = id;
            createdContext.ApiRequestBody = apiRequestBody;
            createdContext.PermissionHash = Permissions.Get(context: createdContext);
            createdContext.ServerScriptDepth = context.ServerScriptDepth + 1;
            if (context.BackgroundServerScript)
            {
                createdContext.BackgroundServerScript = context.BackgroundServerScript;
                createdContext.SetPermissions();
            }
            return createdContext;
        }

        private static string MergedApiRequestBody(Context context, string apiRequestBody)
        {
            if (context.ApiKey.IsNullOrEmpty())
            {
                return apiRequestBody;
            }
            else
            {
                var api = apiRequestBody.Deserialize<Api>() ?? new Api();
                api.ApiKey = api.ApiKey ?? context.ApiKey;
                var json = api.ToJson();
                return json;
            }
        }

        public static ServerScriptModelApiModel[] Get(
            Context context,
            long id,
            string view,
            bool onTesting)
        {
            var apiContext = CreateContext(
                context: context,
                controller: "Items",
                action: "Get",
                id: id,
                apiRequestBody: view);
            var itemModels = new ItemModel(
                context: apiContext,
                referenceId: id)
                    .GetByServerScript(context: apiContext)
                        ?? new BaseItemModel[0];
            var items = itemModels.Select(model => new ServerScriptModelApiModel(
                context: apiContext,
                model: model,
                onTesting: onTesting)).ToArray();
            return items;
        }

        public static ServerScriptModelApiModel[] GetSite(
            Context context,
            long? id = null,
            string title = null,
            string siteName = null,
            string siteGroupName = null,
            string apiRequestBody = null)
        {
            var apiContext = CreateContext(
                context: context,
                controller: "Items",
                action: "Get",
                id: id?.ToLong() ?? context.Id,
                apiRequestBody: apiRequestBody);
            var where = Rds.ItemsWhere();
            if (id != null)
            {
                where.ReferenceId(id);
            }
            else if (title != null)
            {
                where
                    .ReferenceType("Sites")
                    .Sites_Title(title);
            }
            else if (siteName != null)
            {
                where
                    .ReferenceType("Sites")
                    .Sites_SiteName(siteName);
            }
            else if (siteGroupName != null)
            {
                where
                    .ReferenceType("Sites")
                    .Sites_SiteGroupName(siteGroupName);
            }
            else
            {
                return new ServerScriptModelApiModel[0];
            }
            var itemModels = new ItemCollection(
                context: apiContext,
                join: Rds.ItemsJoin().Add(new SqlJoin(
                    "\"Sites\"",
                    SqlJoin.JoinTypes.Inner,
                    $"\"Sites\".\"SiteId\" = \"Items\".\"SiteId\" and \"Sites\".\"TenantId\" = {Parameters.Parameter.SqlParameterPrefix}T")),
                where: where)
                    .Select(itemModel => itemModel.GetSiteByServerScript(context: apiContext)?.FirstOrDefault()
                        ?? new BaseItemModel())
                    .ToArray();
            var items = itemModels.Select(model => new ServerScriptModelApiModel(
                context: apiContext,
                model: model,
                onTesting: false)).ToArray();
            return items;
        }

        public static ServerScriptModelApiModel GetClosestSite(
            Context context,
            long? id = null,
            string siteName = null)
        {
            if (siteName.IsNullOrEmpty()) return null;
            var startId = id ?? context.SiteId;
            var startSs = SiteSettingsUtilities.Get(
                context: context,
                siteId: startId,
                referenceId: startId);
            var startCanRead = context.CanRead(ss: startSs, site: true)
                || context.CanCreate(ss: startSs, site: true);
            if (startCanRead == false) return null;
            var tenantCache = SiteInfo.TenantCaches[context.TenantId];
            var findId = tenantCache.SiteNameTree.Find(
                startId: startId,
                name: siteName);
            if (findId == -1) return null;
            var findSs = SiteSettingsUtilities.Get(
                context: context,
                siteId: findId,
                referenceId: findId);
            var findCanRead = context.CanRead(ss: findSs, site: true)
                || context.CanCreate(ss: findSs, site: true);
            if (findCanRead == false) return null;
            return GetSite(
                context: context,
                id: findId,
                apiRequestBody: string.Empty)
                    .FirstOrDefault();
        }

        public static bool Create(Context context, long id, object model)
        {
            var apiContext = CreateContext(
                context: context,
                controller: "Items",
                action: "Create",
                id: id,
                apiRequestBody: string.Empty);
            return new ItemModel(
                context: apiContext,
                referenceId: id)
                    .CreateByServerScript(
                        context: apiContext,
                        model: model);
        }

        public static bool Update(Context context, long id, object model)
        {
            var apiContext = CreateContext(
                context: context,
                controller: "Items",
                action: "Update",
                id: id,
                apiRequestBody: GetApiRequestBody(model: model));
            return new ItemModel(
                context: apiContext,
                referenceId: id)
                    .UpdateByServerScript(
                        context: apiContext,
                        model: model);
        }

        public static bool Upsert(Context context, long id, object model)
        {
            var apiContext = CreateContext(
                context: context,
                controller: "Items",
                action: "Upsert",
                id: id,
                apiRequestBody: GetApiRequestBody(model: model));
            return new ItemModel(
                context: apiContext,
                referenceId: id)
                    .UpsertByServerScript(
                        context: apiContext,
                        model: model);
        }

        public static bool Delete(Context context, long id)
        {
            var apiContext = CreateContext(
                context: context,
                controller: "Items",
                action: "Delete",
                id: id,
                apiRequestBody: string.Empty);
            return new ItemModel(
                context: apiContext,
                referenceId: id)
                    .DeleteByServerScript(context: apiContext);
        }

        public static long BulkDelete(Context context, long id, string apiRequestBody)
        {
            var apiContext = CreateContext(
                context: context,
                controller: "Items",
                action: "BulkDelete",
                id: id,
                apiRequestBody: apiRequestBody);
            return new ItemModel(
                context: apiContext,
                referenceId: id)
                    .BulkDeleteByServerScript(context: apiContext);
        }

        public static decimal Aggregate(
            Context context,
            SiteSettings ss,
            string view,
            string columnName,
            Sqls.Functions function)
        {
            if (ss == null)
            {
                return 0;
            }
            var apiContext = CreateContext(
                context: context,
                controller: "Items",
                action: "Aggregate",
                id: ss.SiteId,
                apiRequestBody: view);
            var where = (view.IsNullOrEmpty()
                ? new View()
                : apiContext.RequestDataString.Deserialize<Api>()?.View)
                    ?.Where(
                        context: apiContext,
                        ss: ss);
            var join = ss.Join(
                context: apiContext,
                join: new IJoin[]
                {
                    where
                });
            var column = ss.GetColumn(
                context: apiContext,
                columnName: columnName);
            if (where != null
                && column?.TypeName == "decimal"
                && apiContext.CanRead(ss: ss)
                && column.CanRead(
                    context: apiContext,
                    ss: ss,
                    mine: null,
                    noCache: true))
            {
                switch (ss.ReferenceType)
                {
                    case "Issues":
                        return Repository.ExecuteScalar_decimal(
                            context: apiContext,
                            statements: Rds.SelectIssues(
                                column: Rds.IssuesColumn().Add(
                                    column: column,
                                    function: function),
                                join: join,
                                where: where));
                    case "Results":
                        return Repository.ExecuteScalar_decimal(
                            context: apiContext,
                            statements: Rds.SelectResults(
                                column: Rds.ResultsColumn().Add(
                                    column: column,
                                    function: function),
                                join: join,
                                where: where));
                }
            }
            return 0;
        }

        public static long Aggregate(
            Context context,
            SiteSettings ss,
            string view)
        {
            if (ss == null)
            {
                return 0;
            }
            var apiContext = CreateContext(
                context: context,
                controller: "Items",
                action: "Aggregate",
                id: ss.SiteId,
                apiRequestBody: view);
            var where = (view.IsNullOrEmpty()
                ? new View()
                : apiContext.RequestDataString.Deserialize<Api>()?.View)
                    ?.Where(
                        context: apiContext,
                        ss: ss);
            var join = ss.Join(
                context: context,
                join: new IJoin[]
                {
                    where
                });
            if (where != null)
            {
                switch (ss.ReferenceType)
                {
                    case "Issues":
                        return Repository.ExecuteScalar_long(
                            context: apiContext,
                            statements: Rds.SelectCount(
                                tableName: "Issues",
                                join: join,
                                where: where));
                    case "Results":
                        return Repository.ExecuteScalar_long(
                            context: apiContext,
                            statements: Rds.SelectCount(
                                tableName: "Results",
                                join: join,
                                where: where));
                }
            }
            return 0;
        }

        private static string GetApiRequestBody(object model)
        {
            return model is string issueRequestString
                ? issueRequestString
                : string.Empty;
        }

        public static string ImportItem(Context context, long id, string apiRequestBody, string filePath)
        {
            var apiContext = CreateContext(
                context: context,
                controller: "Items",
                action: "importItem",
                id: id,
                apiRequestBody: apiRequestBody);
            return new ItemModel(
                context: apiContext,
                referenceId: id)
                    .ImportByServerScript(context: apiContext, filePath: filePath);
        }

        public static bool ExportItem(Context context, long id, string apiRequestBody, string filePath)
        {
            var apiContext = CreateContext(
                context: context,
                controller: "Items",
                action: "exportItem",
                id: id,
                apiRequestBody: apiRequestBody);
            return new ItemModel(
                context: apiContext,
                referenceId: id)
                    .ExportByServerScript(context: apiContext, filePath: filePath);
        }
    }
}