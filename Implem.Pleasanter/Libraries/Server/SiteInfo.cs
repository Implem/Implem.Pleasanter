using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Server
{
    public static class SiteInfo
    {
        public static Dictionary<int, TenantCache> TenantCaches = new Dictionary<int, TenantCache>();

        public static void Reflesh(bool force = false)
        {
            var tenantId = Sessions.TenantId();
            if (!TenantCaches.ContainsKey(tenantId))
            {
                try
                {
                    TenantCaches.Add(tenantId, new TenantCache(tenantId));
                }
                catch (Exception)
                {
                }
            }
            var tenantCache = TenantCaches.Get(tenantId);
            var monitor = tenantCache.GetUpdateMonitor();
            if (monitor.DeptsUpdated || monitor.UsersUpdated || force)
            {
                var dataSet = Rds.ExecuteDataSet(statements: new SqlStatement[]
                {
                    Rds.SelectDepts(
                        dataTableName: "Depts",
                        column: Rds.DeptsColumn()
                            .TenantId()
                            .DeptId()
                            .DeptName(),
                        where: Rds.DeptsWhere().TenantId(tenantId),
                        _using: monitor.DeptsUpdated || force),
                    Rds.SelectUsers(
                        dataTableName: "Users",
                        column: Rds.UsersColumn()
                            .TenantId()
                            .UserId()
                            .DeptId()
                            .LoginId()
                            .Name()
                            .TenantManager()
                            .ServiceManager()
                            .Disabled(),
                        where: Rds.UsersWhere().TenantId(tenantId),
                        _using: monitor.UsersUpdated || force)
                });
                if (monitor.DeptsUpdated || force)
                {
                    tenantCache.DeptHash = dataSet.Tables["Depts"]
                        .AsEnumerable()
                        .ToDictionary(
                            dataRow => dataRow["DeptId"].ToInt(),
                            dataRow => new Dept(dataRow));
                }
                if (monitor.UsersUpdated || force)
                {
                    tenantCache.UserHash = dataSet.Tables["Users"]
                        .AsEnumerable()
                        .ToDictionary(
                            dataRow => dataRow["UserId"].ToInt(),
                            dataRow => new User(dataRow));
                }
            }
            if (monitor.PermissionsUpdated || monitor.GroupsUpdated || monitor.UsersUpdated || force)
            {
                tenantCache.SiteUserHash = new Dictionary<long, List<int>>();
            }
            if (monitor.SitesUpdated || force)
            {
                tenantCache.SiteMenu = new SiteMenu(tenantId);
            }
            if (monitor.Updated || force)
            {
                monitor.Update();
            }
        }

        public static IEnumerable<int> SiteUsers(int tenantId, long siteId)
        {
            var tenantCache = TenantCaches.Get(tenantId);
            if (!tenantCache.SiteUserHash.ContainsKey(siteId))
            {
                SetSiteUserHash(tenantId, siteId, reload: true);
            }
            return tenantCache.SiteUserHash[siteId];
        }

        public static void SetSiteUserHash(int tenantId, long siteId, bool reload = false)
        {
            var tenantCache = TenantCaches.Get(tenantId);
            if (!tenantCache.SiteUserHash.ContainsKey(siteId))
            {
                try
                {
                    tenantCache.SiteUserHash.Add(siteId, GetSiteUserHash(tenantId, siteId));
                }
                catch (Exception)
                {
                }
            }
            else if (reload)
            {
                tenantCache.SiteUserHash[siteId] = GetSiteUserHash(tenantId, siteId);
            }
        }

        private static List<int> GetSiteUserHash(int tenantId, long siteId)
        {
            var siteUserCollection = new List<int>();
            foreach (DataRow dataRow in SiteUserDataTable(tenantId, siteId).Rows)
            {
                siteUserCollection.Add(dataRow["UserId"].ToInt());
            }
            return siteUserCollection;
        }

        private static DataTable SiteUserDataTable(int tenantId, long siteId)
        {
            var deptRaw = "[Users].[DeptId] and [Users].[DeptId]>0";
            var userRaw = "[Users].[UserId] and [Users].[UserId]>0";
            return Rds.ExecuteTable(statements: Rds.SelectUsers(
                distinct: true,
                column: Rds.UsersColumn().UserId(),
                where: Rds.UsersWhere()
                    .TenantId(tenantId)
                    .Add(
                        subLeft: Rds.SelectPermissions(
                            column: Rds.PermissionsColumn()
                                .PermissionType(function: Sqls.Functions.Max),
                            where: Rds.PermissionsWhere()
                                .ReferenceId(siteId)
                                .Or(Rds.PermissionsWhere()
                                    .DeptId(raw: deptRaw)
                                    .Add(
                                        subLeft: Rds.SelectGroupMembers(
                                            column: Rds.GroupMembersColumn()
                                                .GroupMembersCount(),
                                            where: Rds.GroupMembersWhere()
                                                .GroupId(raw: "[Permissions].[GroupId]")
                                                .Or(Rds.GroupMembersWhere()
                                                    .DeptId(raw: deptRaw)
                                                    .UserId(raw: userRaw))
                                                .Add(raw: "[Permissions].[GroupId]>0")),
                                        _operator: ">0")
                                    .UserId(raw: userRaw))),
                        _operator: ">0")));
        }

        public static Dept Dept(int deptId)
        {
            return TenantCaches.Get(Sessions.TenantId())?.DeptHash?
                .Where(o => o.Key == deptId)
                .Select(o => o.Value)
                .FirstOrDefault();
        }

        public static User User(int userId)
        {
            return TenantCaches.Get(Sessions.TenantId())?.UserHash?
                .Where(o => o.Key == userId)
                .Select(o => o.Value)
                .FirstOrDefault() ?? Anonymouse();
        }

        private static User Anonymouse()
        {
            return new User(DataTypes.User.UserTypes.Anonymous.ToInt());
        }

        public static string UserName(int userId, bool notSet = true)
        {
            var name = User(userId).Name;
            return name != null
                ? name
                : notSet
                    ? Displays.NotSet()
                    : string.Empty;
        }
    }
}