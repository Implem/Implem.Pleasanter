using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.Interfaces;
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
        public static ResponseCollection ClearItemDataResponse(
            Context context, SiteSettings ss, long id)
        {
            var formDataSet = new FormDataSet(
                context: context,
                ss: ss);
            var res = new ResponseCollection().ClearFormData("Id");
            formDataSet
                .Where(o => !o.Suffix.IsNullOrEmpty())
                .Where(o => o.Id == id)
                .ForEach(formData =>
                    formData.Data.Keys.ForEach(controlId =>
                        res.ClearFormData(controlId + formData.Suffix)));
            return res;
        }

        public static SqlJoinCollection ItemJoin(
            this SqlJoinCollection join,
            Sqls.TableTypes tableType,
            string tableName)
        {
            switch (tableName)
            {
                case "Sites":
                case "Issues":
                case "Results":
                case "Wikis":
                    var tableTypeName = string.Empty;
                    switch (tableType)
                    {
                        case Sqls.TableTypes.History:
                            tableTypeName = "_history";
                            break;
                        case Sqls.TableTypes.Deleted:
                            tableTypeName = "_deleted";
                            break;
                    }
                    return join.Add(
                        tableName: "Items" + tableTypeName,
                        joinType: SqlJoin.JoinTypes.Inner,
                        joinExpression: $"[{tableName}].[{Rds.IdColumn(tableName)}]=[{tableName}_Items].[ReferenceId]",
                        _as: tableName + "_Items");
                default:
                    return join;
            }
        }

        public static void UpdateTitles(Context context, SiteSettings ss, long id)
        {
            UpdateTitles(
                context: context,
                ss: ss,
                idList: id.ToSingleList());
        }

        public static void UpdateSourceTitles(Context context, SiteSettings ss, IList<long> idList)
        {
            ss.Sources
                .ForEach(source =>
                {
                    var currentSs = source.Value;
                    var columns = currentSs?.Links?
                        .Where(o => o.SiteId == ss.SiteId)
                        .Select(o => o.ColumnName)
                        .ToList();
                    if (currentSs?.TitleColumns?.Any(o => columns?.Contains(o) == true) == true)
                    {
                        var nextIdList =
                            Rds.ExecuteTable(
                                context: context,
                                statements: Rds.SelectLinks(
                                column: Rds.LinksColumn()
                                    .SourceId(),
                                join: Rds.LinksJoinDefault(),
                                where: Rds.LinksWhere()
                                    .SiteId(currentSs.SiteId)
                                    .DestinationId_In(
                                        value: idList,
                                        _using: idList.Count <= 100)))
                            .AsEnumerable()
                            .Select(dataRow => dataRow.Long("SourceId"))
                            .ToList();
                        UpdateTitles(
                            context: context,
                            ss: currentSs,
                            idList: nextIdList);
                    }
                });
        }

        public static void UpdateTitles(
            Context context, SiteSettings ss, IList<long> idList = null)
        {
            switch (ss?.ReferenceType)
            {
                case "Issues":
                    UpdateIssueTitles(
                        context: context,
                        ss: ss,
                        idList: idList);
                    break;
                case "Results":
                    UpdateResultTitles(
                        context: context,
                        ss: ss,
                        idList: idList);
                    break;
                case "Wikis":
                    UpdateWikiTitles(
                        context: context,
                        ss: ss,
                        idList: idList);
                    break;
                default:
                    break;
            }
        }

        private static void UpdateIssueTitles(
            Context context, SiteSettings ss, IList<long> idList)
        {
            var issues = GetIssues(
                context: context,
                ss: ss,
                idList: idList);
            ss.Links?
                .ForEach(link =>
                    ss.SetChoiceHash(
                        context: context,
                        columnName: link.ColumnName,
                        selectedValues: issues
                            .Select(o => o.PropertyValue(
                                context: context,
                                name: link.ColumnName))
                            .Distinct(),
                        noLimit: true,
                        searchColumnOnly: false));
            if (ss.Links?.Any(o => ss.TitleColumns.Any(p => p == o.ColumnName)) == true)
            {
                issues.ForEach(issueModel =>
                    issueModel.Title = new Title(
                        context: context,
                        ss: ss,
                        id: issueModel.IssueId,
                        ver: issueModel.Ver,
                        isHistory: issueModel.VerType == Versions.VerTypes.History, 
                        data: issueModel.PropertyValues(
                            context: context,
                            names: ss.TitleColumns)));
            }
            issues.ForEach(issueModel =>
                Rds.ExecuteNonQuery(
                    context: context,
                    statements: Rds.UpdateItems(
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
                UpdateSourceTitles(
                    context: context,
                    ss: ss,
                    idList: issues.Select(o => o.IssueId).ToList());
            }
        }

        private static List<IssueModel> GetIssues(
            Context context, SiteSettings ss, IList<long> idList)
        {
            var column = Rds.IssuesColumn()
                .IssueId()
                .Title();
            ss.TitleColumns.ForEach(columnName =>
                column.IssuesColumn(columnName: columnName));
            return idList?.Count <= 100
                ? new IssueCollection(
                    context: context,
                    ss: ss,
                    column: column,
                    where: Rds.IssuesWhere()
                        .SiteId(ss.SiteId)
                        .IssueId_In(idList))
                : new IssueCollection(
                    context: context,
                    ss: ss,
                    column: column,
                    where: Rds.IssuesWhere().SiteId(ss.SiteId));
        }

        private static void UpdateResultTitles(
            Context context, SiteSettings ss, IList<long> idList)
        {
            var results = GetResults(
                context: context,
                ss: ss,
                idList: idList);
            ss.Links?
                .ForEach(link =>
                    ss.SetChoiceHash(
                        context: context,
                        columnName: link.ColumnName,
                        selectedValues: results
                            .Select(o => o.PropertyValue(
                                context: context,
                                name: link.ColumnName))
                            .Distinct(),
                        noLimit: true,
                        searchColumnOnly: false));
            if (ss.Links?.Any(o => ss.TitleColumns.Any(p => p == o.ColumnName)) == true)
            {
                results.ForEach(resultModel =>
                    resultModel.Title = new Title(
                        context: context,
                        ss: ss,
                        id: resultModel.ResultId,
                        ver: resultModel.Ver,
                        isHistory: resultModel.VerType == Versions.VerTypes.History, 
                        data: resultModel.PropertyValues(
                            context: context,
                            names: ss.TitleColumns)));
            }
            results.ForEach(resultModel =>
                Rds.ExecuteNonQuery(
                    context: context,
                    statements: Rds.UpdateItems(
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
                UpdateSourceTitles(
                    context: context,
                    ss: ss,
                    idList: results.Select(o => o.ResultId).ToList());
            }
        }

        private static List<ResultModel> GetResults(
            Context context, SiteSettings ss, IList<long> idList)
        {
            var column = Rds.ResultsColumn()
                .ResultId()
                .Title();
            ss.TitleColumns.ForEach(columnName =>
                column.ResultsColumn(columnName: columnName));
            return idList?.Count <= 100
                ? new ResultCollection(
                    context: context,
                    ss: ss,
                    column: column,
                    where: Rds.ResultsWhere()
                        .SiteId(ss.SiteId)
                        .ResultId_In(idList))
                : new ResultCollection(
                    context: context,
                    ss: ss,
                    column: column,
                    where: Rds.ResultsWhere().SiteId(ss.SiteId));
        }

        private static void UpdateWikiTitles(
            Context context, SiteSettings ss, IList<long> idList)
        {
            var wikis = GetWikis(
                context: context,
                ss: ss,
                idList: idList);
            ss.Links?
                .ForEach(link =>
                    ss.SetChoiceHash(
                        context: context,
                        columnName: link.ColumnName,
                        selectedValues: wikis
                            .Select(o => o.PropertyValue(
                                context: context,
                                name: link.ColumnName))
                            .Distinct(),
                        noLimit: true,
                        searchColumnOnly: false));
            if (ss.Links?.Any(o => ss.TitleColumns.Any(p => p == o.ColumnName)) == true)
            {
                wikis.ForEach(wikiModel =>
                    wikiModel.Title = new Title(
                        context: context,
                        ss: ss,
                        id: wikiModel.WikiId,
                        ver: wikiModel.Ver,
                        isHistory: wikiModel.VerType == Versions.VerTypes.History, 
                        data: wikiModel.PropertyValues(
                            context: context,
                            names: ss.TitleColumns)));
            }
            wikis.ForEach(wikiModel =>
                Rds.ExecuteNonQuery(
                    context: context,
                    statements: Rds.UpdateItems(
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
                UpdateSourceTitles(
                    context: context,
                    ss: ss,
                    idList: wikis.Select(o => o.WikiId).ToList());
            }
        }

        private static List<WikiModel> GetWikis(
            Context context, SiteSettings ss, IList<long> idList)
        {
            var column = Rds.WikisColumn()
                .WikiId()
                .Title();
            ss.TitleColumns.ForEach(columnName =>
                column.WikisColumn(columnName: columnName));
            return idList?.Count <= 100
                ? new WikiCollection(
                    context: context,
                    ss: ss,
                    column: column,
                    where: Rds.WikisWhere()
                        .SiteId(ss.SiteId)
                        .WikiId_In(idList))
                : new WikiCollection(
                    context: context,
                    ss: ss,
                    column: column,
                    where: Rds.WikisWhere().SiteId(ss.SiteId));
        }
    }
}
