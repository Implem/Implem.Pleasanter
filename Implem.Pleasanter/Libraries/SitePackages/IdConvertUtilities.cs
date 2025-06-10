using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using System.Linq;
namespace Implem.Pleasanter.Libraries.SitePackages
{
    public static class IdConvertUtilities
    {
        public static int ConvertedDeptId(
            Context context,
            PermissionIdList permissionIdList,
            int deptId)
        {
            if (permissionIdList.DeptConvertCache.ContainsKey(deptId))
            {
                return permissionIdList.DeptConvertCache[deptId];
            }
            var dept = permissionIdList?.DeptIdList.FirstOrDefault(x => x.DeptId == deptId);
            if (dept == null)
            {
                return 0;
            }
            {
                var id = SiteInfo.TenantCaches.Get(context.TenantId)
                    .DeptHash
                    .Values
                    .FirstOrDefault(o => o.Code == dept.DeptCode)
                    ?.Id ?? 0;
                if (id > 0)
                {
                    permissionIdList.DeptConvertCache[deptId] = id;
                    return id;
                }
            }
            {
                var id = SiteInfo.TenantCaches.Get(context.TenantId)
                    .DeptHash
                    .Values
                    .FirstOrDefault(o => o.Name == dept.DeptName)
                    ?.Id ?? 0;
                if (id > 0)
                {
                    permissionIdList.DeptConvertCache[deptId] = id;
                    return id;
                }
            }
            return 0;
        }

        public static int ConvertedGroupId(
            Context context,
            PermissionIdList permissionIdList,
            int groupId)
        {
            if (permissionIdList.GroupConvertCache.ContainsKey(groupId))
            {
                return permissionIdList.GroupConvertCache[groupId];
            }
            var group = permissionIdList?.GroupIdList.FirstOrDefault(x => x.GroupId == groupId);
            if (group == null)
            {
                return 0;
            }
            {
                var id = SiteInfo.TenantCaches.Get(context.TenantId)
                    .GroupHash
                    .Values
                    .FirstOrDefault(o => o.Name == group.GroupName)
                    ?.Id ?? 0;
                if (id > 0)
                {
                    permissionIdList.GroupConvertCache[groupId] = id;
                    return id;
                }
            }
            return 0;
        }

        public static int ConvertedUserId(
            Context context,
            PermissionIdList permissionIdList,
            int userId)
        {
            if (permissionIdList.UserConvertCache.ContainsKey(userId))
            {
                return permissionIdList.UserConvertCache[userId];
            }
            var user = permissionIdList?.UserIdList.FirstOrDefault(x => x.UserId == userId);
            if (user == null)
            {
                return 0;
            }
            {
                var id = SiteInfo.TenantCaches.Get(context.TenantId)
                    .UserHash
                    .Values
                    .FirstOrDefault(o => o.LoginId == user.LoginId)
                    ?.Id ?? 0;
                if (id > 0)
                {
                    permissionIdList.UserConvertCache[userId] = id;
                    return id;
                }
            }
            if (!user.UserCode.IsNullOrEmpty())
            {
                var id = SiteInfo.TenantCaches.Get(context.TenantId)
                    .UserHash
                    .Values
                    .FirstOrDefault(o => o.UserCode == user.UserCode)
                    ?.Id ?? 0;
                if (id > 0)
                {
                    permissionIdList.UserConvertCache[userId] = id;
                    return id;
                }
            }
            {
                var id = SiteInfo.TenantCaches.Get(context.TenantId)
                    .UserHash
                    .Values
                    .FirstOrDefault(o => o.Name == user.Name)
                    ?.Id ?? 0;
                if (id > 0)
                {
                    permissionIdList.UserConvertCache[userId] = id;
                    return id;
                }
            }
            return 0;
        }
    }
}
