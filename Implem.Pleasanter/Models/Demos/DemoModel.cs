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
    public class DemoModel : BaseModel
    {
        public int DemoId = 0;
        public int TenantId = 0;
        public Title Title = new Title();
        public string Passphrase = string.Empty;
        public string MailAddress = string.Empty;
        public bool Initialized = false;
        public int TimeLag = 0;
        public int SavedDemoId = 0;
        public int SavedTenantId = 0;
        public string SavedTitle = string.Empty;
        public string SavedPassphrase = string.Empty;
        public string SavedMailAddress = string.Empty;
        public bool SavedInitialized = false;
        public int SavedTimeLag = 0;
        public bool DemoId_Updated { get { return DemoId != SavedDemoId; } }
        public bool TenantId_Updated { get { return TenantId != SavedTenantId; } }
        public bool Title_Updated { get { return Title.Value != SavedTitle && Title.Value != null; } }
        public bool Passphrase_Updated { get { return Passphrase != SavedPassphrase && Passphrase != null; } }
        public bool MailAddress_Updated { get { return MailAddress != SavedMailAddress && MailAddress != null; } }
        public bool Initialized_Updated { get { return Initialized != SavedInitialized; } }
        public List<int> SwitchTargets;

        /// <summary>
        /// Fixed:
        /// </summary>
        public DemoModel()
        {
        }

        public DemoModel(
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

        public DemoModel(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            int demoId,
            bool clearSessions = false,
            bool setByForm = false,
            List<int> switchTargets = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            SiteSettings = siteSettings;
            DemoId = demoId;
            PermissionType = permissionType;
            Get();
            if (clearSessions) ClearSessions();
            if (setByForm) SetByForm();
            SwitchTargets = switchTargets;
            MethodType = methodType;
            OnConstructed();
        }

        public DemoModel(
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

        public DemoModel Get(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            Set(Rds.ExecuteTable(statements: Rds.SelectDemos(
                tableType: tableType,
                column: column ?? Rds.DemosDefaultColumns(),
                join: join ??  Rds.DemosJoinDefault(),
                where: where ?? Rds.DemosWhereDefault(this),
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
                    Rds.InsertDemos(
                        tableType: tableType,
                        selectIdentity: true,
                        param: param ?? Rds.DemosParamDefault(
                            this, setDefault: true, paramAll: paramAll))
                });
            DemoId = newId != 0 ? newId : DemoId;
            Get();
            OnCreated();
            return EditorJson(this, Messages.Created(Title.ToString()));
        }

        private void OnCreating()
        {
        }

        private void OnCreated()
        {
        }

        private string ValidateBeforeCreate()
        {
            if (!PermissionType.CanEditSys())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Demos_DemoId": if (!SiteSettings.GetColumn("DemoId").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_Ver": if (!SiteSettings.GetColumn("Ver").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_TenantId": if (!SiteSettings.GetColumn("TenantId").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_Title": if (!SiteSettings.GetColumn("Title").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_Passphrase": if (!SiteSettings.GetColumn("Passphrase").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_MailAddress": if (!SiteSettings.GetColumn("MailAddress").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_Initialized": if (!SiteSettings.GetColumn("Initialized").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_TimeLag": if (!SiteSettings.GetColumn("TimeLag").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_Comments": if (!SiteSettings.GetColumn("Comments").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_Creator": if (!SiteSettings.GetColumn("Creator").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_Updator": if (!SiteSettings.GetColumn("Updator").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_CreatedTime": if (!SiteSettings.GetColumn("CreatedTime").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_UpdatedTime": if (!SiteSettings.GetColumn("UpdatedTime").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_VerUp": if (!SiteSettings.GetColumn("VerUp").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_Timestamp": if (!SiteSettings.GetColumn("Timestamp").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
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
                    Rds.UpdateDemos(
                        verUp: VerUp,
                        where: Rds.DemosWhereDefault(this)
                            .UpdatedTime(timestamp, _using: timestamp.InRange()),
                        param: param ?? Rds.DemosParamDefault(this, paramAll: paramAll),
                        countRecord: true)
                });
            if (count == 0) return ResponseConflicts();
            Get();
            var responseCollection = new DemosResponseCollection(this);
            OnUpdated(ref responseCollection);
            return ResponseByUpdate(responseCollection)
                .PrependComment(Comments, VerType)
                .ToJson();
        }

        private void OnUpdating(ref SqlParamCollection param)
        {
        }

        private void OnUpdated(ref DemosResponseCollection responseCollection)
        {
        }

        private string ValidateBeforeUpdate()
        {
            if (!PermissionType.CanEditSys())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Demos_DemoId": if (!SiteSettings.GetColumn("DemoId").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_Ver": if (!SiteSettings.GetColumn("Ver").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_TenantId": if (!SiteSettings.GetColumn("TenantId").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_Title": if (!SiteSettings.GetColumn("Title").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_Passphrase": if (!SiteSettings.GetColumn("Passphrase").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_MailAddress": if (!SiteSettings.GetColumn("MailAddress").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_Initialized": if (!SiteSettings.GetColumn("Initialized").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_TimeLag": if (!SiteSettings.GetColumn("TimeLag").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_Comments": if (!SiteSettings.GetColumn("Comments").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_Creator": if (!SiteSettings.GetColumn("Creator").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_Updator": if (!SiteSettings.GetColumn("Updator").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_CreatedTime": if (!SiteSettings.GetColumn("CreatedTime").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_UpdatedTime": if (!SiteSettings.GetColumn("UpdatedTime").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_VerUp": if (!SiteSettings.GetColumn("VerUp").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_Timestamp": if (!SiteSettings.GetColumn("Timestamp").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                }
            }
            return null;
        }

        private ResponseCollection ResponseByUpdate(DemosResponseCollection responseCollection)
        {
            return responseCollection
                .Ver()
                .Timestamp()
                .Val("#VerUp", false)
                .FormResponse(this)
                .Disabled("#VerUp", false)
                .Html("#HeaderTitle", Title.Value)
                .Html("#RecordInfo", new HtmlBuilder().RecordInfo(baseModel: this, tableName: "Demos"))
                .Message(Messages.Updated(Title.ToString()))
                .RemoveComment(DeleteCommentId, _using: DeleteCommentId != 0)
                .ClearFormData();
        }

        public string UpdateOrCreate(
            SqlWhereCollection where = null,
            SqlParamCollection param = null)
        {
            if (!PermissionType.CanEditSys())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            SetBySession();
            OnUpdatingOrCreating(ref where, ref param);
            var newId = Rds.ExecuteScalar_int(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.UpdateOrInsertDemos(
                        selectIdentity: true,
                        where: where ?? Rds.DemosWhereDefault(this),
                        param: param ?? Rds.DemosParamDefault(this, setDefault: true))
                });
            DemoId = newId != 0 ? newId : DemoId;
            Get();
            var responseCollection = new DemosResponseCollection(this);
            OnUpdatedOrCreated(ref responseCollection);
            return responseCollection.ToJson();
        }

        private void OnUpdatingOrCreating(
            ref SqlWhereCollection where,
            ref SqlParamCollection param)
        {
        }

        private void OnUpdatedOrCreated(ref DemosResponseCollection responseCollection)
        {
        }

        public string Delete(bool redirect = true)
        {
            if (!PermissionType.CanEditSys())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            OnDeleting();
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.DeleteDemos(
                        where: Rds.DemosWhere().DemoId(DemoId))
                });
            Sessions.Set("Message", Messages.Deleted(Title.Value).Html);
            var responseCollection = new DemosResponseCollection(this);
            OnDeleted(ref responseCollection);
            if (redirect)
            {
                responseCollection.Href(Navigations.Index("Demos"));
            }
            return responseCollection.ToJson();
        }

        private void OnDeleting()
        {
        }

        private void OnDeleted(ref DemosResponseCollection responseCollection)
        {
        }

        public string Restore(int demoId)
        {
            if (!Permissions.Admins().CanEditTenant())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            DemoId = demoId;
            Rds.ExecuteNonQuery(
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreDemos(
                        where: Rds.DemosWhere().DemoId(DemoId))
                });
            return new ResponseCollection().ToJson();
        }

        public string PhysicalDelete(Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            if (!PermissionType.CanEditSys())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            OnPhysicalDeleting();
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: Rds.PhysicalDeleteDemos(
                    tableType: tableType,
                    param: Rds.DemosParam().DemoId(DemoId)));
            var responseCollection = new DemosResponseCollection(this);
            OnPhysicalDeleted(ref responseCollection);
            return responseCollection.ToJson();
        }

        private void OnPhysicalDeleting()
        {
        }

        private void OnPhysicalDeleted(ref DemosResponseCollection responseCollection)
        {
        }

        public string Histories()
        {
            var hb = new HtmlBuilder();
            hb.Table(
                attributes: new HtmlAttributes().Class("grid"),
                action: () => hb
                    .THead(action: () => hb
                        .GridHeader(
                            columnCollection: SiteSettings.HistoryColumnCollection(),
                            sort: false,
                            checkRow: false))
                    .TBody(action: () =>
                        new DemoCollection(
                            siteSettings: SiteSettings,
                            permissionType: PermissionType,
                            where: Rds.DemosWhere().DemoId(DemoId),
                            orderBy: Rds.DemosOrderBy().Ver(SqlOrderBy.Types.desc),
                            tableType: Sqls.TableTypes.NormalAndHistory)
                                .ForEach(demoModel => hb
                                    .Tr(
                                        attributes: new HtmlAttributes()
                                            .Class("grid-row history not-link")
                                            .DataAction("History")
                                            .DataMethod("post")
                                            .DataVer(demoModel.Ver)
                                            .DataLatest(1, _using: demoModel.Ver == Ver),
                                        action: () =>
                                            SiteSettings.HistoryColumnCollection()
                                                .ForEach(column => hb
                                                    .TdValue(column, demoModel))))));
            return new DemosResponseCollection(this).Html("#FieldSetHistories", hb).ToJson();
        }

        public string History()
        {
            Get(
                where: Rds.DemosWhere()
                    .DemoId(DemoId)
                    .Ver(Forms.Int("Ver")),
                tableType: Sqls.TableTypes.NormalAndHistory);
            VerType =  Forms.Bool("Latest")
                ? Versions.VerTypes.Latest
                : Versions.VerTypes.History;
            SwitchTargets = DemoUtilities.GetSwitchTargets(SiteSettings);
            return Editor();
        }

        public string EditorJson()
        {
            return EditorJson(this);
        }

        private string EditorJson(DemoModel demoModel, Message message = null)
        {
            demoModel.MethodType = MethodTypes.Edit;
            return new DemosResponseCollection(this)
                .Invoke("clearDialogs")
                .ReplaceAll(
                    "#MainContainer",
                    demoModel.AccessStatus == Databases.AccessStatuses.Selected
                        ? DemoUtilities.Editor(demoModel, byRest: true)
                        : DemoUtilities.Editor(this, byRest: true))
                .Invoke("setCurrentIndex")
                .Invoke("validateDemos")
                .Message(message)
                .ClearFormData()
                .ToJson();
        }

        private void SetByForm()
        {
            Forms.Keys().ForEach(controlId =>
            {
                switch (controlId)
                {
                    case "Demos_TenantId": TenantId = Forms.Data(controlId).ToInt(); break;
                    case "Demos_Title": Title = new Title(DemoId, Forms.Data(controlId)); break;
                    case "Demos_Passphrase": Passphrase = Forms.Data(controlId).ToString(); break;
                    case "Demos_MailAddress": MailAddress = Forms.Data(controlId).ToString(); break;
                    case "Demos_Initialized": Initialized = Forms.Data(controlId).ToBool(); break;
                    case "Demos_TimeLag": TimeLag = Forms.Data(controlId).ToInt(); break;
                    case "Demos_Timestamp": Timestamp = Forms.Data(controlId).ToString(); break;
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
                    case "DemoId": if (dataRow[name] != DBNull.Value) { DemoId = dataRow[name].ToInt(); SavedDemoId = DemoId; } break;
                    case "Ver": Ver = dataRow[name].ToInt(); SavedVer = Ver; break;
                    case "TenantId": TenantId = dataRow[name].ToInt(); SavedTenantId = TenantId; break;
                    case "Title": Title = new Title(dataRow, "DemoId"); SavedTitle = Title.Value; break;
                    case "Passphrase": Passphrase = dataRow[name].ToString(); SavedPassphrase = Passphrase; break;
                    case "MailAddress": MailAddress = dataRow[name].ToString(); SavedMailAddress = MailAddress; break;
                    case "Initialized": Initialized = dataRow[name].ToBool(); SavedInitialized = Initialized; break;
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
            return new DemosResponseCollection(this)
                .ReplaceAll(
                    "#MainContainer",
                    DemoUtilities.Editor(this, byRest: true))
                .Invoke("validateDemos")
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
        public void InitializeTimeLag()
        {
            TimeLag = (
                DateTime.Now -
                Def.DemoDefinitionCollection
                    .Where(o => o.UpdatedTime >= Parameters.General.MinTime)
                    .Select(o => o.UpdatedTime).Max()).Days - 1;
        }
    }
}
