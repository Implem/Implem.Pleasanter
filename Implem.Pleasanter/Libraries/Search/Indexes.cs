using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Implem.Pleasanter.Libraries.Search
{
    public static class Indexes
    {
        public static void Create(Context context, SiteSettings ss, long id, bool force = false)
        {
            if (Parameters.Search.Provider.IsNullOrEmpty() && Parameters.Search.CreateIndexes)
            {
                var itemModel = new ItemModel(
                    context: context,
                    referenceId: id);
                switch (itemModel.ReferenceType)
                {
                    case "Sites":
                        var siteModel = new SiteModel(context: context, siteId: id);
                        Task.Run(() => CreateIndexes(
                            context: context,
                            ss: ss,
                            hash: siteModel.SearchIndexHash(context: context, ss: ss),
                            id: id));
                        break;
                    case "Issues":
                        var issueModel = new IssueModel(context: context, ss: ss, issueId: id);
                        Task.Run(() => CreateIndexes(
                            context: context,
                            ss: ss,
                            hash: issueModel.SearchIndexHash(context: context, ss: ss),
                            id: id));
                        break;
                    case "Results":
                        var resultModel = new ResultModel(context: context, ss: ss, resultId: id);
                        Task.Run(() => CreateIndexes(
                            context: context,
                            ss: ss,
                            hash: resultModel.SearchIndexHash(context: context, ss: ss),
                            id: id));
                        break;
                    case "Wikis":
                        var wikiModel = new WikiModel(context: context, ss: ss, wikiId: id);
                        Task.Run(() => CreateIndexes(
                            context: context,
                            ss: ss,
                            hash: wikiModel.SearchIndexHash(context: context, ss: ss),
                            id: id));
                        break;
                }
            }
            else if (force)
            {
                var itemModel = new ItemModel(
                    context: context,
                    referenceId: id);
                switch (itemModel.ReferenceType)
                {
                    case "Sites":
                        var siteModel = new SiteModel(context: context, siteId: id);
                        switch (Parameters.Search.Provider)
                        {
                            case "FullText":
                                CreateFullText(
                                    context: context,
                                    id: id,
                                    fullText: siteModel.FullText(
                                        context: context,
                                        ss: ss,
                                        backgroundTask: true));
                                break;
                            default:
                                CreateIndexes(
                                    context: context,
                                    ss: ss,
                                    hash: siteModel.SearchIndexHash(context: context, ss: ss),
                                    id: id);
                                break;
                        }
                        break;
                    case "Issues":
                        var issueModel = new IssueModel(
                            context: context,
                            ss: ss,
                            issueId: id);
                        ss.Links?
                            .Where(o => ss.GetColumn(
                                context: context,
                                columnName: o.ColumnName).UseSearch == true)
                            .ForEach(link =>
                                ss.SetChoiceHash(
                                    context: context,
                                    columnName: link.ColumnName,
                                    selectedValues: new List<string>
                                    {
                                        issueModel.PropertyValue(
                                            context: context, name: link.ColumnName)
                                    }));
                        switch (Parameters.Search.Provider)
                        {
                            case "FullText":
                                CreateFullText(
                                    context: context,
                                    id: id,
                                    fullText: issueModel.FullText(
                                        context: context,
                                        ss: ss,
                                        backgroundTask: true));
                                break;
                            default:
                                CreateIndexes(
                                    context: context,
                                    ss: ss,
                                    hash: issueModel.SearchIndexHash(
                                        context: context,
                                        ss: ss),
                                    id: id);
                                break;
                        }
                        break;
                    case "Results":
                        var resultModel = new ResultModel(
                            context: context,
                            ss: ss,
                            resultId: id);
                        ss.Links?
                            .Where(o => ss.GetColumn(
                                context: context,
                                columnName: o.ColumnName).UseSearch == true)
                            .ForEach(link =>
                                ss.SetChoiceHash(
                                    context: context,
                                    columnName: link.ColumnName,
                                    selectedValues: new List<string>
                                    {
                                        resultModel.PropertyValue(
                                            context: context, name: link.ColumnName)
                                    }));
                        switch (Parameters.Search.Provider)
                        {
                            case "FullText":
                                CreateFullText(
                                    context: context,
                                    id: id,
                                    fullText: resultModel.FullText(
                                        context: context,
                                        ss: ss,
                                        backgroundTask: true));
                                break;
                            default:
                                CreateIndexes(
                                    context: context,
                                    ss: ss,
                                    hash: resultModel.SearchIndexHash(
                                        context: context,
                                        ss: ss),
                                    id: id);
                                break;
                        }
                        break;
                    case "Wikis":
                        var wikiModel = new WikiModel(
                            context: context,
                            ss: ss,
                            wikiId: id);
                        switch (Parameters.Search.Provider)
                        {
                            case "FullText":
                                CreateFullText(
                                    context: context,
                                    id: id,
                                    fullText: wikiModel.FullText(
                                        context: context,
                                        ss: ss,
                                        backgroundTask: true));
                                break;
                            default:
                                CreateIndexes(
                                    context: context,
                                    ss: ss,
                                    hash: wikiModel.SearchIndexHash(
                                        context: context,
                                        ss: ss),
                                    id: id);
                                break;
                        }
                        break;
                }
            }
        }

        public static void CreateIndexes(
            Context context, SiteSettings ss, Dictionary<string, int> hash, long id)
        {
            if (ss.SiteId == id && ss.ReferenceType == "Wikis")
            {
                return;
            }
            var init = true;
            hash?.Buffer(2000).ForEach(data =>
            {
                try
                {
                    Rds.ExecuteNonQuery(
                        context: context,
                        transactional: true,
                        statements: Statements(data, id, init));
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    switch (e.Number)
                    {
                        case 2627: break;
                        default: new SysLogModel(context: context, e: e); break;
                    }
                }
                catch (Exception e)
                {
                    new SysLogModel(context: context, e: e);
                }
                init = false;
            });
        }

        private static SqlStatement[] Statements(
            IList<KeyValuePair<string, int>> searchIndexCollection, long id, bool init)
        {
            var statements = new List<SqlStatement>();
            if (init)
            {
                statements.Add(Rds.PhysicalDeleteSearchIndexes(
                    where: Rds.SearchIndexesWhere().ReferenceId(id)));
            }
            searchIndexCollection.ForEach(word =>
                statements.Add(Rds.InsertSearchIndexes(
                    param: Rds.SearchIndexesParam()
                        .Word(word.Key)
                        .ReferenceId(raw: id.ToString())
                        .Priority(raw: word.Value.ToString()))));
            return statements.ToArray();
        }

        private static void CreateFullText(Context context, long id, string fullText)
        {
            if (fullText != null)
            {
                Rds.ExecuteNonQuery(
                    context: context,
                    statements: Rds.UpdateItems(
                        where: Rds.ItemsWhere().ReferenceId(id),
                        param: Rds.ItemsParam()
                            .FullText(fullText)
                            .SearchIndexCreatedTime(DateTime.Now),
                        addUpdatorParam: false,
                        addUpdatedTimeParam: false));
            }
        }

        public static void Create(Context context, SiteSettings ss, SiteModel siteModel)
        {
            if (Parameters.Search.Provider.IsNullOrEmpty() && Parameters.Search.CreateIndexes)
            {
                Task.Run(() => CreateIndexes(
                    context: context,
                    ss: ss,
                    hash: siteModel.SearchIndexHash(
                        context: context,
                        ss: ss),
                    id: siteModel.SiteId));
            }
        }

        public static void Create(Context context, SiteSettings ss, IssueModel issueModel)
        {
            if (Parameters.Search.Provider.IsNullOrEmpty() && Parameters.Search.CreateIndexes)
            {
                Task.Run(() => CreateIndexes(
                    context: context,
                    ss: ss,
                    hash: issueModel.SearchIndexHash(
                        context: context,
                        ss: ss),
                    id: issueModel.IssueId));
            }
        }

        public static void Create(Context context, SiteSettings ss, ResultModel resultModel)
        {
            if (Parameters.Search.Provider.IsNullOrEmpty() && Parameters.Search.CreateIndexes)
            {
                Task.Run(() => CreateIndexes(
                    context: context,
                    ss: ss,
                    hash: resultModel.SearchIndexHash(
                        context: context,
                        ss: ss),
                    id: resultModel.ResultId));
            }
        }

        public static void Create(Context context, SiteSettings ss, WikiModel wikiModel)
        {
            if (Parameters.Search.Provider.IsNullOrEmpty() && Parameters.Search.CreateIndexes)
            {
                Task.Run(() => CreateIndexes(
                    context: context,
                    ss: ss,
                    hash: wikiModel.SearchIndexHash(
                        context: context,
                        ss: ss),
                    id: wikiModel.WikiId));
            }
        }
    }
}
