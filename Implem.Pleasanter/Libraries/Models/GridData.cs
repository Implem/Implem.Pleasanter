using Implem.Libraries.DataSources.Interfaces;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            int top = 0,
            int offset = 0,
            int pageSize = 0)
        {
            Get(
                context: context,
                ss: ss,
                view: view,
                column: column,
                join: join,
                where: where,
                top: top,
                offset: offset,
                pageSize: pageSize);
        }

        private void Get(
            Context context,
            SiteSettings ss,
            View view,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            int top = 0,
            int offset = 0,
            int pageSize = 0)
        {
            column = column ?? SqlColumnCollection(columns: GridColumns(
                context: context,
                view: view,
                ss: ss));
            where = view.Where(
                context: context,
                ss: ss,
                where: where);
            var orderBy = view.OrderBy(
                context: context,
                ss: ss);
            var statements = new List<SqlStatement>
            {
                Rds.Select(
                    tableName: ss.ReferenceType,
                    tableType: ss.TableType,
                    dataTableName: "Main",
                    column: column,
                    join: join ?? ss.Join(
                        context: context,
                        join: new IJoin[]
                        {
                            column,
                            where,
                            orderBy
                        }),
                    where: where,
                    orderBy: orderBy,
                    top: top,
                    offset: offset,
                    pageSize: pageSize,
                    countRecord: true)
            };
            var dataSet = Rds.ExecuteDataSet(
                context: context,
                transactional: false,
                statements: statements.ToArray());
            DataRows = dataSet.Tables["Main"].AsEnumerable();
            TotalCount = Rds.Count(dataSet);
            ss.SetChoiceHash(dataRows: DataRows);
        }

        private static SqlColumnCollection SqlColumnCollection(IEnumerable<Column> columns)
        {
            return new SqlColumnCollection(columns
                .SelectMany(column => column.SqlColumnCollection())
                .GroupBy(o => o.ColumnBracket + o.As)
                .Select(o => o.First())
                .ToArray());
        }

        private static List<Column> GridColumns(Context context, SiteSettings ss, View view)
        {
            var columns = ss.GetGridColumns(
                context: context,
                view: view).ToList();
            columns
                .GroupBy(o => o.TableAlias)
                .Select(o => o.First())
                .ToList()
                .ForEach(o => AddDefaultColumns(
                    context: context,
                    ss: ss,
                    currentSs: o.SiteSettings,
                    tableAlias: o.Joined
                        ? o.TableAlias
                        : string.Empty,
                    columns: columns));
            columns = columns
                .Concat(ss.IncludedColumns().Select(columnName => ss.GetColumn(
                    context: context,
                    columnName: columnName)))
                .ToList();
            return columns
                .Where(o => o != null)
                .GroupBy(o => o.ColumnName)
                .Select(o => o.First())
                .AllowedColumns(checkPermission: true)
                .ToList();
        }

        private static void AddDefaultColumns(
            Context context,
            SiteSettings ss,
            SiteSettings currentSs,
            string tableAlias,
            List<Column> columns)
        {
            var idColumn = Rds.IdColumn(currentSs.ReferenceType);
            if (currentSs.ColumnHash.ContainsKey(idColumn))
            {
                columns.Add(GetColumn(
                    context: context,
                    ss:ss,
                    tableAlias: tableAlias,
                    columnName: idColumn));
            }
            if (currentSs.ColumnHash.ContainsKey("SiteId"))
            {
                columns.Add(GetColumn(
                    context: context,
                    ss: ss,
                    tableAlias: tableAlias,
                    columnName: "SiteId"));
            }
            currentSs.TitleColumns
                .Where(o => currentSs.ColumnHash.ContainsKey(o))
                .ForEach(name =>
                    columns.Add(GetColumn(
                        context: context,
                        ss: ss,
                        tableAlias: tableAlias,
                        columnName: name)));
            currentSs.Links
                .Where(link => columns.Any(p =>
                    (!tableAlias.IsNullOrEmpty()
                        ? tableAlias + ","
                        : string.Empty) + link.ColumnName == p?.ColumnName))
                .ForEach(link =>
                    columns.Add(GetColumn(
                        context: context,
                        ss: ss,
                        tableAlias: (!tableAlias.IsNullOrEmpty()
                            ? tableAlias + "-"
                            : string.Empty)
                                + link.LinkedTableName(),
                        columnName: "Title")));
            columns.Add(GetColumn(
                context: context,
                ss: ss,
                tableAlias: tableAlias,
                columnName: Rds.IdColumn(currentSs.ReferenceType)));
            columns.Add(GetColumn(
                context: context,
                ss: ss,
                tableAlias: tableAlias,
                columnName: "Creator"));
            columns.Add(GetColumn(
                context: context,
                ss: ss,
                tableAlias: tableAlias,
                columnName: "Updator"));
        }

        private static Column GetColumn(
            Context context, SiteSettings ss, string tableAlias, string columnName)
        {
            return ss.GetColumn(
                context: context,
                columnName: ColumnName(
                    tableAlias: tableAlias,
                    columnName: columnName));
        }

        private static string ColumnName(string tableAlias, string columnName)
        {
            return (tableAlias.IsNullOrEmpty()
                ? string.Empty
                : tableAlias + ",")
                    + columnName;
        }

        public System.Text.StringBuilder Csv(
            Context context,
            SiteSettings ss,
            System.Text.StringBuilder csv,
            IEnumerable<ExportColumn> exportColumns)
        {
            var idColumn = Rds.IdColumn(ss.ReferenceType);
            DataRows.ForEach(dataRow =>
            {
                var dataId = dataRow.Long(idColumn).ToString();
                var data = new List<string>();
                var tenants = new Dictionary<string, TenantModel>();
                var depts = new Dictionary<string, DeptModel>();
                var groups = new Dictionary<string, GroupModel>();
                var users = new Dictionary<string, UserModel>();
                var sites = new Dictionary<string, SiteModel>();
                var issues = new Dictionary<string, IssueModel>();
                var results = new Dictionary<string, ResultModel>();
                var wikis = new Dictionary<string, WikiModel>();
                exportColumns.ForEach(exportColumn =>
                {
                    var column = exportColumn.Column;
                    var key = column.TableName();
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
                                mine: users.Get(key).Mine(context: context)));
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
                                mine: issues.Get(key).Mine(context: context)));
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
                                mine: results.Get(key).Mine(context: context)));
                            break;
                    }
                });
                csv.Append(data.Join(","), "\n");
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
                    .Where(o => o.Column.CanRead)
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
                var tableAlias = currentSs.JoinStacks.First().TableName();
                var idColumn = tableAlias + "," + Rds.IdColumn(currentSs.ReferenceType);
                dataRows
                    .GroupBy(o => o.Long(idColumn))
                    .Where(o => o.Key > 0)
                    .Select(o => o.ToList())
                    .ForEach(groupedDataRows =>
                    {
                        exportModel.AddSource(
                            exportModel: JsonStacks(
                                context: context,
                                ss: currentSs,
                                idColumn: idColumn,
                                dataRows: groupedDataRows,
                                tableAlias: tableAlias));
                    });
            });
        }

        public HtmlBuilder TBody(
            HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            IEnumerable<Column> columns,
            bool checkAll,
            bool checkRow = true)
        {
            var idColumn = Rds.IdColumn(ss.ReferenceType);
            DataRows.ForEach(dataRow =>
            {
                var dataId = dataRow.Long(idColumn).ToString();
                hb.Tr(
                    attributes: new HtmlAttributes()
                        .Class("grid-row")
                        .DataId(dataId),
                    action: () =>
                    {
                        if (checkRow)
                        {
                            hb.Td(action: () => hb
                                .CheckBox(
                                    controlCss: "grid-check",
                                    _checked: checkAll,
                                    dataId: dataId));
                        }
                        var tenants = new Dictionary<string, TenantModel>();
                        var depts = new Dictionary<string, DeptModel>();
                        var groups = new Dictionary<string, GroupModel>();
                        var users = new Dictionary<string, UserModel>();
                        var sites = new Dictionary<string, SiteModel>();
                        var issues = new Dictionary<string, IssueModel>();
                        var results = new Dictionary<string, ResultModel>();
                        var wikis = new Dictionary<string, WikiModel>();
                        columns.ForEach(column =>
                        {
                            var key = column.TableName();
                            switch (column.SiteSettings?.ReferenceType)
                            {
                                case "Tenants":
                                    if (!tenants.ContainsKey(key))
                                    {
                                        tenants.Add(key, new TenantModel(
                                            context: context,
                                            ss: column.SiteSettings,
                                            dataRow: dataRow,
                                            tableAlias: column.TableAlias));
                                    }
                                    hb.TdValue(
                                        context: context,
                                        ss: column.SiteSettings,
                                        column: column,
                                        tenantModel: tenants.Get(key));
                                    break;
                                case "Depts":
                                    if (!depts.ContainsKey(key))
                                    {
                                        depts.Add(key, new DeptModel(
                                            context: context,
                                            ss: column.SiteSettings,
                                            dataRow: dataRow,
                                            tableAlias: column.TableAlias));
                                    }
                                    hb.TdValue(
                                        context: context,
                                        ss: column.SiteSettings,
                                        column: column,
                                        deptModel: depts.Get(key));
                                    break;
                                case "Groups":
                                    if (!groups.ContainsKey(key))
                                    {
                                        groups.Add(key, new GroupModel(
                                            context: context,
                                            ss: column.SiteSettings,
                                            dataRow: dataRow,
                                            tableAlias: column.TableAlias));
                                    }
                                    hb.TdValue(
                                        context: context,
                                        ss: column.SiteSettings,
                                        column: column,
                                        groupModel: groups.Get(key));
                                    break;
                                case "Users":
                                    if (!users.ContainsKey(key))
                                    {
                                        users.Add(key, new UserModel(
                                            context: context,
                                            ss: column.SiteSettings,
                                            dataRow: dataRow,
                                            tableAlias: column.TableAlias));
                                    }
                                    hb.TdValue(
                                        context: context,
                                        ss: column.SiteSettings,
                                        column: column,
                                        userModel: users.Get(key));
                                    break;
                                case "Sites":
                                    if (!sites.ContainsKey(key))
                                    {
                                        sites.Add(key, new SiteModel(
                                            context: context,
                                            dataRow: dataRow,
                                            tableAlias: column.TableAlias));
                                    }
                                    hb.TdValue(
                                        context: context,
                                        ss: column.SiteSettings,
                                        column: column,
                                        siteModel: sites.Get(key));
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
                                    hb.TdValue(
                                        context: context,
                                        ss: column.SiteSettings,
                                        column: column,
                                        issueModel: issues.Get(key));
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
                                    hb.TdValue(
                                        context: context,
                                        ss: column.SiteSettings,
                                        column: column,
                                        resultModel: results.Get(key));
                                    break;
                                case "Wikis":
                                    if (!wikis.ContainsKey(key))
                                    {
                                        wikis.Add(key, new WikiModel(
                                            context: context,
                                            ss: column.SiteSettings,
                                            dataRow: dataRow,
                                            tableAlias: column.TableAlias));
                                    }
                                    hb.TdValue(
                                        context: context,
                                        ss: column.SiteSettings,
                                        column: column,
                                        wikiModel: wikis.Get(key));
                                    break;
                            }
                        });
                    });
            });
            return hb;
        }
    }
}
