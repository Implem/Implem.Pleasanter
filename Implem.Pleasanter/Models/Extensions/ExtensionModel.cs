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
namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class ExtensionModel : BaseModel
    {
        public int ExtensionId = 0;
        public int TenantId = 0;
        public string ExtensionType = string.Empty;
        public string ExtensionName = string.Empty;
        public string ExtensionSettings = string.Empty;
        public string Body = string.Empty;
        public string Description = string.Empty;
        public bool Disabled = false;
        public int SavedExtensionId = 0;
        public int SavedTenantId = 0;
        public string SavedExtensionType = string.Empty;
        public string SavedExtensionName = string.Empty;
        public string SavedExtensionSettings = string.Empty;
        public string SavedBody = string.Empty;
        public string SavedDescription = string.Empty;
        public bool SavedDisabled = false;

        public bool ExtensionId_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToInt() != ExtensionId;
            }
            return ExtensionId != SavedExtensionId
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToInt() != ExtensionId);
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

        public bool ExtensionType_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != ExtensionType;
            }
            return ExtensionType != SavedExtensionType && ExtensionType != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != ExtensionType);
        }

        public bool ExtensionName_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != ExtensionName;
            }
            return ExtensionName != SavedExtensionName && ExtensionName != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != ExtensionName);
        }

        public bool ExtensionSettings_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != ExtensionSettings;
            }
            return ExtensionSettings != SavedExtensionSettings && ExtensionSettings != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != ExtensionSettings);
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

        public bool Description_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != Description;
            }
            return Description != SavedDescription && Description != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != Description);
        }

        public bool Disabled_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToBool() != Disabled;
            }
            return Disabled != SavedDisabled
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToBool() != Disabled);
        }

        public ExtensionModel()
        {
        }

        public ExtensionModel(
            Context context,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            TenantId = context.TenantId;
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public ExtensionModel(
            Context context,
            int extensionId,
            ExtensionApiModel extensionApiModel = null,
            SqlColumnCollection column = null,
            bool clearSessions = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            TenantId = context.TenantId;
            ExtensionId = extensionId;
            if (context.QueryStrings.ContainsKey("ver"))
            {
                Get(
                    context: context,
                    tableType: Sqls.TableTypes.NormalAndHistory,
                    column: column,
                    where: Rds.ExtensionsWhereDefault(
                        context: context,
                        extensionModel: this)
                            .Extensions_Ver(context.QueryStrings.Int("ver")));
            }
            else
            {
                Get(
                    context: context,
                    column: column);
            }
            if (extensionApiModel != null)
            {
                SetByApi(context: context, data: extensionApiModel);
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
            where = where ?? Rds.ExtensionsWhereDefault(
                context: context,
                extensionModel: this);
            column = (column ?? Rds.ExtensionsDefaultColumns());
            join = join ?? Rds.ExtensionsJoinDefault();
            Set(context, Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectExtensions(
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

        public ExtensionApiModel GetByApi(Context context)
        {
            return new ExtensionApiModel()
            {
                ApiVersion = context.ApiVersion,
                ExtensionId = ExtensionId,
                TenantId = TenantId,
                Ver = Ver,
                ExtensionType = ExtensionType,
                ExtensionName = ExtensionName,
                ExtensionSettings = ExtensionSettings,
                Body = Body,
                Description = Description,
                Disabled = Disabled,
                Comments = Comments.ToLocal(context: context).ToJson(),
                Creator = Creator.Id,
                Updator = Updator.Id,
                CreatedTime = CreatedTime.Value.ToLocal(context: context),
                UpdatedTime = UpdatedTime.Value.ToLocal(context: context)
            };
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
            ExtensionId = (response.Id ?? ExtensionId).ToInt();
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
                Rds.InsertExtensions(
                    dataTableName: dataTableName,
                    tableType: tableType,
                    selectIdentity: true,
                    param: param ?? Rds.ExtensionsParamDefault(
                        context: context,
                        ss: ss,
                        extensionModel: this,
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
            List<SqlStatement> additionalStatements = null,
            bool checkConflict = true,
            bool verUp = false)
        {
            var timestamp = Timestamp.ToDateTime();
            var statements = new List<SqlStatement>();
            var where = Rds.ExtensionsWhereDefault(
                context: context,
                extensionModel: this)
                    .UpdatedTime(timestamp, _using: timestamp.InRange() && checkConflict);
            if (verUp)
            {
                statements.Add(Rds.ExtensionsCopyToStatement(
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
                Rds.UpdateExtensions(
                    dataTableName: dataTableName,
                    where: where,
                    param: param ?? Rds.ExtensionsParamDefault(
                        context: context,
                        ss: ss,
                        extensionModel: this,
                        otherInitValue: otherInitValue)),
                new SqlStatement()
                {
                    DataTableName = dataTableName,
                    IfConflicted = true,
                    Id = ExtensionId
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
                Rds.UpdateOrInsertExtensions(
                    where: where ?? Rds.ExtensionsWhereDefault(
                        context: context,
                        extensionModel: this),
                    param: param ?? Rds.ExtensionsParamDefault(
                        context: context,
                        ss: ss,
                        extensionModel: this,
                        setDefault: true))
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
                Rds.DeleteExtensions(
                    factory: context,
                    where: where)
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
                    where: Rds.ExtensionsWhere().ExtensionId(ExtensionId)));
            return new ErrorData(type: Error.Types.None);
        }

        public void SetByModel(ExtensionModel extensionModel)
        {
            TenantId = extensionModel.TenantId;
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
            foreach (DataColumn dataColumn in dataRow.Table.Columns)
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
                        case "TenantId":
                            TenantId = dataRow[column.ColumnName].ToInt();
                            SavedTenantId = TenantId;
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
                || ExtensionId_Updated(context: context)
                || TenantId_Updated(context: context)
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
                || ExtensionId_Updated(context: context)
                || TenantId_Updated(context: context)
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


        public void SetByApi(Context context, ExtensionApiModel data)
        {
            //TenantId = data.TenantId ?? TenantId;
            ExtensionType = data.ExtensionType ?? ExtensionType;
            ExtensionName = data.ExtensionName ?? ExtensionName;
            ExtensionSettings = data.ExtensionSettings == null ? ExtensionSettings : Jsons.ToJson(data.ExtensionSettings);
            Body = data.Body ?? Body;
            Description = data.Description ?? Description;
            Disabled = data.Disabled ;
            Comments = data.Comments == null ? Comments : Comments.Prepend(context: context, ss: null, body: data.Comments); //TODO: Commentsの処理方法は、他と同じ？
            //VerUp = data.VerUp?.ToBool() ?? VerUp;
        }


        public string PropertyValue(Context context, Column column)
        {
            return column?.ColumnName switch
            {
                "ExtensionId" => ExtensionId.ToString(),
                "TenantId" => TenantId.ToString(),
                "Ver" => Ver.ToString(),
                "ExtensionType" => ExtensionType,
                "ExtensionName" => ExtensionName,
                "ExtensionSettings" => ExtensionSettings.ToJson(),
                "Body" => Body,
                "Description" => Description,
                "Disabled" => Disabled.ToString(),
                "Comments" => Comments.ToJson(),
                "Creator" => Creator.Id.ToString(),
                "Updator" => Updator.Id.ToString(),
                "CreatedTime" => CreatedTime.Value.ToString(),
                "UpdatedTime" => UpdatedTime.Value.ToString(),
                "VerUp" => VerUp.ToString(),
                "Timestamp" => Timestamp,
                _ => null,
            };
        }
    }
}
