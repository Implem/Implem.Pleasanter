using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Implem.Pleasanter.Models;
namespace Implem.Pleasanter.Models
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
            switch (Def.ExtendedColumnTypes.Get(columnName))
            {
                case "Date":
                    value = Date(data, columnName);
                    break;
                default:
                    value = Value(data, columnName);
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

        public static IEnumerable<(string Name, object Value)> Values(BaseItemModel model)
        {
            var values = new List<(string, object)>();
            values.AddRange(model
                .ClassHash
                .Select(element => (element.Key, (object)element.Value)));
            values.AddRange(model
                .NumHash
                .Select(element => (element.Key, (object)element.Value)));
            values.AddRange(model
                .DateHash
                .Select(element => (element.Key, (object)element.Value)));
            values.AddRange(model
                .DescriptionHash
                .Select(element => (element.Key, (object)element.Value)));
            values.AddRange(model
                .CheckHash
                .Select(element => (element.Key, (object)element.Value)));
            if (model is ResultModel resultModel)
            {
                values.Add((nameof(ResultModel.Title), resultModel.Title?.Value));
                values.Add((nameof(ResultModel.Body), resultModel.Body));
                values.Add((nameof(ResultModel.Status), resultModel.Status?.Value));
                values.Add((nameof(ResultModel.Manager), resultModel.Manager.Id));
                values.Add((nameof(ResultModel.Owner), resultModel.Owner.Id));
            }
            if (model is IssueModel issueModel)
            {
                values.Add((nameof(IssueModel.Title), issueModel.Title?.Value));
                values.Add((nameof(IssueModel.Body), issueModel.Body));
                values.Add((nameof(IssueModel.StartTime), issueModel.StartTime));
                values.Add((nameof(IssueModel.CompletionTime), issueModel.CompletionTime.Value));
                values.Add((nameof(IssueModel.WorkValue), issueModel.WorkValue.Value));
                values.Add((nameof(IssueModel.ProgressRate), issueModel.ProgressRate.Value));
                values.Add((nameof(IssueModel.Status), issueModel.Status?.Value));
                values.Add((nameof(IssueModel.Manager), issueModel.Manager.Id));
                values.Add((nameof(IssueModel.Owner), issueModel.Owner.Id));
            }
            return values.ToArray();
        }

        public static IEnumerable<(string Name, ServerScriptModel.ServerScriptModelColumn Value)> Columns(SiteSettings ss)
        {
            var columns = Def
                .ColumnDefinitionCollection
                .Select(definition =>
                {
                    Column column = null;
                    ss?.ColumnHash?.TryGetValue(definition.ColumnName, out column);
                    return (
                        definition.ColumnName,
                        new ServerScriptModel.ServerScriptModelColumn
                        {
                            ReadOnly = !(column?.CanRead == true && column?.CanUpdate == true)
                        });
                })
                .ToArray();
            return columns;
        }

        private static Column[] FilterCanUpdateColumns(
            SiteSettings ss,
            IEnumerable<string> columnNames)
        {
            var columns = columnNames
                .Distinct()
                .Select(columnName => ss.ColumnHash.TryGetValue(columnName, out var column)
                    ? column
                    : null)
                .Where(column => column != null && column.CanRead && column.CanUpdate)
                .ToArray();
            return columns;
        }

        private static void SetColumns(SiteSettings ss, ExpandoObject columns)
        {
            columns?.ForEach(datam =>
            {
                if (!ss.ColumnHash.TryGetValue(datam.Key, out var column))
                {
                    return;
                }
                if (!column.CanUpdate)
                {
                    return;
                }
                column.CanUpdate = (datam.Value as ServerScriptModel.ServerScriptModelColumn)?.ReadOnly == false;
            });
        }

        private static void SetExtendedColumnValues(
            Context context,
            BaseItemModel model,
            ExpandoObject data,
            Column[] columns)
        {
            columns
                ?.ForEach(column => model.Value(
                    context: context,
                    columnName: column.ColumnName,
                    value: String(
                        data: data,
                        columnName: column.ColumnName)));
        }

        private static void SetColumnFilterHachValues(
            Context context,
            View view,
            ExpandoObject columnFilterHach)
        {
            if(view == null)
            {
                return;
            }
            columnFilterHach?.ForEach(columnFilter =>
            {
                if(view.ColumnFilterHash == null)
                {
                    view.ColumnFilterHash = new Dictionary<string, string>();
                }
                view.ColumnFilterHash[columnFilter.Key] = String(columnFilterHach, columnFilter.Key);
            });
        }

        private static void SetValue<T>(
            string columnName,
            Dictionary<string, Column> columns,
            Action<T> setter,
            Func<Column,T> getter)
        {
            if (!columns.TryGetValue(columnName, out var column))
            {
                return;
            }
            var value = getter(column);
            if(column.ChoiceHash?.Any() == true
                && !column.ChoiceHash.ContainsKey(value?.ToString()))
            {
                return;
            }
            setter(value);
        }

        private static void SetResultModelValues(
            Context context,
            BaseItemModel model,
            ExpandoObject data,
            Dictionary<string, Column> columns)
        {
            if (!(model is ResultModel resultModel))
            {
                return;
            }
            SetValue(
                columnName: nameof(ResultModel.Title),
                columns: columns,
                setter: value => resultModel.Title.Value = value,
                getter: column =>  String(
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
        }

        private static void SetIssueModelValues(
            Context context,
            BaseItemModel model,
            ExpandoObject data,
            Dictionary<string, Column> columns)
        {
            if (!(model is IssueModel issueModel))
            {
                return;
            }
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
                setter: value => issueModel.CompletionTime.Value = value,
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
        }

        private static void SetViewValues(
            SiteSettings ss,
            ServerScriptModel.ServerScriptModelSiteSettings data)
        {
            if(ss == null)
            {
                return;
            }
            var viewId = data?.DefaultViewId ?? default;
            ss.GridView = ss?.Views?.Any(v => v.Id == viewId) == true ? viewId : default;
        }

        public static void SetValues(
            Context context,
            SiteSettings ss,
            BaseItemModel model,
            View view,
            ServerScriptModel data)
        {
            var valueColumns = FilterCanUpdateColumns(
                ss: ss,
                columnNames: data.GetChangeItemNames());
            var valueColumnDictionary = valueColumns
                .ToDictionary(
                    column => column.ColumnName,
                    column => column);
            SetColumns(
                ss: ss,
                columns: data.Columns);
            SetExtendedColumnValues(
                context: context,
                model: model,
                data: data.Data,
                columns: valueColumns);
            SetColumnFilterHachValues(
                context: context,
                view: view,
                columnFilterHach: data.View.Filters);
            SetResultModelValues(
                context: context,
                model: model,
                data: data.Data,
                columns: valueColumnDictionary);
            SetIssueModelValues(
                context: context,
                model: model,
                data: data.Data,
                columns: valueColumnDictionary);
            SetViewValues(
                ss: ss,
                data: data.SiteSettings);
        }

        public static void Execute(
            Context context,
            SiteSettings ss,
            BaseItemModel itemModel,
            View view,
            ServerScript[] scripts)
        {
            if (!(Parameters.Script.ServerScript != false
                && context.ContractSettings.Script != false))
            {
                return;
            }
            itemModel = itemModel ?? new BaseItemModel();
            using (var model = new ServerScriptModel(
                context: context,
                ss: ss,
                data: Values(itemModel),
                columns: Columns(ss),
                columnFilterHach: view?.ColumnFilterHash))
            {
                //using (var engine = new Microsoft.ClearScript.V8.V8ScriptEngine(
                //    Microsoft.ClearScript.V8.V8ScriptEngineFlags.EnableDateTimeConversion))
                using (var engine = context.CreateScriptEngin())
                {
                    engine.AddHostObject("context", model.Context);
                    engine.AddHostObject("model", model.Data);
                    engine.AddHostObject("columns", model.Columns);
                    engine.AddHostObject("siteSettings", model.SiteSettings);
                    engine.AddHostObject("view", model.View);
                    foreach (var script in scripts)
                    {
                        engine.Execute(script.Body);
                    }
                }
                SetValues(
                    context: context,
                    ss: ss,
                    model: itemModel,
                    view: view,
                    data: model);
            }
        }

        public static void Execute(
            Context context,
            SiteSettings ss,
            BaseItemModel itemModel,
            View view,
            Func<ServerScript, bool> where)
        {
            if (!(Parameters.Script.ServerScript != false
                && context.ContractSettings.Script != false))
            {
                return;
            }
            var serverScripts = ss
                ?.ServerScripts
                ?.Where(where)
                .ToArray();
            if (serverScripts?.Any() != true)
            {
                return;
            }
            ss.SetColumnAccessControls(
                context: context,
                mine: (itemModel is ResultModel resultModel)
                    ? resultModel.Mine(context: context)
                    : (itemModel is IssueModel issueModel)
                        ? issueModel.Mine(context: context)
                        : null);
            Execute(
                context: context,
                ss: ss,
                itemModel: itemModel,
                view: view,
                scripts: serverScripts);
        }
    }
}