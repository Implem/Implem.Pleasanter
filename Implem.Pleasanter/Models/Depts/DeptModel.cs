using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Models
{
    public class DeptModel : BaseModel, Interfaces.IConvertable
    {
        public int TenantId = Sessions.TenantId();
        public int DeptId = 0;
        public int ParentDeptId = 0;
        public string DeptCode = string.Empty;
        public string DeptName = string.Empty;
        public string Body = string.Empty;
        public DeptModel ParentDept { get { return SiteInfo.DeptModel(ParentDeptId); } }
        public DeptModel Dept { get { return SiteInfo.DeptModel(DeptId); } }
        public Title Title { get { return new Title(DeptId, DeptName); } }
        public int SavedTenantId = Sessions.TenantId();
        public int SavedDeptId = 0;
        public int SavedParentDeptId = 0;
        public string SavedDeptCode = string.Empty;
        public string SavedDeptName = string.Empty;
        public string SavedBody = string.Empty;
        public bool TenantId_Updated { get { return TenantId != SavedTenantId; } }
        public bool DeptId_Updated { get { return DeptId != SavedDeptId; } }
        public bool ParentDeptId_Updated { get { return ParentDeptId != SavedParentDeptId; } }
        public bool DeptCode_Updated { get { return DeptCode != SavedDeptCode && DeptCode != null; } }
        public bool DeptName_Updated { get { return DeptName != SavedDeptName && DeptName != null; } }
        public bool Body_Updated { get { return Body != SavedBody && Body != null; } }
        public List<int> SwitchTargets;

        public DeptModel()
        {
        }

        public DeptModel(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            bool setByForm = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            SiteSettings = siteSettings;
            PermissionType = permissionType;
            if (setByForm) SetByForm();
            MethodType = methodType;
            OnConstructed();
        }

        public DeptModel(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            int deptId,
            bool clearSessions = false,
            bool setByForm = false,
            List<int> switchTargets = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            SiteSettings = siteSettings;
            DeptId = deptId;
            PermissionType = permissionType;
            Get();
            if (clearSessions) ClearSessions();
            if (setByForm) SetByForm();
            SwitchTargets = switchTargets;
            MethodType = methodType;
            OnConstructed();
        }

        public DeptModel(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            DataRow dataRow)
        {
            OnConstructing();
            SiteSettings = siteSettings;
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

        public DeptModel Get(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            Set(Rds.ExecuteTable(statements: Rds.SelectDepts(
                tableType: tableType,
                column: column ?? Rds.DeptsDefaultColumns(),
                join: join ??  Rds.DeptsJoinDefault(),
                where: where ?? Rds.DeptsWhereDefault(this),
                orderBy: orderBy ?? null,
                param: param ?? null,
                distinct: distinct,
                top: top)));
            return this;
        }

        public string Create(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool paramAll = false)
        {
            var error = ValidateBeforeCreate();
            if (error != null) return error;
            OnCreating();
            var newId = Rds.ExecuteScalar_int(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.InsertDepts(
                        tableType: tableType,
                        selectIdentity: true,
                        param: param ?? Rds.DeptsParamDefault(
                            this, setDefault: true, paramAll: paramAll))
                });
            DeptId = newId != 0 ? newId : DeptId;
            Get();
            OnCreated();
            return RecordResponse(this, Messages.Created(Title.ToString()));
        }

        private void OnCreating()
        {
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OnCreated()
        {
            SiteInfo.SetDept(this);
        }

        private string ValidateBeforeCreate()
        {
            if (!PermissionType.CanEditTenant())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Depts_TenantId": if (!SiteSettings.AllColumn("TenantId").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Depts_DeptId": if (!SiteSettings.AllColumn("DeptId").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Depts_Ver": if (!SiteSettings.AllColumn("Ver").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Depts_ParentDeptId": if (!SiteSettings.AllColumn("ParentDeptId").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Depts_ParentDept": if (!SiteSettings.AllColumn("ParentDept").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Depts_DeptCode": if (!SiteSettings.AllColumn("DeptCode").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Depts_Dept": if (!SiteSettings.AllColumn("Dept").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Depts_DeptName": if (!SiteSettings.AllColumn("DeptName").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Depts_Body": if (!SiteSettings.AllColumn("Body").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Depts_Title": if (!SiteSettings.AllColumn("Title").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Depts_Comments": if (!SiteSettings.AllColumn("Comments").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Depts_Creator": if (!SiteSettings.AllColumn("Creator").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Depts_Updator": if (!SiteSettings.AllColumn("Updator").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Depts_CreatedTime": if (!SiteSettings.AllColumn("CreatedTime").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Depts_UpdatedTime": if (!SiteSettings.AllColumn("UpdatedTime").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Depts_VerUp": if (!SiteSettings.AllColumn("VerUp").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Depts_Timestamp": if (!SiteSettings.AllColumn("Timestamp").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                }
            }
            return null;
        }

        public string Update(SqlParamCollection param = null, bool paramAll = false)
        {
            var error = ValidateBeforeUpdate();
            if (error != null) return error;
            SetBySession();
            OnUpdating(ref param);
            var timestamp = Timestamp.ToDateTime();
            var count = Rds.ExecuteScalar_int(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.UpdateDepts(
                        verUp: VerUp,
                        where: Rds.DeptsWhereDefault(this)
                            .UpdatedTime(timestamp, _using: timestamp.NotZero()),
                        param: param ?? Rds.DeptsParamDefault(this, paramAll: paramAll),
                        countRecord: true)
                });
            if (count == 0) return ResponseConflicts();
            Get();
            var responseCollection = new DeptsResponseCollection(this);
            OnUpdated(ref responseCollection);
            return ResponseByUpdate(responseCollection)
                .PrependComment(Comments, VerType)
                .ToJson();
        }

        private void OnUpdating(ref SqlParamCollection param)
        {
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        /// <param name="responseCollection"></param>
        private void OnUpdated(ref DeptsResponseCollection responseCollection)
        {
            SiteInfo.SetDept(this);
        }

        private string ValidateBeforeUpdate()
        {
            if (!PermissionType.CanEditTenant())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Depts_TenantId": if (!SiteSettings.AllColumn("TenantId").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Depts_DeptId": if (!SiteSettings.AllColumn("DeptId").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Depts_Ver": if (!SiteSettings.AllColumn("Ver").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Depts_ParentDeptId": if (!SiteSettings.AllColumn("ParentDeptId").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Depts_ParentDept": if (!SiteSettings.AllColumn("ParentDept").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Depts_DeptCode": if (!SiteSettings.AllColumn("DeptCode").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Depts_Dept": if (!SiteSettings.AllColumn("Dept").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Depts_DeptName": if (!SiteSettings.AllColumn("DeptName").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Depts_Body": if (!SiteSettings.AllColumn("Body").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Depts_Title": if (!SiteSettings.AllColumn("Title").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Depts_Comments": if (!SiteSettings.AllColumn("Comments").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Depts_Creator": if (!SiteSettings.AllColumn("Creator").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Depts_Updator": if (!SiteSettings.AllColumn("Updator").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Depts_CreatedTime": if (!SiteSettings.AllColumn("CreatedTime").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Depts_UpdatedTime": if (!SiteSettings.AllColumn("UpdatedTime").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Depts_VerUp": if (!SiteSettings.AllColumn("VerUp").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Depts_Timestamp": if (!SiteSettings.AllColumn("Timestamp").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                }
            }
            return null;
        }

        private ResponseCollection ResponseByUpdate(DeptsResponseCollection responseCollection)
        {
            return responseCollection
                .Ver()
                .Timestamp()
                .Val("#VerUp", false)
                .FormResponse(this)
                .Disabled("#VerUp", false)
                .Html("#HeaderTitle", Title.Value)
                .Html("#RecordInfo", new HtmlBuilder().RecordInfo(baseModel: this, tableName: "Depts"))
                .Message(Messages.Updated(Title.ToString()))
                .RemoveComment(DeleteCommentId, _using: DeleteCommentId != 0)
                .ClearFormData();
        }

        public string UpdateOrCreate(
            SqlWhereCollection where = null,
            SqlParamCollection param = null)
        {
            if (!PermissionType.CanEditTenant())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            SetBySession();
            OnUpdatingOrCreating(ref where, ref param);
            var newId = Rds.ExecuteScalar_int(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.UpdateOrInsertDepts(
                        selectIdentity: true,
                        where: where ?? Rds.DeptsWhereDefault(this),
                        param: param ?? Rds.DeptsParamDefault(this, setDefault: true))
                });
            DeptId = newId != 0 ? newId : DeptId;
            Get();
            var responseCollection = new DeptsResponseCollection(this);
            OnUpdatedOrCreated(ref responseCollection);
            return responseCollection.ToJson();
        }

        private void OnUpdatingOrCreating(
            ref SqlWhereCollection where,
            ref SqlParamCollection param)
        {
        }

        private void OnUpdatedOrCreated(ref DeptsResponseCollection responseCollection)
        {
        }

        public string Delete(bool redirect = true)
        {
            if (!PermissionType.CanEditTenant())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            OnDeleting();
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.DeleteDepts(
                        where: Rds.DeptsWhere().DeptId(DeptId))
                });
            Sessions.Set("Message", Messages.Deleted(Title.Value).Html);
            var responseCollection = new DeptsResponseCollection(this);
            OnDeleted(ref responseCollection);
            if (redirect)
            {
                responseCollection.Href(Navigations.Index("Depts"));
            }
            return responseCollection.ToJson();
        }

        private void OnDeleting()
        {
        }

        private void OnDeleted(ref DeptsResponseCollection responseCollection)
        {
        }

        public string Restore(int deptId)
        {
            if (!Permissions.Admins().CanEditTenant())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            DeptId = deptId;
            Rds.ExecuteNonQuery(
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreDepts(
                        where: Rds.DeptsWhere().DeptId(DeptId))
                });
            return new ResponseCollection().ToJson();
        }

        public string PhysicalDelete(Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            if (!PermissionType.CanEditTenant())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            OnPhysicalDeleting();
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: Rds.PhysicalDeleteDepts(
                    tableType: tableType,
                    param: Rds.DeptsParam().DeptId(DeptId)));
            var responseCollection = new DeptsResponseCollection(this);
            OnPhysicalDeleted(ref responseCollection);
            return responseCollection.ToJson();
        }

        private void OnPhysicalDeleting()
        {
        }

        private void OnPhysicalDeleted(ref DeptsResponseCollection responseCollection)
        {
        }

        public string Histories()
        {
            var hb = new HtmlBuilder();
            hb.Table(
                attributes: new HtmlAttributes().Class("grid"),
                action: () =>
                {
                    hb.GridHeader(
                        columnCollection: SiteSettings.HistoryColumnCollection(),
                        sort: false,
                        checkRow: false);
                    new DeptCollection(
                        siteSettings: SiteSettings,
                        permissionType: PermissionType,
                        where: Rds.DeptsWhere().DeptId(DeptId),
                        orderBy: Rds.DeptsOrderBy().Ver(SqlOrderBy.Types.desc),
                        tableType: Sqls.TableTypes.NormalAndHistory).ForEach(deptModel => hb
                            .Tr(
                                attributes: new HtmlAttributes()
                                    .Class("grid-row history not-link")
                                    .DataAction("History")
                                    .DataMethod("post")
                                    .Add("data-ver", deptModel.Ver)
                                    .Add("data-latest", 1, _using: deptModel.Ver == Ver),
                                action: () =>
                                    SiteSettings.HistoryColumnCollection().ForEach(column =>
                                        hb.TdValue(column, deptModel))));
                });
            return new DeptsResponseCollection(this).Html("#FieldSetHistories", hb).ToJson();
        }

        public string History()
        {
            Get(
                where: Rds.DeptsWhere()
                    .DeptId(DeptId)
                    .Ver(Forms.Int("Ver")),
                tableType: Sqls.TableTypes.NormalAndHistory);
            VerType =  Forms.Bool("Latest")
                ? Versions.VerTypes.Latest
                : Versions.VerTypes.History;
            SwitchTargets = DeptUtilities.GetSwitchTargets(SiteSettings);
            return Editor();
        }

        public string Previous()
        {
            var switchTargets = DeptUtilities.GetSwitchTargets(SiteSettings);
            var deptModel = new DeptModel(
                siteSettings: SiteSettings,
                permissionType: PermissionType,
                deptId: switchTargets.Previous(DeptId),
                switchTargets: switchTargets);
            return RecordResponse(deptModel);
        }

        public string Next()
        {
            var switchTargets = DeptUtilities.GetSwitchTargets(SiteSettings);
            var deptModel = new DeptModel(
                siteSettings: SiteSettings,
                permissionType: PermissionType,
                deptId: switchTargets.Next(DeptId),
                switchTargets: switchTargets);
            return RecordResponse(deptModel);
        }

        public string Reload()
        {
            SwitchTargets = DeptUtilities.GetSwitchTargets(SiteSettings);
            return RecordResponse(this, pushState: false);
        }

        private string RecordResponse(
            DeptModel deptModel, Message message = null, bool pushState = true)
        {
            deptModel.MethodType = BaseModel.MethodTypes.Edit;
            return new DeptsResponseCollection(this)
                .Func("clearDialogs")
                .ReplaceAll(
                    "#MainContainer",
                    deptModel.AccessStatus == Databases.AccessStatuses.Selected
                        ? DeptUtilities.Editor(deptModel, byRest: true)
                        : DeptUtilities.Editor(this, byRest: true))
                .Message(message)
                .PushState(
                    "Edit",
                    Navigations.Edit("Depts", deptModel.DeptId),
                    _using: pushState)
                .ClearFormData()
                .ToJson();
        }

        private void SetByForm()
        {
            Forms.Keys().ForEach(controlId =>
            {
                switch (controlId)
                {
                    case "Depts_ParentDeptId": ParentDeptId = Forms.Data(controlId).ToInt(); break;
                    case "Depts_DeptCode": DeptCode = Forms.Data(controlId).ToString(); break;
                    case "Depts_DeptName": DeptName = Forms.Data(controlId).ToString(); break;
                    case "Depts_Body": Body = Forms.Data(controlId).ToString(); break;
                    case "Depts_Timestamp": Timestamp = Forms.Data(controlId).ToString(); break;
                    case "Comments": Comments = Comments.Prepend(Forms.Data("Comments")); break;
                    case "VerUp": VerUp = Forms.Data(controlId).ToBool(); break;
                    default: break;
                }
            });
            if (Routes.Action() == "deletecomment")
            {
                DeleteCommentId = Forms.Data("ControlId").Split(',')._2nd().ToInt();
                Comments.RemoveAll(o => o.CommentId == DeleteCommentId);
            }
            Forms.FileKeys().ForEach(controlId =>
            {
                switch (controlId)
                {
                    default: break;
                }
            });
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
                    case "TenantId": if (dataRow[name] != DBNull.Value) { TenantId = dataRow[name].ToInt(); SavedTenantId = TenantId; } break;
                    case "DeptId": if (dataRow[name] != DBNull.Value) { DeptId = dataRow[name].ToInt(); SavedDeptId = DeptId; } break;
                    case "Ver": Ver = dataRow[name].ToInt(); SavedVer = Ver; break;
                    case "ParentDeptId": ParentDeptId = dataRow[name].ToInt(); SavedParentDeptId = ParentDeptId; break;
                    case "DeptCode": DeptCode = dataRow[name].ToString(); SavedDeptCode = DeptCode; break;
                    case "DeptName": DeptName = dataRow[name].ToString(); SavedDeptName = DeptName; break;
                    case "Body": Body = dataRow[name].ToString(); SavedBody = Body; break;
                    case "Comments": Comments = dataRow["Comments"].ToString().Deserialize<Comments>() ?? new Comments(); SavedComments = Comments.ToJson(); break;
                    case "Creator": Creator = SiteInfo.User(dataRow.Int(name)); SavedCreator = Creator.Id; break;
                    case "Updator": Updator = SiteInfo.User(dataRow.Int(name)); SavedUpdator = Updator.Id; break;
                    case "CreatedTime": CreatedTime = new Time(dataRow, "CreatedTime"); SavedCreatedTime = CreatedTime.Value; break;
                    case "UpdatedTime": UpdatedTime = new Time(dataRow, "UpdatedTime"); Timestamp = dataRow.Field<DateTime>("UpdatedTime").ToString("yyyy/M/d H:m:s.fff"); SavedUpdatedTime = UpdatedTime.Value; break;
                    case "IsHistory": VerType = dataRow[name].ToBool() ? Versions.VerTypes.History : Versions.VerTypes.Latest; break;
                }
            }
        }

        private string Editor()
        {
            return new DeptsResponseCollection(this)
                .ReplaceAll(
                    "#MainContainer",
                    DeptUtilities.Editor(this, byRest: true))
                .ToJson();
        }

        private string ResponseConflicts()
        {
            Get();
            return AccessStatus == Databases.AccessStatuses.Selected
                ? Messages.ResponseUpdateConflicts(Updator.FullName()).ToJson()
                : Messages.ResponseDeleteConflicts().ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string ToControl(Column column, Permissions.Types permissionType)
        {
            return string.Empty;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string ToResponse()
        {
            return string.Empty;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public HtmlBuilder Td(HtmlBuilder hb, Column column)
        {
            return hb.Td(action: () => hb
                .HtmlDept(DeptId));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string ToExport(Column column)
        {
            return DeptName;
        }
    }
}
