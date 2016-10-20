using Implem.Libraries.DataSources.SqlServer;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Search
{
    public static class Indexes
    {
        public static void Create(long id)
        {
            var itemModel = new ItemModel(id);
            var siteModel = new SiteModel().Get(where: Rds.SitesWhere().SiteId(itemModel.SiteId));
            if (Exclude(itemModel, siteModel)) return;
            SearchIndexHash(siteModel, id, itemModel.ReferenceType)
                .Buffer(2000)
                .Select((o, i) => new { SearchIndexCollection = o, First = (i == 0) })
                .ForEach(data =>
                {
                    try
                    {
                        Rds.ExecuteNonQuery(statements:
                            Statements(data.SearchIndexCollection, id, data.First));
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

        private static bool Exclude(ItemModel itemModel, SiteModel siteModel)
        {
            switch (itemModel.ReferenceType)
            {
                case "Sites":
                    switch (siteModel.ReferenceType)
                    {
                        case "Wikis": return true;
                        default: return false;
                    }
                default: return false;
            }
        }

        private static SqlStatement[] Statements(
            IList<KeyValuePair<string, int>> searchIndexCollection, long id, bool first)
        {
            var statements = new List<SqlStatement>();
            if (first)
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

        private static Dictionary<string, int> SearchIndexHash(
            SiteModel siteModel, long id, string referenceType)
        {
            var siteSettings = SiteSettingsUtility.Get(siteModel);
            switch (referenceType)
            {
                case "Sites":
                    return new SiteModel().Get(where: Rds.SitesWhere().SiteId(id))
                        .SearchIndexHash();
                case "Issues":
                    return new IssueModel(siteSettings, id)
                        .SearchIndexHash();
                case "Results":
                    return new ResultModel(siteSettings, id)
                        .SearchIndexHash();
                case "Wikis":
                    return new WikiModel(siteSettings, id)
                        .SearchIndexHash();
                default: return null;
            }
        }
    }
}
