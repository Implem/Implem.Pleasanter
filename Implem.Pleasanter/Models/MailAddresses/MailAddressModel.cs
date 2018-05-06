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
    public class MailAddressModel : BaseModel
    {
        public long OwnerId = 0;
        public string OwnerType = string.Empty;
        public long MailAddressId = 0;
        public string MailAddress = string.Empty;

        public Title Title
        {
            get
            {
                return new Title(MailAddressId, MailAddress);
            }
        }

        [NonSerialized] public long SavedOwnerId = 0;
        [NonSerialized] public string SavedOwnerType = string.Empty;
        [NonSerialized] public long SavedMailAddressId = 0;
        [NonSerialized] public string SavedMailAddress = string.Empty;

        public bool OwnerId_Updated(Column column = null)
        {
            return OwnerId != SavedOwnerId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToLong() != OwnerId);
        }

        public bool OwnerType_Updated(Column column = null)
        {
            return OwnerType != SavedOwnerType && OwnerType != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != OwnerType);
        }

        public bool MailAddressId_Updated(Column column = null)
        {
            return MailAddressId != SavedMailAddressId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToLong() != MailAddressId);
        }

        public bool MailAddress_Updated(Column column = null)
        {
            return MailAddress != SavedMailAddress && MailAddress != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != MailAddress);
        }

        public MailAddressModel()
        {
        }

        public MailAddressModel(
            bool setByForm = false,
            bool setByApi = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            if (setByForm) SetByForm();
            MethodType = methodType;
            OnConstructed();
        }

        public MailAddressModel(
            long mailAddressId,
            bool clearSessions = false,
            bool setByForm = false,
            bool setByApi = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            MailAddressId = mailAddressId;
            Get();
            if (clearSessions) ClearSessions();
            if (setByForm) SetByForm();
            MethodType = methodType;
            OnConstructed();
        }

        public MailAddressModel(DataRow dataRow, string tableAlias = null)
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

        public MailAddressModel Get(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            Set(Rds.ExecuteTable(statements: Rds.SelectMailAddresses(
                tableType: tableType,
                column: column ?? Rds.MailAddressesDefaultColumns(),
                join: join ??  Rds.MailAddressesJoinDefault(),
                where: where ?? Rds.MailAddressesWhereDefault(this),
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
            MailAddressId = newId != 0 ? newId : MailAddressId;
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
                Rds.InsertMailAddresses(
                    tableType: tableType,
                        selectIdentity: true,
                    param: param ?? Rds.MailAddressesParamDefault(
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
            return Error.Types.None;
        }

        private List<SqlStatement> UpdateStatements(
            List<SqlStatement> statements,
            DateTime timestamp,
            SqlParamCollection param,
            bool otherInitValue = false,
            List<SqlStatement> additionalStatements = null)
        {
            var where = Rds.MailAddressesWhereDefault(this)
                .UpdatedTime(timestamp, _using: timestamp.InRange());
            if (VerUp)
            {
                statements.Add(CopyToStatement(where, Sqls.TableTypes.History));
                Ver++;
            }
            statements.AddRange(new List<SqlStatement>
            {
                Rds.UpdateMailAddresses(
                    where: where,
                    param: param ?? Rds.MailAddressesParamDefault(this, otherInitValue: otherInitValue),
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
            var column = new Rds.MailAddressesColumnCollection();
            var param = new Rds.MailAddressesParamCollection();
            column.OwnerId(function: Sqls.Functions.SingleColumn); param.OwnerId();
            column.OwnerType(function: Sqls.Functions.SingleColumn); param.OwnerType();
            column.MailAddressId(function: Sqls.Functions.SingleColumn); param.MailAddressId();
            column.Ver(function: Sqls.Functions.SingleColumn); param.Ver();
            column.MailAddress(function: Sqls.Functions.SingleColumn); param.MailAddress();
            column.Creator(function: Sqls.Functions.SingleColumn); param.Creator();
            column.Updator(function: Sqls.Functions.SingleColumn); param.Updator();
            column.CreatedTime(function: Sqls.Functions.SingleColumn); param.CreatedTime();
            column.UpdatedTime(function: Sqls.Functions.SingleColumn); param.UpdatedTime();
            if (!Comments.InitialValue())
            {
                column.Comments(function: Sqls.Functions.SingleColumn);
                param.Comments();
            }
            return Rds.InsertMailAddresses(
                tableType: tableType,
                param: param,
                select: Rds.SelectMailAddresses(column: column, where: where),
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
                Rds.UpdateOrInsertMailAddresses(
                    selectIdentity: true,
                    where: where ?? Rds.MailAddressesWhereDefault(this),
                    param: param ?? Rds.MailAddressesParamDefault(this, setDefault: true))
            };
            var newId = Rds.ExecuteScalar_long(
                rdsUser: rdsUser,
                transactional: true,
                statements: statements.ToArray());
            MailAddressId = newId != 0 ? newId : MailAddressId;
            Get();
            return Error.Types.None;
        }

        public Error.Types Delete()
        {
            var statements = new List<SqlStatement>();
            var where = Rds.MailAddressesWhere().MailAddressId(MailAddressId);
            statements.AddRange(new List<SqlStatement>
            {
                CopyToStatement(where, Sqls.TableTypes.Deleted),
                Rds.PhysicalDeleteMailAddresses(where: where)
            });
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: statements.ToArray());
            return Error.Types.None;
        }

        public Error.Types Restore(long mailAddressId)
        {
            MailAddressId = mailAddressId;
            Rds.ExecuteNonQuery(
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreMailAddresses(
                        where: Rds.MailAddressesWhere().MailAddressId(MailAddressId))
                });
            return Error.Types.None;
        }

        public Error.Types PhysicalDelete(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: Rds.PhysicalDeleteMailAddresses(
                    tableType: tableType,
                    param: Rds.MailAddressesParam().MailAddressId(MailAddressId)));
            return Error.Types.None;
        }

        public void SetByForm()
        {
            Forms.Keys().ForEach(controlId =>
            {
                switch (controlId)
                {
                    case "MailAddresses_OwnerId": OwnerId = Forms.Data(controlId).ToLong(); break;
                    case "MailAddresses_OwnerType": OwnerType = Forms.Data(controlId).ToString(); break;
                    case "MailAddresses_MailAddress": MailAddress = Forms.Data(controlId).ToString(); break;
                    case "MailAddresses_Timestamp": Timestamp = Forms.Data(controlId).ToString(); break;
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
                        case "OwnerId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                OwnerId = dataRow[column.ColumnName].ToLong();
                                SavedOwnerId = OwnerId;
                            }
                            break;
                        case "OwnerType":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                OwnerType = dataRow[column.ColumnName].ToString();
                                SavedOwnerType = OwnerType;
                            }
                            break;
                        case "MailAddressId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                MailAddressId = dataRow[column.ColumnName].ToLong();
                                SavedMailAddressId = MailAddressId;
                            }
                            break;
                        case "Ver":
                            Ver = dataRow[column.ColumnName].ToInt();
                            SavedVer = Ver;
                            break;
                        case "MailAddress":
                            MailAddress = dataRow[column.ColumnName].ToString();
                            SavedMailAddress = MailAddress;
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
                OwnerId_Updated() ||
                OwnerType_Updated() ||
                MailAddressId_Updated() ||
                Ver_Updated() ||
                MailAddress_Updated() ||
                Comments_Updated() ||
                Creator_Updated() ||
                Updator_Updated();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public MailAddressModel(long userId)
        {
            Get(
                where: Rds.MailAddressesWhere()
                    .OwnerId(userId)
                    .OwnerType("Users"),
                top: 1);
        }
    }
}
