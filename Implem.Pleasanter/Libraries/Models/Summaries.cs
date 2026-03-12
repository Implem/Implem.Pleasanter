using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Models
{
    public static class Summaries
    {
        private const string MultipleLinkAlias = "Link";

        public static void Synchronize(Context context, SiteSettings ss)
        {
            ss.Summaries?.ForEach(summary => Synchronize(
                context: context, ss: ss, summary: summary));
        }

        public static void Synchronize(Context context, SiteSettings ss, int id)
        {
            var summary = ss.Summaries?.Get(id);
            Synchronize(context: context, ss: ss, summary: summary);
        }

        public static void Synchronize(Context context, SiteSettings ss, Summary summary)
        {
            if (ss.Links?.Any(link => link.SiteId == summary.SiteId
                && link.ColumnName == summary.LinkColumn) is null or false)
            {
                return;
            }
            var destinationSs = SiteSettingsUtilities.Get(
                context: context, siteId: summary?.SiteId ?? 0);
            if (destinationSs != null && summary != null)
            {
                Synchronize(
                    context: context,
                    ss: ss,
                    destinationSs: destinationSs,
                    destinationSiteId: summary.SiteId,
                    destinationColumn: summary.DestinationColumn,
                    destinationCondition: destinationSs.Views?.Get(summary.DestinationCondition),
                    setZeroWhenOutOfCondition: summary.SetZeroWhenOutOfCondition == true,
                    sourceSiteId: ss.SiteId,
                    sourceReferenceType: ss.ReferenceType,
                    linkColumn: summary.LinkColumn,
                    type: summary.Type,
                    sourceColumn: summary.SourceColumn,
                    sourceCondition: ss.Views?.Get(summary.SourceCondition));
            }
        }

        public static string Synchronize(
            Context context,
            SiteSettings ss,
            SiteSettings destinationSs,
            long destinationSiteId,
            string destinationColumn,
            View destinationCondition,
            bool setZeroWhenOutOfCondition,
            long sourceSiteId,
            string sourceReferenceType,
            string linkColumn,
            string type,
            string sourceColumn,
            View sourceCondition,
            long id = 0)
        {
            switch (destinationSs.ReferenceType)
            {
                case "Issues":
                    SynchronizeIssues(
                        context: context,
                        ss: ss,
                        destinationSs: destinationSs,
                        destinationSiteId: destinationSiteId,
                        destinationColumn: destinationColumn,
                        destinationCondition: destinationCondition,
                        setZeroWhenOutOfCondition: setZeroWhenOutOfCondition,
                        sourceSiteId: sourceSiteId,
                        sourceReferenceType: sourceReferenceType,
                        linkColumn: linkColumn,
                        type: type,
                        sourceColumn: sourceColumn,
                        sourceCondition: sourceCondition,
                        issueId: id);
                    break;
                case "Results":
                    SynchronizeResults(
                        context: context,
                        ss: ss,
                        destinationSs: destinationSs,
                        destinationSiteId: destinationSiteId,
                        destinationColumn: destinationColumn,
                        destinationCondition: destinationCondition,
                        setZeroWhenOutOfCondition: setZeroWhenOutOfCondition,
                        sourceSiteId: sourceSiteId,
                        sourceReferenceType: sourceReferenceType,
                        linkColumn: linkColumn,
                        type: type,
                        sourceColumn: sourceColumn,
                        sourceCondition: sourceCondition,
                        resultId: id);
                    break;
            }
            return Messages.ResponseSynchronizationCompleted(context: context).ToJson();
        }

        private static void SynchronizeIssues(
            Context context,
            SiteSettings ss,
            SiteSettings destinationSs,
            long destinationSiteId,
            string destinationColumn,
            View destinationCondition,
            bool setZeroWhenOutOfCondition,
            long sourceSiteId,
            string sourceReferenceType,
            string linkColumn,
            string type,
            string sourceColumn,
            View sourceCondition,
            long issueId = 0)
        {
            if (context.CanUpdate(ss: destinationSs))
            {
                var where = Rds.IssuesWhere()
                    .SiteId(destinationSiteId)
                    .IssueId(issueId, _using: issueId != 0);
                var issueIds = new IssueCollection(
                    context: context,
                    ss: destinationSs,
                    column: Rds.IssuesColumn().IssueId(),
                    where: Where(
                        context: context,
                        ss: destinationSs,
                        view: null,
                        where: where))
                            .Select(o => o.IssueId)
                            .ToList();
                var matchingConditions = destinationCondition != null
                    ? Repository.ExecuteTable(
                        context: context,
                        statements: Rds.SelectIssues(
                            column: Rds.IssuesColumn().IssueId(),
                            where: Where(
                                context: context,
                                ss: destinationSs,
                                view: destinationCondition,
                                where: where)))
                                    .AsEnumerable()
                                    .Select(dataRow => dataRow.Long("IssueId"))
                                    .ToList()
                    : issueIds;
                var linkColumnMultipleSelections =
                    LinkColumnMultipleSelections(context: context, ss: ss, linkColumn: linkColumn);
                var data = issueIds.Any()
                    ? Data(
                        context: context,
                        ss: ss,
                        destinationColumn: destinationColumn,
                        destinations: issueIds,
                        sourceSiteId: sourceSiteId,
                        sourceReferenceType: sourceReferenceType,
                        linkColumn: linkColumn,
                        linkColumnMultipleSelections: linkColumnMultipleSelections,
                        type: type,
                        sourceColumn: sourceColumn,
                        sourceCondition: sourceCondition)
                    : new Dictionary<long, decimal>();
                issueIds.ForEach(issueId =>
                {
                    var issueModel = new IssueModel(
                        context: context,
                        ss: destinationSs,
                        issueId: issueId,
                        column: Rds.IssuesDefaultColumns());
                    if (matchingConditions.Any(o => o == issueModel.IssueId))
                    {
                        Set(
                            issueModel: issueModel,
                            destinationColumn: destinationColumn,
                            value: data.Get(issueModel.IssueId));
                    }
                    else if (setZeroWhenOutOfCondition)
                    {
                        Set(issueModel, destinationColumn, 0);
                    }
                    if (issueModel.Updated(context: context))
                    {
                        issueModel.SetByFormula(
                            context: context,
                            ss: destinationSs);
                        issueModel.SetChoiceHash(
                            context: context,
                            ss: destinationSs);
                        issueModel.Update(
                            context: context,
                            ss: destinationSs,
                            synchronizeSummary: false,
                            get: false);
                    }
                });
            }
        }

        private static EnumerableRowCollection<DataRow> IssuesDataRows(
            Context context,
            SiteSettings ss,
            string destinationColumn,
            IEnumerable<long> destinations,
            long sourceSiteId,
            string sourceReferenceType,
            string linkColumn,
            string type,
            string sourceColumn,
            View sourceCondition)
        {
            switch (destinationColumn)
            {
                case "WorkValue":
                    return Repository.ExecuteTable(
                        context: context,
                        statements: Select(
                            context: context,
                            ss: ss,
                            destinations: destinations,
                            sourceSiteId: sourceSiteId,
                            sourceReferenceType: sourceReferenceType,
                            linkColumn: linkColumn,
                            type: type,
                            sourceColumn: sourceColumn,
                            sourceCondition: sourceCondition)).AsEnumerable();
                default:
                    switch (Def.ExtendedColumnTypes.Get(destinationColumn ?? string.Empty))
                    {
                        case "Num":
                            return Repository.ExecuteTable(
                                context: context,
                                statements: Select(
                                    context: context,
                                    ss: ss,
                                    destinations: destinations,
                                    sourceSiteId: sourceSiteId,
                                    sourceReferenceType: sourceReferenceType,
                                    linkColumn: linkColumn,
                                    type: type,
                                    sourceColumn: sourceColumn,
                                    sourceCondition: sourceCondition)).AsEnumerable();
                        default:
                            return null;
                    }
            }
        }

        private static void Set(
            IssueModel issueModel, string destinationColumn, decimal value)
        {
            switch (destinationColumn)
            {
                case "WorkValue": issueModel.WorkValue.Value = value; break;
                default:
                    switch (Def.ExtendedColumnTypes.Get(destinationColumn ?? string.Empty))
                    {
                        case "Num":
                            issueModel.SetNum(
                                columnName: destinationColumn,
                                value: new Num(value));
                            break;
                    }
                    break;
            }
        }

        private static void SynchronizeResults(
            Context context,
            SiteSettings ss,
            SiteSettings destinationSs,
            long destinationSiteId,
            string destinationColumn,
            View destinationCondition,
            bool setZeroWhenOutOfCondition,
            long sourceSiteId,
            string sourceReferenceType,
            string linkColumn,
            string type,
            string sourceColumn,
            View sourceCondition,
            long resultId = 0)
        {
            if (context.CanUpdate(ss: destinationSs))
            {
                var where = Rds.ResultsWhere()
                    .SiteId(destinationSiteId)
                    .ResultId(resultId, _using: resultId != 0);
                var resultIds = new ResultCollection(
                    context: context,
                    ss: destinationSs,
                    column: Rds.ResultsColumn().ResultId(),
                    where: Where(
                        context: context,
                        ss: destinationSs,
                        view: null,
                        where: where))
                            .Select(o => o.ResultId)
                            .ToList();
                var matchingConditions = destinationCondition != null
                    ? Repository.ExecuteTable(
                        context: context,
                        statements: Rds.SelectResults(
                            column: Rds.ResultsColumn().ResultId(),
                            where: Where(
                                context: context,
                                ss: destinationSs,
                                view: destinationCondition,
                                where: where)))
                                    .AsEnumerable()
                                    .Select(dataRow => dataRow.Long("ResultId"))
                                    .ToList()
                    : resultIds;
                var linkColumnMultipleSelections =
                    LinkColumnMultipleSelections(context: context, ss: ss, linkColumn: linkColumn);
                var data = resultIds.Any()
                    ? Data(
                        context: context,
                        ss: ss,
                        destinationColumn: destinationColumn,
                        destinations: resultIds,
                        sourceSiteId: sourceSiteId,
                        sourceReferenceType: sourceReferenceType,
                        linkColumn: linkColumn,
                        linkColumnMultipleSelections: linkColumnMultipleSelections,
                        type: type,
                        sourceColumn: sourceColumn,
                        sourceCondition: sourceCondition)
                    : new Dictionary<long, decimal>();
                resultIds.ForEach(resultId =>
                {
                    var resultModel = new ResultModel(
                        context: context,
                        ss: destinationSs,
                        resultId: resultId,
                        column: Rds.ResultsDefaultColumns());
                    if (matchingConditions.Any(o => o == resultModel.ResultId))
                    {
                        Set(
                            resultModel: resultModel,
                            destinationColumn: destinationColumn,
                            value: data.Get(resultModel.ResultId));
                    }
                    else if (setZeroWhenOutOfCondition)
                    {
                        Set(resultModel, destinationColumn, 0);
                    }
                    if (resultModel.Updated(context: context))
                    {
                        resultModel.SetByFormula(
                            context: context,
                            ss: destinationSs);
                        resultModel.SetChoiceHash(
                            context: context,
                            ss: destinationSs);
                        resultModel.Update(
                            context: context,
                            ss: destinationSs,
                            synchronizeSummary: false,
                            get: false);
                    }
                });
            }
        }

        private static EnumerableRowCollection<DataRow> ResultsDataRows(
            Context context,
            SiteSettings ss,
            string destinationColumn,
            IEnumerable<long> destinations,
            long sourceSiteId,
            string sourceReferenceType,
            string linkColumn,
            string type,
            string sourceColumn,
            View sourceCondition)
        {
            switch (destinationColumn)
            {
                default:
                    switch (Def.ExtendedColumnTypes.Get(destinationColumn ?? string.Empty))
                    {
                        case "Num":
                            return Repository.ExecuteTable(
                                context: context,
                                statements: Select(
                                    context: context,
                                    ss: ss,
                                    destinations: destinations,
                                    sourceSiteId: sourceSiteId,
                                    sourceReferenceType: sourceReferenceType,
                                    linkColumn: linkColumn,
                                    type: type,
                                    sourceColumn: sourceColumn,
                                    sourceCondition: sourceCondition)).AsEnumerable();
                        default:
                            return null;
                    }
            }
        }

        private static void Set(
            ResultModel resultModel, string destinationColumn, decimal value)
        {
            switch (destinationColumn)
            {
                default:
                    switch (Def.ExtendedColumnTypes.Get(destinationColumn ?? string.Empty))
                    {
                        case "Num":
                            resultModel.SetNum(
                                columnName: destinationColumn,
                                value: new Num(value));
                            break;
                    }
                    break;
            }
        }

        private static Dictionary<long, decimal> Data(
            Context context,
            SiteSettings ss,
            string destinationColumn,
            IEnumerable<long> destinations,
            long sourceSiteId,
            string sourceReferenceType,
            string linkColumn,
            bool linkColumnMultipleSelections,
            string type,
            string sourceColumn,
            View sourceCondition)
        {
            if (linkColumnMultipleSelections)
            {
                return DataMultiple(
                    context: context,
                    ss: ss,
                    destinations: destinations,
                    sourceSiteId: sourceSiteId,
                    sourceReferenceType: sourceReferenceType,
                    linkColumn: linkColumn,
                    type: type,
                    sourceColumn: sourceColumn,
                    sourceCondition: sourceCondition);
            }
            switch (sourceReferenceType)
            {
                case "Issues":
                    return IssuesDataRows(
                        context: context,
                        ss: ss,
                        destinationColumn: destinationColumn,
                        destinations: destinations,
                        sourceSiteId: sourceSiteId,
                        sourceReferenceType: sourceReferenceType,
                        linkColumn: linkColumn,
                        type: type,
                        sourceColumn: sourceColumn,
                        sourceCondition: sourceCondition)
                            .ToDictionary(
                                o => o["Id"].ToLong(),
                                o => o["Value"].ToDecimal());
                case "Results":
                    return ResultsDataRows(
                        context: context,
                        ss: ss,
                        destinationColumn: destinationColumn,
                        destinations: destinations,
                        sourceSiteId: sourceSiteId,
                        sourceReferenceType: sourceReferenceType,
                        linkColumn: linkColumn,
                        type: type,
                        sourceColumn: sourceColumn,
                        sourceCondition: sourceCondition)
                            .ToDictionary(
                                o => o["Id"].ToLong(),
                                o => o["Value"].ToDecimal());
                default: return null;
            }
        }

        private static Dictionary<long, decimal> DataMultiple(
            Context context,
            SiteSettings ss,
            IEnumerable<long> destinations,
            long sourceSiteId,
            string sourceReferenceType,
            string linkColumn,
            string type,
            string sourceColumn,
            View sourceCondition)
        {
            if (linkColumn.IsNullOrEmpty()
                || !Def.ExtendedColumnTypes.ContainsKey(linkColumn))
            {
                return new Dictionary<long, decimal>();
            }
            if (type != "Count" && sourceColumn.IsNullOrEmpty())
            {
                return new Dictionary<long, decimal>();
            }
            var destinationSet = destinations != null
                ? new HashSet<long>(destinations.Where(id => id > 0))
                : new HashSet<long>();
            if (destinationSet.Count == 0)
            {
                return new Dictionary<long, decimal>();
            }
            var dataRows = AggregateRowsForMultiple(
                context: context,
                ss: ss,
                destinations: destinationSet,
                sourceSiteId: sourceSiteId,
                sourceReferenceType: sourceReferenceType,
                linkColumn: linkColumn,
                type: type,
                sourceColumn: sourceColumn,
                sourceCondition: sourceCondition);
            if (dataRows == null)
            {
                return new Dictionary<long, decimal>();
            }
            return dataRows.ToDictionary(
                o => o["Id"].ToLong(),
                o => o["Value"].ToDecimal());
        }

        private static EnumerableRowCollection<DataRow> AggregateRowsForMultiple(
            Context context,
            SiteSettings ss,
            IEnumerable<long> destinations,
            long sourceSiteId,
            string sourceReferenceType,
            string linkColumn,
            string type,
            string sourceColumn,
            View sourceCondition)
        {
            switch (sourceReferenceType)
            {
                case "Issues":
                    return IssuesAggregateRowsMultiple(
                        context: context,
                        ss: ss,
                        destinations: destinations,
                        sourceSiteId: sourceSiteId,
                        linkColumn: linkColumn,
                        type: type,
                        sourceColumn: sourceColumn,
                        sourceCondition: sourceCondition);
                case "Results":
                    return ResultsAggregateRowsMultiple(
                        context: context,
                        ss: ss,
                        destinations: destinations,
                        sourceSiteId: sourceSiteId,
                        linkColumn: linkColumn,
                        type: type,
                        sourceColumn: sourceColumn,
                        sourceCondition: sourceCondition);
                default:
                    return null;
            }
        }

        private static EnumerableRowCollection<DataRow> IssuesAggregateRowsMultiple(
            Context context,
            SiteSettings ss,
            IEnumerable<long> destinations,
            long sourceSiteId,
            string linkColumn,
            string type,
            string sourceColumn,
            View sourceCondition)
        {
            var column = IssuesAggregateColumns(type: type, sourceColumn: sourceColumn);
            if (column == null)
            {
                return null;
            }
            var join = MultipleLinkJoin(tableName: "Issues", linkColumn: linkColumn);
            var where = SourceWhereForMultiple(
                context: context,
                ss: ss,
                tableName: "Issues",
                linkColumn: linkColumn,
                sourceCondition: sourceCondition,
                where: Rds.IssuesWhere().SiteId(value: sourceSiteId));
            AddMultipleLinkDestinationWhere(where: where, destinations: destinations);
            var groupBy = new SqlGroupByCollection()
                .Add(columnBracket: "\"Id\"", tableName: MultipleLinkAlias);
            return Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectIssues(
                    column: column,
                    join: join,
                    groupBy: groupBy,
                    where: where)).AsEnumerable();
        }

        private static EnumerableRowCollection<DataRow> ResultsAggregateRowsMultiple(
            Context context,
            SiteSettings ss,
            IEnumerable<long> destinations,
            long sourceSiteId,
            string linkColumn,
            string type,
            string sourceColumn,
            View sourceCondition)
        {
            var column = ResultsAggregateColumns(type: type, sourceColumn: sourceColumn);
            if (column == null)
            {
                return null;
            }
            var join = MultipleLinkJoin(tableName: "Results", linkColumn: linkColumn);
            var where = SourceWhereForMultiple(
                context: context,
                ss: ss,
                tableName: "Results",
                linkColumn: linkColumn,
                sourceCondition: sourceCondition,
                where: Rds.ResultsWhere().SiteId(value: sourceSiteId));
            AddMultipleLinkDestinationWhere(where: where, destinations: destinations);
            var groupBy = new SqlGroupByCollection()
                .Add(columnBracket: "\"Id\"", tableName: MultipleLinkAlias);
            return Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectResults(
                    column: column,
                    join: join,
                    groupBy: groupBy,
                    where: where)).AsEnumerable();
        }

        private static SqlColumnCollection IssuesAggregateColumns(
            string type,
            string sourceColumn)
        {
            var columns = new SqlColumnCollection()
                .Add(columnBracket: "\"Id\"", tableName: MultipleLinkAlias, columnName: "Id", _as: "Id");
            switch (type)
            {
                case "Count":
                    columns.Add(
                        columnBracket: "\"Id\"",
                        tableName: MultipleLinkAlias,
                        columnName: "Value",
                        _as: "Value",
                        function: Sqls.Functions.Count);
                    return columns;
                case "Total":
                    return IssuesAggregateValueColumn(
                        columns: columns,
                        sourceColumn: sourceColumn,
                        function: Sqls.Functions.Sum);
                case "Average":
                    return IssuesAggregateValueColumn(
                        columns: columns,
                        sourceColumn: sourceColumn,
                        function: Sqls.Functions.Avg);
                case "Min":
                    return IssuesAggregateValueColumn(
                        columns: columns,
                        sourceColumn: sourceColumn,
                        function: Sqls.Functions.Min);
                case "Max":
                    return IssuesAggregateValueColumn(
                        columns: columns,
                        sourceColumn: sourceColumn,
                        function: Sqls.Functions.Max);
                default:
                    return null;
            }
        }

        private static SqlColumnCollection IssuesAggregateValueColumn(
            SqlColumnCollection columns,
            string sourceColumn,
            Sqls.Functions function)
        {
            switch (sourceColumn)
            {
                case "WorkValue":
                    columns.Add(
                        columnBracket: "\"WorkValue\"",
                        tableName: "Issues",
                        columnName: "Value",
                        _as: "Value",
                        function: function);
                    return columns;
                case "RemainingWorkValue":
                    columns.Add(
                        columnBracket: "\"RemainingWorkValue\"",
                        tableName: "Issues",
                        columnName: "Value",
                        _as: "Value",
                        function: function);
                    return columns;
                default:
                    switch (Def.ExtendedColumnTypes.Get(sourceColumn ?? string.Empty))
                    {
                        case "Num":
                            columns.Add(
                                columnBracket: $"\"{sourceColumn}\"",
                                tableName: "Issues",
                                columnName: "Value",
                                _as: "Value",
                                function: function);
                            return columns;
                        default:
                            return null;
                    }
            }
        }

        private static SqlColumnCollection ResultsAggregateColumns(
            string type,
            string sourceColumn)
        {
            var columns = new SqlColumnCollection()
                .Add(columnBracket: "\"Id\"", tableName: MultipleLinkAlias, columnName: "Id", _as: "Id");
            switch (type)
            {
                case "Count":
                    columns.Add(
                        columnBracket: "\"Id\"",
                        tableName: MultipleLinkAlias,
                        columnName: "Value",
                        _as: "Value",
                        function: Sqls.Functions.Count);
                    return columns;
                case "Total":
                    return ResultsAggregateValueColumn(
                        columns: columns,
                        sourceColumn: sourceColumn,
                        function: Sqls.Functions.Sum);
                case "Average":
                    return ResultsAggregateValueColumn(
                        columns: columns,
                        sourceColumn: sourceColumn,
                        function: Sqls.Functions.Avg);
                case "Min":
                    return ResultsAggregateValueColumn(
                        columns: columns,
                        sourceColumn: sourceColumn,
                        function: Sqls.Functions.Min);
                case "Max":
                    return ResultsAggregateValueColumn(
                        columns: columns,
                        sourceColumn: sourceColumn,
                        function: Sqls.Functions.Max);
                default:
                    return null;
            }
        }

        private static SqlColumnCollection ResultsAggregateValueColumn(
            SqlColumnCollection columns,
            string sourceColumn,
            Sqls.Functions function)
        {
            switch (Def.ExtendedColumnTypes.Get(sourceColumn ?? string.Empty))
            {
                case "Num":
                    columns.Add(
                        columnBracket: $"\"{sourceColumn}\"",
                        tableName: "Results",
                        columnName: "Value",
                        _as: "Value",
                        function: function);
                    return columns;
                default:
                    return null;
            }
        }

        private static SqlJoinCollection MultipleLinkJoin(string tableName, string linkColumn)
        {
            var joinTable = MultipleLinkJoinTable(tableName: tableName, linkColumn: linkColumn);
            switch (Parameters.Rds.Dbms)
            {
                case "SQLServer":
                    return new SqlJoinCollection().Add(
                        tableName: $"cross apply {joinTable}",
                        joinExpression: null);
                case "PostgreSQL":
                case "MySQL":
                    return new SqlJoinCollection().Add(
                        tableName: joinTable,
                        joinType: SqlJoin.JoinTypes.Inner,
                        joinExpression: "1=1");
                default:
                    throw new NotSupportedException($"Unsupported DBMS: {Parameters.Rds.Dbms}");
            }
        }

        private static string MultipleLinkJoinTable(string tableName, string linkColumn)
        {
            var linkExpression = MultipleLinkJsonExpression(
                tableName: tableName,
                linkColumn: linkColumn);
            switch (Parameters.Rds.Dbms)
            {
                case "PostgreSQL":
                    return $"lateral jsonb_array_elements_text({linkExpression}) as \"{MultipleLinkAlias}\"(\"Id\")";
                case "SQLServer":
                    return $"openjson({linkExpression}) with (\"Id\" bigint '$') as \"{MultipleLinkAlias}\"";
                case "MySQL":
                    return $"JSON_TABLE({linkExpression}, '$[*]' columns (\"Id\" bigint path '$')) as \"{MultipleLinkAlias}\"";
                default:
                    throw new NotSupportedException($"Unsupported DBMS: {Parameters.Rds.Dbms}");
            }
        }

        private static string MultipleLinkJsonExpression(string tableName, string linkColumn)
        {
            var column = $"\"{tableName}\".\"{linkColumn}\"";
            switch (Parameters.Rds.Dbms)
            {
                case "PostgreSQL":
                    return $"case when {column} is null or {column} = '' then '[]'::jsonb " +
                        $"else (case when jsonb_typeof({column}::jsonb) = 'array' " +
                        $"then {column}::jsonb else jsonb_build_array({column}::jsonb) end) end";
                case "SQLServer":
                    return $"case when {column} is null or {column} = '' then '[]' " +
                        $"when isjson({column})=1 and json_query({column},'$') is not null then {column} " +
                        $"else concat('[',{column},']') end";
                case "MySQL":
                    return $"case when {column} is null or {column} = '' then json_array() " +
                        $"when json_type({column})='ARRAY' then {column} else json_array({column}) end";
                default:
                    throw new NotSupportedException($"Unsupported DBMS: {Parameters.Rds.Dbms}");
            }
        }

        private static void AddMultipleLinkDestinationWhere(
            SqlWhereCollection where,
            IEnumerable<long> destinations)
        {
            var destinationList = destinations?.Where(id => id > 0).Distinct().ToList();
            if (destinationList?.Any() != true)
            {
                return;
            }
            where.Add(
                tableName: MultipleLinkAlias,
                columnBrackets: new[] { "\"Id\"" },
                raw: "({0})".Params(destinationList
                    .Select(o => "'" + o + "'")
                    .Join(",")),
                _operator: " in ");
        }

        private static SqlWhereCollection SourceWhereForMultiple(
            Context context,
            SiteSettings ss,
            string tableName,
            string linkColumn,
            View sourceCondition,
            SqlWhereCollection where)
        {
            if (!linkColumn.IsNullOrEmpty()
                && Def.ExtendedColumnTypes.ContainsKey(linkColumn))
            {
                where.Add(
                    tableName: tableName,
                    columnBrackets: new[] { $"\"{linkColumn}\"" },
                    _operator: " is not null");
                where.Add(
                    tableName: tableName,
                    columnBrackets: new[] { $"\"{linkColumn}\"" },
                    _operator: "!=''");
                where.Add(
                    tableName: tableName,
                    columnBrackets: new[] { $"\"{linkColumn}\"" },
                    _operator: "!='[]'");
            }
            return Where(
                context: context,
                ss: ss,
                view: sourceCondition,
                where: where);
        }

        private static bool LinkColumnMultipleSelections(
            Context context,
            SiteSettings ss,
            string linkColumn)
        {
            return ss?.GetColumn(
                context: context,
                columnName: linkColumn)?.MultipleSelections == true;
        }

        private static SqlSelect Select(
            Context context,
            SiteSettings ss,
            IEnumerable<long> destinations,
            long sourceSiteId,
            string sourceReferenceType,
            string linkColumn,
            string type,
            string sourceColumn,
            View sourceCondition)
        {
            switch (type)
            {
                case "Count": return SelectCount(
                    context: context,
                    ss: ss,
                    destinations: destinations,
                    sourceSiteId: sourceSiteId,
                    sourceReferenceType: sourceReferenceType,
                    linkColumn: linkColumn,
                    sourceCondition: sourceCondition);
                case "Total": return SelectTotal(
                    context: context,
                    ss: ss,
                    destinations: destinations,
                    sourceSiteId: sourceSiteId,
                    sourceReferenceType: sourceReferenceType,
                    linkColumn: linkColumn,
                    sourceColumn: sourceColumn,
                    sourceCondition: sourceCondition);
                case "Average": return SelectAverage(
                    context: context,
                    ss: ss,
                    destinations: destinations,
                    sourceSiteId: sourceSiteId,
                    sourceReferenceType: sourceReferenceType,
                    linkColumn: linkColumn,
                    sourceColumn: sourceColumn,
                    sourceCondition: sourceCondition);
                case "Min": return SelectMin(
                    context: context,
                    ss: ss,
                    destinations: destinations,
                    sourceSiteId: sourceSiteId,
                    sourceReferenceType: sourceReferenceType,
                    linkColumn: linkColumn,
                    sourceColumn: sourceColumn,
                    sourceCondition: sourceCondition);
                case "Max": return SelectMax(
                    context: context,
                    ss: ss,
                    destinations: destinations,
                    sourceSiteId: sourceSiteId,
                    sourceReferenceType: sourceReferenceType,
                    linkColumn: linkColumn,
                    sourceColumn: sourceColumn,
                    sourceCondition: sourceCondition);
                default: return null;
            }
        }

        private static SqlSelect SelectCount(
            Context context,
            SiteSettings ss,
            IEnumerable<long> destinations,
            long sourceSiteId,
            string sourceReferenceType,
            string linkColumn,
            View sourceCondition)
        {
            switch (sourceReferenceType)
            {
                case "Issues": return Rds.SelectIssues(
                    column: Rds.IssuesColumn()
                        .IssuesColumn(linkColumn, _as: "Id")
                        .IssuesCount(_as: "Value"),
                    where: Where(
                        context: context,
                        ss: ss,
                        view: sourceCondition,
                        where: IssuesWhere(destinations, sourceSiteId, linkColumn)),
                    groupBy: Rds.IssuesGroupBy().IssuesGroupBy(linkColumn));
                case "Results": return Rds.SelectResults(
                    column: Rds.ResultsColumn()
                        .ResultsColumn(linkColumn, _as: "Id")
                        .ResultsCount(_as: "Value"),
                    where: Where(
                        context: context,
                        ss: ss,
                        view: sourceCondition,
                        where: ResultsWhere(destinations, sourceSiteId, linkColumn)),
                    groupBy: Rds.ResultsGroupBy().ResultsGroupBy(linkColumn));
                default: return null;
            }
        }

        private static SqlSelect SelectTotal(
            Context context,
            SiteSettings ss,
            IEnumerable<long> destinations,
            long sourceSiteId,
            string sourceReferenceType,
            string linkColumn,
            string sourceColumn,
            View sourceCondition)
        {
            switch (sourceReferenceType)
            {
                case "Issues": return Rds.SelectIssues(
                    column: IssuesTotalColumn(linkColumn, sourceColumn),
                    where: Where(
                        context: context,
                        ss: ss,
                        view: sourceCondition,
                        where: IssuesWhere(destinations, sourceSiteId, linkColumn)),
                    groupBy: Rds.IssuesGroupBy().IssuesGroupBy(linkColumn));
                case "Results": return Rds.SelectResults(
                    column: ResultsTotalColumn(linkColumn, sourceColumn),
                    where: Where(
                        context: context,
                        ss: ss,
                        view: sourceCondition,
                        where: ResultsWhere(destinations, sourceSiteId, linkColumn)),
                    groupBy: Rds.ResultsGroupBy().ResultsGroupBy(linkColumn));
                default: return null;
            }
        }

        private static SqlColumnCollection IssuesTotalColumn(
            string linkColumn, string sourceColumn)
        {
            var issuesColumn = Rds.IssuesColumn().IssuesColumn(linkColumn, _as: "Id");
            switch (sourceColumn)
            {
                case "WorkValue":
                    return issuesColumn.WorkValue(
                        _as: "Value",
                        function: Sqls.Functions.Sum);
                case "RemainingWorkValue":
                    return issuesColumn.RemainingWorkValue(
                        _as: "Value",
                        function: Sqls.Functions.Sum);
                default:
                    switch (Def.ExtendedColumnTypes.Get(sourceColumn ?? string.Empty))
                    {
                        case "Num":
                            return issuesColumn.Add(
                                columnBracket: $"\"{sourceColumn}\"",
                                columnName: sourceColumn,
                                _as: "Value",
                                function: Sqls.Functions.Sum);
                        default:
                            return null;
                    }
            }
        }

        private static SqlColumnCollection ResultsTotalColumn(
            string linkColumn, string sourceColumn)
        {
            var resultsColumn = Rds.ResultsColumn().ResultsColumn(linkColumn, _as: "Id");
            switch (sourceColumn)
            {
                default:
                    switch (Def.ExtendedColumnTypes.Get(sourceColumn ?? string.Empty))
                    {
                        case "Num":
                            return resultsColumn.Add(
                                columnBracket: $"\"{sourceColumn}\"",
                                columnName: sourceColumn,
                                _as: "Value",
                                function: Sqls.Functions.Sum);
                        default:
                            return null;
                    }
            }
        }

        private static SqlSelect SelectAverage(
            Context context,
            SiteSettings ss,
            IEnumerable<long> destinations,
            long sourceSiteId,
            string sourceReferenceType,
            string linkColumn,
            string sourceColumn,
            View sourceCondition)
        {
            switch (sourceReferenceType)
            {
                case "Issues": return Rds.SelectIssues(
                    column: IssuesAverageColumn(linkColumn, sourceColumn),
                    where: Where(
                        context: context,
                        ss: ss,
                        view: sourceCondition,
                        where: IssuesWhere(destinations, sourceSiteId, linkColumn)),
                    groupBy: Rds.IssuesGroupBy().IssuesGroupBy(linkColumn));
                case "Results": return Rds.SelectResults(
                    column: ResultsAverageColumn(linkColumn, sourceColumn),
                    where: Where(
                        context: context,
                        ss: ss,
                        view: sourceCondition,
                        where: ResultsWhere(destinations, sourceSiteId, linkColumn)),
                    groupBy: Rds.ResultsGroupBy().ResultsGroupBy(linkColumn));
                default: return null;
            }
        }

        private static SqlColumnCollection IssuesAverageColumn(
            string linkColumn, string sourceColumn)
        {
            var issuesColumn = Rds.IssuesColumn().IssuesColumn(linkColumn, _as: "Id");
            switch (sourceColumn)
            {
                case "WorkValue":
                    return issuesColumn.WorkValue(
                        _as: "Value", function: Sqls.Functions.Avg);
                case "RemainingWorkValue":
                    return issuesColumn.RemainingWorkValue(
                        _as: "Value", function: Sqls.Functions.Avg);
                default:
                    switch (Def.ExtendedColumnTypes.Get(sourceColumn ?? string.Empty))
                    {
                        case "Num":
                            return issuesColumn.Add(
                                columnBracket: $"\"{sourceColumn}\"",
                                columnName: sourceColumn,
                                _as: "Value",
                                function: Sqls.Functions.Avg);
                        default:
                            return null;
                    }
            }
        }

        private static SqlColumnCollection ResultsAverageColumn(
            string linkColumn, string sourceColumn)
        {
            var resultsColumn = Rds.ResultsColumn().ResultsColumn(linkColumn, _as: "Id");
            switch (sourceColumn)
            {
                default:
                    switch (Def.ExtendedColumnTypes.Get(sourceColumn ?? string.Empty))
                    {
                        case "Num":
                            return resultsColumn.Add(
                                columnBracket: $"\"{sourceColumn}\"",
                                columnName: sourceColumn,
                                _as: "Value",
                                function: Sqls.Functions.Avg);
                        default:
                            return null;
                    }
            }
        }

        private static SqlSelect SelectMin(
            Context context,
            SiteSettings ss,
            IEnumerable<long> destinations,
            long sourceSiteId,
            string sourceReferenceType,
            string linkColumn,
            string sourceColumn,
            View sourceCondition)
        {
            switch (sourceReferenceType)
            {
                case "Issues": return Rds.SelectIssues(
                    column: IssuesMinColumn(linkColumn, sourceColumn),
                    where: Where(
                        context: context,
                        ss: ss,
                        view: sourceCondition,
                        where: IssuesWhere(destinations, sourceSiteId, linkColumn)),
                    groupBy: Rds.IssuesGroupBy().IssuesGroupBy(linkColumn));
                case "Results": return Rds.SelectResults(
                    column: ResultsMinColumn(linkColumn, sourceColumn),
                    where: Where(
                        context: context,
                        ss: ss,
                        view: sourceCondition,
                        where: ResultsWhere(destinations, sourceSiteId, linkColumn)),
                    groupBy: Rds.ResultsGroupBy().ResultsGroupBy(linkColumn));
                default: return null;
            }
        }

        private static SqlColumnCollection IssuesMinColumn(
            string linkColumn, string sourceColumn)
        {
            var issuesColumn = Rds.IssuesColumn().IssuesColumn(linkColumn, _as: "Id");
            switch (sourceColumn)
            {
                case "WorkValue":
                    return issuesColumn.WorkValue(
                        _as: "Value", function: Sqls.Functions.Min);
                case "RemainingWorkValue":
                    return issuesColumn.RemainingWorkValue(
                        _as: "Value", function: Sqls.Functions.Min);
                default:
                    switch (Def.ExtendedColumnTypes.Get(sourceColumn ?? string.Empty))
                    {
                        case "Num":
                            return issuesColumn.Add(
                                columnBracket: $"\"{sourceColumn}\"",
                                columnName: sourceColumn,
                                _as: "Value",
                                function: Sqls.Functions.Min);
                        default:
                            return null;
                    }
            }
        }

        private static SqlColumnCollection ResultsMinColumn(
            string linkColumn, string sourceColumn)
        {
            var resultsColumn = Rds.ResultsColumn().ResultsColumn(linkColumn, _as: "Id");
            switch (sourceColumn)
            {
                default:
                    switch (Def.ExtendedColumnTypes.Get(sourceColumn ?? string.Empty))
                    {
                        case "Num":
                            return resultsColumn.Add(
                                columnBracket: $"\"{sourceColumn}\"",
                                columnName: sourceColumn,
                                _as: "Value",
                                function: Sqls.Functions.Min);
                        default:
                            return null;
                    }
            }
        }

        private static SqlSelect SelectMax(
            Context context,
            SiteSettings ss,
            IEnumerable<long> destinations,
            long sourceSiteId,
            string sourceReferenceType,
            string linkColumn,
            string sourceColumn,
            View sourceCondition)
        {
            switch (sourceReferenceType)
            {
                case "Issues": return Rds.SelectIssues(
                    column: IssuesMaxColumn(linkColumn, sourceColumn),
                    where: Where(
                        context: context,
                        ss: ss,
                        view: sourceCondition,
                        where: IssuesWhere(destinations, sourceSiteId, linkColumn)),
                    groupBy: Rds.IssuesGroupBy().IssuesGroupBy(linkColumn));
                case "Results": return Rds.SelectResults(
                    column: ResultsMaxColumn(linkColumn, sourceColumn),
                    where: Where(
                        context: context,
                        ss: ss,
                        view: sourceCondition,
                        where: ResultsWhere(destinations, sourceSiteId, linkColumn)),
                    groupBy: Rds.ResultsGroupBy().ResultsGroupBy(linkColumn));
                default: return null;
            }
        }

        private static SqlColumnCollection IssuesMaxColumn(
            string linkColumn, string sourceColumn)
        {
            var issuesColumn = Rds.IssuesColumn().IssuesColumn(linkColumn, _as: "Id");
            switch (sourceColumn)
            {
                case "WorkValue":
                    return issuesColumn.WorkValue(
                        _as: "Value", function: Sqls.Functions.Max);
                case "RemainingWorkValue":
                    return issuesColumn.RemainingWorkValue(
                        _as: "Value", function: Sqls.Functions.Max);
                default:
                    switch (Def.ExtendedColumnTypes.Get(sourceColumn ?? string.Empty))
                    {
                        case "Num":
                            return issuesColumn.Add(
                                columnBracket: $"\"{sourceColumn}\"",
                                columnName: sourceColumn,
                                _as: "Value",
                                function: Sqls.Functions.Max);
                        default:
                            return null;
                    }
            }
        }

        private static SqlColumnCollection ResultsMaxColumn(
            string linkColumn, string sourceColumn)
        {
            var resultsColumn = Rds.ResultsColumn().ResultsColumn(linkColumn, _as: "Id");
            switch (sourceColumn)
            {
                default:
                    switch (Def.ExtendedColumnTypes.Get(sourceColumn ?? string.Empty))
                    {
                        case "Num":
                            return resultsColumn.Add(
                                columnBracket: $"\"{sourceColumn}\"",
                                columnName: sourceColumn,
                                _as: "Value",
                                function: Sqls.Functions.Max);
                        default:
                            return null;
                    }
            }
        }

        private static SqlWhereCollection IssuesWhere(
            IEnumerable<long> destinations, long sourceSiteId, string linkColumn)
        {
            return Def.ExtendedColumnTypes.ContainsKey(linkColumn ?? string.Empty)
                ? Rds.IssuesWhere()
                    .SiteId(value: sourceSiteId)
                    .Add(
                        columnBrackets: new string[] { $"\"{linkColumn}\"" },
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ")
                : null;
        }

        private static SqlWhereCollection ResultsWhere(
            IEnumerable<long> destinations, long sourceSiteId, string linkColumn)
        {
            return Def.ExtendedColumnTypes.ContainsKey(linkColumn ?? string.Empty)
                ? Rds.ResultsWhere()
                    .SiteId(value: sourceSiteId)
                    .Add(
                        columnBrackets: new string[] { $"\"{linkColumn}\"" },
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ")
                : null;
        }

        private static SqlWhereCollection Where(
            Context context,
            SiteSettings ss,
            View view,
            SqlWhereCollection where)
        {
            return view != null
                ? view.Where(context: context, ss: ss, where: where)
                : where;
        }
    }
}
