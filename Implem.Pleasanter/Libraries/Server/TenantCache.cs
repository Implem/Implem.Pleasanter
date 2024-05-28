using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using System.Collections.Generic;
using System.Data;
namespace Implem.Pleasanter.Libraries.Server
{
    public class TenantCache
    {
        public int TenantId;
        public string SitesUpdatedTime = Parameters.General.MinTime.ToString("yyyy/M/d H:m:s.fff");
        public Dictionary<long, DataRow> Sites;
        public LinkKeyValues Links;
        public SiteMenu SiteMenu;
        public Dictionary<int, Dept> DeptHash;
        public Dictionary<int, Group> GroupHash;
        public Dictionary<int, User> UserHash;
        public Dictionary<int, object> TenantHash;
        public Dictionary<long, List<int>> SiteDeptHash;
        public Dictionary<long, List<int>> SiteGroupHash;
        public Dictionary<long, List<int>> SiteUserHash;
        public SiteNameTree SiteNameTree;

        public UpdateMonitor UpdateMonitor;

        public TenantCache(Context context)
        {
            TenantId = context.TenantId;
            UpdateMonitor = new UpdateMonitor(context: context);
        }

        public UpdateMonitor GetUpdateMonitor(Context context)
        {
            UpdateMonitor.Monitor(context: context);
            return UpdateMonitor;
        }
    }
}