using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
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

        private static bool Bool(ExpandoObject data, string name)
        {
            return Value(data, name).ToBool();
        }

        private static (string, object) ReadNameValue(
            Context context, SiteSettings ss, string columnName, object value)
        {
            return (
                columnName,
                ss?.ColumnHash.Get(columnName)?.CanRead(
                    context: context,
                    ss: ss,
                    mine: null) == true
                        ? value
                        : null);
        }

        public static IEnumerable<(string Name, object Value)> Values(
            Context context, SiteSettings ss, BaseItemModel model)
        {
            var values = new List<(string, object)>();
            values.Add(ReadNameValue(
                context: context,
                ss: ss,
                columnName: nameof(model.SiteId),
                value: model.SiteId));
            values.Add(ReadNameValue(
                context: context,
                ss: ss,
                columnName: nameof(model.Title),
                value: model.Title?.Value));
            values.Add(ReadNameValue(
                context: context,
                ss: ss,
                columnName: nameof(model.Body),
                value: model.Body));
            values.Add(ReadNameValue(
                context: context,
                ss: ss,
                columnName: nameof(model.Ver),
                value: model.Ver));
            values.Add(ReadNameValue(
                context: context,
                ss: ss,
                columnName: nameof(model.Creator),
                value: model.Creator.Id));
            values.Add(ReadNameValue(
                context: context,
                ss: ss,
                columnName: nameof(model.Updator),
                value: model.Updator.Id));
            values.Add(ReadNameValue(
                context: context,
                ss: ss,
                columnName: nameof(model.CreatedTime),
                value: model.CreatedTime?.Value));
            values.Add(ReadNameValue(
                context: context,
                ss: ss,
                columnName: nameof(model.UpdatedTime),
                value: model.UpdatedTime?.Value));
            values.AddRange(model
                .ClassHash
                .Select(element => ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: element.Key,
                    value: element.Value)));
            values.AddRange(model
                .NumHash
                .Select(element => ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: element.Key,
                    value: element.Value.Value)));
            values.AddRange(model
                .DateHash
                .Select(element => ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: element.Key,
                    value: element.Value)));
            values.AddRange(model
                .DescriptionHash
                .Select(element => ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: element.Key,
                    value: element.Value)));
            values.AddRange(model
                .CheckHash
                .Select(element => ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: element.Key,
                    value: element.Value)));
            if (model is ResultModel resultModel)
            {
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(ResultModel.ResultId),
                    value: resultModel.ResultId));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(ResultModel.Status),
                    value: resultModel.Status?.Value));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(ResultModel.Manager),
                    value: resultModel.Manager.Id));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(ResultModel.Owner),
                    value: resultModel.Owner.Id));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(ResultModel.Locked),
                    value: resultModel.Locked));
            }
            if (model is IssueModel issueModel)
            {
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(IssueModel.IssueId),
                    value: issueModel.IssueId));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(IssueModel.StartTime),
                    value: issueModel.StartTime));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(IssueModel.CompletionTime),
                    value: issueModel.CompletionTime.Value));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(IssueModel.WorkValue),
                    value: issueModel.WorkValue.Value));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(IssueModel.ProgressRate),
                    value: issueModel.ProgressRate.Value));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(IssueModel.RemainingWorkValue),
                    value: issueModel.RemainingWorkValue.Value));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(IssueModel.Status),
                    value: issueModel.Status?.Value));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(IssueModel.Manager),
                    value: issueModel.Manager.Id));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(IssueModel.Owner),
                    value: issueModel.Owner.Id));
                values.Add(ReadNameValue(
                    context: context,
                    ss: ss,
                    columnName: nameof(IssueModel.Locked),
                    value: issueModel.Locked));
            }
            return values.ToArray();
        }

        public static IEnumerable<(string Name, ServerScriptModelColumn Value)> Columns(
            Context context, SiteSettings ss)
        {
            var columns = Def
                .ColumnDefinitionCollection
                .Select(definition =>
                {
                    Column column = null;
                    ss?.ColumnHash?.TryGetValue(definition.ColumnName, out column);
                    return (
                        definition.ColumnName,
                        new ServerScriptModelColumn
                        {
                            ReadOnly = !(column?.CanRead(
                                context: context,
                                ss: ss,
                                mine: null) == true && column?.CanUpdate(
                                    context: context,
                                    ss: ss,
                                    mine: null) == true),
                            ExtendedFieldCss = string.Empty,
                            ExtendedCellCss = string.Empty,
                            ExtendedHtmlBeforeField = string.Empty,
                            ExtendedHtmlAfterField = string.Empty,
                            Hide = column?.Hide == true,
                            RawText = string.Empty
                        });
                })
                .ToArray();
            return columns;
        }

        private static Column[] FilterCanUpdateColumns(
            Context context,
            SiteSettings ss,
            IEnumerable<string> columnNames)
        {
            var columns = columnNames
                .Distinct()
                .Select(columnName => ss.ColumnHash.TryGetValue(columnName, out var column)
                    ? column
                    : null)
                .Where(column => column != null
                    && column.CanRead(
                        context: context,
                        ss: ss,
                        mine: null)
                    && column.CanUpdate(
                        context: context,
                        ss: ss,
                        mine: null))
                .ToArray();
            return columns;
        }

        private static Dictionary<string, ServerScriptModelColumn> SetColumns(
            Context context,
            SiteSettings ss,
            ExpandoObject columns)
        {
            var scriptValues = new Dictionary<string, ServerScriptModelColumn>();
            columns?.ForEach(datam =>
            {
                if (!ss.ColumnHash.TryGetValue(datam.Key, out var column))
                {
                    return;
                }
                var serverScriptColumn = datam.Value as ServerScriptModelColumn;
                scriptValues[datam.Key] = new ServerScriptModelColumn
                {
                    ChoiceHash = serverScriptColumn?.ChoiceHash,
                    ExtendedFieldCss = serverScriptColumn?.ExtendedFieldCss,
                    ExtendedCellCss = serverScriptColumn?.ExtendedCellCss,
                    ExtendedHtmlBeforeField = serverScriptColumn?.ExtendedHtmlBeforeField,
                    ExtendedHtmlAfterField = serverScriptColumn?.ExtendedHtmlAfterField,
                    Hide = serverScriptColumn?.Hide == true,
                    RawText = serverScriptColumn?.RawText,
                    ReadOnly = !(column.CanUpdate(
                        context: context,
                        ss: ss,
                        mine: null)
                            && serverScriptColumn?.ReadOnly != true)
                };
            });
            return scriptValues;
        }

        private static ServerScriptModelRow SetRow(
            Context context,
            SiteSettings ss,
            ExpandoObject model,
            ExpandoObject columns,
            ServerScriptModelHidden hidden)
        {
            var row = new ServerScriptModelRow
            {
                ExtendedRowCss = String(model, nameof(ServerScriptModelRow.ExtendedRowCss)),
                Columns = SetColumns(
                    context: context,
                    ss: ss,
                    columns: columns),
                Hidden = hidden.GetAll()
            };
            return row;
        }

        private static void SetExtendedColumnValues(
            Context context,
            BaseItemModel model,
            ExpandoObject data,
            Column[] columns)
        {
            columns?.ForEach(column => model.Value(
                context: context,
                column: column,
                value: String(
                    data: data,
                    columnName: column.ColumnName)));
        }

        private static void SetColumnFilterHashValues(
            Context context,
            View view,
            ExpandoObject columnFilterHash)
        {
            if (view == null)
            {
                return;
            }
            columnFilterHash?.ForEach(columnFilter =>
            {
                if (view.ColumnFilterHash == null)
                {
                    view.ColumnFilterHash = new Dictionary<string, string>();
                }
                view.ColumnFilterHash[columnFilter.Key] = Value(columnFilterHash, columnFilter.Key).ToString();
            });
        }

        private static void SetColumnSorterHashValues(
            Context context,
            View view,
            ExpandoObject columnSorterHash)
        {
            if (view == null)
            {
                return;
            }
            columnSorterHash?.ForEach(columnFilter =>
            {
                if (view.ColumnSorterHash == null)
                {
                    view.ColumnSorterHash = new Dictionary<string, SqlOrderBy.Types>();
                }
                if (Enum.TryParse<SqlOrderBy.Types>(String(columnSorterHash, columnFilter.Key), out var value))
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
            if (column.ChoiceHash?.Any() == true
                && !column.ChoiceHash.ContainsKey(value?.ToString()))
            {
                return;
            }
            setter(value);
        }

        private static void SetResultModelValues(
            Context context,
            ResultModel resultModel,
            ExpandoObject data,
            Dictionary<string, Column> columns)
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
        }

        private static void SetIssueModelValues(
            Context context,
            IssueModel issueModel,
            ExpandoObject data,
            Dictionary<string, Column> columns)
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
            SetValue(
                columnName: nameof(IssueModel.Locked),
                columns: columns,
                setter: value => issueModel.Locked = value,
                getter: column => Bool(
                    data: data,
                    name: column.Name));
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
                hidden: data.Hidden);
            SetExtendedColumnValues(
                context: context,
                model: model,
                data: data.Model,
                columns: valueColumns);
            SetColumnFilterHashValues(
                context: context,
                view: view,
                columnFilterHash: data.View.Filters);
            SetColumnSorterHashValues(
                context: context,
                view: view,
                columnSorterHash: data.View.Sorters);
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
                            issueModel: issueModel,
                            data: data.Model,
                            columns: valueColumnDictionary);
                    }
                    break;
                case "Results":
                    if (model is ResultModel resultModel)
                    {
                        SetResultModelValues(
                            context: context,
                            resultModel: resultModel,
                            data: data.Model,
                            columns: valueColumnDictionary);
                    }
                    break;
            }
            SetViewValues(
                ss: ss,
                data: data.SiteSettings);
            return scriptValues;
        }

        public static ServerScriptModelRow Execute(
            Context context,
            SiteSettings ss,
            BaseItemModel itemModel,
            View view,
            ServerScript[] scripts,
            bool onTesting = false)
        {
            if (!(Parameters.Script.ServerScript != false
                && context.ContractSettings.NewFeatures()
                && context.ContractSettings.Script != false))
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
                data: Values(
                    context: context,
                    ss: ss,
                    model: itemModel),
                columns: Columns(
                    context: context,
                    ss: ss),
                columnFilterHash: view?.ColumnFilterHash,
                columnSorterHash: view?.ColumnSorterHash,
                onTesting: onTesting))
            {
                using (var engine = new Microsoft.ClearScript.V8.V8ScriptEngine(
                    Microsoft.ClearScript.V8.V8ScriptEngineFlags.EnableDateTimeConversion))
                {
                    try
                    {
                        engine.ContinuationCallback = model.ContinuationCallback;
                        engine.AddHostObject("context", model.Context);
                        engine.AddHostObject("model", model.Model);
                        engine.AddHostObject("depts", model.Depts);
                        engine.AddHostObject("groups", model.Groups);
                        engine.AddHostObject("users", model.Users);
                        engine.AddHostObject("columns", model.Columns);
                        engine.AddHostObject("siteSettings", model.SiteSettings);
                        engine.AddHostObject("view", model.View);
                        engine.AddHostObject("items", model.Items);
                        engine.AddHostObject("hidden", model.Hidden);
                        engine.AddHostObject("extendedSql", model.ExtendedSql);
                        engine.AddHostObject("notifications", model.Notification);
                        foreach (var script in scripts)
                        {
                            engine.Execute(script.Body);
                        }
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
                    view: view,
                    data: model);
            }
            return scriptValues;
        }

        public static ServerScriptModelRow Execute(
            Context context,
            SiteSettings ss,
            BaseItemModel itemModel,
            View view,
            Func<ServerScript, bool> where)
        {
            if (!(Parameters.Script.ServerScript != false
                && context.ContractSettings.NewFeatures()
                && context.ContractSettings.Script != false))
            {
                return null;
            }
            var serverScripts = ss
                ?.GetServerScripts(context: context)
                ?.Where(where)
                .ToArray();
            if (serverScripts?.Any() != true)
            {
                return null;
            }
            var scriptValues = Execute(
                context: context,
                ss: ss,
                itemModel: itemModel,
                view: view,
                scripts: serverScripts);
            return scriptValues;
        }

        private static bool ReadOnly(
            string columnName,
            IEnumerable<ServerScriptModelRow> serverScriptModelRows)
        {
            var readOnly = serverScriptModelRows
                ?.Select(row => row?.Columns?.Get(columnName))
                ?.Any(column => column?.ReadOnly == true) == true;
            return readOnly;
        }

        public static bool Hide(IEnumerable<ServerScriptModelColumn> serverScriptModelColumns)
        {
            var hide = serverScriptModelColumns
                ?.Any(column => column?.Hide == true) == true;
            return hide;
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
            if (baseModel == null || baseModel.ServerScriptModelRows?.Any() != true)
            {
                return column.CanEdit(
                    context: context,
                    ss: ss,
                    mine: baseModel?.Mine(context: context));
            }
            var serverScriptReadOnly = ReadOnly(
                columnName: column.ColumnName,
                serverScriptModelRows: baseModel?.ServerScriptModelRows);
            var canUpdate = column.CanEdit(
                context: context,
                ss: ss,
                mine: baseModel?.Mine(context: context))
                    && !serverScriptReadOnly;
            return canUpdate;
        }

        public static Context CreateContext(Context context, long id, string apiRequestBody)
        {
            var createdContext = new Context();
            createdContext.LogBuilder = context.LogBuilder;
            createdContext.Id = id;
            createdContext.ApiRequestBody = apiRequestBody;
            createdContext.PermissionHash = Permissions.Get(context: createdContext);
            createdContext.ServerScriptDepth = context.ServerScriptDepth + 1;
            return createdContext;
        }

        public static ServerScriptModelApiModel[] Get(
            Context context,
            long id,
            string view,
            bool onTesting)
        {
            var itemModels = new ItemModel(context: context, referenceId: id).GetByServerScript(
                context: context,
                apiContext: CreateContext(
                    context: context,
                    id: id,
                    apiRequestBody: view)) ?? new BaseItemModel[0];
            var items = itemModels.Select(model => new ServerScriptModelApiModel(
                context: context,
                model: model,
                onTesting: onTesting)).ToArray();
            return items;
        }

        public static bool Create(Context context, long id, object model)
        {
            return new ItemModel(context: context, referenceId: id).CreateByServerScript(
                context: context,
                apiContext: CreateContext(
                    context: context,
                    id: id,
                    apiRequestBody: string.Empty),
                model: model);
        }

        public static bool Update(Context context, long id, object model)
        {
            return new ItemModel(context: context, referenceId: id).UpdateByServerScript(
                context: context,
                apiContext: CreateContext(
                    context: context,
                    id: id,
                    apiRequestBody: string.Empty),
                model: model);
        }

        public static bool Delete(Context context, long id)
        {
            return new ItemModel(context: context, referenceId: id).DeleteByServerScript(
                context: context,
                apiContext: CreateContext(
                    context: context,
                    id: id,
                    apiRequestBody: string.Empty));
        }

        public static long BulkDelete(Context context, long id, string apiRequestBody)
        {
            return new ItemModel(context: context, referenceId: id).BulkDeleteByServerScript(
                context: context,
                apiContext: CreateContext(
                    context: context,
                    id: id,
                    apiRequestBody: apiRequestBody));
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
                id: ss.SiteId,
                apiRequestBody: view);
            var where = (view.IsNullOrEmpty()
                ? new View()
                : apiContext.RequestDataString.Deserialize<Api>()?.View)
                    ?.Where(
                        context: apiContext,
                        ss: ss);
            var column = ss.GetColumn(
                context: apiContext,
                columnName: columnName);
            if(where != null
                && column?.TypeName == "decimal"
                && apiContext.CanRead(ss: ss)
                && column.CanRead(
                    context: context,
                    ss: ss,
                    mine: null))
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
                                where: where));
                    case "Results":
                        return Repository.ExecuteScalar_decimal(
                            context: apiContext,
                            statements: Rds.SelectResults(
                                column: Rds.ResultsColumn().Add(
                                    column: column,
                                    function: function),
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
                id: ss.SiteId,
                apiRequestBody: view);
            var where = (view.IsNullOrEmpty()
                ? new View()
                : apiContext.RequestDataString.Deserialize<Api>()?.View)
                    ?.Where(
                        context: apiContext,
                        ss: ss);
            if (where != null
                && apiContext.CanRead(ss: ss))
            {
                switch (ss.ReferenceType)
                {
                    case "Issues":
                        return Repository.ExecuteScalar_long(
                            context: context,
                            statements: Rds.SelectCount(
                                tableName: "Issues",
                                where: where));
                    case "Results":
                        return Repository.ExecuteScalar_long(
                            context: context,
                            statements: Rds.SelectCount(
                                tableName: "Results",
                                where: where));
                }
            }
            return 0;
        }
    }
}