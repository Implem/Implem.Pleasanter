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
                column: column ?? Rds.DemosColumnDefault(),
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
            if (!PermissionType.CanEditSys())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Demos_DemoId": if (!SiteSettings.AllColumn("DemoId").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_Ver": if (!SiteSettings.AllColumn("Ver").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_TenantId": if (!SiteSettings.AllColumn("TenantId").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_Title": if (!SiteSettings.AllColumn("Title").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_Passphrase": if (!SiteSettings.AllColumn("Passphrase").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_MailAddress": if (!SiteSettings.AllColumn("MailAddress").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_Initialized": if (!SiteSettings.AllColumn("Initialized").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_TimeLag": if (!SiteSettings.AllColumn("TimeLag").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_Comments": if (!SiteSettings.AllColumn("Comments").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_Creator": if (!SiteSettings.AllColumn("Creator").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_Updator": if (!SiteSettings.AllColumn("Updator").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_CreatedTime": if (!SiteSettings.AllColumn("CreatedTime").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_UpdatedTime": if (!SiteSettings.AllColumn("UpdatedTime").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_VerUp": if (!SiteSettings.AllColumn("VerUp").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_Timestamp": if (!SiteSettings.AllColumn("Timestamp").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
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
                            .UpdatedTime(timestamp, _using: timestamp.NotZero()),
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
                    case "Demos_DemoId": if (!SiteSettings.AllColumn("DemoId").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_Ver": if (!SiteSettings.AllColumn("Ver").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_TenantId": if (!SiteSettings.AllColumn("TenantId").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_Title": if (!SiteSettings.AllColumn("Title").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_Passphrase": if (!SiteSettings.AllColumn("Passphrase").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_MailAddress": if (!SiteSettings.AllColumn("MailAddress").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_Initialized": if (!SiteSettings.AllColumn("Initialized").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_TimeLag": if (!SiteSettings.AllColumn("TimeLag").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_Comments": if (!SiteSettings.AllColumn("Comments").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_Creator": if (!SiteSettings.AllColumn("Creator").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_Updator": if (!SiteSettings.AllColumn("Updator").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_CreatedTime": if (!SiteSettings.AllColumn("CreatedTime").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_UpdatedTime": if (!SiteSettings.AllColumn("UpdatedTime").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_VerUp": if (!SiteSettings.AllColumn("VerUp").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Demos_Timestamp": if (!SiteSettings.AllColumn("Timestamp").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
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
                .Disabled("#VerUp", false)
                .Html("#HeaderTitle", Title.Value + " - " + Displays.Edit())
                .Html("#RecordInfo", Html.Builder().RecordInfo(baseModel: this, tableName: "Demos"))
                .Html("#RecordHistories", Html.Builder().RecordHistories(ver: Ver, verType: VerType))
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
            var hb = Html.Builder();
            hb.Table(
                attributes: Html.Attributes().Class("grid"),
                action: () =>
                {
                    hb.GridHeader(
                        columnCollection: SiteSettings.HistoryGridColumnCollection(),
                        sort: false,
                        checkRow: false);
                    new DemoCollection(
                        siteSettings: SiteSettings,
                        permissionType: PermissionType,
                        where: Rds.DemosWhere().DemoId(DemoId),
                        orderBy: Rds.DemosOrderBy().UpdatedTime(SqlOrderBy.Types.desc),
                        tableType: Sqls.TableTypes.NormalAndHistory).ForEach(demoModel => hb
                            .Tr(
                                attributes: Html.Attributes()
                                    .Class("grid-row not-link")
                                    .OnClick(Def.JavaScript.HistoryAndCloseDialog
                                        .Params(demoModel.Ver))
                                    .DataAction("History")
                                    .DataMethod("post")
                                    .Add("data-latest", 1, _using: demoModel.Ver == Ver),
                                action: () =>
                                    SiteSettings.HistoryGridColumnCollection().ForEach(column =>
                                        hb.TdValue(column, demoModel))));
                });
            return new DemosResponseCollection(this).Html("#HistoriesForm", hb).ToJson();
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
                : Versions.VerType(DemoId);
            SwitchTargets = DemosUtility.GetSwitchTargets(SiteSettings);
            return Editor();
        }

        public string PreviousHistory()
        {
            Get(
                where: Rds.DemosWhere()
                    .DemoId(DemoId)
                    .Ver(Forms.Int("Ver"), _operator: "<"),
                orderBy: Rds.DemosOrderBy()
                    .Ver(SqlOrderBy.Types.desc),
                tableType: Sqls.TableTypes.History,
                top: 1);
            SwitchTargets = DemosUtility.GetSwitchTargets(SiteSettings);
            switch (AccessStatus)
            {
                case Databases.AccessStatuses.Selected:
                    VerType = Versions.VerType(DemoId, Versions.DirectioTypes.Previous);
                    return Editor();
                default:
                    return new DemosResponseCollection(this).ToJson();
            }
        }

        public string NextHistory()
        {
            Get(
                where: Rds.DemosWhere()
                    .DemoId(DemoId)
                    .Ver(Forms.Int("Ver"), _operator: ">"),
                orderBy: Rds.DemosOrderBy()
                    .Ver(SqlOrderBy.Types.asc),
                tableType: Sqls.TableTypes.History,
                top: 1);
            SwitchTargets = DemosUtility.GetSwitchTargets(SiteSettings);
            switch (AccessStatus)
            {
                case Databases.AccessStatuses.Selected:
                    VerType = Versions.VerType(DemoId, Versions.DirectioTypes.Next);
                    return Editor();
                default:
                    return new DemosResponseCollection(this).ToJson();
            }
        }

        public string Previous()
        {
            var switchTargets = DemosUtility.GetSwitchTargets(SiteSettings);
            var demoModel = new DemoModel(
                siteSettings: SiteSettings,
                permissionType: PermissionType,
                demoId: switchTargets.Previous(DemoId),
                switchTargets: switchTargets);
            return RecordResponse(demoModel);
        }

        public string Next()
        {
            var switchTargets = DemosUtility.GetSwitchTargets(SiteSettings);
            var demoModel = new DemoModel(
                siteSettings: SiteSettings,
                permissionType: PermissionType,
                demoId: switchTargets.Next(DemoId),
                switchTargets: switchTargets);
            return RecordResponse(demoModel);
        }

        public string Reload()
        {
            SwitchTargets = DemosUtility.GetSwitchTargets(SiteSettings);
            return RecordResponse(this, pushState: false);
        }

        private string RecordResponse(
            DemoModel demoModel, Message message = null, bool pushState = true)
        {
            demoModel.MethodType = BaseModel.MethodTypes.Edit;
            return new DemosResponseCollection(this)
                .Func("clearDialogs")
                .Html(
                    "#MainContainer",
                    demoModel.AccessStatus == Databases.AccessStatuses.Selected
                        ? DemosUtility.Editor(demoModel)
                        : DemosUtility.Editor(this))
                .Message(message)
                .PushState(
                    Navigations.Edit("Demos", demoModel.DemoId),
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
                }
            }
        }

        private string Editor()
        {
            return new DemosResponseCollection(this)
                .Html("#MainContainer", DemosUtility.Editor(this))
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
        public void InitializeTimeLag()
        {
            var criteria = DateTime.Today.AddDays(Parameters.Service.DemoInitialDays);
            TimeLag = (
                criteria.AddDays(DayOfWeek.Monday - criteria.DayOfWeek) -
                Def.DemoDefinitionCollection
                    .Where(o => o.CreatedTime >= Parameters.General.MinTime)
                    .Select(o => o.CreatedTime).Min()).Days;
        }
    }

    public class DemoCollection : List<DemoModel>
    {
        public Databases.AccessStatuses AccessStatus = Databases.AccessStatuses.Initialized;
        public Aggregations Aggregations = new Aggregations();

        public DemoCollection(
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
            IEnumerable<Aggregation> aggregationCollection = null,
            bool get = true)
        {
            if (get)
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
        }

        public DemoCollection(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            DataTable dataTable)
        {
            Set(siteSettings, permissionType, dataTable);
        }

        private DemoCollection Set(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            DataTable dataTable)
        {
            if (dataTable.Rows.Count > 0)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    Add(new DemoModel(siteSettings, permissionType, dataRow));
                }
                AccessStatus = Databases.AccessStatuses.Selected;
            }
            else
            {
                AccessStatus = Databases.AccessStatuses.NotFound;
            }
            return this;
        }

        public DemoCollection(
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
                Rds.SelectDemos(
                    dataTableName: "Main",
                    column: column ?? Rds.DemosColumnDefault(),
                    join: join ??  Rds.DemosJoinDefault(),
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
                statements.AddRange(Rds.DemosAggregations(aggregationCollection, where));
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
                statements: Rds.DemosStatement(
                    commandText: commandText,
                    param: param ?? null));
        }
    }

    public static class DemosUtility
    {
        public static string Index(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var hb = Html.Builder();
            var formData = DataViewFilters.SessionFormData();
            var demoCollection = DemoCollection(siteSettings, permissionType, formData);
            var dataViewName = DataViewSelectors.Get(siteSettings.SiteId);
            return hb.Template(
                siteId: siteSettings.SiteId,
                modelName: "Demo",
                title: siteSettings.Title + " - " + Displays.List(),
                permissionType: permissionType,
                verType: Versions.VerTypes.Latest,
                backUrl: Navigations.ItemIndex(siteSettings.ParentId),
                methodType: BaseModel.MethodTypes.Index,
                allowAccess: permissionType.CanRead(),
                script: IndexScript(
                    demoCollection: demoCollection,
                    siteSettings: siteSettings,
                    permissionType: permissionType,
                    formData: formData,
                    dataViewName: dataViewName),
                action: () => hb
                    .Form(
                        attributes: Html.Attributes()
                            .Id_Css("DemosForm", "main-form")
                            .Action(Navigations.ItemAction(siteSettings.SiteId)),
                        action: () => hb
                            .DataViewSelector(
                                referenceType: "Demos",
                                dataViewName: dataViewName)
                            .DataViewFilters(
                                siteSettings: siteSettings,
                                siteId: siteSettings.SiteId)
                            .Aggregations(
                                siteSettings: siteSettings,
                                aggregations: demoCollection.Aggregations)
                            .Div(id: "DataViewContainer", action: () => hb
                                .DataView(
                                    demoCollection: demoCollection,
                                    siteSettings: siteSettings,
                                    permissionType: permissionType,
                                    formData: formData,
                                    dataViewName: dataViewName))
                            .MainCommands(
                                siteId: siteSettings.SiteId,
                                permissionType: permissionType,
                                verType: Versions.VerTypes.Latest,
                                backUrl: Navigations.Index("Admins"),
                                bulkMoveButton: true,
                                bulkDeleteButton: true,
                                importButton: true,
                                exportButton: true)
                            .Div(css: "margin-bottom")
                            .Hidden(controlId: "TableName", value: "Demos")
                            .Hidden(controlId: "BaseUrl", value: Navigations.BaseUrl()))
                    .Dialog_Move("items", siteSettings.SiteId, bulk: true)
                    .Div(attributes: Html.Attributes()
                        .Id_Css("Dialog_ExportSettings", "dialog")
                        .Title(Displays.ExportSettings()))).ToString();
        }

        private static DemoCollection DemoCollection(
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData, int offset = 0)
        {
            return new DemoCollection(
                siteSettings: siteSettings,
                permissionType: permissionType,
                column: GridSqlColumnCollection(siteSettings),
                where: DataViewFilters.Get(
                    siteSettings: siteSettings,
                    tableName: "Demos",
                    formData: formData,
                    where: Rds.DemosWhere().TenantId(Sessions.TenantId())),
                orderBy: GridSorters.Get(
                    formData, Rds.DemosOrderBy().UpdatedTime(SqlOrderBy.Types.desc)),
                offset: offset,
                pageSize: siteSettings.GridPageSize.ToInt(),
                countRecord: true,
                aggregationCollection: siteSettings.AggregationCollection);
        }

        public static string IndexScript(
            DemoCollection demoCollection,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData,
            string dataViewName)
        {
            return string.Empty;
        }

        public static HtmlBuilder DataView(
            this HtmlBuilder hb,
            DemoCollection demoCollection,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData,
            string dataViewName)
        {
            switch (dataViewName)
            {
                default: return hb.Grid(
                    demoCollection: demoCollection,
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
            DemoCollection demoCollection,
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
                            demoCollection: demoCollection,
                            formData: formData))
                .Hidden(
                    controlId: "GridOffset",
                    value: siteSettings.GridPageSize == demoCollection.Count()
                        ? siteSettings.GridPageSize.ToString()
                        : "-1");
        }

        private static string Grid(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var formData = DataViewFilters.SessionFormData();
            var demoCollection = DemoCollection(siteSettings, permissionType, formData);
            return new ResponseCollection()
                .Html("#DataViewContainer", demoCollection.Count > 0
                    ? Html.Builder().Grid(
                        siteSettings: siteSettings,
                        demoCollection: demoCollection,
                        permissionType: permissionType,
                        formData: formData)
                    : Html.Builder())
                .Html("#Aggregations", Html.Builder().Aggregations(
                    siteSettings: siteSettings,
                    aggregations: demoCollection.Aggregations,
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
            var demoCollection = DemoCollection(siteSettings, permissionType, formData, offset);
            return (responseCollection ?? new ResponseCollection())
                .Remove(".grid tr", _using: offset == 0)
                .ClearFormData("GridCheckAll", _using: clearCheck)
                .ClearFormData("GridUnCheckedItems", _using: clearCheck)
                .ClearFormData("GridCheckedItems", _using: clearCheck)
                .Message(message)
                .Append("#Grid", Html.Builder().GridRows(
                    siteSettings: siteSettings,
                    demoCollection: demoCollection,
                    formData: formData,
                    addHeader: offset == 0,
                    clearCheck: clearCheck))
                .Html("#Aggregations", Html.Builder().Aggregations(
                    siteSettings: siteSettings,
                    aggregations: demoCollection.Aggregations,
                    container: false))
                .Val("#GridOffset", siteSettings.NextPageOffset(offset, demoCollection.Count()))
                .Markup()
                .ToJson();
        }

        private static HtmlBuilder GridRows(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            DemoCollection demoCollection,
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
            demoCollection.ForEach(demoModel => hb
                .Tr(
                    attributes: Html.Attributes()
                        .Class("grid-row")
                        .DataId(demoModel.DemoId.ToString()),
                    action: () =>
                    {
                        hb.Td(action: () => hb
                            .CheckBox(
                                controlCss: "grid-check",
                                _checked: checkAll,
                                dataId: demoModel.DemoId.ToString()));
                        siteSettings.GridColumnCollection()
                            .ForEach(column => hb
                                .TdValue(
                                    column: column,
                                    demoModel: demoModel));
                    }));
            return hb;
        }

        private static SqlColumnCollection GridSqlColumnCollection(SiteSettings siteSettings)
        {
            var select = Rds.DemosColumn()
                .DemoId()
                .Creator()
                .Updator();
            siteSettings.GridColumnCollection(withTitle: true).ForEach(columnGrid =>
            {
                switch (columnGrid.ColumnName)
                {
                    case "DemoId": select.DemoId(); break;
                    case "Ver": select.Ver(); break;
                    case "TenantId": select.TenantId(); break;
                    case "Title": select.Title(); break;
                    case "Passphrase": select.Passphrase(); break;
                    case "MailAddress": select.MailAddress(); break;
                    case "Initialized": select.Initialized(); break;
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
            this HtmlBuilder hb, Column column, DemoModel demoModel)
        {
            switch (column.ColumnName)
            {
                case "Ver": return hb.Td(column: column, value: demoModel.Ver);
                case "Comments": return hb.Td(column: column, value: demoModel.Comments);
                case "Creator": return hb.Td(column: column, value: demoModel.Creator);
                case "Updator": return hb.Td(column: column, value: demoModel.Updator);
                case "CreatedTime": return hb.Td(column: column, value: demoModel.CreatedTime);
                case "UpdatedTime": return hb.Td(column: column, value: demoModel.UpdatedTime);
                default: return hb;
            }
        }

        public static string EditorNew()
        {
            return Editor(new DemoModel(
                SiteSettingsUtility.DemosSiteSettings(),
                Permissions.Admins(),
                methodType: BaseModel.MethodTypes.New));
        }

        public static string Editor(int demoId, bool clearSessions)
        {
            var demoModel = new DemoModel(
                SiteSettingsUtility.DemosSiteSettings(),
                Permissions.Admins(),
                demoId: demoId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            demoModel.SwitchTargets = DemosUtility.GetSwitchTargets(
                SiteSettingsUtility.DemosSiteSettings());
            return Editor(demoModel);
        }

        public static string Editor(DemoModel demoModel)
        {
            var hb = Html.Builder();
            var permissionType = Permissions.Admins();
            var siteSettings = SiteSettingsUtility.DemosSiteSettings();
            return hb.Template(
                siteId: 0,
                modelName: "Demo",
                title: demoModel.MethodType != BaseModel.MethodTypes.New
                    ? demoModel.Title.Value + " - " + Displays.Edit()
                    : Displays.Demos() + " - " + Displays.New(),
                permissionType: permissionType,
                verType: demoModel.VerType,
                backUrl: Navigations.ItemIndex(0),
                methodType: demoModel.MethodType,
                allowAccess:
                    permissionType.CanEditTenant() &&
                    demoModel.AccessStatus != Databases.AccessStatuses.NotFound,
                action: () =>
                {
                    permissionType = Permissions.Types.Manager;
                    hb
                        .Editor(
                            demoModel: demoModel,
                            permissionType: permissionType,
                            siteSettings: siteSettings)
                        .Hidden(controlId: "TableName", value: "Demos")
                        .Hidden(controlId: "Id", value: demoModel.DemoId.ToString());
                }).ToString();
        }

        private static HtmlBuilder Editor(
            this HtmlBuilder hb,
            DemoModel demoModel,
            Permissions.Types permissionType,
            SiteSettings siteSettings)
        {
            return hb.Div(css: "edit-form", action: () => hb
                .Form(
                    attributes: Html.Attributes()
                        .Id_Css("DemoForm", "main-form")
                        .Action(demoModel.DemoId != 0
                            ? Navigations.Action("Demos", demoModel.DemoId)
                            : Navigations.Action("Demos")),
                    action: () => hb
                        .RecordHeader(
                            id: demoModel.DemoId,
                            baseModel: demoModel,
                            tableName: "Demos",
                            switchTargets: demoModel.SwitchTargets?
                                .Select(o => o.ToLong()).ToList())
                        .Div(css: "edit-form-comments", action: () => hb
                            .Comments(
                                comments: demoModel.Comments,
                                verType: demoModel.VerType))
                        .Div(css: "edit-form-tabs", action: () => hb
                            .FieldTabs(demoModel: demoModel)
                            .Fields(
                                siteSettings: siteSettings,
                                permissionType: permissionType,
                                demoModel: demoModel)
                            .MainCommands(
                                siteId: 0,
                                permissionType: permissionType,
                                verType: demoModel.VerType,
                                backUrl: Navigations.Index("Demos"),
                                referenceType: "Demos",
                                referenceId: demoModel.DemoId,
                                updateButton: true,
                                mailButton: true,
                                deleteButton: true,
                                extensions: () => hb
                                    .MainCommandExtensions(
                                        demoModel: demoModel,
                                        siteSettings: siteSettings)))
                        .Hidden(
                            controlId: "MethodType",
                            value: demoModel.MethodType.ToString().ToLower())
                        .Hidden(
                            controlId: "Demos_Timestamp",
                            css: "must-transport",
                            value: demoModel.Timestamp)
                        .Hidden(
                            controlId: "SwitchTargets",
                            css: "must-transport",
                            value: demoModel.SwitchTargets?.Join()))
                .OutgoingMailsForm("Demos", demoModel.DemoId, demoModel.Ver)
                .Dialog_Copy("Demos", demoModel.DemoId)
                .Dialog_Histories(Navigations.Action("Demos", demoModel.DemoId))
                .Dialog_OutgoingMail()
                .EditorExtensions(demoModel: demoModel, siteSettings: siteSettings));
        }

        private static HtmlBuilder FieldTabs(this HtmlBuilder hb, DemoModel demoModel)
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
            DemoModel demoModel)
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
                            case "Ver": hb.Field(siteSettings, column, demoModel.Ver.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                        }
                    });
                hb.VerUpCheckBox(demoModel);
            });
        }

        private static HtmlBuilder MainCommandExtensions(
            this HtmlBuilder hb,
            DemoModel demoModel,
            SiteSettings siteSettings)
        {
            return hb;
        }

        private static HtmlBuilder EditorExtensions(
            this HtmlBuilder hb,
            DemoModel demoModel,
            SiteSettings siteSettings)
        {
            return hb;
        }

        public static List<int> GetSwitchTargets(SiteSettings siteSettings)
        {
            var switchTargets = Forms.Data("SwitchTargets").Split(',')
                .Select(o => o.ToInt())
                .Where(o => o != 0)
                .ToList();
            if (switchTargets.Count() == 0)
            {
                var formData = DataViewFilters.SessionFormData();
                switchTargets = Rds.ExecuteTable(
                    transactional: false,
                    statements: Rds.SelectDemos(
                        column: Rds.DemosColumn().DemoId(),
                        where: DataViewFilters.Get(
                            siteSettings: siteSettings,
                            tableName: "Demos",
                            formData: formData,
                            where: Rds.DemosWhere().TenantId(Sessions.TenantId())),
                        orderBy: GridSorters.Get(
                            formData, Rds.DemosOrderBy().UpdatedTime(SqlOrderBy.Types.desc))))
                                .AsEnumerable()
                                .Select(o => o["DemoId"].ToInt())
                                .ToList();    
            }
            return switchTargets;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Register()
        {
            var passphrase = Strings.NewGuid();
            var mailAddress = Forms.Data("Users_DemoMailAddress");
            var tenantModel = new TenantModel()
            {
                SiteSettings = SiteSettingsUtility.TenantsSiteSettings(),
                PermissionType = Permissions.Types.ServiceAdmin,
                TenantName = mailAddress
            };
            tenantModel.Create();
            var demoModel = new DemoModel()
            {
                SiteSettings = SiteSettingsUtility.DemosSiteSettings(),
                PermissionType = Permissions.Types.ServiceAdmin,
                TenantId = tenantModel.TenantId,
                Passphrase = passphrase,
                MailAddress = mailAddress
            };
            demoModel.Create();
            var outgoingMailModel = new OutgoingMailModel()
            {
                SiteSettings = SiteSettingsUtility.OutgoingMailsSiteSettings(),
                PermissionType = Permissions.Types.Manager,
                Title = new Title(Displays.DemoMailTitle()),
                Body = Displays.DemoMailBody(Url.Server(), passphrase),
                From = new System.Net.Mail.MailAddress(Parameters.Mail.SupportFrom),
                To = mailAddress,
                Bcc = Parameters.Mail.SupportFrom
            };
            outgoingMailModel.Send();
            return Messages.ResponseSentAcceptanceMail()
                .Remove("#DemoForm")
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static bool Login()
        {
            var demoModel = new DemoModel().Get(where:
                Rds.DemosWhere().Passphrase(QueryStrings.Data("passphrase")));
            if (demoModel.AccessStatus == Databases.AccessStatuses.Selected)
            {
                Initializ(demoModel);
                return Sessions.LoggedIn();
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void Initializ(DemoModel demoModel)
        {
            var idHash = new Dictionary<string, long>();
            var loginId = LoginId(demoModel, "User1");
            var password = Strings.NewGuid().Sha512Cng();
            if (demoModel.Initialized)
            {
                Rds.ExecuteNonQuery(statements: Rds.UpdateUsers(
                    param: Rds.UsersParam().Password(password),
                    where: Rds.UsersWhere().LoginId(loginId)));
            }
            else
            {
                demoModel.InitializeTimeLag();
                InitializeDepts(demoModel, idHash);
                InitializeUsers(demoModel, idHash, password);
                InitializeSites(demoModel, idHash);
                InitializeIssues(demoModel, idHash);
                InitializeLinks(demoModel, idHash);
                InitializePermissions(idHash);
                Rds.ExecuteNonQuery(statements: Rds.UpdateDemos(
                    param: Rds.DemosParam().Initialized(true),
                    where: Rds.DemosWhere().Passphrase(demoModel.Passphrase)));
            }
            var userModel = new UserModel()
            {
                LoginId = loginId,
                Password = password
            }.Authenticate(string.Empty);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void InitializeDepts(DemoModel demoModel, Dictionary<string, long> idHash)
        {
            Def.DemoDefinitionCollection
                .Where(o => o.Type == "Depts")
                .ForEach(demoDefinition =>
                    idHash.Add(demoDefinition.Id, Rds.ExecuteScalar_long(statements:
                        Rds.InsertDepts(
                            selectIdentity: true,
                            param: Rds.DeptsParam()
                                .TenantId(demoModel.TenantId)
                                .ParentDeptId(idHash.ContainsKey(demoDefinition.ParentId)
                                    ? idHash[demoDefinition.ParentId].ToInt()
                                    : 0)
                                .DeptCode(demoDefinition.ClassA)
                                .DeptName(demoDefinition.Title)
                                .CreatedTime(demoDefinition.CreatedTime.DemoTime(demoModel))
                                .UpdatedTime(demoDefinition.UpdatedTime.DemoTime(demoModel))))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void InitializeUsers(
            DemoModel demoModel, Dictionary<string, long> idHash, string password)
        {
            Def.DemoDefinitionCollection
                .Where(o => o.Type == "Users")
                .ForEach(demoDefinition =>
                    idHash.Add(demoDefinition.Id, Rds.ExecuteScalar_long(statements:
                        Rds.InsertUsers(
                            selectIdentity: true,
                            param: Rds.UsersParam()
                                .TenantId(demoModel.TenantId)
                                .LoginId(LoginId(demoModel, demoDefinition.Id))
                                .Password(password)
                                .LastName(demoDefinition.Title.Split_1st(' '))
                                .FirstName(demoDefinition.Title.Split_2nd(' '))
                                .DeptId(idHash[demoDefinition.ParentId].ToInt())
                                .FirstAndLastNameOrder(demoDefinition.ClassA == "1"
                                    ? Names.FirstAndLastNameOrders.FirstNameIsFirst
                                    : Names.FirstAndLastNameOrders.LastNameIsFirst)
                                .Birthday(demoDefinition.ClassC.ToDateTime())
                                .Sex(demoDefinition.ClassB)
                                .CreatedTime(demoDefinition.CreatedTime.DemoTime(demoModel))
                                .UpdatedTime(demoDefinition.UpdatedTime.DemoTime(demoModel))))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void InitializeSites(DemoModel demoModel, Dictionary<string, long> idHash)
        {
            var topId = Def.DemoDefinitionCollection.First(o =>
                o.Type == "Sites" && o.ParentId == string.Empty).Id;
            Def.DemoDefinitionCollection
                .Where(o => o.Type == "Sites")
                .ForEach(demoDefinition =>
                    idHash.Add(demoDefinition.Id, Rds.ExecuteScalar_long(statements:
                        new SqlStatement[]
                        {
                            Rds.InsertItems(
                                selectIdentity: true,
                                param: Rds.ItemsParam().ReferenceType("Sites")),
                            Rds.InsertSites(
                                selectIdentity: true,
                                addUpdatorParam: false,
                                param: Rds.SitesParam()
                                    .TenantId(demoModel.TenantId)
                                    .SiteId(raw: Def.Sql.Identity)
                                    .Title(demoDefinition.Title)
                                    .ReferenceType(demoDefinition.ClassA)
                                    .ParentId(idHash.ContainsKey(demoDefinition.ParentId)
                                        ? idHash[demoDefinition.ParentId]
                                        : 0)
                                    .InheritPermission(idHash, topId, demoDefinition.ParentId)
                                    .SiteSettings(demoDefinition.Body.Replace(idHash))
                                    .Creator(idHash[demoDefinition.Creator])
                                    .Updator(idHash[demoDefinition.Updator])
                                    .CreatedTime(demoDefinition.CreatedTime.DemoTime(demoModel))
                                    .UpdatedTime(demoDefinition.UpdatedTime.DemoTime(demoModel)))
                        })));
            new SiteCollection(where: Rds.SitesWhere().TenantId(demoModel.TenantId))
                .ForEach(siteModel => Rds.ExecuteNonQuery(statements:
                    Rds.UpdateItems(
                        param: Rds.ItemsParam()
                            .SiteId(siteModel.SiteId)
                            .Title(siteModel.Title.DisplayValue)
                            .Subset(Jsons.ToJson(new SiteSubset(
                                siteModel, siteModel.SiteSettings))),
                        where: Rds.ItemsWhere().ReferenceId(siteModel.SiteId))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Rds.SitesParamCollection InheritPermission(
             this Rds.SitesParamCollection self, 
             Dictionary<string, long> idHash,
             string topId,
             string parentId)
        {
            if (parentId == string.Empty)
            {
                return self.InheritPermission(raw: Def.Sql.Identity);
            }
            else
            {
                return self.InheritPermission(idHash[topId]);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void InitializeIssues(DemoModel demoModel, Dictionary<string, long> idHash)
        {
            Def.DemoDefinitionCollection
                .Where(o => o.Type == "Issues")
                .ForEach(demoDefinition =>
                {
                    var issueId = Rds.ExecuteScalar_long(statements: new SqlStatement[]
                    {
                        Rds.InsertItems(
                            selectIdentity: true,
                            param: Rds.ItemsParam().ReferenceType("Issues")),
                        Rds.InsertIssues(
                            selectIdentity: true,
                            addUpdatorParam: false,
                            param: Rds.IssuesParam()
                                .SiteId(idHash[demoDefinition.ParentId])
                                .IssueId(raw: Def.Sql.Identity)
                                .Title(demoDefinition.Title)
                                .Body(demoDefinition.Body.Replace(idHash))
                                .StartTime(demoDefinition.StartTime.DemoTime(demoModel))
                                .CompletionTime(demoDefinition.CompletionTime
                                    .AddDays(1).DemoTime(demoModel))
                                .WorkValue(demoDefinition.WorkValue)
                                .ProgressRate(demoDefinition.ProgressRate)
                                .Status(demoDefinition.Status)
                                .Manager(idHash[demoDefinition.Manager])
                                .Owner(idHash[demoDefinition.Owner])
                                .ClassA(demoDefinition.ClassA.Replace(idHash))
                                .ClassB(demoDefinition.ClassB.Replace(idHash))
                                .ClassC(demoDefinition.ClassC.Replace(idHash))
                                .Comments(Comments(demoModel, idHash, demoDefinition.Id))
                                .Creator(idHash[demoDefinition.Creator])
                                .Updator(idHash[demoDefinition.Updator])
                                .CreatedTime(demoDefinition.CreatedTime.DemoTime(demoModel))
                                .UpdatedTime(demoDefinition.UpdatedTime.DemoTime(demoModel)))
                    });
                    idHash.Add(demoDefinition.Id, issueId);
                    var siteModel = new SiteModel(idHash[demoDefinition.ParentId]);
                    var issueModel = new IssueModel(
                        siteModel.SiteSettings, Permissions.Types.Manager, issueId);
                    Rds.ExecuteNonQuery(statements:
                        Rds.UpdateItems(
                            param: Rds.ItemsParam()
                                .SiteId(issueModel.SiteId)
                                .Title(issueModel.Title.DisplayValue)
                                .Subset(Jsons.ToJson(new IssueSubset(
                                    issueModel, issueModel.SiteSettings))),
                            where: Rds.ItemsWhere().ReferenceId(issueModel.IssueId)));
                });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void InitializeResults(DemoModel demoModel, Dictionary<string, long> idHash)
        {
            Def.DemoDefinitionCollection
                .Where(o => o.Type == "Results")
                .ForEach(demoDefinition =>
                {
                    var resultId = Rds.ExecuteScalar_long(statements: new SqlStatement[]
                    {
                        Rds.InsertItems(
                            selectIdentity: true,
                            param: Rds.ItemsParam().ReferenceType("Results")),
                        Rds.InsertResults(
                            selectIdentity: true,
                            addUpdatorParam: false,
                            param: Rds.ResultsParam()
                                .SiteId(idHash[demoDefinition.ParentId])
                                .ResultId(raw: Def.Sql.Identity)
                                .Title(demoDefinition.Title)
                                .Body(demoDefinition.Body.Replace(idHash))
                                .Status(demoDefinition.Status)
                                .Manager(idHash[demoDefinition.Manager])
                                .Owner(idHash[demoDefinition.Owner])
                                .ClassA(demoDefinition.ClassA.Replace(idHash))
                                .ClassB(demoDefinition.ClassB.Replace(idHash))
                                .ClassC(demoDefinition.ClassC.Replace(idHash))
                                .Comments(Comments(demoModel, idHash, demoDefinition.Id))
                                .Creator(idHash[demoDefinition.Creator])
                                .Updator(idHash[demoDefinition.Updator])
                                .CreatedTime(demoDefinition.CreatedTime.DemoTime(demoModel))
                                .UpdatedTime(demoDefinition.UpdatedTime.DemoTime(demoModel)))
                    });
                    idHash.Add(demoDefinition.Id, resultId);
                    var siteModel = new SiteModel(idHash[demoDefinition.ParentId]);
                    var resultModel = new ResultModel(
                        siteModel.SiteSettings, Permissions.Types.Manager, resultId);
                    Rds.ExecuteNonQuery(statements:
                        Rds.UpdateItems(
                            param: Rds.ItemsParam()
                                .SiteId(resultModel.SiteId)
                                .Title(resultModel.Title.DisplayValue)
                                .Subset(Jsons.ToJson(new ResultSubset(
                                    resultModel, resultModel.SiteSettings))),
                            where: Rds.ItemsWhere().ReferenceId(resultModel.ResultId)));
                });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void InitializeLinks(DemoModel demoModel, Dictionary<string, long> idHash)
        {
            Def.DemoDefinitionCollection
                .Where(o => o.Type == "Sites")
                .Where(o => o.ClassB.Trim() != string.Empty)
                .ForEach(demoDefinition =>
                    Rds.ExecuteNonQuery(statements:
                        Rds.InsertLinks(param: Rds.LinksParam()
                            .DestinationId(idHash[demoDefinition.ClassB])
                            .SourceId(idHash[demoDefinition.Id]))));
            Def.DemoDefinitionCollection
                .Where(o => o.ClassA.RegexExists("^#[A-Za-z0-9]+?#$"))
                .ForEach(demoDefinition =>
                    Rds.ExecuteNonQuery(statements:
                        Rds.InsertLinks(param: Rds.LinksParam()
                            .DestinationId(idHash[demoDefinition.ClassA
                                .Substring(1, demoDefinition.ClassA.Length - 2)])
                            .SourceId(idHash[demoDefinition.Id]))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void InitializePermissions(Dictionary<string, long> idHash)
        {
            idHash.Where(o => o.Key.StartsWith("Site")).Select(o => o.Value).ForEach(siteId =>
            {
                Rds.ExecuteNonQuery(statements:
                    Rds.InsertPermissions(
                        param: Rds.PermissionsParam()
                            .ReferenceType("Sites")
                            .ReferenceId(siteId)
                            .DeptId(0)
                            .UserId(idHash["User1"])
                            .PermissionType(Permissions.Types.Manager)));
                idHash.Where(o => o.Key.StartsWith("Dept")).Select(o => o.Value).ForEach(deptId =>
                {
                    Rds.ExecuteNonQuery(statements:
                        Rds.InsertPermissions(
                            param: Rds.PermissionsParam()
                                .ReferenceType("Sites")
                                .ReferenceId(siteId)
                                .DeptId(deptId)
                                .UserId(0)
                                .PermissionType(Permissions.Types.ReadWrite)));
                });
            });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string Comments(
            DemoModel demoModel,
            Dictionary<string, long> idHash,
            string parentId)
        {
            var comments = new Comments();
            Def.DemoDefinitionCollection
                .Where(o => o.Type == "Comments")
                .Where(o => o.ParentId == parentId)
                .Select((o, i) => new { DemoDefinition = o, Index = i })
                .ForEach(data =>
                    comments.Add(new Comment
                    {
                        CommentId = data.Index,
                        CreatedTime = data.DemoDefinition.CreatedTime.DemoTime(demoModel),
                        Creator = idHash[data.DemoDefinition.Creator].ToInt(),
                        Body = data.DemoDefinition.Body.Replace(idHash)
                    }));
            return comments.ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string Replace(this string self, Dictionary<string, long> idHash)
        {
            foreach(var id in self.RegexValues("#[A-Za-z0-9]+?#").Distinct())
            {
                self = self.Replace(
                    id, idHash[id.ToString().Substring(1, id.Length - 2)].ToString());
            }
            return self;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string LoginId(DemoModel demoModel, string userId)
        {
            return "Tenant" + demoModel.TenantId + "_" + userId;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static DateTime DemoTime(this DateTime self, DemoModel demoModel)
        {
            return self.AddDays(demoModel.TimeLag).ToUniversal();
        }
    }
}
