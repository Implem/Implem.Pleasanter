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

        public static List<Permission> Get(List<string> formData, Types? type = null)
        {
            var data = new List<Permission>();
            formData?.ForEach(line =>
            {
                var part = line.Split(',');
                if (part.Count() == 3)
                {
                    data.Add(new Permission(
                        part[0],
                        part[1].ToInt(),
                        type != null
                            ? (Types)type
                            : (Types)part[2].ToLong()));
                }
            });
            return data;
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
            return Hash(
                dataRows: Rds.ExecuteTable(statements:
                    Rds.SelectPermissions(
                        distinct: true,
                        column: Rds.PermissionsColumn()
                            .ReferenceId()
                            .PermissionType(),
                        where: Rds.PermissionsWhere()
                            .ReferenceId_In(targets.Where(o => o != 0))
                            .Or(Rds.PermissionsWhere()
                                .GroupId_In(sub: Rds.SelectGroupMembers(
                                    column: Rds.GroupMembersColumn().GroupId(),
                                    where: Rds.GroupMembersWhere()
                                        .Add(raw: DeptOrUser("GroupMembers"))))
                                .Add(raw: DeptOrUser("Permissions")))))
                                    .AsEnumerable());
        }

        public static SqlWhereCollection SetCanReadWhere(
            SiteSettings ss,
            SqlWhereCollection where,
            bool checkPermission = true)
        {
            if (ss.IsSite() && ss.ReferenceType == "Sites")
            {
                where.Add(
                    tableName: "Sites",
                    raw: $"[Sites].[ParentId] in ({ss.SiteId})");
            }
            else
            {
                if (ss.ColumnHash.ContainsKey("SiteId"))
                {
                    if (ss.AllowedIntegratedSites != null)
                    {
                        where.Or(new SqlWhereCollection()
                            .Add(
                                tableName: ss.ReferenceType,
                                raw: "[{0}].[SiteId] in ({1})".Params(
                                    ss.ReferenceType, ss.AllowedIntegratedSites.Join()))
                            .CheckRecordPermission(ss, ss.IntegratedSites));
                    }
                    else
                    {
                        where.Add(
                            tableName: ss.ReferenceType,
                            raw: "[{0}].[SiteId] in ({1})".Params(
                                ss.ReferenceType, ss.SiteId));
                        if (!ss.CanRead(site: true) && checkPermission)
                        {
                            where.CheckRecordPermission(ss);
                        }
                    }
                }
            }
            return where;
        }

        public static SqlWhereCollection CanRead(
            this SqlWhereCollection where, string idColumnBracket)
        {
            return HasPrivilege()
                ? where
                : where
                    .Sites_TenantId(Sessions.TenantId())
                    .Or(or: new SqlWhereCollection()
                        .Add(
                            tableName: null,
                            raw: Def.Sql.CanReadSites)
                        .Add(
                            tableName: null,
                            subLeft: CheckRecordPermission(idColumnBracket),
                            _operator: null));
        }

        private static SqlWhereCollection CheckRecordPermission(
            this SqlWhereCollection where, SiteSettings ss, List<long> siteIdList = null)
        {
            return where.Add(
                tableName: ss.ReferenceType,
                subLeft: CheckRecordPermission(ss.IdColumnBracket(), siteIdList),
                _operator: null);
        }

        public static SqlExists CheckRecordPermission(
            string idColumnBracket, List<long> siteIdList = null)
        {
            return Rds.ExistsPermissions(
                where: Rds.PermissionsWhere()
                    .ReferenceId(raw: idColumnBracket)
                    .ReferenceId(
                        sub: Rds.SelectItems(
                        column: Rds.ItemsColumn().ReferenceId(),
                            where: Rds.ItemsWhere()
                                .ReferenceId(raw: "[Permissions].[ReferenceId]")
                                .SiteId_In(siteIdList)),
                        _using: siteIdList?.Any() == true)
                    .PermissionType(_operator: " & 1 = 1")
                    .Or(Rds.PermissionsWhere()
                        .GroupId_In(sub: Rds.SelectGroupMembers(
                            column: Rds.GroupMembersColumn().GroupId(),
                            where: Rds.GroupMembersWhere()
                                .Add(raw: DeptOrUser("GroupMembers"))))
                        .Add(raw: DeptOrUser("Permissions"))));
        }

        private static string DeptOrUser(string tableName)
        {
            return "((@_D <> 0 and [{0}].[DeptId]=@_D) or(@_U <> 0 and [{0}].[UserId]=@_U))"
                .Params(tableName);
        }

        private static Dictionary<long, Types> Hash(EnumerableRowCollection<DataRow> dataRows)
        {
            var hash = dataRows
                .Select(o => o["ReferenceId"].ToLong())
                .Distinct()
                .ToDictionary(o => o, o => Types.NotSet);
            dataRows.ForEach(dataRow =>
            {
                var key = dataRow["ReferenceId"].ToLong();
                hash[key] |= (Types)dataRow["PermissionType"].ToLong();
            });
            return hash;
        }

        public static Types Get(long siteId)
        {
            var data = Get(siteId.ToSingleList());
            return data.Count() == 1
                ? data.First().Value
                : Types.NotSet;
        }

        public static bool Can(long siteId, Types type)
        {
            return (Get(siteId) & type) == type || HasPrivilege();
        }

        public static bool CanRead(long siteId)
        {
            return (Get(siteId) & Types.Read) == Types.Read || HasPrivilege();
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

        public static IEnumerable<long> AllowSites(
            IEnumerable<long> sites, string referenceType = null)
        {
            return Rds.ExecuteTable(statements:
                Rds.SelectSites(
                    column: Rds.SitesColumn().SiteId(),
                    where: Rds.SitesWhere()
                        .TenantId(Sessions.TenantId())
                        .SiteId_In(sites)
                        .ReferenceType(referenceType, _using: referenceType != null)
                        .Add(
                            raw: Def.Sql.CanReadSites,
                            _using: !HasPrivilege())))
                                .AsEnumerable()
                                .Select(o => o["SiteId"].ToLong());
        }

        public static IEnumerable<Column> AllowedColumns(
            this IEnumerable<Column> columns,
            bool checkPermission,
            IEnumerable<ColumnAccessControl> readColumnAccessControls)
        {
            return columns
                .Where(o => !checkPermission || o.CanRead || readColumnAccessControls?.Any(p =>
                    p.ColumnName == o.ColumnName && p.AllowedUsers?.Any() == true) == true);
        }

        public static IEnumerable<string> AllowedColumns(SiteSettings ss)
        {
            return ss.Columns.AllowedColumns(
                checkPermission: true,
                readColumnAccessControls: ss.ReadColumnAccessControls)
                    .Select(o => o.ColumnName)
                    .ToList();
        }

        public static bool Allowed(
            this List<ColumnAccessControl> columnAccessControls,
            Column column,
            Types? type,
            List<string> mine)
        {
            return columnAccessControls?
                .FirstOrDefault(o => o.ColumnName == column.ColumnName)?
                .Allowed(type, mine) != false;
        }

        public static bool HasPermission(this SiteSettings ss)
        {
            return
                ss.PermissionType != null ||
                ss.ItemPermissionType != null ||
                HasPrivilege();
        }

        public static bool CanRead(this SiteSettings ss, bool site = false)
        {
            switch (Routes.Controller())
            {
                case "depts":
                    return CanManageTenant();
                case "groups":
                    return CanReadGroup();
                case "users":
                    return CanManageTenant() || Sessions.UserId() == Routes.Id();
                default:
                    return ss.Can(Types.Read, site);
            }
        }

        public static bool CanCreate(this SiteSettings ss, bool site = false)
        {
            switch (Routes.Controller())
            {
                case "depts":
                case "users":
                    return CanManageTenant();
                case "groups":
                    return CanEditGroup();
                default:
                    return ss.Can(Types.Create, site);
            }
        }

        public static bool CanUpdate(this SiteSettings ss, bool site = false)
        {
            switch (Routes.Controller())
            {
                case "depts":
                    return CanManageTenant();
                case "groups":
                    return CanEditGroup();
                case "users":
                    return CanManageTenant() || Sessions.UserId() == Routes.Id();
                default:
                    if (ss.ReferenceType == "Sites")
                    {
                        return ss.CanManageSite();
                    }
                    else
                    {
                        return ss.Can(Types.Update, site);
                    }
            }
        }

        public static bool CanMove(SiteSettings source, SiteSettings destination)
        {
            return source.CanUpdate() && destination.CanUpdate();
        }

        public static bool CanDelete(this SiteSettings ss, bool site = false)
        {
            switch (Routes.Controller())
            {
                case "depts":
                    return CanManageTenant();
                case "groups":
                    return CanEditGroup();
                case "users":
                    return CanManageTenant() &&
                        Sessions.UserId() != Routes.Id();
                default:
                    if (ss.ReferenceType == "Sites")
                    {
                        return ss.CanManageSite();
                    }
                    else
                    {
                        return ss.Can(Types.Delete, site);
                    }
            }
        }

        public static bool CanSendMail(this SiteSettings ss, bool site = false)
        {
            if (!Contract.Mail()) return false;
            switch (Routes.Controller())
            {
                case "depts":
                    return CanManageTenant();
                case "groups":
                    return CanEditGroup();
                case "users":
                    return CanManageTenant() || Sessions.UserId() == Routes.Id();
                default:
                    if (ss.ReferenceType == "Sites")
                    {
                        return ss.CanManageSite();
                    }
                    else
                    {
                        return ss.Can(Types.SendMail, site);
                    }
            }
        }

        public static bool CanImport(this SiteSettings ss, bool site = false)
        {
            return Contract.Import() && ss.Can(Types.Import, site);
        }

        public static bool CanExport(this SiteSettings ss, bool site = false)
        {
            return Contract.Export() && ss.Can(Types.Export, site);
        }

        public static bool CanManageSite(this SiteSettings ss, bool site = false)
        {
            return ss.Can(Types.ManageSite, site);
        }

        public static bool CanManagePermission(this SiteSettings ss, bool site = false)
        {
            return ss.Can(Types.ManagePermission, site);
        }

        public static ColumnPermissionTypes ColumnPermissionType(this Column self)
        {
            switch(self.SiteSettings.Context.Action)
            {
                case "new":
                    return self.CanCreate
                        ? ColumnPermissionTypes.Update
                        : self.CanRead
                            ? ColumnPermissionTypes.Read
                            : ColumnPermissionTypes.Deny;
                default:
                    return self.CanRead && self.CanUpdate
                        ? ColumnPermissionTypes.Update
                        : self.CanRead
                            ? ColumnPermissionTypes.Read
                            : ColumnPermissionTypes.Deny;
            }
        }

        public static bool CanManageTenant()
        {
            return Sessions.User().TenantManager || HasPrivilege();
        }

        public static bool CanReadGroup()
        {
            return 
                Sessions.UserSettings().DisableGroupAdmin != true &&
                (Routes.Id() == 0 ||
                CanManageTenant() ||
                Groups().Any() ||
                HasPrivilege());
        }

        public static bool CanEditGroup()
        {
            return
                Sessions.UserSettings().DisableGroupAdmin != true &&
                (Routes.Id() == 0 ||
                CanManageTenant() ||
                Groups().Any(o => o["Admin"].ToBool()) ||
                HasPrivilege());
        }

        private static bool Can(this SiteSettings ss, Types type, bool site)
        {
            return (ss.GetPermissionType(site) & type) == type || HasPrivilege();
        }

        private static EnumerableRowCollection<DataRow> Groups()
        {
            return Rds.ExecuteTable(statements:
                Rds.SelectGroupMembers(
                    column: Rds.GroupMembersColumn().Admin(),
                    where: Rds.GroupMembersWhere()
                        .GroupId(Routes.Id())
                        .Add(raw: DeptOrUser("GroupMembers"))))
                            .AsEnumerable();
        }

        public static SqlWhereCollection GroupMembersWhere()
        {
            return Rds.GroupMembersWhere().Add(raw: DeptOrUser("GroupMembers"));
        }

        public static Types? Admins(Types? type = Types.NotSet)
        {
            var user = Sessions.User();
            if (user.TenantManager) type |= Types.ManageTenant;
            if (user.ServiceManager) type |= Types.ManageService;
            return type;
        }

        public static bool HasPrivilege()
        {
            return Parameters.Security.PrivilegedUsers?.Contains(
                Sessions.User().LoginId) ?? false;
        }
    }
}