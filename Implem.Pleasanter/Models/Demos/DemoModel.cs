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
    public class DemoModel : BaseModel
    {
        public int DemoId = 0;
        public int TenantId = 0;
        public Title Title = new Title();
        public string LoginId = string.Empty;
        public string Passphrase = string.Empty;
        public string MailAddress = string.Empty;
        public bool Initialized = false;
        public int TimeLag = 0;
        [NonSerialized] public int SavedDemoId = 0;
        [NonSerialized] public int SavedTenantId = 0;
        [NonSerialized] public string SavedTitle = string.Empty;
        [NonSerialized] public string SavedLoginId = string.Empty;
        [NonSerialized] public string SavedPassphrase = string.Empty;
        [NonSerialized] public string SavedMailAddress = string.Empty;
        [NonSerialized] public bool SavedInitialized = false;
        [NonSerialized] public int SavedTimeLag = 0;

        public bool DemoId_Updated(Context context, Column column = null)
        {
            return DemoId != SavedDemoId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != DemoId);
        }

        public bool TenantId_Updated(Context context, Column column = null)
        {
            return TenantId != SavedTenantId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != TenantId);
        }

        public bool Title_Updated(Context context, Column column = null)
        {
            return Title.Value != SavedTitle && Title.Value != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != Title.Value);
        }

        public bool LoginId_Updated(Context context, Column column = null)
        {
            return LoginId != SavedLoginId && LoginId != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != LoginId);
        }

        public bool Passphrase_Updated(Context context, Column column = null)
        {
            return Passphrase != SavedPassphrase && Passphrase != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != Passphrase);
        }

        public bool MailAddress_Updated(Context context, Column column = null)
        {
            return MailAddress != SavedMailAddress && MailAddress != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != MailAddress);
        }

        public bool Initialized_Updated(Context context, Column column = null)
        {
            return Initialized != SavedInitialized &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToBool() != Initialized);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public DemoModel()
        {
        }

        public DemoModel(
            Context context,
            bool setByForm = false,
            bool setByApi = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            Context = context;
            TenantId = context.TenantId;
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public DemoModel(
            Context context,
            int demoId,
            bool clearSessions = false,
            bool setByForm = false,
            bool setByApi = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            Context = context;
            TenantId = context.TenantId;
            DemoId = demoId;
            Get(context: context);
            if (clearSessions) ClearSessions(context: context);
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public DemoModel(Context context, DataRow dataRow, string tableAlias = null)
        {
            OnConstructing(context: context);
            Context = context;
            TenantId = context.TenantId;
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

        public DemoModel Get(
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
                statements: Rds.SelectDemos(
                    tableType: tableType,
                    column: column ?? Rds.DemosDefaultColumns(),
                    join: join ??  Rds.DemosJoinDefault(),
                    where: where ?? Rds.DemosWhereDefault(this),
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
            TenantId = context.TenantId;
            var statements = new List<SqlStatement>();
            CreateStatements(context, statements, tableType, param, otherInitValue);
            var response = Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: statements.ToArray());
            DemoId = (response.Identity ?? DemoId).ToInt();
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
                Rds.InsertDemos(
                    tableType: tableType,
                    setIdentity: true,
                    param: param ?? Rds.DemosParamDefault(
                        context: context,
                        demoModel: this,
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
            var where = Rds.DemosWhereDefault(this)
                .UpdatedTime(timestamp, _using: timestamp.InRange());
            if (VerUp)
            {
                statements.Add(CopyToStatement(where, Sqls.TableTypes.History));
                Ver++;
            }
            statements.AddRange(new List<SqlStatement>
            {
                Rds.UpdateDemos(
                    where: where,
                    param: param ?? Rds.DemosParamDefault(
                        context: context, demoModel: this, otherInitValue: otherInitValue),
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
            var column = new Rds.DemosColumnCollection();
            var param = new Rds.DemosParamCollection();
            column.DemoId(function: Sqls.Functions.SingleColumn); param.DemoId();
            column.Ver(function: Sqls.Functions.SingleColumn); param.Ver();
            column.TenantId(function: Sqls.Functions.SingleColumn); param.TenantId();
            column.Title(function: Sqls.Functions.SingleColumn); param.Title();
            column.LoginId(function: Sqls.Functions.SingleColumn); param.LoginId();
            column.Passphrase(function: Sqls.Functions.SingleColumn); param.Passphrase();
            column.MailAddress(function: Sqls.Functions.SingleColumn); param.MailAddress();
            column.Initialized(function: Sqls.Functions.SingleColumn); param.Initialized();
            column.Comments(function: Sqls.Functions.SingleColumn); param.Comments();
            column.Creator(function: Sqls.Functions.SingleColumn); param.Creator();
            column.Updator(function: Sqls.Functions.SingleColumn); param.Updator();
            column.CreatedTime(function: Sqls.Functions.SingleColumn); param.CreatedTime();
            column.UpdatedTime(function: Sqls.Functions.SingleColumn); param.UpdatedTime();
            return Rds.InsertDemos(
                tableType: tableType,
                param: param,
                select: Rds.SelectDemos(column: column, where: where),
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
                Rds.UpdateOrInsertDemos(
                    where: where ?? Rds.DemosWhereDefault(this),
                    param: param ?? Rds.DemosParamDefault(
                        context: context, demoModel: this, setDefault: true))
            };
            var response = Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: statements.ToArray());
            DemoId = (response.Identity ?? DemoId).ToInt();
            Get(context: context);
            return Error.Types.None;
        }

        public Error.Types Delete(Context context)
        {
            var statements = new List<SqlStatement>();
            var where = Rds.DemosWhere().DemoId(DemoId);
            statements.AddRange(new List<SqlStatement>
            {
                Rds.DeleteDemos(where: where)
            });
            var response = Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            return Error.Types.None;
        }

        public Error.Types Restore(Context context, int demoId)
        {
            DemoId = demoId;
            Rds.ExecuteNonQuery(
                context: context,
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreDemos(
                        where: Rds.DemosWhere().DemoId(DemoId))
                });
            return Error.Types.None;
        }

        public Error.Types PhysicalDelete(
            Context context, Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Rds.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: Rds.PhysicalDeleteDemos(
                    tableType: tableType,
                    param: Rds.DemosParam().DemoId(DemoId)));
            return Error.Types.None;
        }

        public void SetByModel(DemoModel demoModel)
        {
            TenantId = demoModel.TenantId;
            Title = demoModel.Title;
            LoginId = demoModel.LoginId;
            Passphrase = demoModel.Passphrase;
            MailAddress = demoModel.MailAddress;
            Initialized = demoModel.Initialized;
            TimeLag = demoModel.TimeLag;
            Comments = demoModel.Comments;
            Creator = demoModel.Creator;
            Updator = demoModel.Updator;
            CreatedTime = demoModel.CreatedTime;
            UpdatedTime = demoModel.UpdatedTime;
            VerUp = demoModel.VerUp;
            Comments = demoModel.Comments;
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
                        case "DemoId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                DemoId = dataRow[column.ColumnName].ToInt();
                                SavedDemoId = DemoId;
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
                        case "Title":
                            Title = new Title(dataRow, "DemoId");
                            SavedTitle = Title.Value;
                            break;
                        case "LoginId":
                            LoginId = dataRow[column.ColumnName].ToString();
                            SavedLoginId = LoginId;
                            break;
                        case "Passphrase":
                            Passphrase = dataRow[column.ColumnName].ToString();
                            SavedPassphrase = Passphrase;
                            break;
                        case "MailAddress":
                            MailAddress = dataRow[column.ColumnName].ToString();
                            SavedMailAddress = MailAddress;
                            break;
                        case "Initialized":
                            Initialized = dataRow[column.ColumnName].ToBool();
                            SavedInitialized = Initialized;
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
                DemoId_Updated(context: context) ||
                Ver_Updated(context: context) ||
                TenantId_Updated(context: context) ||
                Title_Updated(context: context) ||
                LoginId_Updated(context: context) ||
                Passphrase_Updated(context: context) ||
                MailAddress_Updated(context: context) ||
                Initialized_Updated(context: context) ||
                Comments_Updated(context: context) ||
                Creator_Updated(context: context) ||
                Updator_Updated(context: context);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public void InitializeTimeLag()
        {
            TimeLag = (
                DateTime.Now -
                Def.DemoDefinitionCollection
                    .Where(o => o.UpdatedTime >= Parameters.General.MinTime)
                    .Select(o => o.UpdatedTime).Max()).Days - 1;
        }
    }
}
