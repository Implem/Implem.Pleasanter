using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Models
{
    public static class Summaries
    {
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
            var destinationSs = SiteSettingsUtilities.Get(
                context: context, siteId: summary?.SiteId ?? 0);
            if (destinationSs != null && summary != null)
            {
                Synchronize(
                    context: context,
                    ss: ss,
                    destinationSs: destinationSs,
                    destinationSiteId: summary.SiteId,
                    destinationReferenceType: summary.DestinationReferenceType,
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
            string destinationReferenceType,
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
            switch (destinationReferenceType)
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
                var issueCollection = new IssueCollection(
                    context: context,
                    ss: destinationSs,
                    where: Where(
                        context: context, ss: destinationSs, view: null, where: where));
                var matchingConditions = destinationCondition != null
                    ? Rds.ExecuteTable(
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
                    : issueCollection
                        .Select(o => o.IssueId)
                        .ToList();
                var data = issueCollection.Any()
                    ? Data(
                        context: context,
                        ss: ss,
                        destinationColumn: destinationColumn,
                        destinations: issueCollection.Select(o => o.IssueId),
                        sourceSiteId: sourceSiteId,
                        sourceReferenceType: sourceReferenceType,
                        linkColumn: linkColumn,
                        type: type,
                        sourceColumn: sourceColumn,
                        sourceCondition: sourceCondition)
                    : new Dictionary<long, decimal>();
                issueCollection.ForEach(issueModel =>
                {
                    if (matchingConditions.Any(o => o == issueModel.IssueId))
                    {
                        Set(
                            issueModel,
                            destinationColumn,
                            data.Get(issueModel.IssueId));
                    }
                    else if (setZeroWhenOutOfCondition)
                    {
                        Set(issueModel, destinationColumn, 0);
                    }
                    if (issueModel.Updated(context: context))
                    {
                        issueModel.SetByFormula(context: context, ss: destinationSs);
                        issueModel.VerUp = Versions.MustVerUp(
                            context: context,
                            ss: ss,
                            baseModel: issueModel);
                        issueModel.SetChoiceHash(context: context, ss: destinationSs);
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
                    return Rds.ExecuteTable(
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
                    switch (Def.ExtendedColumnTypes.Get(destinationColumn))
                    {
                        case "Num":
                            return Rds.ExecuteTable(
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
                    switch (Def.ExtendedColumnTypes.Get(destinationColumn))
                    {
                        case "Num":
                            issueModel.Num(
                                columnName: destinationColumn,
                                value: value);
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
                var resultCollection = new ResultCollection(
                    context: context,
                    ss: destinationSs,
                    where: Where(
                        context: context, ss: destinationSs, view: null, where: where));
                var matchingConditions = destinationCondition != null
                    ? Rds.ExecuteTable(
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
                    : resultCollection
                        .Select(o => o.ResultId)
                        .ToList();
                var data = resultCollection.Any()
                    ? Data(
                        context: context,
                        ss: ss,
                        destinationColumn: destinationColumn,
                        destinations: resultCollection.Select(o => o.ResultId),
                        sourceSiteId: sourceSiteId,
                        sourceReferenceType: sourceReferenceType,
                        linkColumn: linkColumn,
                        type: type,
                        sourceColumn: sourceColumn,
                        sourceCondition: sourceCondition)
                    : new Dictionary<long, decimal>();
                resultCollection.ForEach(resultModel =>
                {
                    if (matchingConditions.Any(o => o == resultModel.ResultId))
                    {
                        Set(
                            resultModel,
                            destinationColumn,
                            data.Get(resultModel.ResultId));
                    }
                    else if (setZeroWhenOutOfCondition)
                    {
                        Set(resultModel, destinationColumn, 0);
                    }
                    if (resultModel.Updated(context: context))
                    {
                        resultModel.SetByFormula(context: context, ss: destinationSs);
                        resultModel.VerUp = Versions.MustVerUp(
                            context: context,
                            ss: ss,
                            baseModel: resultModel);
                        resultModel.SetChoiceHash(context: context, ss: destinationSs);
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
                    switch (Def.ExtendedColumnTypes.Get(destinationColumn))
                    {
                        case "Num":
                            return Rds.ExecuteTable(
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
                    switch (Def.ExtendedColumnTypes.Get(destinationColumn))
                    {
                        case "Num":
                            resultModel.Num(
                                columnName: destinationColumn,
                                value: value);
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
            string type,
            string sourceColumn,
            View sourceCondition)
        {
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
                    switch (Def.ExtendedColumnTypes.Get(sourceColumn))
                    {
                        case "Num":
                            return issuesColumn.Add(
                                columnBracket: $"[{sourceColumn}]",
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
                    switch (Def.ExtendedColumnTypes.Get(sourceColumn))
                    {
                        case "Num":
                            return resultsColumn.Add(
                                columnBracket: $"[{sourceColumn}]",
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
                    switch (Def.ExtendedColumnTypes.Get(sourceColumn))
                    {
                        case "Num":
                            return issuesColumn.Add(
                                columnBracket: $"[{sourceColumn}]",
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
                    switch (Def.ExtendedColumnTypes.Get(sourceColumn))
                    {
                        case "Num":
                            return resultsColumn.Add(
                                columnBracket: $"[{sourceColumn}]",
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
                    switch (Def.ExtendedColumnTypes.Get(sourceColumn))
                    {
                        case "Num":
                            return issuesColumn.Add(
                                columnBracket: $"[{sourceColumn}]",
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
                    switch (Def.ExtendedColumnTypes.Get(sourceColumn))
                    {
                        case "Num":
                            return resultsColumn.Add(
                                columnBracket: $"[{sourceColumn}]",
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
                    switch (Def.ExtendedColumnTypes.Get(sourceColumn))
                    {
                        case "Num":
                            return issuesColumn.Add(
                                columnBracket: $"[{sourceColumn}]",
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
                    switch (Def.ExtendedColumnTypes.Get(sourceColumn))
                    {
                        case "Num":
                            return resultsColumn.Add(
                                columnBracket: $"[{sourceColumn}]",
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
            return Def.ExtendedColumnTypes.ContainsKey(linkColumn)
                ? Rds.IssuesWhere()
                    .SiteId(value: sourceSiteId)
                    .Add(
                        columnBrackets: new string[] { $"[{linkColumn}]" },
                        raw: "({0})".Params(destinations
                            .Select(o => "'" + o + "'")
                            .Join(",")),
                        _operator: " in ")
                    : null;
        }

        private static SqlWhereCollection ResultsWhere(
            IEnumerable<long> destinations, long sourceSiteId, string linkColumn)
        {
            return Def.ExtendedColumnTypes.ContainsKey(linkColumn)
                ? Rds.ResultsWhere()
                    .SiteId(value: sourceSiteId)
                    .Add(
                        columnBrackets: new string[] { $"[{linkColumn}]" },
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
