using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Analysis;
using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.ServerData;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.Styles;
using Implem.Pleasanter.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Views;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
namespace Implem.Pleasanter.Models
{
    public class ExportSettingModel : BaseModel
    {
        public string ReferenceType = "Sites";
        public long ReferenceId = 0;
        public Title Title = new Title();
        public long ExportSettingId = 0;
        public bool AddHeader = true;
        public ExportColumns ExportColumns = new ExportColumns();
        public string SavedReferenceType = "Sites";
        public long SavedReferenceId = 0;
        public string SavedTitle = string.Empty;
        public long SavedExportSettingId = 0;
        public bool SavedAddHeader = true;
        public string SavedExportColumns = string.Empty;
        public bool ReferenceType_Updated { get { return ReferenceType != SavedReferenceType && ReferenceType != null; } }
        public bool ReferenceId_Updated { get { return ReferenceId != SavedReferenceId; } }
        public bool Title_Updated { get { return Title.Value != SavedTitle && Title.Value != null; } }
        public bool ExportSettingId_Updated { get { return ExportSettingId != SavedExportSettingId; } }
        public bool AddHeader_Updated { get { return AddHeader != SavedAddHeader; } }
        public bool ExportColumns_Updated { get { return ExportColumns.ToJson() != SavedExportColumns && ExportColumns.ToJson() != null; } }

        public Title Session_Title()
        {
            return this.PageSession("Title") != null
                ? this.PageSession("Title") as Title
                : Title;
        }

        public void  Session_Title(object value)
        {
            this.PageSession("Title", value);
        }

        public bool Session_AddHeader()
        {
            return this.PageSession("AddHeader") != null
                ? this.PageSession("AddHeader").ToBool()
                : AddHeader;
        }

        public void  Session_AddHeader(object value)
        {
            this.PageSession("AddHeader", value);
        }

        public ExportColumns Session_ExportColumns()
        {
            return this.PageSession("ExportColumns") != null
                ? this.PageSession("ExportColumns")?.ToString().Deserialize<ExportColumns>() ?? new ExportColumns(ReferenceType)
                : ExportColumns;
        }

        public void  Session_ExportColumns(object value)
        {
            this.PageSession("ExportColumns", value);
        }

        public List<long> SwitchTargets;

        public ExportSettingModel(
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

        public ExportSettingModel(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            long exportSettingId,
            bool clearSessions = false,
            bool setByForm = false,
            List<long> switchTargets = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            SiteSettings = siteSettings;
            ExportSettingId = exportSettingId;
            PermissionType = permissionType;
            Get();
            if (clearSessions) ClearSessions();
            if (setByForm) SetByForm();
            SwitchTargets = switchTargets;
            MethodType = methodType;
            OnConstructed();
        }

        public ExportSettingModel(
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
            Session_Title(null);
            Session_AddHeader(null);
            Session_ExportColumns(null);
        }

        public ExportSettingModel Get(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            Set(Rds.ExecuteTable(statements: Rds.SelectExportSettings(
                tableType: tableType,
                column: column ?? Rds.ExportSettingsColumnDefault(),
                join: join ??  Rds.ExportSettingsJoinDefault(),
                where: where ?? Rds.ExportSettingsWhereDefault(this),
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
            var newId = Rds.ExecuteScalar_long(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.InsertExportSettings(
                        tableType: tableType,
                        selectIdentity: true,
                        param: param ?? Rds.ExportSettingsParamDefault(
                            this, setDefault: true, paramAll: paramAll))
                });
            ExportSettingId = newId != 0 ? newId : ExportSettingId;
            Get();
            OnCreated();
            return RecordResponse(this, Messages.Created(Title.ToString()));
        }

        private void OnCreating()
        {
        }

        private void OnCreated()
        {
        }

        private string ValidateBeforeCreate()
        {
            if (!PermissionType.CanExport())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "ExportSettings_ReferenceType": if (!SiteSettings.AllColumn("ReferenceType").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "ExportSettings_ReferenceId": if (!SiteSettings.AllColumn("ReferenceId").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "ExportSettings_Title": if (!SiteSettings.AllColumn("Title").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "ExportSettings_ExportSettingId": if (!SiteSettings.AllColumn("ExportSettingId").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "ExportSettings_Ver": if (!SiteSettings.AllColumn("Ver").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "ExportSettings_AddHeader": if (!SiteSettings.AllColumn("AddHeader").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "ExportSettings_ExportColumns": if (!SiteSettings.AllColumn("ExportColumns").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "ExportSettings_Comments": if (!SiteSettings.AllColumn("Comments").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "ExportSettings_Creator": if (!SiteSettings.AllColumn("Creator").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "ExportSettings_Updator": if (!SiteSettings.AllColumn("Updator").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "ExportSettings_CreatedTime": if (!SiteSettings.AllColumn("CreatedTime").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "ExportSettings_UpdatedTime": if (!SiteSettings.AllColumn("UpdatedTime").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "ExportSettings_VerUp": if (!SiteSettings.AllColumn("VerUp").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "ExportSettings_Timestamp": if (!SiteSettings.AllColumn("Timestamp").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
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
                    Rds.UpdateExportSettings(
                        verUp: VerUp,
                        where: Rds.ExportSettingsWhereDefault(this)
                            .UpdatedTime(timestamp, _using: timestamp.NotZero()),
                        param: param ?? Rds.ExportSettingsParamDefault(this, paramAll: paramAll),
                        countRecord: true)
                });
            if (count == 0) return ResponseConflicts();
            Get();
            var responseCollection = new ExportSettingsResponseCollection(this);
            OnUpdated(ref responseCollection);
            return ResponseByUpdate(responseCollection)
                .PrependComment(Comments, VerType)
                .ToJson();
        }

        private void OnUpdating(ref SqlParamCollection param)
        {
        }

        private void OnUpdated(ref ExportSettingsResponseCollection responseCollection)
        {
        }

        public string DeleteComment()
        {
            var error = ValidateBeforeUpdate();
            if (error != null) return error;
            var commentId = Forms.Data("ControlId").Split(',')._2nd();
            Comments.RemoveAll(o => o.CommentId.ToString() == commentId);
            if (Rds.ExecuteScalar_int(
                transactional: true,
                statements: Rds.UpdateExportSettings(
                    verUp: VerUp,
                    where: Rds.ExportSettingsWhereDefault(this)
                        .UpdatedTime(Forms.DateTime("ExportSettings_Timestamp")),
                    param: Rds.ExportSettingsParamDefault(this),
                    countRecord: true)) == 0)
            {
                return ResponseConflicts();
            }
            Get();
            return ResponseByUpdate(new ExportSettingsResponseCollection(this))
                .RemoveComment(commentId)
                .ToJson();
        }

        private string ValidateBeforeUpdate()
        {
            if (!PermissionType.CanExport())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "ExportSettings_ReferenceType": if (!SiteSettings.AllColumn("ReferenceType").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "ExportSettings_ReferenceId": if (!SiteSettings.AllColumn("ReferenceId").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "ExportSettings_Title": if (!SiteSettings.AllColumn("Title").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "ExportSettings_ExportSettingId": if (!SiteSettings.AllColumn("ExportSettingId").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "ExportSettings_Ver": if (!SiteSettings.AllColumn("Ver").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "ExportSettings_AddHeader": if (!SiteSettings.AllColumn("AddHeader").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "ExportSettings_ExportColumns": if (!SiteSettings.AllColumn("ExportColumns").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "ExportSettings_Comments": if (!SiteSettings.AllColumn("Comments").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "ExportSettings_Creator": if (!SiteSettings.AllColumn("Creator").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "ExportSettings_Updator": if (!SiteSettings.AllColumn("Updator").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "ExportSettings_CreatedTime": if (!SiteSettings.AllColumn("CreatedTime").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "ExportSettings_UpdatedTime": if (!SiteSettings.AllColumn("UpdatedTime").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "ExportSettings_VerUp": if (!SiteSettings.AllColumn("VerUp").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "ExportSettings_Timestamp": if (!SiteSettings.AllColumn("Timestamp").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                }
            }
            return null;
        }

        private ResponseCollection ResponseByUpdate(ExportSettingsResponseCollection responseCollection)
        {
            return responseCollection
                .Ver()
                .Timestamp()
                .Val("#VerUp", false)
                .Disabled("#VerUp", false)
                .Html("#HeaderTitle", Title.DisplayValue + " - " + Displays.Edit())
                .Html("#RecordInfo", Html.Builder().RecordInfo(baseModel: this, tableName: "ExportSettings"))
                .Html("#RecordHistories", Html.Builder().RecordHistories(ver: Ver, verType: VerType))
                .Message(Messages.Updated(Title.ToString()))
                .ClearFormData();
        }

        public string UpdateOrCreate(
            SqlWhereCollection where = null,
            SqlParamCollection param = null)
        {
            if (!PermissionType.CanExport())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            SetBySession();
            OnUpdatingOrCreating(ref where, ref param);
            var newId = Rds.ExecuteScalar_long(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.UpdateOrInsertExportSettings(
                        selectIdentity: true,
                        where: where ?? Rds.ExportSettingsWhereDefault(this),
                        param: param ?? Rds.ExportSettingsParamDefault(this, setDefault: true))
                });
            ExportSettingId = newId != 0 ? newId : ExportSettingId;
            Get();
            var responseCollection = new ExportSettingsResponseCollection(this);
            OnUpdatedOrCreated(ref responseCollection);
            return responseCollection.ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OnUpdatingOrCreating(
            ref SqlWhereCollection where,
            ref SqlParamCollection param)
        {
            if (ExportColumns.ReferenceType.IsNullOrEmpty())
            {
                ExportColumns.ReferenceType = ReferenceType;
            }
            if (Forms.Data("ExportSettings_Title") == string.Empty)
            {
                Title = new Title(0, Unique.New(
                    new ExportSettingCollection(
                        SiteSettingsUtility.ExportSettingsSiteSettings(),
                        Permissions.Types.NotSet,
                        where: Rds.ExportSettingsWhere()
                            .ReferenceId(ReferenceId))
                                .Select(o => o.Title?.Value),
                    Displays.Setting()));
            }
            where = Rds.ExportSettingsWhere()
                .ReferenceId(ReferenceId)
                .Title(Title.Value);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OnUpdatedOrCreated(ref ExportSettingsResponseCollection responseCollection)
        {
            var exportSettingsCollection = ExportSettingsUtility
                .Collection(ReferenceType, ReferenceId);
            responseCollection
                .Html(
                    "#ExportSettings_ExportSettingId",
                    Html.Builder().OptionCollection(
                    optionCollection: exportSettingsCollection.ToDictionary(
                        o => o.ExportSettingId.ToString(),
                        o => new ControlData(o.Title.Value)),
                    selectedValue: ExportSettingId.ToString(),
                    addSelectedValue: false))
                .Val("#ExportSettings_Title", Title.Value);
        }

        public string Delete(bool redirect = true)
        {
            if (!PermissionType.CanExport())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            OnDeleting();
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.DeleteExportSettings(
                        where: Rds.ExportSettingsWhere().ExportSettingId(ExportSettingId))
                });
            var responseCollection = new ExportSettingsResponseCollection(this);
            OnDeleted(ref responseCollection);
            if (redirect)
            {
                responseCollection.Href(Navigations.Index("ExportSettings"));
            }
            return responseCollection.ToJson();
        }

        private void OnDeleting()
        {
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OnDeleted(ref ExportSettingsResponseCollection responseCollection)
        {
            responseCollection.Edit(ReferenceType, ReferenceId);
        }

        public string PhysicalDelete(Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            if (!PermissionType.CanExport())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            OnPhysicalDeleting();
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: Rds.PhysicalDeleteExportSettings(
                    tableType: tableType,
                    param: Rds.ExportSettingsParam().ExportSettingId(ExportSettingId)));
            var responseCollection = new ExportSettingsResponseCollection(this);
            OnPhysicalDeleted(ref responseCollection);
            return responseCollection.ToJson();
        }

        private void OnPhysicalDeleting()
        {
        }

        private void OnPhysicalDeleted(ref ExportSettingsResponseCollection responseCollection)
        {
        }

        public string Histories()
        {
            var hb = Html.Builder();
            hb.Table(
                attributes: Html.Attributes().Class("grid"),
                action: () =>
                {
                    hb.GridHeader(
                        columnCollection: SiteSettings.HistoryGridColumnCollection(),
                        sort: false,
                        checkRow: false);
                    new ExportSettingCollection(
                        siteSettings: SiteSettings,
                        permissionType: PermissionType,
                        where: Rds.ExportSettingsWhere().ExportSettingId(ExportSettingId),
                        orderBy: Rds.ExportSettingsOrderBy().UpdatedTime(SqlOrderBy.Types.desc),
                        tableType: Sqls.TableTypes.NormalAndHistory).ForEach(exportSettingModel => hb
                            .Tr(
                                attributes: Html.Attributes()
                                    .Class("grid-row not-link")
                                    .OnClick(Def.JavaScript.HistoryAndCloseDialog
                                        .Params(exportSettingModel.Ver))
                                    .DataAction("History")
                                    .DataMethod("post")
                                    .Add("data-latest", 1, _using: exportSettingModel.Ver == Ver),
                                action: () =>
                                    SiteSettings.HistoryGridColumnCollection().ForEach(column =>
                                        hb.TdValue(column, exportSettingModel))));
                });
            return new ExportSettingsResponseCollection(this).Html("#HistoriesForm", hb).ToJson();
        }

        public string History()
        {
            Get(
                where: Rds.ExportSettingsWhere()
                    .ExportSettingId(ExportSettingId)
                    .Ver(Forms.Int("Ver")),
                tableType: Sqls.TableTypes.NormalAndHistory);
            VerType =  Forms.Bool("Latest")
                ? Versions.VerTypes.Latest
                : Versions.VerType(ExportSettingId);
            SwitchTargets = ExportSettingsUtility.GetSwitchTargets(SiteSettings);
            return Editor();
        }

        public string PreviousHistory()
        {
            Get(
                where: Rds.ExportSettingsWhere()
                    .ExportSettingId(ExportSettingId)
                    .Ver(Forms.Int("Ver"), _operator: "<"),
                orderBy: Rds.ExportSettingsOrderBy()
                    .Ver(SqlOrderBy.Types.desc),
                tableType: Sqls.TableTypes.History,
                top: 1);
            SwitchTargets = ExportSettingsUtility.GetSwitchTargets(SiteSettings);
            switch (AccessStatus)
            {
                case Databases.AccessStatuses.Selected:
                    VerType = Versions.VerType(ExportSettingId, Versions.DirectioTypes.Previous);
                    return Editor();
                default:
                    return new ExportSettingsResponseCollection(this).ToJson();
            }
        }

        public string NextHistory()
        {
            Get(
                where: Rds.ExportSettingsWhere()
                    .ExportSettingId(ExportSettingId)
                    .Ver(Forms.Int("Ver"), _operator: ">"),
                orderBy: Rds.ExportSettingsOrderBy()
                    .Ver(SqlOrderBy.Types.asc),
                tableType: Sqls.TableTypes.History,
                top: 1);
            SwitchTargets = ExportSettingsUtility.GetSwitchTargets(SiteSettings);
            switch (AccessStatus)
            {
                case Databases.AccessStatuses.Selected:
                    VerType = Versions.VerType(ExportSettingId, Versions.DirectioTypes.Next);
                    return Editor();
                default:
                    return new ExportSettingsResponseCollection(this).ToJson();
            }
        }

        public string Previous()
        {
            var switchTargets = ExportSettingsUtility.GetSwitchTargets(SiteSettings);
            var exportSettingModel = new ExportSettingModel(
                siteSettings: SiteSettings,
                permissionType: PermissionType,
                exportSettingId: switchTargets.Previous(ExportSettingId),
                switchTargets: switchTargets);
            return RecordResponse(exportSettingModel);
        }

        public string Next()
        {
            var switchTargets = ExportSettingsUtility.GetSwitchTargets(SiteSettings);
            var exportSettingModel = new ExportSettingModel(
                siteSettings: SiteSettings,
                permissionType: PermissionType,
                exportSettingId: switchTargets.Next(ExportSettingId),
                switchTargets: switchTargets);
            return RecordResponse(exportSettingModel);
        }

        public string Reload()
        {
            SwitchTargets = ExportSettingsUtility.GetSwitchTargets(SiteSettings);
            return RecordResponse(this, pushState: false);
        }

        private string RecordResponse(
            ExportSettingModel exportSettingModel, Message message = null, bool pushState = true)
        {
            exportSettingModel.MethodType = BaseModel.MethodTypes.Edit;
            return new ExportSettingsResponseCollection(this)
                .Func("clearDialogs")
                .Html(
                    "#MainContainer",
                    exportSettingModel.AccessStatus == Databases.AccessStatuses.Selected
                        ? ExportSettingsUtility.Editor(exportSettingModel)
                        : ExportSettingsUtility.Editor(this))
                .Message(message)
                .PushState(
                    Navigations.Get("ExportSettings", ExportSettingId.ToString(), "Reload"),
                    Navigations.Edit("ExportSettings", exportSettingModel.ExportSettingId),
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
                    case "ExportSettings_ReferenceType": ReferenceType = Forms.Data(controlId).ToString(); break;
                    case "ExportSettings_ReferenceId": ReferenceId = Forms.Data(controlId).ToLong(); break;
                    case "ExportSettings_Title": Title = new Title(ExportSettingId, Forms.Data(controlId)); break;
                    case "ExportSettings_AddHeader": AddHeader = Forms.Data(controlId).ToBool(); break;
                    case "ExportSettings_Timestamp": Timestamp = Forms.Data(controlId).ToString(); break;
                    case "Comments": Comments = Comments.Prepend(Forms.Data("Comments")); break;
                    case "VerUp": VerUp = Forms.Data(controlId).ToBool(); break;
                    default: break;
                }
            });
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
            if (!Forms.HasData("ExportSettings_Title")) Title = Session_Title();
            if (!Forms.HasData("ExportSettings_AddHeader")) AddHeader = Session_AddHeader();
            if (!Forms.HasData("ExportSettings_ExportColumns")) ExportColumns = Session_ExportColumns();
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
                    case "Title": if (dataRow[name] != DBNull.Value) { Title = new Title(dataRow, "ExportSettingId"); SavedTitle = Title.Value; } break;
                    case "ExportSettingId": if (dataRow[name] != DBNull.Value) { ExportSettingId = dataRow[name].ToLong(); SavedExportSettingId = ExportSettingId; } break;
                    case "Ver": Ver = dataRow[name].ToInt(); SavedVer = Ver; break;
                    case "AddHeader": AddHeader = dataRow[name].ToBool(); SavedAddHeader = AddHeader; break;
                    case "ExportColumns": ExportColumns = dataRow.String("ExportColumns").Deserialize<ExportColumns>() ?? new ExportColumns(ReferenceType); SavedExportColumns = ExportColumns.ToJson(); break;
                    case "Comments": Comments = dataRow["Comments"].ToString().Deserialize<Comments>() ?? new Comments(); SavedComments = Comments.ToJson(); break;
                    case "Creator": Creator = SiteInfo.User(dataRow.Int(name)); SavedCreator = Creator.Id; break;
                    case "Updator": Updator = SiteInfo.User(dataRow.Int(name)); SavedUpdator = Updator.Id; break;
                    case "CreatedTime": CreatedTime = new Time(dataRow, "CreatedTime"); SavedCreatedTime = CreatedTime.Value; break;
                    case "UpdatedTime": UpdatedTime = new Time(dataRow, "UpdatedTime"); Timestamp = dataRow.Field<DateTime>("UpdatedTime").ToString("yyyy/M/d H:m:s.fff"); SavedUpdatedTime = UpdatedTime.Value; break;
                }
            }
        }

        private string Editor()
        {
            return new ExportSettingsResponseCollection(this)
                .Html("#MainContainer", ExportSettingsUtility.Editor(this))
                .ToJson();
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
        public ExportSettingModel(
            Permissions.Types permissionType,
            string referenceType, long referenceId,
            bool withTitle = false)
        {
            OnConstructing();
            PermissionType = permissionType;
            ReferenceType = referenceType;
            ReferenceId = referenceId;
            if (withTitle)
            {
                Title.Value = Forms.Data("ExportSettings_Title");
                Get(where: Rds.ExportSettingsWhere()
                    .ReferenceType(ReferenceType)
                    .ReferenceId(ReferenceId)
                    .Title(Title.Value));
            }
            else
            {
                Get(
                    where: Rds.ExportSettingsWhere()
                        .ReferenceType(ReferenceType)
                        .ReferenceId(ReferenceId),
                    orderBy: Rds.ExportSettingsOrderBy()
                        .Title(),
                    top: 1);
            }
            if (AccessStatus != Databases.AccessStatuses.Selected)
            {
                ExportColumns = new ExportColumns(ReferenceType);
            }
            SetByForm();
            OnConstructed();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public enum FormatTypes : int
        {
            Csv = 1,
            Excel = 2
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string Set()
        {
            var responseCollection = new ExportSettingsResponseCollection(this);
            ExportColumns = Session_ExportColumns();
            ExportColumns.SetExport(
                responseCollection,
                Forms.Data("ControlId"),
                Forms.Data("ExportSettings_Columns")?.Split(';'),
                GetSiteSettings());
            ExportSettingsUtility.SetSessions(this);
            return responseCollection.ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public Dictionary<string, string> ExportColumnHash()
        {
            return ExportColumns.ExportColumnHash(GetSiteSettings());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public SiteSettings GetSiteSettings()
        {
            return Url.RouteData("reference").ToLower() == "items"
                ? new ItemModel(ReferenceId).GetSite().SiteSettings
                : new SiteSettings(ReferenceType);
        }
    }

    public class ExportSettingCollection : List<ExportSettingModel>
    {
        public Databases.AccessStatuses AccessStatus = Databases.AccessStatuses.Initialized;
        public Aggregations Aggregations = new Aggregations();

        public ExportSettingCollection(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
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
            IEnumerable<Aggregation> aggregationCollection = null)
        {
            Set(siteSettings, permissionType, Get(
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

        public ExportSettingCollection(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            DataTable dataTable)
        {
            Set(siteSettings, permissionType, dataTable);
        }

        private ExportSettingCollection Set(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            DataTable dataTable)
        {
            if (dataTable.Rows.Count > 0)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    Add(new ExportSettingModel(siteSettings, permissionType, dataRow));
                }
                AccessStatus = Databases.AccessStatuses.Selected;
            }
            else
            {
                AccessStatus = Databases.AccessStatuses.NotFound;
            }
            return this;
        }

        public ExportSettingCollection(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            string commandText,
            SqlParamCollection param = null)
        {
            Set(siteSettings, permissionType, Get(commandText, param));
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
                Rds.SelectExportSettings(
                    dataTableName: "Main",
                    column: column ?? Rds.ExportSettingsColumnDefault(),
                    join: join ??  Rds.ExportSettingsJoinDefault(),
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
                statements.AddRange(Rds.ExportSettingsAggregations(aggregationCollection, where));
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
                statements: Rds.ExportSettingsStatement(
                    commandText: commandText,
                    param: param ?? null));
        }
    }

    public static class ExportSettingsUtility
    {
        public static string Index(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var hb = Html.Builder();
            var formData = DataViewFilters.SessionFormData();
            var exportSettingCollection = ExportSettingCollection(siteSettings, permissionType, formData);
            var dataViewName = DataViewSelectors.Get(siteSettings.SiteId);
            return hb.Template(
                siteId: siteSettings.SiteId,
                modelName: "ExportSetting",
                title: siteSettings.Title + " - " + Displays.List(),
                permissionType: permissionType,
                verType: Versions.VerTypes.Latest,
                backUrl: Navigations.ItemIndex(siteSettings.ParentId),
                methodType: BaseModel.MethodTypes.Index,
                allowAccess: permissionType.CanRead(),
                script: IndexScript(
                    exportSettingCollection: exportSettingCollection,
                    siteSettings: siteSettings,
                    permissionType: permissionType,
                    formData: formData,
                    dataViewName: dataViewName),
                action: () => hb
                    .Form(
                        attributes: Html.Attributes()
                            .Id_Css("ExportSettingsForm", "main-form")
                            .Action(Navigations.ItemAction(siteSettings.SiteId)),
                        action: () => hb
                            .DataViewSelector(
                                referenceType: "ExportSettings",
                                dataViewName: dataViewName)
                            .DataViewFilters(
                                siteSettings: siteSettings,
                                siteId: siteSettings.SiteId)
                            .Aggregations(
                                siteSettings: siteSettings,
                                aggregations: exportSettingCollection.Aggregations)
                            .Div(id: "DataViewContainer", action: () => hb
                                .DataView(
                                    exportSettingCollection: exportSettingCollection,
                                    siteSettings: siteSettings,
                                    permissionType: permissionType,
                                    formData: formData,
                                    dataViewName: dataViewName))
                            .Div(css: "margin-bottom")
                            .Hidden(controlId: "TableName", value: "ExportSettings")
                            .Hidden(controlId: "BaseUrl", value: Navigations.BaseUrl()))
                    .Dialog_Move("items", siteSettings.SiteId, bulk: true)
                    .Div(attributes: Html.Attributes()
                        .Id_Css("Dialog_ExportSettings", "dialog")
                        .Title(Displays.ExportSettings()))).ToString();
        }

        private static ExportSettingCollection ExportSettingCollection(
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData, int offset = 0)
        {
            return new ExportSettingCollection(
                siteSettings: siteSettings,
                permissionType: permissionType,
                column: GridSqlColumnCollection(siteSettings),
                where: DataViewFilters.Get(
                    siteSettings: siteSettings,
                    tableName: "ExportSettings",
                    formData: formData,
                    where: Rds.ExportSettingsWhere()),
                orderBy: GridSorters.Get(
                    formData, Rds.ExportSettingsOrderBy().UpdatedTime(SqlOrderBy.Types.desc)),
                offset: offset,
                pageSize: siteSettings.GridPageSize.ToInt(),
                countRecord: true,
                aggregationCollection: siteSettings.AggregationCollection);
        }

        public static string IndexScript(
            ExportSettingCollection exportSettingCollection,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData,
            string dataViewName)
        {
            return string.Empty;
        }

        public static HtmlBuilder DataView(
            this HtmlBuilder hb,
            ExportSettingCollection exportSettingCollection,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData,
            string dataViewName)
        {
            switch (dataViewName)
            {
                default: return hb.Grid(
                    exportSettingCollection: exportSettingCollection,
                    siteSettings: siteSettings,
                    permissionType: permissionType,
                    formData: formData);
            }
        }

        public static string DataView(
            SiteSettings siteSettings, Permissions.Types permissionType)
        {
            switch (DataViewSelectors.Get(siteSettings.SiteId))
            {
                default: return Grid(siteSettings: siteSettings, permissionType: permissionType);
            }
        }

        private static HtmlBuilder Grid(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            ExportSettingCollection exportSettingCollection,
            FormData formData)
        {
            return hb
                .Table(
                    attributes: Html.Attributes()
                        .Id_Css("Grid", "grid")
                        .DataAction("GridRows")
                        .DataMethod("post"),
                    action: () => hb
                        .GridRows(
                            siteSettings: siteSettings,
                            exportSettingCollection: exportSettingCollection,
                            formData: formData))
                .Hidden(
                    controlId: "GridOffset",
                    value: siteSettings.GridPageSize.ToString())
                .MainCommands(
                    siteId: siteSettings.SiteId,
                    permissionType: permissionType,
                    verType: Versions.VerTypes.Latest,
                    backUrl: Navigations.Index("Admins"),
                    bulkMoveButton: true,
                    bulkDeleteButton: true,
                    importButton: true,
                    exportButton: true);
        }

        private static string Grid(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var formData = DataViewFilters.SessionFormData();
            var exportSettingCollection = ExportSettingCollection(siteSettings, permissionType, formData);
            return new ResponseCollection()
                .Html("#DataViewContainer", exportSettingCollection.Count > 0
                    ? Html.Builder().Grid(
                        siteSettings: siteSettings,
                        exportSettingCollection: exportSettingCollection,
                        permissionType: permissionType,
                        formData: formData)
                    : Html.Builder())
                .Html("#Aggregations", Html.Builder().Aggregations(
                    siteSettings: siteSettings,
                    aggregations: exportSettingCollection.Aggregations,
                    container: false))
                .WindowScrollTop().ToJson();
        }

        public static string GridRows(
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            ResponseCollection responseCollection = null,
            int offset = 0,
            bool clearCheck = false,
            Message message = null)
        {
            var formData = DataViewFilters.SessionFormData();
            var exportSettingCollection = ExportSettingCollection(siteSettings, permissionType, formData, offset);
            return (responseCollection ?? new ResponseCollection())
                .Remove(".grid tr", _using: offset == 0)
                .ClearFormData("GridCheckAll", _using: clearCheck)
                .ClearFormData("GridUnCheckedItems", _using: clearCheck)
                .ClearFormData("GridCheckedItems", _using: clearCheck)
                .Message(message)
                .Append("#Grid", Html.Builder().GridRows(
                    siteSettings: siteSettings,
                    exportSettingCollection: exportSettingCollection,
                    formData: formData,
                    addHeader: offset == 0,
                    clearCheck: clearCheck))
                .Html("#Aggregations", Html.Builder().Aggregations(
                    siteSettings: siteSettings,
                    aggregations: exportSettingCollection.Aggregations,
                    container: false))
                .Val("#GridOffset", siteSettings.NextPageOffset(offset, exportSettingCollection.Count()))
                .Markup()
                .ToJson();
        }

        private static HtmlBuilder GridRows(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            ExportSettingCollection exportSettingCollection,
            FormData formData,
            bool addHeader = true,
            bool clearCheck = false)
        {
            var checkAll = clearCheck ? false : Forms.Bool("GridCheckAll");
            if (addHeader)
            {
                hb.GridHeader(
                    columnCollection: siteSettings.GridColumnCollection(), 
                    formData: formData,
                    checkAll: checkAll);
            }
            exportSettingCollection.ForEach(exportSettingModel => hb
                .Tr(
                    attributes: Html.Attributes()
                        .Class("grid-row")
                        .DataId(exportSettingModel.ExportSettingId.ToString()),
                    action: () =>
                    {
                        hb.Td(action: () => hb
                            .CheckBox(
                                controlCss: "grid-check",
                                _checked: checkAll,
                                dataId: exportSettingModel.ExportSettingId.ToString()));
                        siteSettings.GridColumnCollection()
                            .ForEach(column => hb
                                .TdValue(
                                    column: column,
                                    exportSettingModel: exportSettingModel));
                    }));
            return hb;
        }

        private static SqlColumnCollection GridSqlColumnCollection(SiteSettings siteSettings)
        {
            var select = Rds.ExportSettingsColumn()
                .ExportSettingId()
                .Creator()
                .Updator();
            siteSettings.GridColumnCollection(withTitle: true).ForEach(columnGrid =>
            {
                switch (columnGrid.ColumnName)
                {
                    case "ReferenceType": select.ReferenceType(); break;
                    case "ReferenceId": select.ReferenceId(); break;
                    case "Title": select.Title(); break;
                    case "ExportSettingId": select.ExportSettingId(); break;
                    case "Ver": select.Ver(); break;
                    case "AddHeader": select.AddHeader(); break;
                    case "ExportColumns": select.ExportColumns(); break;
                    case "Comments": select.Comments(); break;
                    case "Creator": select.Creator(); break;
                    case "Updator": select.Updator(); break;
                    case "CreatedTime": select.CreatedTime(); break;
                    case "UpdatedTime": select.UpdatedTime(); break;
                }
            });
            return select;
        }

        public static HtmlBuilder TdValue(
            this HtmlBuilder hb, Column column, ExportSettingModel exportSettingModel)
        {
            switch (column.ColumnName)
            {
                case "Ver": return hb.Td(column: column, value: exportSettingModel.Ver);
                case "Comments": return hb.Td(column: column, value: exportSettingModel.Comments);
                case "Creator": return hb.Td(column: column, value: exportSettingModel.Creator);
                case "Updator": return hb.Td(column: column, value: exportSettingModel.Updator);
                case "CreatedTime": return hb.Td(column: column, value: exportSettingModel.CreatedTime);
                case "UpdatedTime": return hb.Td(column: column, value: exportSettingModel.UpdatedTime);
                default: return hb;
            }
        }

        public static string EditorNew()
        {
            return Editor(new ExportSettingModel(
                    SiteSettingsUtility.ExportSettingsSiteSettings(),
                    Permissions.Admins(),
                methodType: BaseModel.MethodTypes.New));
        }

        public static string Editor(long exportSettingId, bool clearSessions)
        {
            var exportSettingModel = new ExportSettingModel(
                    SiteSettingsUtility.ExportSettingsSiteSettings(),
                    Permissions.Admins(),
                exportSettingId: exportSettingId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            exportSettingModel.SwitchTargets = ExportSettingsUtility.GetSwitchTargets(
                SiteSettingsUtility.ExportSettingsSiteSettings());
            return Editor(exportSettingModel);
        }

        public static string Editor(ExportSettingModel exportSettingModel)
        {
            var hb = Html.Builder();
            var permissionType = Permissions.Admins();
            var siteSettings = SiteSettingsUtility.ExportSettingsSiteSettings();
            return hb.Template(
                siteId: 0,
                modelName: "ExportSetting",
                title: exportSettingModel.MethodType != BaseModel.MethodTypes.New
                    ? exportSettingModel.Title.DisplayValue + " - " + Displays.Edit()
                    : Displays.ExportSettings() + " - " + Displays.New(),
                permissionType: permissionType,
                verType: exportSettingModel.VerType,
                backUrl: Navigations.ItemIndex(0),
                methodType: exportSettingModel.MethodType,
                allowAccess:
                    permissionType.CanEditTenant() &&
                    exportSettingModel.AccessStatus != Databases.AccessStatuses.NotFound,
                action: () =>
                {
                    permissionType = Permissions.Types.Manager;
                    hb
                        .Editor(
                            exportSettingModel: exportSettingModel,
                            permissionType: permissionType,
                            siteSettings: siteSettings)
                        .Hidden(controlId: "TableName", value: "ExportSettings")
                        .Hidden(controlId: "Id", value: exportSettingModel.ExportSettingId.ToString());
                }).ToString();
        }

        private static HtmlBuilder Editor(
            this HtmlBuilder hb,
            ExportSettingModel exportSettingModel,
            Permissions.Types permissionType,
            SiteSettings siteSettings)
        {
            return hb.Div(css: "edit-form", action: () => hb
                .Form(
                    attributes: Html.Attributes()
                        .Id_Css("ExportSettingForm", "main-form")
                        .Action(exportSettingModel.ExportSettingId != 0
                            ? Navigations.Action("ExportSettings", exportSettingModel.ExportSettingId)
                            : Navigations.Action("ExportSettings")),
                    action: () => hb
                        .RecordHeader(
                            id: exportSettingModel.ExportSettingId,
                            baseModel: exportSettingModel,
                            tableName: "ExportSettings",
                            switchTargets: exportSettingModel.SwitchTargets?
                                .Select(o => o.ToLong()).ToList())
                        .Div(css: "edit-form-comments", action: () => hb
                            .Comments(
                                comments: exportSettingModel.Comments,
                                verType: exportSettingModel.VerType))
                        .Div(css: "edit-form-tabs", action: () => hb
                            .FieldTabs(exportSettingModel: exportSettingModel)
                            .Fields(
                                siteSettings: siteSettings,
                                permissionType: permissionType,
                                exportSettingModel: exportSettingModel)
                            .MainCommands(
                                siteId: 0,
                                permissionType: permissionType,
                                verType: exportSettingModel.VerType,
                                backUrl: Navigations.Index("ExportSettings"),
                                referenceType: "ExportSettings",
                                referenceId: exportSettingModel.ExportSettingId,
                                updateButton: true,
                                mailButton: true,
                                deleteButton: true,
                                extensions: () => hb
                                    .MainCommandExtensions(
                                        exportSettingModel: exportSettingModel,
                                        siteSettings: siteSettings)))
                        .Hidden(
                            controlId: "ExportSettings_Timestamp",
                            css: "must-transport",
                            value: exportSettingModel.Timestamp)
                        .Hidden(
                            controlId: "SwitchTargets",
                            css: "must-transport",
                            value: exportSettingModel.SwitchTargets?.Join()))
                .OutgoingMailsForm("ExportSettings", exportSettingModel.ExportSettingId, exportSettingModel.Ver)
                .Dialog_Copy("ExportSettings", exportSettingModel.ExportSettingId)
                .Dialog_Histories(Navigations.Action("ExportSettings", exportSettingModel.ExportSettingId))
                .Dialog_OutgoingMail()
                .EditorExtensions(exportSettingModel: exportSettingModel, siteSettings: siteSettings));
        }

        private static HtmlBuilder FieldTabs(this HtmlBuilder hb, ExportSettingModel exportSettingModel)
        {
            return hb.Ul(css: "field-tab", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral", 
                        text: Displays.Basic())));
        }

        private static HtmlBuilder Fields(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            ExportSettingModel exportSettingModel)
        {
            return hb.FieldSet(id: "FieldSetGeneral", action: () =>
            {
                siteSettings.ColumnCollection
                    .Where(o => o.EditorVisible.ToBool())
                    .OrderBy(o => siteSettings.EditorOrder.IndexOf(o.ColumnName))
                    .ForEach(column =>
                    {
                        switch (column.ColumnName)
                        {
                            case "ReferenceType": hb.Field(siteSettings, column, exportSettingModel.ReferenceType.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "ReferenceId": hb.Field(siteSettings, column, exportSettingModel.ReferenceId.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "Title": hb.Field(siteSettings, column, exportSettingModel.Title.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "ExportSettingId": hb.Field(siteSettings, column, exportSettingModel.ExportSettingId.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "Ver": hb.Field(siteSettings, column, exportSettingModel.Ver.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "AddHeader": hb.Field(siteSettings, column, exportSettingModel.AddHeader.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                        }
                    });
                hb.VerUpCheckBox(exportSettingModel);
            });
        }

        private static HtmlBuilder MainCommandExtensions(
            this HtmlBuilder hb,
            ExportSettingModel exportSettingModel,
            SiteSettings siteSettings)
        {
            return hb;
        }

        private static HtmlBuilder EditorExtensions(
            this HtmlBuilder hb,
            ExportSettingModel exportSettingModel,
            SiteSettings siteSettings)
        {
            return hb;
        }

        public static List<long> GetSwitchTargets(SiteSettings siteSettings)
        {
            var switchTargets = Forms.Data("SwitchTargets").Split(',')
                .Select(o => o.ToLong())
                .Where(o => o != 0)
                .ToList();
            if (switchTargets.Count() == 0)
            {
                var formData = DataViewFilters.SessionFormData();
                switchTargets = Rds.ExecuteTable(
                    transactional: false,
                    statements: Rds.SelectExportSettings(
                        column: Rds.ExportSettingsColumn().ExportSettingId(),
                        where: DataViewFilters.Get(
                            siteSettings: siteSettings,
                            tableName: "ExportSettings",
                            formData: formData,
                            where: Rds.ExportSettingsWhere()),
                        orderBy: GridSorters.Get(
                            formData, Rds.ExportSettingsOrderBy().UpdatedTime(SqlOrderBy.Types.desc))))
                                .AsEnumerable()
                                .Select(o => o["ExportSettingId"].ToLong())
                                .ToList();    
            }
            return switchTargets;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Edit(string referenceType, long referenceId)
        {
            return Edit(new ResponseCollection(), referenceType, referenceId).ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ResponseCollection Edit(
            this ResponseCollection responseCollection, string referenceType, long referenceId)
        {
            var exportSettingModel = ExportSetting(referenceType, referenceId);
            ExportSettingsUtility.SetSessions(exportSettingModel);
            var hb = Html.Builder();
            return responseCollection
                .Html("#Dialog_ExportSettings", hb
                    .Form(
                        attributes: Html.Attributes()
                            .Id("ExportSettingsForm")
                            .Action(Navigations.ItemAction(referenceId, "ExportSettings")),
                        action: () => hb
                            .Columns(
                                exportSettingModel.ExportColumns,
                                exportSettingModel.GetSiteSettings())
                            .Settings(referenceType, referenceId)
                            .P(css: "message-dialog")
                            .Commands(referenceType, referenceId)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static ExportSettingModel ExportSetting(string referenceType, long referenceId)
        {
            var exportSettingCollection = Collection(referenceType, referenceId);
            return exportSettingCollection.Count > 0
                ? exportSettingCollection.FirstOrDefault()
                : new ExportSettingModel(Permissions.Types.NotSet, referenceType, referenceId);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ExportSettingCollection Collection(string referenceType, long referenceId)
        {
            return new ExportSettingCollection(
                SiteSettingsUtility.ExportSettingsSiteSettings(),
                Permissions.Types.NotSet,
                where: Rds.ExportSettingsWhere()
                    .ReferenceType(referenceType)
                    .ReferenceId(referenceId),
                orderBy: Rds.ExportSettingsOrderBy()
                    .Title());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Columns(
            this HtmlBuilder hb, ExportColumns exportSettings, SiteSettings siteSettings)
        {
            return hb.FieldSet(
                css: "fieldset enclosed-auto w500 h400",
                legendText: Displays.SettingColumnList(),
                action: () => hb
                    .FieldSelectable(
                        controlId: "ExportSettings_Columns",
                        fieldCss: "field-vertical w500",
                        controlContainerCss: "container-selectable",
                        controlCss: " h300",
                        listItemCollection: exportSettings.ExportColumnHash(siteSettings),
                        selectedValueCollection: new List<string>(),
                        commandOptionAction: () => hb
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    controlId: "ColumnToUp",
                                    controlCss: "button-up",
                                    text: Displays.Up(),
                                    onClick: Def.JavaScript.Submit,
                                    action: "Set",
                                    method: "post")
                                .Button(
                                    controlId: "ColumnToDown",
                                    controlCss: "button-down",
                                    text: Displays.Down(),
                                    onClick: Def.JavaScript.Submit,
                                    action: "Set",
                                    method: "post")
                                .Button(
                                    controlId: "ColumnToVisible",
                                    controlCss: "button-visible",
                                    text: Displays.Output(),
                                    onClick: Def.JavaScript.Submit,
                                    action: "Set",
                                    method: "put")
                                .Button(
                                    controlId: "ColumnToHide",
                                    controlCss: "button-hide",
                                    text: Displays.NotOutput(),
                                    onClick: Def.JavaScript.Submit,
                                    action: "Set",
                                    method: "put"))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Settings(this HtmlBuilder hb, string referenceType, long referenceId)
        {
            var exportSettingCollection = new ExportSettingCollection(
                SiteSettingsUtility.ExportSettingsSiteSettings(),
                Permissions.Types.NotSet,
                where: Rds.ExportSettingsWhere()
                    .ReferenceType(referenceType)
                    .ReferenceId(referenceId));
            var exportSettingModel = exportSettingCollection.FirstOrDefault();
            return hb.FieldSet(
                css: "fieldset enclosed-auto w400 h400",
                legendText: Displays.ExportSettings(),
                action: () => hb
                    .FieldDropDown(
                        controlId: "ExportSettings_ExportSettingId",
                        controlCss: " auto-postback",
                        labelText: Displays.ExportSettings_ExportSettingId(),
                        optionCollection: exportSettingCollection.ToDictionary(
                            o => o.ExportSettingId.ToString(),
                            o => new ControlData(o.Title.Value)),
                        action: "Change",
                        method: "put")
                    .FieldTextBox(
                        controlId: "ExportSettings_Title",
                        controlCss: " must-transport",
                        labelText: Displays.ExportSettings_Title(),
                        text: exportSettingModel != null
                            ? exportSettingModel.Title.Value
                            : string.Empty)
                    .FieldCheckBox(
                        controlId: "ExportSettings_AddHeader",
                        controlCss: " must-transport",
                        labelText: Displays.ExportSettings_AddHeader(),
                        _checked: exportSettingModel != null
                            ? exportSettingModel.AddHeader
                            : true,
                        labelPositionIsRight: true)
                    .Div(
                        css: "command-field",
                            action: () => hb
                            .Button(
                                controlId: "UpdateExportSettings",
                                controlCss: "button-save",
                                text: Displays.Save(),
                                onClick: Def.JavaScript.Submit,
                                action: "UpdateOrCreate",
                                method: "put")
                            .Button(
                                controlId: "DeleteExportSettings",
                                controlCss: "button-delete",
                                text: Displays.Delete(),
                                onClick: Def.JavaScript.Submit,
                                action: "Delete",
                                method: "delete",
                                confirm: "Displays_ConfirmDelete")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Commands(
            this HtmlBuilder hb, string referenceType, long referenceId)
        {
            return hb.Div(
                css: "command-center",
                action: () => hb
                    .Button(
                        controlId: "Export",
                        controlCss: "button-export",
                        text: Displays.Export(),
                        onClick: Def.JavaScript.Submit,
                        href: Navigations.Export(Url.RouteData("reference"), referenceId),
                        action: "Set",
                        method: "put")
                    .Button(
                        controlId: "CancelExport",
                        controlCss: "button-cancel",
                        text: Displays.Cancel(),
                        onClick: Def.JavaScript.CancelDialog));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Change()
        {
            var exportSettingModel = new ExportSettingModel(
                SiteSettingsUtility.ExportSettingsSiteSettings(),
                Permissions.Types.NotSet).Get(
                    where: Rds.ExportSettingsWhere()
                        .ExportSettingId(Forms.Long("ExportSettings_ExportSettingId")));
            SetSessions(exportSettingModel);
            exportSettingModel.Session_ExportColumns(Jsons.ToJson(exportSettingModel.ExportColumns));
            return Html.Builder()
                .SelectableListItem(listItemCollection: exportSettingModel.ExportColumnHash())
                .Html("#ExportSettings_Columns")
                .Val("#ExportSettings_Title", exportSettingModel.Title.Value)
                .Val("#ExportSettings_AddHeader", exportSettingModel.AddHeader)
                .ClearFormData()
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static void SetSessions(ExportSettingModel exportSettingModel)
        {
            exportSettingModel.Session_Title(exportSettingModel.Title);
            exportSettingModel.Session_AddHeader(exportSettingModel.AddHeader);
            exportSettingModel.Session_ExportColumns(Jsons.ToJson(exportSettingModel.ExportColumns));
        }
    }
}
