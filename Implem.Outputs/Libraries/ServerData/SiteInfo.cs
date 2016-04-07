using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Views;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.ServerData
{
    public static class SiteInfo
    {
        private static Dictionary<long, List<int>> SiteDeptIdCollection =
            new Dictionary<long, List<int>>();
        private static Dictionary<long, List<int>> SiteUserIdCollection =
            new Dictionary<long, List<int>>();
        public static Dictionary<int, DeptModel> Depts;
        public static Dictionary<int, User> Users;

        /// <summary>
        /// Fixed:
        /// </summary>
        public static void SetSiteDeptIdCollection(long siteId, bool reload = false)
        {
            if (!SiteDeptIdCollection.ContainsKey(siteId))
            {
                SiteDeptIdCollection.Add(siteId, GetSiteDeptIdCollection(siteId));
            }
            else if (reload)
            {
                SiteDeptIdCollection[siteId] = GetSiteDeptIdCollection(siteId);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static List<int> GetSiteDeptIdCollection(long siteId)
        {
            var siteDeptCollection = new List<int>();
            foreach (DataRow dataRow in SiteDeptDataTable(siteId).Rows)
            {
                siteDeptCollection.Add(dataRow["DeptId"].ToInt());
            }
            return siteDeptCollection;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static DataTable SiteDeptDataTable(long siteId)
        {
            return Rds.ExecuteTable(statements: new SqlStatement(
                commandText: Def.Sql.SiteDepts,
                param: Rds.PermissionsParam()
                    .ReferenceType("Sites")
                    .ReferenceId(siteId)));
        }

        public static IEnumerable<int> UserIdCollection(long siteId)
        {
            if (!SiteUserIdCollection.ContainsKey(siteId))
            {
                SetSiteUserIdCollection(siteId, reload: true);
            }
            return SiteUserIdCollection[siteId];
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static void SetSiteUserIdCollection(long siteId, bool reload = false)
        {
            if (!SiteUserIdCollection.ContainsKey(siteId))
            {
                SiteUserIdCollection.Add(siteId, GetSiteUserIdCollection(siteId));
            }
            else if (reload)
            {
                SiteInfo.SiteUserIdCollection[siteId] = GetSiteUserIdCollection(siteId);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static List<int> GetSiteUserIdCollection(long siteId)
        {
            var siteUserCollection = new List<int>();
            foreach (DataRow dataRow in SiteUserDataTable(siteId).Rows)
            {
                siteUserCollection.Add(dataRow["UserId"].ToInt());
            }
            return siteUserCollection;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static DataTable SiteUserDataTable(long siteId)
        {
            return Rds.ExecuteTable(statements: new SqlStatement(
                commandText: Def.Sql.SiteUsers,
                param: Rds.PermissionsParam()
                    .ReferenceType("Sites")
                    .ReferenceId(siteId)));
        }

        public static Dictionary<int, DeptModel> GetDepts()
        {
            if (Depts == null) RefreshDepts();
            return Depts;
        }

        public static void RefreshDepts()
        {
            Depts = new DeptCollection(
                SiteSettingsUtility.DeptsSiteSettings(),
                Permissions.Types.TenantAdmin)
                    .ToDictionary(o => o.DeptId, o => o);
        }

        public static bool DeptExists(int deptId)
        {
            return GetDepts().ContainsKey(deptId);
        }

        public static DeptModel DeptModel(int deptId)
        {
            return GetDepts()
                .Where(o => o.Key == deptId)
                .Select(o => o.Value)
                .FirstOrDefault() ?? new DeptModel(
                    SiteSettingsUtility.DeptsSiteSettings(),
                    Permissions.Types.TenantAdmin,
                    deptId);
        }

        public static Dictionary<int, User> GetUsers()
        {
            if (Users == null) RefreshUsers();
            return Users;
        }

        public static void SetUser(User user)
        {
            if (UserExists(user.Id))
            {
                Users[user.Id] = user;
            }
            else
            {
                Users.Add(user.Id, user);
            }
        }

        public static void RefreshUsers()
        {
            Users = Rds.ExecuteTable(statements:
                Rds.SelectUsers(
                    column: Rds.UsersColumn()
                        .TenantId()
                        .UserId()
                        .DeptId()
                        .FirstName()
                        .LastName()
                        .FirstAndLastNameOrder()
                        .TenantAdmin()
                        .ServiceAdmin())).AsEnumerable().ToDictionary(
                            o => o["UserId"].ToInt(), o => new User(o));
        }

        public static bool UserExists(int userId)
        {
            return GetUsers().ContainsKey(userId);
        }

        public static User User(int userId)
        {
            if (!UserExists(userId))
            {
                var user = new User(userId);
                Users.Add(userId, user);
            }
            return GetUsers()
                .Where(o => o.Key == userId)
                .Select(o => o.Value)
                .FirstOrDefault();
        }

        public static string UserFullName(int userId)
        {
            return SiteInfo.User(userId).FullName;
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

        public static string PageKey()
        {
            return PageKey(Routes.Action());
        }

        public static string PageKey(string callerOfMethod)
        {
            if (Sessions.Created())
            {
                var path = Url.AbsolutePath().ToLower()
                    .Split('/').Where(o => o != string.Empty).ToList();
                var methodIndex = path.IndexOf(callerOfMethod.ToLower());
                return methodIndex != -1
                    ? path.Take(methodIndex).Join("/")
                    : path.Join("/");
            }
            return string.Empty;
        }

        public static string PageKey(BaseModel baseModel, string name)
        {
            return SiteInfo.PageKey(Routes.Action()) + "_" + name.ToLower();
        }
    }
}