using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
using System.Linq;

namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptModelSiteSettings
    {
        private readonly Context Context;
        private readonly SiteSettings SiteSettings;
        public int? DefaultViewId { get; set; }
        public List<Section> Sections { get; set; }

        public ServerScriptModelSiteSettings(
            Context context,
            SiteSettings ss)
        {
            Context = context;
            SiteSettings = ss;
            DefaultViewId = ss?.GridView;
            Sections = ss?.Sections;
        }

        public long SiteId(string title)
        {
            var siteId = GetSiteId(title);
            if (siteId == 0)
            {
                return 0;
            }
            var ss = SiteSettingsUtilities.Get(
                context: Context,
                siteId: siteId);
            siteId = Context.HasPermission(ss: ss)
                ? siteId
                : 0;
            return siteId;
        }

        private long GetSiteId(string title)
        {
            var joinedSs = SiteSettings?.JoinedSsHash?.Values
                .FirstOrDefault(o => o.Title == title);
            if (joinedSs != null)
            {
                return joinedSs.SiteId;
            }
            return Rds.ExecuteScalar_long(
                context: Context,
                statements: Rds.SelectSites(
                    column: Rds.SitesColumn().SiteId(),
                    where: Rds.SitesWhere()
                        .TenantId(Context.TenantId)
                        .Title(title),
                    orderBy: Rds.SitesOrderBy().SiteId(),
                    top: 1));
        }
    }
}