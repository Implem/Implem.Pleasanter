using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Implem.Pleasanter.Models
{
    public static class TenantDeleteService
    {
        public static void Execute(Context context)
        {
            var tenantIds = GetTargetTenantIds(context: context);
            foreach (var tenantId in tenantIds)
            {
                if (Parameters.MultiTenant.IsProtectedTenant(tenantId))
                {
                    new SysLogModel(
                        context: context,
                        method: nameof(Execute),
                        message: $"TenantDeleteService: skipped protected tenant TenantId={tenantId}",
                        sysLogType: SysLogModel.SysLogTypes.Warning);
                    continue;
                }
                var log = new SysLogModel(
                    context: context,
                    method: nameof(Execute),
                    message: $"TenantDeleteService TenantId={tenantId}");
                try
                {
                    PhysicalDelete(
                        context: context,
                        tenantId: tenantId);
                }
                catch (Exception e)
                {
                    new SysLogModel(
                        context: context,
                        e: e,
                        extendedErrorMessage: $"TenantDeleteService TenantId={tenantId}");
                }
                finally
                {
                    log.Finish(context: context);
                }
            }
        }

        private static List<int> GetTargetTenantIds(Context context)
        {
            return Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectTenants(
                    column: Rds.TenantsColumn().TenantId(),
                    where: Rds.TenantsWhere()
                        .DeleteRequestTime(
                            DateTime.Now.AddDays(
                                Parameters.BackgroundService.DeleteTenantRetentionPeriod * -1),
                            _operator: "<")))
                .AsEnumerable()
                .Select(o => o.Int("TenantId"))
                .ToList();
        }

        private static void PhysicalDelete(Context context, int tenantId)
        {
            DeleteByReferenceId(context: context, tenantId: tenantId);
            DeleteByOwnerId(context: context, tenantId: tenantId);
            DeleteByGroupId(context: context, tenantId: tenantId);
            DeleteByUserId(context: context, tenantId: tenantId);
            DeleteBySiteId(context: context, tenantId: tenantId);
            DeleteBinariesLocalFiles(context: context, tenantId: tenantId);
            DeleteByTenantId(context: context, tenantId: tenantId);
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.PhysicalDeleteTenants(
                    where: Rds.TenantsWhere().TenantId(tenantId)));
            SiteInfo.TenantCaches.TryRemove(tenantId, out _);
        }

        private static SqlStatement SitesSub(int tenantId)
        {
            return Rds.SelectSites(
                column: Rds.SitesColumn().SiteId(),
                where: Rds.SitesWhere().TenantId(tenantId));
        }

        private static SqlStatement ItemsSub(int tenantId)
        {
            return Rds.SelectItems(
                column: Rds.ItemsColumn().ReferenceId(),
                where: Rds.ItemsWhere().SiteId_In(sub: SitesSub(tenantId: tenantId)));
        }

        private static SqlStatement UsersSub(int tenantId)
        {
            return Rds.SelectUsers(
                column: Rds.UsersColumn().UserId(),
                where: Rds.UsersWhere().TenantId(tenantId));
        }

        private static SqlStatement GroupsSub(int tenantId)
        {
            return Rds.SelectGroups(
                column: Rds.GroupsColumn().GroupId(),
                where: Rds.GroupsWhere().TenantId(tenantId));
        }

        private static void DeleteByReferenceId(Context context, int tenantId)
        {
            foreach (var tableType in AllTableTypes())
            {
                Repository.ExecuteNonQuery(
                    context: context,
                    statements: new SqlStatement[]
                    {
                        Rds.PhysicalDeleteOrders(
                            tableType: tableType,
                            where: Rds.OrdersWhere()
                                .ReferenceId(
                                    sub: ItemsSub(tenantId: tenantId),
                                    _operator: " in ")),
                        Rds.PhysicalDeleteOutgoingMails(
                            tableType: tableType,
                            where: Rds.OutgoingMailsWhere()
                                .ReferenceId(
                                    sub: ItemsSub(tenantId: tenantId),
                                    _operator: " in ")),
                        Rds.PhysicalDeletePermissions(
                            tableType: tableType,
                            where: Rds.PermissionsWhere()
                                .ReferenceId(
                                    sub: ItemsSub(tenantId: tenantId),
                                    _operator: " in ")),
                        Rds.PhysicalDeleteLinks(
                            tableType: tableType,
                            where: Rds.LinksWhere()
                                .DestinationId_In(sub: ItemsSub(tenantId: tenantId))),
                        Rds.PhysicalDeleteLinks(
                            tableType: tableType,
                            where: Rds.LinksWhere()
                                .SourceId_In(sub: ItemsSub(tenantId: tenantId))),
                    });
            }
        }

        private static void DeleteByOwnerId(Context context, int tenantId)
        {
            foreach (var tableType in AllTableTypes())
            {
                Repository.ExecuteNonQuery(
                    context: context,
                    statements: Rds.PhysicalDeleteMailAddresses(
                        tableType: tableType,
                        where: Rds.MailAddressesWhere()
                            .OwnerId(
                                sub: UsersSub(tenantId: tenantId),
                                _operator: " in ")));
            }
        }

        private static void DeleteByGroupId(Context context, int tenantId)
        {
            foreach (var tableType in AllTableTypes())
            {
                Repository.ExecuteNonQuery(
                    context: context,
                    statements: new SqlStatement[]
                    {
                        Rds.PhysicalDeleteGroupMembers(
                            tableType: tableType,
                            where: Rds.GroupMembersWhere()
                                .GroupId(
                                    sub: GroupsSub(tenantId: tenantId),
                                    _operator: " in ")),
                        Rds.PhysicalDeleteGroupChildren(
                            tableType: tableType,
                            where: Rds.GroupChildrenWhere()
                                .GroupId(
                                    sub: GroupsSub(tenantId: tenantId),
                                    _operator: " in ")),
                    });
            }
        }

        private static void DeleteByUserId(Context context, int tenantId)
        {
            foreach (var tableType in AllTableTypes())
            {
                Repository.ExecuteNonQuery(
                    context: context,
                    statements: new SqlStatement[]
                    {
                        Rds.PhysicalDeletePasskeys(
                            tableType: tableType,
                            where: Rds.PasskeysWhere()
                                .UserId(
                                    sub: UsersSub(tenantId: tenantId),
                                    _operator: " in ")),
                        Rds.PhysicalDeleteLoginKeys(
                            tableType: tableType,
                            where: Rds.LoginKeysWhere()
                                .UserId(
                                    sub: UsersSub(tenantId: tenantId),
                                    _operator: " in ")),
                    });
            }
        }

        private static void DeleteBySiteId(Context context, int tenantId)
        {
            var chunkSize = Parameters.BackgroundService.DeleteTenantChunkSize;
            foreach (var tableType in AllTableTypes())
            {
                Repository.ExecuteNonQuery(
                    context: context,
                    statements: new SqlStatement[]
                    {
                        Rds.PhysicalDeleteDashboards(
                            tableType: tableType,
                            where: Rds.DashboardsWhere()
                                .SiteId_In(sub: SitesSub(tenantId: tenantId))),
                        Rds.PhysicalDeleteItems(
                            tableType: tableType,
                            where: Rds.ItemsWhere()
                                .SiteId_In(sub: SitesSub(tenantId: tenantId))),
                        Rds.PhysicalDeleteReminderSchedules(
                            tableType: tableType,
                            where: Rds.ReminderSchedulesWhere()
                                .SiteId_In(sub: SitesSub(tenantId: tenantId))),
                    });
                DeleteIssuesInChunks(
                    context: context,
                    tenantId: tenantId,
                    tableType: tableType,
                    chunkSize: chunkSize);
                DeleteResultsInChunks(
                    context: context,
                    tenantId: tenantId,
                    tableType: tableType,
                    chunkSize: chunkSize);
                DeleteWikisInChunks(
                    context: context,
                    tenantId: tenantId,
                    tableType: tableType,
                    chunkSize: chunkSize);
            }
        }

        private static void DeleteIssuesInChunks(
            Context context,
            int tenantId,
            Sqls.TableTypes tableType,
            int chunkSize)
        {
            var table = Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectIssues(
                    tableType: tableType,
                    column: Rds.IssuesColumn()
                        .Issues_IssueId(function: Sqls.Functions.Min, _as: "Min")
                        .Issues_IssueId(function: Sqls.Functions.Max, _as: "Max"),
                    where: Rds.IssuesWhere()
                        .SiteId_In(sub: SitesSub(tenantId: tenantId))));
            var min = table.Rows.Count > 0 ? table.Rows[0]["Min"].ToLong() : 0L;
            var max = table.Rows.Count > 0 ? table.Rows[0]["Max"].ToLong() : 0L;
            if (min == 0 && max == 0) return;
            if (chunkSize <= 0) chunkSize = 1000;
            for (var i = min; i <= max; i += chunkSize)
            {
                Repository.ExecuteNonQuery(
                    context: context,
                    statements: Rds.PhysicalDeleteIssues(
                        tableType: tableType,
                        where: Rds.IssuesWhere()
                            .SiteId_In(sub: SitesSub(tenantId: tenantId))
                            .IssueId_Between(
                                begin: i,
                                end: i + chunkSize - 1)));
            }
        }

        private static void DeleteResultsInChunks(
            Context context,
            int tenantId,
            Sqls.TableTypes tableType,
            int chunkSize)
        {
            var resultTable = Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectResults(
                    tableType: tableType,
                    column: Rds.ResultsColumn()
                        .Results_ResultId(function: Sqls.Functions.Min, _as: "Min")
                        .Results_ResultId(function: Sqls.Functions.Max, _as: "Max"),
                    where: Rds.ResultsWhere()
                        .SiteId_In(sub: SitesSub(tenantId: tenantId))));
            var min = resultTable.Rows.Count > 0 ? resultTable.Rows[0]["Min"].ToLong() : 0L;
            var max = resultTable.Rows.Count > 0 ? resultTable.Rows[0]["Max"].ToLong() : 0L;
            if (min == 0 && max == 0) return;
            if (chunkSize <= 0) chunkSize = 1000;
            for (var i = min; i <= max; i += chunkSize)
            {
                Repository.ExecuteNonQuery(
                    context: context,
                    statements: Rds.PhysicalDeleteResults(
                        tableType: tableType,
                        where: Rds.ResultsWhere()
                            .SiteId_In(sub: SitesSub(tenantId: tenantId))
                            .ResultId_Between(
                                begin: i,
                                end: i + chunkSize - 1)));
            }
        }

        private static void DeleteWikisInChunks(
            Context context,
            int tenantId,
            Sqls.TableTypes tableType,
            int chunkSize)
        {
            var wikiTable = Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectWikis(
                    tableType: tableType,
                    column: Rds.WikisColumn()
                        .Wikis_WikiId(function: Sqls.Functions.Min, _as: "Min")
                        .Wikis_WikiId(function: Sqls.Functions.Max, _as: "Max"),
                    where: Rds.WikisWhere()
                        .SiteId_In(sub: SitesSub(tenantId: tenantId))));
            var min = wikiTable.Rows.Count > 0 ? wikiTable.Rows[0]["Min"].ToLong() : 0L;
            var max = wikiTable.Rows.Count > 0 ? wikiTable.Rows[0]["Max"].ToLong() : 0L;
            if (min == 0 && max == 0) return;
            if (chunkSize <= 0) chunkSize = 1000;
            for (var i = min; i <= max; i += chunkSize)
            {
                Repository.ExecuteNonQuery(
                    context: context,
                    statements: Rds.PhysicalDeleteWikis(
                        tableType: tableType,
                        where: Rds.WikisWhere()
                            .SiteId_In(sub: SitesSub(tenantId: tenantId))
                            .WikiId_Between(
                                begin: i,
                                end: i + chunkSize - 1)));
            }
        }

        private static void DeleteBinariesLocalFiles(Context context, int tenantId)
        {
            Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectBinaries(
                    column: Rds.BinariesColumn()
                        .Guid()
                        .BinaryType(),
                    where: Rds.BinariesWhere()
                        .TenantId(tenantId)
                        .Add(raw: "\"Bin\" is null")))
                .AsEnumerable()
                .ForEach(binary => DeleteLocalFile(
                    binaryType: binary.String("BinaryType"),
                    guid: binary.String("Guid")));
        }

        private static void DeleteLocalFile(string binaryType, string guid)
        {
            switch (binaryType)
            {
                case "Attachments":
                    Files.DeleteFile(System.IO.Path.Combine(
                        Directories.BinaryStorage(),
                        "Attachments",
                        guid));
                    break;
                case "Images":
                    Files.DeleteFile(System.IO.Path.Combine(
                        Directories.BinaryStorage(),
                        "Images",
                        guid));
                    Files.DeleteFile(System.IO.Path.Combine(
                        Directories.BinaryStorage(),
                        "Images",
                        guid + "_thumbnail"));
                    break;
            }
        }

        private static void DeleteByTenantId(Context context, int tenantId)
        {
            var binariesChunkSize = Parameters.BackgroundService.DeleteTenantBinariesChunkSize;
            foreach (var tableType in AllTableTypes())
            {
                DeleteBinariesInChunks(
                    context: context,
                    tenantId: tenantId,
                    tableType: tableType,
                    chunkSize: binariesChunkSize);
            }
            foreach (var tableType in AllTableTypes())
            {
                Repository.ExecuteNonQuery(
                    context: context,
                    statements: new SqlStatement[]
                    {
                        Rds.PhysicalDeleteDepts(
                            tableType: tableType,
                            where: Rds.DeptsWhere().TenantId(tenantId)),
                        Rds.PhysicalDeleteGroups(
                            tableType: tableType,
                            where: Rds.GroupsWhere().TenantId(tenantId)),
                        Rds.PhysicalDeleteSites(
                            tableType: tableType,
                            where: Rds.SitesWhere().TenantId(tenantId)),
                        Rds.PhysicalDeleteStatuses(
                            tableType: tableType,
                            where: Rds.StatusesWhere().TenantId(tenantId)),
                        Rds.PhysicalDeleteUsers(
                            tableType: tableType,
                            where: Rds.UsersWhere().TenantId(tenantId)),
                        Rds.PhysicalDeleteExtensions(
                            tableType: tableType,
                            where: Rds.ExtensionsWhere().TenantId(tenantId)),
                        Rds.PhysicalDeleteMcpLogs(
                            tableType: tableType,
                            where: Rds.McpLogsWhere().TenantId(tenantId)),
                        Rds.PhysicalDeleteTenantQuotaUsages(
                            tableType: tableType,
                            where: Rds.TenantQuotaUsagesWhere().TenantId(tenantId)),
                        Rds.PhysicalDeleteAutoNumberings(
                            tableType: tableType,
                            where: Rds.AutoNumberingsWhere().TenantId(tenantId)),
                        Rds.PhysicalDeleteRegistrations(
                            tableType: tableType,
                            where: Rds.RegistrationsWhere().TenantId(tenantId)),
                        Rds.PhysicalDeleteBackgroundJobs(
                            tableType: tableType,
                            where: Rds.BackgroundJobsWhere().TenantId(tenantId)),
                        Rds.PhysicalDeleteDemos(
                            tableType: tableType,
                            where: Rds.DemosWhere().TenantId(tenantId)),
                        Rds.PhysicalDeleteTenants(
                            tableType: tableType,
                            _using: tableType != Sqls.TableTypes.Normal,
                            where: Rds.TenantsWhere().TenantId(tenantId)),
                    });
            }
        }

        private static void DeleteBinariesInChunks(
            Context context,
            int tenantId,
            Sqls.TableTypes tableType,
            int chunkSize)
        {
            var binTable = Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectBinaries(
                    tableType: tableType,
                    column: Rds.BinariesColumn()
                        .Binaries_BinaryId(function: Sqls.Functions.Min, _as: "Min")
                        .Binaries_BinaryId(function: Sqls.Functions.Max, _as: "Max"),
                    where: Rds.BinariesWhere().TenantId(tenantId)));
            var min = binTable.Rows.Count > 0 ? binTable.Rows[0]["Min"].ToLong() : 0L;
            var max = binTable.Rows.Count > 0 ? binTable.Rows[0]["Max"].ToLong() : 0L;
            if (min == 0 && max == 0) return;
            if (chunkSize <= 0) chunkSize = 10;
            for (var i = min; i <= max; i += chunkSize)
            {
                Repository.ExecuteNonQuery(
                    context: context,
                    statements: Rds.PhysicalDeleteBinaries(
                        tableType: tableType,
                        where: Rds.BinariesWhere()
                            .TenantId(tenantId)
                            .BinaryId_Between(
                                begin: i,
                                end: i + chunkSize - 1)));
            }
        }

        private static IEnumerable<Sqls.TableTypes> AllTableTypes()
        {
            return new[]
            {
                Sqls.TableTypes.Normal,
                Sqls.TableTypes.History,
                Sqls.TableTypes.Deleted,
            };
        }
    }
}
