using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Security
{
    public static class Permissions
    {
        public enum Types : long
        {
            NotSet = 0,                         // 00000000000000000000000000000000
            Read = 1,                           // 00000000000000000000000000000001
            Create = 2,                         // 00000000000000000000000000000010
            Update = 4,                         // 00000000000000000000000000000100
            Delete = 8,                         // 00000000000000000000000000001000
            SendMail = 16,                      // 00000000000000000000000000010000
            Export = 32,                        // 00000000000000000000000000100000
            Import = 64,                        // 00000000000000000000000001000000
            ManageSite = 128,                   // 00000000000000000000000010000000
            ManagePermission = 256,             // 00000000000000000000000100000000
            ManageTenant = 1073741824,          // 01000000000000000000000000000000
            ManageService = 2147483648,         // 10000000000000000000000000000000
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
                case "SendMail": return Types.SendMail;
                case "Export": return Types.Export;
                case "Import": return Types.Import;
                case "ManageSite": return Types.ManageSite;
                case "ManagePermission": return Types.ManagePermission;
                case "ManageTenant": return Types.ManageTenant;
                case "ManageService": return Types.ManageService;
                default: return Types.NotSet;
            }
        }

        public static Types General()
        {
            return (Types)Parameters.Permissions.General;
        }

        public static Types Manager()
        {
            return (Types)Parameters.Permissions.Manager;
        }

        public enum ColumnPermissionTypes
        {
            Deny,
            Read,
            Update
        }

        public static Dictionary<long, Types> Get(IEnumerable<long> targets)
        {
            return Hash(Rds.ExecuteTable(statements:
                Rds.SelectPermissions(
                    distinct: true,
                    column: Rds.PermissionsColumn()
                        .ReferenceId()
                        .PermissionType(),
                    where: Rds.PermissionsWhere()
                        .ReferenceType("Sites")
                        .ReferenceId_In(targets)
                        .Or(Rds.PermissionsWhere()
                            .GroupId_In(sub: Rds.SelectGroupMembers(
                                column: Rds.GroupMembersColumn().GroupId(),
                                where: Rds.GroupMembersWhere()
                                    .Add(raw: DeptOrUser("GroupMembers"))))
                            .Add(raw: DeptOrUser("Permissions")))))
                                .AsEnumerable());
        }

        public static SqlWhereCollection CanRead(SiteSettings ss, SqlWhereCollection where)
        {
            return !ss.CanRead()
                ? where.Add(
                    subLeft: Rds.SelectPermissions(
                        column: Rds.PermissionsColumn().PermissionType(),
                        where: Rds.PermissionsWhere()
                            .ReferenceType(ss.ReferenceType)
                            .ReferenceId(raw: ss.IdColumnBracket())
                            .Or(Rds.PermissionsWhere()
                                .GroupId_In(sub: Rds.SelectGroupMembers(
                                    column: Rds.GroupMembersColumn().GroupId(),
                                    where: Rds.GroupMembersWhere()
                                        .Add(raw: DeptOrUser("GroupMembers"))))
                                .Add(raw: DeptOrUser("Permissions")))),
                    _operator: " & 1 = 1")
                : where;
        }

        private static string DeptOrUser(string tableName)
        {
            return "([{0}].[DeptId]=@_D or [{0}].[UserId]=@_U)".Params(tableName);
        }

        private static Dictionary<long, Types> Hash(EnumerableRowCollection<DataRow> dataRows)
        {
            var hash = dataRows
                .Select(o => o["ReferenceId"].ToLong())
                .Distinct()
                .ToDictionary(o => o, o => Types.NotSet);
            dataRows.ForEach(dataRow =>
                hash[dataRow["ReferenceId"].ToLong()] |=
                    (Types)dataRow["PermissionType"].ToLong());
            return hash;
        }

        public static Types Get(string controller, long id)
        {
            return controller.ToLower() == "items"
                ? Get(InheritPermission(id))
                : Types.NotSet;
        }

        public static Types Get(long siteId)
        {
            var data = Get(new List<long> { siteId });
            return data.Count() == 1
                ? data.First().Value
                : Types.NotSet;
        }

        public static long InheritPermission(long id)
        {
            return Rds.ExecuteScalar_long(statements:
                Rds.SelectSites(
                    column: Rds.SitesColumn().InheritPermission(),
                    where: Rds.SitesWhere()
                        .SiteId(sub: Rds.SelectItems(
                            column: Rds.ItemsColumn().SiteId(),
                            where: Rds.ItemsWhere().ReferenceId(id)))));
        }

        public static IEnumerable<long> AllowSites(IEnumerable<long> sites)
        {
            var user = Sessions.User();
            return Rds.ExecuteTable(statements:
                Rds.SelectSites(
                    column: Rds.SitesColumn().SiteId(),
                    where: Rds.SitesWhere()
                        .TenantId(Sessions.TenantId())
                        .SiteId_In(sites)
                        .Add(raw: Def.Sql.CanRead)))
                            .AsEnumerable()
                            .Select(o => o["SiteId"].ToLong());
        }

        public static bool HasPermission(this SiteSettings ss)
        {
            return ss.PermissionType != null;
        }

        public static bool CanRead(this SiteSettings ss)
        {
            switch (Routes.Controller().ToLower())
            {
                case "depts":
                    return CanManageTenant();
                case "groups":
                    return CanReadGroup();
                case "users":
                    return CanManageTenant() ||
                        Sessions.UserId() == Routes.Id();
                default:
                    return (ss.PermissionType & Types.Read) != 0;
            }
        }

        public static bool CanCreate(this SiteSettings ss)
        {
            switch (Routes.Controller().ToLower())
            {
                case "depts":
                case "users":
                    return CanManageTenant();
                case "groups":
                    return CanEditGroup();
                default:
                    return (ss.PermissionType & Types.Create) != 0;
            }
        }

        public static bool CanUpdate(this SiteSettings ss)
        {
            switch (Routes.Controller().ToLower())
            {
                case "depts":
                    return CanManageTenant();
                case "groups":
                    return CanEditGroup();
                case "users":
                    return CanManageTenant() ||
                        Sessions.UserId() == Routes.Id();
                default:
                    return (ss.PermissionType & Types.Update) != 0;
            }
        }

        public static bool CanMove(SiteSettings source, SiteSettings destination)
        {
            return source.CanUpdate() && destination.CanUpdate();
        }

        public static bool CanDelete(this SiteSettings ss)
        {
            switch (Routes.Controller().ToLower())
            {
                case "depts":
                    return CanManageTenant();
                case "groups":
                    return CanEditGroup();
                case "users":
                    return CanManageTenant() &&
                        Sessions.UserId() != Routes.Id();
                default:
                    return (ss.PermissionType & Types.Delete) != 0;
            }
        }

        public static bool CanSendMail(this SiteSettings ss)
        {
            switch (ss.ReferenceType.ToLower())
            {
                case "depts":
                    return CanManageTenant();
                case "groups":
                    return CanEditGroup();
                case "users":
                    return CanManageTenant() ||
                        Sessions.UserId() == Routes.Id();
                default:
                    return (ss.PermissionType & Types.SendMail) != 0;
            }
        }

        public static bool CanExport(this SiteSettings ss)
        {
            return (ss.PermissionType & Types.Export) != 0;
        }

        public static bool CanImport(this SiteSettings ss)
        {
            return (ss.PermissionType & Types.Import) != 0;
        }

        public static bool CanManageSite(this SiteSettings ss)
        {
            return (ss.PermissionType & Types.ManageSite) != 0;
        }

        public static bool CanManagePermission(this SiteSettings ss)
        {
            return (ss.PermissionType & Types.ManagePermission) != 0;
        }

        public static ColumnPermissionTypes ColumnPermissionType(
            this Column self, SiteSettings ss)
        {
            switch(Url.RouteData("action").ToLower())
            {
                case "new":
                    return
                        self.CanCreate(ss)
                            ? ColumnPermissionTypes.Update
                            : self.CanRead(ss)
                                ? ColumnPermissionTypes.Read
                                : ColumnPermissionTypes.Deny;
                default:
                    return
                        self.CanUpdate(ss)
                            ? ColumnPermissionTypes.Update
                            : self.CanRead(ss)
                                ? ColumnPermissionTypes.Read
                                : ColumnPermissionTypes.Deny;
            }
        }

        public static bool CanRead(this Column self, SiteSettings ss)
        {
            return
                self.ReadPermission == 0 ||
                (self.ReadPermission & Admins(ss.PermissionType)) != 0;
        }

        public static bool CanCreate(this Column self, SiteSettings ss)
        {
            return
                self.CreatePermission == 0 ||
                (self.CreatePermission & Admins(ss.PermissionType)) != 0;
        }

        public static bool CanUpdate(this Column self, SiteSettings ss)
        {
            return
                self.UpdatePermission == 0 ||
                (self.UpdatePermission & Admins(ss.PermissionType)) != 0;
        }

        public static bool CanManageTenant()
        {
            return Sessions.User().TenantManager;
        }

        public static bool CanReadGroup()
        {
            return Routes.Id() == 0 || CanManageTenant() || Groups().Any();
        }

        public static bool CanEditGroup()
        {
            return Routes.Id() == 0 || CanManageTenant() || Groups().Any(o => o["Admin"].ToBool());
        }

        private static EnumerableRowCollection<DataRow> Groups()
        {
            return Rds.ExecuteTable(statements:
                Rds.SelectGroupMembers(
                    column: Rds.GroupMembersColumn().Admin(),
                    where: Rds.GroupMembersWhere()
                        .GroupId(Routes.Id())
                        .Or(Rds.GroupMembersWhere()
                            .DeptId(Sessions.DeptId())
                            .UserId(Sessions.UserId()))))
                                .AsEnumerable();
        }

        public static Types? Admins(this Types? type)
        {
            var user = Sessions.User();
            if (user.TenantManager) type |= Types.ManageTenant;
            if (user.ServiceManager) type |= Types.ManageService;
            return type;
        }
    }
}