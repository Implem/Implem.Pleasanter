using Implem.Libraries.DataSources.Interfaces;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Exceptions;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
namespace Implem.Pleasanter.Libraries.Models
{
    public class GridData
    {
        public Databases.AccessStatuses AccessStatus = Databases.AccessStatuses.Initialized;
        public EnumerableRowCollection<DataRow> DataRows;
        public int TotalCount;

        public GridData(
            Context context,
            SiteSettings ss,
            View view,
            Sqls.TableTypes? tableType = null,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            int top = 0,
            int offset = 0,
            int pageSize = 0,
            bool count = true)
        {
            Get(
                context: context,
                ss: ss,
                view: view,
                tableType: tableType ?? ((view.ShowHistory == true)
                    ? Sqls.TableTypes.NormalAndHistory
                    : ss.TableType),
                column: column,
                join: join,
                where: where,
                top: top,
                offset: offset,
                pageSize: pageSize,
                count: count);
        }

        private void Get(
            Context context,
            SiteSettings ss,
            View view,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            int top = 0,
            int offset = 0,
            int pageSize = 0,
            bool count = true)
        {
            var gridColumns = ss.GetGridColumns(
                context: context,
                view: view,
                includedColumns: true);
            if (!gridColumns.Any(o => o.ColumnName == "Ver"))
            {
                gridColumns.Add(ss.GetColumn(
                    context: context,
                    columnName: "Ver"));
            }
            column = column ?? ColumnUtilities.SqlColumnCollection(
                context: context,
                ss: ss,
                view: view,
                columns: gridColumns);
            where = view.Where(
                context: context,
                ss: ss,
                where: where);
            var orderBy = view.OrderBy(
                context: context,
                ss: ss);
            join = join ?? ss.Join(
                context: context,
                join: new IJoin[]
                {
                    column,
                    where,
                    orderBy
                });
            var param = view.Param(
                context: context,
                ss: ss);
            var statements = new List<SqlStatement>
            {
                Rds.Select(
                tableName: ss.ReferenceType,
                tableType: tableType,
                dataTableName: "Main",
                column: column,
                join: join,
                where: where,
                orderBy: orderBy,
                param: param,
                top: top,
                offset: offset,
                pageSize: pageSize)
            };
            if (count)
            {
                statements.Add(Rds.SelectCount(
                    tableName: ss.ReferenceType,
                    tableType: tableType,
                    join: join,
                    where: where));
            }
            DataSet dataSet;
            try
            {
                dataSet = Repository.ExecuteDataSet(
                    context: context,
                    transactional: false,
                    statements: statements.ToArray());
            }
            catch (System.Exception e)
            {
                Views.SetSession(
                    context: context,
                    ss: ss,
                    view: new View(),
                    setSession: true,
                    key: "View",
                    useUsersView: ss.SaveViewType == SiteSettings.SaveViewTypes.User);
                var message = Messages.CanNotGridSort(context: context);
                context.Messages.Add(message);
                throw new CanNotGridSortException(message.Text, e);
            }
            DataRows = dataSet.Tables["Main"].AsEnumerable();
            TotalCount = Rds.Count(dataSet);
            ss.SetChoiceHash(
                context: context,
                dataRows: DataRows);
        }

        public List<Dictionary<string, object>> KeyValues(
            Context context,
            SiteSettings ss,
            View view)
        {
            var data = new List<Dictionary<string, object>>();
            var columns = ss.GetGridColumns(
                context: context,
                view: view,
                checkPermission: true);
            foreach (var dataRow in DataRows)
            {
                var rowData = new Dictionary<string, object>();
                var depts = new Dictionary<string, DeptModel>();
                var groups = new Dictionary<string, GroupModel>();
                var registrations = new Dictionary<string, RegistrationModel>();
                var sites = new Dictionary<string, SiteModel>();
                var sysLogs = new Dictionary<string, SysLogModel>();
                var tenants = new Dictionary<string, TenantModel>();
                var users = new Dictionary<string, UserModel>();
                var dashboards = new Dictionary<string, DashboardModel>();
                var issues = new Dictionary<string, IssueModel>();
                var results = new Dictionary<string, ResultModel>();
                var wikis = new Dictionary<string, WikiModel>();
                foreach (var column in columns)
                {
                    var key = column.TableName();
                    var apiColumn = view.ApiColumnHash.Get(column.ColumnName);
                    var keyDisplayType = apiColumn?.KeyDisplayType ?? view.ApiColumnKeyDisplayType;
                    var valueDisplayType = apiColumn?.ValueDisplayType ?? view.ApiColumnValueDisplayType;
                    string columnKey = string.Empty;
                    switch (keyDisplayType)
                    {
                        case ApiColumn.KeyDisplayTypes.LabelText:
                            columnKey = column.LabelText;
                            break;
                        case ApiColumn.KeyDisplayTypes.ColumnName:
                            columnKey = column.ColumnName;
                            break;
                    }
                    switch (column.SiteSettings?.ReferenceType)
                    {
                        case "Depts":
                            var deptModel = depts.Get(key);
                            if (deptModel == null)
                            {
                                deptModel = new DeptModel(
                                    context: context,
                                    ss: column.SiteSettings,
                                    dataRow: dataRow,
                                    tableAlias: column.TableAlias);
                                depts.Add(key, deptModel);
                                ss.ClearColumnAccessControlCaches(baseModel: deptModel);
                            }
                            switch (valueDisplayType)
                            {
                                case ApiColumn.ValueDisplayTypes.Value:
                                    rowData.AddIfNotConainsKey(columnKey, deptModel.ToApiValue(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        mine: deptModel.Mine(context: context)));
                                    break;
                                case ApiColumn.ValueDisplayTypes.Text:
                                    rowData.AddIfNotConainsKey(columnKey, deptModel.ToDisplay(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        mine: deptModel.Mine(context: context)));
                                    break;
                                default:
                                    rowData.AddIfNotConainsKey(columnKey, deptModel.ToApiDisplayValue(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        mine: deptModel.Mine(context: context)));
                                    break;
                            }
                            break;
                        case "Groups":
                            var groupModel = groups.Get(key);
                            if (groupModel == null)
                            {
                                groupModel = new GroupModel(
                                    context: context,
                                    ss: column.SiteSettings,
                                    dataRow: dataRow,
                                    tableAlias: column.TableAlias);
                                groups.Add(key, groupModel);
                                ss.ClearColumnAccessControlCaches(baseModel: groupModel);
                            }
                            switch (valueDisplayType)
                            {
                                case ApiColumn.ValueDisplayTypes.Value:
                                    rowData.AddIfNotConainsKey(columnKey, groupModel.ToApiValue(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        mine: groupModel.Mine(context: context)));
                                    break;
                                case ApiColumn.ValueDisplayTypes.Text:
                                    rowData.AddIfNotConainsKey(columnKey, groupModel.ToDisplay(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        mine: groupModel.Mine(context: context)));
                                    break;
                                default:
                                    rowData.AddIfNotConainsKey(columnKey, groupModel.ToApiDisplayValue(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        mine: groupModel.Mine(context: context)));
                                    break;
                            }
                            break;
                        case "SysLogs":
                            var sysLogModel = sysLogs.Get(key);
                            if (sysLogModel == null)
                            {
                                sysLogModel = new SysLogModel(
                                    context: context,
                                    ss: column.SiteSettings,
                                    dataRow: dataRow,
                                    tableAlias: column.TableAlias);
                                sysLogs.Add(key, sysLogModel);
                                ss.ClearColumnAccessControlCaches(baseModel: sysLogModel);
                            }
                            switch (valueDisplayType)
                            {
                                case ApiColumn.ValueDisplayTypes.Value:
                                    rowData.AddIfNotConainsKey(columnKey, sysLogModel.ToApiValue(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        mine: sysLogModel.Mine(context: context)));
                                    break;
                                case ApiColumn.ValueDisplayTypes.Text:
                                    rowData.AddIfNotConainsKey(columnKey, sysLogModel.ToDisplay(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        mine: sysLogModel.Mine(context: context)));
                                    break;
                                default:
                                    rowData.AddIfNotConainsKey(columnKey, sysLogModel.ToApiDisplayValue(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        mine: sysLogModel.Mine(context: context)));
                                    break;
                            }
                            break;
                        case "Users":
                            var userModel = users.Get(key);
                            if (userModel == null)
                            {
                                userModel = new UserModel(
                                    context: context,
                                    ss: column.SiteSettings,
                                    dataRow: dataRow,
                                    tableAlias: column.TableAlias);
                                users.Add(key, userModel);
                                ss.ClearColumnAccessControlCaches(baseModel: userModel);
                            }
                            switch (valueDisplayType)
                            {
                                case ApiColumn.ValueDisplayTypes.Value:
                                    rowData.AddIfNotConainsKey(columnKey, userModel.ToApiValue(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        mine: userModel.Mine(context: context)));
                                    break;
                                case ApiColumn.ValueDisplayTypes.Text:
                                    rowData.AddIfNotConainsKey(columnKey, userModel.ToDisplay(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        mine: userModel.Mine(context: context)));
                                    break;
                                default:
                                    rowData.AddIfNotConainsKey(columnKey, userModel.ToApiDisplayValue(
                                        context: context,
                                        ss: ss,
                                        column: column,
                                        mine: userModel.Mine(context: context)));
                                    break;
                            }
                            break;
                        case "Issues":
                            var issueModel = issues.Get(key);
                            if (issueModel == null)
                            {
                                issueModel = new IssueModel(
                                    context: context,
                                    ss: column.SiteSettings,
                                    dataRow: dataRow,
                                    tableAlias: column.TableAlias);
                                issues.Add(key, issueModel);
                                ss.ClearColumnAccessControlCaches(baseModel: issueModel);
                            }
                            if (column.ColumnName.Contains("~")
                                && !Permissions.CanRead(
                                    context: context,
                                    siteId: issueModel.SiteId,
                                    id: issueModel.IssueId))
                            {
                                continue;
                            }
                            else
                            {
                                switch (valueDisplayType)
                                {
                                    case ApiColumn.ValueDisplayTypes.Value:
                                        rowData.AddIfNotConainsKey(columnKey, issueModel.ToApiValue(
                                            context: context,
                                            ss: ss,
                                            column: column,
                                            mine: issueModel.Mine(context: context)));
                                        break;
                                    case ApiColumn.ValueDisplayTypes.Text:
                                        rowData.AddIfNotConainsKey(columnKey, issueModel.ToDisplay(
                                            context: context,
                                            ss: ss,
                                            column: column,
                                            mine: issueModel.Mine(context: context)));
                                        break;
                                    default:
                                        rowData.AddIfNotConainsKey(columnKey, issueModel.ToApiDisplayValue(
                                            context: context,
                                            ss: ss,
                                            column: column,
                                            mine: issueModel.Mine(context: context)));
                                        break;
                                }
                            }
                            break;
                        case "Results":
                            var resultModel = results.Get(key);
                            if (resultModel == null)
                            {
                                resultModel = new ResultModel(
                                    context: context,
                                    ss: column.SiteSettings,
                                    dataRow: dataRow,
                                    tableAlias: column.TableAlias);
                                results.Add(key, resultModel);
                                ss.ClearColumnAccessControlCaches(baseModel: resultModel);
                            }
                            if (column.ColumnName.Contains("~")
                                && !Permissions.CanRead(
                                    context: context,
                                    siteId: resultModel.SiteId,
                                    id: resultModel.ResultId))
                            {
                                continue;
                            }
                            else
                            {
                                switch (valueDisplayType)
                                {
                                    case ApiColumn.ValueDisplayTypes.Value:
                                        rowData.AddIfNotConainsKey(columnKey, resultModel.ToApiValue(
                                            context: context,
                                            ss: ss,
                                            column: column,
                                            mine: resultModel.Mine(context: context)));
                                        break;
                                    case ApiColumn.ValueDisplayTypes.Text:
                                        rowData.AddIfNotConainsKey(columnKey, resultModel.ToDisplay(
                                            context: context,
                                            ss: ss,
                                            column: column,
                                            mine: resultModel.Mine(context: context)));
                                        break;
                                    default:
                                        rowData.AddIfNotConainsKey(columnKey, resultModel.ToApiDisplayValue(
                                            context: context,
                                            ss: ss,
                                            column: column,
                                            mine: resultModel.Mine(context: context)));
                                        break;
                                }
                            }
                            break;
                    }
                }
                data.Add(rowData);
            }
            return data;
        }

        public System.Text.StringBuilder Csv(
            Context context,
            SiteSettings ss,
            View view,
            System.Text.StringBuilder csv,
            IEnumerable<ExportColumn> exportColumns,
            string delimiter,
            bool? encloseDoubleQuotes)
        {
            var idColumn = Rds.IdColumn(ss.ReferenceType);
            DataRows.ForEach(dataRow =>
            {
                var dataId = dataRow.Long(idColumn).ToString();
                var data = new List<string>();
                var depts = new Dictionary<string, DeptModel>();
                var groups = new Dictionary<string, GroupModel>();
                var registrations = new Dictionary<string, RegistrationModel>();
                var sites = new Dictionary<string, SiteModel>();
                var sysLogs = new Dictionary<string, SysLogModel>();
                var tenants = new Dictionary<string, TenantModel>();
                var users = new Dictionary<string, UserModel>();
                var dashboards = new Dictionary<string, DashboardModel>();
                var issues = new Dictionary<string, IssueModel>();
                var results = new Dictionary<string, ResultModel>();
                var wikis = new Dictionary<string, WikiModel>();
                ServerScriptModelRow serverScriptModelRow = null;
                switch (ss.ReferenceType)
                {
                    case "Issues":
                        var issueModel = new IssueModel(
                            context: context,
                            ss: ss,
                            dataRow: dataRow);
                        ss.ClearColumnAccessControlCaches(baseModel: issueModel);
                        serverScriptModelRow = issueModel?.SetByBeforeOpeningRowServerScript(
                            context: context,
                            ss: ss,
                            view: view);
                        issues.Add("Issues", issueModel);
                        break;
                    case "Results":
                        var resultModel = new ResultModel(
                            context: context,
                            ss: ss,
                            dataRow: dataRow);
                        ss.ClearColumnAccessControlCaches(baseModel: resultModel);
                        serverScriptModelRow = resultModel?.SetByBeforeOpeningRowServerScript(
                            context: context,
                            ss: ss,
                            view: view);
                        results.Add("Results", resultModel);
                        break;
                };
                exportColumns.ForEach(exportColumn =>
                {
                    var column = exportColumn.Column;
                    var key = column.TableName();
                    var serverScriptModelColumn = serverScriptModelRow
                        ?.Columns
                        ?.Get(column?.ColumnName);
                    if (serverScriptModelColumn?.RawText.IsNullOrEmpty() == false)
                    {
                        data.Add(CsvUtilities.EncloseDoubleQuotes(
                            value: serverScriptModelColumn.RawText,
                            encloseDoubleQuotes: encloseDoubleQuotes));                        
                    }
                    else
                    {
                        switch (column.SiteSettings?.ReferenceType)
                        {
                            case "Users":
                                if (!users.ContainsKey(key))
                                {
                                    users.Add(key, new UserModel(
                                        context: context,
                                        ss: column.SiteSettings,
                                        dataRow: dataRow,
                                        tableAlias: column.TableAlias));
                                }
                                data.Add(users.Get(key).CsvData(
                                    context: context,
                                    ss: column.SiteSettings,
                                    column: column,
                                    exportColumn: exportColumn,
                                    mine: users.Get(key).Mine(context: context),
                                    encloseDoubleQuotes: encloseDoubleQuotes));
                                break;
                            case "Issues":
                                if (!issues.ContainsKey(key))
                                {
                                    issues.Add(key, new IssueModel(
                                        context: context,
                                        ss: column.SiteSettings,
                                        dataRow: dataRow,
                                        tableAlias: column.TableAlias));
                                }
                                data.Add(issues.Get(key).CsvData(
                                    context: context,
                                    ss: column.SiteSettings,
                                    column: column,
                                    exportColumn: exportColumn,
                                    mine: issues.Get(key).Mine(context: context),
                                    encloseDoubleQuotes: encloseDoubleQuotes));
                                break;
                            case "Results":
                                if (!results.ContainsKey(key))
                                {
                                    results.Add(key, new ResultModel(
                                        context: context,
                                        ss: column.SiteSettings,
                                        dataRow: dataRow,
                                        tableAlias: column.TableAlias));
                                }
                                data.Add(results.Get(key).CsvData(
                                    context: context,
                                    ss: column.SiteSettings,
                                    column: column,
                                    exportColumn: exportColumn,
                                    mine: results.Get(key).Mine(context: context),
                                    encloseDoubleQuotes: encloseDoubleQuotes));
                                break;
                        }
                    }
                });
                csv.Append(data.Join(delimiter), "\n");
            });
            return csv;
        }

        public string Json(
            Context context,
            SiteSettings ss,
            Export export)
        {
            var data = new JsonExport();
            var idColumn = Rds.IdColumn(ss.ReferenceType);
            if (export.Header == true)
            {
                data.Header = new List<JsonExportColumn>();
                export.Columns
                    .Where(o => o.Column.CanRead(
                        context: context,
                        ss: ss,
                        mine: null))
                    .ForEach(exportColumn =>
                    {
                        if (!data.Header.Any(o => o.SiteId == exportColumn.Column.SiteId))
                        {
                            data.Header.Add(new JsonExportColumn(
                                siteId: exportColumn.Column.SiteId,
                                siteTitle: exportColumn.SiteTitle));
                        }
                        data.Header.FirstOrDefault(o => o.SiteId == exportColumn.Column.SiteId)
                            ?.Columns.AddIfNotConainsKey(
                                exportColumn.Column.Name,
                                exportColumn.GetLabelText());
                    });
            }
            data.Body = new List<IExportModel>();
            DataRows
                .GroupBy(o => o.Long(idColumn))
                .Select(o => o.ToList())
                .ForEach(dataRows =>
                {
                    data.Body.Add(JsonStacks(
                        context: context,
                        ss: ss,
                        idColumn: idColumn,
                        dataRows: dataRows));
                });
            return data.ToJson(formatting: Formatting.Indented);
        }

        public JsonExport GetJsonExport(
            Context context,
            SiteSettings ss,
            Export export)
        {
            var data = new JsonExport();
            var idColumn = Rds.IdColumn(ss.ReferenceType);
            if (export.Header == true)
            {
                data.Header = new List<JsonExportColumn>();
                export.Columns
                    .Where(o => o.Column.CanRead(
                        context: context,
                        ss: ss,
                        mine: null))
                    .ForEach(exportColumn =>
                    {
                        if (!data.Header.Any(o => o.SiteId == exportColumn.SiteId))
                        {
                            data.Header.Add(new JsonExportColumn(
                                siteId: exportColumn.SiteId ?? 0,
                                siteTitle: exportColumn.SiteTitle));
                        }
                        data.Header.FirstOrDefault(o => o.SiteId == exportColumn.SiteId)
                            ?.Columns.AddIfNotConainsKey(
                                exportColumn.Column.Name,
                                exportColumn.GetLabelText());
                    });
            }
            data.Body = new List<IExportModel>();
            DataRows
                .GroupBy(o => o.Long(idColumn))
                .Select(o => o.ToList())
                .ForEach(dataRows =>
                {
                    data.Body.Add(JsonStacks(
                        context: context,
                        ss: ss,
                        idColumn: idColumn,
                        dataRows: dataRows));
                });
            return data;
        }

        private IExportModel JsonStacks(
            Context context,
            SiteSettings ss,
            string idColumn,
            List<DataRow> dataRows,
            string tableAlias = null)
        {
            IExportModel exportModel = null;
            switch (ss.ReferenceType)
            {
                case "Issues":
                    exportModel = new IssueExportModel(
                        context: context,
                        ss: ss,
                        dataRow: dataRows.First(),
                        tableAlias: tableAlias);
                    break;
                case "Results":
                    exportModel = new ResultExportModel(
                        context: context,
                        ss: ss,
                        dataRow: dataRows.First(),
                        tableAlias: tableAlias);
                    break;
            }
            SetDestinations(
                context: context,
                ss: ss,
                exportModel: exportModel,
                dataRows: dataRows);
            SetSources(
                context: context,
                ss: ss,
                exportModel: exportModel,
                dataRows: dataRows);
            return exportModel;
        }

        private void SetDestinations(
            Context context,
            SiteSettings ss,
            IExportModel exportModel,
            List<DataRow> dataRows)
        {
            ss.Destinations?.Values.ForEach(currentSs =>
            {
                currentSs.JoinStacks.ForEach(joinStack =>
                {
                    var tableAlias = joinStack.TableName();
                    var idColumn = tableAlias + "," + Rds.IdColumn(currentSs.ReferenceType);
                    dataRows
                        .GroupBy(o => o.Long(idColumn))
                        .Where(o => o.Key > 0)
                        .Select(o => o.ToList())
                        .ForEach(groupedDataRows =>
                        {
                            exportModel.AddDestination(
                                exportModel: JsonStacks(
                                    context: context,
                                    ss: currentSs,
                                    idColumn: idColumn,
                                    dataRows: groupedDataRows,
                                    tableAlias: tableAlias),
                                columnName: joinStack.ColumnName);
                        });
                });
            });
        }

        private void SetSources(
            Context context,
            SiteSettings ss,
            IExportModel exportModel,
            List<DataRow> dataRows)
        {
            ss.Sources?.Values.ForEach(currentSs =>
            {
                var tableAlias = currentSs.JoinStacks.FirstOrDefault()?.TableName();
                if (tableAlias != null)
                {
                    var idColumn = tableAlias + "," + Rds.IdColumn(currentSs.ReferenceType);
                    dataRows
                        .GroupBy(o => o.Long(idColumn))
                        .Where(o => o.Key > 0)
                        .Select(o => o.ToList())
                        .ForEach(groupedDataRows =>
                            exportModel.AddSource(
                                exportModel: JsonStacks(
                                    context: context,
                                    ss: currentSs,
                                    idColumn: idColumn,
                                    dataRows: groupedDataRows,
                                    tableAlias: tableAlias)));
                }
            });
        }
    }
}
