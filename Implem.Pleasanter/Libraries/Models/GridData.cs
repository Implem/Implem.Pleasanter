using Implem.Libraries.DataSources.Interfaces;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
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
            Sqls.TableTypes? tableType = null,
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
                tableType: tableType ?? ((view.ShowHistory == true)
                    ? Sqls.TableTypes.NormalAndHistory
                    : ss.TableType),
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
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            int top = 0,
            int offset = 0,
            int pageSize = 0)
        {
            column = column ?? ColumnUtilities.SqlColumnCollection(
                context: context,
                ss: ss,
                columns: ss.GetGridColumns(
                    context: context,
                    view: view));
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
                    top: top,
                    offset: offset,
                    pageSize: pageSize),
                Rds.SelectCount(
                    tableName: ss.ReferenceType,
                    tableType: tableType,
                    join: join,
                    where: where)
            };
            var dataSet = Repository.ExecuteDataSet(
                context: context,
                transactional: false,
                statements: statements.ToArray());
            DataRows = dataSet.Tables["Main"].AsEnumerable();
            TotalCount = Rds.Count(dataSet);
            ss.SetChoiceHash(dataRows: DataRows);
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
                var registrations = new Dictionary<string, RegistrationModel>();
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
                    .Where(o => o.Column.CanRead)
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
