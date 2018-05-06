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
        public string Passphrase = string.Empty;
        public string MailAddress = string.Empty;
        public bool Initialized = false;
        public int TimeLag = 0;
        [NonSerialized] public int SavedDemoId = 0;
        [NonSerialized] public int SavedTenantId = 0;
        [NonSerialized] public string SavedTitle = string.Empty;
        [NonSerialized] public string SavedPassphrase = string.Empty;
        [NonSerialized] public string SavedMailAddress = string.Empty;
        [NonSerialized] public bool SavedInitialized = false;
        [NonSerialized] public int SavedTimeLag = 0;

        public bool DemoId_Updated(Column column = null)
        {
            return DemoId != SavedDemoId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToInt() != DemoId);
        }

        public bool TenantId_Updated(Column column = null)
        {
            return TenantId != SavedTenantId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToInt() != TenantId);
        }

        public bool Title_Updated(Column column = null)
        {
            return Title.Value != SavedTitle && Title.Value != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != Title.Value);
        }

        public bool Passphrase_Updated(Column column = null)
        {
            return Passphrase != SavedPassphrase && Passphrase != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != Passphrase);
        }

        public bool MailAddress_Updated(Column column = null)
        {
            return MailAddress != SavedMailAddress && MailAddress != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != MailAddress);
        }

        public bool Initialized_Updated(Column column = null)
        {
            return Initialized != SavedInitialized &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToBool() != Initialized);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public DemoModel()
        {
        }

        public DemoModel(
            bool setByForm = false,
            bool setByApi = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            if (setByForm) SetByForm();
            MethodType = methodType;
            OnConstructed();
        }

        public DemoModel(
            int demoId,
            bool clearSessions = false,
            bool setByForm = false,
            bool setByApi = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            DemoId = demoId;
            Get();
            if (clearSessions) ClearSessions();
            if (setByForm) SetByForm();
            MethodType = methodType;
            OnConstructed();
        }

        public DemoModel(DataRow dataRow, string tableAlias = null)
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

        public DemoModel Get(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            Set(Rds.ExecuteTable(statements: Rds.SelectDemos(
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
            RdsUser rdsUser = null,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool otherInitValue = false,
            bool get = true)
        {
            var statements = new List<SqlStatement>();
            CreateStatements(statements, tableType, param, otherInitValue);
            var newId = Rds.ExecuteScalar_int(
                rdsUser: rdsUser,
                transactional: true,
                statements: statements.ToArray());
            DemoId = newId != 0 ? newId : DemoId;
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
                Rds.InsertDemos(
                    tableType: tableType,
                        selectIdentity: true,
                    param: param ?? Rds.DemosParamDefault(
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
                    param: param ?? Rds.DemosParamDefault(this, otherInitValue: otherInitValue),
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
            column.Passphrase(function: Sqls.Functions.SingleColumn); param.Passphrase();
            column.MailAddress(function: Sqls.Functions.SingleColumn); param.MailAddress();
            column.Initialized(function: Sqls.Functions.SingleColumn); param.Initialized();
            column.Creator(function: Sqls.Functions.SingleColumn); param.Creator();
            column.Updator(function: Sqls.Functions.SingleColumn); param.Updator();
            column.CreatedTime(function: Sqls.Functions.SingleColumn); param.CreatedTime();
            column.UpdatedTime(function: Sqls.Functions.SingleColumn); param.UpdatedTime();
            if (!Title.InitialValue())
            {
                column.Title(function: Sqls.Functions.SingleColumn);
                param.Title();
            }
            if (!Comments.InitialValue())
            {
                column.Comments(function: Sqls.Functions.SingleColumn);
                param.Comments();
            }
            return Rds.InsertDemos(
                tableType: tableType,
                param: param,
                select: Rds.SelectDemos(column: column, where: where),
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
                Rds.UpdateOrInsertDemos(
                    selectIdentity: true,
                    where: where ?? Rds.DemosWhereDefault(this),
                    param: param ?? Rds.DemosParamDefault(this, setDefault: true))
            };
            var newId = Rds.ExecuteScalar_int(
                rdsUser: rdsUser,
                transactional: true,
                statements: statements.ToArray());
            DemoId = newId != 0 ? newId : DemoId;
            Get();
            return Error.Types.None;
        }

        public Error.Types Delete()
        {
            var statements = new List<SqlStatement>();
            var where = Rds.DemosWhere().DemoId(DemoId);
            statements.AddRange(new List<SqlStatement>
            {
                CopyToStatement(where, Sqls.TableTypes.Deleted),
                Rds.PhysicalDeleteDemos(where: where)
            });
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: statements.ToArray());
            return Error.Types.None;
        }

        public Error.Types Restore(int demoId)
        {
            DemoId = demoId;
            Rds.ExecuteNonQuery(
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
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: Rds.PhysicalDeleteDemos(
                    tableType: tableType,
                    param: Rds.DemosParam().DemoId(DemoId)));
            return Error.Types.None;
        }

        public void SetByForm()
        {
            Forms.Keys().ForEach(controlId =>
            {
                switch (controlId)
                {
                    case "Demos_TenantId": TenantId = Forms.Data(controlId).ToInt(); break;
                    case "Demos_Title": Title = new Title(DemoId, Forms.Data(controlId)); break;
                    case "Demos_Passphrase": Passphrase = Forms.Data(controlId).ToString(); break;
                    case "Demos_MailAddress": MailAddress = Forms.Data(controlId).ToString(); break;
                    case "Demos_Initialized": Initialized = Forms.Data(controlId).ToBool(); break;
                    case "Demos_TimeLag": TimeLag = Forms.Data(controlId).ToInt(); break;
                    case "Demos_Timestamp": Timestamp = Forms.Data(controlId).ToString(); break;
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
                DemoId_Updated() ||
                Ver_Updated() ||
                TenantId_Updated() ||
                Title_Updated() ||
                Passphrase_Updated() ||
                MailAddress_Updated() ||
                Initialized_Updated() ||
                Comments_Updated() ||
                Creator_Updated() ||
                Updator_Updated();
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
