using Implem.Libraries.DataSources.SqlServer;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
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
            SearchIndexHash(siteModel.SiteSettings, id, itemModel.ReferenceType)
                .Buffer(2000)
                .Select((o, i) => new { SearchIndexCollection = o, First = (i == 0) })
                .ForEach(data =>
                {
                    try
                    {
                        Rds.ExecuteNonQuery(statements:
                            Statements(data.SearchIndexCollection, id, data.First));
                    }
                    catch (Exception e) { new SysLogModel(e); }
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
            SiteSettings siteSettings, long id, string referenceType)
        {
            switch (referenceType)
            {
                case "Sites": return new SiteSubset(
                    new SiteModel().Get(where: Rds.SitesWhere().SiteId(id)),
                    siteSettings)
                        .SearchIndexCollection();
                case "Issues": return new IssueSubset(
                    new IssueModel(siteSettings, Permissions.Admins(), id),
                    siteSettings)
                        .SearchIndexCollection();
                case "Results": return new ResultSubset(
                    new ResultModel(siteSettings, Permissions.Admins(), id),
                    siteSettings)
                        .SearchIndexCollection();
                case "Wikis": return new WikiSubset(
                    new WikiModel(siteSettings, Permissions.Admins(), id),
                    siteSettings)
                        .SearchIndexCollection();
                default: return null;
            }
        }
    }
}
