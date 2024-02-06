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

        public bool ExportSettingId_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToLong() != ExportSettingId;
            }
            return ExportSettingId != SavedExportSettingId
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToLong() != ExportSettingId);
        }

        public bool AddHeader_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToBool() != AddHeader;
            }
            return AddHeader != SavedAddHeader
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToBool() != AddHeader);
        }

        public bool ExportColumns_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != ExportColumns.ToJson();
            }
            return ExportColumns.ToJson() != SavedExportColumns && ExportColumns.ToJson() != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != ExportColumns.ToJson());
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
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public ExportSettingModel(
            Context context,
            long exportSettingId,
            SqlColumnCollection column = null,
            bool clearSessions = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            ExportSettingId = exportSettingId;
            if (context.QueryStrings.ContainsKey("ver"))
            {
                Get(
                    context: context,
                    tableType: Sqls.TableTypes.NormalAndHistory,
                    column: column,
                    where: Rds.ExportSettingsWhereDefault(
                        context: context,
                        exportSettingModel: this)
                            .ExportSettings_Ver(context.QueryStrings.Int("ver")));
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

        public ExportSettingModel(
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
            where = where ?? Rds.ExportSettingsWhereDefault(
                context: context,
                exportSettingModel: this);
            column = (column ?? Rds.ExportSettingsDefaultColumns());
            join = join ?? Rds.ExportSettingsJoinDefault();
            Set(context, Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectExportSettings(
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
            ExportSettingId = (response.Id ?? ExportSettingId).ToLong();
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
                Rds.InsertExportSettings(
                    dataTableName: dataTableName,
                    tableType: tableType,
                    selectIdentity: true,
                    param: param ?? Rds.ExportSettingsParamDefault(
                        context: context,
                        ss: ss,
                        exportSettingModel: this,
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
            List<SqlStatement> additionalStatements = null,
            bool checkConflict = true,
            bool verUp = false)
        {
            var timestamp = Timestamp.ToDateTime();
            var statements = new List<SqlStatement>();
            var where = Rds.ExportSettingsWhereDefault(
                context: context,
                exportSettingModel: this)
                    .UpdatedTime(timestamp, _using: timestamp.InRange() && checkConflict);
            if (verUp)
            {
                statements.Add(Rds.ExportSettingsCopyToStatement(
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
                Rds.UpdateExportSettings(
                    dataTableName: dataTableName,
                    where: where,
                    param: param ?? Rds.ExportSettingsParamDefault(
                        context: context,
                        ss: ss,
                        exportSettingModel: this,
                        otherInitValue: otherInitValue)),
                new SqlStatement(Def.Sql.IfConflicted.Params(ExportSettingId))
                {
                    DataTableName = dataTableName,
                    IfConflicted = true,
                    Id = ExportSettingId
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
                Rds.UpdateOrInsertExportSettings(
                    where: where ?? Rds.ExportSettingsWhereDefault(
                        context: context,
                        exportSettingModel: this),
                    param: param ?? Rds.ExportSettingsParamDefault(
                        context: context,
                        ss: ss,
                        exportSettingModel: this,
                        setDefault: true))
            };
            var response = Repository.ExecuteScalar_response(
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
                Rds.DeleteExportSettings(
                    factory: context,
                    where: where)
            });
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData Restore(Context context, long exportSettingId)
        {
            ExportSettingId = exportSettingId;
            Repository.ExecuteNonQuery(
                context: context,
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreExportSettings(
                        factory: context,
                        where: Rds.ExportSettingsWhere().ExportSettingId(ExportSettingId))
                });
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData PhysicalDelete(
            Context context, Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Repository.ExecuteNonQuery(
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
                || Title_Updated(context: context)
                || ExportSettingId_Updated(context: context)
                || Ver_Updated(context: context)
                || AddHeader_Updated(context: context)
                || ExportColumns_Updated(context: context)
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
