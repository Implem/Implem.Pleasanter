using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Analysis;
using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.ServerData;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
namespace Implem.Pleasanter.Models
{
    public class MailAddressModel : BaseModel
    {
        public long OwnerId = 0;
        public string OwnerType = string.Empty;
        public long MailAddressId = 0;
        public string MailAddress = string.Empty;
        public Title Title { get { return new Title(MailAddressId, MailAddress); } }
        public long SavedOwnerId = 0;
        public string SavedOwnerType = string.Empty;
        public long SavedMailAddressId = 0;
        public string SavedMailAddress = string.Empty;
        public bool OwnerId_Updated { get { return OwnerId != SavedOwnerId; } }
        public bool OwnerType_Updated { get { return OwnerType != SavedOwnerType && OwnerType != null; } }
        public bool MailAddressId_Updated { get { return MailAddressId != SavedMailAddressId; } }
        public bool MailAddress_Updated { get { return MailAddress != SavedMailAddress && MailAddress != null; } }
        public List<long> SwitchTargets;

        public MailAddressModel()
        {
        }

        public MailAddressModel(
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

        public MailAddressModel(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            long mailAddressId,
            bool clearSessions = false,
            bool setByForm = false,
            List<long> switchTargets = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            SiteSettings = siteSettings;
            MailAddressId = mailAddressId;
            PermissionType = permissionType;
            Get();
            if (clearSessions) ClearSessions();
            if (setByForm) SetByForm();
            SwitchTargets = switchTargets;
            MethodType = methodType;
            OnConstructed();
        }

        public MailAddressModel(
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

        public MailAddressModel Get(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            Set(Rds.ExecuteTable(statements: Rds.SelectMailAddresses(
                tableType: tableType,
                column: column ?? Rds.MailAddressesColumnDefault(),
                join: join ??  Rds.MailAddressesJoinDefault(),
                where: where ?? Rds.MailAddressesWhereDefault(this),
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
                    Rds.InsertMailAddresses(
                        tableType: tableType,
                        selectIdentity: true,
                        param: param ?? Rds.MailAddressesParamDefault(
                            this, setDefault: true, paramAll: paramAll))
                });
            MailAddressId = newId != 0 ? newId : MailAddressId;
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
            if (!PermissionType.CanCreate())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "MailAddresses_OwnerId": if (!SiteSettings.AllColumn("OwnerId").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_OwnerType": if (!SiteSettings.AllColumn("OwnerType").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_MailAddressId": if (!SiteSettings.AllColumn("MailAddressId").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_Ver": if (!SiteSettings.AllColumn("Ver").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_MailAddress": if (!SiteSettings.AllColumn("MailAddress").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_Title": if (!SiteSettings.AllColumn("Title").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_Comments": if (!SiteSettings.AllColumn("Comments").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_Creator": if (!SiteSettings.AllColumn("Creator").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_Updator": if (!SiteSettings.AllColumn("Updator").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_CreatedTime": if (!SiteSettings.AllColumn("CreatedTime").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_UpdatedTime": if (!SiteSettings.AllColumn("UpdatedTime").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_VerUp": if (!SiteSettings.AllColumn("VerUp").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_Timestamp": if (!SiteSettings.AllColumn("Timestamp").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
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
                    Rds.UpdateMailAddresses(
                        verUp: VerUp,
                        where: Rds.MailAddressesWhereDefault(this)
                            .UpdatedTime(timestamp, _using: timestamp.NotZero()),
                        param: param ?? Rds.MailAddressesParamDefault(this, paramAll: paramAll),
                        countRecord: true)
                });
            if (count == 0) return ResponseConflicts();
            Get();
            var responseCollection = new MailAddressesResponseCollection(this);
            OnUpdated(ref responseCollection);
            return ResponseByUpdate(responseCollection)
                .PrependComment(Comments, VerType)
                .ToJson();
        }

        private void OnUpdating(ref SqlParamCollection param)
        {
        }

        private void OnUpdated(ref MailAddressesResponseCollection responseCollection)
        {
        }

        private string ValidateBeforeUpdate()
        {
            if (!PermissionType.CanUpdate())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "MailAddresses_OwnerId": if (!SiteSettings.AllColumn("OwnerId").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_OwnerType": if (!SiteSettings.AllColumn("OwnerType").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_MailAddressId": if (!SiteSettings.AllColumn("MailAddressId").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_Ver": if (!SiteSettings.AllColumn("Ver").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_MailAddress": if (!SiteSettings.AllColumn("MailAddress").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_Title": if (!SiteSettings.AllColumn("Title").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_Comments": if (!SiteSettings.AllColumn("Comments").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_Creator": if (!SiteSettings.AllColumn("Creator").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_Updator": if (!SiteSettings.AllColumn("Updator").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_CreatedTime": if (!SiteSettings.AllColumn("CreatedTime").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_UpdatedTime": if (!SiteSettings.AllColumn("UpdatedTime").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_VerUp": if (!SiteSettings.AllColumn("VerUp").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "MailAddresses_Timestamp": if (!SiteSettings.AllColumn("Timestamp").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                }
            }
            return null;
        }

        private ResponseCollection ResponseByUpdate(MailAddressesResponseCollection responseCollection)
        {
            return responseCollection
                .Ver()
                .Timestamp()
                .Val("#VerUp", false)
                .Disabled("#VerUp", false)
                .Html("#HeaderTitle", Title.Value + " - " + Displays.Edit())
                .Html("#RecordInfo", new HtmlBuilder().RecordInfo(baseModel: this, tableName: "MailAddresses"))
                .Message(Messages.Updated(Title.ToString()))
                .RemoveComment(DeleteCommentId, _using: DeleteCommentId != 0)
                .ClearFormData();
        }

        public string UpdateOrCreate(
            SqlWhereCollection where = null,
            SqlParamCollection param = null)
        {
            if (!PermissionType.CanUpdate())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            SetBySession();
            OnUpdatingOrCreating(ref where, ref param);
            var newId = Rds.ExecuteScalar_long(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.UpdateOrInsertMailAddresses(
                        selectIdentity: true,
                        where: where ?? Rds.MailAddressesWhereDefault(this),
                        param: param ?? Rds.MailAddressesParamDefault(this, setDefault: true))
                });
            MailAddressId = newId != 0 ? newId : MailAddressId;
            Get();
            var responseCollection = new MailAddressesResponseCollection(this);
            OnUpdatedOrCreated(ref responseCollection);
            return responseCollection.ToJson();
        }

        private void OnUpdatingOrCreating(
            ref SqlWhereCollection where,
            ref SqlParamCollection param)
        {
        }

        private void OnUpdatedOrCreated(ref MailAddressesResponseCollection responseCollection)
        {
        }

        public string Delete(bool redirect = true)
        {
            if (!PermissionType.CanDelete())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            OnDeleting();
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.DeleteMailAddresses(
                        where: Rds.MailAddressesWhere().MailAddressId(MailAddressId))
                });
            Sessions.Set("Message", Messages.Deleted(Title.Value).Html);
            var responseCollection = new MailAddressesResponseCollection(this);
            OnDeleted(ref responseCollection);
            if (redirect)
            {
                responseCollection.Href(Navigations.Index("MailAddresses"));
            }
            return responseCollection.ToJson();
        }

        private void OnDeleting()
        {
        }

        private void OnDeleted(ref MailAddressesResponseCollection responseCollection)
        {
        }

        public string Restore(long mailAddressId)
        {
            if (!Permissions.Admins().CanEditTenant())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            MailAddressId = mailAddressId;
            Rds.ExecuteNonQuery(
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreMailAddresses(
                        where: Rds.MailAddressesWhere().MailAddressId(MailAddressId))
                });
            return new ResponseCollection().ToJson();
        }

        public string PhysicalDelete(Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            if (!PermissionType.CanDelete())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            OnPhysicalDeleting();
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: Rds.PhysicalDeleteMailAddresses(
                    tableType: tableType,
                    param: Rds.MailAddressesParam().MailAddressId(MailAddressId)));
            var responseCollection = new MailAddressesResponseCollection(this);
            OnPhysicalDeleted(ref responseCollection);
            return responseCollection.ToJson();
        }

        private void OnPhysicalDeleting()
        {
        }

        private void OnPhysicalDeleted(ref MailAddressesResponseCollection responseCollection)
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
                    new MailAddressCollection(
                        siteSettings: SiteSettings,
                        permissionType: PermissionType,
                        where: Rds.MailAddressesWhere().MailAddressId(MailAddressId),
                        orderBy: Rds.MailAddressesOrderBy().Ver(SqlOrderBy.Types.desc),
                        tableType: Sqls.TableTypes.NormalAndHistory).ForEach(mailAddressModel => hb
                            .Tr(
                                attributes: new HtmlAttributes()
                                    .Class("grid-row history not-link")
                                    .DataAction("History")
                                    .DataMethod("post")
                                    .Add("data-ver", mailAddressModel.Ver)
                                    .Add("data-latest", 1, _using: mailAddressModel.Ver == Ver),
                                action: () =>
                                    SiteSettings.HistoryColumnCollection().ForEach(column =>
                                        hb.TdValue(column, mailAddressModel))));
                });
            return new MailAddressesResponseCollection(this).Html("#FieldSetHistories", hb).ToJson();
        }

        public string History()
        {
            Get(
                where: Rds.MailAddressesWhere()
                    .MailAddressId(MailAddressId)
                    .Ver(Forms.Int("Ver")),
                tableType: Sqls.TableTypes.NormalAndHistory);
            VerType =  Forms.Bool("Latest")
                ? Versions.VerTypes.Latest
                : Versions.VerTypes.History;
            SwitchTargets = MailAddressesUtility.GetSwitchTargets(SiteSettings);
            return Editor();
        }

        public string Previous()
        {
            var switchTargets = MailAddressesUtility.GetSwitchTargets(SiteSettings);
            var mailAddressModel = new MailAddressModel(
                siteSettings: SiteSettings,
                permissionType: PermissionType,
                mailAddressId: switchTargets.Previous(MailAddressId),
                switchTargets: switchTargets);
            return RecordResponse(mailAddressModel);
        }

        public string Next()
        {
            var switchTargets = MailAddressesUtility.GetSwitchTargets(SiteSettings);
            var mailAddressModel = new MailAddressModel(
                siteSettings: SiteSettings,
                permissionType: PermissionType,
                mailAddressId: switchTargets.Next(MailAddressId),
                switchTargets: switchTargets);
            return RecordResponse(mailAddressModel);
        }

        public string Reload()
        {
            SwitchTargets = MailAddressesUtility.GetSwitchTargets(SiteSettings);
            return RecordResponse(this, pushState: false);
        }

        private string RecordResponse(
            MailAddressModel mailAddressModel, Message message = null, bool pushState = true)
        {
            mailAddressModel.MethodType = BaseModel.MethodTypes.Edit;
            return new MailAddressesResponseCollection(this)
                .Func("clearDialogs")
                .Html(
                    "#MainContainer",
                    mailAddressModel.AccessStatus == Databases.AccessStatuses.Selected
                        ? MailAddressesUtility.Editor(mailAddressModel)
                        : MailAddressesUtility.Editor(this))
                .Message(message)
                .PushState(
                    "Edit",
                    Navigations.Edit("MailAddresses", mailAddressModel.MailAddressId),
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
                    case "MailAddresses_OwnerId": OwnerId = Forms.Data(controlId).ToLong(); break;
                    case "MailAddresses_OwnerType": OwnerType = Forms.Data(controlId).ToString(); break;
                    case "MailAddresses_MailAddress": MailAddress = Forms.Data(controlId).ToString(); break;
                    case "MailAddresses_Timestamp": Timestamp = Forms.Data(controlId).ToString(); break;
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
                    case "OwnerId": if (dataRow[name] != DBNull.Value) { OwnerId = dataRow[name].ToLong(); SavedOwnerId = OwnerId; } break;
                    case "OwnerType": if (dataRow[name] != DBNull.Value) { OwnerType = dataRow[name].ToString(); SavedOwnerType = OwnerType; } break;
                    case "MailAddressId": if (dataRow[name] != DBNull.Value) { MailAddressId = dataRow[name].ToLong(); SavedMailAddressId = MailAddressId; } break;
                    case "Ver": Ver = dataRow[name].ToInt(); SavedVer = Ver; break;
                    case "MailAddress": MailAddress = dataRow[name].ToString(); SavedMailAddress = MailAddress; break;
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
            return new MailAddressesResponseCollection(this)
                .Html("#MainContainer", MailAddressesUtility.Editor(this))
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
        public MailAddressModel(long userId)
        {
            Get(
                where: Rds.MailAddressesWhere()
                    .OwnerId(userId)
                    .OwnerType("Users"),
                top: 1);
        }
    }

    public class MailAddressCollection : List<MailAddressModel>
    {
        public Databases.AccessStatuses AccessStatus = Databases.AccessStatuses.Initialized;
        public Aggregations Aggregations = new Aggregations();

        public MailAddressCollection(
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

        public MailAddressCollection(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            DataTable dataTable)
        {
            Set(siteSettings, permissionType, dataTable);
        }

        private MailAddressCollection Set(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            DataTable dataTable)
        {
            if (dataTable.Rows.Count > 0)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    Add(new MailAddressModel(siteSettings, permissionType, dataRow));
                }
                AccessStatus = Databases.AccessStatuses.Selected;
            }
            else
            {
                AccessStatus = Databases.AccessStatuses.NotFound;
            }
            return this;
        }

        public MailAddressCollection(
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
                Rds.SelectMailAddresses(
                    dataTableName: "Main",
                    column: column ?? Rds.MailAddressesColumnDefault(),
                    join: join ??  Rds.MailAddressesJoinDefault(),
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
                statements.AddRange(Rds.MailAddressesAggregations(aggregationCollection, where));
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
                statements: Rds.MailAddressesStatement(
                    commandText: commandText,
                    param: param ?? null));
        }
    }

    public static class MailAddressesUtility
    {
        public static string Index(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var hb = new HtmlBuilder();
            var formData = DataViewFilters.SessionFormData();
            var mailAddressCollection = MailAddressCollection(siteSettings, permissionType, formData);
            var dataViewName = DataViewSelectors.Get(siteSettings.SiteId);
            return hb.Template(
                siteId: siteSettings.SiteId,
                referenceId: "MailAddresses",
                title: siteSettings.Title + " - " + Displays.List(),
                permissionType: permissionType,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.Index,
                allowAccess: permissionType.CanRead(),
                script: IndexScript(
                    mailAddressCollection: mailAddressCollection,
                    siteSettings: siteSettings,
                    permissionType: permissionType,
                    formData: formData,
                    dataViewName: dataViewName),
                userStyle: siteSettings.GridStyle,
                userScript: siteSettings.GridScript,
                action: () => hb
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id_Css("MailAddressesForm", "main-form")
                            .Action(Navigations.ItemAction(siteSettings.SiteId)),
                        action: () => hb
                            .DataViewSelector(
                                referenceType: "MailAddresses",
                                dataViewName: dataViewName)
                            .DataViewFilters(
                                siteSettings: siteSettings,
                                siteId: siteSettings.SiteId)
                            .Aggregations(
                                siteSettings: siteSettings,
                                aggregations: mailAddressCollection.Aggregations)
                            .Div(id: "DataViewContainer", action: () => hb
                                .DataView(
                                    mailAddressCollection: mailAddressCollection,
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
                            .Hidden(controlId: "TableName", value: "MailAddresses")
                            .Hidden(controlId: "BaseUrl", value: Navigations.BaseUrl()))
                    .Dialog_Move("items", siteSettings.SiteId, bulk: true)
                    .Div(attributes: new HtmlAttributes()
                        .Id_Css("Dialog_ExportSettings", "dialog")
                        .Title(Displays.ExportSettings()))).ToString();
        }

        private static MailAddressCollection MailAddressCollection(
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData, int offset = 0)
        {
            return new MailAddressCollection(
                siteSettings: siteSettings,
                permissionType: permissionType,
                column: GridSqlColumnCollection(siteSettings),
                where: DataViewFilters.Get(
                    siteSettings: siteSettings,
                    tableName: "MailAddresses",
                    formData: formData,
                    where: Rds.MailAddressesWhere()),
                orderBy: GridSorters.Get(
                    formData, Rds.MailAddressesOrderBy().UpdatedTime(SqlOrderBy.Types.desc)),
                offset: offset,
                pageSize: siteSettings.GridPageSize.ToInt(),
                countRecord: true,
                aggregationCollection: siteSettings.AggregationCollection);
        }

        public static string IndexScript(
            MailAddressCollection mailAddressCollection,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData,
            string dataViewName)
        {
            return string.Empty;
        }

        public static HtmlBuilder DataView(
            this HtmlBuilder hb,
            MailAddressCollection mailAddressCollection,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData,
            string dataViewName)
        {
            switch (dataViewName)
            {
                default: return hb.Grid(
                    mailAddressCollection: mailAddressCollection,
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
            MailAddressCollection mailAddressCollection,
            FormData formData)
        {
            return hb
                .Table(
                    attributes: new HtmlAttributes()
                        .Id_Css("Grid", "grid")
                        .DataAction("GridRows")
                        .DataMethod("post"),
                    action: () => hb
                        .GridRows(
                            siteSettings: siteSettings,
                            mailAddressCollection: mailAddressCollection,
                            formData: formData))
                .Hidden(
                    controlId: "GridOffset",
                    value: siteSettings.GridPageSize == mailAddressCollection.Count()
                        ? siteSettings.GridPageSize.ToString()
                        : "-1");
        }

        private static string Grid(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var formData = DataViewFilters.SessionFormData();
            var mailAddressCollection = MailAddressCollection(siteSettings, permissionType, formData);
            return new ResponseCollection()
                .Html("#DataViewContainer", new HtmlBuilder().Grid(
                    siteSettings: siteSettings,
                    mailAddressCollection: mailAddressCollection,
                    permissionType: permissionType,
                    formData: formData))
                .Html("#Aggregations", new HtmlBuilder().Aggregations(
                    siteSettings: siteSettings,
                    aggregations: mailAddressCollection.Aggregations,
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
            var mailAddressCollection = MailAddressCollection(siteSettings, permissionType, formData, offset);
            return (responseCollection ?? new ResponseCollection())
                .Remove(".grid tr", _using: offset == 0)
                .ClearFormData("GridCheckAll", _using: clearCheck)
                .ClearFormData("GridUnCheckedItems", _using: clearCheck)
                .ClearFormData("GridCheckedItems", _using: clearCheck)
                .Message(message)
                .Append("#Grid", new HtmlBuilder().GridRows(
                    siteSettings: siteSettings,
                    mailAddressCollection: mailAddressCollection,
                    formData: formData,
                    addHeader: offset == 0,
                    clearCheck: clearCheck))
                .Html("#Aggregations", new HtmlBuilder().Aggregations(
                    siteSettings: siteSettings,
                    aggregations: mailAddressCollection.Aggregations,
                    container: false))
                .Val("#GridOffset", siteSettings.NextPageOffset(offset, mailAddressCollection.Count()))
                .Markup()
                .ToJson();
        }

        private static HtmlBuilder GridRows(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            MailAddressCollection mailAddressCollection,
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
            mailAddressCollection.ForEach(mailAddressModel => hb
                .Tr(
                    attributes: new HtmlAttributes()
                        .Class("grid-row")
                        .DataId(mailAddressModel.MailAddressId.ToString()),
                    action: () =>
                    {
                        hb.Td(action: () => hb
                            .CheckBox(
                                controlCss: "grid-check",
                                _checked: checkAll,
                                dataId: mailAddressModel.MailAddressId.ToString()));
                        siteSettings.GridColumnCollection()
                            .ForEach(column => hb
                                .TdValue(
                                    column: column,
                                    mailAddressModel: mailAddressModel));
                    }));
            return hb;
        }

        private static SqlColumnCollection GridSqlColumnCollection(SiteSettings siteSettings)
        {
            var select = Rds.MailAddressesColumn()
                .MailAddressId()
                .Creator()
                .Updator();
            siteSettings.GridColumnCollection(withTitle: true).ForEach(columnGrid =>
            {
                switch (columnGrid.ColumnName)
                {
                    case "OwnerId": select.OwnerId(); break;
                    case "OwnerType": select.OwnerType(); break;
                    case "MailAddressId": select.MailAddressId(); break;
                    case "Ver": select.Ver(); break;
                    case "MailAddress": select.MailAddress(); break;
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
            this HtmlBuilder hb, Column column, MailAddressModel mailAddressModel)
        {
            switch (column.ColumnName)
            {
                case "Ver": return hb.Td(column: column, value: mailAddressModel.Ver);
                case "Comments": return hb.Td(column: column, value: mailAddressModel.Comments);
                case "Creator": return hb.Td(column: column, value: mailAddressModel.Creator);
                case "Updator": return hb.Td(column: column, value: mailAddressModel.Updator);
                case "CreatedTime": return hb.Td(column: column, value: mailAddressModel.CreatedTime);
                case "UpdatedTime": return hb.Td(column: column, value: mailAddressModel.UpdatedTime);
                default: return hb;
            }
        }

        public static string EditorNew()
        {
            return Editor(new MailAddressModel(
                SiteSettingsUtility.MailAddressesSiteSettings(),
                Permissions.Admins(),
                methodType: BaseModel.MethodTypes.New));
        }

        public static string Editor(long mailAddressId, bool clearSessions)
        {
            var mailAddressModel = new MailAddressModel(
                SiteSettingsUtility.MailAddressesSiteSettings(),
                Permissions.Admins(),
                mailAddressId: mailAddressId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            mailAddressModel.SwitchTargets = MailAddressesUtility.GetSwitchTargets(
                SiteSettingsUtility.MailAddressesSiteSettings());
            return Editor(mailAddressModel);
        }

        public static string Editor(MailAddressModel mailAddressModel)
        {
            var hb = new HtmlBuilder();
            var permissionType = Permissions.Admins();
            var siteSettings = SiteSettingsUtility.MailAddressesSiteSettings();
            return hb.Template(
                siteId: 0,
                referenceId: "MailAddresses",
                title: mailAddressModel.MethodType == BaseModel.MethodTypes.New
                    ? Displays.MailAddresses() + " - " + Displays.New()
                    : mailAddressModel.Title.Value,
                permissionType: permissionType,
                verType: mailAddressModel.VerType,
                methodType: mailAddressModel.MethodType,
                allowAccess:
                    permissionType.CanEditTenant() &&
                    mailAddressModel.AccessStatus != Databases.AccessStatuses.NotFound,
                action: () =>
                {
                    permissionType = Permissions.Types.Manager;
                    hb
                        .Editor(
                            mailAddressModel: mailAddressModel,
                            permissionType: permissionType,
                            siteSettings: siteSettings)
                        .Hidden(controlId: "TableName", value: "MailAddresses")
                        .Hidden(controlId: "Id", value: mailAddressModel.MailAddressId.ToString());
                }).ToString();
        }

        private static HtmlBuilder Editor(
            this HtmlBuilder hb,
            MailAddressModel mailAddressModel,
            Permissions.Types permissionType,
            SiteSettings siteSettings)
        {
            return hb.Div(css: "edit-form", action: () => hb
                .Form(
                    attributes: new HtmlAttributes()
                        .Id_Css("MailAddressForm", "main-form")
                        .Action(mailAddressModel.MailAddressId != 0
                            ? Navigations.Action("MailAddresses", mailAddressModel.MailAddressId)
                            : Navigations.Action("MailAddresses")),
                    action: () => hb
                        .RecordHeader(
                            id: mailAddressModel.MailAddressId,
                            baseModel: mailAddressModel,
                            tableName: "MailAddresses",
                            switchTargets: mailAddressModel.SwitchTargets?
                                .Select(o => o.ToLong()).ToList())
                        .Div(css: "edit-form-comments", action: () => hb
                            .Comments(
                                comments: mailAddressModel.Comments,
                                verType: mailAddressModel.VerType))
                        .Div(css: "edit-form-tabs", action: () => hb
                            .FieldTabs(mailAddressModel: mailAddressModel)
                            .FieldSetGeneral(
                                siteSettings: siteSettings,
                                permissionType: permissionType,
                                mailAddressModel: mailAddressModel)
                            .FieldSet(
                                attributes: new HtmlAttributes()
                                    .Id("FieldSetHistories")
                                    .DataAction("Histories")
                                    .DataMethod("get"),
                                _using: mailAddressModel.MethodType != BaseModel.MethodTypes.New)
                            .MainCommands(
                                siteId: 0,
                                permissionType: permissionType,
                                verType: mailAddressModel.VerType,
                                backUrl: Navigations.Index("MailAddresses"),
                                referenceType: "MailAddresses",
                                referenceId: mailAddressModel.MailAddressId,
                                updateButton: true,
                                mailButton: true,
                                deleteButton: true,
                                extensions: () => hb
                                    .MainCommandExtensions(
                                        mailAddressModel: mailAddressModel,
                                        siteSettings: siteSettings)))
                        .Hidden(
                            controlId: "MethodType",
                            value: mailAddressModel.MethodType.ToString().ToLower())
                        .Hidden(
                            controlId: "MailAddresses_Timestamp",
                            css: "must-transport",
                            value: mailAddressModel.Timestamp)
                        .Hidden(
                            controlId: "SwitchTargets",
                            css: "must-transport",
                            value: mailAddressModel.SwitchTargets?.Join()))
                .OutgoingMailsForm("MailAddresses", mailAddressModel.MailAddressId, mailAddressModel.Ver)
                .Dialog_Copy("MailAddresses", mailAddressModel.MailAddressId)
                .Dialog_OutgoingMail()
                .EditorExtensions(mailAddressModel: mailAddressModel, siteSettings: siteSettings));
        }

        private static HtmlBuilder FieldTabs(this HtmlBuilder hb, MailAddressModel mailAddressModel)
        {
            return hb.Ul(css: "field-tab", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral", 
                        text: Displays.Basic()))
                .Li(
                    _using: mailAddressModel.MethodType != BaseModel.MethodTypes.New,
                    action: () => hb
                        .A(
                            href: "#FieldSetHistories",
                            text: Displays.Histories())));
        }

        private static HtmlBuilder FieldSetGeneral(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            MailAddressModel mailAddressModel)
        {
            return hb.FieldSet(id: "FieldSetGeneral", action: () =>
            {
                siteSettings.ColumnCollection
                    .Where(o => o.EditorVisible.ToBool())
                    .OrderBy(o => siteSettings.EditorColumnsOrder.IndexOf(o.ColumnName))
                    .ForEach(column =>
                    {
                        switch (column.ColumnName)
                        {
                            case "OwnerId": hb.Field(siteSettings, column, mailAddressModel.MethodType, mailAddressModel.OwnerId.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "OwnerType": hb.Field(siteSettings, column, mailAddressModel.MethodType, mailAddressModel.OwnerType.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "MailAddressId": hb.Field(siteSettings, column, mailAddressModel.MethodType, mailAddressModel.MailAddressId.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "Ver": hb.Field(siteSettings, column, mailAddressModel.MethodType, mailAddressModel.Ver.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "MailAddress": hb.Field(siteSettings, column, mailAddressModel.MethodType, mailAddressModel.MailAddress.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "Title": hb.Field(siteSettings, column, mailAddressModel.MethodType, mailAddressModel.Title.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                        }
                    });
                hb.VerUpCheckBox(mailAddressModel);
            });
        }

        private static HtmlBuilder MainCommandExtensions(
            this HtmlBuilder hb,
            MailAddressModel mailAddressModel,
            SiteSettings siteSettings)
        {
            return hb;
        }

        private static HtmlBuilder EditorExtensions(
            this HtmlBuilder hb,
            MailAddressModel mailAddressModel,
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
                    statements: Rds.SelectMailAddresses(
                        column: Rds.MailAddressesColumn().MailAddressId(),
                        where: DataViewFilters.Get(
                            siteSettings: siteSettings,
                            tableName: "MailAddresses",
                            formData: formData,
                            where: Rds.MailAddressesWhere()),
                        orderBy: GridSorters.Get(
                            formData, Rds.MailAddressesOrderBy().UpdatedTime(SqlOrderBy.Types.desc))))
                                .AsEnumerable()
                                .Select(o => o["MailAddressId"].ToLong())
                                .ToList();    
            }
            return switchTargets;
        }
    }
}
