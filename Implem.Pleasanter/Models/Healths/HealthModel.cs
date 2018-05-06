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
    public class HealthModel : BaseModel
    {
        public long HealthId = 0;
        public int TenantCount = 0;
        public int UserCount = 0;
        public int ItemCount = 0;
        public int ErrorCount = 0;
        public double Elapsed = 0;
        [NonSerialized] public long SavedHealthId = 0;
        [NonSerialized] public int SavedTenantCount = 0;
        [NonSerialized] public int SavedUserCount = 0;
        [NonSerialized] public int SavedItemCount = 0;
        [NonSerialized] public int SavedErrorCount = 0;
        [NonSerialized] public double SavedElapsed = 0;

        public bool HealthId_Updated(Column column = null)
        {
            return HealthId != SavedHealthId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToLong() != HealthId);
        }

        public bool TenantCount_Updated(Column column = null)
        {
            return TenantCount != SavedTenantCount &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToInt() != TenantCount);
        }

        public bool UserCount_Updated(Column column = null)
        {
            return UserCount != SavedUserCount &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToInt() != UserCount);
        }

        public bool ItemCount_Updated(Column column = null)
        {
            return ItemCount != SavedItemCount &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToInt() != ItemCount);
        }

        public bool ErrorCount_Updated(Column column = null)
        {
            return ErrorCount != SavedErrorCount &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToInt() != ErrorCount);
        }

        public bool Elapsed_Updated(Column column = null)
        {
            return Elapsed != SavedElapsed &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToDouble() != Elapsed);
        }

        public HealthModel()
        {
        }

        public HealthModel(
            bool setByForm = false,
            bool setByApi = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            if (setByForm) SetByForm();
            MethodType = methodType;
            OnConstructed();
        }

        public HealthModel(
            long healthId,
            bool clearSessions = false,
            bool setByForm = false,
            bool setByApi = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            HealthId = healthId;
            Get();
            if (clearSessions) ClearSessions();
            if (setByForm) SetByForm();
            MethodType = methodType;
            OnConstructed();
        }

        public HealthModel(DataRow dataRow, string tableAlias = null)
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

        public HealthModel Get(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            Set(Rds.ExecuteTable(statements: Rds.SelectHealths(
                tableType: tableType,
                column: column ?? Rds.HealthsDefaultColumns(),
                join: join ??  Rds.HealthsJoinDefault(),
                where: where ?? Rds.HealthsWhereDefault(this),
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
            HealthId = newId != 0 ? newId : HealthId;
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
                Rds.InsertHealths(
                    tableType: tableType,
                        selectIdentity: true,
                    param: param ?? Rds.HealthsParamDefault(
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
            var where = Rds.HealthsWhereDefault(this)
                .UpdatedTime(timestamp, _using: timestamp.InRange());
            if (VerUp)
            {
                statements.Add(CopyToStatement(where, Sqls.TableTypes.History));
                Ver++;
            }
            statements.AddRange(new List<SqlStatement>
            {
                Rds.UpdateHealths(
                    where: where,
                    param: param ?? Rds.HealthsParamDefault(this, otherInitValue: otherInitValue),
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
            var column = new Rds.HealthsColumnCollection();
            var param = new Rds.HealthsParamCollection();
            column.HealthId(function: Sqls.Functions.SingleColumn); param.HealthId();
            column.Ver(function: Sqls.Functions.SingleColumn); param.Ver();
            column.TenantCount(function: Sqls.Functions.SingleColumn); param.TenantCount();
            column.UserCount(function: Sqls.Functions.SingleColumn); param.UserCount();
            column.ItemCount(function: Sqls.Functions.SingleColumn); param.ItemCount();
            column.ErrorCount(function: Sqls.Functions.SingleColumn); param.ErrorCount();
            column.Elapsed(function: Sqls.Functions.SingleColumn); param.Elapsed();
            column.Creator(function: Sqls.Functions.SingleColumn); param.Creator();
            column.Updator(function: Sqls.Functions.SingleColumn); param.Updator();
            column.CreatedTime(function: Sqls.Functions.SingleColumn); param.CreatedTime();
            column.UpdatedTime(function: Sqls.Functions.SingleColumn); param.UpdatedTime();
            if (!Comments.InitialValue())
            {
                column.Comments(function: Sqls.Functions.SingleColumn);
                param.Comments();
            }
            return Rds.InsertHealths(
                tableType: tableType,
                param: param,
                select: Rds.SelectHealths(column: column, where: where),
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
                Rds.UpdateOrInsertHealths(
                    selectIdentity: true,
                    where: where ?? Rds.HealthsWhereDefault(this),
                    param: param ?? Rds.HealthsParamDefault(this, setDefault: true))
            };
            var newId = Rds.ExecuteScalar_long(
                rdsUser: rdsUser,
                transactional: true,
                statements: statements.ToArray());
            HealthId = newId != 0 ? newId : HealthId;
            Get();
            return Error.Types.None;
        }

        public Error.Types Delete()
        {
            var statements = new List<SqlStatement>();
            var where = Rds.HealthsWhere().HealthId(HealthId);
            statements.AddRange(new List<SqlStatement>
            {
                CopyToStatement(where, Sqls.TableTypes.Deleted),
                Rds.PhysicalDeleteHealths(where: where)
            });
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: statements.ToArray());
            return Error.Types.None;
        }

        public Error.Types Restore(long healthId)
        {
            HealthId = healthId;
            Rds.ExecuteNonQuery(
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreHealths(
                        where: Rds.HealthsWhere().HealthId(HealthId))
                });
            return Error.Types.None;
        }

        public Error.Types PhysicalDelete(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: Rds.PhysicalDeleteHealths(
                    tableType: tableType,
                    param: Rds.HealthsParam().HealthId(HealthId)));
            return Error.Types.None;
        }

        public void SetByForm()
        {
            Forms.Keys().ForEach(controlId =>
            {
                switch (controlId)
                {
                    case "Healths_TenantCount": TenantCount = Forms.Data(controlId).ToInt(); break;
                    case "Healths_UserCount": UserCount = Forms.Data(controlId).ToInt(); break;
                    case "Healths_ItemCount": ItemCount = Forms.Data(controlId).ToInt(); break;
                    case "Healths_ErrorCount": ErrorCount = Forms.Data(controlId).ToInt(); break;
                    case "Healths_Elapsed": Elapsed = Forms.Data(controlId).ToDouble(); break;
                    case "Healths_Timestamp": Timestamp = Forms.Data(controlId).ToString(); break;
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
                        case "HealthId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                HealthId = dataRow[column.ColumnName].ToLong();
                                SavedHealthId = HealthId;
                            }
                            break;
                        case "Ver":
                            Ver = dataRow[column.ColumnName].ToInt();
                            SavedVer = Ver;
                            break;
                        case "TenantCount":
                            TenantCount = dataRow[column.ColumnName].ToInt();
                            SavedTenantCount = TenantCount;
                            break;
                        case "UserCount":
                            UserCount = dataRow[column.ColumnName].ToInt();
                            SavedUserCount = UserCount;
                            break;
                        case "ItemCount":
                            ItemCount = dataRow[column.ColumnName].ToInt();
                            SavedItemCount = ItemCount;
                            break;
                        case "ErrorCount":
                            ErrorCount = dataRow[column.ColumnName].ToInt();
                            SavedErrorCount = ErrorCount;
                            break;
                        case "Elapsed":
                            Elapsed = dataRow[column.ColumnName].ToDouble();
                            SavedElapsed = Elapsed;
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
                HealthId_Updated() ||
                Ver_Updated() ||
                TenantCount_Updated() ||
                UserCount_Updated() ||
                ItemCount_Updated() ||
                ErrorCount_Updated() ||
                Elapsed_Updated() ||
                Comments_Updated() ||
                Creator_Updated() ||
                Updator_Updated();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public HealthModel(DateTime time)
        {
            var now = DateTime.Now;
            var dataTable = Rds.ExecuteDataSet(statements: new SqlStatement[]
            {
                Rds.SelectTenants(
                    dataTableName: "TenantCount",
                    column: Rds.TenantsColumn().TenantsCount()),
                Rds.SelectUsers(
                    dataTableName: "UserCount",
                    column: Rds.UsersColumn().UsersCount()),
                Rds.SelectItems(
                    dataTableName: "ItemCount",
                    column: Rds.ItemsColumn().ItemsCount()),
                Rds.SelectSysLogs(
                    dataTableName: "ErrorCount",
                    column: Rds.SysLogsColumn().SysLogsCount(),
                    where: Rds.SysLogsWhere()
                        .CreatedTime(time, _operator: ">=")
                        .ErrMessage(_operator: " is not null"))
            });
            TenantCount = dataTable.Tables["TenantCount"].Rows[0][0].ToInt();
            UserCount = dataTable.Tables["UserCount"].Rows[0][0].ToInt();
            ItemCount = dataTable.Tables["ItemCount"].Rows[0][0].ToInt();
            ErrorCount = dataTable.Tables["ErrorCount"].Rows[0][0].ToInt();
            Elapsed = (DateTime.Now - now).Milliseconds;
            Create();
        }
    }
}
