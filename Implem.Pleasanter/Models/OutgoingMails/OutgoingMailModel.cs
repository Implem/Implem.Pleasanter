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

        public bool ReferenceType_Updated(Column column = null)
        {
            return ReferenceType != SavedReferenceType && ReferenceType != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != ReferenceType);
        }

        public bool ReferenceId_Updated(Column column = null)
        {
            return ReferenceId != SavedReferenceId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToLong() != ReferenceId);
        }

        public bool ReferenceVer_Updated(Column column = null)
        {
            return ReferenceVer != SavedReferenceVer &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToInt() != ReferenceVer);
        }

        public bool OutgoingMailId_Updated(Column column = null)
        {
            return OutgoingMailId != SavedOutgoingMailId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToLong() != OutgoingMailId);
        }

        public bool Host_Updated(Column column = null)
        {
            return Host != SavedHost && Host != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != Host);
        }

        public bool Port_Updated(Column column = null)
        {
            return Port != SavedPort &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToInt() != Port);
        }

        public bool From_Updated(Column column = null)
        {
            return From.ToString() != SavedFrom && From.ToString() != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != From.ToString());
        }

        public bool To_Updated(Column column = null)
        {
            return To != SavedTo && To != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != To);
        }

        public bool Cc_Updated(Column column = null)
        {
            return Cc != SavedCc && Cc != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != Cc);
        }

        public bool Bcc_Updated(Column column = null)
        {
            return Bcc != SavedBcc && Bcc != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != Bcc);
        }

        public bool Title_Updated(Column column = null)
        {
            return Title.Value != SavedTitle && Title.Value != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != Title.Value);
        }

        public bool Body_Updated(Column column = null)
        {
            return Body != SavedBody && Body != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != Body);
        }

        public bool SentTime_Updated(Column column = null)
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
            bool setByForm = false,
            bool setByApi = false,
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
            bool setByApi = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            OutgoingMailId = outgoingMailId;
            Get();
            if (clearSessions) ClearSessions();
            if (setByForm) SetByForm();
            MethodType = methodType;
            OnConstructed();
        }

        public OutgoingMailModel(DataRow dataRow, string tableAlias = null)
        {
            OnConstructing();
            Set(dataRow, tableAlias);
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
            RdsUser rdsUser = null,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool otherInitValue = false,
            bool get = true)
        {
            var statements = new List<SqlStatement>();
            CreateStatements(statements, tableType, param, otherInitValue);
            var newId = Rds.ExecuteScalar_long(
                rdsUser: rdsUser,
                transactional: true,
                statements: statements.ToArray());
            OutgoingMailId = newId != 0 ? newId : OutgoingMailId;
            if (get) Get();
            return Error.Types.None;
        }

        public List<SqlStatement> CreateStatements(
            List<SqlStatement> statements,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool otherInitValue = false)
        {
            statements.AddRange(new List<SqlStatement>
            {
                Rds.InsertOutgoingMails(
                    tableType: tableType,
                        selectIdentity: true,
                    param: param ?? Rds.OutgoingMailsParamDefault(
                        this, setDefault: true, otherInitValue: otherInitValue))
            });
            return statements;
        }

        public Error.Types Update(
            RdsUser rdsUser = null,
            SqlParamCollection param = null,
            List<SqlStatement> additionalStatements = null,
            bool otherInitValue = false,
            bool get = true)
        {
            SetBySession();
            var timestamp = Timestamp.ToDateTime();
            var statements = new List<SqlStatement>();
            UpdateStatements(statements, timestamp, param, otherInitValue, additionalStatements);
            var count = Rds.ExecuteScalar_int(
                rdsUser: rdsUser,
                transactional: true,
                statements: statements.ToArray());
            if (count == 0) return Error.Types.UpdateConflicts;
            if (get) Get();
            var siteModel = new ItemModel(ReferenceId).GetSite();
            var ss = SiteSettingsUtilities.Get(siteModel, siteModel.SiteId);
            Libraries.Search.Indexes.Create(ss, ReferenceId, force: true);
            return Error.Types.None;
        }

        private List<SqlStatement> UpdateStatements(
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
                    param: param ?? Rds.OutgoingMailsParamDefault(this, otherInitValue: otherInitValue),
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
            column.Creator(function: Sqls.Functions.SingleColumn); param.Creator();
            column.Updator(function: Sqls.Functions.SingleColumn); param.Updator();
            column.CreatedTime(function: Sqls.Functions.SingleColumn); param.CreatedTime();
            column.UpdatedTime(function: Sqls.Functions.SingleColumn); param.UpdatedTime();
            if (!Host.InitialValue())
            {
                column.Host(function: Sqls.Functions.SingleColumn);
                param.Host();
            }
            if (!Port.InitialValue())
            {
                column.Port(function: Sqls.Functions.SingleColumn);
                param.Port();
            }
            if (!From.InitialValue())
            {
                column.From(function: Sqls.Functions.SingleColumn);
                param.From();
            }
            if (!To.InitialValue())
            {
                column.To(function: Sqls.Functions.SingleColumn);
                param.To();
            }
            if (!Cc.InitialValue())
            {
                column.Cc(function: Sqls.Functions.SingleColumn);
                param.Cc();
            }
            if (!Bcc.InitialValue())
            {
                column.Bcc(function: Sqls.Functions.SingleColumn);
                param.Bcc();
            }
            if (!Title.InitialValue())
            {
                column.Title(function: Sqls.Functions.SingleColumn);
                param.Title();
            }
            if (!Body.InitialValue())
            {
                column.Body(function: Sqls.Functions.SingleColumn);
                param.Body();
            }
            if (!SentTime.InitialValue())
            {
                column.SentTime(function: Sqls.Functions.SingleColumn);
                param.SentTime();
            }
            if (!Comments.InitialValue())
            {
                column.Comments(function: Sqls.Functions.SingleColumn);
                param.Comments();
            }
            return Rds.InsertOutgoingMails(
                tableType: tableType,
                param: param,
                select: Rds.SelectOutgoingMails(column: column, where: where),
                addUpdatorParam: false);
        }

        public Error.Types UpdateOrCreate(
            RdsUser rdsUser = null,
            SqlWhereCollection where = null,
            SqlParamCollection param = null)
        {
            SetBySession();
            var statements = new List<SqlStatement>
            {
                Rds.UpdateOrInsertOutgoingMails(
                    selectIdentity: true,
                    where: where ?? Rds.OutgoingMailsWhereDefault(this),
                    param: param ?? Rds.OutgoingMailsParamDefault(this, setDefault: true))
            };
            var newId = Rds.ExecuteScalar_long(
                rdsUser: rdsUser,
                transactional: true,
                statements: statements.ToArray());
            OutgoingMailId = newId != 0 ? newId : OutgoingMailId;
            Get();
            return Error.Types.None;
        }

        public Error.Types Delete()
        {
            var statements = new List<SqlStatement>();
            var where = Rds.OutgoingMailsWhere().OutgoingMailId(OutgoingMailId);
            statements.AddRange(new List<SqlStatement>
            {
                CopyToStatement(where, Sqls.TableTypes.Deleted),
                Rds.PhysicalDeleteOutgoingMails(where: where)
            });
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: statements.ToArray());
            return Error.Types.None;
        }

        public Error.Types Restore(long outgoingMailId)
        {
            OutgoingMailId = outgoingMailId;
            Rds.ExecuteNonQuery(
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
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: Rds.PhysicalDeleteOutgoingMails(
                    tableType: tableType,
                    param: Rds.OutgoingMailsParam().OutgoingMailId(OutgoingMailId)));
            return Error.Types.None;
        }

        public void SetByForm()
        {
            Forms.Keys().ForEach(controlId =>
            {
                switch (controlId)
                {
                    case "OutgoingMails_To": To = Forms.List(controlId).Join(";"); break;
                    case "OutgoingMails_Cc": Cc = Forms.List(controlId).Join(";"); break;
                    case "OutgoingMails_Bcc": Bcc = Forms.List(controlId).Join(";"); break;
                    case "OutgoingMails_Title": Title = new Title(OutgoingMailId, Forms.Data(controlId)); break;
                    case "OutgoingMails_Body": Body = Forms.Data(controlId).ToString(); break;
                    case "OutgoingMails_SentTime": SentTime = new Time(Forms.Data(controlId).ToDateTime(), byForm: true); break;
                    case "OutgoingMails_DestinationSearchRange": DestinationSearchRange = Forms.Data(controlId).ToString(); break;
                    case "OutgoingMails_DestinationSearchText": DestinationSearchText = Forms.Data(controlId).ToString(); break;
                    case "OutgoingMails_Timestamp": Timestamp = Forms.Data(controlId).ToString(); break;
                    case "Comments": Comments.Prepend(Forms.Data("Comments")); break;
                    case "VerUp": VerUp = Forms.Data(controlId).ToBool(); break;
                    default:
                        if (controlId.RegexExists("Comment[0-9]+"))
                        {
                            Comments.Update(
                                controlId.Substring("Comment".Length).ToInt(),
                                Forms.Data(controlId));
                        }
                        break;
                }
            });
            if (Routes.Action() == "deletecomment")
            {
                DeleteCommentId = Forms.ControlId().Split(',')._2nd().ToInt();
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

        private void Set(DataRow dataRow, string tableAlias = null)
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
                            SentTime = new Time(dataRow, column.ColumnName);
                            SavedSentTime = SentTime.Value;
                            break;
                        case "Comments":
                            Comments = dataRow[column.ColumnName].ToString().Deserialize<Comments>() ?? new Comments();
                            SavedComments = Comments.ToJson();
                            break;
                        case "Creator":
                            Creator = SiteInfo.User(dataRow[column.ColumnName].ToInt());
                            SavedCreator = Creator.Id;
                            break;
                        case "Updator":
                            Updator = SiteInfo.User(dataRow[column.ColumnName].ToInt());
                            SavedUpdator = Updator.Id;
                            break;
                        case "CreatedTime":
                            CreatedTime = new Time(dataRow, column.ColumnName);
                            SavedCreatedTime = CreatedTime.Value;
                            break;
                        case "UpdatedTime":
                            UpdatedTime = new Time(dataRow, column.ColumnName); Timestamp = dataRow.Field<DateTime>(column.ColumnName).ToString("yyyy/M/d H:m:s.fff");
                            SavedUpdatedTime = UpdatedTime.Value;
                            break;
                        case "IsHistory": VerType = dataRow[column.ColumnName].ToBool() ? Versions.VerTypes.History : Versions.VerTypes.Latest; break;
                    }
                }
            }
        }

        public bool Updated()
        {
            return
                ReferenceType_Updated() ||
                ReferenceId_Updated() ||
                ReferenceVer_Updated() ||
                OutgoingMailId_Updated() ||
                Ver_Updated() ||
                Host_Updated() ||
                Port_Updated() ||
                From_Updated() ||
                To_Updated() ||
                Cc_Updated() ||
                Bcc_Updated() ||
                Title_Updated() ||
                Body_Updated() ||
                SentTime_Updated() ||
                Comments_Updated() ||
                Creator_Updated() ||
                Updator_Updated();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public OutgoingMailModel(string reference, long referenceId)
        {
            if (reference.ToLower() == "items")
            {
                var itemModel = new ItemModel(referenceId);
                ReferenceType = itemModel.ReferenceType;
            }
            else
            {
                ReferenceType = reference.ToLower();
            }
            ReferenceId = referenceId;
            ReferenceVer = Forms.Int("Ver");
            From = OutgoingMailUtilities.From();
            SetByForm();
            if (Libraries.Mails.Addresses.FixedFrom(From))
            {
                Body += "\n\n{0}<{1}>".Params(From.DisplayName, From.Address);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string GetDestinations()
        {
            if (!Contract.Mail())
            {
                return Error.Types.Restricted.MessageJson();
            }
            var siteModel = new ItemModel(ReferenceId).GetSite();
            var ss = siteModel.SitesSiteSettings(ReferenceId);
            return new OutgoingMailsResponseCollection(this)
                .Html("#OutgoingMails_MailAddresses",
                    new HtmlBuilder().SelectableItems(
                        listItemCollection: OutgoingMailUtilities.Destinations(
                            referenceId: siteModel.InheritPermission,
                            addressBook: OutgoingMailUtilities.AddressBook(ss),
                            searchRange: DestinationSearchRange,
                            searchText: DestinationSearchText),
                        selectedValueTextCollection: new List<string>())).ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public Error.Types Send(List<SqlStatement> additionalStatements = null)
        {
            var error = Create();
            if (error.Has()) return error;
            Host = Parameters.Mail.SmtpHost;
            Port = Parameters.Mail.SmtpPort;
            switch (Host)
            {
                case "smtp.sendgrid.net": SendBySendGrid(); break;
                default: SendBySmtp(); break;
            }
            SentTime = new Time(DateTime.Now);
            error = Update(additionalStatements: additionalStatements);
            return error.Has()
                ? error
                : Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SendBySmtp()
        {
            new Smtp(
                Host,
                Port,
                From,
                To,
                Cc,
                Bcc,
                Title.Value,
                Body)
                    .Send();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SendBySendGrid()
        {
            new SendGridMail(
                Host,
                From,
                To,
                Cc,
                Bcc,
                Title.Value,
                Body)
                    .Send();
        }
    }
}
