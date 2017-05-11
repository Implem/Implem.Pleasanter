using Implem.Pleasanter.Libraries.DataTypes;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.Server
{
    public class TenantCache
    {
        public int TenantId;
        public SiteMenu SiteMenu;
        public Dictionary<int, Dept> DeptHash;
        public Dictionary<int, User> UserHash;
        public Dictionary<long, List<int>> SiteUserHash;
        public UpdateMonitor UpdateMonitor;

        public TenantCache(int tenantId)
        {
            TenantId = tenantId;
            UpdateMonitor = new UpdateMonitor(tenantId);
        }

        public UpdateMonitor GetUpdateMonitor()
        {
            UpdateMonitor.Monitor(TenantId);
            return UpdateMonitor;
        }
    }
}