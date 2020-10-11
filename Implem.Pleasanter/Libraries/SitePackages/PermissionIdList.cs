using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.SitePackages
{
    public class PermissionIdList
    {
        public List<DeptIdHash> DeptIdList;
        public List<GroupIdHash> GroupIdList;
        public List<UserIdHash> UserIdList;

        public PermissionIdList()
        {
        }

        public PermissionIdList(
            Context context,
            List<PackageSiteModel> sites,
            List<PackagePermissionModel> packagePermissionModels)
        {
            DeptIdList = new List<DeptIdHash>();
            GroupIdList = new List<GroupIdHash>();
            UserIdList = new List<UserIdHash>();
            Add(
                context: context,
                sites: sites,
                packagePermissionModels: packagePermissionModels);
        }

        private void Add(
            Context context,
            List<PackageSiteModel> sites,
            List<PackagePermissionModel> packagePermissionModels)
        {
            var deptIds = new HashSet<int>();
            var groupIds = new HashSet<int>();
            var userIds = new HashSet<int>();
            foreach (var packagePermissionModel in packagePermissionModels)
            {
                foreach (var permission in packagePermissionModel.Permissions)
                {
                    if (permission.DeptId > 0 && !deptIds.Contains(permission.DeptId))
                    {
                        deptIds.Add(permission.DeptId);
                    }
                    if (permission.GroupId > 0 && !groupIds.Contains(permission.GroupId))
                    {
                        groupIds.Add(permission.GroupId);
                    }
                    if (permission.UserId > 0 && !userIds.Contains(permission.UserId))
                    {
                        userIds.Add(permission.UserId);
                    }
                }
            }
            foreach (var packageSiteModel in sites)
            {
                foreach (var cca in packageSiteModel.SiteSettings.CreateColumnAccessControls)
                {
                    foreach (var dept in cca.Depts ?? new List<int>() { 0 })
                    {
                        deptIds.Add(dept);
                    }
                    foreach (var group in cca.Groups ?? new List<int>() { 0 })
                    {
                        groupIds.Add(group);
                    }
                    foreach (var user in cca.Users ?? new List<int>() { 0 })
                    {
                        userIds.Add(user);
                    }
                }
                foreach (var rca in packageSiteModel.SiteSettings.ReadColumnAccessControls)
                {
                    foreach (var dept in rca.Depts ?? new List<int>() { 0 })
                    {
                        deptIds.Add(dept);
                    }
                    foreach (var group in rca.Groups ?? new List<int>() { 0 })
                    {
                        groupIds.Add(group);
                    }
                    foreach (var user in rca.Users ?? new List<int>() { 0 })
                    {
                        userIds.Add(user);
                    }
                }
                foreach (var uca in packageSiteModel.SiteSettings.UpdateColumnAccessControls)
                {
                    foreach (var dept in uca.Depts ?? new List<int>() { 0 })
                    {
                        deptIds.Add(dept);
                    }
                    foreach (var group in uca.Groups ?? new List<int>() { 0 })
                    {
                        groupIds.Add(group);
                    }
                    foreach (var user in uca.Users ?? new List<int>() { 0 })
                    {
                        userIds.Add(user);
                    }
                }
            }
            if (deptIds.Any())
            {
                var deptTable = Repository.ExecuteTable(
                    context: context,
                    statements: Rds.SelectDepts(
                        column: Rds.DeptsColumn()
                            .DeptId()
                            .DeptCode(),
                        where: Rds.DeptsWhere()
                            .DeptId_In(deptIds.Where(o => o != 0))));
                foreach (DataRow data in deptTable.Rows)
                {
                    DeptIdList.Add(
                        new DeptIdHash()
                        {
                            DeptId = data[0].ToInt(),
                            DeptCode = data[1].ToString()
                        });
                }
            }
            if (groupIds.Count() > 0)
            {
                var groupTable = Repository.ExecuteTable(
                    context: context,
                    statements: Rds.SelectGroups(
                        column: Rds.GroupsColumn()
                            .GroupId()
                            .GroupName(),
                        where: Rds.GroupsWhere()
                            .GroupId_In(
                                value: groupIds.Where(o => o != 0))));
                foreach (DataRow dataRow in groupTable.Rows)
                {
                    GroupIdList.Add(
                        new GroupIdHash()
                        {
                            GroupId = dataRow[0].ToInt(),
                            GroupName = dataRow[1].ToString()
                        });
                }
            }
            if (userIds.Count() > 0)
            {
                var userTable = Repository.ExecuteTable(
                    context: context,
                    statements: Rds.SelectUsers(
                        column: Rds.UsersColumn()
                            .UserId()
                            .LoginId(),
                        where: Rds.UsersWhere()
                            .UserId_In(
                                value: userIds.Where(o => o != 0))));
                foreach (DataRow dataRow in userTable.Rows)
                {
                    UserIdList.Add(
                        new UserIdHash()
                        {
                            UserId = dataRow[0].ToInt(),
                            LoginId = dataRow[1].ToString()
                        });
                }
            }
        }
    }
}