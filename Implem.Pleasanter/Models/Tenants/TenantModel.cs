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
    public class TenantModel : BaseModel
    {
        public int TenantId = 0;
        public string TenantName = string.Empty;
        public Title Title = new Title();
        public string Body = string.Empty;
        public ContractSettings ContractSettings = new ContractSettings();
        public DateTime ContractDeadline = 0.ToDateTime();
        public bool DisableAllUsersPermission = false;
        public bool DisableApi = false;
        public bool AllowExtensionsApi = false;
        public bool DisableStartGuide = false;
        public LogoTypes LogoType = (LogoTypes)0;
        public string HtmlTitleTop = "[ProductName]";
        public string HtmlTitleSite = "[ProductName]";
        public string HtmlTitleRecord = "[ProductName]";
        public string TopStyle = string.Empty;
        public string TopScript = string.Empty;
        public string TopDashboards = string.Empty;
        public string Theme = string.Empty;
        public string Language = string.Empty;
        public string TimeZone = string.Empty;
        public TenantSettings TenantSettings = new TenantSettings();
        public int SavedTenantId = 0;
        public string SavedTenantName = string.Empty;
        public string SavedTitle = string.Empty;
        public string SavedBody = string.Empty;
        public string SavedContractSettings = string.Empty;
        public DateTime SavedContractDeadline = 0.ToDateTime();
        public bool SavedDisableAllUsersPermission = false;
        public bool SavedDisableApi = false;
        public bool SavedAllowExtensionsApi = false;
        public bool SavedDisableStartGuide = false;
        public int SavedLogoType = 0;
        public string SavedHtmlTitleTop = "[ProductName]";
        public string SavedHtmlTitleSite = "[ProductName]";
        public string SavedHtmlTitleRecord = "[ProductName]";
        public string SavedTopStyle = string.Empty;
        public string SavedTopScript = string.Empty;
        public string SavedTopDashboards = string.Empty;
        public string SavedTheme = string.Empty;
        public string SavedLanguage = string.Empty;
        public string SavedTimeZone = string.Empty;
        public string SavedTenantSettings = string.Empty;

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

        public bool TenantName_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != TenantName;
            }
            return TenantName != SavedTenantName && TenantName != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != TenantName);
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

        public bool ContractSettings_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != ContractSettings?.RecordingJson();
            }
            return ContractSettings?.RecordingJson() != SavedContractSettings && ContractSettings?.RecordingJson() != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != ContractSettings?.RecordingJson());
        }

        public bool DisableAllUsersPermission_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToBool() != DisableAllUsersPermission;
            }
            return DisableAllUsersPermission != SavedDisableAllUsersPermission
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToBool() != DisableAllUsersPermission);
        }

        public bool DisableApi_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToBool() != DisableApi;
            }
            return DisableApi != SavedDisableApi
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToBool() != DisableApi);
        }

        public bool AllowExtensionsApi_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToBool() != AllowExtensionsApi;
            }
            return AllowExtensionsApi != SavedAllowExtensionsApi
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToBool() != AllowExtensionsApi);
        }

        public bool DisableStartGuide_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToBool() != DisableStartGuide;
            }
            return DisableStartGuide != SavedDisableStartGuide
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToBool() != DisableStartGuide);
        }

        public bool LogoType_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToInt() != LogoType.ToInt();
            }
            return LogoType.ToInt() != SavedLogoType
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToInt() != LogoType.ToInt());
        }

        public bool HtmlTitleTop_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != HtmlTitleTop;
            }
            return HtmlTitleTop != SavedHtmlTitleTop && HtmlTitleTop != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != HtmlTitleTop);
        }

        public bool HtmlTitleSite_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != HtmlTitleSite;
            }
            return HtmlTitleSite != SavedHtmlTitleSite && HtmlTitleSite != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != HtmlTitleSite);
        }

        public bool HtmlTitleRecord_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != HtmlTitleRecord;
            }
            return HtmlTitleRecord != SavedHtmlTitleRecord && HtmlTitleRecord != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != HtmlTitleRecord);
        }

        public bool TopStyle_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != TopStyle;
            }
            return TopStyle != SavedTopStyle && TopStyle != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != TopStyle);
        }

        public bool TopScript_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != TopScript;
            }
            return TopScript != SavedTopScript && TopScript != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != TopScript);
        }

        public bool TopDashboards_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != TopDashboards;
            }
            return TopDashboards != SavedTopDashboards && TopDashboards != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != TopDashboards);
        }

        public bool Theme_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != Theme;
            }
            return Theme != SavedTheme && Theme != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != Theme);
        }

        public bool Language_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != Language;
            }
            return Language != SavedLanguage && Language != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != Language);
        }

        public bool TimeZone_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != TimeZone;
            }
            return TimeZone != SavedTimeZone && TimeZone != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != TimeZone);
        }

        public bool TenantSettings_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != TenantSettings.RecordingJson(context: context);
            }
            return TenantSettings.RecordingJson(context: context) != SavedTenantSettings && TenantSettings.RecordingJson(context: context) != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != TenantSettings.RecordingJson(context: context));
        }

        public bool ContractDeadline_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToDateTime() != ContractDeadline;
            }
            return ContractDeadline != SavedContractDeadline
                && (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.DefaultTime(context: context).Date != ContractDeadline.Date);
        }

        public TenantSettings Session_TenantSettings(Context context)
        {
            return context.SessionData.Get("TenantSettings") != null
                ? TenantSettings.GetTenantSettings(context: context, context.SessionData.Get("TenantSettings")) ?? new TenantSettings(context: context)
                : TenantSettings;
        }

        public void Session_TenantSettings(Context context, string value)
        {
            string key = "TenantSettings";
            if (Parameters.Session.UseKeyValueStore && value == null)
            {
                string pageName = context.Page ?? string.Empty;
                StackExchange.Redis.IDatabase iDatabase = Implem.Pleasanter.Libraries.Redis.CacheForRedisConnection.Connection.GetDatabase();
                string fieldName = pageName.IsNullOrEmpty() ? $"{key}" : $"{key}_{pageName}";
                iDatabase.HashDelete(
                    context.SessionGuid,
                    fieldName);
            }
            else
            {
                SessionUtilities.Set(
                context: context,
                key: key,
                value: value,
                page: true);
            }
        }

        public List<int> SwitchTargets;

        public TenantModel()
        {
        }

        public TenantModel(
            Context context,
            SiteSettings ss,
            Dictionary<string, string> formData = null,
            TenantApiModel tenantApiModel = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            TenantId = context.TenantId;
            if (formData != null)
            {
                SetByForm(
                    context: context,
                    ss: ss,
                    formData: formData);
            }
            if (tenantApiModel != null)
            {
                SetByApi(context: context, ss: ss, data: tenantApiModel);
            }
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public TenantModel(
            Context context,
            SiteSettings ss,
            int tenantId,
            Dictionary<string, string> formData = null,
            TenantApiModel tenantApiModel = null,
            SqlColumnCollection column = null,
            bool clearSessions = false,
            List<int> switchTargets = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            TenantId = context.TenantId;
            if (context.QueryStrings.ContainsKey("ver"))
            {
                Get(
                    context: context,
                    tableType: Sqls.TableTypes.NormalAndHistory,
                    column: column,
                    where: Rds.TenantsWhereDefault(
                        context: context,
                        tenantModel: this)
                            .Tenants_Ver(context.QueryStrings.Int("ver")), ss: ss);
            }
            else
            {
                Get(
                    context: context,
                    ss: ss,
                    column: column);
            }
            if (clearSessions) ClearSessions(context: context);
            if (formData != null)
            {
                SetByForm(
                    context: context,
                    ss: ss,
                    formData: formData);
            }
            if (tenantApiModel != null)
            {
                SetByApi(context: context, ss: ss, data: tenantApiModel);
            }
            SwitchTargets = switchTargets;
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public TenantModel(
            Context context,
            SiteSettings ss,
            DataRow dataRow,
            Dictionary<string, string> formData = null,
            string tableAlias = null)
        {
            OnConstructing(context: context);
            TenantId = context.TenantId;
            if (dataRow != null)
            {
                Set(
                    context: context,
                    ss: ss,
                    dataRow: dataRow,
                    tableAlias: tableAlias);
            }
            if (formData != null)
            {
                SetByForm(
                    context: context,
                    ss: ss,
                    formData: formData);
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
            Session_TenantSettings(context: context, value: null);
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
            where = where ?? Rds.TenantsWhereDefault(
                context: context,
                tenantModel: this);
            column = (column ?? Rds.TenantsDefaultColumns());
            join = join ?? Rds.TenantsJoinDefault();
            Set(context, ss, Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectTenants(
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

        public TenantApiModel GetByApi(Context context, SiteSettings ss)
        {
            var data = new TenantApiModel()
            {
                ApiVersion = context.ApiVersion
            };
            ss.ReadableColumns(context: context, noJoined: true).ForEach(column =>
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
                    case "DisableAllUsersPermission": data.DisableAllUsersPermission = DisableAllUsersPermission; break;
                    case "DisableApi": data.DisableApi = DisableApi; break;
                    case "AllowExtensionsApi": data.AllowExtensionsApi = AllowExtensionsApi; break;
                    case "DisableStartGuide": data.DisableStartGuide = DisableStartGuide; break;
                    case "LogoType": data.LogoType = LogoType.ToInt(); break;
                    case "HtmlTitleTop": data.HtmlTitleTop = HtmlTitleTop; break;
                    case "HtmlTitleSite": data.HtmlTitleSite = HtmlTitleSite; break;
                    case "HtmlTitleRecord": data.HtmlTitleRecord = HtmlTitleRecord; break;
                    case "TopStyle": data.TopStyle = TopStyle; break;
                    case "TopScript": data.TopScript = TopScript; break;
                    case "TopDashboards": data.TopDashboards = TopDashboards; break;
                    case "Theme": data.Theme = Theme; break;
                    case "Language": data.Language = Language; break;
                    case "TimeZone": data.TimeZone = TimeZone; break;
                    case "TenantSettings": data.TenantSettings = TenantSettings.RecordingJson(context: context); break;
                    case "Creator": data.Creator = Creator.Id; break;
                    case "Updator": data.Updator = Updator.Id; break;
                    case "CreatedTime": data.CreatedTime = CreatedTime.Value.ToLocal(context: context); break;
                    case "UpdatedTime": data.UpdatedTime = UpdatedTime.Value.ToLocal(context: context); break;
                    case "Comments": data.Comments = Comments.ToLocal(context: context).ToJson(); break;
                    default: 
                        data.Value(
                            context: context,
                            column: column,
                            value: GetValue(
                                context: context,
                                column: column,
                                toLocal: true));
                        break;
                }
            });
            return data;
        }

        public string ToDisplay(Context context, SiteSettings ss, Column column, List<string> mine)
        {
            if (!ss.ReadColumnAccessControls.Allowed(
                context: context,
                ss: ss,
                column: column,
                mine: mine))
            {
                return string.Empty;
            }
            switch (column.Name)
            {
                case "TenantName":
                    return TenantName.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Title":
                    return Title.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Body":
                    return Body.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "ContractDeadline":
                    return ContractDeadline.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "DisableAllUsersPermission":
                    return DisableAllUsersPermission.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "DisableApi":
                    return DisableApi.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "AllowExtensionsApi":
                    return AllowExtensionsApi.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "DisableStartGuide":
                    return DisableStartGuide.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "HtmlTitleTop":
                    return HtmlTitleTop.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "HtmlTitleSite":
                    return HtmlTitleSite.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "HtmlTitleRecord":
                    return HtmlTitleRecord.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "TopStyle":
                    return TopStyle.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "TopScript":
                    return TopScript.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "TopDashboards":
                    return TopDashboards.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Theme":
                    return Theme.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Language":
                    return Language.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "TimeZone":
                    return TimeZone.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Timestamp":
                    return Timestamp.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Ver":
                    return Ver.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Comments":
                    return Comments.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Creator":
                    return Creator.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Updator":
                    return Updator.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "CreatedTime":
                    return CreatedTime.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "UpdatedTime":
                    return UpdatedTime.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                default:
                    switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                    {
                        case "Class":
                            return GetClass(columnName: column.Name).ToDisplay(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Num":
                            return GetNum(columnName: column.Name).ToDisplay(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Date":
                            return GetDate(columnName: column.Name).ToDisplay(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Description":
                            return GetDescription(columnName: column.Name).ToDisplay(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Check":
                            return GetCheck(columnName: column.Name).ToDisplay(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Attachments":
                            return GetAttachments(columnName: column.Name).ToDisplay(
                                context: context,
                                ss: ss,
                                column: column);
                        default:
                            return string.Empty;
                    }
            }
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
            TenantId = (response.Id ?? TenantId).ToInt();
            if (get) Get(context: context, ss: ss);
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
                Rds.InsertTenants(
                    dataTableName: dataTableName,
                    tableType: tableType,
                    selectIdentity: true,
                    param: param ?? Rds.TenantsParamDefault(
                        context: context,
                        ss: ss,
                        tenantModel: this,
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
                    id: TenantId);
            }
            if (get)
            {
                Get(context: context, ss: ss);
            }
            SetByAfterUpdateBackgroundServerScript(
                context: context,
                ss: ss);
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
            var where = Rds.TenantsWhereDefault(
                context: context,
                tenantModel: this)
                    .UpdatedTime(timestamp, _using: timestamp.InRange() && checkConflict);
            if (verUp)
            {
                statements.Add(Rds.TenantsCopyToStatement(
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
                Rds.UpdateTenants(
                    dataTableName: dataTableName,
                    where: where,
                    param: param ?? Rds.TenantsParamDefault(
                        context: context,
                        ss: ss,
                        tenantModel: this,
                        otherInitValue: otherInitValue)),
                new SqlStatement()
                {
                    DataTableName = dataTableName,
                    IfConflicted = true,
                    Id = TenantId
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
                Rds.UpdateOrInsertTenants(
                    where: where ?? Rds.TenantsWhereDefault(
                        context: context,
                        tenantModel: this),
                    param: param ?? Rds.TenantsParamDefault(
                        context: context,
                        ss: ss,
                        tenantModel: this,
                        setDefault: true))
            };
            var response = Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: statements.ToArray());
            TenantId = (response.Id ?? TenantId).ToInt();
            Get(context: context, ss: ss);
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData Delete(Context context, SiteSettings ss, bool notice = false)
        {
            var statements = new List<SqlStatement>();
            var where = Rds.TenantsWhere().TenantId(TenantId);
            statements.AddRange(new List<SqlStatement>
            {
                Rds.DeleteTenants(
                    factory: context,
                    where: where)
            });
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            var tenantHash = SiteInfo.TenantCaches.Get(context.TenantId)?.TenantHash;
            if (tenantHash.Keys.Contains(TenantId))
            {
                tenantHash.Remove(TenantId);
            }
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData Restore(Context context, SiteSettings ss,int tenantId)
        {
            TenantId = tenantId;
            Repository.ExecuteNonQuery(
                context: context,
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreTenants(
                        factory: context,
                        where: Rds.TenantsWhere().TenantId(TenantId))
                });
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData PhysicalDelete(
            Context context, SiteSettings ss,Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: Rds.PhysicalDeleteTenants(
                    tableType: tableType,
                    where: Rds.TenantsWhere().TenantId(TenantId)));
            return new ErrorData(type: Error.Types.None);
        }

        public void SetByForm(
            Context context,
            SiteSettings ss,
            Dictionary<string, string> formData)
        {
            SetByFormData(
                context: context,
                ss: ss,
                formData: formData);
            if (context.QueryStrings.ContainsKey("ver"))
            {
                Ver = context.QueryStrings.Int("ver");
            }
            if (context.Action == "deletecomment")
            {
                DeleteCommentId = formData.Get("ControlId")?
                    .Split(',')
                    ._2nd()
                    .ToInt() ?? 0;
                Comments.RemoveAll(o => o.CommentId == DeleteCommentId);
            }
        }

        private void SetByFormData(Context context, SiteSettings ss, Dictionary<string, string> formData)
        {
            formData.ForEach(data =>
            {
                var key = data.Key;
                var value = data.Value ?? string.Empty;
                switch (key)
                {
                    case "Tenants_TenantName": TenantName = value.ToString(); break;
                    case "Tenants_Title": Title = new Title(TenantId, value); break;
                    case "Tenants_Body": Body = value.ToString(); break;
                    case "Tenants_ContractDeadline": ContractDeadline = value.ToDateTime().ToUniversal(context: context); break;
                    case "Tenants_DisableAllUsersPermission": DisableAllUsersPermission = value.ToBool(); break;
                    case "Tenants_DisableApi": DisableApi = value.ToBool(); break;
                    case "Tenants_AllowExtensionsApi": AllowExtensionsApi = value.ToBool(); break;
                    case "Tenants_DisableStartGuide": DisableStartGuide = value.ToBool(); break;
                    case "Tenants_LogoType": LogoType = (LogoTypes)value.ToInt(); break;
                    case "Tenants_HtmlTitleTop": HtmlTitleTop = value.ToString(); break;
                    case "Tenants_HtmlTitleSite": HtmlTitleSite = value.ToString(); break;
                    case "Tenants_HtmlTitleRecord": HtmlTitleRecord = value.ToString(); break;
                    case "Tenants_TopStyle": TopStyle = value.ToString(); break;
                    case "Tenants_TopScript": TopScript = value.ToString(); break;
                    case "Tenants_TopDashboards": TopDashboards = $"[{value.ToLong()}]"; break;
                    case "Tenants_Theme": Theme = value.ToString(); break;
                    case "Tenants_Language": Language = value.ToString(); break;
                    case "Tenants_TimeZone": TimeZone = value.ToString(); break;
                    case "Tenants_Timestamp": Timestamp = value.ToString(); break;
                    case "Comments": Comments.Prepend(
                        context: context,
                        ss: ss,
                        body: value); break;
                    case "VerUp": VerUp = value.ToBool(); break;
                    default:
                        if (key.RegexExists("Comment[0-9]+"))
                        {
                            Comments.Update(
                                context: context,
                                ss: ss,
                                commentId: key.Substring("Comment".Length).ToInt(),
                                body: value);
                        }
                        else
                        {
                            var column = ss.GetColumn(
                                context: context,
                                columnName: key.Split_2nd('_'));
                            switch (Def.ExtendedColumnTypes.Get(column?.ColumnName ?? string.Empty))
                            {
                                case "Class":
                                    SetClass(
                                        columnName: column.ColumnName,
                                        value: value);
                                    break;
                                case "Num":
                                    SetNum(
                                        columnName: column.ColumnName,
                                        value: new Num(
                                            context: context,
                                            column: column,
                                            value: value));
                                    break;
                                case "Date":
                                    SetDate(
                                        columnName: column.ColumnName,
                                        value: value.ToDateTime().ToUniversal(context: context));
                                    break;
                                case "Description":
                                    SetDescription(
                                        columnName: column.ColumnName,
                                        value: Implem.Pleasanter.Models.BinaryUtilities.NormalizeFormBinaryPath(context, value.ToString()));
                                    break;
                                case "Check":
                                    SetCheck(
                                        columnName: column.ColumnName,
                                        value: value.ToBool());
                                    break;
                                case "Attachments":
                                    SetAttachments(
                                        columnName: column.ColumnName,
                                        value: value.Deserialize<Attachments>());
                                    break;
                            }
                        }
                        break;
                }
            });
        }

        public void SetByModel(TenantModel tenantModel)
        {
            TenantName = tenantModel.TenantName;
            Title = tenantModel.Title;
            Body = tenantModel.Body;
            ContractSettings = tenantModel.ContractSettings;
            ContractDeadline = tenantModel.ContractDeadline;
            DisableAllUsersPermission = tenantModel.DisableAllUsersPermission;
            DisableApi = tenantModel.DisableApi;
            AllowExtensionsApi = tenantModel.AllowExtensionsApi;
            DisableStartGuide = tenantModel.DisableStartGuide;
            LogoType = tenantModel.LogoType;
            HtmlTitleTop = tenantModel.HtmlTitleTop;
            HtmlTitleSite = tenantModel.HtmlTitleSite;
            HtmlTitleRecord = tenantModel.HtmlTitleRecord;
            TopStyle = tenantModel.TopStyle;
            TopScript = tenantModel.TopScript;
            TopDashboards = tenantModel.TopDashboards;
            Theme = tenantModel.Theme;
            Language = tenantModel.Language;
            TimeZone = tenantModel.TimeZone;
            TenantSettings = tenantModel.TenantSettings;
            Comments = tenantModel.Comments;
            Creator = tenantModel.Creator;
            Updator = tenantModel.Updator;
            CreatedTime = tenantModel.CreatedTime;
            UpdatedTime = tenantModel.UpdatedTime;
            VerUp = tenantModel.VerUp;
            Comments = tenantModel.Comments;
            ClassHash = tenantModel.ClassHash;
            NumHash = tenantModel.NumHash;
            DateHash = tenantModel.DateHash;
            DescriptionHash = tenantModel.DescriptionHash;
            CheckHash = tenantModel.CheckHash;
            AttachmentsHash = tenantModel.AttachmentsHash;
        }

        public void SetByApi(Context context, SiteSettings ss, TenantApiModel data)
        {
            if (data.TenantName != null) TenantName = data.TenantName.ToString().ToString();
            if (data.Title != null) Title = new Title(data.Title.ToString());
            if (data.Body != null) Body = data.Body.ToString().ToString();
            if (data.ContractDeadline != null) ContractDeadline = data.ContractDeadline.ToDateTime().ToDateTime().ToUniversal(context: context);
            if (data.DisableAllUsersPermission != null) DisableAllUsersPermission = data.DisableAllUsersPermission.ToBool().ToBool();
            if (data.DisableApi != null) DisableApi = data.DisableApi.ToBool().ToBool();
            if (data.AllowExtensionsApi != null) AllowExtensionsApi = data.AllowExtensionsApi.ToBool().ToBool();
            if (data.DisableStartGuide != null) DisableStartGuide = data.DisableStartGuide.ToBool().ToBool();
            if (data.LogoType != null) LogoType = (LogoTypes)data.LogoType.ToInt().ToInt();
            if (data.HtmlTitleTop != null) HtmlTitleTop = data.HtmlTitleTop.ToString().ToString();
            if (data.HtmlTitleSite != null) HtmlTitleSite = data.HtmlTitleSite.ToString().ToString();
            if (data.HtmlTitleRecord != null) HtmlTitleRecord = data.HtmlTitleRecord.ToString().ToString();
            if (data.TopStyle != null) TopStyle = data.TopStyle.ToString().ToString();
            if (data.TopScript != null) TopScript = data.TopScript.ToString().ToString();
            if (data.TopDashboards != null) TopDashboards = data.TopDashboards.ToString().ToString();
            if (data.Theme != null) Theme = data.Theme.ToString().ToString();
            if (data.Language != null) Language = data.Language.ToString().ToString();
            if (data.TimeZone != null) TimeZone = data.TimeZone.ToString().ToString();
            if (data.Comments != null) Comments.ClearAndSplitPrependByApi(context: context, ss: ss, body: data.Comments, update: AccessStatus == Databases.AccessStatuses.Selected);
            if (data.VerUp != null) VerUp = data.VerUp.ToBool();
            data.ClassHash?.ForEach(o => SetClass(
                columnName: o.Key,
                value: o.Value));
            data.NumHash?.ForEach(o => SetNum(
                columnName: o.Key,
                value: new Num(
                    context: context,
                    column: ss.GetColumn(
                        context: context,
                        columnName: o.Key),
                    value: o.Value.ToString())));
            data.DateHash?.ForEach(o => SetDate(
                columnName: o.Key,
                value: o.Value.ToDateTime().ToUniversal(context: context)));
            data.DescriptionHash?.ForEach(o => SetDescription(
                columnName: o.Key,
                value: o.Value));
            data.CheckHash?.ForEach(o => SetCheck(
                columnName: o.Key,
                value: o.Value));
            data.AttachmentsHash?.ForEach(o =>
            {
                string columnName = o.Key;
                Attachments newAttachments = o.Value;
                Attachments oldAttachments;
                if (columnName == "Attachments#Uploading")
                {
                    var kvp = AttachmentsHash
                        .FirstOrDefault(x => x.Value
                            .Any(att => att.Guid == newAttachments.FirstOrDefault()?.Guid?.Split_1st()));
                    columnName = kvp.Key;
                    oldAttachments = kvp.Value;
                    var column = ss.GetColumn(
                        context: context,
                        columnName: columnName);
                    if (column.OverwriteSameFileName == true)
                    {
                        var oldAtt = oldAttachments
                            .FirstOrDefault(att => att.Guid == newAttachments.FirstOrDefault()?.Guid?.Split_1st());
                        if (oldAtt != null)
                        {
                            oldAtt.Deleted = true;
                            oldAtt.Overwritten = true;
                        }
                    }
                    newAttachments.ForEach(att => att.Guid = att.Guid.Split_2nd());
                }
                else
                {
                    oldAttachments = AttachmentsHash.Get(columnName);
                }
                if (oldAttachments != null)
                {
                    var column = ss.GetColumn(
                        context: context,
                        columnName: columnName);
                    var newGuidSet = new HashSet<string>(newAttachments.Select(x => x.Guid).Distinct());
                    var newNameSet = new HashSet<string>(newAttachments.Select(x => x.Name).Distinct());
                    newAttachments.ForEach(newAttachment =>
                    {
                        newAttachment.AttachmentAction(
                            context: context,
                            column: column,
                            oldAttachments: oldAttachments);
                    });
                    if (column.OverwriteSameFileName == true)
                    {
                        newAttachments.AddRange(oldAttachments.
                            Where((oldvalue) =>
                                !newGuidSet.Contains(oldvalue.Guid) &&
                                !newNameSet.Contains(oldvalue.Name)));
                    }
                    else
                    {
                        newAttachments.AddRange(oldAttachments.
                            Where((oldvalue) => !newGuidSet.Contains(oldvalue.Guid)));
                    }
                }
                SetAttachments(columnName: columnName, value: newAttachments);
            });
        }

        public string ReplacedDisplayValues(
            Context context,
            SiteSettings ss,
            string value)
        {
            ss.IncludedColumns(value: value).ForEach(column =>
                value = value.Replace(
                    $"[{column.ColumnName}]",
                    ToDisplay(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: Mine(context: context))));
            value = ReplacedContextValues(context, value);
            return value;
        }

        private string ReplacedContextValues(Context context, string value)
        {
            var url = Locations.ItemEditAbsoluteUri(
                context: context,
                id: TenantId);
            var mailAddress = MailAddressUtilities.Get(
                context: context,
                userId: context.UserId);
            value = value
                .Replace("{Url}", url)
                .Replace("{LoginId}", context.User.LoginId)
                .Replace("{UserName}", context.User.Name)
                .Replace("{MailAddress}", mailAddress);
            return value;
        }

        private void SetBySession(Context context)
        {
            if (!context.Forms.Exists("Tenants_TenantSettings")) TenantSettings = Session_TenantSettings(context: context);
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
            foreach (DataColumn dataColumn in dataRow.Table.Columns)
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
                        case "DisableAllUsersPermission":
                            DisableAllUsersPermission = dataRow[column.ColumnName].ToBool();
                            SavedDisableAllUsersPermission = DisableAllUsersPermission;
                            break;
                        case "DisableApi":
                            DisableApi = dataRow[column.ColumnName].ToBool();
                            SavedDisableApi = DisableApi;
                            break;
                        case "AllowExtensionsApi":
                            AllowExtensionsApi = dataRow[column.ColumnName].ToBool();
                            SavedAllowExtensionsApi = AllowExtensionsApi;
                            break;
                        case "DisableStartGuide":
                            DisableStartGuide = dataRow[column.ColumnName].ToBool();
                            SavedDisableStartGuide = DisableStartGuide;
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
                        case "TopStyle":
                            TopStyle = dataRow[column.ColumnName].ToString();
                            SavedTopStyle = TopStyle;
                            break;
                        case "TopScript":
                            TopScript = dataRow[column.ColumnName].ToString();
                            SavedTopScript = TopScript;
                            break;
                        case "TopDashboards":
                            TopDashboards = dataRow[column.ColumnName].ToString();
                            SavedTopDashboards = TopDashboards;
                            break;
                        case "Theme":
                            Theme = dataRow[column.ColumnName].ToString();
                            SavedTheme = Theme;
                            break;
                        case "Language":
                            Language = dataRow[column.ColumnName].ToString();
                            SavedLanguage = Language;
                            break;
                        case "TimeZone":
                            TimeZone = dataRow[column.ColumnName].ToString();
                            SavedTimeZone = TimeZone;
                            break;
                        case "TenantSettings":
                            TenantSettings = GetTenantSettings(context: context, dataRow: dataRow);
                            SavedTenantSettings = TenantSettings.RecordingJson(context: context);
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
                || TenantId_Updated(context: context)
                || Ver_Updated(context: context)
                || TenantName_Updated(context: context)
                || Title_Updated(context: context)
                || Body_Updated(context: context)
                || ContractSettings_Updated(context: context)
                || ContractDeadline_Updated(context: context)
                || DisableAllUsersPermission_Updated(context: context)
                || DisableApi_Updated(context: context)
                || AllowExtensionsApi_Updated(context: context)
                || DisableStartGuide_Updated(context: context)
                || LogoType_Updated(context: context)
                || HtmlTitleTop_Updated(context: context)
                || HtmlTitleSite_Updated(context: context)
                || HtmlTitleRecord_Updated(context: context)
                || TopStyle_Updated(context: context)
                || TopScript_Updated(context: context)
                || TopDashboards_Updated(context: context)
                || Theme_Updated(context: context)
                || Language_Updated(context: context)
                || TimeZone_Updated(context: context)
                || TenantSettings_Updated(context: context)
                || Comments_Updated(context: context)
                || Creator_Updated(context: context)
                || Updator_Updated(context: context);
        }

        private bool UpdatedWithColumn(Context context, SiteSettings ss)
        {
            return ClassHash.Any(o => Class_Updated(
                    columnName: o.Key,
                    context: context,
                    column: ss.GetColumn(context: context, o.Key)))
                || NumHash.Any(o => Num_Updated(
                    columnName: o.Key,
                    context: context,
                    column: ss.GetColumn(context: context, o.Key)))
                || DateHash.Any(o => Date_Updated(
                    columnName: o.Key,
                    context: context,
                    column: ss.GetColumn(context: context, o.Key)))
                || DescriptionHash.Any(o => Description_Updated(
                    columnName: o.Key,
                    context: context,
                    column: ss.GetColumn(context: context, o.Key)))
                || CheckHash.Any(o => Check_Updated(
                    columnName: o.Key,
                    context: context,
                    column: ss.GetColumn(context: context, o.Key)))
                || AttachmentsHash.Any(o => Attachments_Updated(
                    columnName: o.Key,
                    context: context,
                    column: ss.GetColumn(context: context, o.Key)));
        }

        public bool Updated(Context context, SiteSettings ss)
        {
            return UpdatedWithColumn(context: context, ss: ss)
                || TenantId_Updated(context: context)
                || Ver_Updated(context: context)
                || TenantName_Updated(context: context)
                || Title_Updated(context: context)
                || Body_Updated(context: context)
                || ContractSettings_Updated(context: context)
                || ContractDeadline_Updated(context: context)
                || DisableAllUsersPermission_Updated(context: context)
                || DisableApi_Updated(context: context)
                || AllowExtensionsApi_Updated(context: context)
                || DisableStartGuide_Updated(context: context)
                || LogoType_Updated(context: context)
                || HtmlTitleTop_Updated(context: context)
                || HtmlTitleSite_Updated(context: context)
                || HtmlTitleRecord_Updated(context: context)
                || TopStyle_Updated(context: context)
                || TopScript_Updated(context: context)
                || TopDashboards_Updated(context: context)
                || Theme_Updated(context: context)
                || Language_Updated(context: context)
                || TimeZone_Updated(context: context)
                || TenantSettings_Updated(context: context)
                || Comments_Updated(context: context)
                || Creator_Updated(context: context)
                || Updator_Updated(context: context);
        }

        public override List<string> Mine(Context context)
        {
            if (MineCache == null)
            {
                var mine = new List<string>();
                var userId = context.UserId;
                if (SavedCreator == userId) mine.Add("Creator");
                if (SavedUpdator == userId) mine.Add("Updator");
                MineCache = mine;
            }
            return MineCache;
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

        /// <summary>
        /// Fixed:
        /// </summary>
        public string SetBGServerScript(
            Context context,
            bool setSiteSettingsPropertiesBySession = true)
        {
            SetBySession(context: context);
            var res = new TenantsResponseCollection(
                context: context,
                tenantModel: this);
            SetTenantSettings(context: context, res: res);
            Session_TenantSettings(
                context: context,
                value: TenantSettings.RecordingJson(context: context));
            return res
                .SetMemory("formChanged", true)
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetTenantSettings(Context context, ResponseCollection res)
        {
            var controlId = context.Forms.ControlId();
            switch (controlId)
            {
                case "ExecServerScript":
                    ExecServerScript(
                        context: context,
                        res: res,
                        scriptId: context.Forms.Int("ServerScriptId"));
                    break;
                case "MoveUpServerScripts":
                case "MoveDownServerScripts":
                    OpsServerScripts(
                        context: context,
                        res: res,
                        action: (selected, scripts) => scripts.MoveUpOrDown(ColumnUtilities.ChangeCommand(controlId), selected));
                    break;
                case "NewServerScript":
                case "EditServerScript":
                    OpenServerScriptDialog(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "AddServerScript":
                    AddServerScript(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "UpdateServerScript":
                    UpdateServerScript(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "CopyServerScripts":
                    OpsServerScripts(
                        context: context,
                        res: res,
                        action: (selected, scripts) => scripts.Copy(selected));
                    break;
                case "DeleteServerScripts":
                    OpsServerScripts(
                        context: context,
                        res: res,
                        action: (selected, scripts) => scripts.Delete(selected));
                    break;
                case "MoveUpServerScriptSchedules":
                case "MoveDownServerScriptSchedules":
                    OpsServerScriptSchedules(
                        context: context,
                        res: res,
                        action: (selected, schedules) => schedules.MoveUpOrDown(ColumnUtilities.ChangeCommand(controlId), selected));
                    break;
                case "NewServerScriptSchedules":
                case "EditServerScriptSchedules":
                    OpenServerScriptScheduleDialog(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "AddServerScriptSchedules":
                    AddServerScriptSchedules(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "UpdateServerScriptSchedules":
                    UpdateServerScriptSchedules(
                        context: context,
                        res: res,
                        controlId: controlId);
                    break;
                case "CopyServerScriptSchedules":
                    OpsServerScriptSchedules(
                        context: context,
                        res: res,
                        action: (selected, schedules) => schedules.Copy(selected));
                    break;
                case "DeleteServerScriptSchedules":
                    OpsServerScriptSchedules(
                        context: context,
                        res: res,
                        action: (selected, schedules) => schedules.Delete(selected));
                    break;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenServerScriptDialog(Context context, ResponseCollection res, string controlId)
        {
            if (controlId == "NewServerScript")
            {
                var script = new BackgroundServerScript();
                OpenServerScriptDialog(
                    context: context,
                    res: res,
                    script: script);
            }
            else
            {
                var script = TenantSettings.BackgroundServerScripts.Scripts?.Get(context.Forms.Int("ServerScriptId"));
                if (script == null)
                {
                    OpenDialogError(
                        res: res,
                        message: Messages.SelectOne(context: context),
                        target: "#ServerScriptDialog");
                }
                else
                {
                    OpenServerScriptDialog(
                        context: context,
                        res: res,
                        script: script);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenServerScriptDialog(Context context, ResponseCollection res, BackgroundServerScript script)
        {
            res.Html("#ServerScriptDialog", TenantUtilities.ServerScriptDialog(
                context: context,
                tenantModel: this,
                controlId: context.Forms.ControlId(),
                script: script));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void ExecServerScript(Context context, ResponseCollection res, int scriptId)
        {
            var script = GetBackgroundServerScriptFromForm(context);
            if (script.UserId == 0)
            {
                res.Message(Messages.PleaseInputData(context: context, data: Displays.ExecutionUser(context: context)));
                return;
            }
            else if (script.Shared == true)
            {
                res.Message(Messages.PleaseUncheck(context: context, data: Displays.Shared(context: context)));
                return;
            }
            script.Disabled = false;
            var backgroundServerScripts = TenantSettings.BackgroundServerScripts.ToJson().Deserialize<BackgroundServerScripts>();
            if (scriptId != 0) backgroundServerScripts.Scripts.Delete(new int[] { scriptId });
            backgroundServerScripts.Scripts.Add(script);
            _ = BackgroundServerScriptUtilities.ExecuteNow(backgroundServerScripts: backgroundServerScripts, scriptId: scriptId);
            res.Message(new Message()
            {
                Text = Displays.ProcessExecuted(context: context, data: script.Name),
                Css = "alert-success"
            });
            return;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddServerScript(Context context, ResponseCollection res, string controlId)
        {
            if (!context.HasPrivilege)
            {
                res.Message(Error.Types.HasNotPermission.Message(context: context));
                return;
            }
            var script = GetBackgroundServerScriptFromForm(context);
            var invalid = BackgroundServerScriptValidators.OnCreating(
                context: context,
                script: script);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    res.Message(invalid.Message(context: context));
                    return;
            }
            if (TenantSettings.BackgroundServerScripts == null)
            {
                TenantSettings.BackgroundServerScripts = new BackgroundServerScripts(context: context);
            }
            script.Id = TenantSettings.BackgroundServerScripts.Scripts.MaxOrDefault(o => o.Id) + 1;
            TenantSettings.BackgroundServerScripts.Scripts.Add(script);
            Session_TenantSettings(
                context: context,
                value: TenantSettings.RecordingJson(context: context));
            res
                .ReplaceAll("#EditServerScriptWrap", new HtmlBuilder()
                    .EditServerScript(
                        context: context,
                        tenantModel: this))
                .CloseDialog(target: "#ServerScriptDialog");
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateServerScript(Context context, ResponseCollection res, string controlId)
        {
            var script = GetBackgroundServerScriptFromForm(context);
            var invalid = BackgroundServerScriptValidators.OnUpdating(
                context: context,
                script: script);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    res.Message(invalid.Message(context: context));
                    return;
            }
            TenantSettings.BackgroundServerScripts?.Scripts?
                .FirstOrDefault(o => o.Id == script.Id)?
                .Update(
                    userId: script.UserId,
                    title: script.Title,
                    name: script.Name,
                    shared: script.Shared ?? default,
                    disabled: script.Disabled ?? default,
                    body: script.Body,
                    timeOut: script.TimeOut,
                    functionalize: script.Functionalize,
                    tryCatch: script.TryCatch,
                    backgoundSchedules: script.backgoundSchedules);
            res
                .ReplaceAll("#EditServerScriptWrap", new HtmlBuilder()
                    .EditServerScript(
                        context: context,
                        tenantModel: this))
                .CloseDialog(target: "#ServerScriptDialog");
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static BackgroundServerScript GetBackgroundServerScriptFromForm(Context context)
        {
            return new BackgroundServerScript(
                id: context.Forms.Int("ServerScriptId"),
                userId: context.Forms.Int("ServerScriptUser"),
                title: context.Forms.Data("ServerScriptTitle"),
                name: context.Forms.Data("ServerScriptName"),
                shared: context.Forms.Bool("ServerScriptShared"),
                disabled: context.Forms.Bool("ServerScriptDisabled"),
                body: context.Forms.Data("ServerScriptBody"),
                timeOut: context.Forms.Int("ServerScriptTimeOut"),
                functionalize: context.Forms.Bool("ServerScriptFunctionalize"),
                tryCatch: context.Forms.Bool("ServerScriptTryCatch"),
                backgoundSchedules: context.Forms.Data("BackgroundSchedule").Deserialize<IEnumerable<BackgroundSchedule>>());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpsServerScripts(Context context, ResponseCollection res, Action<List<int>, SettingList<BackgroundServerScript>> action)
        {
            var selected = context.Forms.IntList("EditServerScript");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                var scripts = TenantSettings.BackgroundServerScripts.Scripts;
                action(selected, scripts);
                Session_TenantSettings(
                    context: context,
                    value: TenantSettings.RecordingJson(context: context));
                res
                    .ReplaceAll("#EditServerScriptWrap", new HtmlBuilder()
                        .EditServerScript(
                            context: context,
                            tenantModel: this));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenServerScriptScheduleDialog(Context context, ResponseCollection res, string controlId)
        {
            var schedules = context.Forms.Data("BackgroundSchedule").Deserialize<SettingList<BackgroundSchedule>>();
            if (controlId == "NewServerScriptSchedules")
            {
                var schedule = new BackgroundSchedule(timeZoneId: context.TimeZoneInfo?.Id);
                OpenServerScriptScheduleDialog(
                    context: context,
                    res: res,
                    schedules: schedules,
                    schedule: schedule);
            }
            else
            {
                var schedule = schedules.Get(context.Forms.Int("ServerScriptScheduleId"));
                if (schedule == null)
                {
                    OpenDialogError(
                        res: res,
                        message: Messages.SelectOne(context: context),
                        target: "#ServerScriptScheduleDialog");
                }
                else
                {
                    OpenServerScriptScheduleDialog(
                        context: context,
                        res: res,
                        schedules: schedules,
                        schedule: schedule);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenServerScriptScheduleDialog(Context context, ResponseCollection res, SettingList<BackgroundSchedule> schedules, BackgroundSchedule schedule)
        {
            res.Html("#ServerScriptScheduleDialog", TenantUtilities.ServerScriptScheduleDialog(
                context: context,
                tenantModel: this,
                controlId: context.Forms.ControlId(),
                schedules: schedules,
                schedule: schedule));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void AddServerScriptSchedules(Context context, ResponseCollection res, string controlId)
        {
            var schedule = GetScheduleFromForm(context);
            var invalid = BackgroundScheduleValidators.OnCreating(
                context: context,
                schedule: schedule);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    res.Message(invalid.Message(context: context));
                    return;
            }
            var schedules = context.Forms.Data("EditServerScriptScheduleList").Deserialize<SettingList<BackgroundSchedule>>();
            schedule.Id = schedules.MaxOrDefault(o => o.Id) + 1;
            schedules.Add(schedule);
            res
                .ReplaceAll("#EditServerScriptSchedulesWrap", new HtmlBuilder()
                    .EditServerScriptSchedules(
                        context: context,
                        backgoundSchedules: schedules))
                .CloseDialog(target: "#ServerScriptScheduleDialog")
                .Val("#BackgroundSchedule", schedules.ToJson());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateServerScriptSchedules(Context context, ResponseCollection res, string controlId)
        {
            var schedule = GetScheduleFromForm(context);
            var invalid = BackgroundScheduleValidators.OnUpdating(
                context: context,
                schedule: schedule);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    res.Message(invalid.Message(context: context));
                    return;
            }
            var schedules = context.Forms.Data("EditServerScriptScheduleList").Deserialize<SettingList<BackgroundSchedule>>();
            schedules?
                .FirstOrDefault(o => o.Id == schedule.Id)?
                .UpdateFromRecode(schedule: schedule);
            res
                .ReplaceAll("#EditServerScriptSchedulesWrap", new HtmlBuilder()
                    .EditServerScriptSchedules(
                        context: context,
                        backgoundSchedules: schedules))
                .CloseDialog(target: "#ServerScriptScheduleDialog")
                .Val("#BackgroundSchedule", schedules.ToJson());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static BackgroundSchedule GetScheduleFromForm(Context context)
        {
            return new BackgroundSchedule(
                id: context.Forms.Int("ServerScriptScheduleId"),
                name: context.Forms.Data("ServerScriptScheduleName"),
                type: context.Forms.Data("ServerScriptScheduleType"),
                timeZoneId: context.Forms.Data("ServerScriptScheduleTimeZoneId"),
                dailyTime: context.Forms.Data("ServerScriptScheduleDailyTime"),
                hourlyTime: context.Forms.Data("ServerScriptScheduleHourlyTime"),
                weeklyWeek: context.Forms.Data("ServerScriptScheduleWeeklyWeek"),
                weeklyTime: context.Forms.Data("ServerScriptScheduleWeeklyTime"),
                monthlyMonth: context.Forms.Data("ServerScriptScheduleMonthlyMonth"),
                monthlyDay: context.Forms.Data("ServerScriptScheduleMonthlyDay"),
                monthlyTime: context.Forms.Data("ServerScriptScheduleMonthlyTime"),
                onlyOnceTime: context.Forms.Data("ServerScriptScheduleOnlyOnceTime"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpsServerScriptSchedules(
            Context context,
            ResponseCollection res,
            Action<List<int>, SettingList<BackgroundSchedule>> action)
        {
            var selected = context.Forms.IntList("EditServerScriptSchedules");
            if (selected?.Any() != true)
            {
                res.Message(Messages.SelectTargets(context: context)).ToJson();
            }
            else
            {
                var schedules = context.Forms.Data("BackgroundSchedule").Deserialize<SettingList<BackgroundSchedule>>();
                action(selected, schedules);
                res
                    .ReplaceAll("#EditServerScriptSchedulesWrap", new HtmlBuilder()
                    .EditServerScriptSchedules(
                        context: context,
                        backgoundSchedules: schedules))
                    .Val("#BackgroundSchedule", schedules.ToJson());
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OpenDialogError(ResponseCollection res, Message message, string target = null)
        {
            res
                .CloseDialog(target: target)
                .Message(message);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private TenantSettings GetTenantSettings(Context context, DataRow dataRow)
        {
            return TenantSettings.GetTenantSettings(context: context, value: dataRow.String("TenantSettings")) ?? new TenantSettings(context: context);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetByAfterUpdateBackgroundServerScript(Context context, SiteSettings ss)
        {
            BackgroundServerScriptUtilities.Reschedule(
                tenantId: TenantId,
                backgroundServerScripts: TenantSettings.BackgroundServerScripts);
        }
    }
}
