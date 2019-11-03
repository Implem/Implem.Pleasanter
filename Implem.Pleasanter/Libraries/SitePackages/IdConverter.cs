using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
namespace Implem.Pleasanter.Libraries.SitePackages
{

    public class IdConverter
    {
        public bool Convert = false;
        public int ConvertDeptId;
        public int ConvertGroupId;
        public int ConvertUserId;

        public IdConverter(
            Context context,
            long siteId,
            PermissionShortModel permissionShortModel,
            PermissionIdList permissionIdList,
            long convertSiteId)
        {
            Convert = true;
            ConvertDeptId = permissionShortModel.DeptId;
            ConvertGroupId = permissionShortModel.GroupId;
            ConvertUserId = permissionShortModel.UserId;
            if (permissionShortModel.DeptId > 0)
            {
                var deptId = Utilities.ConvertedDeptId(
                    context: context,
                    permissionIdList: permissionIdList,
                    deptId: permissionShortModel.DeptId);
                if (deptId > 0)
                {
                    ConvertDeptId = deptId;
                }
                else
                {
                    Convert = false;
                }
            }
            if (permissionShortModel.GroupId > 0)
            {
                var groupId = Utilities.ConvertedGroupId(
                    context: context,
                    permissionIdList: permissionIdList,
                    groupId: permissionShortModel.GroupId);
                if (groupId > 0)
                {
                    ConvertGroupId = groupId;
                }
                else
                {
                    Convert = false;
                }
            }
            if (permissionShortModel.UserId == -1)
            {
                var tenant = new TenantModel(
                    context: context,
                    ss: new SiteModel(
                        context: context,
                        siteId: siteId).SiteSettings);
                if (tenant.DisableAllUsersPermission == true)
                {
                    Convert = false;
                }
            }
            if (permissionShortModel.UserId > 0)
            {
                var userId = Utilities.ConvertedUserId(
                    context: context,
                    permissionIdList: permissionIdList,
                    userId: permissionShortModel.UserId);
                if (userId > 0)
                {
                    ConvertUserId = userId;
                }
                else
                {
                    Convert = false;
                }
            }
            if (context.UserId == ConvertUserId)
            {
                var check = Rds.ExecuteTable(
                    context: context,
                    statements: Rds.SelectPermissions(
                        column: Rds.PermissionsColumn().PermissionType(),
                        where: Rds.PermissionsWhere()
                            .ReferenceId(convertSiteId)
                            .DeptId(ConvertDeptId)
                            .GroupId(ConvertGroupId)
                            .UserId(ConvertUserId)));
                Convert = (check.Rows.Count == 0);
            }
        }
    }
}