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
using Implem.Pleasanter.Libraries.ServerScripts;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
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
        public MimeKit.MailboxAddress From = null;
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

        public bool ReferenceType_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != ReferenceType;
            }
            return ReferenceType != SavedReferenceType && ReferenceType != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != ReferenceType);
        }

        public bool ReferenceId_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToLong() != ReferenceId;
            }
            return ReferenceId != SavedReferenceId
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToLong() != ReferenceId);
        }

        public bool ReferenceVer_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToInt() != ReferenceVer;
            }
            return ReferenceVer != SavedReferenceVer
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToInt() != ReferenceVer);
        }

        public bool OutgoingMailId_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToLong() != OutgoingMailId;
            }
            return OutgoingMailId != SavedOutgoingMailId
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToLong() != OutgoingMailId);
        }

        public bool Host_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != Host;
            }
            return Host != SavedHost && Host != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != Host);
        }

        public bool Port_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToInt() != Port;
            }
            return Port != SavedPort
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToInt() != Port);
        }

        public bool From_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != From.ToString();
            }
            return From.ToString() != SavedFrom && From.ToString() != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != From.ToString());
        }

        public bool To_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != To;
            }
            return To != SavedTo && To != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != To);
        }

        public bool Cc_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != Cc;
            }
            return Cc != SavedCc && Cc != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != Cc);
        }

        public bool Bcc_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != Bcc;
            }
            return Bcc != SavedBcc && Bcc != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != Bcc);
        }

        public bool Title_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != Title.Value;
            }
            return Title.Value != SavedTitle && Title.Value != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != Title.Value);
        }

        public bool Body_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != Body;
            }
            return Body != SavedBody && Body != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != Body);
        }

        public bool SentTime_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToDateTime() != SentTime.Value;
            }
            return SentTime.Value != SavedSentTime
                && (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.DefaultTime(context: context).Date != SentTime.Value.Date);
        }

        public OutgoingMailModel()
        {
        }

        public OutgoingMailModel(
            Context context,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public OutgoingMailModel(
            Context context,
            long outgoingMailId,
            SqlColumnCollection column = null,
            bool clearSessions = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            OutgoingMailId = outgoingMailId;
            if (context.QueryStrings.ContainsKey("ver"))
            {
                Get(
                    context: context,
                    tableType: Sqls.TableTypes.NormalAndHistory,
                    column: column,
                    where: Rds.OutgoingMailsWhereDefault(
                        context: context,
                        outgoingMailModel: this)
                            .OutgoingMails_Ver(context.QueryStrings.Int("ver")));
            }
            else
            {
                Get(
                    context: context,
                    column: column);
            }
            if (clearSessions) ClearSessions(context: context);
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public OutgoingMailModel(
            Context context,
            DataRow dataRow,
            string tableAlias = null)
        {
            OnConstructing(context: context);
            if (dataRow != null)
            {
                Set(
                    context: context,
                    dataRow: dataRow,
                    tableAlias: tableAlias);
            }
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
            where = where ?? Rds.OutgoingMailsWhereDefault(
                context: context,
                outgoingMailModel: this);
            column = (column ?? Rds.OutgoingMailsDefaultColumns());
            join = join ?? Rds.OutgoingMailsJoinDefault();
            Set(context, Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectOutgoingMails(
                    tableType: tableType,
                    column: column,
                    join: join,
                    where: where,
                    orderBy: orderBy,
                    param: param,
                    distinct: distinct,
                    top: top)));
            return this;
        }

        public ErrorData Create(
            Context context,
            SiteSettings ss,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            string noticeType = "Created",
            bool otherInitValue = false,
            bool get = true)
        {
            var statements = new List<SqlStatement>();
            statements.AddRange(CreateStatements(
                context: context,
                ss: ss,
                tableType: tableType,
                param: param,
                otherInitValue: otherInitValue));
            var response = Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: statements.ToArray());
            OutgoingMailId = (response.Id ?? OutgoingMailId).ToLong();
            if (get) Get(context: context);
            return new ErrorData(type: Error.Types.None);
        }

        public List<SqlStatement> CreateStatements(
            Context context,
            SiteSettings ss,
            string dataTableName = null,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool otherInitValue = false)
        {
            var statements = new List<SqlStatement>();
            statements.AddRange(new List<SqlStatement>
            {
                Rds.InsertOutgoingMails(
                    dataTableName: dataTableName,
                    tableType: tableType,
                    selectIdentity: true,
                    param: param ?? Rds.OutgoingMailsParamDefault(
                        context: context,
                        ss: ss,
                        outgoingMailModel: this,
                        setDefault: true,
                        otherInitValue: otherInitValue))
            });
            return statements;
        }

        public ErrorData Update(
            Context context,
            SiteSettings ss,
            SqlParamCollection param = null,
            List<SqlStatement> additionalStatements = null,
            bool otherInitValue = false,
            bool setBySession = true,
            bool get = true,
            bool checkConflict = true)
        {
            if (setBySession)
            {
                SetBySession(context: context);
            }
            var statements = new List<SqlStatement>();
            var verUp = Versions.VerUp(
                context: context,
                ss: ss,
                verUp: VerUp);
            statements.AddRange(UpdateStatements(
                context: context,
                ss: ss,
                param: param,
                otherInitValue: otherInitValue,
                additionalStatements: additionalStatements,
                checkConflict: checkConflict,
                verUp: verUp));
            var response = Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            if (response.Event == "Conflicted")
            {
                return new ErrorData(
                    type: Error.Types.UpdateConflicts,
                    id: OutgoingMailId);
            }
            if (get)
            {
                Get(context: context);
            }
            return new ErrorData(type: Error.Types.None);
        }

        public List<SqlStatement> UpdateStatements(
            Context context,
            SiteSettings ss,
            string dataTableName = null,
            SqlParamCollection param = null,
            bool otherInitValue = false,
            List<SqlStatement> additionalStatements = null,
            bool checkConflict = true,
            bool verUp = false)
        {
            var timestamp = Timestamp.ToDateTime();
            var statements = new List<SqlStatement>();
            var where = Rds.OutgoingMailsWhereDefault(
                context: context,
                outgoingMailModel: this)
                    .UpdatedTime(timestamp, _using: timestamp.InRange() && checkConflict);
            if (verUp)
            {
                statements.Add(Rds.OutgoingMailsCopyToStatement(
                    where: where,
                    tableType: Sqls.TableTypes.History,
                    ColumnNames()));
                Ver++;
            }
            statements.AddRange(UpdateStatements(
                context: context,
                ss: ss,
                dataTableName: dataTableName,
                where: where,
                param: param,
                otherInitValue: otherInitValue));
            if (additionalStatements?.Any() == true)
            {
                statements.AddRange(additionalStatements);
            }
            return statements;
        }

        private List<SqlStatement> UpdateStatements(
            Context context,
            SiteSettings ss,
            string dataTableName = null,
            SqlWhereCollection where = null,
            SqlParamCollection param = null,
            bool otherInitValue = false)
        {
            return new List<SqlStatement>
            {
                Rds.UpdateOutgoingMails(
                    dataTableName: dataTableName,
                    where: where,
                    param: param ?? Rds.OutgoingMailsParamDefault(
                        context: context,
                        ss: ss,
                        outgoingMailModel: this,
                        otherInitValue: otherInitValue)),
                new SqlStatement(Def.Sql.IfConflicted.Params(OutgoingMailId))
                {
                    DataTableName = dataTableName,
                    IfConflicted = true,
                    Id = OutgoingMailId
                }
            };
        }

        public ErrorData UpdateOrCreate(
            Context context,
            SiteSettings ss,
            string dataTableName = null,
            SqlWhereCollection where = null,
            SqlParamCollection param = null)
        {
            SetBySession(context: context);
            var statements = new List<SqlStatement>
            {
                Rds.UpdateOrInsertOutgoingMails(
                    where: where ?? Rds.OutgoingMailsWhereDefault(
                        context: context,
                        outgoingMailModel: this),
                    param: param ?? Rds.OutgoingMailsParamDefault(
                        context: context,
                        ss: ss,
                        outgoingMailModel: this,
                        setDefault: true))
            };
            var response = Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: statements.ToArray());
            OutgoingMailId = (response.Id ?? OutgoingMailId).ToLong();
            Get(context: context);
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData Delete(Context context)
        {
            var statements = new List<SqlStatement>();
            var where = Rds.OutgoingMailsWhere().OutgoingMailId(OutgoingMailId);
            statements.AddRange(new List<SqlStatement>
            {
                Rds.DeleteOutgoingMails(
                    factory: context,
                    where: where)
            });
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData Restore(Context context, long outgoingMailId)
        {
            OutgoingMailId = outgoingMailId;
            Repository.ExecuteNonQuery(
                context: context,
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreOutgoingMails(
                        factory: context,
                        where: Rds.OutgoingMailsWhere().OutgoingMailId(OutgoingMailId))
                });
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData PhysicalDelete(
            Context context, Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: Rds.PhysicalDeleteOutgoingMails(
                    tableType: tableType,
                    where: Rds.OutgoingMailsWhere().OutgoingMailId(OutgoingMailId)));
            return new ErrorData(type: Error.Types.None);
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
            ClassHash = outgoingMailModel.ClassHash;
            NumHash = outgoingMailModel.NumHash;
            DateHash = outgoingMailModel.DateHash;
            DescriptionHash = outgoingMailModel.DescriptionHash;
            CheckHash = outgoingMailModel.CheckHash;
            AttachmentsHash = outgoingMailModel.AttachmentsHash;
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
            foreach (DataColumn dataColumn in dataRow.Table.Columns)
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
                            From = MimeKit.MailboxAddress.Parse(dataRow[column.ColumnName].ToString());
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
                        case "IsHistory":
                            VerType = dataRow.Bool(column.ColumnName)
                                ? Versions.VerTypes.History
                                : Versions.VerTypes.Latest; break;
                        default:
                            switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                            {
                                case "Class":
                                    SetClass(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToString());
                                    SetSavedClass(
                                        columnName: column.Name,
                                        value: GetClass(columnName: column.Name));
                                    break;
                                case "Num":
                                    SetNum(
                                        columnName: column.Name,
                                        value: new Num(
                                            dataRow: dataRow,
                                            name: column.ColumnName));
                                    SetSavedNum(
                                        columnName: column.Name,
                                        value: GetNum(columnName: column.Name).Value);
                                    break;
                                case "Date":
                                    SetDate(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToDateTime());
                                    SetSavedDate(
                                        columnName: column.Name,
                                        value: GetDate(columnName: column.Name));
                                    break;
                                case "Description":
                                    SetDescription(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToString());
                                    SetSavedDescription(
                                        columnName: column.Name,
                                        value: GetDescription(columnName: column.Name));
                                    break;
                                case "Check":
                                    SetCheck(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToBool());
                                    SetSavedCheck(
                                        columnName: column.Name,
                                        value: GetCheck(columnName: column.Name));
                                    break;
                                case "Attachments":
                                    SetAttachments(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToString()
                                            .Deserialize<Attachments>() ?? new Attachments());
                                    SetSavedAttachments(
                                        columnName: column.Name,
                                        value: GetAttachments(columnName: column.Name).ToJson());
                                    break;
                            }
                            break;
                    }
                }
            }
        }

        public bool Updated(Context context)
        {
            return Updated()
                || ReferenceType_Updated(context: context)
                || ReferenceId_Updated(context: context)
                || ReferenceVer_Updated(context: context)
                || OutgoingMailId_Updated(context: context)
                || Ver_Updated(context: context)
                || Host_Updated(context: context)
                || Port_Updated(context: context)
                || From_Updated(context: context)
                || To_Updated(context: context)
                || Cc_Updated(context: context)
                || Bcc_Updated(context: context)
                || Title_Updated(context: context)
                || Body_Updated(context: context)
                || SentTime_Updated(context: context)
                || Comments_Updated(context: context)
                || Creator_Updated(context: context)
                || Updator_Updated(context: context);
        }

        private bool UpdatedWithColumn(Context context, SiteSettings ss)
        {
            return ClassHash.Any(o => Class_Updated(
                    columnName: o.Key,
                    column: ss.GetColumn(context: context, o.Key)))
                || NumHash.Any(o => Num_Updated(
                    columnName: o.Key,
                    column: ss.GetColumn(context: context, o.Key)))
                || DateHash.Any(o => Date_Updated(
                    columnName: o.Key,
                    column: ss.GetColumn(context: context, o.Key)))
                || DescriptionHash.Any(o => Description_Updated(
                    columnName: o.Key,
                    column: ss.GetColumn(context: context, o.Key)))
                || CheckHash.Any(o => Check_Updated(
                    columnName: o.Key,
                    column: ss.GetColumn(context: context, o.Key)))
                || AttachmentsHash.Any(o => Attachments_Updated(
                    columnName: o.Key,
                    column: ss.GetColumn(context: context, o.Key)));
        }

        public bool Updated(Context context, SiteSettings ss)
        {
            return UpdatedWithColumn(context: context, ss: ss)
                || ReferenceType_Updated(context: context)
                || ReferenceId_Updated(context: context)
                || ReferenceVer_Updated(context: context)
                || OutgoingMailId_Updated(context: context)
                || Ver_Updated(context: context)
                || Host_Updated(context: context)
                || Port_Updated(context: context)
                || From_Updated(context: context)
                || To_Updated(context: context)
                || Cc_Updated(context: context)
                || Bcc_Updated(context: context)
                || Title_Updated(context: context)
                || Body_Updated(context: context)
                || SentTime_Updated(context: context)
                || Comments_Updated(context: context)
                || Creator_Updated(context: context)
                || Updator_Updated(context: context);
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
            ReferenceVer = context.Forms.Int("Ver");
            From = OutgoingMailUtilities.From(
                context: context,
                userId: context.UserId);
            SetByForm(context: context);
            if (Libraries.Mails.Addresses.FixedFrom(From))
            {
                Body += "\n\n{0}<{1}>".Params(From.Name, From.Address);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public void SetByForm(Context context)
        {
            var ss = SiteSettingsUtilities.OutgoingMailsSiteSettings(context: context);
            context.Forms.Keys.ForEach(controlId =>
            {
                switch (controlId)
                {
                    case "OutgoingMails_To": To = context.Forms.List(controlId).Join(";"); break;
                    case "OutgoingMails_Cc": Cc = context.Forms.List(controlId).Join(";"); break;
                    case "OutgoingMails_Bcc": Bcc = context.Forms.List(controlId).Join(";"); break;
                    case "OutgoingMails_Title": Title = new Title(OutgoingMailId, context.Forms.Data(controlId)); break;
                    case "OutgoingMails_Body": Body = context.Forms.Data(controlId).ToString(); break;
                    case "OutgoingMails_SentTime": SentTime = new Time(context, context.Forms.Data(controlId).ToDateTime(), byForm: true); break;
                    case "OutgoingMails_DestinationSearchRange": DestinationSearchRange = context.Forms.Data(controlId).ToString(); break;
                    case "OutgoingMails_DestinationSearchText": DestinationSearchText = context.Forms.Data(controlId).ToString(); break;
                    case "OutgoingMails_Timestamp": Timestamp = context.Forms.Data(controlId).ToString(); break;
                    case "Comments":
                        Comments.Prepend(
                            context: context,
                            ss: new SiteSettings(),
                            body: context.Forms.Data("Comments"));
                        break;
                    case "VerUp": VerUp = context.Forms.Data(controlId).ToBool(); break;
                    default:
                        if (controlId.RegexExists("Comment[0-9]+"))
                        {
                            Comments.Update(
                                context: context,
                                ss: ss,
                                commentId: controlId.Substring("Comment".Length).ToInt(),
                                body: context.Forms.Data(controlId));
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
            return new OutgoingMailsResponseCollection(
                context: context,
                outgoingMailModel: this)
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
        public ErrorData Send(
            Context context,
            SiteSettings ss,
            List<SqlStatement> additionalStatements = null,
            Attachments attachments = null)
        {
            var errorData = Create(context: context, ss: ss);
            if (errorData.Type.Has()) return errorData;
            Host = Parameters.Mail.SmtpHost;
            Port = Parameters.Mail.SmtpPort;
            switch (Host)
            {
                case "smtp.sendgrid.net":
                    SendBySendGrid(
                        context: context,
                        attachments: attachments);
                    break;
                default:
                    SendBySmtp(
                        context: context,
                        attachments: attachments);
                    break;
            }
            SentTime = new Time(context, DateTime.Now);
            errorData = Update(
                context: context,
                ss: ss,
                additionalStatements: additionalStatements);
            return errorData.Type.Has()
                ? errorData
                : new ErrorData(type: Error.Types.None);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SendBySmtp(Context context, Attachments attachments = null)
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
                    .Send(
                        context: context,
                        attachments: attachments);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SendBySendGrid(Context context, Attachments attachments = null)
        {
            _ = new SendGridMail(
                context: context,
                host: Host,
                from: From,
                to: To,
                cc: Cc,
                bcc: Bcc,
                subject: Title.Value,
                body: Body)
                    .SendAsync(
                        context: context,
                        attachments: attachments);
        }
    }
}
