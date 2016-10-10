using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
namespace Implem.Pleasanter.Libraries.Security
{
    public static class Permissions
    {
        public enum Types : long
        {
                                                // |-Sys--||Tenant||-General------|
            NotSet = 0,                         // 00000000000000000000000000000000
            Read = 1,                           // 00000000000000000000000000000001
            Create = 2,                         // 00000000000000000000000000000010
            Update = 4,                         // 00000000000000000000000000000100
            Delete = 8,                         // 00000000000000000000000000001000
            DownloadFile = 16,                  // 00000000000000000000000000010000
            Export = 32,                        // 00000000000000000000000000100000
            Import = 64,                        // 00000000000000000000000001000000
            EditSite = 128,                     // 00000000000000000000000010000000
            EditPermission = 256,               // 00000000000000000000000100000000
            EditProfile = 4194304,              // 00000000010000000000000000000000
            EditTenant = 8388608,               // 00000000100000000000000000000000
            EditService = 2147483648,           // 10000000000000000000000000000000

            ReadOnly = 17,                      // 00000000000000000000000000010001
            ReadWrite = 31,                     // 00000000000000000000000000011111
            Leader = 255,                       // 00000000000000000000000011111111
            Manager = 511,                      // 00000000000000000000000111111111
            TenantAdmin = 16711680,             // 00000000111111110000000000000000
            ServiceAdmin = 4278190080           // 11111111000000000000000000000000
        }

        public static Types Get(string name)
        {
            switch (name)
            {
                case "NotSet": return Types.NotSet;
                case "Read": return Types.Read;
                case "Create": return Types.Create;
                case "Update": return Types.Update;
                case "Delete": return Types.Delete;
                case "DownloadFile": return Types.DownloadFile;
                case "Export": return Types.Export;
                case "Import": return Types.Import;
                case "EditSite": return Types.EditSite;
                case "EditPermission": return Types.EditPermission;
                case "EditProfile": return Types.EditProfile;
                case "EditTenant": return Types.EditTenant;
                case "EditService": return Types.EditService;
                default: return Types.NotSet;
            }
        }

        public enum ColumnPermissionTypes
        {
            Deny,
            Read,
            Update
        }

        public static Types CurrentType()
        {
            var permissionType = Types.NotSet;
            var userModel = Sessions.User();
            if (userModel.TenantAdmin)
            {
                permissionType |= Types.TenantAdmin;
            }
            if (userModel.ServiceAdmin)
            {
                permissionType |= Types.TenantAdmin;
            }
            return permissionType;
        }

        public static Types GetBySiteId(long siteId)
        {
            var userModel = Sessions.User();
            return ((Types)Rds.ExecuteScalar_long(statements:
                Rds.SelectPermissions(
                    column: Rds.PermissionsColumn()
                        .PermissionType(),
                    where: Rds.PermissionsWhere()
                        .ReferenceType("Sites")
                        .ReferenceId(siteId)
                        .Add(raw: "[t0].[DeptId] = {0} or [t0].[UserId] = {1}".Params(
                            userModel.DeptId, userModel.Id))))).Admins();
        }

        public static bool CanRead(this Types self)
        {
            switch (Routes.Controller().ToLower())
            {
                case "depts":
                    return self.CanEditTenant();
                case "users":
                    return self.CanEditTenant() ||
                        Sessions.UserId() == Routes.Id();
                default:
                    return (self & Types.Read) != 0;
            }
        }

        public static bool CanCreate(this Types self)
        {
            switch (Routes.Controller().ToLower())
            {
                case "depts":
                case "users":
                    return self.CanEditTenant();
                default:
                    return (self & Types.Create) != 0;
            }
        }

        public static bool CanUpdate(this Types self)
        {
            switch (Routes.Controller().ToLower())
            {
                case "depts":
                    return self.CanEditTenant();
                case "users":
                    return self.CanEditTenant() ||
                        Sessions.UserId() == Routes.Id();
                default:
                    return (self & Types.Update) != 0;
            }
        }

        public static bool CanMove(Types source, Types destination)
        {
            return source.CanUpdate() && destination.CanUpdate();
        }

        public static bool CanDelete(this Types self)
        {
            switch (Routes.Controller().ToLower())
            {
                case "depts":
                    return self.CanEditTenant();
                case "users":
                    return self.CanEditTenant() &&
                        Sessions.UserId() != Routes.Id();
                default:
                    return (self & Types.Delete) != 0;
            }
        }

        public static bool CanDownloadFile(this Types self)
        {
            return (self & Types.DownloadFile) != 0;
        }

        public static bool CanExport(this Types self)
        {
            return (self & Types.Export) != 0;
        }

        public static bool CanImport(this Types self)
        {
            return (self & Types.Import) != 0;
        }

        public static bool CanEditSite(this Types self)
        {
            return (self & Types.EditSite) != 0;
        }

        public static bool CanEditPermission(this Types self)
        {
            return (self & Types.EditPermission) != 0;
        }

        public static bool CanEditProfile(this Types self)
        {
            return (self & Types.EditProfile) != 0;
        }

        public static bool CanEditTenant(this Types self)
        {
            return (self & Types.EditTenant) != 0;
        }

        public static bool CanEditSys(this Types self)
        {
            return (self & Types.EditService) != 0;
        }

        public static ColumnPermissionTypes ColumnPermissionType(
            this Column self, Types permissionType)
        {
            switch(Url.RouteData("action").ToLower())
            {
                case "new":
                    return
                        self.CanCreate(permissionType)
                            ? ColumnPermissionTypes.Update
                            : self.CanRead(permissionType)
                                ? ColumnPermissionTypes.Read
                                : ColumnPermissionTypes.Deny;
                default:
                    return
                        self.CanUpdate(permissionType)
                            ? ColumnPermissionTypes.Update
                            : self.CanRead(permissionType)
                                ? ColumnPermissionTypes.Read
                                : ColumnPermissionTypes.Deny;
            }
        }

        public static bool CanRead(
            this Column self, Types permissionType)
        {
            return
                self.ReadPermission == 0 ||
                (self.ReadPermission & Admins(permissionType)) != 0;
        }

        public static bool CanCreate(
            this Column self, Types permissionType)
        {
            return
                self.CreatePermission == 0 ||
                (self.CreatePermission & Admins(permissionType)) != 0;
        }

        public static bool CanUpdate(
            this Column self, Types permissionType)
        {
            return
                self.UpdatePermission == 0 ||
                (self.UpdatePermission & Admins(permissionType)) != 0;
        }

        public static Types Admins()
        {
            return Types.NotSet.Admins();
        }

        public static Types Admins(this Types permissionType)
        {
            var userModel = Sessions.User();
            var tenantAdmin = userModel.TenantAdmin
                ? Types.TenantAdmin
                : Types.NotSet;
            var sysAdmin = userModel.ServiceAdmin
                ? Types.ServiceAdmin
                : Types.NotSet;
            return permissionType | tenantAdmin | sysAdmin;
        }
    }
}