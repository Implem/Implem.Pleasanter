using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.Server
{
    public static class Contract
    {
        public static Dictionary<int, ContractSettings> ContractHash =
            new Dictionary<int, ContractSettings>();

        public static void Set()
        {
            var tenantId = Sessions.TenantId();
            var cs = ContractSettings(tenantId);
            if (!ContractHash.ContainsKey(tenantId))
            {
                ContractHash.Add(tenantId, cs);
            }
            else
            {
                if (ContractHash[tenantId]?.UpdatedTime < cs?.UpdatedTime)
                {
                    ContractHash[tenantId] = cs;
                }
            }
        }

        private static ContractSettings ContractSettings(int tenantId)
        {
            return Rds.ExecuteScalar_string(statements:
                Rds.SelectTenants(
                    column: Rds.TenantsColumn().ContractSettings(),
                    where: Rds.TenantsWhere().TenantId(tenantId)))
                        .Deserialize<ContractSettings>();
        }
    }
}