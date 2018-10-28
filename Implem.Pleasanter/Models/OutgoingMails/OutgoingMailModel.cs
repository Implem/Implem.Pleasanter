using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.General;
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
    [Serializable]
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
        [NonSerialized] public string SavedReferenceType = string.Empty;
        [NonSerialized] public long SavedReferenceId = 0;
        [NonSerialized] public int SavedReferenceVer = 0;
        [NonSerialized] public long SavedOutgoingMailId = 0;
        [NonSerialized] public string SavedHost = string.Empty;
        [NonSerialized] public int SavedPort = 0;
        [NonSerialized] public string SavedFrom = "null";
        [NonSerialized] public string SavedTo = string.Empty;
        [NonSerialized] public string SavedCc = string.Empty;
        [NonSerialized] public string SavedBcc = string.Empty;
        [NonSerialized] public string SavedTitle = string.Empty;
        [NonSerialized] public string SavedBody = string.Empty;
        [NonSerialized] public DateTime SavedSentTime = 0.ToDateTime();
        [NonSerialized] public string SavedDestinationSearchRange = string.Empty;
        [NonSerialized] public string SavedDestinationSearchText = string.Empty;

        public bool ReferenceType_Updated(Context context, Column column = null)
        {
            return ReferenceType != SavedReferenceType && ReferenceType != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != ReferenceType);
        }

        public bool ReferenceId_Updated(Context context, Column column = null)
        {
            return ReferenceId != SavedReferenceId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToLong() != ReferenceId);
        }

        public bool ReferenceVer_Updated(Context context, Column column = null)
        {
            return ReferenceVer != SavedReferenceVer &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != ReferenceVer);
        }

        public bool OutgoingMailId_Updated(Context context, Column column = null)
        {
            return OutgoingMailId != SavedOutgoingMailId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToLong() != OutgoingMailId);
        }

        public bool Host_Updated(Context context, Column column = null)
        {
            return Host != SavedHost && Host != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != Host);
        }

        public bool Port_Updated(Context context, Column column = null)
        {
            return Port != SavedPort &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != Port);
        }

        public bool From_Updated(Context context, Column column = null)
        {
            return From.ToString() != SavedFrom && From.ToString() != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != From.ToString());
        }

        public bool To_Updated(Context context, Column column = null)
        {
            return To != SavedTo && To != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != To);
        }

        public bool Cc_Updated(Context context, Column column = null)
        {
            return Cc != SavedCc && Cc != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != Cc);
        }

        public bool Bcc_Updated(Context context, Column column = null)
        {
            return Bcc != SavedBcc && Bcc != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != Bcc);
        }

        public bool Title_Updated(Context context, Column column = null)
        {
            return Title.Value != SavedTitle && Title.Value != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != Title.Value);
        }

        public bool Body_Updated(Context context, Column column = null)
        {
            return Body != SavedBody && Body != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != Body);
        }

        public bool SentTime_Updated(Context context, Column column = null)
        {
            return SentTime.Value != SavedSentTime &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != SentTime.Value.Date);
        }

        public OutgoingMailModel()
        {
        }

        public OutgoingMailModel(
            Context context,
            bool setByForm = false,
            bool setByApi = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            Context = context;
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public OutgoingMailModel(
            Context context,
            long outgoingMailId,
            bool clearSessions = false,
            bool setByForm = false,
            bool setByApi = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            Context = context;
            OutgoingMailId = outgoingMailId;
            Get(context: context);
            if (clearSessions) ClearSessions(context: context);
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public OutgoingMailModel(Context context, DataRow dataRow, string tableAlias = null)
        {
            OnConstructing(context: context);
            Context = context;
            if (dataRow != null) Set(context, dataRow, tableAlias);
            OnConstructed(context: context);
        }

        private void OnConstructing(Context context)
        {
        }

        private void OnConstructed(Context context)
        {
        }

        public void ClearSessions(Context context)
        {
        }

        public OutgoingMailModel Get(
            Context context,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            Set(context, Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectOutgoingMails(
                    tableType: tableType,
                    column: column ?? Rds.OutgoingMailsDefaultColumns(),
                    join: join ??  Rds.OutgoingMailsJoinDefault(),
                    where: where ?? Rds.OutgoingMailsWhereDefault(this),
                    orderBy: orderBy,
                    param: param,
                    distinct: distinct,
                    top: top)));
            return this;
        }

        public Error.Types Create(
            Context context,
            SiteSettings ss,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool otherInitValue = false,
            bool get = true)
        {
            var statements = new List<SqlStatement>();
            CreateStatements(context, statements, tableType, param, otherInitValue);
            var response = Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: statements.ToArray());
            OutgoingMailId = (response.Identity ?? OutgoingMailId).ToLong();
            if (get) Get(context: context);
            return Error.Types.None;
        }

        public List<SqlStatement> CreateStatements(
            Context context,
            List<SqlStatement> statements,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool otherInitValue = false)
        {
            statements.AddRange(new List<SqlStatement>
            {
                Rds.InsertOutgoingMails(
                    tableType: tableType,
                    setIdentity: true,
                    param: param ?? Rds.OutgoingMailsParamDefault(
                        context: context,
                        outgoingMailModel: this,
                        setDefault: true,
                        otherInitValue: otherInitValue))
            });
            return statements;
        }

        public Error.Types Update(
            Context context,
            SiteSettings ss,
            SqlParamCollection param = null,
            List<SqlStatement> additionalStatements = null,
            bool otherInitValue = false,
            bool setBySession = true,
            bool get = true)
        {
            if (setBySession) SetBySession(context: context);
            var timestamp = Timestamp.ToDateTime();
            var statements = new List<SqlStatement>();
            UpdateStatements(
                context: context,
                ss: ss,
                statements: statements,
                timestamp: timestamp,
                param: param,
                otherInitValue: otherInitValue,
                additionalStatements: additionalStatements);
            var response = Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            if (response.Count == 0) return Error.Types.UpdateConflicts;
            if (get) Get(context: context);
            Libraries.Search.Indexes.Create(context, ss, ReferenceId, force: true);
            return Error.Types.None;
        }

        private List<SqlStatement> UpdateStatements(
            Context context,
            SiteSettings ss,
            List<SqlStatement> statements,
            DateTime timestamp,
            SqlParamCollection param,
            bool otherInitValue = false,
            List<SqlStatement> additionalStatements = null)
        {
            var where = Rds.OutgoingMailsWhereDefault(this)
                .UpdatedTime(timestamp, _using: timestamp.InRange());
            if (VerUp)
            {
                statements.Add(CopyToStatement(where, Sqls.TableTypes.History));
                Ver++;
            }
            statements.AddRange(new List<SqlStatement>
            {
                Rds.UpdateOutgoingMails(
                    where: where,
                    param: param ?? Rds.OutgoingMailsParamDefault(
                        context: context, outgoingMailModel: this, otherInitValue: otherInitValue),
                    countRecord: true)
            });
            if (additionalStatements?.Any() == true)
            {
                statements.AddRange(additionalStatements);
            }
            return statements;
        }

        private SqlStatement CopyToStatement(SqlWhereCollection where, Sqls.TableTypes tableType)
        {
            var column = new Rds.OutgoingMailsColumnCollection();
            var param = new Rds.OutgoingMailsParamCollection();
            column.ReferenceType(function: Sqls.Functions.SingleColumn); param.ReferenceType();
            column.ReferenceId(function: Sqls.Functions.SingleColumn); param.ReferenceId();
            column.ReferenceVer(function: Sqls.Functions.SingleColumn); param.ReferenceVer();
            column.OutgoingMailId(function: Sqls.Functions.SingleColumn); param.OutgoingMailId();
            column.Ver(function: Sqls.Functions.SingleColumn); param.Ver();
            column.Host(function: Sqls.Functions.SingleColumn); param.Host();
            column.Port(function: Sqls.Functions.SingleColumn); param.Port();
            column.From(function: Sqls.Functions.SingleColumn); param.From();
            column.To(function: Sqls.Functions.SingleColumn); param.To();
            column.Cc(function: Sqls.Functions.SingleColumn); param.Cc();
            column.Bcc(function: Sqls.Functions.SingleColumn); param.Bcc();
            column.Title(function: Sqls.Functions.SingleColumn); param.Title();
            column.Body(function: Sqls.Functions.SingleColumn); param.Body();
            column.SentTime(function: Sqls.Functions.SingleColumn); param.SentTime();
            column.Comments(function: Sqls.Functions.SingleColumn); param.Comments();
            column.Creator(function: Sqls.Functions.SingleColumn); param.Creator();
            column.Updator(function: Sqls.Functions.SingleColumn); param.Updator();
            column.CreatedTime(function: Sqls.Functions.SingleColumn); param.CreatedTime();
            column.UpdatedTime(function: Sqls.Functions.SingleColumn); param.UpdatedTime();
            return Rds.InsertOutgoingMails(
                tableType: tableType,
                param: param,
                select: Rds.SelectOutgoingMails(column: column, where: where),
                addUpdatorParam: false);
        }

        public Error.Types UpdateOrCreate(
            Context context,
            SqlWhereCollection where = null,
            SqlParamCollection param = null)
        {
            SetBySession(context: context);
            var statements = new List<SqlStatement>
            {
                Rds.UpdateOrInsertOutgoingMails(
                    where: where ?? Rds.OutgoingMailsWhereDefault(this),
                    param: param ?? Rds.OutgoingMailsParamDefault(
                        context: context, outgoingMailModel: this, setDefault: true))
            };
            var response = Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: statements.ToArray());
            OutgoingMailId = (response.Identity ?? OutgoingMailId).ToLong();
            Get(context: context);
            return Error.Types.None;
        }

        public Error.Types Delete(Context context)
        {
            var statements = new List<SqlStatement>();
            var where = Rds.OutgoingMailsWhere().OutgoingMailId(OutgoingMailId);
            statements.AddRange(new List<SqlStatement>
            {
                Rds.DeleteOutgoingMails(where: where)
            });
            var response = Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            return Error.Types.None;
        }

        public Error.Types Restore(Context context, long outgoingMailId)
        {
            OutgoingMailId = outgoingMailId;
            Rds.ExecuteNonQuery(
                context: context,
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreOutgoingMails(
                        where: Rds.OutgoingMailsWhere().OutgoingMailId(OutgoingMailId))
                });
            return Error.Types.None;
        }

        public Error.Types PhysicalDelete(
            Context context, Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Rds.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: Rds.PhysicalDeleteOutgoingMails(
                    tableType: tableType,
                    param: Rds.OutgoingMailsParam().OutgoingMailId(OutgoingMailId)));
            return Error.Types.None;
        }

        public void SetByModel(OutgoingMailModel outgoingMailModel)
        {
            ReferenceType = outgoingMailModel.ReferenceType;
            ReferenceId = outgoingMailModel.ReferenceId;
            ReferenceVer = outgoingMailModel.ReferenceVer;
            Host = outgoingMailModel.Host;
            Port = outgoingMailModel.Port;
            From = outgoingMailModel.From;
            To = outgoingMailModel.To;
            Cc = outgoingMailModel.Cc;
            Bcc = outgoingMailModel.Bcc;
            Title = outgoingMailModel.Title;
            Body = outgoingMailModel.Body;
            SentTime = outgoingMailModel.SentTime;
            DestinationSearchRange = outgoingMailModel.DestinationSearchRange;
            DestinationSearchText = outgoingMailModel.DestinationSearchText;
            Comments = outgoingMailModel.Comments;
            Creator = outgoingMailModel.Creator;
            Updator = outgoingMailModel.Updator;
            CreatedTime = outgoingMailModel.CreatedTime;
            UpdatedTime = outgoingMailModel.UpdatedTime;
            VerUp = outgoingMailModel.VerUp;
            Comments = outgoingMailModel.Comments;
        }

        private void SetBySession(Context context)
        {
        }

        private void Set(Context context, DataTable dataTable)
        {
            switch (dataTable.Rows.Count)
            {
                case 1: Set(context, dataTable.Rows[0]); break;
                case 0: AccessStatus = Databases.AccessStatuses.NotFound; break;
                default: AccessStatus = Databases.AccessStatuses.Overlap; break;
            }
        }

        private void Set(Context context, DataRow dataRow, string tableAlias = null)
        {
            AccessStatus = Databases.AccessStatuses.Selected;
            foreach(DataColumn dataColumn in dataRow.Table.Columns)
            {
                var column = new ColumnNameInfo(dataColumn.ColumnName);
                if (column.TableAlias == tableAlias)
                {
                    switch (column.Name)
                    {
                        case "ReferenceType":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                ReferenceType = dataRow[column.ColumnName].ToString();
                                SavedReferenceType = ReferenceType;
                            }
                            break;
                        case "ReferenceId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                ReferenceId = dataRow[column.ColumnName].ToLong();
                                SavedReferenceId = ReferenceId;
                            }
                            break;
                        case "ReferenceVer":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                ReferenceVer = dataRow[column.ColumnName].ToInt();
                                SavedReferenceVer = ReferenceVer;
                            }
                            break;
                        case "OutgoingMailId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                OutgoingMailId = dataRow[column.ColumnName].ToLong();
                                SavedOutgoingMailId = OutgoingMailId;
                            }
                            break;
                        case "Ver":
                            Ver = dataRow[column.ColumnName].ToInt();
                            SavedVer = Ver;
                            break;
                        case "Host":
                            Host = dataRow[column.ColumnName].ToString();
                            SavedHost = Host;
                            break;
                        case "Port":
                            Port = dataRow[column.ColumnName].ToInt();
                            SavedPort = Port;
                            break;
                        case "From":
                            From = new System.Net.Mail.MailAddress(dataRow[column.ColumnName].ToString());
                            SavedFrom = From.ToString();
                            break;
                        case "To":
                            To = dataRow[column.ColumnName].ToString();
                            SavedTo = To;
                            break;
                        case "Cc":
                            Cc = dataRow[column.ColumnName].ToString();
                            SavedCc = Cc;
                            break;
                        case "Bcc":
                            Bcc = dataRow[column.ColumnName].ToString();
                            SavedBcc = Bcc;
                            break;
                        case "Title":
                            Title = new Title(dataRow, "OutgoingMailId");
                            SavedTitle = Title.Value;
                            break;
                        case "Body":
                            Body = dataRow[column.ColumnName].ToString();
                            SavedBody = Body;
                            break;
                        case "SentTime":
                            SentTime = new Time(context, dataRow, column.ColumnName);
                            SavedSentTime = SentTime.Value;
                            break;
                        case "Comments":
                            Comments = dataRow[column.ColumnName].ToString().Deserialize<Comments>() ?? new Comments();
                            SavedComments = Comments.ToJson();
                            break;
                        case "Creator":
                            Creator = SiteInfo.User(context: context, userId: dataRow.Int(column.ColumnName));
                            SavedCreator = Creator.Id;
                            break;
                        case "Updator":
                            Updator = SiteInfo.User(context: context, userId: dataRow.Int(column.ColumnName));
                            SavedUpdator = Updator.Id;
                            break;
                        case "CreatedTime":
                            CreatedTime = new Time(context, dataRow, column.ColumnName);
                            SavedCreatedTime = CreatedTime.Value;
                            break;
                        case "UpdatedTime":
                            UpdatedTime = new Time(context, dataRow, column.ColumnName); Timestamp = dataRow.Field<DateTime>(column.ColumnName).ToString("yyyy/M/d H:m:s.fff");
                            SavedUpdatedTime = UpdatedTime.Value;
                            break;
                        case "IsHistory": VerType = dataRow[column.ColumnName].ToBool() ? Versions.VerTypes.History : Versions.VerTypes.Latest; break;
                    }
                }
            }
        }

        public bool Updated(Context context)
        {
            return
                ReferenceType_Updated(context: context) ||
                ReferenceId_Updated(context: context) ||
                ReferenceVer_Updated(context: context) ||
                OutgoingMailId_Updated(context: context) ||
                Ver_Updated(context: context) ||
                Host_Updated(context: context) ||
                Port_Updated(context: context) ||
                From_Updated(context: context) ||
                To_Updated(context: context) ||
                Cc_Updated(context: context) ||
                Bcc_Updated(context: context) ||
                Title_Updated(context: context) ||
                Body_Updated(context: context) ||
                SentTime_Updated(context: context) ||
                Comments_Updated(context: context) ||
                Creator_Updated(context: context) ||
                Updator_Updated(context: context);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public OutgoingMailModel(Context context, string reference, long referenceId)
        {
            if (reference.ToLower() == "items")
            {
                var itemModel = new ItemModel(
                    context: context,
                    referenceId: referenceId);
                ReferenceType = itemModel.ReferenceType;
            }
            else
            {
                ReferenceType = reference.ToLower();
            }
            ReferenceId = referenceId;
            ReferenceVer = Forms.Int("Ver");
            From = OutgoingMailUtilities.From(
                context: context,
                userId: context.UserId);
            SetByForm(context: context);
            if (Libraries.Mails.Addresses.FixedFrom(From))
            {
                Body += "\n\n{0}<{1}>".Params(From.DisplayName, From.Address);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public void SetByForm(Context context)
        {
            var ss = SiteSettingsUtilities.OutgoingMailsSiteSettings(context: context);
            Forms.Keys().ForEach(controlId =>
            {
                switch (controlId)
                {
                    case "OutgoingMails_To": To = Forms.List(controlId).Join(";"); break;
                    case "OutgoingMails_Cc": Cc = Forms.List(controlId).Join(";"); break;
                    case "OutgoingMails_Bcc": Bcc = Forms.List(controlId).Join(";"); break;
                    case "OutgoingMails_Title": Title = new Title(OutgoingMailId, Forms.Data(controlId)); break;
                    case "OutgoingMails_Body": Body = Forms.Data(controlId).ToString(); break;
                    case "OutgoingMails_SentTime": SentTime = new Time(context, Forms.Data(controlId).ToDateTime(), byForm: true); break;
                    case "OutgoingMails_DestinationSearchRange": DestinationSearchRange = Forms.Data(controlId).ToString(); break;
                    case "OutgoingMails_DestinationSearchText": DestinationSearchText = Forms.Data(controlId).ToString(); break;
                    case "OutgoingMails_Timestamp": Timestamp = Forms.Data(controlId).ToString(); break;
                    case "Comments":
                        Comments.Prepend(
                            context: context,
                            ss: new SiteSettings(),
                            body: Forms.Data("Comments"));
                        break;
                    case "VerUp": VerUp = Forms.Data(controlId).ToBool(); break;
                    default:
                        if (controlId.RegexExists("Comment[0-9]+"))
                        {
                            Comments.Update(
                                context: context,
                                ss: ss,
                                commentId: controlId.Substring("Comment".Length).ToInt(),
                                body: Forms.Data(controlId));
                        }
                        break;
                }
            });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string GetDestinations(Context context)
        {
            var siteModel = new ItemModel(
                context: context,
                referenceId: ReferenceId).GetSite(context: context);
            var ss = siteModel.SitesSiteSettings(context: context, referenceId: ReferenceId);
            if (context.ContractSettings.Mail == false)
            {
                return Error.Types.Restricted.MessageJson(context: context);
            }
            return new OutgoingMailsResponseCollection(this)
                .Html("#OutgoingMails_MailAddresses",
                    new HtmlBuilder().SelectableItems(
                        listItemCollection: OutgoingMailUtilities.Destinations(
                            context: context,
                            ss: ss,
                            referenceId: siteModel.InheritPermission,
                            addressBook: OutgoingMailUtilities.AddressBook(ss),
                            searchRange: DestinationSearchRange,
                            searchText: DestinationSearchText),
                        selectedValueTextCollection: new List<string>())).ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public Error.Types Send(
            Context context, SiteSettings ss, List<SqlStatement> additionalStatements = null)
        {
            var error = Create(context: context, ss: ss);
            if (error.Has()) return error;
            Host = Parameters.Mail.SmtpHost;
            Port = Parameters.Mail.SmtpPort;
            switch (Host)
            {
                case "smtp.sendgrid.net":
                    SendBySendGrid(context: context);
                    break;
                default:
                    SendBySmtp(context: context);
                    break;
            }
            SentTime = new Time(context, DateTime.Now);
            error = Update(context: context, ss: ss, additionalStatements: additionalStatements);
            return error.Has()
                ? error
                : Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SendBySmtp(Context context)
        {
            new Smtp(
                context: context,
                host: Host,
                port: Port,
                from: From,
                to: To,
                cc: Cc,
                bcc: Bcc,
                subject: Title.Value,
                body: Body)
                    .Send();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SendBySendGrid(Context context)
        {
            new SendGridMail(
                context: context,
                host: Host,
                from: From,
                to: To,
                cc: Cc,
                bcc: Bcc,
                subject: Title.Value,
                body: Body)
                    .Send();
        }
    }
}
