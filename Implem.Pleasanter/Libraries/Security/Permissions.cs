using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.ServerScripts;
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
            if (context.Authenticated)
            {
                var statements = new List<SqlStatement>()
                {
                    new SqlStatement(
                        context.Sqls.GetPermissions,
                        new SqlParamCollection())
                };
                if (context.Id > 0 && context.Id != context.SiteId)
                {
                    statements.Add(new SqlStatement(
                        context.Sqls.GetPermissionsById.Replace("@ReferenceId", context.Id.ToStr()),
                        new SqlParamCollection()));
                }
                return Hash(
                    dataRows: Repository.ExecuteTable(
                    context: context,
                    statements: statements.ToArray())
                        .AsEnumerable());
            }
            else
            {
                return new Dictionary<long, Types>();
            }
        }

        public static Types GetById(Context context, long id)
        {
            var type = Repository.ExecuteScalar_int(
                context: context,
                statements: new SqlStatement[]
                {
                    Rds.SelectPermissions(
                        column: Rds.PermissionsColumn()
                            .PermissionType(),
                        where: Rds.PermissionsWhere()
                            .ReferenceId(id)
                            .PermissionsWhere(context: context)),
                });
            return (Types)type;
        }

        public static SqlWhereCollection SetPermissionsWhere(
            Context context,
            SiteSettings ss,
            SqlWhereCollection where,
            Types permissionType,
            bool checkPermission = true)
        {
            if (ss.ColumnHash.ContainsKey("TenantId"))
            {
                where.Add(
                    tableName: ss.ReferenceType,
                    raw: $"\"{ss.ReferenceType}\".\"TenantId\"={context.TenantId}");
            }
            if (ss.IsSite(context: context) && ss.ReferenceType == "Sites")
            {
                where.Add(
                    tableName: "Sites",
                    raw: $"\"Sites\".\"ParentId\"={ss.SiteId}");
            }
            else
            {
                if (ss.ColumnHash.ContainsKey("SiteId") && ss.SiteId > 0)
                {
                    if (ss.AllowedIntegratedSites == null)
                    {
                        where.Add(
                            tableName: ss.ReferenceType,
                            raw: $"\"{ss.ReferenceType}\".\"SiteId\"={ss.SiteId}");
                        // Itemsの場合にはサイトの権限のチェック、それ以外の場合にはテナント管理などの権限をチェック
                        if (!CheckSitePermission(
                            context: context,
                            ss: ss,
                            permissionType: permissionType) && checkPermission)
                        {
                            // サイトの権限をもっていない場合にはレコードのアクセス権が存在するか確認するWhereが追加になる
                            where.CheckRecordPermission(
                                context: context,
                                ss: ss,
                                // すでに保有しているサイトの権限以外の権限をレコードの権限としてもっているか(XOR)確認
                                permissionType: permissionType ^ ((ss.PermissionType ?? Types.NotSet) & permissionType));
                        }
                    }
                    else
                    {
                        var integratedSites = ss.GetIntegratedSites(context: context);
                        var denySites = integratedSites
                           .Where(siteId => !ss.AllowedIntegratedSites.Contains(siteId))
                           .ToList();
                        denySites = denySites.Any()
                            ? Repository.ExecuteTable(
                                context: context,
                                statements: Rds.SelectSites(
                                    column: Rds.SitesColumn().SiteId(),
                                    where: Rds.SitesWhere()
                                        .TenantId(context.TenantId)
                                        .SiteId_In(denySites)))
                                            .AsEnumerable()
                                            .Select(dataRow => dataRow.Long("SiteId"))
                                            .ToList()
                            : new List<long>();
                        if (!denySites.Any())
                        {
                            where.Add(
                                tableName: ss.ReferenceType,
                                raw: $"\"{ss.ReferenceType}\".\"SiteId\" in ({ss.AllowedIntegratedSites.Join()})");
                        }
                        else
                        {
                            // サイト統合で一部のサイトにサイトの読み取り権限が無い場合、下記の条件を加える
                            // (1) サイトの読み取り権限があるレコード
                            // (2) レコードの読み取り権限があるレコード
                            where.Add(raw: context.Sqls.IntegratedSitesPermissionsWhere(
                                tableName: ss.ReferenceType,
                                sites: integratedSites));
                        }
                    }
                }
            }
            return where;
        }

        private static bool CheckSitePermission(Context context, SiteSettings ss, Types permissionType)
        {
            return context.Controller == "items"
                ? context.ItemsCan(
                    ss: ss,
                    type: permissionType,
                    site: true)
                : context.CanRead(
                    ss: ss,
                    site: true);
        }

        public static SqlWhereCollection SiteDeptWhere(
            this Rds.DeptsWhereCollection where,
            Context context,
            long siteId,
            bool _using = true)
        {
            return _using
                ? where.Add(raw: context.Sqls.SiteDeptWhere.Params(siteId))
                : where;
        }

        public static SqlWhereCollection SiteGroupWhere(
            this Rds.GroupsWhereCollection where,
            Context context,
            long siteId,
            bool _using = true)
        {
            return _using
                ? where.Add(raw: context.Sqls.SiteGroupWhere.Params(siteId))
                : where;
        }

        public static SqlWhereCollection SiteUserWhere(
            this Rds.UsersWhereCollection where,
            Context context,
            long siteId,
            bool _using = true)
        {
            return _using
                ? where
                    .Add(or: new SqlWhereCollection()
                        .Add(
                            raw: context.Sqls.SiteUserWhere.Params(siteId))
                        .Add(
                            raw: context.Sqls.SitePermissionsWhere.Params(siteId)))
                : where;
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
                    .Add(or: new SqlWhereCollection()
                        .Add(
                            tableName: null,
                            raw: Def.Sql.CanReadSites)
                        .Add(
                            tableName: null,
                            subLeft: CheckRecordPermission(
                                context: context,
                                idColumnBracket: idColumnBracket),
                            _operator: null))
                : where;
        }

        private static SqlWhereCollection CheckRecordPermission(
            this SqlWhereCollection where,
            Context context,
            SiteSettings ss,
            Types permissionType,
            List<long> siteIdList = null)
        {
            return where.Add(
                tableName: ss.ReferenceType,
                subLeft: CheckRecordPermission(
                    context: context,
                    idColumnBracket: ss.IdColumnBracket(),
                    permissionType: permissionType,
                    siteIdList: siteIdList),
                _operator: null);
        }

        public static SqlExists CheckRecordPermission(
            Context context,
            string idColumnBracket,
            Types permissionType = Types.Read,
            List<long> siteIdList = null)
        {
            var type = permissionType.ToInt().ToString();
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
                    // PermissionTypeとの論理積をとりビットが立っているか確認する
                    .PermissionType(_operator: $" & {type}={type}")
                    .PermissionsWhere(context: context));
        }

        private static SqlWhereCollection PermissionsWhere(
            this SqlWhereCollection where,
            Context context)
        {
            return where.Add(raw: context.Sqls.PermissionsWhere);
        }

        public static string DeptOrUser(string tableName)
        {
            return $"(({Parameters.Parameter.SqlParameterPrefix}D<>0 "
                + $"and \"{tableName}\".\"DeptId\"={Parameters.Parameter.SqlParameterPrefix}D)"
                + $"or({Parameters.Parameter.SqlParameterPrefix}U<>0 "
                + $"and \"{tableName}\".\"UserId\"={Parameters.Parameter.SqlParameterPrefix}U)"
                + $"or(\"{tableName}\".\"UserId\"=-1))";
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
            var canRead = context.HasPrivilege
                || (Get(
                    context: context,
                    siteId: siteId) & Types.Read) == Types.Read;
            return canRead;
        }

        public static bool CanRead(Context context, long siteId, long id)
        {
            var canRead = CanRead(
                context: context,
                siteId: siteId)
                    || (GetById(
                        context: context,
                        id: id) & Types.Read) == Types.Read;
            return canRead;
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
            this IEnumerable<Column> columns,
            Context context,
            SiteSettings ss,
            bool checkPermission)
        {
            return columns.Where(o => !checkPermission
                || o.CanRead(
                    context: context,
                    ss: ss,
                    mine: null));
        }

        public static IEnumerable<string> AllowedColumns(
            Context context, SiteSettings ss)
        {
            return ss.Columns.AllowedColumns(
                context: context,
                ss: ss,
                checkPermission: true)
                    .Select(o => o.ColumnName);
        }

        public static bool Allowed(
            this List<ColumnAccessControl> columnAccessControls,
            Context context,
            SiteSettings ss,
            Column column,
            List<string> mine)
        {
            return columnAccessControls?
                .FirstOrDefault(o => o.ColumnName == column.Name)?
                .Allowed(
                    context: context,
                    ss: ss,
                    mine: mine) != false;
        }

        public static bool HasPermission(this Context context, SiteSettings ss, bool isSearch = false)
        {
            return ss.PermissionType != null
                || ss.ItemPermissionType != null
                || ss.ReferenceType == null
                || context.HasPrivilege
                || isSearch;
        }

        public static Types SiteTopPermission(this Context context)
        {
            return context.UserSettings?.AllowCreationAtTopSite(context: context) != true
                ? Types.Read
                : (Types)Parameters.Permissions.Manager;
        }

        public static bool CanRead(
            this Context context,
            SiteSettings ss,
            bool site = false)
        {
            switch (context.Controller)
            {
                case "tenants":
                    return CanManageTenant(context: context)
                        || context.UserSettings?.EnableManageTenant == true;
                case "syslogs":
                    return context.HasPrivilege;
                case "depts":
                    return CanManageTenant(context: context)
                        || context.UserSettings?.EnableManageTenant == true;
                case "groups":
                    return CanReadGroup(context: context)
                        || context.UserSettings?.EnableManageTenant == true;
                case "users":
                    return CanManageTenant(context: context)
                        || context.UserId == context.Id
                        || context.UserSettings?.EnableManageTenant == true;
                case "registrations":
                    return CanManageRegistrations(context: context, any: true);
                case "publishes":
                    return context.Publish;
                case "extensions":
                    return CanManageTenant(context: context);
                default:
                    if (ss.IsSiteEditor(context: context) || ss.IsDashboardEditor(context: context))
                    {
                        return context.CanManageSite(ss: ss);
                    }
                    else
                    {
                        return context.ItemsCan(
                            ss: ss,
                            type: Types.Read,
                            site: site);
                    }
            }
        }

        public static bool CanCreate(this Context context, SiteSettings ss, bool site = false)
        {
            switch (context.Controller)
            {
                case "tenants":
                    return false;
                case "syslogs":
                    return false;
                case "depts":
                case "users":
                    if (context.UserSettings?.EnableManageTenant == true)
                    {
                        return false;
                    }
                    else
                    {
                        return CanManageTenant(context: context);
                    }
                case "registrations":
                    return CanManageRegistrations(context: context);
                case "groups":
                    return CanCreateGroup(context: context)
                        || context.UserSettings?.EnableManageTenant == true;
                case "versions":
                    return false;
                case "extensions":
                    return CanManageTenant(context: context);
                default:
                    if (ss.IsSiteEditor(context: context) || ss.IsDashboardEditor(context: context))
                    {
                        return context.CanManageSite(ss: ss);
                    }
                    else
                    {
                        return context.ItemsCan(ss: ss, type: Types.Create, site: site);
                    }
            }
        }

        public static bool CanUpdate(
            this Context context,
            SiteSettings ss,
            bool site = false,
            bool checkLocked = true)
        {
            switch (context.Controller)
            {
                case "tenants":
                    return CanManageTenant(context: context)
                        || context.UserSettings?.EnableManageTenant == true;
                case "syslogs":
                    return false;
                case "depts":
                    return CanManageTenant(context: context)
                        || context.UserSettings?.EnableManageTenant == true;
                case "groups":
                    return CanEditGroup(context: context)
                        || context.UserSettings?.EnableManageTenant == true;
                case "users":
                    return CanManageTenant(context: context)
                        || context.UserId == context.Id
                        || context.UserSettings?.EnableManageTenant == true;
                case "registrations":
                    return CanManageRegistrations(context: context, any: true);
                case "extensions":
                    return CanManageTenant(context: context);
                default:
                    if (ss.IsSiteEditor(context: context) || ss.IsDashboardEditor(context: context))
                    {
                        return context.CanManageSite(ss: ss);
                    }
                    else
                    {
                        return context.ItemsCan(
                            ss: ss,
                            type: Types.Update,
                            site: site,
                            checkLocked: checkLocked);
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
                case "syslogs":
                    return false;
                case "depts":
                    return CanManageTenant(context: context);
                case "groups":
                    return CanEditGroup(context: context)
                        || context.UserSettings?.EnableManageTenant == true;
                case "users":
                    return CanManageTenant(context: context)
                        && context.UserId != context.Id;
                case "registrations":
                    return PrivilegedUsers(loginId: context.LoginId);
                case "extensions":
                    return CanManageTenant(context: context);
                default:
                    if (ss.IsSiteEditor(context: context) || ss.IsDashboardEditor(context: context))
                    {
                        return context.CanManageSite(ss: ss);
                    }
                    else
                    {
                        return context.ItemsCan(ss: ss, type: Types.Delete, site: site);
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
                case "syslogs":
                    return false;
                case "depts":
                    return CanManageTenant(context: context);
                case "groups":
                    return CanEditGroup(context: context)
                        || context.UserSettings?.EnableManageTenant == true;
                case "users":
                    return CanManageTenant(context: context)
                        || context.UserId == context.Id;
                default:
                    if (ss.IsSiteEditor(context: context) || ss.IsDashboardEditor(context: context))
                    {
                        return context.CanManageSite(ss: ss);
                    }
                    else
                    {
                        return context.ItemsCan(ss: ss, type: Types.SendMail, site: site);
                    }
            }
        }

        public static bool CanImport(this Context context, SiteSettings ss, bool site = false)
        {
            if (context.ContractSettings.Import == false) return false;
            switch (context.Controller)
            {
                case "tenants":
                    return false;
                case "syslogs":
                    return false;
                case "depts":
                    return CanManageTenant(context: context);
                case "groups":
                    return CanManageTenant(context: context)
                        || context.UserSettings?.EnableManageTenant == true;
                case "users":
                    return CanManageTenant(context: context);
                default:
                    return context.ItemsCan(ss: ss, type: Types.Import, site: site);
            }
        }

        public static bool CanExport(this Context context, SiteSettings ss, bool site = false)
        {
            if (context.ContractSettings.Export == false) return false;
            switch (context.Controller)
            {
                case "tenants":
                    return false;
                case "syslogs":
                    return context.HasPrivilege;
                case "depts":
                    return CanManageTenant(context: context);
                case "groups":
                    return CanManageTenant(context: context)
                        || context.UserSettings?.EnableManageTenant == true;
                case "users":
                    return CanManageTenant(context: context);
                default:
                    return context.ItemsCan(ss: ss, type: Types.Export, site: site);
            }
        }

        public static bool CanManageSite(this Context context, SiteSettings ss, bool site = false)
        {
            return context.ItemsCan(ss: ss, type: Types.ManageSite, site: site);
        }

        public static bool CanManagePermission(this Context context, SiteSettings ss, bool site = false)
        {
            return context.ItemsCan(ss: ss, type: Types.ManagePermission, site: site);
        }

        public static ColumnPermissionTypes ColumnPermissionType(
            Context context,
            SiteSettings ss,
            Column column,
            BaseModel baseModel)
        {
            var canEdit = column.CanEdit(
                context: context,
                ss: ss,
                baseModel: baseModel);
            if (context.IsNew)
            {
                return column.CanCreate(
                    context: context,
                    ss: ss,
                    mine: baseModel?.Mine(context: context))
                        && canEdit
                            ? ColumnPermissionTypes.Update
                            : column.CanRead(
                                context: context,
                                ss: ss,
                                mine: baseModel?.Mine(context: context))
                                    ? ColumnPermissionTypes.Read
                                    : ColumnPermissionTypes.Deny;
            }
            else
            {
                return column.CanRead(
                    context: context,
                    ss: ss,
                    mine: baseModel?.Mine(context: context))
                        && canEdit
                            ? ColumnPermissionTypes.Update
                            : column.CanRead(
                                context: context,
                                ss: ss,
                                mine: baseModel?.Mine(context: context))
                                    ? ColumnPermissionTypes.Read
                                    : ColumnPermissionTypes.Deny;
            }
        }

        public static bool CanManageTenant(Context context)
        {
            return context.User?.TenantManager == true
                || context.HasPrivilege;
        }

        public static bool CanManageTenantOrEnableManageTenant(Context context)
        {
            //テナントの管理者、特権ユーザ、またはPleasanter.netの管理者の場合にTrueを返す
            return CanManageTenant(context: context)
                || context.UserSettings?.EnableManageTenant == true;
        }

        public static bool CanManageUser(Context context)
        {
            return (context.User?.TenantManager == true
                && Parameters.Service.ShowProfiles)
                    || context.HasPrivilege;
        }

        //メソッド名を考える
        public static bool CannotManageUsers(Context context)
        {
            return (context.UserSettings?.EnableManageTenant == false
                || context.UserSettings?.EnableManageTenant == null)
                && !context.HasPrivilege
                && !Parameters.Service.ShowProfiles;
        }

        public static bool CanManageRegistrations(Context context, bool any = false)
        {
            return Parameters.Registration.Enabled
                ? Parameters.Registration.PrivilegedUserOnly
                    ? PrivilegedUsers(loginId: context.LoginId) || (any && Registrations(context: context).Any())
                    : true
                : false;
        }

        public static bool CanReadGroup(Context context)
        {
            return context.UserSettings?.AllowGroupAdministration(context: context) == true
                && (context.Id == 0
                    || CanManageTenant(context: context)
                    || Groups(context: context).Any()
                    || context.HasPrivilege);
        }

        public static bool CanCreateGroup(Context context)
        {
            return context.UserSettings?.AllowGroupCreation(context: context) == true
                || context.HasPrivilege;
        }

        public static bool CanEditGroup(Context context)
        {
            return context.UserSettings?.AllowGroupAdministration(context: context) == true
                && (context.Id == 0
                    || CanManageTenant(context: context)
                    || Groups(context: context).Any(o => o["Admin"].ToBool())
                    || context.HasPrivilege);
        }

        /// <summary>
        /// Items以外(Tenants/Depts/Groups/Users等)では使用しない
        /// </summary>
        private static bool ItemsCan(
            this Context context,
            SiteSettings ss,
            Types type,
            bool site,
            bool checkLocked = true)
        {
            if (checkLocked && ss.Locked())
            {
                if ((type & Types.Update) == Types.Update) return false;
                if ((type & Types.Delete) == Types.Delete) return false;
            }
            if (ss.LockedTable())
            {
                if ((type & Types.Create) == Types.Create) return false;
                if ((type & Types.Import) == Types.Import) return false;
            }
            return (ss.GetPermissionType(
                context: context,
                site: site) & type) == type
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

        private static EnumerableRowCollection<DataRow> Registrations(Context context)
        {
            return Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectRegistrations(
                    column: Rds.RegistrationsColumn(),
                    where: Rds.RegistrationsWhere()
                        .Passphrase(context.QueryStrings.Data("passphrase"), _operator: context.Sqls.Like)))
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