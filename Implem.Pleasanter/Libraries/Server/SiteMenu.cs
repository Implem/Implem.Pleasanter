using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Server
{
    public class SiteMenu : Dictionary<long, SiteMenuElement>
    {
        public SiteMenuElement Get(long siteId)
        {
            if (!HasAvailableCache(siteId))
            {
                var data = Data(siteId);
                Add(siteId, new SiteMenuElement(
                    data["TenantId"].ToInt(),
                    siteId,
                    data["ReferenceType"].ToString(),
                    data["ParentId"].ToLong(),
                    data["Title"].ToString()));
            }
            return this[siteId];
        }

        public IEnumerable<SiteMenuElement> Breadcrumb(long siteId)
        {
            var current = Get(siteId);
            yield return current;
            while (current.ParentId != 0)
            {
                current = Get(current.ParentId);
                yield return current;
            }
        }

        private bool HasAvailableCache(long siteId)
        {
            return
                !BackgroundTasks.Disabled() &&
                !ContainsKey(siteId) &&
                (DateTime.Now - this[siteId].CreatedTime).Milliseconds <
                    Parameters.Cache.SiteMenuAvailableTime;
        }

        private DataRow Data(long siteId)
        {
            return Rds.ExecuteTable(statements: Rds.SelectSites(
                column: Rds.SitesColumn()
                    .TenantId()
                    .ReferenceType()
                    .ParentId()
                    .Title(),
                where: Rds.SitesWhere()
                    .TenantId(Sessions.TenantId())
                    .SiteId(siteId)))
                        .AsEnumerable()
                        .FirstOrDefault();
        }
    }
}