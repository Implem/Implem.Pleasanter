using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Models
{
    public class GridData
    {
        public Databases.AccessStatuses AccessStatus = Databases.AccessStatuses.Initialized;
        public EnumerableRowCollection<DataRow> DataRows;
        public Aggregations Aggregations = new Aggregations();

        public GridData(
            SiteSettings ss,
            View view,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            int top = 0,
            int offset = 0,
            int pageSize = 0,
            bool countRecord = false,
            IEnumerable<Aggregation> aggregations = null)
        {
            Get(
                ss: ss,
                view: view,
                column: column,
                join: join,
                where: where,
                top: top,
                offset: offset,
                pageSize: pageSize,
                countRecord: countRecord,
                aggregations: aggregations);
        }

        private void Get(
            SiteSettings ss,
            View view,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            int top = 0,
            int offset = 0,
            int pageSize = 0,
            bool history = false,
            bool countRecord = false,
            IEnumerable<Aggregation> aggregations = null)
        {
            column = column ?? SqlColumnCollection(ss, GridColumns(ss));
            join = join ?? ss.Join(withColumn: true);
            where = view.Where(ss: ss, where: where);
            var orderBy = view.OrderBy(ss: ss, pageSize: pageSize);
            var statements = new List<SqlStatement>
            {
                Rds.Select(
                    tableName: ss.ReferenceType,
                    tableType: ss.TableType,
                    dataTableName: "Main",
                    column: column,
                    join: join,
                    where: where,
                    orderBy: orderBy,
                    top: top,
                    offset: offset,
                    pageSize: pageSize,
                    countRecord: countRecord)
            };
            if (aggregations != null)
            {
                SetAggregations(
                    ss: ss,
                    aggregations: aggregations,
                    join: join,
                    where: where,
                    statements: statements);
            }
            var dataSet = Rds.ExecuteDataSet(
                transactional: false,
                statements: statements.ToArray());
            Aggregations.Set(dataSet, aggregations, ss);
            DataRows = dataSet.Tables["Main"].AsEnumerable();
            ss.SetChoiceHash(DataRows);
        }

        private static SqlColumnCollection SqlColumnCollection(
            SiteSettings ss, IEnumerable<Column> columns)
        {
            return new SqlColumnCollection(columns
                .SelectMany(column => column.SqlColumnCollection(ss))
                .GroupBy(o => o.ColumnBracket + o.As)
                .Select(o => o.First())
                .ToArray());
        }

        private static List<Column> GridColumns(SiteSettings ss)
        {
            var columns = ss.GetGridColumns(checkPermission: true).ToList();
            columns
                .GroupBy(o => o.SiteId)
                .Select(o => o.First())
                .ToList()
                .ForEach(o => AddDefaultColumns(
                    tableAlias: o.Joined
                        ? o.TableAlias + ","
                        : string.Empty,
                    ss: ss,
                    currentSs: o.SiteSettings,
                    columns: columns));
            columns = columns
                .Concat(ss.IncludedColumns().Select(o => ss.GetColumn(o)))
                .ToList();
            return columns
                .Where(o => o != null)
                .ToList();
        }

        private static void AddDefaultColumns(
            string tableAlias, SiteSettings ss, SiteSettings currentSs, List<Column> columns)
        {
            if (currentSs.ColumnHash.ContainsKey("SiteId"))
            {
                columns.Add(ss.GetColumn(tableAlias + "SiteId"));
            }
            currentSs.TitleColumns
                .Where(o => currentSs.ColumnHash.ContainsKey(o))
                .ForEach(name =>
                    columns.Add(ss.GetColumn(tableAlias + name)));
            columns.Add(ss.GetColumn(tableAlias + Rds.IdColumn(currentSs.ReferenceType)));
            columns.Add(ss.GetColumn(tableAlias + "Creator"));
            columns.Add(ss.GetColumn(tableAlias + "Updator"));
        }

        private static void SetAggregations(
            SiteSettings ss,
            IEnumerable<Aggregation> aggregations,
            SqlJoinCollection join,
            SqlWhereCollection where,
            List<SqlStatement> statements)
        {
            switch (ss.ReferenceType)
            {
                case "Depts":
                    statements.AddRange(Rds.DeptsAggregations(aggregations, join, where));
                    break;
                case "Groups":
                    statements.AddRange(Rds.GroupsAggregations(aggregations, join, where));
                    break;
                case "Users":
                    statements.AddRange(Rds.UsersAggregations(aggregations, join, where));
                    break;
                case "Sites":
                    statements.AddRange(Rds.SitesAggregations(aggregations, join, where));
                    break;
                case "Issues":
                    statements.AddRange(Rds.IssuesAggregations(aggregations, join, where));
                    break;
                case "Results":
                    statements.AddRange(Rds.ResultsAggregations(aggregations, join, where));
                    break;
                case "Wikis":
                    statements.AddRange(Rds.WikisAggregations(aggregations, join, where));
                    break;
            }
        }

        public void TBody(
            HtmlBuilder hb,
            SiteSettings ss,
            IEnumerable<Column> columns,
            bool checkAll)
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
                        hb.Td(action: () => hb
                            .CheckBox(
                                controlCss: "grid-check",
                                _checked: checkAll,
                                dataId: dataId));
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
                                case "Depts":
                                    if (!depts.ContainsKey(key))
                                    {
                                        depts.Add(key, new DeptModel(
                                            column.SiteSettings, dataRow, column.TableAlias));
                                    }
                                    hb.TdValue(
                                        ss: column.SiteSettings,
                                        column: column,
                                        deptModel: depts.Get(key));
                                    break;
                                case "Groups":
                                    if (!groups.ContainsKey(key))
                                    {
                                        groups.Add(key, new GroupModel(
                                            column.SiteSettings, dataRow, column.TableAlias));
                                    }
                                    hb.TdValue(
                                        ss: column.SiteSettings,
                                        column: column,
                                        groupModel: groups.Get(key));
                                    break;
                                case "Users":
                                    if (!users.ContainsKey(key))
                                    {
                                        users.Add(key, new UserModel(
                                            column.SiteSettings, dataRow, column.TableAlias));
                                    }
                                    hb.TdValue(
                                        ss: column.SiteSettings,
                                        column: column,
                                        userModel: users.Get(key));
                                    break;
                                case "Sites":
                                    if (!sites.ContainsKey(key))
                                    {
                                        sites.Add(key, new SiteModel(
                                            dataRow, column.TableAlias));
                                    }
                                    hb.TdValue(
                                        ss: column.SiteSettings,
                                        column: column,
                                        siteModel: sites.Get(key));
                                    break;
                                case "Issues":
                                    if (!issues.ContainsKey(key))
                                    {
                                        issues.Add(key, new IssueModel(
                                            column.SiteSettings, dataRow, column.TableAlias));
                                    }
                                    hb.TdValue(
                                        ss: column.SiteSettings,
                                        column: column,
                                        issueModel: issues.Get(key));
                                    break;
                                case "Results":
                                    if (!results.ContainsKey(key))
                                    {
                                        results.Add(key, new ResultModel(
                                            column.SiteSettings, dataRow, column.TableAlias));
                                    }
                                    hb.TdValue(
                                        ss: column.SiteSettings,
                                        column: column,
                                        resultModel: results.Get(key));
                                    break;
                                case "Wikis":
                                    if (!wikis.ContainsKey(key))
                                    {
                                        wikis.Add(key, new WikiModel(
                                            column.SiteSettings, dataRow, column.TableAlias));
                                    }
                                    hb.TdValue(
                                        ss: column.SiteSettings,
                                        column: column,
                                        wikiModel: wikis.Get(key));
                                    break;
                            }
                        });
                    });
            });
        }
    }
}
