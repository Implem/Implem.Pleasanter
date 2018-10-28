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

        public bool HealthId_Updated(Context context, Column column = null)
        {
            return HealthId != SavedHealthId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToLong() != HealthId);
        }

        public bool TenantCount_Updated(Context context, Column column = null)
        {
            return TenantCount != SavedTenantCount &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != TenantCount);
        }

        public bool UserCount_Updated(Context context, Column column = null)
        {
            return UserCount != SavedUserCount &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != UserCount);
        }

        public bool ItemCount_Updated(Context context, Column column = null)
        {
            return ItemCount != SavedItemCount &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != ItemCount);
        }

        public bool ErrorCount_Updated(Context context, Column column = null)
        {
            return ErrorCount != SavedErrorCount &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != ErrorCount);
        }

        public bool Elapsed_Updated(Context context, Column column = null)
        {
            return Elapsed != SavedElapsed &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToDouble() != Elapsed);
        }

        public HealthModel()
        {
        }

        public HealthModel(
            Context context,
            bool setByForm = false,
            bool setByApi = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            Context = context;
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public HealthModel(
            Context context,
            long healthId,
            bool clearSessions = false,
            bool setByForm = false,
            bool setByApi = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            Context = context;
            HealthId = healthId;
            Get(context: context);
            if (clearSessions) ClearSessions(context: context);
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public HealthModel(Context context, DataRow dataRow, string tableAlias = null)
        {
            OnConstructing(context: context);
            Context = context;
            if (dataRow != null) Set(context, dataRow, tableAlias);
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

        public HealthModel Get(
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
                statements: Rds.SelectHealths(
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
            Context context,
            SiteSettings ss,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool otherInitValue = false,
            bool get = true)
        {
            var statements = new List<SqlStatement>();
            CreateStatements(context, statements, tableType, param, otherInitValue);
            var response = Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: statements.ToArray());
            HealthId = (response.Identity ?? HealthId).ToLong();
            if (get) Get(context: context);
            return Error.Types.None;
        }

        public List<SqlStatement> CreateStatements(
            Context context,
            List<SqlStatement> statements,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool otherInitValue = false)
        {
            statements.AddRange(new List<SqlStatement>
            {
                Rds.InsertHealths(
                    tableType: tableType,
                    setIdentity: true,
                    param: param ?? Rds.HealthsParamDefault(
                        context: context,
                        healthModel: this,
                        setDefault: true,
                        otherInitValue: otherInitValue))
            });
            return statements;
        }

        public Error.Types Update(
            Context context,
            SiteSettings ss,
            SqlParamCollection param = null,
            List<SqlStatement> additionalStatements = null,
            bool otherInitValue = false,
            bool setBySession = true,
            bool get = true)
        {
            if (setBySession) SetBySession(context: context);
            var timestamp = Timestamp.ToDateTime();
            var statements = new List<SqlStatement>();
            UpdateStatements(
                context: context,
                ss: ss,
                statements: statements,
                timestamp: timestamp,
                param: param,
                otherInitValue: otherInitValue,
                additionalStatements: additionalStatements);
            var response = Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            if (response.Count == 0) return Error.Types.UpdateConflicts;
            if (get) Get(context: context);
            return Error.Types.None;
        }

        private List<SqlStatement> UpdateStatements(
            Context context,
            SiteSettings ss,
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
                    param: param ?? Rds.HealthsParamDefault(
                        context: context, healthModel: this, otherInitValue: otherInitValue),
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
            column.Comments(function: Sqls.Functions.SingleColumn); param.Comments();
            column.Creator(function: Sqls.Functions.SingleColumn); param.Creator();
            column.Updator(function: Sqls.Functions.SingleColumn); param.Updator();
            column.CreatedTime(function: Sqls.Functions.SingleColumn); param.CreatedTime();
            column.UpdatedTime(function: Sqls.Functions.SingleColumn); param.UpdatedTime();
            return Rds.InsertHealths(
                tableType: tableType,
                param: param,
                select: Rds.SelectHealths(column: column, where: where),
                addUpdatorParam: false);
        }

        public Error.Types UpdateOrCreate(
            Context context,
            SqlWhereCollection where = null,
            SqlParamCollection param = null)
        {
            SetBySession(context: context);
            var statements = new List<SqlStatement>
            {
                Rds.UpdateOrInsertHealths(
                    where: where ?? Rds.HealthsWhereDefault(this),
                    param: param ?? Rds.HealthsParamDefault(
                        context: context, healthModel: this, setDefault: true))
            };
            var response = Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: statements.ToArray());
            HealthId = (response.Identity ?? HealthId).ToLong();
            Get(context: context);
            return Error.Types.None;
        }

        public Error.Types Delete(Context context)
        {
            var statements = new List<SqlStatement>();
            var where = Rds.HealthsWhere().HealthId(HealthId);
            statements.AddRange(new List<SqlStatement>
            {
                Rds.DeleteHealths(where: where)
            });
            var response = Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            return Error.Types.None;
        }

        public Error.Types Restore(Context context, long healthId)
        {
            HealthId = healthId;
            Rds.ExecuteNonQuery(
                context: context,
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
            Context context, Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Rds.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: Rds.PhysicalDeleteHealths(
                    tableType: tableType,
                    param: Rds.HealthsParam().HealthId(HealthId)));
            return Error.Types.None;
        }

        public void SetByModel(HealthModel healthModel)
        {
            TenantCount = healthModel.TenantCount;
            UserCount = healthModel.UserCount;
            ItemCount = healthModel.ItemCount;
            ErrorCount = healthModel.ErrorCount;
            Elapsed = healthModel.Elapsed;
            Comments = healthModel.Comments;
            Creator = healthModel.Creator;
            Updator = healthModel.Updator;
            CreatedTime = healthModel.CreatedTime;
            UpdatedTime = healthModel.UpdatedTime;
            VerUp = healthModel.VerUp;
            Comments = healthModel.Comments;
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
                        case "IsHistory": VerType = dataRow[column.ColumnName].ToBool() ? Versions.VerTypes.History : Versions.VerTypes.Latest; break;
                    }
                }
            }
        }

        public bool Updated(Context context)
        {
            return
                HealthId_Updated(context: context) ||
                Ver_Updated(context: context) ||
                TenantCount_Updated(context: context) ||
                UserCount_Updated(context: context) ||
                ItemCount_Updated(context: context) ||
                ErrorCount_Updated(context: context) ||
                Elapsed_Updated(context: context) ||
                Comments_Updated(context: context) ||
                Creator_Updated(context: context) ||
                Updator_Updated(context: context);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public HealthModel(Context context, DateTime time)
        {
            var now = DateTime.Now;
            var dataTable = Rds.ExecuteDataSet(
                context: context,
                statements: new SqlStatement[]
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
            Create(context: context, ss: new SiteSettings());
        }
    }
}
