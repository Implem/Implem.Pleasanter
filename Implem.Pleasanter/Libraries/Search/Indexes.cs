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
        public static void Create(SiteSettings ss, long id)
        {
            if (Parameters.Service.CreateIndexes)
            {
                Task.Run(() => CreateIndexes(ss, id));
            }
        }

        public static void CreateIndexes(SiteSettings ss, long id)
        {
            var hash = SearchIndexHash(ss, id);
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

        private static Dictionary<string, int> SearchIndexHash(SiteSettings ss, long id)
        {
            var referenceType = ss.ReferenceType;
            if (ss.SiteId == id)
            {
                referenceType = "Sites";
                if (ss.ReferenceType == "Wikis") return null;
            }
            switch (referenceType)
            {
                case "Sites":
                    return new SiteModel().Get(where: Rds.SitesWhere().SiteId(id))
                        .SearchIndexHash(ss);
                case "Issues":
                    return new IssueModel(ss, id).SearchIndexHash(ss);
                case "Results":
                    return new ResultModel(ss, id).SearchIndexHash(ss);
                case "Wikis":
                    return new WikiModel(ss, id).SearchIndexHash(ss);
                default: return null;
            }
        }
    }
}
