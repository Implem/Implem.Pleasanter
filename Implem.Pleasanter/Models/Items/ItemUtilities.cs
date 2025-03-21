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
using Implem.Pleasanter.Libraries.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Web;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
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
            var res = new ResponseCollection(context: context).ClearFormData("Id");
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
                case "Dashboards":
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
                        tableName: $"\"Items{tableTypeName}\"",
                        joinType: SqlJoin.JoinTypes.Inner,
                        joinExpression: $"\"{tableName}\".\"{Rds.IdColumn(tableName)}\"=\"{tableName}_Items\".\"ReferenceId\" and \"{tableName}\".\"SiteId\"=\"{tableName}_Items\".\"SiteId\"",
                        _as: tableName + "_Items");
                default:
                    return join;
            }
        }

        public static void UpdateSourceTitles(
            Context context,
            SiteSettings ss,
            IList<long> siteIdList,
            IList<long> idList)
        {
            ss.Sources.ForEach(source =>
            {
                var currentSs = source.Value;
                var columns = currentSs?.Links
                    ?.Where(o => o.SiteId > 0)
                    .Where(o => o.SiteId == ss.SiteId)
                    .Select(o => o.ColumnName)
                    .ToList();
                if (currentSs?.TitleColumns?.Any(o => columns?.Contains(o) == true) == true)
                {
                    currentSs.SetLinkedSiteSettings(context: context);
                    // 再帰的に呼び出されたいる場合は実行しない
                    if (!siteIdList.Contains(currentSs.SiteId))
                    {
                        siteIdList.Add(currentSs.SiteId);
                        var nextIdList = Repository.ExecuteTable(
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
                            siteIdList: siteIdList,
                            idList: nextIdList);
                    }
                }
            });
        }

        public static void UpdateTitles(
            Context context,
            SiteSettings ss,
            IList<long> siteIdList,
            IList<long> idList = null)
        {
            switch (ss?.ReferenceType)
            {
                case "Issues":
                    UpdateIssueTitles(
                        context: context,
                        ss: ss,
                        siteIdList: siteIdList,
                        idList: idList);
                    break;
                case "Results":
                    UpdateResultTitles(
                        context: context,
                        ss: ss,
                        siteIdList: siteIdList,
                        idList: idList);
                    break;
                case "Wikis":
                    UpdateWikiTitles(
                        context: context,
                        ss: ss,
                        siteIdList: siteIdList,
                        idList: idList);
                    break;
                default:
                    break;
            }
        }

        private static void UpdateIssueTitles(
            Context context,
            SiteSettings ss,
            IList<long> siteIdList,
            IList<long> idList)
        {
            var issues = GetIssues(
                context: context,
                ss: ss,
                idList: idList);
            if (ss.Links
                ?.Where(o => o.SiteId > 0)
                .Any(o => ss.TitleColumns.Any(p => p == o.ColumnName)) == true)
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
                            columns: ss.GetTitleColumns(context: context)),
                        getLinkedTitle: true));
            }
            issues.ForEach(issueModel =>
                Repository.ExecuteNonQuery(
                    context: context,
                    statements: Rds.UpdateItems(
                        param: Rds.ItemsParam()
                            .Title(issueModel.Title.MessageDisplay(context: context))
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
                    siteIdList: siteIdList,
                    idList: issues.Select(o => o.IssueId).ToList());
            }
        }

        private static List<IssueModel> GetIssues(
            Context context,
            SiteSettings ss,
            IList<long> idList)
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
            Context context,
            SiteSettings ss,
            IList<long> siteIdList,
            IList<long> idList)
        {
            var results = GetResults(
                context: context,
                ss: ss,
                idList: idList);
            if (ss.Links
                ?.Where(o => o.SiteId > 0)
                .Any(o => ss.TitleColumns.Any(p => p == o.ColumnName)) == true)
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
                            columns: ss.GetTitleColumns(context: context)),
                        getLinkedTitle: true));
            }
            results.ForEach(resultModel =>
                Repository.ExecuteNonQuery(
                    context: context,
                    statements: Rds.UpdateItems(
                        param: Rds.ItemsParam()
                            .Title(resultModel.Title.MessageDisplay(context: context))
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
                    siteIdList: siteIdList,
                    idList: results.Select(o => o.ResultId).ToList());
            }
        }

        private static List<ResultModel> GetResults(
            Context context,
            SiteSettings ss,
            IList<long> idList)
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
            Context context,
            SiteSettings ss,
            IList<long> siteIdList,
            IList<long> idList)
        {
            var wikis = GetWikis(
                context: context,
                ss: ss,
                idList: idList);
            if (ss.Links
                ?.Where(o => o.SiteId > 0)
                .Any(o => ss.TitleColumns.Any(p => p == o.ColumnName)) == true)
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
                            columns: ss.GetTitleColumns(context: context)),
                        getLinkedTitle: true));
            }
            wikis.ForEach(wikiModel =>
                Repository.ExecuteNonQuery(
                    context: context,
                    statements: Rds.UpdateItems(
                        param: Rds.ItemsParam()
                            .Title(wikiModel.Title.MessageDisplay(context: context))
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
                    siteIdList: siteIdList,
                    idList: wikis.Select(o => o.WikiId).ToList());
            }
        }

        private static List<WikiModel> GetWikis(
            Context context,
            SiteSettings ss,
            IList<long> idList)
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

        public static void SetChoiceHashByFilterExpressions(
            Context context,
            SiteSettings ss,
            Column column,
            long referenceId,
            string searchText,
            int offset,
            bool search,
            bool searchFormat)
        {
            var controlId = context.Forms.Get("DropDownSearchTarget")?.Split('_');
            // 一覧編集の場合レコードを特定するためのsuffixデータを抽出
            var suffix = controlId?.Count() == 4
                ? $"_{controlId[2]}_{controlId[3]}"
                : string.Empty;
            // 一覧編集の場合はcontrolId[3]が-1で新規作成を判定
            var isNew = suffix.IsNullOrEmpty()
                ? context.Forms.Bool("IsNew")
                : controlId[3] == "-1";
            // 一覧編集の場合はsuffixが一致するフォームデータのみ抽出
            var formData = suffix.IsNullOrEmpty()
                ? context.Forms
                : context.Forms
                    .Where(o => o.Key.EndsWith(suffix))
                    .ToDictionary(
                        o => o.Key.CutEnd(suffix),
                        o => o.Value);
            var copyFrom = context.Forms.Int("CopyFrom");
            if (copyFrom > 0 && !Permissions.CanRead(
                context: context,
                siteId: context.SiteId,
                id: copyFrom))
            {
                return;
            }
            switch (ss.ReferenceType)
            {
                case "Issues":
                    IssueUtilities.SetChoiceHashByFilterExpressions(
                        context: context,
                        ss: ss,
                        column: column,
                        issueModel: isNew
                            ? new IssueModel(
                                context: context,
                                ss: ss,
                                issueId: copyFrom,
                                setCopyDefault: copyFrom > 0,
                                methodType: BaseModel.MethodTypes.New,
                                formData: formData)
                            : new IssueModel(
                                context: context,
                                ss: ss,
                                issueId: referenceId,
                                methodType: BaseModel.MethodTypes.Edit,
                                formData: formData),
                        searchText: searchText,
                        offset: offset,
                        search: search,
                        searchFormat: searchFormat);
                    break;
                case "Results":
                    ResultUtilities.SetChoiceHashByFilterExpressions(
                        context: context,
                        ss: ss,
                        column: column,
                        resultModel: isNew
                            ? new ResultModel(
                                context: context,
                                ss: ss,
                                resultId: copyFrom,
                                setCopyDefault: copyFrom > 0,
                                methodType: BaseModel.MethodTypes.New,
                                formData: formData)
                            : new ResultModel(
                                context: context,
                                ss: ss,
                                resultId: referenceId,
                                methodType: BaseModel.MethodTypes.Edit,
                                formData: formData),
                        searchText: searchText,
                        offset: offset,
                        search: search,
                        searchFormat: searchFormat);
                    break;
            }
        }
    }
}
