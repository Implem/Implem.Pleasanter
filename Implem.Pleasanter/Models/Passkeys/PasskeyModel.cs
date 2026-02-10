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
    public class PasskeyModel : BaseModel
    {
        public int PasskeyId = 0;
        public string CredentialId = string.Empty;
        public string Title = string.Empty;
        public int UserId = 0;
        public PasskeyData PasskeyData = new PasskeyData();
        public int SavedPasskeyId = 0;
        public string SavedCredentialId = string.Empty;
        public string SavedTitle = string.Empty;
        public int SavedUserId = 0;
        public string SavedPasskeyData = "{}";

        public bool PasskeyId_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToInt() != PasskeyId;
            }
            return PasskeyId != SavedPasskeyId
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToInt() != PasskeyId);
        }

        public bool CredentialId_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != CredentialId;
            }
            return CredentialId != SavedCredentialId && CredentialId != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != CredentialId);
        }

        public bool Title_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != Title;
            }
            return Title != SavedTitle && Title != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != Title);
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

        public bool PasskeyData_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != PasskeyData.ToJson();
            }
            return PasskeyData.ToJson() != SavedPasskeyData && PasskeyData.ToJson() != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != PasskeyData.ToJson());
        }

        public PasskeyModel()
        {
        }

        public PasskeyModel(
            Context context,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public PasskeyModel(
            Context context,
            int passkeyId,
            SqlColumnCollection column = null,
            bool clearSessions = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            PasskeyId = passkeyId;
            if (context.QueryStrings.ContainsKey("ver"))
            {
                Get(
                    context: context,
                    tableType: Sqls.TableTypes.NormalAndHistory,
                    column: column,
                    where: Rds.PasskeysWhereDefault(
                        context: context,
                        passkeyModel: this)
                            .Passkeys_Ver(context.QueryStrings.Int("ver")));
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

        public PasskeyModel(
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

        public PasskeyModel Get(
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
            where = where ?? Rds.PasskeysWhereDefault(
                context: context,
                passkeyModel: this);
            column = (column ?? Rds.PasskeysDefaultColumns());
            join = join ?? Rds.PasskeysJoinDefault();
            Set(context, Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectPasskeys(
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
            PasskeyId = (response.Id ?? PasskeyId).ToInt();
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
                Rds.InsertPasskeys(
                    dataTableName: dataTableName,
                    tableType: tableType,
                    selectIdentity: true,
                    param: param ?? Rds.PasskeysParamDefault(
                        context: context,
                        ss: ss,
                        passkeyModel: this,
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
                    id: PasskeyId);
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
            var where = Rds.PasskeysWhereDefault(
                context: context,
                passkeyModel: this)
                    .UpdatedTime(timestamp, _using: timestamp.InRange() && checkConflict);
            if (verUp)
            {
                statements.Add(Rds.PasskeysCopyToStatement(
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
                Rds.UpdatePasskeys(
                    dataTableName: dataTableName,
                    where: where,
                    param: param ?? Rds.PasskeysParamDefault(
                        context: context,
                        ss: ss,
                        passkeyModel: this,
                        otherInitValue: otherInitValue)),
                new SqlStatement()
                {
                    DataTableName = dataTableName,
                    IfConflicted = true,
                    Id = PasskeyId
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
                Rds.UpdateOrInsertPasskeys(
                    where: where ?? Rds.PasskeysWhereDefault(
                        context: context,
                        passkeyModel: this),
                    param: param ?? Rds.PasskeysParamDefault(
                        context: context,
                        ss: ss,
                        passkeyModel: this,
                        setDefault: true))
            };
            var response = Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: statements.ToArray());
            PasskeyId = (response.Id ?? PasskeyId).ToInt();
            Get(context: context);
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData Delete(Context context)
        {
            var statements = new List<SqlStatement>();
            var where = Rds.PasskeysWhere().PasskeyId(PasskeyId);
            statements.AddRange(new List<SqlStatement>
            {
                Rds.DeletePasskeys(
                    factory: context,
                    where: where)
            });
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData Restore(Context context, int passkeyId)
        {
            PasskeyId = passkeyId;
            Repository.ExecuteNonQuery(
                context: context,
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestorePasskeys(
                        factory: context,
                        where: Rds.PasskeysWhere().PasskeyId(PasskeyId))
                });
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData PhysicalDelete(
            Context context, Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: Rds.PhysicalDeletePasskeys(
                    tableType: tableType,
                    where: Rds.PasskeysWhere().PasskeyId(PasskeyId)));
            return new ErrorData(type: Error.Types.None);
        }

        public void SetByModel(PasskeyModel passkeyModel)
        {
            CredentialId = passkeyModel.CredentialId;
            Title = passkeyModel.Title;
            UserId = passkeyModel.UserId;
            PasskeyData = passkeyModel.PasskeyData;
            Comments = passkeyModel.Comments;
            Creator = passkeyModel.Creator;
            Updator = passkeyModel.Updator;
            CreatedTime = passkeyModel.CreatedTime;
            UpdatedTime = passkeyModel.UpdatedTime;
            VerUp = passkeyModel.VerUp;
            Comments = passkeyModel.Comments;
            ClassHash = passkeyModel.ClassHash;
            NumHash = passkeyModel.NumHash;
            DateHash = passkeyModel.DateHash;
            DescriptionHash = passkeyModel.DescriptionHash;
            CheckHash = passkeyModel.CheckHash;
            AttachmentsHash = passkeyModel.AttachmentsHash;
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
                        case "PasskeyId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                PasskeyId = dataRow[column.ColumnName].ToInt();
                                SavedPasskeyId = PasskeyId;
                            }
                            break;
                        case "Ver":
                            Ver = dataRow[column.ColumnName].ToInt();
                            SavedVer = Ver;
                            break;
                        case "CredentialId":
                            CredentialId = dataRow[column.ColumnName].ToString();
                            SavedCredentialId = CredentialId;
                            break;
                        case "Title":
                            Title = dataRow[column.ColumnName].ToString();
                            SavedTitle = Title;
                            break;
                        case "UserId":
                            UserId = dataRow[column.ColumnName].ToInt();
                            SavedUserId = UserId;
                            break;
                        case "PasskeyData":
                            PasskeyData = dataRow[column.ColumnName].ToString().Deserialize<PasskeyData>() ?? new PasskeyData();
                            SavedPasskeyData = PasskeyData.ToJson();
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
                || PasskeyId_Updated(context: context)
                || Ver_Updated(context: context)
                || CredentialId_Updated(context: context)
                || Title_Updated(context: context)
                || UserId_Updated(context: context)
                || PasskeyData_Updated(context: context)
                || Comments_Updated(context: context)
                || Creator_Updated(context: context)
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
                || PasskeyId_Updated(context: context)
                || Ver_Updated(context: context)
                || CredentialId_Updated(context: context)
                || Title_Updated(context: context)
                || UserId_Updated(context: context)
                || PasskeyData_Updated(context: context)
                || Comments_Updated(context: context)
                || Creator_Updated(context: context)
                || Updator_Updated(context: context);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public PasskeyModel(Context context, int userId)
        {
            OnConstructing(context: context);
            UserId = userId;
            OnConstructed(context: context);
        }
    }
}
