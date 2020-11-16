using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
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
                values.Add((nameof(ResultModel.Status), resultModel.Status?.Value));
            }
            if (model is IssueModel issueModel)
            {
                values.Add((nameof(IssueModel.Status), issueModel.Status?.Value));
            }
            return values.ToArray();
        }

        public static IEnumerable<(string Name, bool Value)> ReadOnlys(SiteSettings ss)
        {
            var readOnlys = ss
                ?.EditorColumnHash
                ?.Values
                .SelectMany(columnName => columnName)
                .Select(columnName => ss
                    .ColumnHash
                    .TryGetValue(columnName, out var column) ? column : null)
                .Where(column => column != null)
                .Select(column => (column.ColumnName, column.CanRead && column.CanUpdate))
                .ToArray();
            return readOnlys;
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

        private static void SetReadOnlys(SiteSettings ss, ExpandoObject readOnlys)
        {
            readOnlys?.ForEach(readOnly =>
            {
                if (!ss.ColumnHash.TryGetValue(readOnly.Key, out var column))
                {
                    return;
                }
                if (!column.CanUpdate)
                {
                    return;
                }
                column.CanUpdate = readOnly.Value as bool? ?? default(bool);
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

        private static void SetValue(
            string columnName,
            Dictionary<string, Column> columns,
            Action<Column> setter)
        {
            if (!columns.TryGetValue(columnName, out var column))
            {
                return;
            }
            setter(column);
        }

        private static void SetResultModelValues(
            BaseItemModel model,
            ExpandoObject data,
            Dictionary<string, Column> columns)
        {
            if (!(model is ResultModel resultModel))
            {
                return;
            }
            SetValue(
                columnName: nameof(ResultModel.Status),
                columns: columns,
                setter: column => resultModel.Status.Value = Int(
                    data: data,
                    name: column.ColumnName));
        }

        private static void SetIssueModelValues(
            BaseItemModel model,
            ExpandoObject data,
            Dictionary<string, Column> columns)
        {
            if (!(model is IssueModel issueModel))
            {
                return;
            }
            SetValue(
                columnName: nameof(IssueModel.Status),
                columns: columns,
                setter: column => issueModel.Status.Value = Int(
                    data: data,
                    name: column.ColumnName));
        }

        public static void SetValues(
            Context context,
            SiteSettings ss,
            BaseItemModel model,
            ServerScriptModel data)
        {
            var valueColumns = FilterCanUpdateColumns(
                ss: ss,
                columnNames: data.GetChangeItemNames());
            var valueColumnDictionary = valueColumns
                .ToDictionary(
                    column => column.ColumnName,
                    column => column);
            SetReadOnlys(
                ss: ss,
                readOnlys: data.ReadOnly);
            SetExtendedColumnValues(
                context: context,
                model: model,
                data: data.Data,
                columns: valueColumns);
            SetResultModelValues(
                model: model,
                data: data.Data,
                columns: valueColumnDictionary);
            SetIssueModelValues(model: model,
                data: data.Data,
                columns: valueColumnDictionary);
        }

        public static void Execute(
            Context context,
            SiteSettings ss,
            BaseItemModel itemModel,
            ServerScript[] scripts)
        {
            using (var model = new ServerScriptModel(
                context: context,
                data: Values(itemModel),
                readOnly: ReadOnlys(ss)))
            {
                using (var engine = new Microsoft.ClearScript.V8.V8ScriptEngine(
                    Microsoft.ClearScript.V8.V8ScriptEngineFlags.EnableDateTimeConversion))
                {
                    engine.AddHostObject("context", model.Context);
                    engine.AddHostObject("model", model.Data);
                    engine.AddHostObject("readOnly", model.ReadOnly);
                    foreach (var script in scripts)
                    {
                        engine.Execute(script.Body);
                    }
                }
                SetValues(
                    context: context,
                    ss: ss,
                    model: itemModel,
                    data: model);
            }
        }

        public static void Execute(
            Context context,
            SiteSettings ss,
            BaseItemModel itemModel,
            Func<ServerScript, bool> where)
        {
            if (!Parameters.Script.ServerScript)
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
                scripts: serverScripts);
        }
    }
}