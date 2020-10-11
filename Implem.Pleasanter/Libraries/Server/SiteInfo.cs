﻿using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Group = Implem.Pleasanter.Libraries.DataTypes.Group;
namespace Implem.Pleasanter.Libraries.Server
{
    public static class SiteInfo
    {
        public static Dictionary<int, TenantCache> TenantCaches = new Dictionary<int, TenantCache>();

        public static void Reflesh(Context context, bool force = false)
        {
            if (context.TenantId == 0)
            {
                return;
            }
            var tenantCache = TenantCaches.Get(context.TenantId);
            var monitor = tenantCache.GetUpdateMonitor(context: context);
            if (monitor.DeptsUpdated || monitor.GroupsUpdated || monitor.UsersUpdated || force)
            {
                var dataSet = Repository.ExecuteDataSet(
                    context: context,
                    statements: new SqlStatement[]
                    {
                        Rds.SelectDepts(
                            dataTableName: "Depts",
                            column: Rds.DeptsColumn()
                                .TenantId()
                                .DeptId()
                                .DeptCode()
                                .DeptName(),
                            where: Rds.DeptsWhere().TenantId(context.TenantId),
                            _using: monitor.DeptsUpdated || force),
                        Rds.SelectGroups(
                            dataTableName: "Groups",
                            column: Rds.GroupsColumn()
                                .TenantId()
                                .GroupId()
                                .GroupName(),
                            where: Rds.GroupsWhere().TenantId(context.TenantId),
                            _using: monitor.GroupsUpdated || force),
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
                            where: Rds.UsersWhere().TenantId(context.TenantId),
                            _using: monitor.UsersUpdated || force)
                    });
                if (monitor.DeptsUpdated || force)
                {
                    tenantCache.DeptHash = dataSet.Tables["Depts"]
                        .AsEnumerable()
                        .ToDictionary(
                            dataRow => dataRow.Int("DeptId"),
                            dataRow => new Dept(dataRow));
                }
                if (monitor.GroupsUpdated || force)
                {
                    tenantCache.GroupHash = dataSet.Tables["Groups"]
                        .AsEnumerable()
                        .ToDictionary(
                            dataRow => dataRow.Int("GroupId"),
                            dataRow => new Group(dataRow));
                }
                if (monitor.UsersUpdated || force)
                {
                    tenantCache.UserHash = dataSet.Tables["Users"]
                        .AsEnumerable()
                        .ToDictionary(
                            dataRow => dataRow.Int("UserId"),
                            dataRow => new User(
                                context: context,
                                dataRow: dataRow));
                }
            }
            if (monitor.PermissionsUpdated || monitor.GroupsUpdated || monitor.UsersUpdated || force)
            {
                tenantCache.SiteDeptHash = new Dictionary<long, List<int>>();
                tenantCache.SiteGroupHash = new Dictionary<long, List<int>>();
                tenantCache.SiteUserHash = new Dictionary<long, List<int>>();
            }
            if (monitor.SitesUpdated || force)
            {
                tenantCache.SiteMenu = new SiteMenu(context: context);
            }
            if (monitor.Updated || force)
            {
                monitor.Update();
            }
        }

        public static IEnumerable<int> SiteDepts(Context context, long siteId)
        {
            if (context.TenantId == 0)
            {
                return new List<int>();
            }
            var tenantCache = TenantCaches.Get(context.TenantId);
            if (!tenantCache.SiteDeptHash.ContainsKey(siteId))
            {
                SetSiteDeptHash(
                    context: context,
                    siteId: siteId,
                    reload: true);
            }
            return tenantCache.SiteDeptHash.Get(siteId);
        }

        public static IEnumerable<int> SiteGroups(Context context, long siteId)
        {
            if (context.TenantId == 0)
            {
                return new List<int>();
            }
            var tenantCache = TenantCaches.Get(context.TenantId);
            if (!tenantCache.SiteGroupHash.ContainsKey(siteId))
            {
                SetSiteGroupHash(
                    context: context,
                    siteId: siteId,
                    reload: true);
            }
            return tenantCache.SiteGroupHash.Get(siteId);
        }

        public static IEnumerable<int> SiteUsers(Context context, long siteId)
        {
            if (context.TenantId == 0)
            {
                return new List<int>();
            }
            var tenantCache = TenantCaches.Get(context.TenantId);
            if (!tenantCache.SiteUserHash.ContainsKey(siteId))
            {
                SetSiteUserHash(
                    context: context,
                    siteId: siteId,
                    reload: true);
            }
            return tenantCache.SiteUserHash.Get(siteId);
        }

        public static void SetSiteDeptHash(Context context, long siteId, bool reload = false)
        {
            if (context.TenantId == 0)
            {
                return;
            }
            var tenantCache = TenantCaches.Get(context.TenantId);
            if (!tenantCache.SiteDeptHash.ContainsKey(siteId))
            {
                try
                {
                    tenantCache.SiteDeptHash.Add(siteId, GetSiteDeptHash(
                        context: context,
                        siteId: siteId));
                }
                catch (Exception)
                {
                }
            }
            else if (reload)
            {
                tenantCache.SiteDeptHash[siteId] = GetSiteDeptHash(
                    context: context,
                    siteId: siteId);
            }
        }

        public static void SetSiteGroupHash(Context context, long siteId, bool reload = false)
        {
            if (context.TenantId == 0)
            {
                return;
            }
            var tenantCache = TenantCaches.Get(context.TenantId);
            if (!tenantCache.SiteGroupHash.ContainsKey(siteId))
            {
                try
                {
                    tenantCache.SiteGroupHash.Add(siteId, GetSiteGroupHash(
                        context: context,
                        siteId: siteId));
                }
                catch (Exception)
                {
                }
            }
            else if (reload)
            {
                tenantCache.SiteGroupHash[siteId] = GetSiteGroupHash(
                    context: context,
                    siteId: siteId);
            }
        }

        public static void SetSiteUserHash(Context context, long siteId, bool reload = false)
        {
            if (context.TenantId == 0)
            {
                return;
            }
            var tenantCache = TenantCaches.Get(context.TenantId);
            if (!tenantCache.SiteUserHash.ContainsKey(siteId))
            {
                try
                {
                    tenantCache.SiteUserHash.Add(siteId, GetSiteUserHash(
                        context: context,
                        siteId: siteId));
                }
                catch (Exception)
                {
                }
            }
            else if (reload)
            {
                tenantCache.SiteUserHash[siteId] = GetSiteUserHash(
                    context: context,
                    siteId: siteId);
            }
        }

        private static List<int> GetSiteDeptHash(Context context, long siteId)
        {
            var siteDeptCollection = new List<int>();
            foreach (DataRow dataRow in SiteDeptDataTable(
                context: context,
                siteId: siteId).Rows)
            {
                siteDeptCollection.Add(dataRow["DeptId"].ToInt());
            }
            return siteDeptCollection;
        }

        private static List<int> GetSiteGroupHash(Context context, long siteId)
        {
            var siteGroupCollection = new List<int>();
            foreach (DataRow dataRow in SiteGroupDataTable(
                context: context,
                siteId: siteId).Rows)
            {
                siteGroupCollection.Add(dataRow["GroupId"].ToInt());
            }
            return siteGroupCollection;
        }

        private static List<int> GetSiteUserHash(Context context, long siteId)
        {
            var siteUserCollection = new List<int>();
            foreach (DataRow dataRow in SiteUserDataTable(
                context: context,
                siteId: siteId).Rows)
            {
                siteUserCollection.Add(dataRow["UserId"].ToInt());
            }
            return siteUserCollection;
        }

        private static DataTable SiteDeptDataTable(Context context, long siteId)
        {
            var deptRaw = "[Depts].[DeptId] and [Depts].[DeptId]>0";
            return Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectDepts(
                    distinct: true,
                    column: Rds.DeptsColumn().DeptId(),
                    where: Rds.DeptsWhere()
                        .TenantId(context.TenantId)
                        .Add(
                            subLeft: Rds.SelectPermissions(
                                column: Rds.PermissionsColumn()
                                    .PermissionType(function: Sqls.Functions.Max),
                                where: Rds.PermissionsWhere()
                                    .ReferenceId(siteId)
                                    .DeptId(raw: deptRaw)),
                            _operator: ">0")));
        }

        private static DataTable SiteGroupDataTable(Context context, long siteId)
        {
            var groupRaw = "[Groups].[GroupId] and [Groups].[GroupId]>0";
            return Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectGroups(
                    distinct: true,
                    column: Rds.GroupsColumn().GroupId(),
                    where: Rds.GroupsWhere()
                        .TenantId(context.TenantId)
                        .Add(
                            subLeft: Rds.SelectPermissions(
                                column: Rds.PermissionsColumn()
                                    .PermissionType(function: Sqls.Functions.Max),
                                where: Rds.PermissionsWhere()
                                    .ReferenceId(siteId)
                                    .GroupId(raw: groupRaw)),
                            _operator: ">0")));
        }

        private static DataTable SiteUserDataTable(Context context, long siteId)
        {
            return Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectUsers(
                    distinct: true,
                    column: Rds.UsersColumn().UserId(),
                    where: Rds.UsersWhere()
                        .TenantId(context.TenantId)
                        .SiteUserWhere(siteId: siteId)));
        }

        public static string Name(Context context, int id, Settings.Column.Types type)
        {
            switch (type)
            {
                case Settings.Column.Types.Dept:
                    return Dept(
                        tenantId: context.TenantId,
                        deptId: id).Name;
                case Settings.Column.Types.Group:
                    return Group(
                        tenantId: context.TenantId,
                        groupId: id).Name;
                case Settings.Column.Types.User:
                    return UserName(
                        context: context,
                        userId: id);
                default:
                    return string.Empty;
            }
        }

        public static Dept Dept(int tenantId, int deptId)
        {
            if (tenantId == 0 || deptId == 0)
            {
                return new Dept();
            }
            return TenantCaches.Get(tenantId)?.DeptHash?
                .Where(o => o.Key == deptId)
                .Select(o => o.Value)
                .FirstOrDefault() ?? new Dept();
        }

        public static Group Group(int tenantId, int groupId)
        {
            if (tenantId == 0 || groupId == 0)
            {
                return new Group();
            }
            return TenantCaches.Get(tenantId)?.GroupHash?
                .Where(o => o.Key == groupId)
                .Select(o => o.Value)
                .FirstOrDefault() ?? new Group();
        }

        public static User User(Context context, int userId)
        {
            if (context.TenantId == 0)
            {
                return new User();
            }
            if (userId == -1)
            {
                return new User()
                {
                    TenantId = context.TenantId,
                    Id = -1,
                    Name = Displays.AllUsers(context: context)
                };
            }
            return TenantCaches.Get(context.TenantId)?.UserHash?.Get(userId)
                ?? Anonymous(context: context);
        }

        private static User Anonymous(Context context)
        {
            return new User(
                context: context,
                userId: DataTypes.User.UserTypes.Anonymous.ToInt());
        }

        public static string UserName(
            Context context, int userId, bool notSet = true, bool showDeptName = false)
        {
            var user = User(
                context: context,
                userId: userId);
            var name = user?.Name != null
                ? showDeptName
                    ? user.Dept.Name + ")" + user.Name
                    : user.Name
                : null;
            return name != null
                ? name
                : notSet
                    ? Displays.NotSet(context: context)
                    : string.Empty;
        }
    }
}