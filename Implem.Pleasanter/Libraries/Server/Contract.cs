using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Server
{
    public static class Contract
    {
        public static Dictionary<int, ContractSettings> ContractHash =
            new Dictionary<int, ContractSettings>();

        public static void Set(Context context)
        {
            var cs = ContractSettings(context: context);
            if (!ContractHash.ContainsKey(context.TenantId))
            {
                ContractHash.Add(context.TenantId, cs);
            }
            else
            {
                ContractHash[context.TenantId] = cs;
            }
        }

        private static ContractSettings ContractSettings(Context context)
        {
            var dataRow = Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectTenants(
                    column: Rds.TenantsColumn()
                        .ContractSettings()
                        .ContractDeadline(),
                    where: Rds.TenantsWhere().TenantId(context.TenantId)))
                        .AsEnumerable()
                        .FirstOrDefault();
            var cs = dataRow?.String("ContractSettings").Deserialize<ContractSettings>();
            if (cs != null)
            {
                cs.Deadline = dataRow.DateTime("ContractDeadline");
            }
            return cs;
        }

        public static string DisplayName(Context context)
        {
            return
                ContractHash.ContainsKey(context.TenantId)
                    ? ContractHash.Get(context.TenantId)?.DisplayName
                    : null;
        }

        public static bool OverDeadline(Context context)
        {
            return
                ContractHash.ContainsKey(context.TenantId) &&
                ContractHash.Get(context.TenantId)?.Deadline.InRange() == true &&
                ContractHash.Get(context.TenantId)?.Deadline.ToDateTime() < DateTime.Now.ToLocal();
        }

        public static bool UsersLimit(Context context, int number = 1)
        {
            return
                ContractHash.ContainsKey(context.TenantId) &&
                ContractHash.Get(context.TenantId)?.Users > 0 &&
                Rds.ExecuteScalar_int(
                    context: context,
                    statements: Rds.SelectUsers(
                    column: Rds.UsersColumn().UsersCount(),
                        where: Rds.UsersWhere().TenantId(context.TenantId))) + number >
                            ContractHash.Get(context.TenantId)?.Users;
        }

        public static bool SitesLimit(Context context, int number = 1)
        {
            return
                ContractHash.ContainsKey(context.TenantId) &&
                ContractHash.Get(context.TenantId)?.Sites > 0 &&
                Rds.ExecuteScalar_int(
                    context: context,
                    statements: Rds.SelectSites(
                        column: Rds.SitesColumn().SitesCount(),
                        where: Rds.SitesWhere().TenantId(context.TenantId))) + number >
                            ContractHash.Get(context.TenantId)?.Sites;
        }

        public static bool ItemsLimit(Context context, long siteId, int number = 1)
        {
            return
                ContractHash.ContainsKey(context.TenantId) &&
                ContractHash.Get(context.TenantId)?.Sites > 0 &&
                Rds.ExecuteScalar_int(
                    context: context,
                    statements: Rds.SelectItems(
                        column: Rds.ItemsColumn().ItemsCount(),
                        where: Rds.ItemsWhere().SiteId(siteId))) + number >
                            ContractHash.Get(context.TenantId)?.Items;
        }

        public static bool Attachments(Context context)
        {
            return
                Parameters.BinaryStorage.Attachments &&
                ContractHash.ContainsKey(context.TenantId) &&
                ContractHash.Get(context.TenantId)?.StorageSize != 0;
        }

        public static bool Images(Context context)
        {
            return Parameters.BinaryStorage.Images
                && Attachments(context);
        }

        public static decimal? TenantStorageSize(Context context)
        {
            return ContractHash.Get(context.TenantId)?.StorageSize;
        }

        public static bool Import(Context context)
        {
            return ContractHash.ContainsKey(context.TenantId)
                && ContractHash.Get(context.TenantId)?.Import != false;
        }

        public static bool Export(Context context)
        {
            return ContractHash.ContainsKey(context.TenantId)
                && ContractHash.Get(context.TenantId)?.Export != false;
        }

        public static bool Notice(Context context)
        {
            return ContractHash.ContainsKey(context.TenantId)
                && ContractHash.Get(context.TenantId)?.Notice != false;
        }

        public static bool Remind(Context context)
        {
            return ContractHash.ContainsKey(context.TenantId) 
                && ContractHash.Get(context.TenantId)?.Remind != false;
        }

        public static bool Mail(Context context)
        {
            return ContractHash.ContainsKey(context.TenantId)
                && ContractHash.Get(context.TenantId)?.Mail != false;
        }

        public static bool Style(Context context)
        {
            return ContractHash.ContainsKey(context.TenantId)
                && ContractHash.Get(context.TenantId)?.Style != false;
        }

        public static bool Script(Context context)
        {
            return ContractHash.ContainsKey(context.TenantId)
                && ContractHash.Get(context.TenantId)?.Script != false;
        }

        public static bool Api(Context context)
        {
            return Parameters.Api.Enabled
                && ContractHash.ContainsKey(context.TenantId)
                && ContractHash.Get(context.TenantId)?.Api != false;
        }
    }
}