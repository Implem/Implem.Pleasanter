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
    public class BackgroundJobModel : BaseModel
    {
        public long BackgroundJobId = 0;
        public int TenantId = 0;
        public long SiteId = 0;
        public int UserId = 0;
        public string JobType = string.Empty;
        public int Status = 0;
        public int Priority = 100;
        public DateTime JobEnqueuedTime = 0.ToDateTime();
        public DateTime JobStartedTime = 0.ToDateTime();
        public DateTime JobFinishedTime = 0.ToDateTime();
        public string ResultMessage = string.Empty;
        public string File = string.Empty;
        public string JobParameters = string.Empty;
        public long SavedBackgroundJobId = 0;
        public int SavedTenantId = 0;
        public long SavedSiteId = 0;
        public int SavedUserId = 0;
        public string SavedJobType = string.Empty;
        public int SavedStatus = 0;
        public int SavedPriority = 100;
        public DateTime SavedJobEnqueuedTime = 0.ToDateTime();
        public DateTime SavedJobStartedTime = 0.ToDateTime();
        public DateTime SavedJobFinishedTime = 0.ToDateTime();
        public string SavedResultMessage = string.Empty;
        public string SavedFile = string.Empty;
        public string SavedJobParameters = string.Empty;

        public bool BackgroundJobId_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToLong() != BackgroundJobId;
            }
            return BackgroundJobId != SavedBackgroundJobId
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToLong() != BackgroundJobId);
        }

        public bool TenantId_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToInt() != TenantId;
            }
            return TenantId != SavedTenantId
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToInt() != TenantId);
        }

        public bool SiteId_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToLong() != SiteId;
            }
            return SiteId != SavedSiteId
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToLong() != SiteId);
        }

        public bool UserId_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToInt() != UserId;
            }
            return UserId != SavedUserId
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToInt() != UserId);
        }

        public bool JobType_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != JobType;
            }
            return JobType != SavedJobType && JobType != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != JobType);
        }

        public bool Status_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToInt() != Status;
            }
            return Status != SavedStatus
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToInt() != Status);
        }

        public bool Priority_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToInt() != Priority;
            }
            return Priority != SavedPriority
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToInt() != Priority);
        }

        public bool ResultMessage_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != ResultMessage;
            }
            return ResultMessage != SavedResultMessage && ResultMessage != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != ResultMessage);
        }

        public bool File_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != File;
            }
            return File != SavedFile && File != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != File);
        }

        public bool JobParameters_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != JobParameters;
            }
            return JobParameters != SavedJobParameters && JobParameters != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != JobParameters);
        }

        public bool JobEnqueuedTime_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToDateTime() != JobEnqueuedTime;
            }
            return JobEnqueuedTime != SavedJobEnqueuedTime
                && (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.DefaultTime(context: context).Date != JobEnqueuedTime.Date);
        }

        public bool JobStartedTime_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToDateTime() != JobStartedTime;
            }
            return JobStartedTime != SavedJobStartedTime
                && (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.DefaultTime(context: context).Date != JobStartedTime.Date);
        }

        public bool JobFinishedTime_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToDateTime() != JobFinishedTime;
            }
            return JobFinishedTime != SavedJobFinishedTime
                && (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.DefaultTime(context: context).Date != JobFinishedTime.Date);
        }

        public BackgroundJobModel()
        {
        }

        public BackgroundJobModel(
            Context context,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            TenantId = context.TenantId;
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public BackgroundJobModel(
            Context context,
            long backgroundJobId,
            SqlColumnCollection column = null,
            bool clearSessions = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            TenantId = context.TenantId;
            BackgroundJobId = backgroundJobId;
            if (context.QueryStrings.ContainsKey("ver"))
            {
                Get(
                    context: context,
                    tableType: Sqls.TableTypes.NormalAndHistory,
                    column: column,
                    where: Rds.BackgroundJobsWhereDefault(
                        context: context,
                        backgroundJobModel: this)
                            .BackgroundJobs_Ver(context.QueryStrings.Int("ver")));
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

        public BackgroundJobModel(
            Context context,
            DataRow dataRow,
            string tableAlias = null)
        {
            OnConstructing(context: context);
            TenantId = context.TenantId;
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

        public BackgroundJobModel Get(
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
            where = where ?? Rds.BackgroundJobsWhereDefault(
                context: context,
                backgroundJobModel: this);
            column = (column ?? Rds.BackgroundJobsDefaultColumns());
            join = join ?? Rds.BackgroundJobsJoinDefault();
            Set(context, Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectBackgroundJobs(
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
            TenantId = context.TenantId;
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
            BackgroundJobId = (response.Id ?? BackgroundJobId).ToLong();
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
                Rds.InsertBackgroundJobs(
                    dataTableName: dataTableName,
                    tableType: tableType,
                    selectIdentity: true,
                    param: param ?? Rds.BackgroundJobsParamDefault(
                        context: context,
                        ss: ss,
                        backgroundJobModel: this,
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
                    id: BackgroundJobId);
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
            var where = Rds.BackgroundJobsWhereDefault(
                context: context,
                backgroundJobModel: this)
                    .UpdatedTime(timestamp, _using: timestamp.InRange() && checkConflict);
            if (verUp)
            {
                statements.Add(Rds.BackgroundJobsCopyToStatement(
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
                Rds.UpdateBackgroundJobs(
                    dataTableName: dataTableName,
                    where: where,
                    param: param ?? Rds.BackgroundJobsParamDefault(
                        context: context,
                        ss: ss,
                        backgroundJobModel: this,
                        otherInitValue: otherInitValue)),
                new SqlStatement()
                {
                    DataTableName = dataTableName,
                    IfConflicted = true,
                    Id = BackgroundJobId
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
                Rds.UpdateOrInsertBackgroundJobs(
                    where: where ?? Rds.BackgroundJobsWhereDefault(
                        context: context,
                        backgroundJobModel: this),
                    param: param ?? Rds.BackgroundJobsParamDefault(
                        context: context,
                        ss: ss,
                        backgroundJobModel: this,
                        setDefault: true))
            };
            var response = Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: statements.ToArray());
            BackgroundJobId = (response.Id ?? BackgroundJobId).ToLong();
            Get(context: context);
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData Delete(Context context)
        {
            var statements = new List<SqlStatement>();
            var where = Rds.BackgroundJobsWhere().BackgroundJobId(BackgroundJobId);
            statements.AddRange(new List<SqlStatement>
            {
                Rds.DeleteBackgroundJobs(
                    factory: context,
                    where: where)
            });
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData Restore(Context context, long backgroundJobId)
        {
            BackgroundJobId = backgroundJobId;
            Repository.ExecuteNonQuery(
                context: context,
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreBackgroundJobs(
                        factory: context,
                        where: Rds.BackgroundJobsWhere().BackgroundJobId(BackgroundJobId))
                });
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData PhysicalDelete(
            Context context, Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: Rds.PhysicalDeleteBackgroundJobs(
                    tableType: tableType,
                    where: Rds.BackgroundJobsWhere().BackgroundJobId(BackgroundJobId)));
            return new ErrorData(type: Error.Types.None);
        }

        public void SetByModel(BackgroundJobModel backgroundJobModel)
        {
            TenantId = backgroundJobModel.TenantId;
            SiteId = backgroundJobModel.SiteId;
            UserId = backgroundJobModel.UserId;
            JobType = backgroundJobModel.JobType;
            Status = backgroundJobModel.Status;
            Priority = backgroundJobModel.Priority;
            JobEnqueuedTime = backgroundJobModel.JobEnqueuedTime;
            JobStartedTime = backgroundJobModel.JobStartedTime;
            JobFinishedTime = backgroundJobModel.JobFinishedTime;
            ResultMessage = backgroundJobModel.ResultMessage;
            File = backgroundJobModel.File;
            JobParameters = backgroundJobModel.JobParameters;
            Creator = backgroundJobModel.Creator;
            Comments = backgroundJobModel.Comments;
            Updator = backgroundJobModel.Updator;
            CreatedTime = backgroundJobModel.CreatedTime;
            UpdatedTime = backgroundJobModel.UpdatedTime;
            VerUp = backgroundJobModel.VerUp;
            Comments = backgroundJobModel.Comments;
            ClassHash = backgroundJobModel.ClassHash;
            NumHash = backgroundJobModel.NumHash;
            DateHash = backgroundJobModel.DateHash;
            DescriptionHash = backgroundJobModel.DescriptionHash;
            CheckHash = backgroundJobModel.CheckHash;
            AttachmentsHash = backgroundJobModel.AttachmentsHash;
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
                        case "BackgroundJobId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                BackgroundJobId = dataRow[column.ColumnName].ToLong();
                                SavedBackgroundJobId = BackgroundJobId;
                            }
                            break;
                        case "Ver":
                            Ver = dataRow[column.ColumnName].ToInt();
                            SavedVer = Ver;
                            break;
                        case "TenantId":
                            TenantId = dataRow[column.ColumnName].ToInt();
                            SavedTenantId = TenantId;
                            break;
                        case "SiteId":
                            SiteId = dataRow[column.ColumnName].ToLong();
                            SavedSiteId = SiteId;
                            break;
                        case "UserId":
                            UserId = dataRow[column.ColumnName].ToInt();
                            SavedUserId = UserId;
                            break;
                        case "JobType":
                            JobType = dataRow[column.ColumnName].ToString();
                            SavedJobType = JobType;
                            break;
                        case "Status":
                            Status = dataRow[column.ColumnName].ToInt();
                            SavedStatus = Status;
                            break;
                        case "Priority":
                            Priority = dataRow[column.ColumnName].ToInt();
                            SavedPriority = Priority;
                            break;
                        case "JobEnqueuedTime":
                            JobEnqueuedTime = dataRow[column.ColumnName].ToDateTime();
                            SavedJobEnqueuedTime = JobEnqueuedTime;
                            break;
                        case "JobStartedTime":
                            JobStartedTime = dataRow[column.ColumnName].ToDateTime();
                            SavedJobStartedTime = JobStartedTime;
                            break;
                        case "JobFinishedTime":
                            JobFinishedTime = dataRow[column.ColumnName].ToDateTime();
                            SavedJobFinishedTime = JobFinishedTime;
                            break;
                        case "ResultMessage":
                            ResultMessage = dataRow[column.ColumnName].ToString();
                            SavedResultMessage = ResultMessage;
                            break;
                        case "File":
                            File = dataRow[column.ColumnName].ToString();
                            SavedFile = File;
                            break;
                        case "JobParameters":
                            JobParameters = dataRow[column.ColumnName].ToString();
                            SavedJobParameters = JobParameters;
                            break;
                        case "Creator":
                            Creator = (dataRow.Int("TenantId") == context.TenantId ? SiteInfo.User(context: context, userId: dataRow.Int(column.ColumnName)) : new User(context: context, userId: dataRow.Int(column.ColumnName)));
                            SavedCreator = Creator.Id;
                            break;
                        case "Comments":
                            Comments = dataRow[column.ColumnName].ToString().Deserialize<Comments>() ?? new Comments();
                            SavedComments = Comments.ToJson();
                            break;
                        case "Updator":
                            Updator = (dataRow.Int("TenantId") == context.TenantId ? SiteInfo.User(context: context, userId: dataRow.Int(column.ColumnName)) : new User(context: context, userId: dataRow.Int(column.ColumnName)));
                            SavedUpdator = Updator.Id;
                            break;
                        case "CreatedTime":
                            CreatedTime = new Time(context, dataRow, column.ColumnName);
                            SavedCreatedTime = CreatedTime.Value;
                            break;
                        case "UpdatedTime":
                            UpdatedTime = new Time(context, dataRow, column.ColumnName);
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
                || BackgroundJobId_Updated(context: context)
                || Ver_Updated(context: context)
                || TenantId_Updated(context: context)
                || SiteId_Updated(context: context)
                || UserId_Updated(context: context)
                || JobType_Updated(context: context)
                || Status_Updated(context: context)
                || Priority_Updated(context: context)
                || JobEnqueuedTime_Updated(context: context)
                || JobStartedTime_Updated(context: context)
                || JobFinishedTime_Updated(context: context)
                || ResultMessage_Updated(context: context)
                || File_Updated(context: context)
                || JobParameters_Updated(context: context)
                || Creator_Updated(context: context)
                || Comments_Updated(context: context)
                || Updator_Updated(context: context);
        }

        private bool UpdatedWithColumn(Context context, SiteSettings ss, bool paramDefault = false)
        {
            return ClassHash.Any(o => Class_Updated(
                    columnName: o.Key,
                    context: context,
                    column: ss.GetColumn(context: context, o.Key),
                    paramDefault: paramDefault))
                || NumHash.Any(o => Num_Updated(
                    columnName: o.Key,
                    context: context,
                    column: ss.GetColumn(context: context, o.Key),
                    paramDefault: paramDefault))
                || DateHash.Any(o => Date_Updated(
                    columnName: o.Key,
                    context: context,
                    column: ss.GetColumn(context: context, o.Key),
                    paramDefault: paramDefault))
                || DescriptionHash.Any(o => Description_Updated(
                    columnName: o.Key,
                    context: context,
                    column: ss.GetColumn(context: context, o.Key),
                    paramDefault: paramDefault))
                || CheckHash.Any(o => Check_Updated(
                    columnName: o.Key,
                    context: context,
                    column: ss.GetColumn(context: context, o.Key),
                    paramDefault: paramDefault))
                || AttachmentsHash.Any(o => Attachments_Updated(
                    columnName: o.Key,
                    context: context,
                    column: ss.GetColumn(context: context, o.Key)));
        }

        public bool Updated(Context context, SiteSettings ss, bool paramDefault = false)
        {
            return UpdatedWithColumn(context: context, ss: ss, paramDefault: paramDefault)
                || BackgroundJobId_Updated(context: context)
                || Ver_Updated(context: context)
                || TenantId_Updated(context: context)
                || SiteId_Updated(context: context)
                || UserId_Updated(context: context)
                || JobType_Updated(context: context)
                || Status_Updated(context: context)
                || Priority_Updated(context: context)
                || JobEnqueuedTime_Updated(context: context)
                || JobStartedTime_Updated(context: context)
                || JobFinishedTime_Updated(context: context)
                || ResultMessage_Updated(context: context)
                || File_Updated(context: context)
                || JobParameters_Updated(context: context)
                || Creator_Updated(context: context)
                || Comments_Updated(context: context)
                || Updator_Updated(context: context);
        }
    }
}
