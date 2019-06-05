﻿using Implem.DefinitionAccessor;
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
        public string SavedExportColumns = "{}";

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

        public bool Title_Updated(Context context, Column column = null)
        {
            return Title.Value != SavedTitle && Title.Value != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != Title.Value);
        }

        public bool ExportSettingId_Updated(Context context, Column column = null)
        {
            return ExportSettingId != SavedExportSettingId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToLong() != ExportSettingId);
        }

        public bool AddHeader_Updated(Context context, Column column = null)
        {
            return AddHeader != SavedAddHeader &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToBool() != AddHeader);
        }

        public bool ExportColumns_Updated(Context context, Column column = null)
        {
            return ExportColumns.ToJson() != SavedExportColumns && ExportColumns.ToJson() != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != ExportColumns.ToJson());
        }

        public Title Session_Title(Context context)
        {
            return context.SessionData.Get("Title") != null
                ? context.SessionData.Get("Title").Deserialize<Title>() ?? new Title()
                : Title;
        }

        public void Session_Title(Context context, string value)
        {
            SessionUtilities.Set(
                context: context,
                key: "Title",
                value: value,
                page: true);
        }

        public bool Session_AddHeader(Context context)
        {
            return context.SessionData.Get("AddHeader") != null
                ? context.SessionData.Get("AddHeader").ToBool()
                : AddHeader;
        }

        public void Session_AddHeader(Context context, string value)
        {
            SessionUtilities.Set(
                context: context,
                key: "AddHeader",
                value: value,
                page: true);
        }

        public ExportColumns Session_ExportColumns(Context context)
        {
            return context.SessionData.Get("ExportColumns") != null
                ? context.SessionData.Get("ExportColumns")?.ToString().Deserialize<ExportColumns>() ?? new ExportColumns(ReferenceType)
                : ExportColumns;
        }

        public void Session_ExportColumns(Context context, string value)
        {
            SessionUtilities.Set(
                context: context,
                key: "ExportColumns",
                value: value,
                page: true);
        }

        public ExportSettingModel()
        {
        }

        public ExportSettingModel(
            Context context,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            Context = context;
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public ExportSettingModel(
            Context context,
            long exportSettingId,
            bool clearSessions = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            Context = context;
            ExportSettingId = exportSettingId;
            if (context.QueryStrings.ContainsKey("ver"))
            {
                Get(context: context,
                    tableType: Sqls.TableTypes.NormalAndHistory,
                    where: Rds.ExportSettingsWhereDefault(this)
                        .ExportSettings_Ver(context.QueryStrings.Int("ver")));
            }
            else
            {
                Get(context: context);
            }
            if (clearSessions) ClearSessions(context: context);
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public ExportSettingModel(
            Context context,
            DataRow dataRow,
            string tableAlias = null)
        {
            OnConstructing(context: context);
            Context = context;
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
            Session_Title(context: context, value: null);
            Session_AddHeader(context: context, value: null);
            Session_ExportColumns(context: context, value: null);
        }

        public ExportSettingModel Get(
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
                statements: Rds.SelectExportSettings(
                    tableType: tableType,
                    column: column ?? Rds.ExportSettingsDefaultColumns(),
                    join: join ??  Rds.ExportSettingsJoinDefault(),
                    where: where ?? Rds.ExportSettingsWhereDefault(this),
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
            bool otherInitValue = false,
            bool get = true)
        {
            var statements = new List<SqlStatement>();
            statements.AddRange(CreateStatements(
                context: context,
                tableType: tableType,
                param: param,
                otherInitValue: otherInitValue));
            var response = Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: statements.ToArray());
            ExportSettingId = (response.Id ?? ExportSettingId).ToLong();
            if (get) Get(context: context);
            return new ErrorData(type: Error.Types.None);
        }

        public List<SqlStatement> CreateStatements(
            Context context,
            string dataTableName = null,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool otherInitValue = false)
        {
            var statements = new List<SqlStatement>();
            statements.AddRange(new List<SqlStatement>
            {
                Rds.InsertExportSettings(
                    dataTableName: dataTableName,
                    tableType: tableType,
                    selectIdentity: true,
                    param: param ?? Rds.ExportSettingsParamDefault(
                        context: context,
                        exportSettingModel: this,
                        setDefault: true,
                        otherInitValue: otherInitValue))
            });
            statements.AddRange(UpdateAttachmentsStatements(context: context));
            return statements;
        }

        public ErrorData Update(
            Context context,
            SiteSettings ss,
            SqlParamCollection param = null,
            List<SqlStatement> additionalStatements = null,
            bool otherInitValue = false,
            bool setBySession = true,
            bool get = true)
        {
            if (setBySession)
            {
                SetBySession(context: context);
            }
            var statements = new List<SqlStatement>();
            statements.AddRange(UpdateStatements(
                context: context,
                ss: ss,
                param: param,
                otherInitValue: otherInitValue,
                additionalStatements: additionalStatements));
            var response = Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            if (response.Event == "Conflicted")
            {
                return new ErrorData(
                    type: Error.Types.UpdateConflicts,
                    id: ExportSettingId);
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
            List<SqlStatement> additionalStatements = null)
        {
            var timestamp = Timestamp.ToDateTime();
            var statements = new List<SqlStatement>();
            var where = Rds.ExportSettingsWhereDefault(this)
                .UpdatedTime(timestamp, _using: timestamp.InRange());
            if (VerUp)
            {
                statements.Add(CopyToStatement(
                    where: where,
                    tableType: Sqls.TableTypes.History));
                Ver++;
            }
            statements.AddRange(UpdateStatements(
                context: context,
                dataTableName: dataTableName,
                where: where,
                param: param,
                otherInitValue: otherInitValue));
            statements.AddRange(UpdateAttachmentsStatements(context: context));
            if (additionalStatements?.Any() == true)
            {
                statements.AddRange(additionalStatements);
            }
            return statements;
        }

        private SqlStatement CopyToStatement(SqlWhereCollection where, Sqls.TableTypes tableType)
        {
            var column = new Rds.ExportSettingsColumnCollection();
            var param = new Rds.ExportSettingsParamCollection();
            column.ReferenceType(function: Sqls.Functions.SingleColumn); param.ReferenceType();
            column.ReferenceId(function: Sqls.Functions.SingleColumn); param.ReferenceId();
            column.Title(function: Sqls.Functions.SingleColumn); param.Title();
            column.ExportSettingId(function: Sqls.Functions.SingleColumn); param.ExportSettingId();
            column.Ver(function: Sqls.Functions.SingleColumn); param.Ver();
            column.AddHeader(function: Sqls.Functions.SingleColumn); param.AddHeader();
            column.ExportColumns(function: Sqls.Functions.SingleColumn); param.ExportColumns();
            column.Comments(function: Sqls.Functions.SingleColumn); param.Comments();
            column.Creator(function: Sqls.Functions.SingleColumn); param.Creator();
            column.Updator(function: Sqls.Functions.SingleColumn); param.Updator();
            column.CreatedTime(function: Sqls.Functions.SingleColumn); param.CreatedTime();
            column.UpdatedTime(function: Sqls.Functions.SingleColumn); param.UpdatedTime();
            ColumnNames().ForEach(columnName =>
            {
                column.Add(
                    columnBracket: $"[{columnName}]",
                    columnName: columnName,
                    function: Sqls.Functions.SingleColumn);
                param.Add(
                    columnBracket: $"[{columnName}]",
                    name: columnName);
            });
            return Rds.InsertExportSettings(
                tableType: tableType,
                param: param,
                select: Rds.SelectExportSettings(column: column, where: where),
                addUpdatorParam: false);
        }

        private List<SqlStatement> UpdateStatements(
            Context context,
            string dataTableName = null,
            SqlWhereCollection where = null,
            SqlParamCollection param = null,
            bool otherInitValue = false)
        {
            return new List<SqlStatement>
            {
                Rds.UpdateExportSettings(
                    dataTableName: dataTableName,
                    where: where,
                    param: param ?? Rds.ExportSettingsParamDefault(
                        context: context,
                        exportSettingModel: this,
                        otherInitValue: otherInitValue)),
                new SqlStatement(Def.Sql.IfConflicted.Params(ExportSettingId))
            };
        }

        private List<SqlStatement> UpdateAttachmentsStatements(Context context)
        {
            var statements = new List<SqlStatement>();
            ColumnNames()
                .Where(columnName => columnName.StartsWith("Attachments"))
                .Where(columnName => Attachments_Updated(columnName: columnName))
                .ForEach(columnName =>
                    Attachments(columnName: columnName).Write(
                        context: context,
                        statements: statements,
                        referenceId: ExportSettingId));
            return statements;
        }

        public ErrorData UpdateOrCreate(
            Context context,
            SqlWhereCollection where = null,
            SqlParamCollection param = null)
        {
            SetBySession(context: context);
            var statements = new List<SqlStatement>
            {
                Rds.UpdateOrInsertExportSettings(
                    where: where ?? Rds.ExportSettingsWhereDefault(this),
                    param: param ?? Rds.ExportSettingsParamDefault(
                        context: context, exportSettingModel: this, setDefault: true))
            };
            var response = Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: statements.ToArray());
            ExportSettingId = (response.Id ?? ExportSettingId).ToLong();
            Get(context: context);
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData Delete(Context context)
        {
            var statements = new List<SqlStatement>();
            var where = Rds.ExportSettingsWhere().ExportSettingId(ExportSettingId);
            statements.AddRange(new List<SqlStatement>
            {
                Rds.DeleteExportSettings(where: where)
            });
            var response = Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData Restore(Context context, long exportSettingId)
        {
            ExportSettingId = exportSettingId;
            Rds.ExecuteNonQuery(
                context: context,
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreExportSettings(
                        where: Rds.ExportSettingsWhere().ExportSettingId(ExportSettingId))
                });
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData PhysicalDelete(
            Context context, Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Rds.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: Rds.PhysicalDeleteExportSettings(
                    tableType: tableType,
                    param: Rds.ExportSettingsParam().ExportSettingId(ExportSettingId)));
            return new ErrorData(type: Error.Types.None);
        }

        public void SetByModel(ExportSettingModel exportSettingModel)
        {
            ReferenceType = exportSettingModel.ReferenceType;
            ReferenceId = exportSettingModel.ReferenceId;
            Title = exportSettingModel.Title;
            AddHeader = exportSettingModel.AddHeader;
            ExportColumns = exportSettingModel.ExportColumns;
            Comments = exportSettingModel.Comments;
            Creator = exportSettingModel.Creator;
            Updator = exportSettingModel.Updator;
            CreatedTime = exportSettingModel.CreatedTime;
            UpdatedTime = exportSettingModel.UpdatedTime;
            VerUp = exportSettingModel.VerUp;
            Comments = exportSettingModel.Comments;
            ClassHash = exportSettingModel.ClassHash;
            NumHash = exportSettingModel.NumHash;
            DateHash = exportSettingModel.DateHash;
            DescriptionHash = exportSettingModel.DescriptionHash;
            CheckHash = exportSettingModel.CheckHash;
            AttachmentsHash = exportSettingModel.AttachmentsHash;
        }

        private void SetBySession(Context context)
        {
            if (!context.Forms.Exists("ExportSettings_Title")) Title = Session_Title(context: context);
            if (!context.Forms.Exists("ExportSettings_AddHeader")) AddHeader = Session_AddHeader(context: context);
            if (!context.Forms.Exists("ExportSettings_ExportColumns")) ExportColumns = Session_ExportColumns(context: context);
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
                        case "Title":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                Title = new Title(dataRow, "ExportSettingId");
                                SavedTitle = Title.Value;
                            }
                            break;
                        case "ExportSettingId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                ExportSettingId = dataRow[column.ColumnName].ToLong();
                                SavedExportSettingId = ExportSettingId;
                            }
                            break;
                        case "Ver":
                            Ver = dataRow[column.ColumnName].ToInt();
                            SavedVer = Ver;
                            break;
                        case "AddHeader":
                            AddHeader = dataRow[column.ColumnName].ToBool();
                            SavedAddHeader = AddHeader;
                            break;
                        case "ExportColumns":
                            ExportColumns = dataRow[column.ColumnName].ToString().Deserialize<ExportColumns>() ?? new ExportColumns(ReferenceType);
                            SavedExportColumns = ExportColumns.ToJson();
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
                            switch (Def.ExtendedColumnTypes.Get(column.Name))
                            {
                                case "Class":
                                    Class(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToString());
                                    SavedClass(
                                        columnName: column.Name,
                                        value: Class(columnName: column.Name));
                                    break;
                                case "Num":
                                    Num(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToDecimal());
                                    SavedNum(
                                        columnName: column.Name,
                                        value: Num(columnName: column.Name));
                                    break;
                                case "Date":
                                    Date(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToDateTime());
                                    SavedDate(
                                        columnName: column.Name,
                                        value: Date(columnName: column.Name));
                                    break;
                                case "Description":
                                    Description(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToString());
                                    SavedDescription(
                                        columnName: column.Name,
                                        value: Description(columnName: column.Name));
                                    break;
                                case "Check":
                                    Check(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToBool());
                                    SavedCheck(
                                        columnName: column.Name,
                                        value: Check(columnName: column.Name));
                                    break;
                                case "Attachments":
                                    Attachments(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToString()
                                            .Deserialize<Attachments>() ?? new Attachments());
                                    SavedAttachments(
                                        columnName: column.Name,
                                        value: Attachments(columnName: column.Name).ToJson());
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
                || Title_Updated(context: context)
                || ExportSettingId_Updated(context: context)
                || Ver_Updated(context: context)
                || AddHeader_Updated(context: context)
                || ExportColumns_Updated(context: context)
                || Comments_Updated(context: context)
                || Creator_Updated(context: context)
                || Updator_Updated(context: context);
        }
    }
}
