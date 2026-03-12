using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Implem.Pleasanter.Models
{
    public static class LinkedRecordReferenceUtilities
    {
        public static void CleanupDeletedReferences(
            Context context,
            SiteSettings ss,
            IEnumerable<long> deletedRecordIds)
        {
            var deletedIds = deletedRecordIds?
                .Where(id => id > 0)
                .Distinct()
                .ToList();
            if (deletedIds?.Any() != true
                || ss?.Sources?.Any() != true)
            {
                return;
            }
            ss.Sources.Values
                .Where(sourceSs =>
                    sourceSs?.Links?
                        .Any(link => link.SiteId == ss.SiteId)
                            == true)
                .Where(sourceSs =>
                    sourceSs.ReferenceType == "Issues"
                    || sourceSs.ReferenceType == "Results")
                .ToList()
                .ForEach(sourceSs =>
                {
                    if (CleanupSourceSite(
                        context: context,
                        sourceSs: sourceSs,
                        destinationSiteId: ss.SiteId,
                        deletedRecordIds: deletedIds))
                    {
                        Summaries.Synchronize(
                            context: context,
                            ss: sourceSs);
                    }
                });
        }

        private static bool CleanupSourceSite(
            Context context,
            SiteSettings sourceSs,
            long destinationSiteId,
            List<long> deletedRecordIds)
        {
            if (!context.CanUpdate(ss: sourceSs))
            {
                return false;
            }
            var cleanupColumns = sourceSs.Links?
                .Where(link => link.SiteId == destinationSiteId)
                .Select(link => sourceSs.GetColumn(
                    context: context,
                    columnName: link.ColumnName))
                .Where(column => column != null)
                .GroupBy(column => column.ColumnName)
                .Select(column => column.First())
                .ToList();
            if (cleanupColumns?.Any() != true)
            {
                return false;
            }
            var allLinkColumns = sourceSs.Links?
                .Where(link => link.SiteId > 0)
                .Select(link => sourceSs.GetColumn(
                    context: context,
                    columnName: link.ColumnName))
                .Where(column => column != null)
                .GroupBy(column => column.ColumnName)
                .Select(column => column.First())
                .ToList();
            if (allLinkColumns?.Any() != true)
            {
                return false;
            }
            var sourceSiteIds = sourceSs.IntegratedSites?.Any() == true
                ? sourceSs.AllowedIntegratedSites
                    ?? sourceSs.SiteId.ToSingleList()
                : sourceSs.SiteId.ToSingleList();
            if (sourceSiteIds?.Any() != true)
            {
                return false;
            }
            var deletedIdHash = new HashSet<long>(deletedRecordIds);
            var statements = new List<SqlStatement>();
            sourceSiteIds.ForEach(sourceSiteId =>
            {
                var dataTable = SelectSourceRows(
                    context: context,
                    sourceSs: sourceSs,
                    sourceSiteId: sourceSiteId,
                    deletedRecordIds: deletedRecordIds,
                    allLinkColumns: allLinkColumns);
                var dataRows = dataTable?.AsEnumerable().ToList();
                if (dataRows?.Any() != true)
                {
                    return;
                }
                dataRows.ForEach(dataRow =>
                {
                    var updatedValues = new Dictionary<string, string>();
                    cleanupColumns.ForEach(column =>
                    {
                        var updatedValue = RemoveDeletedReference(
                            value: dataRow.String(column.ColumnName),
                            multipleSelections: column.MultipleSelections == true,
                            deletedIdHash: deletedIdHash,
                            changed: out var changed);
                        if (changed)
                        {
                            updatedValues[column.ColumnName] = updatedValue;
                        }
                    });
                    if (updatedValues.Any())
                    {
                        AddUpdateStatements(
                            sourceSs: sourceSs,
                            sourceSiteId: sourceSiteId,
                            dataRow: dataRow,
                            allLinkColumns: allLinkColumns,
                            updatedValues: updatedValues,
                            statements: statements);
                    }
                });
            });
            if (!statements.Any())
            {
                return false;
            }
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            return true;
        }

        private static DataTable SelectSourceRows(
            Context context,
            SiteSettings sourceSs,
            long sourceSiteId,
            List<long> deletedRecordIds,
            List<Column> allLinkColumns)
        {
            var sourceSub = Rds.SelectLinks(
                column: Rds.LinksColumn().SourceId(),
                join: Rds.LinksJoinDefault(),
                where: Rds.LinksWhere()
                    .DestinationId_In(value: deletedRecordIds)
                    .Items_SiteId(sourceSiteId));
            switch (sourceSs.ReferenceType)
            {
                case "Issues":
                    var issueColumns = Rds.IssuesColumn().IssueId();
                    allLinkColumns.ForEach(column =>
                        issueColumns.IssuesColumn(column.ColumnName));
                    return Repository.ExecuteTable(
                        context: context,
                        statements: Rds.SelectIssues(
                            column: issueColumns,
                            where: Rds.IssuesWhere()
                                .SiteId(sourceSiteId)
                                .IssueId_In(sub: sourceSub)));
                case "Results":
                    var resultColumns = Rds.ResultsColumn().ResultId();
                    allLinkColumns.ForEach(column =>
                        resultColumns.ResultsColumn(column.ColumnName));
                    return Repository.ExecuteTable(
                        context: context,
                        statements: Rds.SelectResults(
                            column: resultColumns,
                            where: Rds.ResultsWhere()
                                .SiteId(sourceSiteId)
                                .ResultId_In(sub: sourceSub)));
                default:
                    return null;
            }
        }

        private static void AddUpdateStatements(
            SiteSettings sourceSs,
            long sourceSiteId,
            DataRow dataRow,
            List<Column> allLinkColumns,
            Dictionary<string, string> updatedValues,
            List<SqlStatement> statements)
        {
            switch (sourceSs.ReferenceType)
            {
                case "Issues":
                    var issueId = dataRow.Long("IssueId");
                    if (issueId <= 0)
                    {
                        return;
                    }
                    statements.Add(Rds.UpdateIssues(
                        where: Rds.IssuesWhere()
                            .SiteId(sourceSiteId)
                            .IssueId(issueId),
                        param: SetClassValues(
                            param: Rds.IssuesParam(),
                            values: updatedValues)));
                    statements.Add(Rds.PhysicalDeleteLinks(
                        where: Rds.LinksWhere().SourceId(issueId)));
                    statements.Add(LinkUtilities.Insert(
                        link: RebuildLinks(
                            sourceId: issueId,
                            dataRow: dataRow,
                            allLinkColumns: allLinkColumns,
                            updatedValues: updatedValues)));
                    break;
                case "Results":
                    var resultId = dataRow.Long("ResultId");
                    if (resultId <= 0)
                    {
                        return;
                    }
                    statements.Add(Rds.UpdateResults(
                        where: Rds.ResultsWhere()
                            .SiteId(sourceSiteId)
                            .ResultId(resultId),
                        param: SetClassValues(
                            param: Rds.ResultsParam(),
                            values: updatedValues)));
                    statements.Add(Rds.PhysicalDeleteLinks(
                        where: Rds.LinksWhere().SourceId(resultId)));
                    statements.Add(LinkUtilities.Insert(
                        link: RebuildLinks(
                            sourceId: resultId,
                            dataRow: dataRow,
                            allLinkColumns: allLinkColumns,
                            updatedValues: updatedValues)));
                    break;
            }
        }

        private static SqlParamCollection SetClassValues(
            SqlParamCollection param,
            Dictionary<string, string> values)
        {
            foreach (var value in values)
            {
                param.Add(
                    columnBracket: $"\"{value.Key}\"",
                    name: value.Key,
                    value: value.Value.MaxLength(1024),
                    sub: null,
                    raw: null);
            }
            return param;
        }

        private static Dictionary<long, long> RebuildLinks(
            long sourceId,
            DataRow dataRow,
            List<Column> allLinkColumns,
            Dictionary<string, string> updatedValues)
        {
            return allLinkColumns
                .SelectMany(column => ParseLinkIds(
                    value: updatedValues.ContainsKey(column.ColumnName)
                        ? updatedValues[column.ColumnName]
                        : dataRow.String(column.ColumnName),
                    multipleSelections: column.MultipleSelections == true))
                .Where(id => id > 0)
                .Distinct()
                .ToDictionary(id => id, id => sourceId);
        }

        private static string RemoveDeletedReference(
            string value,
            bool multipleSelections,
            HashSet<long> deletedIdHash,
            out bool changed)
        {
            changed = false;
            if (value.IsNullOrEmpty())
            {
                return value;
            }
            if (multipleSelections)
            {
                var ids = ParseMultipleSelectionIds(value: value);
                if (!ids.Any())
                {
                    return value;
                }
                var updatedIds = ids
                    .Where(id => !deletedIdHash.Contains(id))
                    .ToList();
                changed = updatedIds.Count != ids.Count;
                if (!changed)
                {
                    return value;
                }
                return updatedIds.Any()
                    ? updatedIds
                        .Select(id => id.ToString())
                        .ToList()
                        .ToJson()
                    : string.Empty;
            }
            var idValue = value.ToLong();
            if (idValue > 0
                && deletedIdHash.Contains(idValue))
            {
                changed = true;
                return string.Empty;
            }
            return value;
        }

        private static List<long> ParseLinkIds(string value, bool multipleSelections)
        {
            return multipleSelections
                ? ParseMultipleSelectionIds(value: value)
                : value.ToLong() > 0
                    ? value.ToLong().ToSingleList()
                    : new List<long>();
        }

        private static List<long> ParseMultipleSelectionIds(string value)
        {
            if (value.IsNullOrEmpty())
            {
                return new List<long>();
            }
            var stringIds = value.Deserialize<List<string>>();
            if (stringIds?.Any() == true)
            {
                return stringIds
                    .Select(id => id.ToLong())
                    .Where(id => id > 0)
                    .ToList();
            }
            var longIds = value.Deserialize<List<long>>();
            if (longIds?.Any() == true)
            {
                return longIds
                    .Where(id => id > 0)
                    .ToList();
            }
            return value.Split(',')
                .Select(id => id.ToLong())
                .Where(id => id > 0)
                .ToList();
        }
    }
}
