using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.Server
{
    public class TenantCache
    {
        public Context Context;
        public int TenantId;
        public SiteMenu SiteMenu;
        public Dictionary<int, Dept> DeptHash;
        public Dictionary<int, User> UserHash;
        public Dictionary<long, List<int>> SiteUserHash;
        public UpdateMonitor UpdateMonitor;

        public TenantCache(Context context)
        {
            Context = context;
            TenantId = context.TenantId;
            UpdateMonitor = new UpdateMonitor(context: context);
        }

        public UpdateMonitor GetUpdateMonitor()
        {
            UpdateMonitor.Monitor(context: Context);
            return UpdateMonitor;
        }
    }
}