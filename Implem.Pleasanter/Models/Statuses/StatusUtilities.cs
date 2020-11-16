using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.Interfaces;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Resources;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
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
        public static void Initialize(Context context)
        {
            if (context.TenantId != 0 && !MonitorInitialized(context: context))
            {
                Repository.ExecuteNonQuery(
                    context: context,
                    statements: MonitorHash()
                        .Select(o => UpdateStatus(context.TenantId, o.Key))
                        .ToArray());
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string AssemblyVersion(Context context)
        {
            return Repository.ExecuteScalar_string(
                context: context,
                statements: Rds.SelectStatuses(
                    column: Rds.StatusesColumn().Value(),
                    where: Rds.StatusesWhere()
                        .TenantId(0)
                        .StatusId(Types.AssemblyVersion)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static void UpdateAssemblyVersion(Context context, string version)
        {
            if (Repository.ExecuteScalar_int(
                context: context,
                statements: Rds.SelectStatuses(
                    column: Rds.StatusesColumn().StatusesCount(),
                    where: Rds.StatusesWhere()
                        .TenantId(0)
                        .StatusId(Types.AssemblyVersion))) == 0)
            {
                Repository.ExecuteNonQuery(
                    context: context,
                    statements: Rds.InsertStatuses(
                        param: Rds.StatusesParam()
                            .TenantId(0)
                            .StatusId(Types.AssemblyVersion)
                            .Value(version)));
            }
            else
            {
                Repository.ExecuteNonQuery(
                    context: context,
                    statements: Rds.UpdateStatuses(
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
        public static Dictionary<Types, DateTime> Monitors(Context context)
        {
            var hash = MonitorHash();
            MonitorDataRows(
                context: context,
                hash: hash).ForEach(dataRow =>
                {
                    var type = (Types)dataRow.Int("StatusId");
                    if (hash.ContainsKey(type))
                    {
                        hash[type] = dataRow.DateTime("UpdatedTime");
                    }
                });
            return hash;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static bool MonitorInitialized(Context context)
        {
            var hash = MonitorHash();
            return MonitorDataRows(
                context: context,
                hash: hash).Count() == hash.Count;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static EnumerableRowCollection<DataRow> MonitorDataRows(
            Context context, Dictionary<Types, DateTime> hash = null)
        {
            hash = hash ?? MonitorHash();
            return Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectStatuses(
                    column: Rds.StatusesColumn()
                        .TenantId()
                        .StatusId()
                        .UpdatedTime(),
                    where: Rds.StatusesWhere()
                        .TenantId(context.TenantId)
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
        public static SqlUpdateOrInsert UpdateStatus(int tenantId, Types type, string value = "")
        {
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
