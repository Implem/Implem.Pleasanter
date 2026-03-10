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
using Implem.Pleasanter.Models.McpLogs;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class McpLogModel : BaseModel
    {
        private static readonly Logger logger = LogManager.GetLogger("mcplogs");
        public long McpLogId = 0;
        public DateTime StartTime = 0.ToDateTime();
        public DateTime EndTime = 0.ToDateTime();
        public string McpRequestId = string.Empty;
        public string McpSessionId = string.Empty;
        public string McpMethod = string.Empty;
        public string TargetName = string.Empty;
        public int TenantId = 0;
        public User UserId = new User();
        public string ApiKeyPrefix = string.Empty;
        public string ClientName = string.Empty;
        public string ClientVersion = string.Empty;
        public string ProtocolVersion = string.Empty;
        public double Elapsed = 0;
        public int Status = 200;
        public int JsonRpcErrorCode = 0;
        public string ErrMessage = string.Empty;
        public string RequestData = string.Empty;
        public string ResponseData = string.Empty;
        public string UserHostAddress = string.Empty;
        public string UserAgent = string.Empty;

        public Title Title
        {
            get
            {
                return new Title(McpLogId, McpLogId.ToString());
            }
        }

        public long SavedMcpLogId = 0;
        public DateTime SavedStartTime = 0.ToDateTime();
        public DateTime SavedEndTime = 0.ToDateTime();
        public string SavedMcpRequestId = string.Empty;
        public string SavedMcpSessionId = string.Empty;
        public string SavedMcpMethod = string.Empty;
        public string SavedTargetName = string.Empty;
        public int SavedTenantId = 0;
        public int SavedUserId = 0;
        public string SavedApiKeyPrefix = string.Empty;
        public string SavedClientName = string.Empty;
        public string SavedClientVersion = string.Empty;
        public string SavedProtocolVersion = string.Empty;
        public double SavedElapsed = 0;
        public int SavedStatus = 200;
        public int SavedJsonRpcErrorCode = 0;
        public string SavedErrMessage = string.Empty;
        public string SavedRequestData = string.Empty;
        public string SavedResponseData = string.Empty;
        public string SavedUserHostAddress = string.Empty;
        public string SavedUserAgent = string.Empty;

        public bool McpLogId_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToLong() != McpLogId;
            }
            return McpLogId != SavedMcpLogId
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToLong() != McpLogId);
        }

        public bool McpRequestId_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != McpRequestId;
            }
            return McpRequestId != SavedMcpRequestId && McpRequestId != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != McpRequestId);
        }

        public bool McpSessionId_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != McpSessionId;
            }
            return McpSessionId != SavedMcpSessionId && McpSessionId != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != McpSessionId);
        }

        public bool McpMethod_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != McpMethod;
            }
            return McpMethod != SavedMcpMethod && McpMethod != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != McpMethod);
        }

        public bool TargetName_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != TargetName;
            }
            return TargetName != SavedTargetName && TargetName != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != TargetName);
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

        public bool UserId_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToInt() != UserId.Id;
            }
            return UserId.Id != SavedUserId
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToInt() != UserId.Id);
        }

        public bool ApiKeyPrefix_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != ApiKeyPrefix;
            }
            return ApiKeyPrefix != SavedApiKeyPrefix && ApiKeyPrefix != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != ApiKeyPrefix);
        }

        public bool ClientName_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != ClientName;
            }
            return ClientName != SavedClientName && ClientName != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != ClientName);
        }

        public bool ClientVersion_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != ClientVersion;
            }
            return ClientVersion != SavedClientVersion && ClientVersion != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != ClientVersion);
        }

        public bool ProtocolVersion_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != ProtocolVersion;
            }
            return ProtocolVersion != SavedProtocolVersion && ProtocolVersion != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != ProtocolVersion);
        }

        public bool Elapsed_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToDouble() != Elapsed;
            }
            return Elapsed != SavedElapsed
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToDouble() != Elapsed);
        }

        public bool Status_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToInt() != Status;
            }
            return Status != SavedStatus
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToInt() != Status);
        }

        public bool JsonRpcErrorCode_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToInt() != JsonRpcErrorCode;
            }
            return JsonRpcErrorCode != SavedJsonRpcErrorCode
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToInt() != JsonRpcErrorCode);
        }

        public bool ErrMessage_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != ErrMessage;
            }
            return ErrMessage != SavedErrMessage && ErrMessage != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != ErrMessage);
        }

        public bool RequestData_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != RequestData;
            }
            return RequestData != SavedRequestData && RequestData != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != RequestData);
        }

        public bool ResponseData_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != ResponseData;
            }
            return ResponseData != SavedResponseData && ResponseData != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != ResponseData);
        }

        public bool UserHostAddress_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != UserHostAddress;
            }
            return UserHostAddress != SavedUserHostAddress && UserHostAddress != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != UserHostAddress);
        }

        public bool UserAgent_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != UserAgent;
            }
            return UserAgent != SavedUserAgent && UserAgent != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != UserAgent);
        }

        public bool StartTime_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToDateTime() != StartTime;
            }
            return StartTime != SavedStartTime
                && (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.DefaultTime(context: context).Date != StartTime.Date);
        }

        public bool EndTime_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToDateTime() != EndTime;
            }
            return EndTime != SavedEndTime
                && (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.DefaultTime(context: context).Date != EndTime.Date);
        }

        public string CsvData(
            Context context,
            SiteSettings ss,
            Column column,
            ExportColumn exportColumn,
            List<string> mine,
            bool? encloseDoubleQuotes)
        {
            var value = string.Empty;
            switch (column.Name)
            {
                case "McpLogId":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? McpLogId.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "StartTime":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? StartTime.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "EndTime":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? EndTime.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "McpRequestId":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? McpRequestId.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "McpSessionId":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? McpSessionId.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "McpMethod":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? McpMethod.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "TargetName":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? TargetName.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "TenantId":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? TenantId.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "UserId":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? UserId.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "ApiKeyPrefix":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? ApiKeyPrefix.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "ClientName":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? ClientName.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "ClientVersion":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? ClientVersion.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "ProtocolVersion":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? ProtocolVersion.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Elapsed":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? Elapsed.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Status":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? Status.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "JsonRpcErrorCode":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? JsonRpcErrorCode.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "ErrMessage":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? ErrMessage.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "RequestData":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? RequestData.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "ResponseData":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? ResponseData.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "UserHostAddress":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? UserHostAddress.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "UserAgent":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? UserAgent.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Ver":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? Ver.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Comments":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? Comments.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Updator":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? Updator.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Creator":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? Creator.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "CreatedTime":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? CreatedTime.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "UpdatedTime":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? UpdatedTime?.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                                    ?? String.Empty
                            : string.Empty;
                    break;
                default:
                    switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                    {
                        case "Class":
                            value = ss.ReadColumnAccessControls.Allowed(
                                context: context,
                                ss: ss,
                                column: column,
                                mine: mine)
                                    ? GetClass(columnName: column.Name).ToExport(
                                        context: context,
                                        column: column,
                                        exportColumn: exportColumn)
                                    : string.Empty;
                            break;
                        case "Num":
                            value = ss.ReadColumnAccessControls.Allowed(
                                context: context,
                                ss: ss,
                                column: column,
                                mine: mine)
                                    ? GetNum(columnName: column.Name).ToExport(
                                        context: context,
                                        column: column,
                                        exportColumn: exportColumn)
                                    : string.Empty;
                            break;
                        case "Date":
                            value = ss.ReadColumnAccessControls.Allowed(
                                context: context,
                                ss: ss,
                                column: column,
                                mine: mine)
                                    ? GetDate(columnName: column.Name).ToExport(
                                        context: context,
                                        column: column,
                                        exportColumn: exportColumn)
                                    : string.Empty;
                            break;
                        case "Description":
                            value = ss.ReadColumnAccessControls.Allowed(
                                context: context,
                                ss: ss,
                                column: column,
                                mine: mine)
                                    ? GetDescription(columnName: column.Name).ToExport(
                                        context: context,
                                        column: column,
                                        exportColumn: exportColumn)
                                    : string.Empty;
                            break;
                        case "Check":
                            value = ss.ReadColumnAccessControls.Allowed(
                                context: context,
                                ss: ss,
                                column: column,
                                mine: mine)
                                    ? GetCheck(columnName: column.Name).ToExport(
                                        context: context,
                                        column: column,
                                        exportColumn: exportColumn)
                                    : string.Empty;
                            break;
                        case "Attachments":
                            value = ss.ReadColumnAccessControls.Allowed(
                                context: context,
                                ss: ss,
                                column: column,
                                mine: mine)
                                    ? GetAttachments(columnName: column.Name).ToExport(
                                        context: context,
                                        column: column,
                                        exportColumn: exportColumn)
                                    : string.Empty;
                            break;
                        default: return string.Empty;
                    }
                    break;
            }
            return CsvUtilities.EncloseDoubleQuotes(
                value: value,
                encloseDoubleQuotes: encloseDoubleQuotes);
        }

        public List<long> SwitchTargets;

        public McpLogModel()
        {
        }

        public McpLogModel(
            Context context,
            SiteSettings ss,
            Dictionary<string, string> formData = null,
            McpLogApiModel mcpLogApiModel = null,
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
            if (mcpLogApiModel != null)
            {
                SetByApi(context: context, ss: ss, data: mcpLogApiModel);
            }
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public McpLogModel(
            Context context,
            SiteSettings ss,
            long mcpLogId,
            Dictionary<string, string> formData = null,
            McpLogApiModel mcpLogApiModel = null,
            SqlColumnCollection column = null,
            bool clearSessions = false,
            List<long> switchTargets = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            TenantId = context.TenantId;
            McpLogId = mcpLogId;
            if (context.QueryStrings.ContainsKey("ver"))
            {
                Get(
                    context: context,
                    tableType: Sqls.TableTypes.NormalAndHistory,
                    column: column,
                    where: Rds.McpLogsWhereDefault(
                        context: context,
                        mcpLogModel: this)
                            .McpLogs_Ver(context.QueryStrings.Int("ver")), ss: ss);
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
            if (mcpLogApiModel != null)
            {
                SetByApi(context: context, ss: ss, data: mcpLogApiModel);
            }
            SwitchTargets = switchTargets;
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public McpLogModel(
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
        }

        public McpLogModel Get(
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
            where = where ?? Rds.McpLogsWhereDefault(
                context: context,
                mcpLogModel: this);
            column = (column ?? Rds.McpLogsDefaultColumns());
            join = join ?? Rds.McpLogsJoinDefault();
            Set(context, ss, Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectMcpLogs(
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

        public McpLogApiModel GetByApi(Context context, SiteSettings ss)
        {
            var data = new McpLogApiModel()
            {
                ApiVersion = context.ApiVersion
            };
            ss.ReadableColumns(context: context, noJoined: true).ForEach(column =>
            {
                switch (column.ColumnName)
                {
                    case "McpLogId": data.McpLogId = McpLogId; break;
                    case "StartTime": data.StartTime = StartTime.ToLocal(context: context); break;
                    case "EndTime": data.EndTime = EndTime.ToLocal(context: context); break;
                    case "McpRequestId": data.McpRequestId = McpRequestId; break;
                    case "McpSessionId": data.McpSessionId = McpSessionId; break;
                    case "McpMethod": data.McpMethod = McpMethod; break;
                    case "TargetName": data.TargetName = TargetName; break;
                    case "TenantId": data.TenantId = TenantId; break;
                    case "UserId": data.UserId = UserId.Id; break;
                    case "ApiKeyPrefix": data.ApiKeyPrefix = ApiKeyPrefix; break;
                    case "ClientName": data.ClientName = ClientName; break;
                    case "ClientVersion": data.ClientVersion = ClientVersion; break;
                    case "ProtocolVersion": data.ProtocolVersion = ProtocolVersion; break;
                    case "Elapsed": data.Elapsed = Elapsed; break;
                    case "Status": data.Status = Status; break;
                    case "JsonRpcErrorCode": data.JsonRpcErrorCode = JsonRpcErrorCode; break;
                    case "ErrMessage": data.ErrMessage = ErrMessage; break;
                    case "RequestData": data.RequestData = RequestData; break;
                    case "ResponseData": data.ResponseData = ResponseData; break;
                    case "UserHostAddress": data.UserHostAddress = UserHostAddress; break;
                    case "UserAgent": data.UserAgent = UserAgent; break;
                    case "Ver": data.Ver = Ver; break;
                    case "Updator": data.Updator = Updator.Id; break;
                    case "UpdatedTime": data.UpdatedTime = UpdatedTime.Value.ToLocal(context: context); break;
                    case "Creator": data.Creator = Creator.Id; break;
                    case "CreatedTime": data.CreatedTime = CreatedTime.Value.ToLocal(context: context); break;
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

        public object ToApiDisplayValue(Context context, SiteSettings ss, Column column, List<string> mine)
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
                case "McpLogId":
                    return McpLogId.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "StartTime":
                    return StartTime.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "EndTime":
                    return EndTime.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "McpRequestId":
                    return McpRequestId.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "McpSessionId":
                    return McpSessionId.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "McpMethod":
                    return McpMethod.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "TargetName":
                    return TargetName.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "TenantId":
                    return TenantId.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "UserId":
                    return UserId.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "ApiKeyPrefix":
                    return ApiKeyPrefix.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "ClientName":
                    return ClientName.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "ClientVersion":
                    return ClientVersion.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "ProtocolVersion":
                    return ProtocolVersion.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Elapsed":
                    return Elapsed.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Status":
                    return Status.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "JsonRpcErrorCode":
                    return JsonRpcErrorCode.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "ErrMessage":
                    return ErrMessage.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "RequestData":
                    return RequestData.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "ResponseData":
                    return ResponseData.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "UserHostAddress":
                    return UserHostAddress.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "UserAgent":
                    return UserAgent.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Ver":
                    return Ver.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Title":
                    return Title.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Comments":
                    return Comments.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Updator":
                    return Updator.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "UpdatedTime":
                    return UpdatedTime.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Creator":
                    return Creator.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "CreatedTime":
                    return CreatedTime.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "VerUp":
                    return VerUp.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Timestamp":
                    return Timestamp.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                default:
                    switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                    {
                        case "Class":
                            return GetClass(columnName: column.Name).ToApiDisplayValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Num":
                            return GetNum(columnName: column.Name).ToApiDisplayValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Date":
                            return GetDate(columnName: column.Name).ToApiDisplayValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Description":
                            return GetDescription(columnName: column.Name).ToApiDisplayValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Check":
                            return GetCheck(columnName: column.Name).ToApiDisplayValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Attachments":
                            return GetAttachments(columnName: column.Name).ToApiDisplayValue(
                                context: context,
                                ss: ss,
                                column: column);
                        default:
                            return string.Empty;
                    }
            }
        }

        public object ToApiValue(Context context, SiteSettings ss, Column column, List<string> mine)
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
                case "McpLogId":
                    return McpLogId.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "StartTime":
                    return StartTime.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "EndTime":
                    return EndTime.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "McpRequestId":
                    return McpRequestId.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "McpSessionId":
                    return McpSessionId.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "McpMethod":
                    return McpMethod.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "TargetName":
                    return TargetName.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "TenantId":
                    return TenantId.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "UserId":
                    return UserId.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "ApiKeyPrefix":
                    return ApiKeyPrefix.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "ClientName":
                    return ClientName.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "ClientVersion":
                    return ClientVersion.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "ProtocolVersion":
                    return ProtocolVersion.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Elapsed":
                    return Elapsed.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Status":
                    return Status.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "JsonRpcErrorCode":
                    return JsonRpcErrorCode.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "ErrMessage":
                    return ErrMessage.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "RequestData":
                    return RequestData.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "ResponseData":
                    return ResponseData.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "UserHostAddress":
                    return UserHostAddress.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "UserAgent":
                    return UserAgent.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Ver":
                    return Ver.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Title":
                    return Title.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Comments":
                    return Comments.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Updator":
                    return Updator.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "UpdatedTime":
                    return UpdatedTime.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Creator":
                    return Creator.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "CreatedTime":
                    return CreatedTime.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "VerUp":
                    return VerUp.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Timestamp":
                    return Timestamp.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                default:
                    switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                    {
                        case "Class":
                            return GetClass(columnName: column.Name).ToApiValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Num":
                            return GetNum(columnName: column.Name).ToApiValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Date":
                            return GetDate(columnName: column.Name).ToApiValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Description":
                            return GetDescription(columnName: column.Name).ToApiValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Check":
                            return GetCheck(columnName: column.Name).ToApiValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Attachments":
                            return GetAttachments(columnName: column.Name).ToApiValue(
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
            McpLogId = (response.Id ?? McpLogId).ToLong();
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
                Rds.InsertMcpLogs(
                    dataTableName: dataTableName,
                    tableType: tableType,
                    selectIdentity: true,
                    param: param ?? Rds.McpLogsParamDefault(
                        context: context,
                        ss: ss,
                        mcpLogModel: this,
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
                    id: McpLogId);
            }
            if (get)
            {
                Get(context: context, ss: ss);
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
            var where = Rds.McpLogsWhereDefault(
                context: context,
                mcpLogModel: this)
                    .UpdatedTime(timestamp, _using: timestamp.InRange() && checkConflict);
            if (verUp)
            {
                statements.Add(Rds.McpLogsCopyToStatement(
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
                Rds.UpdateMcpLogs(
                    dataTableName: dataTableName,
                    where: where,
                    param: param ?? Rds.McpLogsParamDefault(
                        context: context,
                        ss: ss,
                        mcpLogModel: this,
                        otherInitValue: otherInitValue)),
                new SqlStatement()
                {
                    DataTableName = dataTableName,
                    IfConflicted = true,
                    Id = McpLogId
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
                Rds.UpdateOrInsertMcpLogs(
                    where: where ?? Rds.McpLogsWhereDefault(
                        context: context,
                        mcpLogModel: this),
                    param: param ?? Rds.McpLogsParamDefault(
                        context: context,
                        ss: ss,
                        mcpLogModel: this,
                        setDefault: true))
            };
            var response = Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: statements.ToArray());
            McpLogId = (response.Id ?? McpLogId).ToLong();
            Get(context: context, ss: ss);
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData Delete(Context context, SiteSettings ss, bool notice = false)
        {
            var statements = new List<SqlStatement>();
            var where = Rds.McpLogsWhere().McpLogId(McpLogId);
            statements.AddRange(new List<SqlStatement>
            {
                Rds.DeleteMcpLogs(
                    factory: context,
                    where: where)
            });
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData Restore(Context context, SiteSettings ss,long mcpLogId)
        {
            McpLogId = mcpLogId;
            Repository.ExecuteNonQuery(
                context: context,
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreMcpLogs(
                        factory: context,
                        where: Rds.McpLogsWhere().McpLogId(McpLogId))
                });
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData PhysicalDelete(
            Context context, SiteSettings ss,Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: Rds.PhysicalDeleteMcpLogs(
                    tableType: tableType,
                    where: Rds.McpLogsWhere().McpLogId(McpLogId)));
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
                    case "McpLogs_Timestamp": Timestamp = value.ToString(); break;
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

        public void SetByModel(McpLogModel mcpLogModel)
        {
            StartTime = mcpLogModel.StartTime;
            EndTime = mcpLogModel.EndTime;
            McpRequestId = mcpLogModel.McpRequestId;
            McpSessionId = mcpLogModel.McpSessionId;
            McpMethod = mcpLogModel.McpMethod;
            TargetName = mcpLogModel.TargetName;
            TenantId = mcpLogModel.TenantId;
            UserId = mcpLogModel.UserId;
            ApiKeyPrefix = mcpLogModel.ApiKeyPrefix;
            ClientName = mcpLogModel.ClientName;
            ClientVersion = mcpLogModel.ClientVersion;
            ProtocolVersion = mcpLogModel.ProtocolVersion;
            Elapsed = mcpLogModel.Elapsed;
            Status = mcpLogModel.Status;
            JsonRpcErrorCode = mcpLogModel.JsonRpcErrorCode;
            ErrMessage = mcpLogModel.ErrMessage;
            RequestData = mcpLogModel.RequestData;
            ResponseData = mcpLogModel.ResponseData;
            UserHostAddress = mcpLogModel.UserHostAddress;
            UserAgent = mcpLogModel.UserAgent;
            Comments = mcpLogModel.Comments;
            Updator = mcpLogModel.Updator;
            UpdatedTime = mcpLogModel.UpdatedTime;
            Creator = mcpLogModel.Creator;
            CreatedTime = mcpLogModel.CreatedTime;
            VerUp = mcpLogModel.VerUp;
            Comments = mcpLogModel.Comments;
            ClassHash = mcpLogModel.ClassHash;
            NumHash = mcpLogModel.NumHash;
            DateHash = mcpLogModel.DateHash;
            DescriptionHash = mcpLogModel.DescriptionHash;
            CheckHash = mcpLogModel.CheckHash;
            AttachmentsHash = mcpLogModel.AttachmentsHash;
        }

        public void SetByApi(Context context, SiteSettings ss, McpLogApiModel data)
        {
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
                id: McpLogId);
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
                        case "McpLogId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                McpLogId = dataRow[column.ColumnName].ToLong();
                                SavedMcpLogId = McpLogId;
                            }
                            break;
                        case "StartTime":
                            StartTime = dataRow[column.ColumnName].ToDateTime();
                            SavedStartTime = StartTime;
                            break;
                        case "EndTime":
                            EndTime = dataRow[column.ColumnName].ToDateTime();
                            SavedEndTime = EndTime;
                            break;
                        case "McpRequestId":
                            McpRequestId = dataRow[column.ColumnName].ToString();
                            SavedMcpRequestId = McpRequestId;
                            break;
                        case "McpSessionId":
                            McpSessionId = dataRow[column.ColumnName].ToString();
                            SavedMcpSessionId = McpSessionId;
                            break;
                        case "McpMethod":
                            McpMethod = dataRow[column.ColumnName].ToString();
                            SavedMcpMethod = McpMethod;
                            break;
                        case "TargetName":
                            TargetName = dataRow[column.ColumnName].ToString();
                            SavedTargetName = TargetName;
                            break;
                        case "TenantId":
                            TenantId = dataRow[column.ColumnName].ToInt();
                            SavedTenantId = TenantId;
                            break;
                        case "UserId":
                            UserId = SiteInfo.User(context: context, userId: dataRow.Int(column.ColumnName));
                            SavedUserId = UserId.Id;
                            break;
                        case "ApiKeyPrefix":
                            ApiKeyPrefix = dataRow[column.ColumnName].ToString();
                            SavedApiKeyPrefix = ApiKeyPrefix;
                            break;
                        case "ClientName":
                            ClientName = dataRow[column.ColumnName].ToString();
                            SavedClientName = ClientName;
                            break;
                        case "ClientVersion":
                            ClientVersion = dataRow[column.ColumnName].ToString();
                            SavedClientVersion = ClientVersion;
                            break;
                        case "ProtocolVersion":
                            ProtocolVersion = dataRow[column.ColumnName].ToString();
                            SavedProtocolVersion = ProtocolVersion;
                            break;
                        case "Elapsed":
                            Elapsed = dataRow[column.ColumnName].ToDouble();
                            SavedElapsed = Elapsed;
                            break;
                        case "Status":
                            Status = dataRow[column.ColumnName].ToInt();
                            SavedStatus = Status;
                            break;
                        case "JsonRpcErrorCode":
                            JsonRpcErrorCode = dataRow[column.ColumnName].ToInt();
                            SavedJsonRpcErrorCode = JsonRpcErrorCode;
                            break;
                        case "ErrMessage":
                            ErrMessage = dataRow[column.ColumnName].ToString();
                            SavedErrMessage = ErrMessage;
                            break;
                        case "RequestData":
                            RequestData = dataRow[column.ColumnName].ToString();
                            SavedRequestData = RequestData;
                            break;
                        case "ResponseData":
                            ResponseData = dataRow[column.ColumnName].ToString();
                            SavedResponseData = ResponseData;
                            break;
                        case "UserHostAddress":
                            UserHostAddress = dataRow[column.ColumnName].ToString();
                            SavedUserHostAddress = UserHostAddress;
                            break;
                        case "UserAgent":
                            UserAgent = dataRow[column.ColumnName].ToString();
                            SavedUserAgent = UserAgent;
                            break;
                        case "Ver":
                            Ver = dataRow[column.ColumnName].ToInt();
                            SavedVer = Ver;
                            break;
                        case "Comments":
                            Comments = dataRow[column.ColumnName].ToString().Deserialize<Comments>() ?? new Comments();
                            SavedComments = Comments.ToJson();
                            break;
                        case "Updator":
                            Updator = SiteInfo.User(context: context, userId: dataRow.Int(column.ColumnName));
                            SavedUpdator = Updator.Id;
                            break;
                        case "UpdatedTime":
                            UpdatedTime = new Time(context, dataRow, column.ColumnName); Timestamp = dataRow.Field<DateTime>(column.ColumnName).ToString("yyyy/M/d H:m:s.fff");
                            SavedUpdatedTime = UpdatedTime.Value;
                            break;
                        case "Creator":
                            Creator = SiteInfo.User(context: context, userId: dataRow.Int(column.ColumnName));
                            SavedCreator = Creator.Id;
                            break;
                        case "CreatedTime":
                            CreatedTime = new Time(context, dataRow, column.ColumnName);
                            SavedCreatedTime = CreatedTime.Value;
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
                || McpLogId_Updated(context: context)
                || StartTime_Updated(context: context)
                || EndTime_Updated(context: context)
                || McpRequestId_Updated(context: context)
                || McpSessionId_Updated(context: context)
                || McpMethod_Updated(context: context)
                || TargetName_Updated(context: context)
                || TenantId_Updated(context: context)
                || UserId_Updated(context: context)
                || ApiKeyPrefix_Updated(context: context)
                || ClientName_Updated(context: context)
                || ClientVersion_Updated(context: context)
                || ProtocolVersion_Updated(context: context)
                || Elapsed_Updated(context: context)
                || Status_Updated(context: context)
                || JsonRpcErrorCode_Updated(context: context)
                || ErrMessage_Updated(context: context)
                || RequestData_Updated(context: context)
                || ResponseData_Updated(context: context)
                || UserHostAddress_Updated(context: context)
                || UserAgent_Updated(context: context)
                || Ver_Updated(context: context)
                || Comments_Updated(context: context)
                || Updator_Updated(context: context)
                || Creator_Updated(context: context);
        }

        private bool UpdatedWithColumn(Context context, SiteSettings ss, bool paramDefault = false)
        {
            return ClassHash.Any(o => Class_Updated(
                    columnName: o.Key,
                    context: context,
                    column: ss.GetColumn(context: context, o.Key),
                    paramDefault: paramDefault))
                || NumHash.Any(o => Num_Updated(
                    columnName: o.Key,
                    context: context,
                    column: ss.GetColumn(context: context, o.Key),
                    paramDefault: paramDefault))
                || DateHash.Any(o => Date_Updated(
                    columnName: o.Key,
                    context: context,
                    column: ss.GetColumn(context: context, o.Key),
                    paramDefault: paramDefault))
                || DescriptionHash.Any(o => Description_Updated(
                    columnName: o.Key,
                    context: context,
                    column: ss.GetColumn(context: context, o.Key),
                    paramDefault: paramDefault))
                || CheckHash.Any(o => Check_Updated(
                    columnName: o.Key,
                    context: context,
                    column: ss.GetColumn(context: context, o.Key),
                    paramDefault: paramDefault))
                || AttachmentsHash.Any(o => Attachments_Updated(
                    columnName: o.Key,
                    context: context,
                    column: ss.GetColumn(context: context, o.Key)));
        }

        public bool Updated(Context context, SiteSettings ss, bool paramDefault = false)
        {
            return UpdatedWithColumn(context: context, ss: ss, paramDefault: paramDefault)
                || McpLogId_Updated(context: context)
                || StartTime_Updated(context: context)
                || EndTime_Updated(context: context)
                || McpRequestId_Updated(context: context)
                || McpSessionId_Updated(context: context)
                || McpMethod_Updated(context: context)
                || TargetName_Updated(context: context)
                || TenantId_Updated(context: context)
                || UserId_Updated(context: context)
                || ApiKeyPrefix_Updated(context: context)
                || ClientName_Updated(context: context)
                || ClientVersion_Updated(context: context)
                || ProtocolVersion_Updated(context: context)
                || Elapsed_Updated(context: context)
                || Status_Updated(context: context)
                || JsonRpcErrorCode_Updated(context: context)
                || ErrMessage_Updated(context: context)
                || RequestData_Updated(context: context)
                || ResponseData_Updated(context: context)
                || UserHostAddress_Updated(context: context)
                || UserAgent_Updated(context: context)
                || Ver_Updated(context: context)
                || Comments_Updated(context: context)
                || Updator_Updated(context: context)
                || Creator_Updated(context: context);
        }

        public override List<string> Mine(Context context)
        {
            if (MineCache == null)
            {
                var mine = new List<string>();
                var userId = context.UserId;
                if (SavedUpdator == userId) mine.Add("Updator");
                if (SavedCreator == userId) mine.Add("Creator");
                MineCache = mine;
            }
            return MineCache;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public void WriteMcpLog(Context context)
        {
            if (NotLoggingIp(UserHostAddress))
            {
                return;
            }
            if (Parameters.McpServer?.Logging?.EnableLoggingToDatabase == true)
            {
                try
                {
                    McpLogId = Repository.ExecuteScalar_response(
                        context: context,
                        selectIdentity: true,
                        statements: Rds.InsertMcpLogs(
                            selectIdentity: true,
                            param: McpLogParam(context: context)))
                                .Id.ToLong();
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "McpLog DB save failed. Fallback to file.");
                    WriteToNLog(LogLevel.Info);
                    return;
                }
            }
            if (Parameters.McpServer?.Logging?.EnableLoggingToFile == true)
            {
                WriteToNLog(LogLevel.Info);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void WriteToNLog(LogLevel logLevel)
        {
            logger.ForLogEvent(logLevel)
                .Message("WriteMcpLog")
                .Property("mcplog", ToLogModel())
                .Log();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private SqlParamCollection McpLogParam(Context context)
        {
            return Rds.McpLogsParamDefault(
                context: context,
                ss: null,
                mcpLogModel: this,
                setDefault: true);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private bool NotLoggingIp(string ipAddress)
        {
            if (Parameters.McpServer?.Logging?.NotLoggingIp?.Any() != true)
            {
                return false;
            }
            return Parameters.McpServer.Logging.NotLoggingIp
                .Select(addr => IpRange.FromCidr(addr))
                .Any(range => range.InRange(ipAddress));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public McpLogLogModel ToLogModel()
        {
            return new McpLogLogModel
            {
                McpLogId = McpLogId,
                StartTime = StartTime,
                EndTime = EndTime,
                McpRequestId = McpRequestId,
                McpSessionId = McpSessionId,
                McpMethod = McpMethod,
                TargetName = TargetName,
                TenantId = TenantId,
                UserId = UserId?.Id ?? 0,
                ApiKeyPrefix = ApiKeyPrefix,
                ClientName = ClientName,
                ClientVersion = ClientVersion,
                ProtocolVersion = ProtocolVersion,
                Elapsed = Elapsed,
                Status = Status,
                JsonRpcErrorCode = JsonRpcErrorCode,
                ErrMessage = ErrMessage,
                RequestData = RequestData,
                ResponseData = ResponseData,
                UserHostAddress = UserHostAddress,
                UserAgent = UserAgent,
                Ver = Ver,
                Creator = Creator?.Id ?? 0,
                Updator = Updator?.Id ?? 0,
                CreatedTime = CreatedTime?.Value ?? DateTime.MinValue,
                UpdatedTime = UpdatedTime?.Value ?? DateTime.MinValue
            };
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public void Save(Context context)
        {
            if (NotLoggingIp(UserHostAddress))
            {
                return;
            }
            if (Parameters.McpServer?.Logging?.EnableLoggingToDatabase == true)
            {
                try
                {
                    Repository.ExecuteNonQuery(
                        context: context,
                        transactional: false,
                        writeSqlToDebugLog: false,
                        statements: Rds.InsertMcpLogs(
                            param: McpLogParam(context: context)));
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "McpLog DB save failed. Fallback to file.");
                    logger.ForLogEvent(LogLevel.Info)
                        .Message("McpLogFallback")
                        .Property("mcplog", ToLogModel())
                        .Log();
                }
            }
            if (Parameters.McpServer?.Logging?.EnableLoggingToFile == true)
            {
                var logLevel = Status >= 500 ? LogLevel.Error
                    : Status >= 400 ? LogLevel.Warn
                    : LogLevel.Info;
                logger.ForLogEvent(logLevel)
                    .Message("McpLog")
                    .Property("mcplog", ToLogModel())
                    .Log();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public class McpLogLogModel
        {
            public long McpLogId { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public string McpRequestId { get; set; }
            public string McpSessionId { get; set; }
            public string McpMethod { get; set; }
            public string TargetName { get; set; }
            public int TenantId { get; set; }
            public int UserId { get; set; }
            public string ApiKeyPrefix { get; set; }
            public string ClientName { get; set; }
            public string ClientVersion { get; set; }
            public string ProtocolVersion { get; set; }
            public double Elapsed { get; set; }
            public int Status { get; set; }
            public int JsonRpcErrorCode { get; set; }
            public string ErrMessage { get; set; }
            public string RequestData { get; set; }
            public string ResponseData { get; set; }
            public string UserHostAddress { get; set; }
            public string UserAgent { get; set; }
            public int Ver { get; set; }
            public int Creator { get; set; }
            public int Updator { get; set; }
            public DateTime CreatedTime { get; set; }
            public DateTime UpdatedTime { get; set; }
        }
    }
}
