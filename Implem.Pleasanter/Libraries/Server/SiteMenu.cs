using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Responses;
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
            if (siteId == 0)
            {
                return null;
            }
            else
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
        }

        public IEnumerable<SiteMenuElement> Breadcrumb(long siteId)
        {
            var ret = new List<SiteMenuElement>();
            if (siteId != 0)
            {
                var current = Get(siteId);
                ret.Add(current);
                while (current.ParentId != 0)
                {
                    current = Get(current.ParentId);
                    ret.Add(current);
                }
                ret.Reverse();
            }
            return ret;
        }

        private bool HasAvailableCache(long siteId)
        {
            return
                ContainsKey(siteId) &&
                (BackgroundTasks.Enabled() ||
                (DateTime.Now - this[siteId].CreatedTime).Milliseconds <
                    Parameters.Cache.SiteMenuAvailableTime);
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