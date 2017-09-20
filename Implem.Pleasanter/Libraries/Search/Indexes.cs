using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
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
        public static void Create(SiteSettings ss, long id, bool force = false)
        {
            if (Parameters.Search.Provider.IsNullOrEmpty() && Parameters.Search.CreateIndexes)
            {
                var itemModel = new ItemModel(id);
                switch (itemModel.ReferenceType)
                {
                    case "Sites":
                        var siteModel = new SiteModel(id);
                        Task.Run(() => CreateIndexes(
                            ss,
                            siteModel.SearchIndexHash(ss),
                            id));
                        break;
                    case "Issues":
                        var issueModel = new IssueModel(ss, id);
                        Task.Run(() => CreateIndexes(
                            ss,
                            issueModel.SearchIndexHash(ss),
                            id));
                        break;
                    case "Results":
                        var resultModel = new ResultModel(ss, id);
                        Task.Run(() => CreateIndexes(
                            ss,
                            resultModel.SearchIndexHash(ss),
                            id));
                        break;
                    case "Wikis":
                        var wikiModel = new WikiModel(ss, id);
                        Task.Run(() => CreateIndexes(
                            ss,
                            wikiModel.SearchIndexHash(ss),
                            id));
                        break;
                }
            }
            else if (force)
            {
                var itemModel = new ItemModel(id);
                switch (itemModel.ReferenceType)
                {
                    case "Sites":
                        var siteModel = new SiteModel(id);
                        switch (Parameters.Search.Provider)
                        {
                            case "FullText":
                                CreateFullText(
                                    id,
                                    siteModel.FullText(ss, backgroundTask: true));
                                break;
                            default:
                                CreateIndexes(
                                    ss,
                                    siteModel.SearchIndexHash(ss),
                                    id);
                                break;
                        }
                        break;
                    case "Issues":
                        var issueModel = new IssueModel(ss, id);
                        ss.Links?
                            .Where(o => ss.GetColumn(o.ColumnName).UseSearch == true)
                            .ForEach(link =>
                                ss.SetChoiceHash(
                                    columnName: link.ColumnName,
                                    selectedValues: new List<string>
                                    {
                                        issueModel.PropertyValue(link.ColumnName)
                                    }));
                        switch (Parameters.Search.Provider)
                        {
                            case "FullText":
                                CreateFullText(
                                    id,
                                    issueModel.FullText(ss, backgroundTask: true));
                                break;
                            default:
                                CreateIndexes(
                                    ss,
                                    issueModel.SearchIndexHash(ss),
                                    id);
                                break;
                        }
                        break;
                    case "Results":
                        var resultModel = new ResultModel(ss, id);
                        ss.Links?
                            .Where(o => ss.GetColumn(o.ColumnName).UseSearch == true)
                            .ForEach(link =>
                                ss.SetChoiceHash(
                                    columnName: link.ColumnName,
                                    selectedValues: new List<string>
                                    {
                                        resultModel.PropertyValue(link.ColumnName)
                                    }));
                        switch (Parameters.Search.Provider)
                        {
                            case "FullText":
                                CreateFullText(
                                    id,
                                    resultModel.FullText(ss, backgroundTask: true));
                                break;
                            default:
                                CreateIndexes(
                                    ss,
                                    resultModel.SearchIndexHash(ss),
                                    id);
                                break;
                        }
                        break;
                    case "Wikis":
                        var wikiModel = new WikiModel(ss, id);
                        switch (Parameters.Search.Provider)
                        {
                            case "FullText":
                                CreateFullText(
                                    id,
                                    wikiModel.FullText(ss, backgroundTask: true));
                                break;
                            default:
                                CreateIndexes(
                                    ss,
                                    wikiModel.SearchIndexHash(ss),
                                    id);
                                break;
                        }
                        break;
                }
            }
        }

        public static void Create(SiteSettings ss, SiteModel siteModel)
        {
            if (Parameters.Search.Provider.IsNullOrEmpty() && Parameters.Search.CreateIndexes)
            {
                Task.Run(() => CreateIndexes(
                    ss,
                    siteModel.SearchIndexHash(ss),
                    siteModel.SiteId));
            }
        }

        public static void Create(SiteSettings ss, IssueModel issueModel)
        {
            if (Parameters.Search.Provider.IsNullOrEmpty() && Parameters.Search.CreateIndexes)
            {
                Task.Run(() => CreateIndexes(
                    ss,
                    issueModel.SearchIndexHash(ss),
                    issueModel.IssueId));
            }
        }

        public static void Create(SiteSettings ss, ResultModel resultModel)
        {
            if (Parameters.Search.Provider.IsNullOrEmpty() && Parameters.Search.CreateIndexes)
            {
                Task.Run(() => CreateIndexes(
                    ss,
                    resultModel.SearchIndexHash(ss),
                    resultModel.ResultId));
            }
        }

        public static void Create(SiteSettings ss, WikiModel wikiModel)
        {
            if (Parameters.Search.Provider.IsNullOrEmpty() && Parameters.Search.CreateIndexes)
            {
                Task.Run(() => CreateIndexes(
                    ss,
                    wikiModel.SearchIndexHash(ss),
                    wikiModel.WikiId));
            }
        }

        public static void CreateIndexes(SiteSettings ss, Dictionary<string, int> hash, long id)
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
                        transactional: true,
                        statements: Statements(data, id, init));
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    switch (e.Number)
                    {
                        case 2627: break;
                        default: new SysLogModel(e); break;
                    }
                }
                catch (Exception e)
                {
                    new SysLogModel(e);
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

        private static void CreateFullText(long id, string fullText)
        {
            if (fullText != null)
            {
                Rds.ExecuteNonQuery(statements:
                    Rds.UpdateItems(
                        where: Rds.ItemsWhere().ReferenceId(id),
                        param: Rds.ItemsParam()
                            .FullText(fullText)
                            .SearchIndexCreatedTime(DateTime.Now),
                        addUpdatorParam: false,
                        addUpdatedTimeParam: false));
            }
        }
    }
}
