using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Models
{
    public static class StatusUtilities
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public enum Types : int
        {
            AssemblyVersion = 100,
            DeptsUpdated = 210,
            GroupsUpdated = 220,
            UsersUpdated = 230,
            PermissionsUpdated = 240,
            SitesUpdated = 250
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static void Initialize(int tenantId)
        {
            if (tenantId != 0 && !MonitorInitialized(tenantId))
            {
                Rds.ExecuteNonQuery(statements: MonitorHash()
                    .Select(o => UpdateStatus(o.Key))
                    .ToArray());
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string AssemblyVersion()
        {
            return Rds.ExecuteScalar_string(statements:
                Rds.SelectStatuses(
                    column: Rds.StatusesColumn().Value(),
                    where: Rds.StatusesWhere()
                        .TenantId(0)
                        .StatusId(Types.AssemblyVersion)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static void UpdateAssemblyVersion(string version)
        {
            if (Rds.ExecuteScalar_int(statements: Rds.SelectStatuses(
                column: Rds.StatusesColumn().StatusesCount(),
                where: Rds.StatusesWhere()
                    .TenantId(0)
                    .StatusId(Types.AssemblyVersion))) == 0)
            {
                Rds.ExecuteNonQuery(statements: Rds.InsertStatuses(
                    param: Rds.StatusesParam()
                        .TenantId(0)
                        .StatusId(Types.AssemblyVersion)
                        .Value(version)));
            }
            else
            {
                Rds.ExecuteNonQuery(statements: Rds.UpdateStatuses(
                    where: Rds.StatusesWhere()
                        .TenantId(0)
                        .StatusId(Types.AssemblyVersion)
                        .Value(version, _operator: "<>"),
                    param: Rds.StatusesParam().Value(version)));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Dictionary<Types, DateTime> Monitors(int tenantId)
        {
            var hash = MonitorHash();
            MonitorDataRows(tenantId, hash).ForEach(dataRow =>
            {
                var type = (Types)dataRow["StatusId"].ToInt();
                if (hash.ContainsKey(type))
                {
                    hash[type] = dataRow["UpdatedTime"].ToDateTime();
                }
            });
            return hash;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static bool MonitorInitialized(int tenantId)
        {
            var hash = MonitorHash();
            return MonitorDataRows(tenantId, hash).Count() == hash.Count;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static EnumerableRowCollection<DataRow> MonitorDataRows(
            int tenantId, Dictionary<Types, DateTime> hash = null)
        {
            hash = hash ?? MonitorHash();
            return Rds.ExecuteTable(statements: Rds.SelectStatuses(
                column: Rds.StatusesColumn()
                    .TenantId()
                    .StatusId()
                    .UpdatedTime(),
                where: Rds.StatusesWhere()
                    .TenantId(tenantId)
                    .StatusId_In(hash.Keys.Select(o => o.ToInt()))))
                        .AsEnumerable();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Dictionary<Types, DateTime> MonitorHash()
        {
            var now = DateTime.Now;
            return new Dictionary<Types, DateTime>
            {
                { Types.DeptsUpdated, now },
                { Types.GroupsUpdated, now },
                { Types.UsersUpdated, now },
                { Types.PermissionsUpdated, now },
                { Types.SitesUpdated, now }
            };
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static SqlUpdateOrInsert UpdateStatus(Types type, string value = "")
        {
            var tenantId = Sessions.TenantId();
            return Rds.UpdateOrInsertStatuses(
                where: Rds.StatusesWhere()
                    .TenantId(tenantId)
                    .StatusId(type),
                param: Rds.StatusesParam()
                    .TenantId(tenantId)
                    .StatusId(type)
                    .Value(value));
        }
    }
}
