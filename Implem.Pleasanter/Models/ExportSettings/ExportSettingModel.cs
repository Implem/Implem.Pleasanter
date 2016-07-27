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

        public ExportSettingModel()
        {
        }

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
                column: column ?? Rds.ExportSettingsDefaultColumns(),
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
                .FormResponse(this)
                .Disabled("#VerUp", false)
                .Html("#HeaderTitle", Title.Value)
                .Html("#RecordInfo", new HtmlBuilder().RecordInfo(baseModel: this, tableName: "ExportSettings"))
                .Message(Messages.Updated(Title.ToString()))
                .RemoveComment(DeleteCommentId, _using: DeleteCommentId != 0)
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
            var exportSettingsCollection = ExportSettingUtilities
                .Collection(ReferenceType, ReferenceId);
            responseCollection
                .Html(
                    "#ExportSettings_ExportSettingId",
                    new HtmlBuilder().OptionCollection(
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
            Sessions.Set("Message", Messages.Deleted(Title.Value).Html);
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

        public string Restore(long exportSettingId)
        {
            if (!Permissions.Admins().CanEditTenant())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            ExportSettingId = exportSettingId;
            Rds.ExecuteNonQuery(
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreExportSettings(
                        where: Rds.ExportSettingsWhere().ExportSettingId(ExportSettingId))
                });
            return new ResponseCollection().ToJson();
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
            var hb = new HtmlBuilder();
            hb.Table(
                attributes: new HtmlAttributes().Class("grid"),
                action: () =>
                {
                    hb.GridHeader(
                        columnCollection: SiteSettings.HistoryColumnCollection(),
                        sort: false,
                        checkRow: false);
                    new ExportSettingCollection(
                        siteSettings: SiteSettings,
                        permissionType: PermissionType,
                        where: Rds.ExportSettingsWhere().ExportSettingId(ExportSettingId),
                        orderBy: Rds.ExportSettingsOrderBy().Ver(SqlOrderBy.Types.desc),
                        tableType: Sqls.TableTypes.NormalAndHistory).ForEach(exportSettingModel => hb
                            .Tr(
                                attributes: new HtmlAttributes()
                                    .Class("grid-row history not-link")
                                    .DataAction("History")
                                    .DataMethod("post")
                                    .Add("data-ver", exportSettingModel.Ver)
                                    .Add("data-latest", 1, _using: exportSettingModel.Ver == Ver),
                                action: () =>
                                    SiteSettings.HistoryColumnCollection().ForEach(column =>
                                        hb.TdValue(column, exportSettingModel))));
                });
            return new ExportSettingsResponseCollection(this).Html("#FieldSetHistories", hb).ToJson();
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
                : Versions.VerTypes.History;
            SwitchTargets = ExportSettingUtilities.GetSwitchTargets(SiteSettings);
            return Editor();
        }

        public string Previous()
        {
            var switchTargets = ExportSettingUtilities.GetSwitchTargets(SiteSettings);
            var exportSettingModel = new ExportSettingModel(
                siteSettings: SiteSettings,
                permissionType: PermissionType,
                exportSettingId: switchTargets.Previous(ExportSettingId),
                switchTargets: switchTargets);
            return RecordResponse(exportSettingModel);
        }

        public string Next()
        {
            var switchTargets = ExportSettingUtilities.GetSwitchTargets(SiteSettings);
            var exportSettingModel = new ExportSettingModel(
                siteSettings: SiteSettings,
                permissionType: PermissionType,
                exportSettingId: switchTargets.Next(ExportSettingId),
                switchTargets: switchTargets);
            return RecordResponse(exportSettingModel);
        }

        public string Reload()
        {
            SwitchTargets = ExportSettingUtilities.GetSwitchTargets(SiteSettings);
            return RecordResponse(this, pushState: false);
        }

        private string RecordResponse(
            ExportSettingModel exportSettingModel, Message message = null, bool pushState = true)
        {
            exportSettingModel.MethodType = BaseModel.MethodTypes.Edit;
            return new ExportSettingsResponseCollection(this)
                .Func("clearDialogs")
                .ReplaceAll(
                    "#MainContainer",
                    exportSettingModel.AccessStatus == Databases.AccessStatuses.Selected
                        ? ExportSettingUtilities.Editor(exportSettingModel, byRest: true)
                        : ExportSettingUtilities.Editor(this, byRest: true))
                .Message(message)
                .PushState(
                    "Edit",
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
                    case "IsHistory": VerType = dataRow[name].ToBool() ? Versions.VerTypes.History : Versions.VerTypes.Latest; break;
                }
            }
        }

        private string Editor()
        {
            return new ExportSettingsResponseCollection(this)
                .ReplaceAll(
                    "#MainContainer",
                    ExportSettingUtilities.Editor(this, byRest: true))
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
            ExportSettingUtilities.SetSessions(this);
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
}
