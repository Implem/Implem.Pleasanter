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
using System.Data.Common;
using System.Linq;
namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class ExtensionModel : BaseModel
    {
        public int ExtensionId = 0;
        public string ExtensionType = string.Empty;
        public string ExtensionName = string.Empty;
        public string ExtensionSettings = string.Empty;
        public string Body = string.Empty;
        public string Description = string.Empty;
        public bool Disabled = false;
        public int SavedExtensionId = 0;
        public string SavedExtensionType = string.Empty;
        public string SavedExtensionName = string.Empty;
        public string SavedExtensionSettings = string.Empty;
        public string SavedBody = string.Empty;
        public string SavedDescription = string.Empty;
        public bool SavedDisabled = false;

        public bool ExtensionId_Updated(Context context, Column column = null)
        {
            return ExtensionId != SavedExtensionId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != ExtensionId);
        }

        public bool ExtensionType_Updated(Context context, Column column = null)
        {
            return ExtensionType != SavedExtensionType && ExtensionType != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != ExtensionType);
        }

        public bool ExtensionName_Updated(Context context, Column column = null)
        {
            return ExtensionName != SavedExtensionName && ExtensionName != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != ExtensionName);
        }

        public bool ExtensionSettings_Updated(Context context, Column column = null)
        {
            return ExtensionSettings != SavedExtensionSettings && ExtensionSettings != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != ExtensionSettings);
        }

        public bool Body_Updated(Context context, Column column = null)
        {
            return Body != SavedBody && Body != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != Body);
        }

        public bool Description_Updated(Context context, Column column = null)
        {
            return Description != SavedDescription && Description != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != Description);
        }

        public bool Disabled_Updated(Context context, Column column = null)
        {
            return Disabled != SavedDisabled &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToBool() != Disabled);
        }

        public ExtensionModel()
        {
        }

        public ExtensionModel(
            Context context,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            Context = context;
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public ExtensionModel(
            Context context,
            int extensionId,
            bool clearSessions = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            Context = context;
            ExtensionId = extensionId;
            if (context.QueryStrings.ContainsKey("ver"))
            {
                Get(context: context,
                    tableType: Sqls.TableTypes.NormalAndHistory,
                    where: Rds.ExtensionsWhereDefault(this)
                        .Extensions_Ver(context.QueryStrings.Int("ver")));
            }
            else
            {
                Get(context: context);
            }
            if (clearSessions) ClearSessions(context: context);
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public ExtensionModel(
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
        }

        public ExtensionModel Get(
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
            Set(context, Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectExtensions(
                    tableType: tableType,
                    column: column ?? Rds.ExtensionsDefaultColumns(),
                    join: join ??  Rds.ExtensionsJoinDefault(),
                    where: where ?? Rds.ExtensionsWhereDefault(this),
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
            var response = Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: statements.ToArray());
            ExtensionId = (response.Id ?? ExtensionId).ToInt();
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
                Rds.InsertExtensions(
                    dataTableName: dataTableName,
                    tableType: tableType,
                    selectIdentity: true,
                    param: param ?? Rds.ExtensionsParamDefault(
                        context: context,
                        extensionModel: this,
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
            var response = Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            if (response.Event == "Conflicted")
            {
                return new ErrorData(
                    type: Error.Types.UpdateConflicts,
                    id: ExtensionId);
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
            var where = Rds.ExtensionsWhereDefault(this)
                .UpdatedTime(timestamp, _using: timestamp.InRange());
            if (Versions.VerUp(
                context: context,
                ss: ss,
                verUp: VerUp))
            {
                statements.Add(Rds.ExtensionsCopyToStatement(
                    where: where,
                    tableType: Sqls.TableTypes.History,
                    ColumnNames()));
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

        private List<SqlStatement> UpdateStatements(
            Context context,
            string dataTableName = null,
            SqlWhereCollection where = null,
            SqlParamCollection param = null,
            bool otherInitValue = false)
        {
            return new List<SqlStatement>
            {
                Rds.UpdateExtensions(
                    dataTableName: dataTableName,
                    where: where,
                    param: param ?? Rds.ExtensionsParamDefault(
                        context: context,
                        extensionModel: this,
                        otherInitValue: otherInitValue)),
                new SqlStatement(Def.Sql.IfConflicted.Params(ExtensionId)) {
                    IfConflicted = true,
                    Id = ExtensionId
                }
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
                        referenceId: ExtensionId));
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
                Rds.UpdateOrInsertExtensions(
                    where: where ?? Rds.ExtensionsWhereDefault(this),
                    param: param ?? Rds.ExtensionsParamDefault(
                        context: context, extensionModel: this, setDefault: true))
            };
            var response = Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: statements.ToArray());
            ExtensionId = (response.Id ?? ExtensionId).ToInt();
            Get(context: context);
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData Delete(Context context)
        {
            var statements = new List<SqlStatement>();
            var where = Rds.ExtensionsWhere().ExtensionId(ExtensionId);
            statements.AddRange(new List<SqlStatement>
            {
                Rds.DeleteExtensions(factory: context, where: where)
            });
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData Restore(Context context, int extensionId)
        {
            ExtensionId = extensionId;
            Repository.ExecuteNonQuery(
                context: context,
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreExtensions(
                        factory: context,
                        where: Rds.ExtensionsWhere().ExtensionId(ExtensionId))
                });
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData PhysicalDelete(
            Context context, Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: Rds.PhysicalDeleteExtensions(
                    tableType: tableType,
                    param: Rds.ExtensionsParam().ExtensionId(ExtensionId)));
            return new ErrorData(type: Error.Types.None);
        }

        public void SetByModel(ExtensionModel extensionModel)
        {
            ExtensionType = extensionModel.ExtensionType;
            ExtensionName = extensionModel.ExtensionName;
            ExtensionSettings = extensionModel.ExtensionSettings;
            Body = extensionModel.Body;
            Description = extensionModel.Description;
            Disabled = extensionModel.Disabled;
            Comments = extensionModel.Comments;
            Creator = extensionModel.Creator;
            Updator = extensionModel.Updator;
            CreatedTime = extensionModel.CreatedTime;
            UpdatedTime = extensionModel.UpdatedTime;
            VerUp = extensionModel.VerUp;
            Comments = extensionModel.Comments;
            ClassHash = extensionModel.ClassHash;
            NumHash = extensionModel.NumHash;
            DateHash = extensionModel.DateHash;
            DescriptionHash = extensionModel.DescriptionHash;
            CheckHash = extensionModel.CheckHash;
            AttachmentsHash = extensionModel.AttachmentsHash;
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
                        case "ExtensionId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                ExtensionId = dataRow[column.ColumnName].ToInt();
                                SavedExtensionId = ExtensionId;
                            }
                            break;
                        case "Ver":
                            Ver = dataRow[column.ColumnName].ToInt();
                            SavedVer = Ver;
                            break;
                        case "ExtensionType":
                            ExtensionType = dataRow[column.ColumnName].ToString();
                            SavedExtensionType = ExtensionType;
                            break;
                        case "ExtensionName":
                            ExtensionName = dataRow[column.ColumnName].ToString();
                            SavedExtensionName = ExtensionName;
                            break;
                        case "ExtensionSettings":
                            ExtensionSettings = dataRow[column.ColumnName].ToString();
                            SavedExtensionSettings = ExtensionSettings;
                            break;
                        case "Body":
                            Body = dataRow[column.ColumnName].ToString();
                            SavedBody = Body;
                            break;
                        case "Description":
                            Description = dataRow[column.ColumnName].ToString();
                            SavedDescription = Description;
                            break;
                        case "Disabled":
                            Disabled = dataRow[column.ColumnName].ToBool();
                            SavedDisabled = Disabled;
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
                || ExtensionId_Updated(context: context)
                || Ver_Updated(context: context)
                || ExtensionType_Updated(context: context)
                || ExtensionName_Updated(context: context)
                || ExtensionSettings_Updated(context: context)
                || Body_Updated(context: context)
                || Description_Updated(context: context)
                || Disabled_Updated(context: context)
                || Comments_Updated(context: context)
                || Creator_Updated(context: context)
                || Updator_Updated(context: context);
        }
    }
}
