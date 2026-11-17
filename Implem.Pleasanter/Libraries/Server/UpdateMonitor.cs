using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Server
{
    public class UpdateMonitor
    {
        public string DeptsUpdatedToken;
        public string GroupsUpdatedToken;
        public string UsersUpdatedToken;
        public string PermissionsUpdatedToken;
        public string NowDeptsUpdatedToken;
        public string NowGroupsUpdatedToken;
        public string NowUsersUpdatedToken;
        public string NowPermissionsUpdatedToken;
        public Dictionary<string, DateTime> UpdatedTimeHash;
        public bool Updated;
        public bool DeptsUpdated;
        public bool GroupsUpdated;
        public bool UsersUpdated;
        public bool PermissionsUpdated;

        public UpdateMonitor(Context context)
        {
            Monitor(context: context);
        }

        internal UpdateMonitor()
        {
        }

        public void Monitor(Context context)
        {
            Monitor(hash: MonitorTokens(dataRows: Repository.ExecuteTable(
                context: context,
                statements: MonitorStatement(tenantId: context.TenantId))
                    .AsEnumerable()));
        }

        internal void Monitor(Dictionary<StatusUtilities.Types, string> hash)
        {
            NowDeptsUpdatedToken = hash[StatusUtilities.Types.DeptsUpdated];
            NowGroupsUpdatedToken = hash[StatusUtilities.Types.GroupsUpdated];
            NowUsersUpdatedToken = hash[StatusUtilities.Types.UsersUpdated];
            NowPermissionsUpdatedToken = hash[StatusUtilities.Types.PermissionsUpdated];
            DeptsUpdated = DeptsUpdatedToken != NowDeptsUpdatedToken;
            GroupsUpdated = GroupsUpdatedToken != NowGroupsUpdatedToken;
            UsersUpdated = UsersUpdatedToken != NowUsersUpdatedToken;
            PermissionsUpdated = PermissionsUpdatedToken != NowPermissionsUpdatedToken;
            Updated = DeptsUpdated || GroupsUpdated || UsersUpdated || PermissionsUpdated;
        }

        internal static SqlSelect MonitorStatement(int tenantId)
        {
            return Rds.SelectStatuses(
                column: Rds.StatusesColumn()
                    .StatusId()
                    .UpdatedTime()
                    .Value(),
                where: Rds.StatusesWhere()
                    .TenantId(tenantId)
                    .StatusId_In(StatusUtilities.MonitorHash().Keys
                        .Select(o => o.ToInt())));
        }

        internal static Dictionary<StatusUtilities.Types, string> MonitorTokens(
            IEnumerable<DataRow> dataRows)
        {
            var hash = StatusUtilities.MonitorHash().Keys.ToDictionary(
                o => o,
                _ => Strings.NewGuid());
            foreach (var dataRow in dataRows)
            {
                var type = (StatusUtilities.Types)dataRow.Int("StatusId");
                if (hash.ContainsKey(type))
                {
                    hash[type] = dataRow.Field<DateTime>("UpdatedTime").Ticks
                        + "|"
                        + (dataRow.Field<string>("Value") ?? string.Empty);
                }
            }
            return hash;
        }

        public void Update()
        {
            DeptsUpdatedToken = NowDeptsUpdatedToken;
            GroupsUpdatedToken = NowGroupsUpdatedToken;
            UsersUpdatedToken = NowUsersUpdatedToken;
            PermissionsUpdatedToken = NowPermissionsUpdatedToken;
        }
    }
}