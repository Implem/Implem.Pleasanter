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
using Implem.Pleasanter.Libraries.ViewParts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
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
                column: column ?? Rds.OutgoingMailsColumnDefault(),
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
                        param: Rds.ItemsParam().UpdateTarget(1)),
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
                .Disabled("#VerUp", false)
                .Html("#HeaderTitle", Title.Value + " - " + Displays.Edit())
                .Html("#RecordInfo", Html.Builder().RecordInfo(baseModel: this, tableName: "OutgoingMails"))
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
            var hb = Html.Builder();
            hb.Table(
                attributes: Html.Attributes().Class("grid"),
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
                                attributes: Html.Attributes()
                                    .Class("grid-row not-link")
                                    .OnClick(Def.JavaScript.HistoryAndCloseDialog
                                        .Params(outgoingMailModel.Ver))
                                    .DataAction("History")
                                    .DataMethod("post")
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
                : Versions.VerType(OutgoingMailId);
            SwitchTargets = OutgoingMailsUtility.GetSwitchTargets(SiteSettings);
            return Editor();
        }

        public string Previous()
        {
            var switchTargets = OutgoingMailsUtility.GetSwitchTargets(SiteSettings);
            var outgoingMailModel = new OutgoingMailModel(
                outgoingMailId: switchTargets.Previous(OutgoingMailId),
                switchTargets: switchTargets);
            return RecordResponse(outgoingMailModel);
        }

        public string Next()
        {
            var switchTargets = OutgoingMailsUtility.GetSwitchTargets(SiteSettings);
            var outgoingMailModel = new OutgoingMailModel(
                outgoingMailId: switchTargets.Next(OutgoingMailId),
                switchTargets: switchTargets);
            return RecordResponse(outgoingMailModel);
        }

        public string Reload()
        {
            SwitchTargets = OutgoingMailsUtility.GetSwitchTargets(SiteSettings);
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
                        ? OutgoingMailsUtility.Editor(outgoingMailModel)
                        : OutgoingMailsUtility.Editor(this))
                .Message(message)
                .PushState(
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
                }
            }
        }

        private string Editor()
        {
            return new OutgoingMailsResponseCollection(this)
                .Html("#MainContainer", OutgoingMailsUtility.Editor(this))
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
                new MailAddressModel(Sessions.UserId()).MailAddress));
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
                    Html.Builder().SelectableItems(
                        listItemCollection: OutgoingMailsUtility.Destinations(
                            referenceId: siteModel.InheritPermission,
                            addressBook: OutgoingMailsUtility.AddressBook(siteSettings),
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
            var badMailAddress = BadMailAddress(mailAddresses);
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
            MailAddresses(To).ForEach(to => mailMessage.To.Add(to));
            MailAddresses(Cc).ForEach(cc => mailMessage.CC.Add(cc));
            MailAddresses(Bcc).ForEach(bcc => mailMessage.Bcc.Add(bcc));
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
            MailAddresses(To).ForEach(to => sendGridMessage.AddTo(to));
            MailAddresses(Cc).ForEach(cc => sendGridMessage.AddCc(cc));
            MailAddresses(Bcc).ForEach(bcc => sendGridMessage.AddBcc(bcc));
            sendGridMessage.Subject = Title.Value;
            sendGridMessage.Text = Body;
            new SendGrid.Web(new System.Net.NetworkCredential(
                Parameters.Mail.SendGridSmtpUser,
                Parameters.Mail.SendGridSmtpPassword)).DeliverAsync(sendGridMessage);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private IEnumerable<string> MailAddresses(string mailAddresses)
        {
            return mailAddresses.Split(';')
                .Select(o => o.Trim())
                .Where(o => o != string.Empty);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private string BadMailAddress(string mailAddresses)
        {
            foreach (var mailAddress in MailAddresses(mailAddresses))
            {
                if (OutgoingMailsUtility.MailAddress(mailAddress) == string.Empty)
                {
                    return mailAddress;
                }
            }
            return string.Empty;
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
            foreach (var mailAddress in MailAddresses(mailAddresses))
            {
                if (!domains.Any(o => OutgoingMailsUtility.MailAddress(mailAddress).EndsWith(o)))
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
                .Prepend("#OutgoingMailsForm", Html.Builder().OutgoingMailListItem(
                    this, selector: "#ImmediatelyAfterSending" + OutgoingMailId))
                .Markup()
                .Message(Messages.MailTransmissionCompletion())
                .ToJson();
        }
    }

    public class OutgoingMailCollection : List<OutgoingMailModel>
    {
        public Databases.AccessStatuses AccessStatus = Databases.AccessStatuses.Initialized;
        public Aggregations Aggregations = new Aggregations();

        public OutgoingMailCollection(
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

        public OutgoingMailCollection(
            DataTable dataTable)
        {
            Set(dataTable);
        }

        private OutgoingMailCollection Set(
            DataTable dataTable)
        {
            if (dataTable.Rows.Count > 0)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    Add(new OutgoingMailModel(dataRow));
                }
                AccessStatus = Databases.AccessStatuses.Selected;
            }
            else
            {
                AccessStatus = Databases.AccessStatuses.NotFound;
            }
            return this;
        }

        public OutgoingMailCollection(
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
                Rds.SelectOutgoingMails(
                    dataTableName: "Main",
                    column: column ?? Rds.OutgoingMailsColumnDefault(),
                    join: join ??  Rds.OutgoingMailsJoinDefault(),
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
                statements.AddRange(Rds.OutgoingMailsAggregations(aggregationCollection, where));
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
                statements: Rds.OutgoingMailsStatement(
                    commandText: commandText,
                    param: param ?? null));
        }
    }

    public static class OutgoingMailsUtility
    {
        public static string Index(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var hb = Html.Builder();
            var formData = DataViewFilters.SessionFormData();
            var outgoingMailCollection = OutgoingMailCollection(siteSettings, permissionType, formData);
            var dataViewName = DataViewSelectors.Get(siteSettings.SiteId);
            return hb.Template(
                siteId: siteSettings.SiteId,
                modelName: "OutgoingMail",
                title: siteSettings.Title + " - " + Displays.List(),
                permissionType: permissionType,
                verType: Versions.VerTypes.Latest,
                backUrl: Navigations.ItemIndex(siteSettings.ParentId),
                methodType: BaseModel.MethodTypes.Index,
                allowAccess: permissionType.CanRead(),
                script: IndexScript(
                    outgoingMailCollection: outgoingMailCollection,
                    siteSettings: siteSettings,
                    permissionType: permissionType,
                    formData: formData,
                    dataViewName: dataViewName),
                action: () => hb
                    .Form(
                        attributes: Html.Attributes()
                            .Id_Css("OutgoingMailsForm", "main-form")
                            .Action(Navigations.ItemAction(siteSettings.SiteId)),
                        action: () => hb
                            .DataViewSelector(
                                referenceType: "OutgoingMails",
                                dataViewName: dataViewName)
                            .DataViewFilters(
                                siteSettings: siteSettings,
                                siteId: siteSettings.SiteId)
                            .Aggregations(
                                siteSettings: siteSettings,
                                aggregations: outgoingMailCollection.Aggregations)
                            .Div(id: "DataViewContainer", action: () => hb
                                .DataView(
                                    outgoingMailCollection: outgoingMailCollection,
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
                            .Hidden(controlId: "TableName", value: "OutgoingMails")
                            .Hidden(controlId: "BaseUrl", value: Navigations.BaseUrl()))
                    .Dialog_Move("items", siteSettings.SiteId, bulk: true)
                    .Div(attributes: Html.Attributes()
                        .Id_Css("Dialog_ExportSettings", "dialog")
                        .Title(Displays.ExportSettings()))).ToString();
        }

        private static OutgoingMailCollection OutgoingMailCollection(
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData, int offset = 0)
        {
            return new OutgoingMailCollection(
                column: GridSqlColumnCollection(siteSettings),
                where: DataViewFilters.Get(
                    siteSettings: siteSettings,
                    tableName: "OutgoingMails",
                    formData: formData,
                    where: Rds.OutgoingMailsWhere()),
                orderBy: GridSorters.Get(
                    formData, Rds.OutgoingMailsOrderBy().UpdatedTime(SqlOrderBy.Types.desc)),
                offset: offset,
                pageSize: siteSettings.GridPageSize.ToInt(),
                countRecord: true,
                aggregationCollection: siteSettings.AggregationCollection);
        }

        public static string IndexScript(
            OutgoingMailCollection outgoingMailCollection,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData,
            string dataViewName)
        {
            return string.Empty;
        }

        public static HtmlBuilder DataView(
            this HtmlBuilder hb,
            OutgoingMailCollection outgoingMailCollection,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData,
            string dataViewName)
        {
            switch (dataViewName)
            {
                default: return hb.Grid(
                    outgoingMailCollection: outgoingMailCollection,
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
            OutgoingMailCollection outgoingMailCollection,
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
                            outgoingMailCollection: outgoingMailCollection,
                            formData: formData))
                .Hidden(
                    controlId: "GridOffset",
                    value: siteSettings.GridPageSize == outgoingMailCollection.Count()
                        ? siteSettings.GridPageSize.ToString()
                        : "-1");
        }

        private static string Grid(SiteSettings siteSettings, Permissions.Types permissionType)
        {
            var formData = DataViewFilters.SessionFormData();
            var outgoingMailCollection = OutgoingMailCollection(siteSettings, permissionType, formData);
            return new ResponseCollection()
                .Html("#DataViewContainer", Html.Builder().Grid(
                    siteSettings: siteSettings,
                    outgoingMailCollection: outgoingMailCollection,
                    permissionType: permissionType,
                    formData: formData))
                .Html("#Aggregations", Html.Builder().Aggregations(
                    siteSettings: siteSettings,
                    aggregations: outgoingMailCollection.Aggregations,
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
            var outgoingMailCollection = OutgoingMailCollection(siteSettings, permissionType, formData, offset);
            return (responseCollection ?? new ResponseCollection())
                .Remove(".grid tr", _using: offset == 0)
                .ClearFormData("GridCheckAll", _using: clearCheck)
                .ClearFormData("GridUnCheckedItems", _using: clearCheck)
                .ClearFormData("GridCheckedItems", _using: clearCheck)
                .Message(message)
                .Append("#Grid", Html.Builder().GridRows(
                    siteSettings: siteSettings,
                    outgoingMailCollection: outgoingMailCollection,
                    formData: formData,
                    addHeader: offset == 0,
                    clearCheck: clearCheck))
                .Html("#Aggregations", Html.Builder().Aggregations(
                    siteSettings: siteSettings,
                    aggregations: outgoingMailCollection.Aggregations,
                    container: false))
                .Val("#GridOffset", siteSettings.NextPageOffset(offset, outgoingMailCollection.Count()))
                .Markup()
                .ToJson();
        }

        private static HtmlBuilder GridRows(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            OutgoingMailCollection outgoingMailCollection,
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
            outgoingMailCollection.ForEach(outgoingMailModel => hb
                .Tr(
                    attributes: Html.Attributes()
                        .Class("grid-row")
                        .DataId(outgoingMailModel.OutgoingMailId.ToString()),
                    action: () =>
                    {
                        hb.Td(action: () => hb
                            .CheckBox(
                                controlCss: "grid-check",
                                _checked: checkAll,
                                dataId: outgoingMailModel.OutgoingMailId.ToString()));
                        siteSettings.GridColumnCollection()
                            .ForEach(column => hb
                                .TdValue(
                                    column: column,
                                    outgoingMailModel: outgoingMailModel));
                    }));
            return hb;
        }

        private static SqlColumnCollection GridSqlColumnCollection(SiteSettings siteSettings)
        {
            var select = Rds.OutgoingMailsColumn()
                .OutgoingMailId()
                .Creator()
                .Updator();
            siteSettings.GridColumnCollection(withTitle: true).ForEach(columnGrid =>
            {
                switch (columnGrid.ColumnName)
                {
                    case "ReferenceType": select.ReferenceType(); break;
                    case "ReferenceId": select.ReferenceId(); break;
                    case "ReferenceVer": select.ReferenceVer(); break;
                    case "OutgoingMailId": select.OutgoingMailId(); break;
                    case "Ver": select.Ver(); break;
                    case "Host": select.Host(); break;
                    case "Port": select.Port(); break;
                    case "From": select.From(); break;
                    case "To": select.To(); break;
                    case "Cc": select.Cc(); break;
                    case "Bcc": select.Bcc(); break;
                    case "Title": select.Title(); break;
                    case "Body": select.Body(); break;
                    case "SentTime": select.SentTime(); break;
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
            this HtmlBuilder hb, Column column, OutgoingMailModel outgoingMailModel)
        {
            switch (column.ColumnName)
            {
                case "Ver": return hb.Td(column: column, value: outgoingMailModel.Ver);
                case "Comments": return hb.Td(column: column, value: outgoingMailModel.Comments);
                case "Creator": return hb.Td(column: column, value: outgoingMailModel.Creator);
                case "Updator": return hb.Td(column: column, value: outgoingMailModel.Updator);
                case "CreatedTime": return hb.Td(column: column, value: outgoingMailModel.CreatedTime);
                case "UpdatedTime": return hb.Td(column: column, value: outgoingMailModel.UpdatedTime);
                default: return hb;
            }
        }

        public static string EditorNew()
        {
            return Editor(new OutgoingMailModel(
                methodType: BaseModel.MethodTypes.New));
        }

        public static string Editor(long outgoingMailId, bool clearSessions)
        {
            var outgoingMailModel = new OutgoingMailModel(
                outgoingMailId: outgoingMailId,
                clearSessions: clearSessions,
                methodType: BaseModel.MethodTypes.Edit);
            outgoingMailModel.SwitchTargets = OutgoingMailsUtility.GetSwitchTargets(
                SiteSettingsUtility.OutgoingMailsSiteSettings());
            return Editor(outgoingMailModel);
        }

        public static string Editor(OutgoingMailModel outgoingMailModel)
        {
            var hb = Html.Builder();
            var permissionType = Permissions.Admins();
            var siteSettings = SiteSettingsUtility.OutgoingMailsSiteSettings();
            return hb.Template(
                siteId: 0,
                modelName: "OutgoingMail",
                title: outgoingMailModel.MethodType != BaseModel.MethodTypes.New
                    ? outgoingMailModel.Title.Value + " - " + Displays.Edit()
                    : Displays.OutgoingMails() + " - " + Displays.New(),
                permissionType: permissionType,
                verType: outgoingMailModel.VerType,
                backUrl: Navigations.ItemIndex(0),
                methodType: outgoingMailModel.MethodType,
                allowAccess:
                    permissionType.CanEditTenant() &&
                    outgoingMailModel.AccessStatus != Databases.AccessStatuses.NotFound,
                action: () =>
                {
                    permissionType = Permissions.Types.Manager;
                    hb
                        .Editor(
                            outgoingMailModel: outgoingMailModel,
                            permissionType: permissionType,
                            siteSettings: siteSettings)
                        .Hidden(controlId: "TableName", value: "OutgoingMails")
                        .Hidden(controlId: "Id", value: outgoingMailModel.OutgoingMailId.ToString());
                }).ToString();
        }

        private static HtmlBuilder Editor(
            this HtmlBuilder hb,
            OutgoingMailModel outgoingMailModel,
            Permissions.Types permissionType,
            SiteSettings siteSettings)
        {
            return hb.Div(css: "edit-form", action: () => hb
                .Form(
                    attributes: Html.Attributes()
                        .Id_Css("OutgoingMailForm", "main-form")
                        .Action(outgoingMailModel.OutgoingMailId != 0
                            ? Navigations.Action("OutgoingMails", outgoingMailModel.OutgoingMailId)
                            : Navigations.Action("OutgoingMails")),
                    action: () => hb
                        .RecordHeader(
                            id: outgoingMailModel.OutgoingMailId,
                            baseModel: outgoingMailModel,
                            tableName: "OutgoingMails",
                            switchTargets: outgoingMailModel.SwitchTargets?
                                .Select(o => o.ToLong()).ToList())
                        .Div(css: "edit-form-comments", action: () => hb
                            .Comments(
                                comments: outgoingMailModel.Comments,
                                verType: outgoingMailModel.VerType))
                        .Div(css: "edit-form-tabs", action: () => hb
                            .FieldTabs(outgoingMailModel: outgoingMailModel)
                            .FieldSetGeneral(
                                siteSettings: siteSettings,
                                permissionType: permissionType,
                                outgoingMailModel: outgoingMailModel)
                            .FieldSet(attributes: Html.Attributes()
                                .Id("FieldSetHistories")
                                .DataAction("Histories")
                                .DataMethod("get"))
                            .MainCommands(
                                siteId: 0,
                                permissionType: permissionType,
                                verType: outgoingMailModel.VerType,
                                backUrl: Navigations.Index("OutgoingMails"),
                                referenceType: "OutgoingMails",
                                referenceId: outgoingMailModel.OutgoingMailId,
                                updateButton: true,
                                mailButton: true,
                                deleteButton: true,
                                extensions: () => hb
                                    .MainCommandExtensions(
                                        outgoingMailModel: outgoingMailModel,
                                        siteSettings: siteSettings)))
                        .Hidden(
                            controlId: "MethodType",
                            value: outgoingMailModel.MethodType.ToString().ToLower())
                        .Hidden(
                            controlId: "OutgoingMails_Timestamp",
                            css: "must-transport",
                            value: outgoingMailModel.Timestamp)
                        .Hidden(
                            controlId: "SwitchTargets",
                            css: "must-transport",
                            value: outgoingMailModel.SwitchTargets?.Join()))
                .OutgoingMailsForm("OutgoingMails", outgoingMailModel.OutgoingMailId, outgoingMailModel.Ver)
                .Dialog_Copy("OutgoingMails", outgoingMailModel.OutgoingMailId)
                .Dialog_OutgoingMail()
                .EditorExtensions(outgoingMailModel: outgoingMailModel, siteSettings: siteSettings));
        }

        private static HtmlBuilder FieldTabs(this HtmlBuilder hb, OutgoingMailModel outgoingMailModel)
        {
            return hb.Ul(css: "field-tab", action: () => hb
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetGeneral", 
                        text: Displays.Basic()))
                .Li(action: () => hb
                    .A(
                        href: "#FieldSetHistories",
                        text: Displays.Histories())));
        }

        private static HtmlBuilder FieldSetGeneral(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            OutgoingMailModel outgoingMailModel)
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
                            case "ReferenceType": hb.Field(siteSettings, column, outgoingMailModel.MethodType, outgoingMailModel.ReferenceType.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "ReferenceId": hb.Field(siteSettings, column, outgoingMailModel.MethodType, outgoingMailModel.ReferenceId.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "OutgoingMailId": hb.Field(siteSettings, column, outgoingMailModel.MethodType, outgoingMailModel.OutgoingMailId.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "Ver": hb.Field(siteSettings, column, outgoingMailModel.MethodType, outgoingMailModel.Ver.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "To": hb.Field(siteSettings, column, outgoingMailModel.MethodType, outgoingMailModel.To.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "Cc": hb.Field(siteSettings, column, outgoingMailModel.MethodType, outgoingMailModel.Cc.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "Bcc": hb.Field(siteSettings, column, outgoingMailModel.MethodType, outgoingMailModel.Bcc.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "Title": hb.Field(siteSettings, column, outgoingMailModel.MethodType, outgoingMailModel.Title.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "Body": hb.Field(siteSettings, column, outgoingMailModel.MethodType, outgoingMailModel.Body.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "SentTime": hb.Field(siteSettings, column, outgoingMailModel.MethodType, outgoingMailModel.SentTime.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "DestinationSearchRange": hb.Field(siteSettings, column, outgoingMailModel.MethodType, outgoingMailModel.DestinationSearchRange.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                            case "DestinationSearchText": hb.Field(siteSettings, column, outgoingMailModel.MethodType, outgoingMailModel.DestinationSearchText.ToControl(column), column.ColumnPermissionType(permissionType)); break;
                        }
                    });
                hb.VerUpCheckBox(outgoingMailModel);
            });
        }

        private static HtmlBuilder MainCommandExtensions(
            this HtmlBuilder hb,
            OutgoingMailModel outgoingMailModel,
            SiteSettings siteSettings)
        {
            return hb;
        }

        private static HtmlBuilder EditorExtensions(
            this HtmlBuilder hb,
            OutgoingMailModel outgoingMailModel,
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
                    statements: Rds.SelectOutgoingMails(
                        column: Rds.OutgoingMailsColumn().OutgoingMailId(),
                        where: DataViewFilters.Get(
                            siteSettings: siteSettings,
                            tableName: "OutgoingMails",
                            formData: formData,
                            where: Rds.OutgoingMailsWhere()),
                        orderBy: GridSorters.Get(
                            formData, Rds.OutgoingMailsOrderBy().UpdatedTime(SqlOrderBy.Types.desc))))
                                .AsEnumerable()
                                .Select(o => o["OutgoingMailId"].ToLong())
                                .ToList();    
            }
            return switchTargets;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder OutgoingMailsForm(
            this HtmlBuilder hb, string referenceType, long referenceId, int referenceVer)
        {
            return hb.Form(
                attributes: Html.Attributes()
                    .Id_Css("OutgoingMailsForm", "edit-form-mail-list")
                    .Action(Navigations.ItemAction(referenceId, "OutgoingMails")),
                action: () =>
                    new OutgoingMailCollection(
                        where: Rds.OutgoingMailsWhere()
                            .ReferenceType(referenceType)
                            .ReferenceId(referenceId)
                            .ReferenceVer(referenceVer, _operator: "<="),
                        orderBy: Rds.OutgoingMailsOrderBy()
                            .OutgoingMailId(SqlOrderBy.Types.desc))
                                .ForEach(outgoingMailModel => hb
                                    .OutgoingMailListItem(outgoingMailModel)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder OutgoingMailListItem(
            this HtmlBuilder hb, OutgoingMailModel outgoingMailModel, string selector = "")
        {
            return hb.Div(
                id: selector,
                css: "mail-list",
                action: () => hb
                    .H(number: 3, css: "title-header", action: () => hb
                        .Displays_SentMail())
                    .Div(css: "mail-content", action: () => hb
                        .FieldText(
                            controlId: string.Empty,
                            labelText: Displays.OutgoingMails_SentTime(),
                            text: outgoingMailModel.SentTime.ToString(),
                            fieldCss: "field-auto")
                        .FieldText(
                            controlId: string.Empty,
                            labelText: Displays.OutgoingMails_From(),
                            text: outgoingMailModel.From.ToString(),
                            fieldCss: "field-auto-thin")
                        .OutgoingMailListItemDestination(
                            outgoingMailModel.To, Displays.OutgoingMails_To())
                        .OutgoingMailListItemDestination(
                            outgoingMailModel.Cc, Displays.OutgoingMails_Cc())
                        .OutgoingMailListItemDestination(
                            outgoingMailModel.Bcc, Displays.OutgoingMails_Bcc())
                        .FieldText(
                            controlId: string.Empty,
                            labelText: Displays.OutgoingMails_Title(),
                            text: outgoingMailModel.Title.Value,
                            fieldCss: "field-wide")
                        .FieldMarkUp(
                            controlId: string.Empty,
                            labelText: Displays.OutgoingMails_Body(),
                            text: outgoingMailModel.Body,
                            fieldCss: "field-wide")
                        .Div(css: "command-right", action: () => hb
                            .Button(
                                text: Displays.Reply(),
                                controlCss: "button-send-mail",
                                onClick: Def.JavaScript.Reply,
                                dataId: outgoingMailModel.OutgoingMailId.ToString(),
                                action: "Reply",
                                method: "put"))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder OutgoingMailListItemDestination(
            this HtmlBuilder hb, string distinations, string labelText)
        {
            if (distinations != string.Empty)
            {
                return hb.FieldText(
                    controlId: string.Empty,
                    labelText: labelText,
                    text: distinations,
                    fieldCss: "field-wide");
            }
            else
            {
                return hb;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder Dialog_OutgoingMail(this HtmlBuilder hb)
        {
            return hb.Div(attributes: Html.Attributes()
                .Id_Css("Dialog_OutgoingMail", "dialog"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Editor(string referenceType, long referenceId)
        {
            var siteModel = new ItemModel(referenceId).GetSite();
            var siteSettings = siteModel.SitesSiteSettings();
            var outgoingMailModel = new OutgoingMailModel().Get(
                where: Rds.OutgoingMailsWhere().OutgoingMailId(
                    Forms.Long("OutgoingMails_OutgoingMailId")));
            var hb = Html.Builder();
            return new ResponseCollection()
                .Html("#Dialog_OutgoingMail", hb
                    .Div(css: "edit-form-tabs-max no-border", action: () => hb
                        .Ul(css: "field-tab", action: () => hb
                            .Li(action: () => hb
                                .A(
                                    href: "#FieldSetMailEditor",
                                    text: Displays.Mail()))
                            .Li(action: () => hb
                                .A(
                                    href: "#FieldSetAddressBook",
                                    text: Displays.AddressBook())))
                        .FieldSet(id: "FieldSetMailEditor", action: () => hb
                            .Form(
                                attributes: Html.Attributes()
                                    .Id("OutgoingMailForm")
                                    .Action(Navigations.Action(
                                        referenceType, referenceId, "OutgoingMails")),
                                action: () => hb
                                    .Editor(
                                        siteSettings: siteSettings,
                                        outgoingMailModel: outgoingMailModel)))
                        .FieldSet(id: "FieldSetAddressBook", action: () => hb
                            .Form(
                                attributes: Html.Attributes()
                                    .Id("OutgoingMailDestinationForm")
                                    .Action(Navigations.Action(
                                        referenceType, referenceId, "OutgoingMails")),
                                action: () => hb
                                    .Destinations(
                                        siteSettings: siteSettings,
                                        referenceId: siteModel.InheritPermission)))))
                .Func("initDialog_OutgoingMail")
                .Focus("#OutgoingMails_Body")
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Editor(
            this HtmlBuilder hb,
            SiteSettings siteSettings,
            OutgoingMailModel outgoingMailModel)
        {
            return hb
                .FieldBasket(
                    controlId: "OutgoingMails_To",
                    fieldCss: "field-wide",
                    controlCss: "control-basket cf",
                    labelAction: () => hb
                        .Displays_OutgoingMails_To())
                .FieldBasket(
                    controlId: "OutgoingMails_Cc",
                    fieldCss: "field-wide",
                    controlCss: "control-basket cf",
                    labelAction: () => hb
                        .Displays_OutgoingMails_Cc())
                .FieldBasket(
                    controlId: "OutgoingMails_Bcc",
                    fieldCss: "field-wide",
                    controlCss: "control-basket cf",
                    labelAction: () => hb
                        .Displays_OutgoingMails_Bcc())
                .FieldTextBox(
                    controlId: "OutgoingMails_Title",
                    fieldCss: "field-wide",
                    controlCss: " must-transport",
                    labelText: Displays.OutgoingMails_Title(),
                    text: ReplyTitle(outgoingMailModel))
                .FieldTextBox(
                    textStyle: HtmlControls.TextStyles.MultiLine,
                    controlId: "OutgoingMails_Body",
                    fieldCss: "field-wide",
                    controlCss: " must-transport h300",
                    labelText: Displays.OutgoingMails_Body(),
                    text: ReplyBody(outgoingMailModel))
                .P(css: "message-dialog")
                .Div(css: "command-center", action: () => hb
                    .Button(
                        controlId: "OutgoingMails_Send",
                        controlCss: "button-send-mail validate",
                        text: Displays.SendMail(),
                        onClick: Def.JavaScript.SendMail,
                        action: "Send",
                        method: "post",
                        confirm: "Displays_ConfirmSendMail")
                    .Button(
                        controlId: "OutgoingMails_Cancel",
                        controlCss: "button-cancel",
                        text: Displays.Cancel(),
                        onClick: Def.JavaScript.CancelDialog))
            .Hidden(controlId: "OutgoingMails_Location", value: Location())
            .Hidden(
                controlId: "OutgoingMails_Reply",
                value: outgoingMailModel.AccessStatus == Databases.AccessStatuses.Selected
                    ? "1"
                    : "0")
            .Hidden(
                controlId: "MailToDefault",
                value: MailDefault(outgoingMailModel, siteSettings.MailToDefault, "to"))
            .Hidden(
                controlId: "MailCcDefault",
                value: MailDefault(outgoingMailModel, siteSettings.MailCcDefault, "cc"))
            .Hidden(
                controlId: "MailBccDefault",
                value: MailDefault(outgoingMailModel, siteSettings.MailBccDefault, "bcc"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string ReplyTitle(OutgoingMailModel outgoingMailModel)
        {
            var title = outgoingMailModel.Title.Value;
            return outgoingMailModel.AccessStatus == Databases.AccessStatuses.Selected
                ? title.StartsWith("Re: ") ? title : "Re: " + title
                : string.Empty;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string ReplyBody(OutgoingMailModel outgoingMailModel)
        {
            return outgoingMailModel.AccessStatus == Databases.AccessStatuses.Selected
                ? Displays.OriginalMessage().Params(
                    Location(),
                    outgoingMailModel.From,
                    outgoingMailModel.SentTime.DisplayValue.ToString(
                        Displays.Get("YmdahmsFormat"), Sessions.CultureInfo()),
                    outgoingMailModel.Title.Value,
                    outgoingMailModel.Body)
                : string.Empty;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string MailDefault(
            OutgoingMailModel outgoingMailModel, string mailDefault, string type)
        {
            var myAddress = new MailAddressModel(Sessions.UserId()).MailAddress;
            if ( outgoingMailModel.AccessStatus == Databases.AccessStatuses.Selected)
            {
                switch (type)
                {
                    case "to":
                        var to = outgoingMailModel.To
                            .Split(';')
                            .Where(o => MailAddress(o) != myAddress)
                            .Where(o => o.Trim() != string.Empty)
                            .Join(";");
                        return to.Trim() != string.Empty
                            ? outgoingMailModel.From.ToString() + ";" + to
                            : outgoingMailModel.From.ToString();
                    case "cc":
                        return outgoingMailModel.Cc;
                    case "bcc":
                        return outgoingMailModel.Bcc;
                }
            }
            return mailDefault;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string Location()
        {
            var location = HttpContext.Current.Request.Url.AbsoluteUri.ToLower();
            return location.Substring(0, location.IndexOf("/outgoingmails"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Destinations(
            this HtmlBuilder hb, SiteSettings siteSettings, long referenceId)
        {
            var addressBook = AddressBook(siteSettings);
            var searchRangeDefault = SiteInfo.IsItem()
                ? addressBook.Count > 0
                    ? "DefaultAddressBook"
                    : "SiteUser"
                : "All";
            return hb
                .Div(css: "container-left", action: () => hb
                    .FieldDropDown(
                        controlId: "OutgoingMails_DestinationSearchRange",
                        labelText: Displays.OutgoingMails_DestinationSearchRange(),
                        optionCollection: SearchRangeOptionCollection(
                            searchRangeDefault: searchRangeDefault,
                            addressBook: addressBook),
                        controlCss: " auto-postback must-transport",
                        action: "GetDestinations",
                        method: "put")
                    .FieldTextBox(
                        controlId: "OutgoingMails_DestinationSearchText",
                        labelText: Displays.OutgoingMails_DestinationSearchText(),
                        controlCss: " auto-postback",
                        action: "GetDestinations",
                        method: "put"))
                .Div(css: "container-right", action: () => hb
                    .Div(action: () => hb
                        .FieldSelectable(
                            controlId: "OutgoingMails_MailAddresses",
                            fieldCss: "field-vertical both",
                            controlContainerCss: "container-selectable",
                            controlCss: " h500",
                            listItemCollection: Destinations(
                                referenceId: referenceId,
                                addressBook: addressBook,
                                searchRange: searchRangeDefault),
                            selectedValueCollection: new List<string>())
                        .Div(css: "command-left", action: () => hb
                            .Button(
                                controlId: "OutgoingMails_AddTo",
                                text: Displays.OutgoingMails_To(),
                                controlCss: "button-person")
                            .Button(
                                controlId: "OutgoingMails_AddCc",
                                text: Displays.OutgoingMails_Cc(),
                                controlCss: "button-person")
                            .Button(
                                controlId: "OutgoingMails_AddBcc",
                                text: Displays.OutgoingMails_Bcc(),
                                controlCss: "button-person"))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Dictionary<string, string> AddressBook(SiteSettings siteSettings)
        {
            return siteSettings.AddressBook
                .SplitReturn()
                .Select(o => o.Trim())
                .Where(o => o != string.Empty)
                .ToDictionary(o => o, o => o);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Dictionary<string, ControlData> SearchRangeOptionCollection(
            string searchRangeDefault, Dictionary<string, string> addressBook)
        {
            switch (searchRangeDefault)
            {
                case "DefaultAddressBook":
                    return new Dictionary<string, ControlData>
                    {
                        { "DefaultAddressBook", new ControlData(Displays.DefaultAddressBook()) },
                        { "SiteUser", new ControlData(Displays.SiteUser()) },
                        { "All", new ControlData(Displays.All()) }
                    };
                case "SiteUser":
                    return new Dictionary<string, ControlData>
                    {
                        { "SiteUser", new ControlData(Displays.SiteUser()) },
                        { "All", new ControlData(Displays.All()) }
                    };
                default:
                    return new Dictionary<string, ControlData>
                    {
                        { "All", new ControlData(Displays.All()) }
                    };
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Dictionary<string, string> Destinations(
            long referenceId,
            Dictionary<string, string> addressBook,
            string searchRange,
            string searchText = "")
        {
            var joinMailAddresses = new SqlJoin(
                "inner join [MailAddresses] on [MailAddresses].[OwnerId]=[t0].[UserId]");
            switch (searchRange)
            {
                case "DefaultAddressBook":
                    return searchText == string.Empty
                        ? addressBook
                        : addressBook
                            .Where(o => o.Value.IndexOf(searchText,
                                System.Globalization.CompareOptions.IgnoreCase |
                                System.Globalization.CompareOptions.IgnoreKanaType |
                                System.Globalization.CompareOptions.IgnoreWidth) != -1)
                            .ToDictionary(o => o.Key, o => o.Value);
                case "SiteUser":
                    var joinPermissions = new SqlJoin(
                        "inner join [Permissions] on " +
                        "([t0].[UserId]=[Permissions].[UserId] and [Permissions].[UserId] <> 0) or " +
                        "([t0].[DeptId]=[Permissions].[DeptId] and [Permissions].[DeptId] <> 0)");
                    return DestinationCollection(
                        Sqls.SqlJoinCollection(joinMailAddresses, joinPermissions),
                        Rds.UsersWhere()
                            .MailAddresses_OwnerType("Users")
                            .Permissions_ReferenceType("Sites")
                            .Permissions_ReferenceId(referenceId)
                            .SearchText(searchText)
                            .Users_TenantId(Sessions.TenantId(), "t0"));
                case "All":
                default:
                    return !searchText.IsNullOrEmpty()
                        ? DestinationCollection(
                            Sqls.SqlJoinCollection(joinMailAddresses),
                            Rds.UsersWhere()
                                .MailAddresses_OwnerType("Users")
                                .SearchText(searchText)
                                .Users_TenantId(Sessions.TenantId(), "t0"))
                        : new Dictionary<string, string>();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static SqlWhereCollection SearchText(
            this SqlWhereCollection self, string searchText)
        {
            return self.SqlWhereLike(searchText,
                Rds.Users_UserId_WhereLike(),
                Rds.Users_LoginId_WhereLike(),
                Rds.Users_FirstName_WhereLike(),
                Rds.Users_LastName_WhereLike(),
                Rds.MailAddresses_MailAddress_WhereLike("MailAddresses"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Dictionary<string, string> DestinationCollection(
            SqlJoinCollection join, SqlWhereCollection where)
        {
            return Rds.ExecuteTable(
                transactional: false,
                statements: Rds.SelectUsers(
                    column: Rds.UsersColumn()
                        .FirstName()
                        .LastName()
                        .FirstAndLastNameOrder()
                        .MailAddresses_MailAddress(),
                    join: join,
                    where: where,
                    distinct: true)).AsEnumerable()
                        .Select((o, i) => new { Name = Names.FullName(
                            (Names.FirstAndLastNameOrders)o["FirstAndLastNameOrder"],
                            "\"" + o["FirstName"].ToString() + " " + o["LastName"].ToString() +
                                "\" <" + o["MailAddress"].ToString() + ">",
                            "\"" + o["LastName"].ToString() + " " + o["FirstName"].ToString() +
                                "\" <" + o["MailAddress"].ToString() + ">"), Index = i })
                        .ToDictionary(
                            o => o.Index.ToString(),
                            o => o.Name);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string MailAddress(string mailAddress)
        {
            return mailAddress.RegexFirst(
                @"\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }
    }
}
