using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
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

        public static Dictionary<long, Types> Get(Context context)
        {
            return Hash(
                dataRows: Repository.ExecuteTable(
                context: context,
                statements: new SqlStatement[]
                {
                    Rds.SelectSites(
                        distinct: true,
                        column: Rds.SitesColumn()
                            .SiteId(_as: "ReferenceId")
                            .Permissions_PermissionType(),
                        join: Rds.SitesJoinDefault()
                            .Add(new SqlJoin(
                                tableBracket: "\"Permissions\"",
                                joinType: SqlJoin.JoinTypes.Inner,
                                joinExpression: "\"Permissions\".\"ReferenceId\"=\"Sites\".\"InheritPermission\"")),
                        where: Rds.SitesWhere()
                            .TenantId(context.TenantId)
                            .PermissionsWhere()),
                    Rds.SelectPermissions(
                        column: Rds.PermissionsColumn()
                            .ReferenceId()
                            .PermissionType(),
                        where: Rds.PermissionsWhere()
                            .ReferenceId(context.Id)
                            .PermissionsWhere(),
                        unionType: Sqls.UnionTypes.UnionAll,
                        _using: context.Id > 0 && context.Id != context.SiteId),
                })
                    .AsEnumerable());
        }

        public static SqlWhereCollection SetCanReadWhere(
            Context context,
            SiteSettings ss,
            SqlWhereCollection where,
            bool checkPermission = true)
        {
            if (ss.IsSite(context: context) && ss.ReferenceType == "Sites")
            {
                where.Add(
                    tableName: "Sites",
                    raw: $"\"Sites\".\"ParentId\" in ({ss.SiteId})");
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
                                raw: "\"{0}\".\"SiteId\" in ({1})".Params(
                                    ss.ReferenceType, ss.AllowedIntegratedSites.Join()))
                            .CheckRecordPermission(ss, ss.IntegratedSites));
                    }
                    else
                    {
                        where.Add(
                            tableName: ss.ReferenceType,
                            raw: "\"{0}\".\"SiteId\" in ({1})".Params(
                                ss.ReferenceType, ss.SiteId));
                        if (!context.CanRead(ss: ss, site: true) && checkPermission)
                        {
                            where.CheckRecordPermission(ss);
                        }
                    }
                }
            }
            return where;
        }

        public static SqlWhereCollection SiteUserWhere(
            this Rds.UsersWhereCollection where, long siteId)
        {
            var deptRaw = "\"Users\".\"DeptId\" and \"Users\".\"DeptId\">0";
            var userRaw = "\"Users\".\"UserId\" and \"Users\".\"UserId\">0";
            return where.Add(
                subLeft: Rds.SelectPermissions(
                    column: Rds.PermissionsColumn()
                        .PermissionType(function: Sqls.Functions.Max),
                    where: Rds.PermissionsWhere()
                        .ReferenceId(siteId)
                        .Or(Rds.PermissionsWhere()
                            .DeptId(raw: deptRaw)
                            .Add(
                                subLeft: Rds.SelectGroupMembers(
                                    column: Rds.GroupMembersColumn()
                                        .GroupMembersCount(),
                                    where: Rds.GroupMembersWhere()
                                        .GroupId(raw: "\"Permissions\".\"GroupId\"")
                                        .Or(Rds.GroupMembersWhere()
                                            .DeptId(raw: deptRaw)
                                            .UserId(raw: userRaw))
                                        .Add(raw: "\"Permissions\".\"GroupId\">0")),
                                _operator: ">0")
                            .UserId(raw: userRaw)
                            .UserId(-1))),
                _operator: ">0");
        }

        public static SqlWhereCollection CanRead(
            this SqlWhereCollection where,
            Context context,
            string idColumnBracket,
            bool _using = true)
        {
            return _using && !context.HasPrivilege
                ? where
                    .Sites_TenantId(context.TenantId)
                    .Or(or: new SqlWhereCollection()
                        .Add(
                            tableName: null,
                            raw: Def.Sql.CanReadSites)
                        .Add(
                            tableName: null,
                            subLeft: CheckRecordPermission(idColumnBracket),
                            _operator: null))
                : where;
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
                                .ReferenceId(raw: "\"Permissions\".\"ReferenceId\"")
                                .SiteId_In(siteIdList)),
                        _using: siteIdList?.Any() == true)
                    .PermissionType(_operator: " & 1 = 1")
                    .PermissionsWhere());
        }

        private static SqlWhereCollection PermissionsWhere(this SqlWhereCollection where)
        {
            return where.Or(Rds.PermissionsWhere()
                .GroupId_In(sub: Rds.SelectGroupMembers(
                    column: Rds.GroupMembersColumn().GroupId(),
                    where: Rds.GroupMembersWhere()
                        .Add(raw: DeptOrUser("GroupMembers"))))
                .Add(raw: DeptOrUser("Permissions")));
        }

        public static string DeptOrUser(string tableName)
        {
            return $"(({Parameters.Parameter.SqlParameterPrefix}D<>0 and \"{{0}}\".\"DeptId\"={Parameters.Parameter.SqlParameterPrefix}D)or({Parameters.Parameter.SqlParameterPrefix}U<>0 and \"{{0}}\".\"UserId\"={Parameters.Parameter.SqlParameterPrefix}U)or(\"{{0}}\".\"UserId\"=-1))"
                .Params(tableName);
        }

        private static Dictionary<long, Types> Hash(EnumerableRowCollection<DataRow> dataRows)
        {
            var hash = dataRows
                .Select(o => o.Long("ReferenceId"))
                .Distinct()
                .ToDictionary(o => o, o => Types.NotSet);
            dataRows.ForEach(dataRow =>
            {
                var key = dataRow.Long("ReferenceId");
                hash[key] |= (Types)dataRow.Long("PermissionType");
            });
            return hash;
        }

        public static Types Get(Context context, long siteId)
        {
            return context.PermissionHash.Get(siteId);
        }

        public static bool Can(Context context, long siteId, Types type)
        {
            return (Get(context: context, siteId: siteId) & type) == type
                || context.HasPrivilege;
        }

        public static bool CanRead(Context context, long siteId)
        {
            return (Get(context: context, siteId: siteId) & Types.Read) == Types.Read
                || context.HasPrivilege;
        }

        public static long InheritPermission(Context context, long id)
        {
            return Repository.ExecuteScalar_long(
                context: context,
                statements: Rds.SelectSites(
                    column: Rds.SitesColumn().InheritPermission(),
                    where: Rds.SitesWhere()
                        .SiteId(sub: Rds.SelectItems(
                            column: Rds.ItemsColumn().SiteId(),
                            where: Rds.ItemsWhere().ReferenceId(id)))));
        }

        public static IEnumerable<long> AllowSites(
            Context context, IEnumerable<long> sites, string referenceType = null)
        {
            return Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectSites(
                    column: Rds.SitesColumn().SiteId(),
                    where: Rds.SitesWhere()
                        .TenantId(context.TenantId)
                        .SiteId_In(sites)
                        .ReferenceType(referenceType, _using: referenceType != null)
                        .Add(
                            raw: Def.Sql.CanReadSites,
                            _using: !context.HasPrivilege)))
                                .AsEnumerable()
                                .Select(o => o["SiteId"].ToLong());
        }

        public static IEnumerable<Column> AllowedColumns(
            this IEnumerable<Column> columns, bool checkPermission)
        {
            return columns.Where(o => !checkPermission || o.CanRead);
        }

        public static IEnumerable<string> AllowedColumns(SiteSettings ss)
        {
            return ss.Columns.AllowedColumns(checkPermission: true)
                .Select(o => o.ColumnName)
                .ToList();
        }

        public static bool Allowed(
            this List<ColumnAccessControl> columnAccessControls,
            Context context,
            SiteSettings ss,
            Column column,
            Types? type,
            List<string> mine)
        {
            return columnAccessControls?
                .FirstOrDefault(o => o.ColumnName == column.Name)?
                .Allowed(
                    context: context,
                    ss: ss,
                    type: type,
                    mine: mine) != false;
        }

        public static bool HasPermission(this Context context, SiteSettings ss)
        {
            return ss.PermissionType != null
                || ss.ItemPermissionType != null
                || context.HasPrivilege;
        }

        public static bool CanRead(this Context context, SiteSettings ss, bool site = false)
        {
            switch (context.Controller)
            {
                case "tenants":
                case "depts":
                    return CanManageTenant(context: context)
                        || context.UserSettings?.EnableManageTenant == true;
                case "groups":
                    return CanReadGroup(context: context);
                case "users":
                    return CanManageTenant(context: context)
                        || context.UserId == context.Id;
                default:
                    return context.Can(ss: ss, type: Types.Read, site: site);
            }
        }

        public static bool CanCreate(this Context context, SiteSettings ss, bool site = false)
        {
            switch (context.Controller)
            {
                case "tenants":
                    return false;
                case "depts":
                case "users":
                    return CanManageTenant(context: context);
                case "groups":
                    return CanEditGroup(context: context);
                case "versions":
                    return false;
                default:
                    return context.Can(ss: ss, type: Types.Create, site: site);
            }
        }

        public static bool CanUpdate(this Context context, SiteSettings ss, bool site = false)
        {
            switch (context.Controller)
            {
                case "tenants":
                case "depts":
                    return CanManageTenant(context: context)
                        || context.UserSettings?.EnableManageTenant == true;
                case "groups":
                    return CanEditGroup(context: context);
                case "users":
                    return CanManageTenant(context: context)
                        || context.UserId == context.Id;
                default:
                    if (ss.ReferenceType == "Sites")
                    {
                        return context.CanManageSite(ss: ss);
                    }
                    else
                    {
                        return context.Can(ss: ss, type: Types.Update, site: site);
                    }
            }
        }

        public static bool CanMove(Context context, SiteSettings source, SiteSettings destination)
        {
            return context.CanUpdate(ss: source)
                && context.CanUpdate(ss: destination);
        }

        public static bool CanDelete(this Context context, SiteSettings ss, bool site = false)
        {
            switch (context.Controller)
            {
                case "tenants":
                    return false;
                case "depts":
                    return CanManageTenant(context: context);
                case "groups":
                    return CanEditGroup(context: context);
                case "users":
                    return CanManageTenant(context: context)
                        && context.UserId != context.Id;
                default:
                    if (ss.ReferenceType == "Sites")
                    {
                        return context.CanManageSite(ss: ss);
                    }
                    else
                    {
                        return context.Can(ss: ss, type: Types.Delete, site: site);
                    }
            }
        }

        public static bool CanSendMail(this Context context, SiteSettings ss, bool site = false)
        {
            if (context.ContractSettings.Mail == false) return false;
            switch (Strings.CoalesceEmpty(
                context.Forms.Get("Controller"),
                context.Controller))
            {
                case "tenants":
                    return false;
                case "depts":
                    return CanManageTenant(context: context);
                case "groups":
                    return CanEditGroup(context: context);
                case "users":
                    return CanManageTenant(context: context)
                        || context.UserId == context.Id;
                default:
                    if (ss.ReferenceType == "Sites")
                    {
                        return context.CanManageSite(ss: ss);
                    }
                    else
                    {
                        return context.Can(ss: ss, type: Types.SendMail, site: site);
                    }
            }
        }

        public static bool CanImport(this Context context, SiteSettings ss, bool site = false)
        {
            if (context.ContractSettings.Import == false) return false;
            switch (context.Controller)
            {
                case "tenants":
                case "depts":
                case "groups":
                    return false;
                case "users":
                    return CanManageTenant(context: context);
                default:
                    return context.Can(ss: ss, type: Types.Import, site: site);
            }
        }

        public static bool CanExport(this Context context, SiteSettings ss, bool site = false)
        {
            if (context.ContractSettings.Import == false) return false;
            switch (context.Controller)
            {
                case "tenants":
                case "depts":
                case "groups":
                    return false;
                case "users":
                    return CanManageTenant(context: context);
                default:
                    return context.Can(ss: ss, type: Types.Export, site: site);
            }
        }

        public static bool CanManageSite(this Context context, SiteSettings ss, bool site = false)
        {
            return context.Can(ss: ss, type: Types.ManageSite, site: site);
        }

        public static bool CanManagePermission(this Context context, SiteSettings ss, bool site = false)
        {
            return context.Can(ss: ss, type: Types.ManagePermission, site: site);
        }

        public static ColumnPermissionTypes ColumnPermissionType(this Column self, Context context)
        {
            switch(context.Action)
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

        public static bool CanManageTenant(Context context)
        {
            return context.User?.TenantManager == true
                || context.HasPrivilege;
        }

        public static bool CanReadGroup(Context context)
        {
            return 
                context.UserSettings?.DisableGroupAdmin != true &&
                (context.Id == 0 ||
                CanManageTenant(context: context) ||
                Groups(context: context).Any() ||
                context.HasPrivilege);
        }

        public static bool CanEditGroup(Context context)
        {
            return
                context.UserSettings?.DisableGroupAdmin != true
                && (context.Id == 0
                || CanManageTenant(context: context)
                || Groups(context: context).Any(o => o["Admin"].ToBool())
                || context.HasPrivilege);
        }

        private static bool Can(this Context context, SiteSettings ss, Types type, bool site)
        {
            if (ss.Locked())
            {
                if ((type & Types.Create) == Types.Create) return false;
                if ((type & Types.Update) == Types.Update) return false;
                if ((type & Types.Delete) == Types.Delete) return false;
                if ((type & Types.SendMail) == Types.SendMail) return false;
                if ((type & Types.Import) == Types.Import) return false;
            }
            return (ss.GetPermissionType(site) & type) == type
                || context.HasPrivilege;
        }

        private static EnumerableRowCollection<DataRow> Groups(Context context)
        {
            return Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectGroupMembers(
                    column: Rds.GroupMembersColumn().Admin(),
                    where: Rds.GroupMembersWhere()
                        .GroupId(context.Id)
                        .Add(raw: DeptOrUser("GroupMembers"))))
                            .AsEnumerable();
        }

        public static Types? Admins(Context context, Types? type = Types.NotSet)
        {
            if (context.User?.TenantManager == true) type |= Types.ManageTenant;
            if (context.User?.ServiceManager == true) type |= Types.ManageService;
            return type;
        }

        public static bool PrivilegedUsers(string loginId)
        {
            return loginId != null &&
                Parameters.Security.PrivilegedUsers?.Contains(loginId) == true;
        }
    }
}