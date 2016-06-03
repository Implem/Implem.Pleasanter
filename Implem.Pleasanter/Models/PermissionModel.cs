using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Analysis;
using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.ServerData;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Models
{
    public class PermissionModel : BaseModel
    {
        public string ReferenceType = "Sites";
        public long ReferenceId = 0;
        public int DeptId = 0;
        public int UserId = 0;
        public string DeptName = string.Empty;
        public string FullName1 = string.Empty;
        public string FullName2 = string.Empty;
        public Names.FirstAndLastNameOrders FirstAndLastNameOrder = (Names.FirstAndLastNameOrders)1;
        public string SavedReferenceType = "Sites";
        public long SavedReferenceId = 0;
        public int SavedDeptId = 0;
        public int SavedUserId = 0;
        public string SavedDeptName = string.Empty;
        public string SavedFullName1 = string.Empty;
        public string SavedFullName2 = string.Empty;
        public int SavedFirstAndLastNameOrder = 1;
        public long SavedPermissionType = 31;
        public bool ReferenceType_Updated { get { return ReferenceType != SavedReferenceType && ReferenceType != null; } }
        public bool ReferenceId_Updated { get { return ReferenceId != SavedReferenceId; } }
        public bool DeptId_Updated { get { return DeptId != SavedDeptId; } }
        public bool UserId_Updated { get { return UserId != SavedUserId; } }
        public bool PermissionType_Updated { get { return PermissionType.ToLong() != SavedPermissionType; } }

        public PermissionModel(
            DataRow dataRow)
        {
            OnConstructing();
            Set(dataRow);
            OnConstructed();
        }

        private void OnConstructing()
        {
        }

        private void OnConstructed()
        {
        }

        public void ClearSessions()
        {
        }

        public PermissionModel Get(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            Set(Rds.ExecuteTable(statements: Rds.SelectPermissions(
                tableType: tableType,
                column: column ?? Rds.PermissionsColumnDefault(),
                join: join ??  Rds.PermissionsJoinDefault(),
                where: where ?? Rds.PermissionsWhereDefault(this),
                orderBy: orderBy ?? null,
                param: param ?? null,
                distinct: distinct,
                top: top)));
            return this;
        }

        private void SetBySession()
        {
        }

        private void Set(DataTable dataTable)
        {
            switch (dataTable.Rows.Count)
            {
                case 1: Set(dataTable.Rows[0]); break;
                case 0: AccessStatus = Databases.AccessStatuses.NotFound; break;
                default: AccessStatus = Databases.AccessStatuses.Overlap; break;
            }
        }

        private void Set(DataRow dataRow)
        {
            AccessStatus = Databases.AccessStatuses.Selected;
            foreach(DataColumn dataColumn in dataRow.Table.Columns)
            {
                var name = dataColumn.ColumnName;
                switch(name)
                {
                    case "ReferenceType": if (dataRow[name] != DBNull.Value) { ReferenceType = dataRow[name].ToString(); SavedReferenceType = ReferenceType; } break;
                    case "ReferenceId": if (dataRow[name] != DBNull.Value) { ReferenceId = dataRow[name].ToLong(); SavedReferenceId = ReferenceId; } break;
                    case "DeptId": if (dataRow[name] != DBNull.Value) { DeptId = dataRow[name].ToInt(); SavedDeptId = DeptId; } break;
                    case "UserId": if (dataRow[name] != DBNull.Value) { UserId = dataRow[name].ToInt(); SavedUserId = UserId; } break;
                    case "Ver": Ver = dataRow[name].ToInt(); SavedVer = Ver; break;
                    case "DeptName": DeptName = dataRow[name].ToString(); SavedDeptName = DeptName; break;
                    case "FullName1": FullName1 = dataRow[name].ToString(); SavedFullName1 = FullName1; break;
                    case "FullName2": FullName2 = dataRow[name].ToString(); SavedFullName2 = FullName2; break;
                    case "FirstAndLastNameOrder": FirstAndLastNameOrder = (Names.FirstAndLastNameOrders)dataRow[name].ToInt(); SavedFirstAndLastNameOrder = FirstAndLastNameOrder.ToInt(); break;
                    case "PermissionType": PermissionType = (Permissions.Types)dataRow[name].ToLong(); SavedPermissionType = PermissionType.ToLong(); break;
                    case "Comments": Comments = dataRow["Comments"].ToString().Deserialize<Comments>() ?? new Comments(); SavedComments = Comments.ToJson(); break;
                    case "Creator": Creator = SiteInfo.User(dataRow.Int(name)); SavedCreator = Creator.Id; break;
                    case "Updator": Updator = SiteInfo.User(dataRow.Int(name)); SavedUpdator = Updator.Id; break;
                    case "CreatedTime": CreatedTime = new Time(dataRow, "CreatedTime"); SavedCreatedTime = CreatedTime.Value; break;
                    case "UpdatedTime": UpdatedTime = new Time(dataRow, "UpdatedTime"); Timestamp = dataRow.Field<DateTime>("UpdatedTime").ToString("yyyy/M/d H:m:s.fff"); SavedUpdatedTime = UpdatedTime.Value; break;
                    case "IsHistory": VerType = dataRow[name].ToBool() ? Versions.VerTypes.History : Versions.VerTypes.Latest; break;
                }
            }
        }

        private string ResponseConflicts()
        {
            Get();
            return AccessStatus == Databases.AccessStatuses.Selected
                ? Messages.ResponseUpdateConflicts(Updator.FullName).ToJson()
                : Messages.ResponseDeleteConflicts().ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string PermissionId
        {
            get
            {
                if (DeptId != 0)
                {
                    return "Dept," + DeptId;
                }
                else
                {
                    return "User," + UserId;
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string PermissionTitle
        {
            get
            {
                if (DeptId != 0)
                {
                    return "[" + Displays.Depts() + " " + DeptId + "] " + DeptName;
                }
                else
                {
                    return "[" + Displays.Users() + " " + UserId + "] " + 
                        Names.FullName(FirstAndLastNameOrder, FullName1, FullName2);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string PermissionTypeName
        {
            get
            {
                switch (PermissionType)
                {
                    case Permissions.Types.ReadOnly: return Displays.ReadOnly();
                    case Permissions.Types.ReadWrite: return Displays.ReadWrite();
                    case Permissions.Types.Leader: return Displays.Leader();
                    case Permissions.Types.Manager: return Displays.Manager();
                    default: return string.Empty;
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public bool Check(Permissions.Types permissionType)
        {
            return (PermissionType & permissionType) != 0;
        }
    }

    public class PermissionCollection : List<PermissionModel>
    {
        public Databases.AccessStatuses AccessStatus = Databases.AccessStatuses.Initialized;
        public Aggregations Aggregations = new Aggregations();

        public PermissionCollection(
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            bool distinct = false,
            int top = 0,
            int offset = 0,
            int pageSize = 0,
            bool countRecord = false,
            IEnumerable<Aggregation> aggregationCollection = null,
            bool get = true)
        {
            if (get)
            {
                Set(Get(
                    column: column,
                    join: join,
                    where: where,
                    orderBy: orderBy,
                    param: param,
                    tableType: tableType,
                    distinct: distinct,
                    top: top,
                    offset: offset,
                    pageSize: pageSize,
                    countRecord: countRecord,
                    aggregationCollection: aggregationCollection));
            }
        }

        public PermissionCollection(
            DataTable dataTable)
        {
            Set(dataTable);
        }

        private PermissionCollection Set(
            DataTable dataTable)
        {
            if (dataTable.Rows.Count > 0)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    Add(new PermissionModel(dataRow));
                }
                AccessStatus = Databases.AccessStatuses.Selected;
            }
            else
            {
                AccessStatus = Databases.AccessStatuses.NotFound;
            }
            return this;
        }

        public PermissionCollection(
            string commandText,
            SqlParamCollection param = null)
        {
            Set(Get(commandText, param));
        }

        private DataTable Get(
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            bool distinct = false,
            int top = 0,
            int offset = 0,
            int pageSize = 0,
            bool history = false,
            bool countRecord = false,
            IEnumerable<Aggregation> aggregationCollection = null)
        {
            var statements = new List<SqlStatement>
            {
                Rds.SelectPermissions(
                    dataTableName: "Main",
                    column: column ?? Rds.PermissionsColumnDefault(),
                    join: join ??  Rds.PermissionsJoinDefault(),
                    where: where ?? null,
                    orderBy: orderBy ?? null,
                    param: param ?? null,
                    tableType: tableType,
                    distinct: distinct,
                    top: top,
                    offset: offset,
                    pageSize: pageSize,
                    countRecord: countRecord)
            };
            if (aggregationCollection != null)
            {
                statements.AddRange(Rds.PermissionsAggregations(aggregationCollection, where));
            }
            var dataSet = Rds.ExecuteDataSet(
                transactional: false,
                statements: statements.ToArray());
            Aggregations.Set(dataSet, aggregationCollection);
            return dataSet.Tables["Main"];
        }

        private DataTable Get(string commandText, SqlParamCollection param = null)
        {
            return Rds.ExecuteTable(
                transactional: false,
                statements: Rds.PermissionsStatement(
                    commandText: commandText,
                    param: param ?? null));
        }
    }

    public static class PermissionsUtility
    {
        public static HtmlBuilder TdValue(
            this HtmlBuilder hb, Column column, PermissionModel permissionModel)
        {
            switch (column.ColumnName)
            {
                case "Ver": return hb.Td(column: column, value: permissionModel.Ver);
                case "Comments": return hb.Td(column: column, value: permissionModel.Comments);
                case "Creator": return hb.Td(column: column, value: permissionModel.Creator);
                case "Updator": return hb.Td(column: column, value: permissionModel.Updator);
                case "CreatedTime": return hb.Td(column: column, value: permissionModel.CreatedTime);
                case "UpdatedTime": return hb.Td(column: column, value: permissionModel.UpdatedTime);
                default: return hb;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public enum Types
        {
            Destination,
            Source
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Editor(long siteId)
        {
            var siteModel = new SiteModel(siteId, clearSessions: true);
            var siteSettings = siteModel.PermissionsSiteSettings();
            var hb = new HtmlBuilder();
            hb.Template(
                siteId: siteModel.SiteId,
                referenceId: "Permissions",
                title: siteModel.Title.Value + " - " + Displays.EditPermissions(),
                permissionType: siteModel.PermissionType,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.Edit,
                allowAccess: siteModel.PermissionType.CanEditPermission(),
                useNavigationButtons: false,
                action: () => hb
                    .Editor(siteModel: siteModel, siteSettings: siteSettings));
            return hb.ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Editor(
            this HtmlBuilder hb, SiteModel siteModel, SiteSettings siteSettings)
        {
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id_Css("PermissionForm", "edit-form")
                    .Action(Navigations.ItemAction(siteModel.SiteId, "Permissions")),
                action: () => hb
                    .Div(
                        css: "edit-form-tabs-max",
                        action: () => hb
                            .FieldTabs()
                            .Fields(siteModel: siteModel, siteSettings: siteSettings)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder FieldTabs(this HtmlBuilder hb)
        {
            return hb.Ul(css: "field-tab", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetPermissionEditor",
                        text: Displays.PermissionSetting())));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Fields(
            this HtmlBuilder hb, SiteModel siteModel, SiteSettings siteSettings)
        {
            SetPermissionCollectionSession(siteModel);
            return hb.FieldSet(
                id: "FieldSetPermissionEditor",
                action: () => hb
                    .Inherit(siteModel: siteModel)
                    .Div(id: "Selectables", action: () => hb
                        .Selectables(siteModel: siteModel))
                    .MainCommands(
                        siteId: siteModel.SiteId,
                        permissionType: siteModel.PermissionType,
                        verType: Versions.VerTypes.Latest,
                        backUrl: Navigations.ItemEdit(siteModel.SiteId),
                        updateButton: true));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder Inherit(this HtmlBuilder hb, SiteModel siteModel)
        {
            return siteModel.SiteId != 0
                ? hb.FieldDropDown(
                    controlId: "Sites_InheritPermission",
                    fieldCss: "field-auto-thin",
                    controlCss: " auto-postback",
                    labelText: Displays.Sites_InheritPermission(),
                    optionCollection: InheritTargets(siteModel.SiteId),
                    selectedValue: siteModel.InheritPermission.ToString(),
                    action: "ChangeInherit",
                    method: "put")
                : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Dictionary<string, ControlData> InheritTargets(long siteId)
        {
            return new Dictionary<string, ControlData>
            {
                { siteId.ToString(), new ControlData(Displays.NotInheritPermission()) },
            }.AddRange(Rds.ExecuteTable(statements:
                Rds.SelectSites(
                    column: Rds.SitesColumn()
                        .SiteId()
                        .Title(),
                    join: Rds.SitesJoinDefault(),
                    where: Rds.SitesWhere()
                        .TenantId(Sessions.TenantId())
                        .SiteId(siteId, _operator: "<>")
                        .InheritPermission(raw: "[t0].[SiteId]")
                        .PermissionType(_operator: " is not null "),
                    orderBy: Rds.SitesOrderBy().Title()))
                        .AsEnumerable()
                        .ToDictionary(
                            o => o["SiteId"].ToString(),
                            o => new ControlData(o["Title"].ToString())));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Selectables(this HtmlBuilder hb, SiteModel siteModel)
        {
            return siteModel.SiteId == siteModel.InheritPermission
                ? hb
                    .Destinations(
                        permissionCollection: siteModel.Session_PermissionDestinationCollection())
                    .Sources(
                        permissionCollection: siteModel.Session_PermissionSourceCollection())
                : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void SetPermissionCollectionSession(SiteModel siteModel)
        {
            siteModel.Session_PermissionDestinationCollection(
                PermissionsUtility.DestinationCollection(
                    "Sites", siteModel.SiteId));
            siteModel.Session_PermissionSourceCollection(
                PermissionsUtility.SourceCollection(
                    "Sites", 
                    siteModel.SiteId, 
                    Forms.Data("SearchText")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Destinations(
            this HtmlBuilder hb, PermissionCollection permissionCollection)
        {
            return hb.FieldSelectable(
                controlId: "PermissionDestination",
                fieldCss: "field-vertical both",
                controlContainerCss: "container-selectable",
                labelText: Displays.PermissionDestination(),
                listItemCollection: permissionCollection.OrderBy(o => o.PermissionId)
                    .ToDictionary(
                        o => o.PermissionId,
                        o => o.PermissionTitle + " - [" + o.PermissionTypeName + "]"),
                selectedValueCollection: new List<string>(),
                commandOptionPositionIsTop: true,
                commandOptionAction: () => hb
                    .Div(css: "command-left", action: () => hb
                        .Button(
                            controlId: "ReadOnly",
                            controlCss: "button-person post",
                            text: Displays.ReadOnly(),
                            onClick: Def.JavaScript.Submit,
                            action: "Set",
                            method: "put")
                        .Button(
                            controlId: "ReadWrite",
                            controlCss: "button-person post",
                            text: Displays.ReadWrite(),
                            onClick: Def.JavaScript.Submit,
                            action: "Set",
                            method: "put")
                        .Button(
                            controlId: "Leader",
                            controlCss: "button-person post",
                            text: Displays.Leader(),
                            onClick: Def.JavaScript.Submit,
                            action: "Set",
                            method: "put")
                        .Button(
                            controlId: "Manager",
                            controlCss: "button-person post",
                            text: Displays.Manager(),
                            onClick: Def.JavaScript.Submit,
                            action: "Set",
                            method: "put")
                        .Button(
                            controlId: "Delete",
                            controlCss: "button-to-right post",
                            text: Displays.DeletePermission(),
                            onClick: Def.JavaScript.Submit,
                            action: "Set",
                            method: "delete")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Sources(
            this HtmlBuilder hb, PermissionCollection permissionCollection)
        {
            return hb.FieldSelectable(
                controlId: "PermissionSource",
                fieldCss: "field-vertical",
                controlContainerCss: "container-selectable",
                controlCss: " h550",
                labelText: Displays.PermissionSource(),
                listItemCollection: permissionCollection
                    .OrderBy(o => o.PermissionId)
                    .ToDictionary(o => o.PermissionId, o => o.PermissionTitle),
                selectedValueCollection: new List<string>(),
                commandOptionPositionIsTop: true,
                commandOptionAction: () => hb
                    .Div(css: "command-left", action: () => hb
                        .Button(
                            controlId: "Add",
                            controlCss: "button-to-left post",
                            text: Displays.AddPermission(),
                            onClick: Def.JavaScript.Submit,
                            action: "Set",
                            method: "post")
                        .Span(css: "ui-icon ui-icon-search")
                        .TextBox(
                            controlId: "SearchText",
                            controlCss: " auto-postback w100",
                            placeholder: Displays.Search(),
                            action: "Set",
                            method: "put")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static PermissionCollection DestinationCollection(
            string referenceType, long referenceId)
        {
            return new PermissionCollection(
                where: Rds.PermissionsWhere()
                    .ReferenceType(referenceType)
                    .ReferenceId(referenceId));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static PermissionCollection SourceCollection(
            string referenceType, long referenceId, string searchText)
        {
            return !searchText.IsNullOrEmpty()
                ? new PermissionCollection(Rds.ExecuteTable(
                    transactional: false,
                    statements: new SqlStatement[]
                    {
                        Rds.SelectDepts(
                            column: Rds.DeptsColumn()
                                .Add("@ReferenceId_Param1 as [ReferenceId]")
                                .Add("@ReferenceType_Param1 as [ReferenceType]")
                                .DeptId()
                                .DeptName()
                                .Add("null as [UserId]")
                                .Add("null as [FullName1]")
                                .Add("null as [FullName2]")
                                .Add("1 as [FirstAndLastNameOrder]")
                                .Add("@PermissionType_Param1 as [PermissionType]"),
                            where: Rds.DeptsWhere()
                                .TenantId(Sessions.TenantId())
                                .SqlWhereExists(Rds.SqlWhereNotExists_Permissions,
                                    "[Permissions].[ReferenceId] = @ReferenceId_Param1",
                                    "[Permissions].[ReferenceType] = @ReferenceType_Param1",
                                    "[Permissions].[DeptId] = [t0].[DeptId]")
                                .SqlWhereLike(
                                    searchText,
                                    Rds.Depts_DeptId_WhereLike(),
                                    Rds.Depts_DeptCode_WhereLike(),
                                    Rds.Depts_DeptName_WhereLike()),
                            param: Rds.PermissionsParam()
                                .ReferenceType(referenceType)
                                .ReferenceId(referenceId)
                                .PermissionType(Permissions.Types.ReadWrite),
                            unionType: Sqls.UnionTypes.Union),
                        Rds.SelectUsers(
                            column: Rds.UsersColumn()
                                .Add("@ReferenceId_Param2 as [ReferenceId]")
                                .Add("@ReferenceType_Param2 as [ReferenceType]")
                                .Add("null as [DeptId]")
                                .Add("null as [DeptName]")
                                .UserId()
                                .Add("[t0].[FirstName] + ' ' + [t0].[LastName] as [FullName1]")
                                .Add("[t0].[LastName] + ' ' + [t0].[FirstName] as [FullName2]")
                                .FirstAndLastNameOrder()
                                .Add("@PermissionType_Param2 as [PermissionType]"),
                            where: Rds.UsersWhere()
                                .TenantId(Sessions.TenantId())
                                .SqlWhereExists(Rds.SqlWhereNotExists_Permissions,
                                    "[Permissions].[ReferenceId] = @ReferenceId_Param2",
                                    "[Permissions].[ReferenceType] = @ReferenceType_Param2",
                                    "[Permissions].[UserId] = [t0].[UserId]")
                                .SqlWhereLike(
                                    searchText,
                                    Rds.Users_LoginId_WhereLike(),
                                    Rds.Users_UserId_WhereLike(),
                                    Rds.Users_FirstName_WhereLike(),
                                    Rds.Users_LastName_WhereLike()),
                            param: Rds.PermissionsParam()
                                .ReferenceType(referenceType)
                                .ReferenceId(referenceId)
                                .PermissionType(Permissions.Types.ReadWrite))
                    }))
                : new PermissionCollection(get: false);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string PermissionListItem(
            SiteModel siteModel,
            Types sourceOrDestination,
            List<string> selectedValueTextCollection = null)
        {
            switch (sourceOrDestination)
            {
                case Types.Destination:
                    return new HtmlBuilder().SelectableItems(
                        listItemCollection: siteModel.Session_PermissionDestinationCollection()
                            .OrderBy(o => o.PermissionId)
                            .ToDictionary(
                                o => o.PermissionId, 
                                o => o.PermissionTitle + " - [" + o.PermissionTypeName + "]"),
                        selectedValueTextCollection:
                            selectedValueTextCollection ?? new List<string>()).ToString();
                case Types.Source:
                    return new HtmlBuilder().SelectableItems(
                        listItemCollection: siteModel.Session_PermissionSourceCollection()
                            .OrderBy(o => o.PermissionId)
                            .ToDictionary(o => o.PermissionId, o => o.PermissionTitle),
                        selectedValueTextCollection:
                            selectedValueTextCollection ?? new List<string>()).ToString();
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string ChangeInherit(long siteId)
        {
            var inheritPermission = Forms.Long("Sites_InheritPermission");
            var hb = new HtmlBuilder();
            if (siteId == inheritPermission)
            {
                var inheritSite = new SiteModel(siteId).InheritSite();
                SetPermissionCollectionSession(inheritSite);
                hb.Selectables(inheritSite);
            }
            return new ResponseCollection()
                .Html("#Selectables", hb)
                .SetFormData("Sites_InheritPermission", inheritPermission).ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Update(long siteId)
        {
            var siteModel = new SiteModel(siteId, setByForm: true);
            if (siteModel.PermissionType.CanEditPermission())
            {
                var statements = new List<SqlStatement>();
                statements.Add(Rds.PhysicalDeletePermissions(
                    where: Rds.PermissionsWhere().ReferenceId(siteId)));
                if (siteModel.InheritPermission == siteId)
                {
                    siteModel.Session_PermissionDestinationCollection()
                        .ForEach(permissionModel =>
                            statements.Add(Insert(permissionModel, siteId)));
                }
                statements.Add(Rds.UpdateSites(
                    verUp: false,
                    where: Rds.SitesWhere().SiteId(siteModel.SiteId),
                    param: Rds.SitesParam().InheritPermission(siteModel.InheritPermission)));
                Rds.ExecuteNonQuery(transactional: true, statements: statements.ToArray());
                SiteInfo.SetSiteUserIdCollection(siteModel.InheritPermission, reload: true);
                return Messages.ResponseUpdated("permissions").ToJson();
            }
            else
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static SqlInsert Insert(PermissionModel permissionModel, long siteId)
        {
            return Rds.InsertPermissions(param: Rds.PermissionsParam()
                .ReferenceType(raw: "'Sites'")
                .ReferenceId(raw: siteId.ToString())
                .PermissionType(raw: permissionModel.PermissionType.ToLong().ToString())
                .DeptId(
                    raw: permissionModel.DeptId.ToString(),
                    _using: permissionModel.DeptId != 0)
                .UserId(raw: "0", _using: permissionModel.DeptId != 0)
                .DeptId(raw: "0", _using: permissionModel.UserId != 0)
                .UserId(
                    raw: permissionModel.UserId.ToString(),
                    _using: permissionModel.UserId != 0));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Set(long siteId)
        {
            var siteModel = new SiteModel(siteId, setByForm: true);
            var responseCollection = new ResponseCollection();
            var selectedDestinationPermissionType_ItemIdCollection = Forms.Data("PermissionDestination")
                .SortedSet(';')
                .Where(o => o != string.Empty)
                .ToList<string>();
            var selectedSourcePermissionType_ItemIdCollection = Forms.Data("PermissionSource")
                .SortedSet(';')
                .Where(o => o != string.Empty)
                .ToList<string>();
            if (Forms.Data("command") != "AddPermission" &&
                selectedDestinationPermissionType_ItemIdCollection.Contains("User," + Sessions.UserId()))
            {
                responseCollection.Message(Messages.PermissionNotSelfChange());
            }
            else
            {
                switch (Forms.Data("ControlId"))
                {
                    case "ReadOnly":
                        responseCollection.SetPermissionType(
                            siteModel,
                            selectedDestinationPermissionType_ItemIdCollection,
                            Permissions.Types.ReadOnly);
                        break;
                    case "ReadWrite":
                        responseCollection.SetPermissionType(
                            siteModel,
                            selectedDestinationPermissionType_ItemIdCollection,
                            Permissions.Types.ReadWrite);
                        break;
                    case "Leader":
                        responseCollection.SetPermissionType(
                            siteModel,
                            selectedDestinationPermissionType_ItemIdCollection,
                            Permissions.Types.Leader);
                        break;
                    case "Manager":
                        responseCollection.SetPermissionType(
                            siteModel,
                            selectedDestinationPermissionType_ItemIdCollection,
                            Permissions.Types.Manager);
                        break;
                    case "Add":
                        siteModel.Session_PermissionDestinationCollection().AddRange(
                            siteModel.Session_PermissionSourceCollection().Where(o =>
                                selectedSourcePermissionType_ItemIdCollection
                                    .Contains(o.PermissionId)));
                        siteModel.Session_PermissionDestinationCollection().Where(o =>
                            selectedSourcePermissionType_ItemIdCollection.Contains(o.PermissionId))
                            .ForEach(o =>
                                o.PermissionType = Permissions.Types.ReadWrite);
                        siteModel.Session_PermissionSourceCollection().RemoveAll(o =>
                            selectedSourcePermissionType_ItemIdCollection
                                .Contains(o.PermissionId));
                        responseCollection
                            .Html("#PermissionDestination", PermissionListItem(
                                siteModel, Types.Destination,
                                selectedSourcePermissionType_ItemIdCollection))
                            .Html("#PermissionSource", PermissionListItem(siteModel, Types.Source))
                            .SetFormData("PermissionDestination", selectedSourcePermissionType_ItemIdCollection.Join(";"))
                            .SetFormData("PermissionSource", string.Empty);
                        break;
                    case "Delete":
                        siteModel.Session_PermissionSourceCollection().AddRange(
                            siteModel.Session_PermissionDestinationCollection().Where(o =>
                                selectedDestinationPermissionType_ItemIdCollection
                                    .Contains(o.PermissionId)));
                        siteModel.Session_PermissionDestinationCollection().RemoveAll(o =>
                            selectedDestinationPermissionType_ItemIdCollection
                                .Contains(o.PermissionId));
                        responseCollection
                            .Html("#PermissionDestination", PermissionListItem(siteModel, Types.Destination))
                            .Html("#PermissionSource", PermissionListItem(
                                siteModel, Types.Source,
                                selectedDestinationPermissionType_ItemIdCollection))
                            .SetFormData("PermissionDestination", string.Empty)
                            .SetFormData("PermissionSource", selectedDestinationPermissionType_ItemIdCollection.Join(";"));
                        break;
                    case "SearchText":
                        siteModel.Session_PermissionSourceCollection(
                            PermissionsUtility.SourceCollection(
                                "Sites",
                                siteModel.SiteId,
                                Forms.Data("SearchText")));
                        siteModel.Session_PermissionSourceCollection().RemoveAll(o =>
                            siteModel.Session_PermissionDestinationCollection()
                                .Any(p => p.PermissionId == o.PermissionId));
                        responseCollection.Html("#PermissionSource", PermissionListItem(
                            siteModel, Types.Source,
                            selectedDestinationPermissionType_ItemIdCollection));
                        break;
                }
            }
            return responseCollection.ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static void SetPermissionType(
            this ResponseCollection responseCollection,
            SiteModel siteModel,
            List<string> selectedPermissionType_ItemIdCollection,
            Permissions.Types permissionType)
        {
            selectedPermissionType_ItemIdCollection.ForEach(permissionType_ItemId =>
                siteModel.Session_PermissionDestinationCollection()
                    .Where(o => (o.PermissionId == permissionType_ItemId))
                    .First()
                    .PermissionType = permissionType);
            responseCollection.Html("#PermissionDestination", PermissionListItem(
                siteModel,
                Types.Destination,
                selectedPermissionType_ItemIdCollection));
        }
    }
}
