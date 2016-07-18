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
    public class OutgoingMailModel : BaseModel
    {
        public string ReferenceType = string.Empty;
        public long ReferenceId = 0;
        public int ReferenceVer = 0;
        public long OutgoingMailId = 0;
        public string Host = string.Empty;
        public int Port = 0;
        public System.Net.Mail.MailAddress From = null;
        public string To = string.Empty;
        public string Cc = string.Empty;
        public string Bcc = string.Empty;
        public Title Title = new Title();
        public string Body = string.Empty;
        public Time SentTime = new Time();
        public string DestinationSearchRange = string.Empty;
        public string DestinationSearchText = string.Empty;
        public string SavedReferenceType = string.Empty;
        public long SavedReferenceId = 0;
        public int SavedReferenceVer = 0;
        public long SavedOutgoingMailId = 0;
        public string SavedHost = string.Empty;
        public int SavedPort = 0;
        public string SavedFrom = "null";
        public string SavedTo = string.Empty;
        public string SavedCc = string.Empty;
        public string SavedBcc = string.Empty;
        public string SavedTitle = string.Empty;
        public string SavedBody = string.Empty;
        public DateTime SavedSentTime = 0.ToDateTime();
        public string SavedDestinationSearchRange = string.Empty;
        public string SavedDestinationSearchText = string.Empty;
        public bool ReferenceType_Updated { get { return ReferenceType != SavedReferenceType && ReferenceType != null; } }
        public bool ReferenceId_Updated { get { return ReferenceId != SavedReferenceId; } }
        public bool ReferenceVer_Updated { get { return ReferenceVer != SavedReferenceVer; } }
        public bool OutgoingMailId_Updated { get { return OutgoingMailId != SavedOutgoingMailId; } }
        public bool Host_Updated { get { return Host != SavedHost && Host != null; } }
        public bool Port_Updated { get { return Port != SavedPort; } }
        public bool From_Updated { get { return From.ToString() != SavedFrom && From.ToString() != null; } }
        public bool To_Updated { get { return To != SavedTo && To != null; } }
        public bool Cc_Updated { get { return Cc != SavedCc && Cc != null; } }
        public bool Bcc_Updated { get { return Bcc != SavedBcc && Bcc != null; } }
        public bool Title_Updated { get { return Title.Value != SavedTitle && Title.Value != null; } }
        public bool Body_Updated { get { return Body != SavedBody && Body != null; } }
        public bool SentTime_Updated { get { return SentTime.Value != SavedSentTime && SentTime.Value != null; } }
        public List<long> SwitchTargets;

        public OutgoingMailModel()
        {
        }

        public OutgoingMailModel(
            bool setByForm = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            if (setByForm) SetByForm();
            MethodType = methodType;
            OnConstructed();
        }

        public OutgoingMailModel(
            long outgoingMailId,
            bool clearSessions = false,
            bool setByForm = false,
            List<long> switchTargets = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            OutgoingMailId = outgoingMailId;
            Get();
            if (clearSessions) ClearSessions();
            if (setByForm) SetByForm();
            SwitchTargets = switchTargets;
            MethodType = methodType;
            OnConstructed();
        }

        public OutgoingMailModel(
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

        public OutgoingMailModel Get(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            Set(Rds.ExecuteTable(statements: Rds.SelectOutgoingMails(
                tableType: tableType,
                column: column ?? Rds.EditorOutgoingMailsColumn(SiteSettings),
                join: join ??  Rds.OutgoingMailsJoinDefault(),
                where: where ?? Rds.OutgoingMailsWhereDefault(this),
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
                    Rds.InsertOutgoingMails(
                        tableType: tableType,
                        selectIdentity: true,
                        param: param ?? Rds.OutgoingMailsParamDefault(
                            this, setDefault: true, paramAll: paramAll))
                });
            OutgoingMailId = newId != 0 ? newId : OutgoingMailId;
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
                    case "OutgoingMails_ReferenceType": if (!SiteSettings.AllColumn("ReferenceType").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "OutgoingMails_ReferenceId": if (!SiteSettings.AllColumn("ReferenceId").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "OutgoingMails_ReferenceVer": if (!SiteSettings.AllColumn("ReferenceVer").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "OutgoingMails_OutgoingMailId": if (!SiteSettings.AllColumn("OutgoingMailId").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "OutgoingMails_Ver": if (!SiteSettings.AllColumn("Ver").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "OutgoingMails_Host": if (!SiteSettings.AllColumn("Host").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "OutgoingMails_Port": if (!SiteSettings.AllColumn("Port").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "OutgoingMails_From": if (!SiteSettings.AllColumn("From").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "OutgoingMails_To": if (!SiteSettings.AllColumn("To").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "OutgoingMails_Cc": if (!SiteSettings.AllColumn("Cc").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "OutgoingMails_Bcc": if (!SiteSettings.AllColumn("Bcc").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "OutgoingMails_Title": if (!SiteSettings.AllColumn("Title").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "OutgoingMails_Body": if (!SiteSettings.AllColumn("Body").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "OutgoingMails_SentTime": if (!SiteSettings.AllColumn("SentTime").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "OutgoingMails_DestinationSearchRange": if (!SiteSettings.AllColumn("DestinationSearchRange").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "OutgoingMails_DestinationSearchText": if (!SiteSettings.AllColumn("DestinationSearchText").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "OutgoingMails_Comments": if (!SiteSettings.AllColumn("Comments").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "OutgoingMails_Creator": if (!SiteSettings.AllColumn("Creator").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "OutgoingMails_Updator": if (!SiteSettings.AllColumn("Updator").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "OutgoingMails_CreatedTime": if (!SiteSettings.AllColumn("CreatedTime").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "OutgoingMails_UpdatedTime": if (!SiteSettings.AllColumn("UpdatedTime").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "OutgoingMails_VerUp": if (!SiteSettings.AllColumn("VerUp").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "OutgoingMails_Timestamp": if (!SiteSettings.AllColumn("Timestamp").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
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
                    Rds.UpdateOutgoingMails(
                        verUp: VerUp,
                        where: Rds.OutgoingMailsWhereDefault(this)
                            .UpdatedTime(timestamp, _using: timestamp.NotZero()),
                        param: param ?? Rds.OutgoingMailsParamDefault(this, paramAll: paramAll),
                        countRecord: true),
                    Rds.If("@@rowcount = 1"),
                    Rds.UpdateItems(
                        where: Rds.ItemsWhere()
                            .ReferenceType(ReferenceType)
                            .ReferenceId(ReferenceId),
                        param: Rds.ItemsParam().MaintenanceTarget(true)),
                    Rds.End()
                });
            if (count == 0) return ResponseConflicts();
            Get();
            var responseCollection = new OutgoingMailsResponseCollection(this);
            OnUpdated(ref responseCollection);
            return ResponseByUpdate(responseCollection)
                .PrependComment(Comments, VerType)
                .ToJson();
        }

        private void OnUpdating(ref SqlParamCollection param)
        {
        }

        private void OnUpdated(ref OutgoingMailsResponseCollection responseCollection)
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
                    case "OutgoingMails_ReferenceType": if (!SiteSettings.AllColumn("ReferenceType").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "OutgoingMails_ReferenceId": if (!SiteSettings.AllColumn("ReferenceId").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "OutgoingMails_ReferenceVer": if (!SiteSettings.AllColumn("ReferenceVer").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "OutgoingMails_OutgoingMailId": if (!SiteSettings.AllColumn("OutgoingMailId").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "OutgoingMails_Ver": if (!SiteSettings.AllColumn("Ver").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "OutgoingMails_Host": if (!SiteSettings.AllColumn("Host").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "OutgoingMails_Port": if (!SiteSettings.AllColumn("Port").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "OutgoingMails_From": if (!SiteSettings.AllColumn("From").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "OutgoingMails_To": if (!SiteSettings.AllColumn("To").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "OutgoingMails_Cc": if (!SiteSettings.AllColumn("Cc").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "OutgoingMails_Bcc": if (!SiteSettings.AllColumn("Bcc").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "OutgoingMails_Title": if (!SiteSettings.AllColumn("Title").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "OutgoingMails_Body": if (!SiteSettings.AllColumn("Body").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "OutgoingMails_SentTime": if (!SiteSettings.AllColumn("SentTime").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "OutgoingMails_DestinationSearchRange": if (!SiteSettings.AllColumn("DestinationSearchRange").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "OutgoingMails_DestinationSearchText": if (!SiteSettings.AllColumn("DestinationSearchText").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "OutgoingMails_Comments": if (!SiteSettings.AllColumn("Comments").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "OutgoingMails_Creator": if (!SiteSettings.AllColumn("Creator").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "OutgoingMails_Updator": if (!SiteSettings.AllColumn("Updator").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "OutgoingMails_CreatedTime": if (!SiteSettings.AllColumn("CreatedTime").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "OutgoingMails_UpdatedTime": if (!SiteSettings.AllColumn("UpdatedTime").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "OutgoingMails_VerUp": if (!SiteSettings.AllColumn("VerUp").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "OutgoingMails_Timestamp": if (!SiteSettings.AllColumn("Timestamp").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                }
            }
            return null;
        }

        private ResponseCollection ResponseByUpdate(OutgoingMailsResponseCollection responseCollection)
        {
            return responseCollection
                .Ver()
                .Timestamp()
                .Val("#VerUp", false)
                .FormResponse(this)
                .Disabled("#VerUp", false)
                .Html("#HeaderTitle", Title.Value)
                .Html("#RecordInfo", new HtmlBuilder().RecordInfo(baseModel: this, tableName: "OutgoingMails"))
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
                    Rds.UpdateOrInsertOutgoingMails(
                        selectIdentity: true,
                        where: where ?? Rds.OutgoingMailsWhereDefault(this),
                        param: param ?? Rds.OutgoingMailsParamDefault(this, setDefault: true))
                });
            OutgoingMailId = newId != 0 ? newId : OutgoingMailId;
            Get();
            var responseCollection = new OutgoingMailsResponseCollection(this);
            OnUpdatedOrCreated(ref responseCollection);
            return responseCollection.ToJson();
        }

        private void OnUpdatingOrCreating(
            ref SqlWhereCollection where,
            ref SqlParamCollection param)
        {
        }

        private void OnUpdatedOrCreated(ref OutgoingMailsResponseCollection responseCollection)
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
                    Rds.DeleteOutgoingMails(
                        where: Rds.OutgoingMailsWhere().OutgoingMailId(OutgoingMailId))
                });
            Sessions.Set("Message", Messages.Deleted(Title.Value).Html);
            var responseCollection = new OutgoingMailsResponseCollection(this);
            OnDeleted(ref responseCollection);
            if (redirect)
            {
                responseCollection.Href(Navigations.Index("OutgoingMails"));
            }
            return responseCollection.ToJson();
        }

        private void OnDeleting()
        {
        }

        private void OnDeleted(ref OutgoingMailsResponseCollection responseCollection)
        {
        }

        public string Restore(long outgoingMailId)
        {
            if (!Permissions.Admins().CanEditTenant())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            OutgoingMailId = outgoingMailId;
            Rds.ExecuteNonQuery(
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreOutgoingMails(
                        where: Rds.OutgoingMailsWhere().OutgoingMailId(OutgoingMailId))
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
                statements: Rds.PhysicalDeleteOutgoingMails(
                    tableType: tableType,
                    param: Rds.OutgoingMailsParam().OutgoingMailId(OutgoingMailId)));
            var responseCollection = new OutgoingMailsResponseCollection(this);
            OnPhysicalDeleted(ref responseCollection);
            return responseCollection.ToJson();
        }

        private void OnPhysicalDeleting()
        {
        }

        private void OnPhysicalDeleted(ref OutgoingMailsResponseCollection responseCollection)
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
                    new OutgoingMailCollection(
                        where: Rds.OutgoingMailsWhere().OutgoingMailId(OutgoingMailId),
                        orderBy: Rds.OutgoingMailsOrderBy().Ver(SqlOrderBy.Types.desc),
                        tableType: Sqls.TableTypes.NormalAndHistory).ForEach(outgoingMailModel => hb
                            .Tr(
                                attributes: new HtmlAttributes()
                                    .Class("grid-row history not-link")
                                    .DataAction("History")
                                    .DataMethod("post")
                                    .Add("data-ver", outgoingMailModel.Ver)
                                    .Add("data-latest", 1, _using: outgoingMailModel.Ver == Ver),
                                action: () =>
                                    SiteSettings.HistoryColumnCollection().ForEach(column =>
                                        hb.TdValue(column, outgoingMailModel))));
                });
            return new OutgoingMailsResponseCollection(this).Html("#FieldSetHistories", hb).ToJson();
        }

        public string History()
        {
            Get(
                where: Rds.OutgoingMailsWhere()
                    .OutgoingMailId(OutgoingMailId)
                    .Ver(Forms.Int("Ver")),
                tableType: Sqls.TableTypes.NormalAndHistory);
            VerType =  Forms.Bool("Latest")
                ? Versions.VerTypes.Latest
                : Versions.VerTypes.History;
            SwitchTargets = OutgoingMailUtilities.GetSwitchTargets(SiteSettings);
            return Editor();
        }

        public string Previous()
        {
            var switchTargets = OutgoingMailUtilities.GetSwitchTargets(SiteSettings);
            var outgoingMailModel = new OutgoingMailModel(
                outgoingMailId: switchTargets.Previous(OutgoingMailId),
                switchTargets: switchTargets);
            return RecordResponse(outgoingMailModel);
        }

        public string Next()
        {
            var switchTargets = OutgoingMailUtilities.GetSwitchTargets(SiteSettings);
            var outgoingMailModel = new OutgoingMailModel(
                outgoingMailId: switchTargets.Next(OutgoingMailId),
                switchTargets: switchTargets);
            return RecordResponse(outgoingMailModel);
        }

        public string Reload()
        {
            SwitchTargets = OutgoingMailUtilities.GetSwitchTargets(SiteSettings);
            return RecordResponse(this, pushState: false);
        }

        private string RecordResponse(
            OutgoingMailModel outgoingMailModel, Message message = null, bool pushState = true)
        {
            outgoingMailModel.MethodType = BaseModel.MethodTypes.Edit;
            return new OutgoingMailsResponseCollection(this)
                .Func("clearDialogs")
                .Html(
                    "#MainContainer",
                    outgoingMailModel.AccessStatus == Databases.AccessStatuses.Selected
                        ? OutgoingMailUtilities.Editor(outgoingMailModel)
                        : OutgoingMailUtilities.Editor(this))
                .Message(message)
                .PushState(
                    "Edit",
                    Navigations.Edit("OutgoingMails", outgoingMailModel.OutgoingMailId),
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
                    case "OutgoingMails_To": To = Forms.Data(controlId).ToString(); break;
                    case "OutgoingMails_Cc": Cc = Forms.Data(controlId).ToString(); break;
                    case "OutgoingMails_Bcc": Bcc = Forms.Data(controlId).ToString(); break;
                    case "OutgoingMails_Title": Title = new Title(OutgoingMailId, Forms.Data(controlId)); break;
                    case "OutgoingMails_Body": Body = Forms.Data(controlId).ToString(); break;
                    case "OutgoingMails_SentTime": SentTime = new Time(Forms.Data(controlId).ToDateTime(), byForm: true); break;
                    case "OutgoingMails_DestinationSearchRange": DestinationSearchRange = Forms.Data(controlId).ToString(); break;
                    case "OutgoingMails_DestinationSearchText": DestinationSearchText = Forms.Data(controlId).ToString(); break;
                    case "OutgoingMails_Timestamp": Timestamp = Forms.Data(controlId).ToString(); break;
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
                    case "ReferenceType": if (dataRow[name] != DBNull.Value) { ReferenceType = dataRow[name].ToString(); SavedReferenceType = ReferenceType; } break;
                    case "ReferenceId": if (dataRow[name] != DBNull.Value) { ReferenceId = dataRow[name].ToLong(); SavedReferenceId = ReferenceId; } break;
                    case "ReferenceVer": if (dataRow[name] != DBNull.Value) { ReferenceVer = dataRow[name].ToInt(); SavedReferenceVer = ReferenceVer; } break;
                    case "OutgoingMailId": if (dataRow[name] != DBNull.Value) { OutgoingMailId = dataRow[name].ToLong(); SavedOutgoingMailId = OutgoingMailId; } break;
                    case "Ver": Ver = dataRow[name].ToInt(); SavedVer = Ver; break;
                    case "Host": Host = dataRow[name].ToString(); SavedHost = Host; break;
                    case "Port": Port = dataRow[name].ToInt(); SavedPort = Port; break;
                    case "From": From = new System.Net.Mail.MailAddress(dataRow.String("From")); SavedFrom = From.ToString(); break;
                    case "To": To = dataRow[name].ToString(); SavedTo = To; break;
                    case "Cc": Cc = dataRow[name].ToString(); SavedCc = Cc; break;
                    case "Bcc": Bcc = dataRow[name].ToString(); SavedBcc = Bcc; break;
                    case "Title": Title = new Title(dataRow, "OutgoingMailId"); SavedTitle = Title.Value; break;
                    case "Body": Body = dataRow[name].ToString(); SavedBody = Body; break;
                    case "SentTime": SentTime = new Time(dataRow, "SentTime"); SavedSentTime = SentTime.Value; break;
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
            return new OutgoingMailsResponseCollection(this)
                .Html("#MainContainer", OutgoingMailUtilities.Editor(this))
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
        public OutgoingMailModel(string reference, long referenceId)
        {
            SiteSettings = SiteSettingsUtility.OutgoingMailsSiteSettings();
            if (reference.ToLower() == "items")
            {
                var itemModel = new ItemModel(referenceId);
                PermissionType = itemModel.GetSite().PermissionType;
                ReferenceType = itemModel.ReferenceType;
            }
            else
            {
                PermissionType = Permissions.Admins().CanEditTenant()
                    ? Permissions.Types.Create | Permissions.Types.Update
                    : Permissions.Types.NotSet;
                ReferenceType = reference.ToLower();
            }
            ReferenceId = referenceId;
            ReferenceVer = Forms.Int("Ver");
            From = new System.Net.Mail.MailAddress("\"{0}\" <{1}>".Params(
                Sessions.User().FullName,
                OutgoingMailUtilities.FromMailAddress()));
            SetByForm();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string GetDestinations()
        {
            var siteModel = new ItemModel(ReferenceId).GetSite();
            var siteSettings = siteModel.SitesSiteSettings();
            return new OutgoingMailsResponseCollection(this)
                .Html("#OutgoingMails_MailAddresses",
                    new HtmlBuilder().SelectableItems(
                        listItemCollection: OutgoingMailUtilities.Destinations(
                            referenceId: siteModel.InheritPermission,
                            addressBook: OutgoingMailUtilities.AddressBook(siteSettings),
                            searchRange: DestinationSearchRange,
                            searchText: DestinationSearchText),
                        selectedValueTextCollection: new List<string>())).ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string Send()
        {
            var error = VerifyBeforeSending();
            if (error != null) return error;
            var permissionType = new ItemModel(ReferenceId).GetSite().PermissionType;
            Create();
            switch (Parameters.Mail.SmtpProvider)
            {
                case "SendGrid": SendBySendGrid(); break;
                default: SendBySmtp(); break;
            }
            SentTime = new Time(DateTime.Now);
            Update();
            return Response();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private string VerifyBeforeSending()
        {
            if ((To + Cc + Bcc).Trim() == string.Empty)
            {
                return Messages.ResponseRequireMailAddresses().ToJson();
            }
            var badTo = VerifyBadMailAddress(To); if (badTo != null) return badTo;
            var badCc = VerifyBadMailAddress(Cc); if (badCc != null) return badCc;
            var badBcc = VerifyBadMailAddress(Bcc); if (badBcc != null) return badBcc;
            var externalTo = VerifyExternalMailAddress(To); if (externalTo != null) return externalTo;
            var externalCc = VerifyExternalMailAddress(Cc); if (externalCc != null) return externalCc;
            var externalBcc = VerifyExternalMailAddress(Bcc); if (externalBcc != null) return externalBcc;
            return null;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private string VerifyBadMailAddress(string mailAddresses)
        {
            var badMailAddress = Libraries.Mails.Addresses.BadAddress(mailAddresses);
            if (badMailAddress != string.Empty)
            {
                return Messages.ResponseBadMailAddress(badMailAddress).ToJson();
            }
            return null;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private string VerifyExternalMailAddress(string mailAddresses)
        {
            var externalMailAddress = ExternalMailAddress(mailAddresses);
            if (externalMailAddress != string.Empty)
            {
                return Messages.ResponseExternalMailAddress(externalMailAddress).ToJson();
            }
            return null;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SendBySmtp()
        {
            Host = Parameters.Mail.SmtpHost;
            Port = Parameters.Mail.SmtpPort;
            var mailMessage = new System.Net.Mail.MailMessage();
            mailMessage.From = From;
            Libraries.Mails.Addresses.GetEnumerable(To).ForEach(to => mailMessage.To.Add(to));
            Libraries.Mails.Addresses.GetEnumerable(Cc).ForEach(cc => mailMessage.CC.Add(cc));
            Libraries.Mails.Addresses.GetEnumerable(Bcc).ForEach(bcc => mailMessage.Bcc.Add(bcc));
            mailMessage.Subject = Title.Value;
            mailMessage.Body = Body;
            var smtpClient = new System.Net.Mail.SmtpClient();
            smtpClient.Host = Host;
            smtpClient.Port = Port;
            smtpClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            smtpClient.Send(mailMessage);
            smtpClient.Dispose();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SendBySendGrid()
        {
            Host = "smtp.sendgrid.net";
            var sendGridMessage = new SendGrid.SendGridMessage();
            sendGridMessage.From = From;
            Libraries.Mails.Addresses.GetEnumerable(To).ForEach(to => sendGridMessage.AddTo(to));
            Libraries.Mails.Addresses.GetEnumerable(Cc).ForEach(cc => sendGridMessage.AddCc(cc));
            Libraries.Mails.Addresses.GetEnumerable(Bcc).ForEach(bcc => sendGridMessage.AddBcc(bcc));
            sendGridMessage.Subject = Title.Value;
            sendGridMessage.Text = Body;
            new SendGrid.Web(new System.Net.NetworkCredential(
                Parameters.Mail.SendGridSmtpUser,
                Parameters.Mail.SendGridSmtpPassword)).DeliverAsync(sendGridMessage);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private string ExternalMailAddress(string mailAddresses)
        {
            var domains = Parameters.Mail.InternalDomains
                .Split(',')
                .Select(o => o.Trim())
                .Where(o => o != string.Empty);
            if (domains.Count() == 0) return string.Empty;
            foreach (var mailAddress in Libraries.Mails.Addresses.GetEnumerable(mailAddresses))
            {
                if (!domains.Any(o => Libraries.Mails.Addresses.Get(mailAddress).EndsWith(o)))
                {
                    return mailAddress;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private string Response()
        {
            return new OutgoingMailsResponseCollection(this)
                .CloseDialog()
                .ClearFormData()
                .Html("#Dialog_OutgoingMail", string.Empty)
                .Val("#OutgoingMails_Title", string.Empty)
                .Val("#OutgoingMails_Body", string.Empty)
                .Prepend("#OutgoingMailsForm", new HtmlBuilder().OutgoingMailListItem(
                    this, selector: "#ImmediatelyAfterSending" + OutgoingMailId))
                .Markup()
                .Message(Messages.MailTransmissionCompletion())
                .ToJson();
        }
    }
}
