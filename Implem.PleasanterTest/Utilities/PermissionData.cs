using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Implem.PleasanterTest.Utilities
{
    public static class PermissionData
    {
        public enum PermissionInitTypes
        {
            Nothing,
            InheritSiteAllow,
            SiteAllow,
            RecordAllow,
            SiteEntry,
            SiteEntryAndRecordAllow
        }

        public enum PermissionIdTypes
        {
            Dept,
            Group,
            DeptGroup,
            User,
            AllUser
        }

        public static IEnumerable<PermissionInitTypes> GetPermissionInitTypes()
        {
            foreach (PermissionInitTypes permissionInitTypes in Enum.GetValues(typeof(PermissionInitTypes)))
            {
                yield return permissionInitTypes;
            }
        }

        public static (int DeptId, int GroupId, int UserId) PermissionIds(
            PermissionIdTypes permissionIdType,
            int userId)
        {
            var userModel = Initializer.Users.Get(userId);
            switch (permissionIdType)
            {
                case PermissionIdTypes.Dept:
                    return (
                        userModel.DeptId,
                        0,
                        0);
                case PermissionIdTypes.Group:
                    return (
                        0,
                        Initializer.GroupMembers.FirstOrDefault(o => o.UserId == userModel.UserId)?.GroupId ?? 0,
                        0);
                case PermissionIdTypes.DeptGroup:
                    return (
                        0,
                        Initializer.GroupMembers.FirstOrDefault(o => o.DeptId == userModel.DeptId)?.GroupId ?? 0,
                        0);
                case PermissionIdTypes.User:
                    return (
                        0,
                        0,
                        userModel.UserId);
                case PermissionIdTypes.AllUser:
                    return (
                        0,
                        0,
                        -1);
                default:
                    return (
                        0,
                        0,
                        0);
            }
        }

        public static void Init(
            string title,
            PermissionInitTypes permissionInitType,
            int deptId = 0,
            int groupId = 0,
            int userId = 0,
            Permissions.Types permissionType = (Permissions.Types)31)
        {
            var siteModel = Initializer.Sites.Get(title);
            Rds.ExecuteNonQuery(
                context: Initializer.Context,
                statements: Rds.PhysicalDeletePermissions(
                    where: Rds.PermissionsWhere()
                        .ReferenceId_In(sub: Rds.SelectItems(
                            column: Rds.ItemsColumn().ReferenceId(),
                            where: Rds.ItemsWhere()
                                .SiteId_In(sub: Rds.SelectSites(
                                    column: Rds.SitesColumn().SiteId(),
                                    where: Rds.SitesWhere().Add(or: Rds.SitesWhere()
                                        .SiteId(siteModel.SiteId)
                                        .ParentId(siteModel.SiteId))))))));
            switch (permissionInitType)
            {
                case PermissionInitTypes.Nothing:
                    break;
                case PermissionInitTypes.InheritSiteAllow:
                    SetInheritPermissions(
                        siteId: siteModel.SiteId,
                        deptId: deptId,
                        groupId: groupId,
                        userId: userId,
                        permissionType: permissionType);
                    break;
                case PermissionInitTypes.SiteAllow:
                    SetSitePermissions(
                        siteId: siteModel.SiteId,
                        deptId: deptId,
                        groupId: groupId,
                        userId: userId,
                        permissionType: permissionType);
                    break;
                case PermissionInitTypes.RecordAllow:
                    SetRecordPermissions(
                        siteId: siteModel.SiteId,
                        deptId: deptId,
                        groupId: groupId,
                        userId: userId,
                        permissionType: permissionType);
                    break;
                case PermissionInitTypes.SiteEntry:
                    SetSiteEntry(
                        siteId: siteModel.SiteId,
                        deptId: deptId,
                        groupId: groupId,
                        userId: userId);
                    break;
                case PermissionInitTypes.SiteEntryAndRecordAllow:
                    SetSiteEntryAndRecordAllow(
                        siteId: siteModel.SiteId,
                        deptId: deptId,
                        groupId: groupId,
                        userId: userId,
                        permissionType: permissionType);
                    break;
            }
        }

        private static void SetInheritPermissions(
            long siteId,
            int deptId,
            int groupId,
            int userId,
            Permissions.Types permissionType)
        {
            Rds.ExecuteNonQuery(
                context: Initializer.Context,
                statements: Rds.InsertPermissions(
                    param: Rds.PermissionsParam()
                        .ReferenceId(siteId)
                        .DeptId(deptId)
                        .GroupId(groupId)
                        .UserId(userId)
                        .PermissionType(permissionType)));
            Initializer.Sites
                .Values
                .Where(o => o.ParentId == siteId)
                .ForEach(siteModel => Rds.ExecuteNonQuery(
                    context: Initializer.Context,
                    statements: Rds.UpdateSites(
                        param: Rds.SitesParam().InheritPermission(raw: "\"ParentId\""),
                        where: Rds.SitesWhere()
                            .TenantId(Initializer.TenantId)
                            .SiteId(siteModel.SiteId))));
        }

        private static void SetSitePermissions(
            long siteId,
            int deptId,
            int groupId,
            int userId,
            Permissions.Types permissionType)
        {
            Rds.ExecuteNonQuery(
                context: Initializer.Context,
                statements: Rds.InsertPermissions(
                    param: Rds.PermissionsParam()
                        .ReferenceId(siteId)
                        .DeptId(deptId)
                        .GroupId(groupId)
                        .UserId(userId)
                        .PermissionType(permissionType)));
            Initializer.Sites
                .Values
                .Where(o => o.ParentId == siteId)
                .ForEach(siteModel =>
                {
                    Rds.ExecuteNonQuery(
                        context: Initializer.Context,
                        statements: Rds.InsertPermissions(
                            param: Rds.PermissionsParam()
                                .ReferenceId(siteModel.SiteId)
                                .DeptId(deptId)
                                .GroupId(groupId)
                                .UserId(userId)
                                .PermissionType(permissionType)));
                    Rds.ExecuteNonQuery(
                        context: Initializer.Context,
                        statements: Rds.UpdateSites(
                            param: Rds.SitesParam().InheritPermission(raw: "\"SiteId\""),
                            where: Rds.SitesWhere()
                                .TenantId(Initializer.TenantId)
                                .SiteId(siteModel.SiteId)));
                });
        }

        private static void SetRecordPermissions(
            long siteId,
            int deptId,
            int groupId,
            int userId,
            Permissions.Types permissionType)
        {
            Initializer.Sites
                .Values
                .Where(o => o.ParentId == siteId)
                .ForEach(siteModel =>
                {
                    Initializer.Items
                        .Values
                        .Where(o => o.SiteId == siteModel.SiteId)
                        .Where(o => o.SiteId != o.ReferenceId)
                        .ForEach(itemModel =>
                            Rds.ExecuteNonQuery(
                                context: Initializer.Context,
                                statements: Rds.InsertPermissions(
                                    param: Rds.PermissionsParam()
                                        .ReferenceId(itemModel.ReferenceId)
                                        .DeptId(deptId)
                                        .GroupId(groupId)
                                        .UserId(userId)
                                        .PermissionType(permissionType))));
                    Rds.ExecuteNonQuery(
                        context: Initializer.Context,
                        statements: Rds.UpdateSites(
                            param: Rds.SitesParam().InheritPermission(raw: "\"SiteId\""),
                            where: Rds.SitesWhere()
                                .TenantId(Initializer.TenantId)
                                .SiteId(siteModel.SiteId)));
                });
        }

        private static void SetSiteEntry(
            long siteId,
            int deptId,
            int groupId,
            int userId)
        {
            Initializer.Sites
                .Values
                .Where(o => o.ParentId == siteId)
                .ForEach(siteModel =>
                {
                    Rds.ExecuteNonQuery(
                        context: Initializer.Context,
                        statements: Rds.InsertPermissions(
                            param: Rds.PermissionsParam()
                                .ReferenceId(siteModel.SiteId)
                                .DeptId(deptId)
                                .GroupId(groupId)
                                .UserId(userId)
                                .PermissionType(0)));
                    Rds.ExecuteNonQuery(
                        context: Initializer.Context,
                        statements: Rds.UpdateSites(
                            param: Rds.SitesParam().InheritPermission(raw: "\"SiteId\""),
                            where: Rds.SitesWhere()
                                .TenantId(Initializer.TenantId)
                                .SiteId(siteModel.SiteId)));
                });
        }

        private static void SetSiteEntryAndRecordAllow(
            long siteId,
            int deptId,
            int groupId,
            int userId,
            Permissions.Types permissionType)
        {
            Initializer.Sites
                .Values
                .Where(o => o.ParentId == siteId)
                .ForEach(siteModel =>
                {
                    Rds.ExecuteNonQuery(
                        context: Initializer.Context,
                        statements: Rds.InsertPermissions(
                            param: Rds.PermissionsParam()
                                .ReferenceId(siteModel.SiteId)
                                .DeptId(deptId)
                                .GroupId(groupId)
                                .UserId(userId)
                                .PermissionType(0)));
                    Initializer.Items
                        .Values
                        .Where(o => o.SiteId == siteModel.SiteId)
                        .Where(o => o.SiteId != o.ReferenceId)
                        .ForEach(itemModel =>
                            Rds.ExecuteNonQuery(
                                context: Initializer.Context,
                                statements: Rds.InsertPermissions(
                                    param: Rds.PermissionsParam()
                                        .ReferenceId(itemModel.ReferenceId)
                                        .DeptId(deptId)
                                        .GroupId(groupId)
                                        .UserId(userId)
                                        .PermissionType(permissionType))));
                    Rds.ExecuteNonQuery(
                        context: Initializer.Context,
                        statements: Rds.UpdateSites(
                            param: Rds.SitesParam().InheritPermission(raw: "\"SiteId\""),
                            where: Rds.SitesWhere()
                                .TenantId(Initializer.TenantId)
                                .SiteId(siteModel.SiteId)));
                });
        }
    }
}
