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
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            int top = 0,
            int offset = 0,
            int pageSize = 0,
            bool countRecord = false,
            IEnumerable<Aggregation> aggregations = null,
            bool get = true)
        {
            Get(
                ss: ss,
                view: view,
                tableType: tableType,
                top: top,
                offset: offset,
                pageSize: pageSize,
                countRecord: countRecord,
                aggregations: aggregations);
        }

        private void Get(
            SiteSettings ss,
            View view,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            int top = 0,
            int offset = 0,
            int pageSize = 0,
            bool history = false,
            bool countRecord = false,
            IEnumerable<Aggregation> aggregations = null)
        {
            var columns = GridDefaultColumns(ss)
                .Distinct()
                .Select(o => ss.GetColumn(o))
                .Where(o => o != null)
                .ToList();
            columns
                .GroupBy(o => o.SiteId)
                .Select(o => o.First().SiteSettings)
                .ForEach(o =>
                {
                    o.SetPermissions(o.SiteId);
                    o.SetChoiceHash();
                });
            var where = view.Where(ss);
            var orderBy = view.OrderBy(ss);
            if (pageSize > 0 && orderBy?.Any() != true)
            {
                orderBy = new SqlOrderByCollection().Add(
                    tableName: ss.ReferenceType,
                    columnBracket: "[UpdatedTime]",
                    orderType: SqlOrderBy.Types.desc);
            }
            var statements = new List<SqlStatement>
            {
                Rds.Select(
                    tableName: ss.ReferenceType,
                    dataTableName: "Main",
                    column: Column(ss, columns),
                    join: Join(ss, columns.Where(o => o.Joined)),
                    where: where,
                    orderBy: orderBy,
                    tableType: tableType,
                    top: top,
                    offset: offset,
                    pageSize: pageSize,
                    countRecord: countRecord)
            };
            if (aggregations != null)
            {
                SetAggregations(ss, aggregations, where, statements);
            }
            var dataSet = Rds.ExecuteDataSet(
                transactional: false,
                statements: statements.ToArray());
            Aggregations.Set(dataSet, aggregations, ss);
            DataRows = dataSet.Tables["Main"].AsEnumerable();
        }

        private static List<string> GridDefaultColumns(SiteSettings ss)
        {
            var columns = ss.GridColumns.ToList();
            var hash = columns
                .Select(o => o.Contains(",")
                    ? o.Split_1st() + ","
                    : string.Empty)
                .Distinct()
                .ToDictionary(o => o, o => o == string.Empty
                    ? ss
                    : ss.JoinedSsHash.Get(o
                        .Split_1st()
                        .Split('-')
                        .Last()
                        .Split_2nd(':')
                        .ToLong()));
            hash
                .Where(o => o.Value != null)
                .ForEach(data => AddDefaultColumns(
                    dataTableName: data.Key,
                    ss: data.Value,
                    columns: columns));
            return columns;
        }

        private static void AddDefaultColumns(
            string dataTableName, SiteSettings ss, List<string> columns)
        {
            if (ss.ColumnHash.ContainsKey("SiteId")) columns.Add(dataTableName + "SiteId");
            columns.Add(dataTableName + Rds.IdColumn(ss.ReferenceType));
            columns.Add(dataTableName + "Creator");
            columns.Add(dataTableName + "Updator");
        }

        private static SqlColumnCollection Column(SiteSettings ss, IEnumerable<Column> columns)
        {
            return new SqlColumnCollection(columns
                .SelectMany(column => column.SqlColumnCollection(ss))
                .GroupBy(o => o.ColumnBracket + o.As)
                .Select(o => o.First())
                .ToArray());
        }

        private static SqlJoinCollection Join(SiteSettings ss, IEnumerable<Column> joins)
        {
            return new SqlJoinCollection(joins
                .SelectMany(o => o.SqlJoinCollection(ss))
                .Where(o => o != null)
                .OrderBy(o => o.JoinExpression.Length)
                .GroupBy(o => o.JoinExpression)
                .Select(o => o.First())
                .ToArray());
        }

        private static void SetAggregations(
            SiteSettings ss,
            IEnumerable<Aggregation> aggregations,
            SqlWhereCollection where,
            List<SqlStatement> statements)
        {
            switch (ss.ReferenceType)
            {
                case "Depts":
                    statements.AddRange(Rds.DeptsAggregations(aggregations, where));
                    break;
                case "Groups":
                    statements.AddRange(Rds.GroupsAggregations(aggregations, where));
                    break;
                case "Users":
                    statements.AddRange(Rds.UsersAggregations(aggregations, where));
                    break;
                case "Issues":
                    statements.AddRange(Rds.IssuesAggregations(aggregations, where));
                    break;
                case "Results":
                    statements.AddRange(Rds.ResultsAggregations(aggregations, where));
                    break;
                case "Wikis":
                    statements.AddRange(Rds.WikisAggregations(aggregations, where));
                    break;
            }
        }

        public void TBody(
            HtmlBuilder hb, SiteSettings ss, IEnumerable<Column> columns, bool checkAll)
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
                        var issues = new Dictionary<string, IssueModel>();
                        var results = new Dictionary<string, ResultModel>();
                        var wikis = new Dictionary<string, WikiModel>();
                        columns.ForEach(column =>
                        {
                            var tableAlias = column.Joined
                                ? column.TableAlias
                                : ss.ReferenceType;
                            switch (column.SiteSettings?.ReferenceType)
                            {
                                case "Depts":
                                    if (!depts.ContainsKey(tableAlias))
                                    {
                                        depts.Add(tableAlias, new DeptModel(
                                            column.SiteSettings, dataRow, tableAlias));
                                    }
                                    hb.TdValue(
                                        ss: column.SiteSettings,
                                        column: column,
                                        deptModel: depts.Get(tableAlias));
                                    break;
                                case "Groups":
                                    if (!groups.ContainsKey(tableAlias))
                                    {
                                        groups.Add(tableAlias, new GroupModel(
                                            column.SiteSettings, dataRow, tableAlias));
                                    }
                                    hb.TdValue(
                                        ss: column.SiteSettings,
                                        column: column,
                                        groupModel: groups.Get(tableAlias));
                                    break;
                                case "Users":
                                    if (!users.ContainsKey(tableAlias))
                                    {
                                        users.Add(tableAlias, new UserModel(
                                            column.SiteSettings, dataRow, tableAlias));
                                    }
                                    hb.TdValue(
                                        ss: column.SiteSettings,
                                        column: column,
                                        userModel: users.Get(tableAlias));
                                    break;
                                case "Issues":
                                    if (!issues.ContainsKey(tableAlias))
                                    {
                                        issues.Add(tableAlias, new IssueModel(
                                            column.SiteSettings, dataRow, tableAlias));
                                    }
                                    hb.TdValue(
                                        ss: column.SiteSettings,
                                        column: column,
                                        issueModel: issues.Get(tableAlias));
                                    break;
                                case "Results":
                                    if (!results.ContainsKey(tableAlias))
                                    {
                                        results.Add(tableAlias, new ResultModel(
                                            column.SiteSettings, dataRow, tableAlias));
                                    }
                                    hb.TdValue(
                                        ss: column.SiteSettings,
                                        column: column,
                                        resultModel: results.Get(tableAlias));
                                    break;
                                case "Wikis":
                                    if (!wikis.ContainsKey(tableAlias))
                                    {
                                        wikis.Add(tableAlias, new WikiModel(
                                            column.SiteSettings, dataRow, tableAlias));
                                    }
                                    hb.TdValue(
                                        ss: column.SiteSettings,
                                        column: column,
                                        wikiModel: wikis.Get(tableAlias));
                                    break;
                            }
                        });
                    });
            });
        }
    }
}
