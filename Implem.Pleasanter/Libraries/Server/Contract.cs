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

        public static bool UsersLimit(int number = 1)
        {
            var tenantId = Sessions.TenantId();
            return
                ContractHash.ContainsKey(tenantId) &&
                ContractHash[tenantId]?.Users > 0 &&
                Rds.ExecuteScalar_int(statements: Rds.SelectUsers(
                    column: Rds.UsersColumn().UsersCount(),
                    where: Rds.UsersWhere().TenantId(tenantId))) + number >
                        ContractHash[tenantId]?.Users;
        }

        public static bool SitesLimit(int number = 1)
        {
            var tenantId = Sessions.TenantId();
            return
                ContractHash.ContainsKey(tenantId) &&
                ContractHash[tenantId]?.Sites > 0 &&
                Rds.ExecuteScalar_int(statements: Rds.SelectSites(
                    column: Rds.SitesColumn().SitesCount(),
                    where: Rds.SitesWhere().TenantId(tenantId))) + number >
                        ContractHash[tenantId]?.Sites;
        }

        public static bool ItemsLimit(long siteId, int number = 1)
        {
            var tenantId = Sessions.TenantId();
            return
                ContractHash.ContainsKey(tenantId) &&
                ContractHash[tenantId]?.Sites > 0 &&
                Rds.ExecuteScalar_int(statements: Rds.SelectItems(
                    column: Rds.ItemsColumn().ItemsCount(),
                    where: Rds.ItemsWhere().SiteId(siteId))) + number >
                        ContractHash[tenantId]?.Items;
        }
    }
}