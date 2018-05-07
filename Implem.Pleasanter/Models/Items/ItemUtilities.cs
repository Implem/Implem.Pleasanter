using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Resources;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Models
{
    public static class ItemUtilities
    {
        public static void UpdateTitles(long siteId, long id)
        {
            UpdateTitles(siteId, id.ToSingleList());
        }

        public static void UpdateTitles(long siteId, IEnumerable<long> idList)
        {
            idList
                .Chunk(100)
                .SelectMany(chunked =>
                    Rds.ExecuteTable(statements: Rds.SelectLinks(
                        column: Rds.LinksColumn()
                            .SourceId()
                            .SiteId(),
                        join: Rds.LinksJoinDefault(),
                        where: Rds.LinksWhere().DestinationId_In(chunked)))
                            .AsEnumerable())
                .Select(dataRow => new
                {
                    Id = dataRow.Long("SourceId"),
                    SiteId = dataRow.Long("SiteId")
                })
                .GroupBy(o => o.SiteId)
                .ForEach(links =>
                {
                    var targetSiteId = links.First().SiteId;
                    var siteModel = new SiteModel(targetSiteId);
                    var ss = siteModel.AccessStatus == Databases.AccessStatuses.Selected
                        ? SiteSettingsUtilities.Get(siteModel, targetSiteId)
                        : null;
                    var columns = ss?.Links?
                        .Where(o => o.SiteId == siteId)
                        .Select(o => o.ColumnName)
                        .ToList();
                    if (ss?.TitleColumns?.Any(o => columns?.Contains(o) == true) == true)
                    {
                        UpdateTitles(ss, links.Select(o => o.Id));
                    }
                });
        }

        public static void UpdateTitles(SiteSettings ss, IEnumerable<long> idList = null)
        {
            switch (ss?.ReferenceType)
            {
                case "Issues": UpdateIssueTitles(ss, idList); break;
                case "Results": UpdateResultTitles(ss, idList); break;
                case "Wikis": UpdateWikiTitles(ss, idList); break;
                default: break;
            }
        }

        private static void UpdateIssueTitles(SiteSettings ss, IEnumerable<long> idList)
        {
            var issues = GetIssues(ss, idList);
            var links = ss.GetUseSearchLinks();
            links?.ForEach(link =>
                ss.SetChoiceHash(
                    columnName: link.ColumnName,
                    selectedValues: issues
                        .Select(o => o.PropertyValue(link.ColumnName))
                        .Distinct(),
                    noLimit: true));
            if (links?.Any(o => ss.TitleColumns.Any(p => p == o.ColumnName)) == true)
            {
                issues.ForEach(issueModel =>
                    issueModel.Title = new Title(
                        ss,
                        issueModel.IssueId,
                        issueModel.PropertyValues(ss.TitleColumns)));
            }
            issues.ForEach(issueModel =>
                Rds.ExecuteNonQuery(statements: Rds.UpdateItems(
                    param: Rds.ItemsParam()
                        .Title(issueModel.Title.DisplayValue)
                        .SearchIndexCreatedTime(raw: "null"),
                    where: Rds.ItemsWhere()
                        .ReferenceId(issueModel.IssueId)
                        .SiteId(ss.SiteId),
                    addUpdatorParam: false,
                    addUpdatedTimeParam: false)));
            if (ss.Sources?.Any() == true)
            {
                UpdateTitles(ss.SiteId, issues.Select(o => o.IssueId));
            }
        }

        private static List<IssueModel> GetIssues(SiteSettings ss, IEnumerable<long> idList)
        {
            var column = Rds.IssuesColumn()
                .IssueId()
                .Title();
            ss.TitleColumns.ForEach(o => column.IssuesColumn(o));
            return idList?.Any() == true
                ? idList
                    .Chunk(100)
                    .SelectMany(chunked =>
                        new IssueCollection(
                            ss,
                            column: column,
                            where: Rds.IssuesWhere()
                                .SiteId(ss.SiteId)
                                .IssueId_In(idList)))
                    .ToList()
                : new IssueCollection(
                    ss,
                    column: column,
                    where: Rds.IssuesWhere().SiteId(ss.SiteId));
        }

        private static void UpdateResultTitles(SiteSettings ss, IEnumerable<long> idList)
        {
            var results = GetResults(ss, idList);
            var links = ss.GetUseSearchLinks();
            links?.ForEach(link =>
                ss.SetChoiceHash(
                    columnName: link.ColumnName,
                    selectedValues: results
                        .Select(o => o.PropertyValue(link.ColumnName))
                        .Distinct(),
                    noLimit: true));
            if (links?.Any(o => ss.TitleColumns.Any(p => p == o.ColumnName)) == true)
            {
                results.ForEach(resultModel =>
                    resultModel.Title = new Title(
                        ss,
                        resultModel.ResultId,
                        resultModel.PropertyValues(ss.TitleColumns)));
            }
            results.ForEach(resultModel =>
                Rds.ExecuteNonQuery(statements: Rds.UpdateItems(
                    param: Rds.ItemsParam()
                        .Title(resultModel.Title.DisplayValue)
                        .SearchIndexCreatedTime(raw: "null"),
                    where: Rds.ItemsWhere()
                        .ReferenceId(resultModel.ResultId)
                        .SiteId(ss.SiteId),
                    addUpdatorParam: false,
                    addUpdatedTimeParam: false)));
            if (ss.Sources?.Any() == true)
            {
                UpdateTitles(ss.SiteId, results.Select(o => o.ResultId));
            }
        }

        private static List<ResultModel> GetResults(SiteSettings ss, IEnumerable<long> idList)
        {
            var column = Rds.ResultsColumn()
                .ResultId()
                .Title();
            ss.TitleColumns.ForEach(o => column.ResultsColumn(o));
            return idList?.Any() == true
                ? idList
                    .Chunk(100)
                    .SelectMany(chunked =>
                        new ResultCollection(
                            ss,
                            column: column,
                            where: Rds.ResultsWhere()
                                .SiteId(ss.SiteId)
                                .ResultId_In(idList)))
                    .ToList()
                : new ResultCollection(
                    ss,
                    column: column,
                    where: Rds.ResultsWhere().SiteId(ss.SiteId));
        }

        private static void UpdateWikiTitles(SiteSettings ss, IEnumerable<long> idList)
        {
            var wikis = GetWikis(ss, idList);
            var links = ss.GetUseSearchLinks();
            links?.ForEach(link =>
                ss.SetChoiceHash(
                    columnName: link.ColumnName,
                    selectedValues: wikis
                        .Select(o => o.PropertyValue(link.ColumnName))
                        .Distinct(),
                    noLimit: true));
            if (links?.Any(o => ss.TitleColumns.Any(p => p == o.ColumnName)) == true)
            {
                wikis.ForEach(wikiModel =>
                    wikiModel.Title = new Title(
                        ss,
                        wikiModel.WikiId,
                        wikiModel.PropertyValues(ss.TitleColumns)));
            }
            wikis.ForEach(wikiModel =>
                Rds.ExecuteNonQuery(statements: Rds.UpdateItems(
                    param: Rds.ItemsParam()
                        .Title(wikiModel.Title.DisplayValue)
                        .SearchIndexCreatedTime(raw: "null"),
                    where: Rds.ItemsWhere()
                        .ReferenceId(wikiModel.WikiId)
                        .SiteId(ss.SiteId),
                    addUpdatorParam: false,
                    addUpdatedTimeParam: false)));
            if (ss.Sources?.Any() == true)
            {
                UpdateTitles(ss.SiteId, wikis.Select(o => o.WikiId));
            }
        }

        private static List<WikiModel> GetWikis(SiteSettings ss, IEnumerable<long> idList)
        {
            var column = Rds.WikisColumn()
                .WikiId()
                .Title();
            ss.TitleColumns.ForEach(o => column.WikisColumn(o));
            return idList?.Any() == true
                ? idList
                    .Chunk(100)
                    .SelectMany(chunked =>
                        new WikiCollection(
                            ss,
                            column: column,
                            where: Rds.WikisWhere()
                                .SiteId(ss.SiteId)
                                .WikiId_In(idList)))
                    .ToList()
                : new WikiCollection(
                    ss,
                    column: column,
                    where: Rds.WikisWhere().SiteId(ss.SiteId));
        }
    }
}
