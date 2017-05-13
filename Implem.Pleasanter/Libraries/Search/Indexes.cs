using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
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
        public static void Create(
            SiteSettings ss, long id, bool async = true, bool backgroundTask = false)
        {
            if (Parameters.Service.CreateIndexes || backgroundTask)
            {
                var itemModel = new ItemModel(id);
                if (async)
                {
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
                else
                {
                    switch (itemModel.ReferenceType)
                    {
                        case "Sites":
                            var siteModel = new SiteModel(id);
                            CreateIndexes(
                                ss,
                                siteModel.SearchIndexHash(ss),
                                id);
                            break;
                        case "Issues":
                            var issueModel = new IssueModel(ss, id);
                            CreateIndexes(
                                ss,
                                issueModel.SearchIndexHash(ss),
                                id);
                            break;
                        case "Results":
                            var resultModel = new ResultModel(ss, id);
                            CreateIndexes(
                                ss,
                                resultModel.SearchIndexHash(ss),
                                id);
                            break;
                        case "Wikis":
                            var wikiModel = new WikiModel(ss, id);
                            CreateIndexes(
                                ss,
                                wikiModel.SearchIndexHash(ss),
                                id);
                            break;
                    }
                }
            }
        }

        public static void Create(SiteSettings ss, SiteModel siteModel)
        {
            if (Parameters.Service.CreateIndexes)
            {
                Task.Run(() => CreateIndexes(
                    ss,
                    siteModel.SearchIndexHash(ss),
                    siteModel.SiteId));
            }
        }

        public static void Create(SiteSettings ss, IssueModel issueModel)
        {
            if (Parameters.Service.CreateIndexes)
            {
                Task.Run(() => CreateIndexes(
                    ss,
                    issueModel.SearchIndexHash(ss),
                    issueModel.IssueId));
            }
        }

        public static void Create(SiteSettings ss, ResultModel resultModel)
        {
            if (Parameters.Service.CreateIndexes)
            {
                Task.Run(() => CreateIndexes(
                    ss,
                    resultModel.SearchIndexHash(ss),
                    resultModel.ResultId));
            }
        }

        public static void Create(SiteSettings ss, WikiModel wikiModel)
        {
            if (Parameters.Service.CreateIndexes)
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
            Rds.ExecuteNonQuery(statements: Rds.PhysicalDeleteSearchIndexes(
                where: Rds.SearchIndexesWhere().ReferenceId(id)));
            hash?.Buffer(2000).ForEach(data =>
            {
                try
                {
                    Rds.ExecuteNonQuery(statements: Statements(data, id));
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
            });
        }

        private static SqlStatement[] Statements(
            IList<KeyValuePair<string, int>> searchIndexCollection, long id)
        {
            var statements = new List<SqlStatement>();
            searchIndexCollection.ForEach(word =>
                statements.Add(Rds.InsertSearchIndexes(
                    param: Rds.SearchIndexesParam()
                        .Word(word.Key)
                        .ReferenceId(raw: id.ToString())
                        .Priority(raw: word.Value.ToString()))));
            return statements.ToArray();
        }
    }
}
