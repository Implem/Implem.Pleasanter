using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
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
        public static string Title(long referenceId, List<Link> links)
        {
            return Rds.ExecuteScalar_string(statements:
                Rds.SelectItems(
                    column: Rds.ItemsColumn().Title(),
                    where: Rds.ItemsWhere()
                        .ReferenceId(referenceId)
                        .SiteId_In(links?.Select(o => o.SiteId))));
        }

        public static void UpdateTitles(long id)
        {
            Rds.ExecuteTable(statements: Rds.SelectLinks(
                column: Rds.LinksColumn()
                    .SourceId()
                    .SiteId(),
                join: Rds.LinksJoinDefault(),
                where: Rds.LinksWhere().DestinationId(id)))
                    .AsEnumerable()
                    .Select(o => new
                    {
                        Id = o["SourceId"].ToLong(),
                        SiteId = o["SiteId"].ToLong()
                    })
                    .GroupBy(o => o.SiteId)
                    .ForEach(sites =>
                    {
                        var siteModel = new SiteModel(sites.First().SiteId);
                        var ss = siteModel.AccessStatus == Databases.AccessStatuses.Selected
                            ? SiteSettingsUtilities.Get(siteModel, sites.First().Id)
                            : null;
                        UpdateTitles(ss, sites.Select(o => o.Id));
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
            var column = Rds.IssuesColumn().IssueId();
            ss.TitleColumns.ForEach(o => column.IssuesColumn(o));
            var issueCollection = new IssueCollection(
                ss,
                column: column,
                where: Rds.IssuesWhere()
                    .SiteId(ss.SiteId)
                    .IssueId_In(idList, _using: idList != null));
            var links = ss.GetUseSearchLinks(titleOnly: true);
            links?.ForEach(link =>
                ss.SetChoiceHash(
                    columnName: link.ColumnName,
                    selectedValues: issueCollection
                        .Select(o => o.PropertyValue(link.ColumnName))
                        .Distinct()));
            if (links?.Any(o => ss.TitleColumns.Any(p => p == o.ColumnName)) == true)
            {
                issueCollection.ForEach(issueModel =>
                    issueModel.Title = new Title(
                        ss,
                        issueModel.IssueId,
                        issueModel.PropertyValues(ss.TitleColumns)));
            }
            issueCollection.ForEach(issueModel =>
                Rds.ExecuteNonQuery(statements: Rds.UpdateItems(
                    param: Rds.ItemsParam()
                        .Title(issueModel.Title.DisplayValue)
                        .SearchIndexCreatedTime(raw: "null"),
                    where: Rds.ItemsWhere().ReferenceId(issueModel.IssueId),
                    addUpdatorParam: false,
                    addUpdatedTimeParam: false)));
            if (ss.Sources?.Any() == true)
            {
                issueCollection.ForEach(issueModel =>
                    UpdateTitles(issueModel.IssueId));
            }
        }

        private static void UpdateResultTitles(SiteSettings ss, IEnumerable<long> idList)
        {
            var column = Rds.ResultsColumn().ResultId();
            ss.TitleColumns.ForEach(o => column.ResultsColumn(o));
            var resultCollection = new ResultCollection(
                ss,
                column: column,
                where: Rds.ResultsWhere()
                    .SiteId(ss.SiteId)
                    .ResultId_In(idList, _using: idList != null));
            var links = ss.GetUseSearchLinks(titleOnly: true);
            links?.ForEach(link =>
                ss.SetChoiceHash(
                    columnName: link.ColumnName,
                    selectedValues: resultCollection
                        .Select(o => o.PropertyValue(link.ColumnName))
                        .Distinct()));
            if (links?.Any(o => ss.TitleColumns.Any(p => p == o.ColumnName)) == true)
            {
                resultCollection.ForEach(resultModel =>
                    resultModel.Title = new Title(
                        ss,
                        resultModel.ResultId,
                        resultModel.PropertyValues(ss.TitleColumns)));
            }
            resultCollection.ForEach(resultModel =>
                Rds.ExecuteNonQuery(statements: Rds.UpdateItems(
                    param: Rds.ItemsParam()
                        .Title(resultModel.Title.DisplayValue)
                        .SearchIndexCreatedTime(raw: "null"),
                    where: Rds.ItemsWhere().ReferenceId(resultModel.ResultId),
                    addUpdatorParam: false,
                    addUpdatedTimeParam: false)));
            if (ss.Sources?.Any() == true)
            {
                resultCollection.ForEach(resultModel =>
                    UpdateTitles(resultModel.ResultId));
            }
        }

        private static void UpdateWikiTitles(SiteSettings ss, IEnumerable<long> idList)
        {
            var column = Rds.WikisColumn().WikiId();
            ss.TitleColumns.ForEach(o => column.WikisColumn(o));
            var wikiCollection = new WikiCollection(
                ss,
                column: column,
                where: Rds.WikisWhere()
                    .SiteId(ss.SiteId)
                    .WikiId_In(idList, _using: idList != null));
            var links = ss.GetUseSearchLinks(titleOnly: true);
            links?.ForEach(link =>
                ss.SetChoiceHash(
                    columnName: link.ColumnName,
                    selectedValues: wikiCollection
                        .Select(o => o.PropertyValue(link.ColumnName))
                        .Distinct()));
            if (links?.Any(o => ss.TitleColumns.Any(p => p == o.ColumnName)) == true)
            {
                wikiCollection.ForEach(wikiModel =>
                    wikiModel.Title = new Title(
                        ss,
                        wikiModel.WikiId,
                        wikiModel.PropertyValues(ss.TitleColumns)));
            }
            wikiCollection.ForEach(wikiModel =>
                Rds.ExecuteNonQuery(statements: Rds.UpdateItems(
                    param: Rds.ItemsParam()
                        .Title(wikiModel.Title.DisplayValue)
                        .SearchIndexCreatedTime(raw: "null"),
                    where: Rds.ItemsWhere().ReferenceId(wikiModel.WikiId),
                    addUpdatorParam: false,
                    addUpdatedTimeParam: false)));
            if (ss.Sources?.Any() == true)
            {
                wikiCollection.ForEach(wikiModel =>
                    UpdateTitles(wikiModel.WikiId));
            }
        }
    }
}
