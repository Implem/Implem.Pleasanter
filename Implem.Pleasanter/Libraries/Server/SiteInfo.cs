using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Server
{
    public static class SiteInfo
    {
        public static SiteMenu SiteMenu;
        public static Dictionary<int, Dept> DeptHash;
        public static Dictionary<int, User> UserHash;
        public static Dictionary<long, List<int>> SiteUserHash;

        public static void Reflesh(bool force = false)
        {
            var monitor = new UpdateMonitor();
            if (monitor.Updated || force)
            {
                var tenantId = Sessions.TenantId();
                var dataSet = Rds.ExecuteDataSet(statements: new SqlStatement[]
                {
                    Rds.SelectSites(
                        dataTableName: "Sites",
                        column: Rds.SitesColumn()
                            .TenantId()
                            .ReferenceType()
                            .ParentId()
                            .Title(),
                        where: Rds.SitesWhere()
                            .TenantId(tenantId, _using: tenantId != 0)
                            .PermissionType(_operator: ">0")),
                    Rds.SelectDepts(
                        dataTableName: "Depts",
                        column: Rds.DeptsColumn()
                            .TenantId()
                            .DeptId()
                            .DeptName(),
                        _using: monitor.DeptsUpdated || force),
                    Rds.SelectUsers(
                        dataTableName: "Users",
                        column: Rds.UsersColumn()
                            .TenantId()
                            .UserId()
                            .DeptId()
                            .FirstName()
                            .LastName()
                            .FirstAndLastNameOrder()
                            .TenantManager()
                            .ServiceManager(),
                        _using: monitor.UsersUpdated || force)
                });
                if (monitor.DeptsUpdated || force)
                {
                    DeptHash = dataSet.Tables["Depts"]
                        .AsEnumerable()
                        .ToDictionary(
                            dataRow => dataRow["DeptId"].ToInt(),
                            dataRow => new Dept(dataRow));
                }
                if (monitor.UsersUpdated || force)
                {
                    UserHash = dataSet.Tables["Users"]
                        .AsEnumerable()
                        .ToDictionary(
                            dataRow => dataRow["UserId"].ToInt(),
                            dataRow => new User(dataRow));
                }
                if (monitor.PermissionsUpdated || monitor.GroupsUpdated || force)
                {
                    SiteUserHash = new Dictionary<long, List<int>>();
                }
                if (monitor.SitesUpdated || force)
                {
                    SiteMenu = new SiteMenu();
                }
                monitor.Update();
            }
        }

        public static IEnumerable<int> SiteUsers(long siteId)
        {
            if (!SiteUserHash.ContainsKey(siteId))
            {
                SetSiteUserHash(siteId, reload: true);
            }
            return SiteUserHash[siteId];
        }

        public static void SetSiteUserHash(long siteId, bool reload = false)
        {
            if (!SiteUserHash.ContainsKey(siteId))
            {
                SiteUserHash.Add(siteId, GetSiteUserHash(siteId));
            }
            else if (reload)
            {
                SiteUserHash[siteId] = GetSiteUserHash(siteId);
            }
        }

        private static List<int> GetSiteUserHash(long siteId)
        {
            var siteUserCollection = new List<int>();
            foreach (DataRow dataRow in SiteUserDataTable(siteId).Rows)
            {
                siteUserCollection.Add(dataRow["UserId"].ToInt());
            }
            return siteUserCollection;
        }

        private static DataTable SiteUserDataTable(long siteId)
        {
            var deptRaw = "[Users].[DeptId] and [Users].[DeptId]>0";
            var userRaw = "[Users].[UserId] and [Users].[UserId]>0";
            return Rds.ExecuteTable(statements: Rds.SelectUsers(
                distinct: true,
                column: Rds.UsersColumn().UserId(),
                where: Rds.UsersWhere()
                    .TenantId(Sessions.TenantId())
                    .Add(
                        subLeft: Rds.SelectPermissions(
                            column: Rds.PermissionsColumn()
                                .PermissionType(function: Sqls.Functions.Max),
                            where: Rds.PermissionsWhere()
                                .ReferenceType("Sites")
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
            return DeptHash?
                .Where(o => o.Key == deptId)
                .Select(o => o.Value)
                .FirstOrDefault();
        }

        public static User User(int userId)
        {
            return UserHash?
                .Where(o => o.Key == userId)
                .Select(o => o.Value)
                .FirstOrDefault() ?? Anonymouse();
        }

        private static User Anonymouse()
        {
            return new User(DataTypes.User.UserTypes.Anonymous.ToInt());
        }

        public static string UserFullName(int userId, bool notSet = true)
        {
            var fullName = User(userId).FullName();
            return notSet || fullName != Displays.NotSet()
                ? fullName
                : string.Empty;
        }

        public static string IndexReferenceType(string referenceType, long referenceId)
        {
            return referenceType.ToLower() == "items"
                ? new SiteModel(referenceId).ReferenceType
                : referenceType;
        }

        public static bool IsItem()
        {
            return Url.RouteData("reference") != null
                ? Url.RouteData("reference").ToLower() == "items"
                : Routes.Controller() == "items";
        }
    }
}