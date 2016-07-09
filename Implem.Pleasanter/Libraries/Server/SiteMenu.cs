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
                    Set(siteId);
                }
                return this[siteId];
            }
        }

        public void Set(long siteId)
        {
            var dataRow = SiteMenuElementDataRow(siteId);
            var siteMenuElement = new SiteMenuElement(
                dataRow["TenantId"].ToInt(),
                siteId,
                dataRow["ReferenceType"].ToString(),
                dataRow["ParentId"].ToLong(),
                dataRow["Title"].ToString());
            if (ContainsKey(siteId))
            {
                this[siteId] = siteMenuElement;
            }
            else
            {
                Add(siteId, siteMenuElement);
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

        private DataRow SiteMenuElementDataRow(long siteId)
        {
            var tenantId = Sessions.TenantId();
            return Rds.ExecuteTable(statements: Rds.SelectSites(
                column: Rds.SitesColumn()
                    .TenantId()
                    .ReferenceType()
                    .ParentId()
                    .Title(),
                where: Rds.SitesWhere()
                    .TenantId(tenantId, _using: tenantId != 0)
                    .SiteId(siteId)))
                        .AsEnumerable()
                        .FirstOrDefault();
        }
    }
}