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
    public class TenantModel : BaseModel
    {
        public int TenantId = 0;
        public string TenantName = string.Empty;
        public Title Title = new Title();
        public string Body = string.Empty;
        public ContractSettings ContractSettings = new ContractSettings();
        public DateTime ContractDeadline = 0.ToDateTime();
        public LogoTypes LogoType = (LogoTypes)0;
        public string HtmlTitleTop = "[ProductName]";
        public string HtmlTitleSite = "[ProductName]";
        public string HtmlTitleRecord = "[ProductName]";
        [NonSerialized] public int SavedTenantId = 0;
        [NonSerialized] public string SavedTenantName = string.Empty;
        [NonSerialized] public string SavedTitle = string.Empty;
        [NonSerialized] public string SavedBody = string.Empty;
        [NonSerialized] public string SavedContractSettings = string.Empty;
        [NonSerialized] public DateTime SavedContractDeadline = 0.ToDateTime();
        [NonSerialized] public int SavedLogoType = 0;
        [NonSerialized] public string SavedHtmlTitleTop = "[ProductName]";
        [NonSerialized] public string SavedHtmlTitleSite = "[ProductName]";
        [NonSerialized] public string SavedHtmlTitleRecord = "[ProductName]";

        public bool TenantId_Updated(Context context, Column column = null)
        {
            return TenantId != SavedTenantId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != TenantId);
        }

        public bool TenantName_Updated(Context context, Column column = null)
        {
            return TenantName != SavedTenantName && TenantName != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != TenantName);
        }

        public bool Title_Updated(Context context, Column column = null)
        {
            return Title.Value != SavedTitle && Title.Value != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != Title.Value);
        }

        public bool Body_Updated(Context context, Column column = null)
        {
            return Body != SavedBody && Body != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != Body);
        }

        public bool ContractSettings_Updated(Context context, Column column = null)
        {
            return ContractSettings?.RecordingJson() != SavedContractSettings && ContractSettings?.RecordingJson() != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != ContractSettings?.RecordingJson());
        }

        public bool LogoType_Updated(Context context, Column column = null)
        {
            return LogoType.ToInt() != SavedLogoType &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != LogoType.ToInt());
        }

        public bool HtmlTitleTop_Updated(Context context, Column column = null)
        {
            return HtmlTitleTop != SavedHtmlTitleTop && HtmlTitleTop != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != HtmlTitleTop);
        }

        public bool HtmlTitleSite_Updated(Context context, Column column = null)
        {
            return HtmlTitleSite != SavedHtmlTitleSite && HtmlTitleSite != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != HtmlTitleSite);
        }

        public bool HtmlTitleRecord_Updated(Context context, Column column = null)
        {
            return HtmlTitleRecord != SavedHtmlTitleRecord && HtmlTitleRecord != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != HtmlTitleRecord);
        }

        public bool ContractDeadline_Updated(Context context, Column column = null)
        {
            return ContractDeadline != SavedContractDeadline &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != ContractDeadline.Date);
        }

        public List<int> SwitchTargets;

        public TenantModel()
        {
        }

        public TenantModel(
            Context context,
            SiteSettings ss,
            bool setByForm = false,
            bool setByApi = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            Context = context;
            TenantId = context.TenantId;
            if (setByForm) SetByForm(context: context, ss: ss);
            if (setByApi) SetByApi(context: context, ss: ss);
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public TenantModel(
            Context context,
            SiteSettings ss,
            int tenantId,
            bool clearSessions = false,
            bool setByForm = false,
            bool setByApi = false,
            List<int> switchTargets = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            Context = context;
            TenantId = context.TenantId;
            Get(context: context, ss: ss);
            if (clearSessions) ClearSessions(context: context);
            if (setByForm) SetByForm(context: context, ss: ss);
            if (setByApi) SetByApi(context: context, ss: ss);
            SwitchTargets = switchTargets;
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public TenantModel(Context context, SiteSettings ss, DataRow dataRow, string tableAlias = null)
        {
            OnConstructing(context: context);
            Context = context;
            TenantId = context.TenantId;
            if (dataRow != null) Set(context, ss, dataRow, tableAlias);
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

        public TenantModel Get(
            Context context,
            SiteSettings ss,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            Set(context, ss, Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectTenants(
                    tableType: tableType,
                    column: column ?? Rds.TenantsDefaultColumns(),
                    join: join ??  Rds.TenantsJoinDefault(),
                    where: where ?? Rds.TenantsWhereDefault(this),
                    orderBy: orderBy,
                    param: param,
                    distinct: distinct,
                    top: top)));
            return this;
        }

        public TenantApiModel GetByApi(Context context, SiteSettings ss)
        {
            var data = new TenantApiModel();
            ss.ReadableColumns(noJoined: true).ForEach(column =>
            {
                switch (column.ColumnName)
                {
                    case "TenantId": data.TenantId = TenantId; break;
                    case "Ver": data.Ver = Ver; break;
                    case "TenantName": data.TenantName = TenantName; break;
                    case "Title": data.Title = Title.Value; break;
                    case "Body": data.Body = Body; break;
                    case "ContractSettings": data.ContractSettings = ContractSettings?.RecordingJson(); break;
                    case "ContractDeadline": data.ContractDeadline = ContractDeadline.ToLocal(context: context); break;
                    case "LogoType": data.LogoType = LogoType.ToInt(); break;
                    case "HtmlTitleTop": data.HtmlTitleTop = HtmlTitleTop; break;
                    case "HtmlTitleSite": data.HtmlTitleSite = HtmlTitleSite; break;
                    case "HtmlTitleRecord": data.HtmlTitleRecord = HtmlTitleRecord; break;
                    case "Creator": data.Creator = Creator.Id; break;
                    case "Updator": data.Updator = Updator.Id; break;
                    case "CreatedTime": data.CreatedTime = CreatedTime.Value.ToLocal(context: context); break;
                    case "UpdatedTime": data.UpdatedTime = UpdatedTime.Value.ToLocal(context: context); break;
                    case "Comments": data.Comments = Comments.ToLocal(context: context).ToJson(); break;
                }
            });
            return data;
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
            CreateStatements(context, ss, statements, tableType, param, otherInitValue);
            var response = Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: statements.ToArray());
            TenantId = (response.Identity ?? TenantId).ToInt();
            if (get) Get(context: context, ss: ss);
            return Error.Types.None;
        }

        public List<SqlStatement> CreateStatements(
            Context context,
            SiteSettings ss,
            List<SqlStatement> statements,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool otherInitValue = false)
        {
            statements.AddRange(new List<SqlStatement>
            {
                Rds.InsertTenants(
                    tableType: tableType,
                    setIdentity: true,
                    param: param ?? Rds.TenantsParamDefault(
                        context: context,
                        tenantModel: this,
                        setDefault: true,
                        otherInitValue: otherInitValue))
            });
            return statements;
        }

        public Error.Types Update(
            Context context,
            SiteSettings ss,
            IEnumerable<string> permissions = null,
            bool permissionChanged = false,
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
            if (get) Get(context: context, ss: ss);
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
            var where = Rds.TenantsWhereDefault(this)
                .UpdatedTime(timestamp, _using: timestamp.InRange());
            if (VerUp)
            {
                statements.Add(CopyToStatement(where, Sqls.TableTypes.History));
                Ver++;
            }
            statements.AddRange(new List<SqlStatement>
            {
                Rds.UpdateTenants(
                    where: where,
                    param: param ?? Rds.TenantsParamDefault(
                        context: context, tenantModel: this, otherInitValue: otherInitValue),
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
            var column = new Rds.TenantsColumnCollection();
            var param = new Rds.TenantsParamCollection();
            column.TenantId(function: Sqls.Functions.SingleColumn); param.TenantId();
            column.Ver(function: Sqls.Functions.SingleColumn); param.Ver();
            column.TenantName(function: Sqls.Functions.SingleColumn); param.TenantName();
            column.Title(function: Sqls.Functions.SingleColumn); param.Title();
            column.Body(function: Sqls.Functions.SingleColumn); param.Body();
            column.ContractSettings(function: Sqls.Functions.SingleColumn); param.ContractSettings();
            column.ContractDeadline(function: Sqls.Functions.SingleColumn); param.ContractDeadline();
            column.LogoType(function: Sqls.Functions.SingleColumn); param.LogoType();
            column.HtmlTitleTop(function: Sqls.Functions.SingleColumn); param.HtmlTitleTop();
            column.HtmlTitleSite(function: Sqls.Functions.SingleColumn); param.HtmlTitleSite();
            column.HtmlTitleRecord(function: Sqls.Functions.SingleColumn); param.HtmlTitleRecord();
            column.Comments(function: Sqls.Functions.SingleColumn); param.Comments();
            column.Creator(function: Sqls.Functions.SingleColumn); param.Creator();
            column.Updator(function: Sqls.Functions.SingleColumn); param.Updator();
            column.CreatedTime(function: Sqls.Functions.SingleColumn); param.CreatedTime();
            column.UpdatedTime(function: Sqls.Functions.SingleColumn); param.UpdatedTime();
            return Rds.InsertTenants(
                tableType: tableType,
                param: param,
                select: Rds.SelectTenants(column: column, where: where),
                addUpdatorParam: false);
        }

        public Error.Types UpdateOrCreate(
            Context context,
            SiteSettings ss,
            SqlWhereCollection where = null,
            SqlParamCollection param = null)
        {
            SetBySession(context: context);
            var statements = new List<SqlStatement>
            {
                Rds.UpdateOrInsertTenants(
                    where: where ?? Rds.TenantsWhereDefault(this),
                    param: param ?? Rds.TenantsParamDefault(
                        context: context, tenantModel: this, setDefault: true))
            };
            var response = Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: statements.ToArray());
            TenantId = (response.Identity ?? TenantId).ToInt();
            Get(context: context, ss: ss);
            return Error.Types.None;
        }

        public Error.Types Delete(Context context, SiteSettings ss, bool notice = false)
        {
            var statements = new List<SqlStatement>();
            var where = Rds.TenantsWhere().TenantId(TenantId);
            statements.AddRange(new List<SqlStatement>
            {
                Rds.DeleteTenants(where: where)
            });
            var response = Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            var tenantHash = SiteInfo.TenantCaches.Get(context.TenantId)?.TenantHash;
            if (tenantHash.Keys.Contains(TenantId))
            {
                tenantHash.Remove(TenantId);
            }
            return Error.Types.None;
        }

        public Error.Types Restore(Context context, SiteSettings ss,int tenantId)
        {
            TenantId = tenantId;
            Rds.ExecuteNonQuery(
                context: context,
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreTenants(
                        where: Rds.TenantsWhere().TenantId(TenantId))
                });
            return Error.Types.None;
        }

        public Error.Types PhysicalDelete(
            Context context, SiteSettings ss,Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Rds.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: Rds.PhysicalDeleteTenants(
                    tableType: tableType,
                    param: Rds.TenantsParam().TenantId(TenantId)));
            return Error.Types.None;
        }

        public void SetByForm(Context context, SiteSettings ss)
        {
            context.Forms.Keys.ForEach(controlId =>
            {
                switch (controlId)
                {
                    case "Tenants_TenantName": TenantName = context.Forms.Data(controlId).ToString(); break;
                    case "Tenants_Title": Title = new Title(TenantId, context.Forms.Data(controlId)); break;
                    case "Tenants_Body": Body = context.Forms.Data(controlId).ToString(); break;
                    case "Tenants_ContractDeadline": ContractDeadline = context.Forms.Data(controlId).ToDateTime().ToUniversal(context: context); break;
                    case "Tenants_LogoType": LogoType = (LogoTypes)context.Forms.Data(controlId).ToInt(); break;
                    case "Tenants_HtmlTitleTop": HtmlTitleTop = context.Forms.Data(controlId).ToString(); break;
                    case "Tenants_HtmlTitleSite": HtmlTitleSite = context.Forms.Data(controlId).ToString(); break;
                    case "Tenants_HtmlTitleRecord": HtmlTitleRecord = context.Forms.Data(controlId).ToString(); break;
                    case "Tenants_Timestamp": Timestamp = context.Forms.Data(controlId).ToString(); break;
                    case "Comments": Comments.Prepend(context: context, ss: ss, body: context.Forms.Data("Comments")); break;
                    case "VerUp": VerUp = context.Forms.Data(controlId).ToBool(); break;
                    default:
                        if (controlId.RegexExists("Comment[0-9]+"))
                        {
                            Comments.Update(
                                context: context,
                                ss: ss,
                                commentId: controlId.Substring("Comment".Length).ToInt(),
                                body: context.Forms.Data(controlId));
                        }
                        break;
                }
            });
            if (context.Action == "deletecomment")
            {
                DeleteCommentId = context.Forms.ControlId().Split(',')._2nd().ToInt();
                Comments.RemoveAll(o => o.CommentId == DeleteCommentId);
            }
        }

        public void SetByModel(TenantModel tenantModel)
        {
            TenantName = tenantModel.TenantName;
            Title = tenantModel.Title;
            Body = tenantModel.Body;
            ContractSettings = tenantModel.ContractSettings;
            ContractDeadline = tenantModel.ContractDeadline;
            LogoType = tenantModel.LogoType;
            HtmlTitleTop = tenantModel.HtmlTitleTop;
            HtmlTitleSite = tenantModel.HtmlTitleSite;
            HtmlTitleRecord = tenantModel.HtmlTitleRecord;
            Comments = tenantModel.Comments;
            Creator = tenantModel.Creator;
            Updator = tenantModel.Updator;
            CreatedTime = tenantModel.CreatedTime;
            UpdatedTime = tenantModel.UpdatedTime;
            VerUp = tenantModel.VerUp;
            Comments = tenantModel.Comments;
        }

        public void SetByApi(Context context, SiteSettings ss)
        {
            var data = context.RequestDataString.Deserialize<TenantApiModel>();
            if (data == null)
            {
                return;
            }
            if (data.TenantName != null) TenantName = data.TenantName.ToString().ToString();
            if (data.Title != null) Title = new Title(data.Title.ToString());
            if (data.Body != null) Body = data.Body.ToString().ToString();
            if (data.ContractDeadline != null) ContractDeadline = data.ContractDeadline.ToDateTime().ToDateTime().ToUniversal(context: context);
            if (data.LogoType != null) LogoType = (LogoTypes)data.LogoType.ToInt().ToInt();
            if (data.HtmlTitleTop != null) HtmlTitleTop = data.HtmlTitleTop.ToString().ToString();
            if (data.HtmlTitleSite != null) HtmlTitleSite = data.HtmlTitleSite.ToString().ToString();
            if (data.HtmlTitleRecord != null) HtmlTitleRecord = data.HtmlTitleRecord.ToString().ToString();
            if (data.Comments != null) Comments.Prepend(context: context, ss: ss, body: data.Comments);
            if (data.VerUp != null) VerUp = data.VerUp.ToBool();
        }

        private void SetBySession(Context context)
        {
        }

        private void Set(Context context, SiteSettings ss, DataTable dataTable)
        {
            switch (dataTable.Rows.Count)
            {
                case 1: Set(context, ss, dataTable.Rows[0]); break;
                case 0: AccessStatus = Databases.AccessStatuses.NotFound; break;
                default: AccessStatus = Databases.AccessStatuses.Overlap; break;
            }
        }

        private void Set(Context context, SiteSettings ss, DataRow dataRow, string tableAlias = null)
        {
            AccessStatus = Databases.AccessStatuses.Selected;
            foreach(DataColumn dataColumn in dataRow.Table.Columns)
            {
                var column = new ColumnNameInfo(dataColumn.ColumnName);
                if (column.TableAlias == tableAlias)
                {
                    switch (column.Name)
                    {
                        case "TenantId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                TenantId = dataRow[column.ColumnName].ToInt();
                                SavedTenantId = TenantId;
                            }
                            break;
                        case "Ver":
                            Ver = dataRow[column.ColumnName].ToInt();
                            SavedVer = Ver;
                            break;
                        case "TenantName":
                            TenantName = dataRow[column.ColumnName].ToString();
                            SavedTenantName = TenantName;
                            break;
                        case "Title":
                            Title = new Title(dataRow, "TenantId");
                            SavedTitle = Title.Value;
                            break;
                        case "Body":
                            Body = dataRow[column.ColumnName].ToString();
                            SavedBody = Body;
                            break;
                        case "ContractSettings":
                            ContractSettings = GetContractSettings(dataRow);
                            SavedContractSettings = ContractSettings?.RecordingJson();
                            break;
                        case "ContractDeadline":
                            ContractDeadline = dataRow[column.ColumnName].ToDateTime();
                            SavedContractDeadline = ContractDeadline;
                            break;
                        case "LogoType":
                            LogoType = (LogoTypes)dataRow[column.ColumnName].ToInt();
                            SavedLogoType = LogoType.ToInt();
                            break;
                        case "HtmlTitleTop":
                            HtmlTitleTop = dataRow[column.ColumnName].ToString();
                            SavedHtmlTitleTop = HtmlTitleTop;
                            break;
                        case "HtmlTitleSite":
                            HtmlTitleSite = dataRow[column.ColumnName].ToString();
                            SavedHtmlTitleSite = HtmlTitleSite;
                            break;
                        case "HtmlTitleRecord":
                            HtmlTitleRecord = dataRow[column.ColumnName].ToString();
                            SavedHtmlTitleRecord = HtmlTitleRecord;
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
                TenantId_Updated(context: context) ||
                Ver_Updated(context: context) ||
                TenantName_Updated(context: context) ||
                Title_Updated(context: context) ||
                Body_Updated(context: context) ||
                ContractSettings_Updated(context: context) ||
                ContractDeadline_Updated(context: context) ||
                LogoType_Updated(context: context) ||
                HtmlTitleTop_Updated(context: context) ||
                HtmlTitleSite_Updated(context: context) ||
                HtmlTitleRecord_Updated(context: context) ||
                Comments_Updated(context: context) ||
                Creator_Updated(context: context) ||
                Updator_Updated(context: context);
        }

        public List<string> Mine(Context context)
        {
            var mine = new List<string>();
            var userId = context.UserId;
            if (SavedCreator == userId) mine.Add("Creator");
            if (SavedUpdator == userId) mine.Add("Updator");
            return mine;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private ContractSettings GetContractSettings(DataRow dataRow)
        {
            return null;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public enum LogoTypes
        {
            ImageOnly,
            ImageAndTitle
        }
    }
}
