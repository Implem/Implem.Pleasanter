using Implem.DefinitionAccessor;
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
        public static SiteMenu SiteMenu = new SiteMenu();
        public static Dictionary<long, List<int>> SiteUserIdCollection =
            new Dictionary<long, List<int>>();
        public static Dictionary<int, Dept> DeptHash;
        public static Dictionary<int, User> UserHash;

        public static IEnumerable<int> UserIdCollection(long siteId)
        {
            if (!SiteUserIdCollection.ContainsKey(siteId))
            {
                SetSiteUserIdCollection(siteId, reload: true);
            }
            return SiteUserIdCollection[siteId];
        }

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

        private static List<int> GetSiteUserIdCollection(long siteId)
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
            return Rds.ExecuteTable(statements: new SqlStatement(
                commandText: Def.Sql.SiteUsers,
                param: Rds.PermissionsParam()
                    .ReferenceType("Sites")
                    .ReferenceId(siteId)));
        }

        public static Dictionary<int, Dept> GetDepts()
        {
            if (DeptHash == null) RefreshDepts();
            return DeptHash;
        }

        public static void SetDept(DeptModel deptModel)
        {
            if (DeptExists(deptModel.DeptId))
            {
                DeptHash[deptModel.DeptId] = new Dept(
                    deptModel.TenantId, deptModel.DeptId, deptModel.DeptName);
            }
            else
            {
                DeptHash.Add(deptModel.DeptId, new Dept(
                    deptModel.TenantId, deptModel.DeptId, deptModel.DeptName));
            }
        }

        public static void RefreshDepts()
        {
            DeptHash = Rds.ExecuteTable(statements:
                Rds.SelectDepts(
                    column: Rds.DeptsColumn()
                        .TenantId()
                        .DeptId()
                        .DeptName()))
                            .AsEnumerable()
                            .Select(o => new Dept(
                                o["TenantId"].ToInt(),
                                o["DeptId"].ToInt(),
                                o["DeptName"].ToString()))
                            .ToDictionary(o => o.Id, o => o);
        }

        public static bool DeptExists(int deptId)
        {
            return GetDepts().ContainsKey(deptId);
        }

        public static Dept Dept(int deptId)
        {
            return GetDepts()
                .Where(o => o.Key == deptId)
                .Select(o => o.Value)
                .FirstOrDefault();
        }

        public static Dictionary<int, User> GetUsers()
        {
            if (UserHash == null) RefreshUsers();
            return UserHash;
        }

        public static void SetUser(User user)
        {
            if (UserExists(user.Id))
            {
                UserHash[user.Id] = user;
            }
            else
            {
                UserHash.Add(user.Id, user);
            }
        }

        public static void RefreshUsers()
        {
            UserHash = Rds.ExecuteTable(statements:
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
                UserHash.Add(userId, user);
            }
            return GetUsers()
                .Where(o => o.Key == userId)
                .Select(o => o.Value)
                .FirstOrDefault();
        }

        public static string UserFullName(int userId, bool notSet = true)
        {
            var fullName = SiteInfo.User(userId).FullName();
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