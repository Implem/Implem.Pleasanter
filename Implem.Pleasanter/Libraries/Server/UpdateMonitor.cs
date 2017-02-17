using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Server
{
    public class UpdateMonitor
    {
        public static DateTime DeptsUpdatedTime;
        public static DateTime UsersUpdatedTime;
        public static DateTime PermissionsUpdatedTime;
        public static DateTime SitesUpdatedTime;
        public DateTime NowDeptsUpdatedTime;
        public DateTime NowUsersUpdatedTime;
        public DateTime NowPermissionsUpdatedTime;
        public DateTime NowSitesUpdatedTime;
        public Dictionary<string, DateTime> UpdatedTimeHash;
        public bool Updated;
        public bool DeptsUpdated;
        public bool UsersUpdated;
        public bool PermissionsUpdated;
        public bool SitesUpdated;

        public UpdateMonitor()
        {
            Set();
            DeptsUpdated = DeptsUpdatedTime < NowDeptsUpdatedTime;
            UsersUpdated = UsersUpdatedTime < NowUsersUpdatedTime;
            PermissionsUpdated = PermissionsUpdatedTime < NowPermissionsUpdatedTime;
            SitesUpdated = SitesUpdatedTime < NowSitesUpdatedTime;
            Updated = DeptsUpdated || UsersUpdated || SitesUpdated || PermissionsUpdated;
        }

        private void Set()
        {
            var dataSet = Rds.ExecuteDataSet(statements: new SqlStatement[]
            {
                Rds.SelectDepts(
                    dataTableName: "Depts",
                    tableType: Sqls.TableTypes.NormalAndDeleted,
                    column: Rds.DeptsColumn().UpdatedTime(),
                    where: Rds.DeptsWhere().UpdatedTime(
                        DeptsUpdatedTime,
                        _operator: ">=",
                        _using: DeptsUpdatedTime >= Parameters.General.MinTime),
                    orderBy: Rds.DeptsOrderBy()
                        .UpdatedTime(SqlOrderBy.Types.desc, tableName: null)),
                Rds.SelectUsers(
                    dataTableName: "Users",
                    tableType: Sqls.TableTypes.NormalAndDeleted,
                    column: Rds.UsersColumn().UpdatedTime(),
                    where: Rds.UsersWhere().UpdatedTime(
                        UsersUpdatedTime,
                        _operator: ">=",
                        _using: UsersUpdatedTime >= Parameters.General.MinTime),
                    orderBy: Rds.UsersOrderBy()
                        .UpdatedTime(SqlOrderBy.Types.desc, tableName: null)),
                Rds.SelectPermissions(
                    dataTableName: "Permissions",
                    tableType: Sqls.TableTypes.NormalAndDeleted,
                    column: Rds.PermissionsColumn().UpdatedTime(),
                    where: Rds.PermissionsWhere().UpdatedTime(
                        PermissionsUpdatedTime,
                        _operator: ">=",
                        _using: PermissionsUpdatedTime >= Parameters.General.MinTime),
                    orderBy: Rds.PermissionsOrderBy()
                        .UpdatedTime(SqlOrderBy.Types.desc, tableName: null)),
                Rds.SelectSites(
                    dataTableName: "Sites",
                    tableType: Sqls.TableTypes.NormalAndDeleted,
                    column: Rds.SitesColumn().UpdatedTime(),
                    where: Rds.SitesWhere().UpdatedTime(
                        SitesUpdatedTime,
                        _operator: ">=",
                        _using: SitesUpdatedTime >= Parameters.General.MinTime),
                    orderBy: Rds.SitesOrderBy()
                        .UpdatedTime(SqlOrderBy.Types.desc, tableName: null)),
            });
            NowDeptsUpdatedTime = UpdatedTime(dataSet, "Depts");
            NowUsersUpdatedTime = UpdatedTime(dataSet, "Users");
            NowPermissionsUpdatedTime = UpdatedTime(dataSet, "Permissions");
            NowSitesUpdatedTime = UpdatedTime(dataSet, "Sites");
        }

        private DateTime UpdatedTime(DataSet dataSet, string name)
        {
            var time = dataSet.Tables[name]?
                .AsEnumerable()
                .Select(o => o["UpdatedTime"].ToDateTime())
                .OrderByDescending(o => o)
                .FirstOrDefault();
            return time >= Parameters.General.MinTime
                ? time.ToDateTime()
                : Parameters.General.MinTime;
        }

        public void Update()
        {
            if (DeptsUpdated) DeptsUpdatedTime = NowDeptsUpdatedTime;
            if (UsersUpdated) UsersUpdatedTime = NowUsersUpdatedTime;
            if (PermissionsUpdated) PermissionsUpdatedTime = NowPermissionsUpdatedTime;
            if (SitesUpdated) SitesUpdatedTime = NowSitesUpdatedTime;
        }
    }
}