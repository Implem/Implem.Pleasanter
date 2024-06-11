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
    public class SysLogModel : BaseModel
    {
        public long SysLogId = 0;
        public DateTime StartTime = 0.ToDateTime();
        public DateTime EndTime = 0.ToDateTime();
        public SysLogTypes SysLogType = (SysLogTypes)10;
        public bool OnAzure = false;
        public string MachineName = string.Empty;
        public string ServiceName = string.Empty;
        public string TenantName = string.Empty;
        public string Application = string.Empty;
        public string Class = string.Empty;
        public string Method = string.Empty;
        public bool Api = false;
        public long SiteId = 0;
        public long ReferenceId = 0;
        public string ReferenceType = string.Empty;
        public long Status = 0;
        public string Description = string.Empty;
        public string RequestData = string.Empty;
        public string HttpMethod = string.Empty;
        public int RequestSize = 0;
        public int ResponseSize = 0;
        public double Elapsed = 0;
        public double ApplicationAge = 0;
        public double ApplicationRequestInterval = 0;
        public double SessionAge = 0;
        public double SessionRequestInterval = 0;
        public long WorkingSet64 = 0;
        public long VirtualMemorySize64 = 0;
        public int ProcessId = 0;
        public string ProcessName = string.Empty;
        public int BasePriority = 0;
        public string Url = string.Empty;
        public string UrlReferer = string.Empty;
        public string UserHostName = string.Empty;
        public string UserHostAddress = string.Empty;
        public string UserLanguage = string.Empty;
        public string UserAgent = string.Empty;
        public string SessionGuid = string.Empty;
        public string ErrMessage = string.Empty;
        public string ErrStackTrace = string.Empty;
        public bool InDebug = false;
        public string AssemblyVersion = string.Empty;

        public Title Title
        {
            get
            {
                return new Title(SysLogId, SysLogId.ToString());
            }
        }

        public long SavedSysLogId = 0;
        public DateTime SavedStartTime = 0.ToDateTime();
        public DateTime SavedEndTime = 0.ToDateTime();
        public int SavedSysLogType = 10;
        public bool SavedOnAzure = false;
        public string SavedMachineName = string.Empty;
        public string SavedServiceName = string.Empty;
        public string SavedTenantName = string.Empty;
        public string SavedApplication = string.Empty;
        public string SavedClass = string.Empty;
        public string SavedMethod = string.Empty;
        public bool SavedApi = false;
        public long SavedSiteId = 0;
        public long SavedReferenceId = 0;
        public string SavedReferenceType = string.Empty;
        public long SavedStatus = 0;
        public string SavedDescription = string.Empty;
        public string SavedRequestData = string.Empty;
        public string SavedHttpMethod = string.Empty;
        public int SavedRequestSize = 0;
        public int SavedResponseSize = 0;
        public double SavedElapsed = 0;
        public double SavedApplicationAge = 0;
        public double SavedApplicationRequestInterval = 0;
        public double SavedSessionAge = 0;
        public double SavedSessionRequestInterval = 0;
        public long SavedWorkingSet64 = 0;
        public long SavedVirtualMemorySize64 = 0;
        public int SavedProcessId = 0;
        public string SavedProcessName = string.Empty;
        public int SavedBasePriority = 0;
        public string SavedUrl = string.Empty;
        public string SavedUrlReferer = string.Empty;
        public string SavedUserHostName = string.Empty;
        public string SavedUserHostAddress = string.Empty;
        public string SavedUserLanguage = string.Empty;
        public string SavedUserAgent = string.Empty;
        public string SavedSessionGuid = string.Empty;
        public string SavedErrMessage = string.Empty;
        public string SavedErrStackTrace = string.Empty;
        public bool SavedInDebug = false;
        public string SavedAssemblyVersion = string.Empty;

        public bool SysLogId_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToLong() != SysLogId;
            }
            return SysLogId != SavedSysLogId
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToLong() != SysLogId);
        }

        public bool SysLogType_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToInt() != SysLogType.ToInt();
            }
            return SysLogType.ToInt() != SavedSysLogType
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToInt() != SysLogType.ToInt());
        }

        public bool OnAzure_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToBool() != OnAzure;
            }
            return OnAzure != SavedOnAzure
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToBool() != OnAzure);
        }

        public bool MachineName_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != MachineName;
            }
            return MachineName != SavedMachineName && MachineName != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != MachineName);
        }

        public bool ServiceName_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != ServiceName;
            }
            return ServiceName != SavedServiceName && ServiceName != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != ServiceName);
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

        public bool Application_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != Application;
            }
            return Application != SavedApplication && Application != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != Application);
        }

        public bool Class_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != Class;
            }
            return Class != SavedClass && Class != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != Class);
        }

        public bool Method_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != Method;
            }
            return Method != SavedMethod && Method != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != Method);
        }

        public bool Api_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToBool() != Api;
            }
            return Api != SavedApi
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToBool() != Api);
        }

        public bool SiteId_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToLong() != SiteId;
            }
            return SiteId != SavedSiteId
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToLong() != SiteId);
        }

        public bool ReferenceId_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToLong() != ReferenceId;
            }
            return ReferenceId != SavedReferenceId
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToLong() != ReferenceId);
        }

        public bool ReferenceType_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != ReferenceType;
            }
            return ReferenceType != SavedReferenceType && ReferenceType != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != ReferenceType);
        }

        public bool Status_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToLong() != Status;
            }
            return Status != SavedStatus
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToLong() != Status);
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

        public bool HttpMethod_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != HttpMethod;
            }
            return HttpMethod != SavedHttpMethod && HttpMethod != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != HttpMethod);
        }

        public bool RequestSize_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToInt() != RequestSize;
            }
            return RequestSize != SavedRequestSize
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToInt() != RequestSize);
        }

        public bool ResponseSize_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToInt() != ResponseSize;
            }
            return ResponseSize != SavedResponseSize
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToInt() != ResponseSize);
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

        public bool ApplicationAge_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToDouble() != ApplicationAge;
            }
            return ApplicationAge != SavedApplicationAge
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToDouble() != ApplicationAge);
        }

        public bool ApplicationRequestInterval_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToDouble() != ApplicationRequestInterval;
            }
            return ApplicationRequestInterval != SavedApplicationRequestInterval
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToDouble() != ApplicationRequestInterval);
        }

        public bool SessionAge_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToDouble() != SessionAge;
            }
            return SessionAge != SavedSessionAge
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToDouble() != SessionAge);
        }

        public bool SessionRequestInterval_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToDouble() != SessionRequestInterval;
            }
            return SessionRequestInterval != SavedSessionRequestInterval
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToDouble() != SessionRequestInterval);
        }

        public bool WorkingSet64_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToLong() != WorkingSet64;
            }
            return WorkingSet64 != SavedWorkingSet64
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToLong() != WorkingSet64);
        }

        public bool VirtualMemorySize64_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToLong() != VirtualMemorySize64;
            }
            return VirtualMemorySize64 != SavedVirtualMemorySize64
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToLong() != VirtualMemorySize64);
        }

        public bool ProcessId_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToInt() != ProcessId;
            }
            return ProcessId != SavedProcessId
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToInt() != ProcessId);
        }

        public bool ProcessName_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != ProcessName;
            }
            return ProcessName != SavedProcessName && ProcessName != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != ProcessName);
        }

        public bool BasePriority_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToInt() != BasePriority;
            }
            return BasePriority != SavedBasePriority
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToInt() != BasePriority);
        }

        public bool Url_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != Url;
            }
            return Url != SavedUrl && Url != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != Url);
        }

        public bool UrlReferer_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != UrlReferer;
            }
            return UrlReferer != SavedUrlReferer && UrlReferer != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != UrlReferer);
        }

        public bool UserHostName_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != UserHostName;
            }
            return UserHostName != SavedUserHostName && UserHostName != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != UserHostName);
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

        public bool UserLanguage_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != UserLanguage;
            }
            return UserLanguage != SavedUserLanguage && UserLanguage != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != UserLanguage);
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

        public bool SessionGuid_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != SessionGuid;
            }
            return SessionGuid != SavedSessionGuid && SessionGuid != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != SessionGuid);
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

        public bool ErrStackTrace_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != ErrStackTrace;
            }
            return ErrStackTrace != SavedErrStackTrace && ErrStackTrace != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != ErrStackTrace);
        }

        public bool InDebug_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToBool() != InDebug;
            }
            return InDebug != SavedInDebug
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToBool() != InDebug);
        }

        public bool AssemblyVersion_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != AssemblyVersion;
            }
            return AssemblyVersion != SavedAssemblyVersion && AssemblyVersion != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != AssemblyVersion);
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
                case "SysLogId":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? SysLogId.ToExport(
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
                case "SysLogType":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? SysLogType.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "OnAzure":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? OnAzure.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "MachineName":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? MachineName.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "ServiceName":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? ServiceName.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "TenantName":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? TenantName.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Application":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? Application.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Class":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? Class.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Method":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? Method.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Api":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? Api.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "SiteId":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? SiteId.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "ReferenceId":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? ReferenceId.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "ReferenceType":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? ReferenceType.ToExport(
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
                case "Description":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? Description.ToExport(
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
                case "HttpMethod":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? HttpMethod.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "RequestSize":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? RequestSize.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "ResponseSize":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? ResponseSize.ToExport(
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
                case "ApplicationAge":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? ApplicationAge.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "ApplicationRequestInterval":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? ApplicationRequestInterval.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "SessionAge":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? SessionAge.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "SessionRequestInterval":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? SessionRequestInterval.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "WorkingSet64":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? WorkingSet64.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "VirtualMemorySize64":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? VirtualMemorySize64.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "ProcessId":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? ProcessId.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "ProcessName":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? ProcessName.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "BasePriority":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? BasePriority.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Url":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? Url.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "UrlReferer":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? UrlReferer.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "UserHostName":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? UserHostName.ToExport(
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
                case "UserLanguage":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? UserLanguage.ToExport(
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
                case "SessionGuid":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? SessionGuid.ToExport(
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
                case "ErrStackTrace":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? ErrStackTrace.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "InDebug":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? InDebug.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "AssemblyVersion":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? AssemblyVersion.ToExport(
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
                case "UpdatedTime":
                    UpdatedTime ??= new Time();
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? UpdatedTime.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
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

        public SysLogModel()
        {
        }

        public SysLogModel(
            Context context,
            SiteSettings ss,
            Dictionary<string, string> formData = null,
            SysLogApiModel sysLogApiModel = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            if (formData != null)
            {
                SetByForm(
                    context: context,
                    ss: ss,
                    formData: formData);
            }
            if (sysLogApiModel != null)
            {
                SetByApi(context: context, ss: ss, data: sysLogApiModel);
            }
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public SysLogModel(
            Context context,
            SiteSettings ss,
            long sysLogId,
            Dictionary<string, string> formData = null,
            SysLogApiModel sysLogApiModel = null,
            SqlColumnCollection column = null,
            bool clearSessions = false,
            List<long> switchTargets = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            SysLogId = sysLogId;
            if (context.QueryStrings.ContainsKey("ver"))
            {
                Get(
                    context: context,
                    tableType: Sqls.TableTypes.NormalAndHistory,
                    column: column,
                    where: Rds.SysLogsWhereDefault(
                        context: context,
                        sysLogModel: this)
                            .SysLogs_Ver(context.QueryStrings.Int("ver")), ss: ss);
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
            if (sysLogApiModel != null)
            {
                SetByApi(context: context, ss: ss, data: sysLogApiModel);
            }
            SwitchTargets = switchTargets;
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public SysLogModel(
            Context context,
            SiteSettings ss,
            DataRow dataRow,
            Dictionary<string, string> formData = null,
            string tableAlias = null)
        {
            OnConstructing(context: context);
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

        public SysLogModel Get(
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
            where = where ?? Rds.SysLogsWhereDefault(
                context: context,
                sysLogModel: this);
            column = (column ?? Rds.SysLogsDefaultColumns());
            join = join ?? Rds.SysLogsJoinDefault();
            Set(context, ss, Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectSysLogs(
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

        public SysLogApiModel GetByApi(Context context, SiteSettings ss)
        {
            var data = new SysLogApiModel()
            {
                ApiVersion = context.ApiVersion
            };
            ss.ReadableColumns(context: context, noJoined: true).ForEach(column =>
            {
                switch (column.ColumnName)
                {
                    case "CreatedTime": data.CreatedTime = CreatedTime.Value.ToLocal(context: context); break;
                    case "SysLogId": data.SysLogId = SysLogId; break;
                    case "Ver": data.Ver = Ver; break;
                    case "SysLogType": data.SysLogType = SysLogType.ToInt(); break;
                    case "OnAzure": data.OnAzure = OnAzure; break;
                    case "MachineName": data.MachineName = MachineName; break;
                    case "ServiceName": data.ServiceName = ServiceName; break;
                    case "TenantName": data.TenantName = TenantName; break;
                    case "Application": data.Application = Application; break;
                    case "Class": data.Class = Class; break;
                    case "Method": data.Method = Method; break;
                    case "Api": data.Api = Api; break;
                    case "SiteId": data.SiteId = SiteId; break;
                    case "ReferenceId": data.ReferenceId = ReferenceId; break;
                    case "ReferenceType": data.ReferenceType = ReferenceType; break;
                    case "Status": data.Status = Status; break;
                    case "Description": data.Description = Description; break;
                    case "RequestData": data.RequestData = RequestData; break;
                    case "HttpMethod": data.HttpMethod = HttpMethod; break;
                    case "RequestSize": data.RequestSize = RequestSize; break;
                    case "ResponseSize": data.ResponseSize = ResponseSize; break;
                    case "Elapsed": data.Elapsed = Elapsed; break;
                    case "ApplicationAge": data.ApplicationAge = ApplicationAge; break;
                    case "ApplicationRequestInterval": data.ApplicationRequestInterval = ApplicationRequestInterval; break;
                    case "SessionAge": data.SessionAge = SessionAge; break;
                    case "SessionRequestInterval": data.SessionRequestInterval = SessionRequestInterval; break;
                    case "WorkingSet64": data.WorkingSet64 = WorkingSet64; break;
                    case "VirtualMemorySize64": data.VirtualMemorySize64 = VirtualMemorySize64; break;
                    case "ProcessId": data.ProcessId = ProcessId; break;
                    case "ProcessName": data.ProcessName = ProcessName; break;
                    case "BasePriority": data.BasePriority = BasePriority; break;
                    case "Url": data.Url = Url; break;
                    case "UrlReferer": data.UrlReferer = UrlReferer; break;
                    case "UserHostName": data.UserHostName = UserHostName; break;
                    case "UserHostAddress": data.UserHostAddress = UserHostAddress; break;
                    case "UserLanguage": data.UserLanguage = UserLanguage; break;
                    case "UserAgent": data.UserAgent = UserAgent; break;
                    case "SessionGuid": data.SessionGuid = SessionGuid; break;
                    case "ErrMessage": data.ErrMessage = ErrMessage; break;
                    case "ErrStackTrace": data.ErrStackTrace = ErrStackTrace; break;
                    case "InDebug": data.InDebug = InDebug; break;
                    case "AssemblyVersion": data.AssemblyVersion = AssemblyVersion; break;
                    case "Creator": data.Creator = Creator.Id; break;
                    case "Updator": data.Updator = Updator.Id; break;
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
                case "SysLogId":
                    return SysLogId.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "StartTime":
                    return StartTime.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "EndTime":
                    return EndTime.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "SysLogType":
                    return SysLogType.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "OnAzure":
                    return OnAzure.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "MachineName":
                    return MachineName.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "ServiceName":
                    return ServiceName.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "TenantName":
                    return TenantName.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Application":
                    return Application.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Class":
                    return Class.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Method":
                    return Method.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Api":
                    return Api.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "RequestData":
                    return RequestData.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "HttpMethod":
                    return HttpMethod.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "RequestSize":
                    return RequestSize.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "ResponseSize":
                    return ResponseSize.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Elapsed":
                    return Elapsed.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "ApplicationAge":
                    return ApplicationAge.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "ApplicationRequestInterval":
                    return ApplicationRequestInterval.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "SessionAge":
                    return SessionAge.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "SessionRequestInterval":
                    return SessionRequestInterval.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "WorkingSet64":
                    return WorkingSet64.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "VirtualMemorySize64":
                    return VirtualMemorySize64.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "ProcessId":
                    return ProcessId.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "ProcessName":
                    return ProcessName.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "BasePriority":
                    return BasePriority.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Url":
                    return Url.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "UrlReferer":
                    return UrlReferer.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "UserHostName":
                    return UserHostName.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "UserHostAddress":
                    return UserHostAddress.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "UserLanguage":
                    return UserLanguage.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "UserAgent":
                    return UserAgent.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "SessionGuid":
                    return SessionGuid.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "ErrMessage":
                    return ErrMessage.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "ErrStackTrace":
                    return ErrStackTrace.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "InDebug":
                    return InDebug.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "AssemblyVersion":
                    return AssemblyVersion.ToDisplay(
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
                case "CreatedTime":
                    return CreatedTime.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "SysLogId":
                    return SysLogId.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Ver":
                    return Ver.ToApiDisplayValue(
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
                case "SysLogType":
                    return SysLogType.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "OnAzure":
                    return OnAzure.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "MachineName":
                    return MachineName.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "ServiceName":
                    return ServiceName.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "TenantName":
                    return TenantName.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Application":
                    return Application.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Class":
                    return Class.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Method":
                    return Method.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Api":
                    return Api.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "SiteId":
                    return SiteId.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "ReferenceId":
                    return ReferenceId.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "ReferenceType":
                    return ReferenceType.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Status":
                    return Status.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Description":
                    return Description.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "RequestData":
                    return RequestData.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "HttpMethod":
                    return HttpMethod.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "RequestSize":
                    return RequestSize.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "ResponseSize":
                    return ResponseSize.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Elapsed":
                    return Elapsed.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "ApplicationAge":
                    return ApplicationAge.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "ApplicationRequestInterval":
                    return ApplicationRequestInterval.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "SessionAge":
                    return SessionAge.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "SessionRequestInterval":
                    return SessionRequestInterval.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "WorkingSet64":
                    return WorkingSet64.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "VirtualMemorySize64":
                    return VirtualMemorySize64.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "ProcessId":
                    return ProcessId.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "ProcessName":
                    return ProcessName.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "BasePriority":
                    return BasePriority.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Url":
                    return Url.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "UrlReferer":
                    return UrlReferer.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "UserHostName":
                    return UserHostName.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "UserHostAddress":
                    return UserHostAddress.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "UserLanguage":
                    return UserLanguage.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Title":
                    return Title.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "UserAgent":
                    return UserAgent.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "SessionGuid":
                    return SessionGuid.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "ErrMessage":
                    return ErrMessage.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "ErrStackTrace":
                    return ErrStackTrace.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "InDebug":
                    return InDebug.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "AssemblyVersion":
                    return AssemblyVersion.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Comments":
                    return Comments.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Creator":
                    return Creator.ToApiDisplayValue(
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
                case "CreatedTime":
                    return CreatedTime.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "SysLogId":
                    return SysLogId.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Ver":
                    return Ver.ToApiValue(
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
                case "SysLogType":
                    return SysLogType.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "OnAzure":
                    return OnAzure.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "MachineName":
                    return MachineName.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "ServiceName":
                    return ServiceName.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "TenantName":
                    return TenantName.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Application":
                    return Application.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Class":
                    return Class.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Method":
                    return Method.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Api":
                    return Api.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "SiteId":
                    return SiteId.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "ReferenceId":
                    return ReferenceId.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "ReferenceType":
                    return ReferenceType.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Status":
                    return Status.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Description":
                    return Description.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "RequestData":
                    return RequestData.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "HttpMethod":
                    return HttpMethod.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "RequestSize":
                    return RequestSize.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "ResponseSize":
                    return ResponseSize.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Elapsed":
                    return Elapsed.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "ApplicationAge":
                    return ApplicationAge.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "ApplicationRequestInterval":
                    return ApplicationRequestInterval.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "SessionAge":
                    return SessionAge.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "SessionRequestInterval":
                    return SessionRequestInterval.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "WorkingSet64":
                    return WorkingSet64.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "VirtualMemorySize64":
                    return VirtualMemorySize64.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "ProcessId":
                    return ProcessId.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "ProcessName":
                    return ProcessName.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "BasePriority":
                    return BasePriority.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Url":
                    return Url.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "UrlReferer":
                    return UrlReferer.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "UserHostName":
                    return UserHostName.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "UserHostAddress":
                    return UserHostAddress.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "UserLanguage":
                    return UserLanguage.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Title":
                    return Title.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "UserAgent":
                    return UserAgent.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "SessionGuid":
                    return SessionGuid.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "ErrMessage":
                    return ErrMessage.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "ErrStackTrace":
                    return ErrStackTrace.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "InDebug":
                    return InDebug.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "AssemblyVersion":
                    return AssemblyVersion.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Comments":
                    return Comments.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Creator":
                    return Creator.ToApiValue(
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
            SysLogId = (response.Id ?? SysLogId).ToLong();
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
                Rds.InsertSysLogs(
                    dataTableName: dataTableName,
                    tableType: tableType,
                    selectIdentity: true,
                    param: param ?? Rds.SysLogsParamDefault(
                        context: context,
                        ss: ss,
                        sysLogModel: this,
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
                    id: SysLogId);
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
            var where = Rds.SysLogsWhereDefault(
                context: context,
                sysLogModel: this)
                    .UpdatedTime(timestamp, _using: timestamp.InRange() && checkConflict);
            if (verUp)
            {
                statements.Add(Rds.SysLogsCopyToStatement(
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
                Rds.UpdateSysLogs(
                    dataTableName: dataTableName,
                    where: where,
                    param: param ?? Rds.SysLogsParamDefault(
                        context: context,
                        ss: ss,
                        sysLogModel: this,
                        otherInitValue: otherInitValue)),
                new SqlStatement(Def.Sql.IfConflicted.Params(SysLogId))
                {
                    DataTableName = dataTableName,
                    IfConflicted = true,
                    Id = SysLogId
                }
            };
        }

        public ErrorData Delete(Context context, SiteSettings ss, bool notice = false)
        {
            var statements = new List<SqlStatement>();
            var where = Rds.SysLogsWhere().SysLogId(SysLogId);
            statements.AddRange(new List<SqlStatement>
            {
                Rds.DeleteSysLogs(
                    factory: context,
                    where: where)
            });
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: statements.ToArray());
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
                    case "SysLogs_StartTime": StartTime = value.ToDateTime().ToUniversal(context: context); break;
                    case "SysLogs_EndTime": EndTime = value.ToDateTime().ToUniversal(context: context); break;
                    case "SysLogs_SysLogType": SysLogType = (SysLogTypes)value.ToInt(); break;
                    case "SysLogs_OnAzure": OnAzure = value.ToBool(); break;
                    case "SysLogs_MachineName": MachineName = value.ToString(); break;
                    case "SysLogs_ServiceName": ServiceName = value.ToString(); break;
                    case "SysLogs_TenantName": TenantName = value.ToString(); break;
                    case "SysLogs_Application": Application = value.ToString(); break;
                    case "SysLogs_Class": Class = value.ToString(); break;
                    case "SysLogs_Method": Method = value.ToString(); break;
                    case "SysLogs_Api": Api = value.ToBool(); break;
                    case "SysLogs_RequestData": RequestData = value.ToString(); break;
                    case "SysLogs_HttpMethod": HttpMethod = value.ToString(); break;
                    case "SysLogs_RequestSize": RequestSize = value.ToInt(); break;
                    case "SysLogs_ResponseSize": ResponseSize = value.ToInt(); break;
                    case "SysLogs_Elapsed": Elapsed = value.ToDouble(); break;
                    case "SysLogs_ApplicationAge": ApplicationAge = value.ToDouble(); break;
                    case "SysLogs_ApplicationRequestInterval": ApplicationRequestInterval = value.ToDouble(); break;
                    case "SysLogs_SessionAge": SessionAge = value.ToDouble(); break;
                    case "SysLogs_SessionRequestInterval": SessionRequestInterval = value.ToDouble(); break;
                    case "SysLogs_WorkingSet64": WorkingSet64 = value.ToLong(); break;
                    case "SysLogs_VirtualMemorySize64": VirtualMemorySize64 = value.ToLong(); break;
                    case "SysLogs_ProcessId": ProcessId = value.ToInt(); break;
                    case "SysLogs_ProcessName": ProcessName = value.ToString(); break;
                    case "SysLogs_BasePriority": BasePriority = value.ToInt(); break;
                    case "SysLogs_Url": Url = value.ToString(); break;
                    case "SysLogs_UrlReferer": UrlReferer = value.ToString(); break;
                    case "SysLogs_UserHostName": UserHostName = value.ToString(); break;
                    case "SysLogs_UserHostAddress": UserHostAddress = value.ToString(); break;
                    case "SysLogs_UserLanguage": UserLanguage = value.ToString(); break;
                    case "SysLogs_UserAgent": UserAgent = value.ToString(); break;
                    case "SysLogs_SessionGuid": SessionGuid = value.ToString(); break;
                    case "SysLogs_ErrMessage": ErrMessage = value.ToString(); break;
                    case "SysLogs_ErrStackTrace": ErrStackTrace = value.ToString(); break;
                    case "SysLogs_InDebug": InDebug = value.ToBool(); break;
                    case "SysLogs_AssemblyVersion": AssemblyVersion = value.ToString(); break;
                    case "SysLogs_Timestamp": Timestamp = value.ToString(); break;
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
                                        value: value);
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

        public void SetByApi(Context context, SiteSettings ss, SysLogApiModel data)
        {
            if (data.SysLogType != null) SysLogType = (SysLogTypes)data.SysLogType.ToInt().ToInt();
            if (data.OnAzure != null) OnAzure = data.OnAzure.ToBool().ToBool();
            if (data.MachineName != null) MachineName = data.MachineName.ToString().ToString();
            if (data.ServiceName != null) ServiceName = data.ServiceName.ToString().ToString();
            if (data.TenantName != null) TenantName = data.TenantName.ToString().ToString();
            if (data.Application != null) Application = data.Application.ToString().ToString();
            if (data.Class != null) Class = data.Class.ToString().ToString();
            if (data.Method != null) Method = data.Method.ToString().ToString();
            if (data.Api != null) Api = data.Api.ToBool().ToBool();
            if (data.RequestData != null) RequestData = data.RequestData.ToString().ToString();
            if (data.HttpMethod != null) HttpMethod = data.HttpMethod.ToString().ToString();
            if (data.RequestSize != null) RequestSize = data.RequestSize.ToInt().ToInt();
            if (data.ResponseSize != null) ResponseSize = data.ResponseSize.ToInt().ToInt();
            if (data.Elapsed != null) Elapsed = data.Elapsed.ToDouble().ToDouble();
            if (data.ApplicationAge != null) ApplicationAge = data.ApplicationAge.ToDouble().ToDouble();
            if (data.ApplicationRequestInterval != null) ApplicationRequestInterval = data.ApplicationRequestInterval.ToDouble().ToDouble();
            if (data.SessionAge != null) SessionAge = data.SessionAge.ToDouble().ToDouble();
            if (data.SessionRequestInterval != null) SessionRequestInterval = data.SessionRequestInterval.ToDouble().ToDouble();
            if (data.WorkingSet64 != null) WorkingSet64 = data.WorkingSet64.ToLong().ToLong();
            if (data.VirtualMemorySize64 != null) VirtualMemorySize64 = data.VirtualMemorySize64.ToLong().ToLong();
            if (data.ProcessId != null) ProcessId = data.ProcessId.ToInt().ToInt();
            if (data.ProcessName != null) ProcessName = data.ProcessName.ToString().ToString();
            if (data.BasePriority != null) BasePriority = data.BasePriority.ToInt().ToInt();
            if (data.Url != null) Url = data.Url.ToString().ToString();
            if (data.UrlReferer != null) UrlReferer = data.UrlReferer.ToString().ToString();
            if (data.UserHostName != null) UserHostName = data.UserHostName.ToString().ToString();
            if (data.UserHostAddress != null) UserHostAddress = data.UserHostAddress.ToString().ToString();
            if (data.UserLanguage != null) UserLanguage = data.UserLanguage.ToString().ToString();
            if (data.UserAgent != null) UserAgent = data.UserAgent.ToString().ToString();
            if (data.SessionGuid != null) SessionGuid = data.SessionGuid.ToString().ToString();
            if (data.ErrMessage != null) ErrMessage = data.ErrMessage.ToString().ToString();
            if (data.ErrStackTrace != null) ErrStackTrace = data.ErrStackTrace.ToString().ToString();
            if (data.InDebug != null) InDebug = data.InDebug.ToBool().ToBool();
            if (data.AssemblyVersion != null) AssemblyVersion = data.AssemblyVersion.ToString().ToString();
            if (data.Comments != null) Comments.Prepend(context: context, ss: ss, body: data.Comments);
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
                id: SysLogId);
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
                        case "CreatedTime":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                CreatedTime = new Time(context, dataRow, column.ColumnName);
                                SavedCreatedTime = CreatedTime.Value;
                            }
                            break;
                        case "SysLogId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                SysLogId = dataRow[column.ColumnName].ToLong();
                                SavedSysLogId = SysLogId;
                            }
                            break;
                        case "Ver":
                            Ver = dataRow[column.ColumnName].ToInt();
                            SavedVer = Ver;
                            break;
                        case "SysLogType":
                            SysLogType = (SysLogTypes)dataRow[column.ColumnName].ToInt();
                            SavedSysLogType = SysLogType.ToInt();
                            break;
                        case "OnAzure":
                            OnAzure = dataRow[column.ColumnName].ToBool();
                            SavedOnAzure = OnAzure;
                            break;
                        case "MachineName":
                            MachineName = dataRow[column.ColumnName].ToString();
                            SavedMachineName = MachineName;
                            break;
                        case "ServiceName":
                            ServiceName = dataRow[column.ColumnName].ToString();
                            SavedServiceName = ServiceName;
                            break;
                        case "TenantName":
                            TenantName = dataRow[column.ColumnName].ToString();
                            SavedTenantName = TenantName;
                            break;
                        case "Application":
                            Application = dataRow[column.ColumnName].ToString();
                            SavedApplication = Application;
                            break;
                        case "Class":
                            Class = dataRow[column.ColumnName].ToString();
                            SavedClass = Class;
                            break;
                        case "Method":
                            Method = dataRow[column.ColumnName].ToString();
                            SavedMethod = Method;
                            break;
                        case "Api":
                            Api = dataRow[column.ColumnName].ToBool();
                            SavedApi = Api;
                            break;
                        case "SiteId":
                            SiteId = dataRow[column.ColumnName].ToLong();
                            SavedSiteId = SiteId;
                            break;
                        case "ReferenceId":
                            ReferenceId = dataRow[column.ColumnName].ToLong();
                            SavedReferenceId = ReferenceId;
                            break;
                        case "ReferenceType":
                            ReferenceType = dataRow[column.ColumnName].ToString();
                            SavedReferenceType = ReferenceType;
                            break;
                        case "Status":
                            Status = dataRow[column.ColumnName].ToLong();
                            SavedStatus = Status;
                            break;
                        case "Description":
                            Description = dataRow[column.ColumnName].ToString();
                            SavedDescription = Description;
                            break;
                        case "RequestData":
                            RequestData = dataRow[column.ColumnName].ToString();
                            SavedRequestData = RequestData;
                            break;
                        case "HttpMethod":
                            HttpMethod = dataRow[column.ColumnName].ToString();
                            SavedHttpMethod = HttpMethod;
                            break;
                        case "RequestSize":
                            RequestSize = dataRow[column.ColumnName].ToInt();
                            SavedRequestSize = RequestSize;
                            break;
                        case "ResponseSize":
                            ResponseSize = dataRow[column.ColumnName].ToInt();
                            SavedResponseSize = ResponseSize;
                            break;
                        case "Elapsed":
                            Elapsed = dataRow[column.ColumnName].ToDouble();
                            SavedElapsed = Elapsed;
                            break;
                        case "ApplicationAge":
                            ApplicationAge = dataRow[column.ColumnName].ToDouble();
                            SavedApplicationAge = ApplicationAge;
                            break;
                        case "ApplicationRequestInterval":
                            ApplicationRequestInterval = dataRow[column.ColumnName].ToDouble();
                            SavedApplicationRequestInterval = ApplicationRequestInterval;
                            break;
                        case "SessionAge":
                            SessionAge = dataRow[column.ColumnName].ToDouble();
                            SavedSessionAge = SessionAge;
                            break;
                        case "SessionRequestInterval":
                            SessionRequestInterval = dataRow[column.ColumnName].ToDouble();
                            SavedSessionRequestInterval = SessionRequestInterval;
                            break;
                        case "WorkingSet64":
                            WorkingSet64 = dataRow[column.ColumnName].ToLong();
                            SavedWorkingSet64 = WorkingSet64;
                            break;
                        case "VirtualMemorySize64":
                            VirtualMemorySize64 = dataRow[column.ColumnName].ToLong();
                            SavedVirtualMemorySize64 = VirtualMemorySize64;
                            break;
                        case "ProcessId":
                            ProcessId = dataRow[column.ColumnName].ToInt();
                            SavedProcessId = ProcessId;
                            break;
                        case "ProcessName":
                            ProcessName = dataRow[column.ColumnName].ToString();
                            SavedProcessName = ProcessName;
                            break;
                        case "BasePriority":
                            BasePriority = dataRow[column.ColumnName].ToInt();
                            SavedBasePriority = BasePriority;
                            break;
                        case "Url":
                            Url = dataRow[column.ColumnName].ToString();
                            SavedUrl = Url;
                            break;
                        case "UrlReferer":
                            UrlReferer = dataRow[column.ColumnName].ToString();
                            SavedUrlReferer = UrlReferer;
                            break;
                        case "UserHostName":
                            UserHostName = dataRow[column.ColumnName].ToString();
                            SavedUserHostName = UserHostName;
                            break;
                        case "UserHostAddress":
                            UserHostAddress = dataRow[column.ColumnName].ToString();
                            SavedUserHostAddress = UserHostAddress;
                            break;
                        case "UserLanguage":
                            UserLanguage = dataRow[column.ColumnName].ToString();
                            SavedUserLanguage = UserLanguage;
                            break;
                        case "UserAgent":
                            UserAgent = dataRow[column.ColumnName].ToString();
                            SavedUserAgent = UserAgent;
                            break;
                        case "SessionGuid":
                            SessionGuid = dataRow[column.ColumnName].ToString();
                            SavedSessionGuid = SessionGuid;
                            break;
                        case "ErrMessage":
                            ErrMessage = dataRow[column.ColumnName].ToString();
                            SavedErrMessage = ErrMessage;
                            break;
                        case "ErrStackTrace":
                            ErrStackTrace = dataRow[column.ColumnName].ToString();
                            SavedErrStackTrace = ErrStackTrace;
                            break;
                        case "InDebug":
                            InDebug = dataRow[column.ColumnName].ToBool();
                            SavedInDebug = InDebug;
                            break;
                        case "AssemblyVersion":
                            AssemblyVersion = dataRow[column.ColumnName].ToString();
                            SavedAssemblyVersion = AssemblyVersion;
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
                || SysLogId_Updated(context: context)
                || Ver_Updated(context: context)
                || SysLogType_Updated(context: context)
                || OnAzure_Updated(context: context)
                || MachineName_Updated(context: context)
                || ServiceName_Updated(context: context)
                || TenantName_Updated(context: context)
                || Application_Updated(context: context)
                || Class_Updated(context: context)
                || Method_Updated(context: context)
                || Api_Updated(context: context)
                || SiteId_Updated(context: context)
                || ReferenceId_Updated(context: context)
                || ReferenceType_Updated(context: context)
                || Status_Updated(context: context)
                || Description_Updated(context: context)
                || RequestData_Updated(context: context)
                || HttpMethod_Updated(context: context)
                || RequestSize_Updated(context: context)
                || ResponseSize_Updated(context: context)
                || Elapsed_Updated(context: context)
                || ApplicationAge_Updated(context: context)
                || ApplicationRequestInterval_Updated(context: context)
                || SessionAge_Updated(context: context)
                || SessionRequestInterval_Updated(context: context)
                || WorkingSet64_Updated(context: context)
                || VirtualMemorySize64_Updated(context: context)
                || ProcessId_Updated(context: context)
                || ProcessName_Updated(context: context)
                || BasePriority_Updated(context: context)
                || Url_Updated(context: context)
                || UrlReferer_Updated(context: context)
                || UserHostName_Updated(context: context)
                || UserHostAddress_Updated(context: context)
                || UserLanguage_Updated(context: context)
                || UserAgent_Updated(context: context)
                || SessionGuid_Updated(context: context)
                || ErrMessage_Updated(context: context)
                || ErrStackTrace_Updated(context: context)
                || InDebug_Updated(context: context)
                || AssemblyVersion_Updated(context: context)
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
                || SysLogId_Updated(context: context)
                || Ver_Updated(context: context)
                || SysLogType_Updated(context: context)
                || OnAzure_Updated(context: context)
                || MachineName_Updated(context: context)
                || ServiceName_Updated(context: context)
                || TenantName_Updated(context: context)
                || Application_Updated(context: context)
                || Class_Updated(context: context)
                || Method_Updated(context: context)
                || Api_Updated(context: context)
                || SiteId_Updated(context: context)
                || ReferenceId_Updated(context: context)
                || ReferenceType_Updated(context: context)
                || Status_Updated(context: context)
                || Description_Updated(context: context)
                || RequestData_Updated(context: context)
                || HttpMethod_Updated(context: context)
                || RequestSize_Updated(context: context)
                || ResponseSize_Updated(context: context)
                || Elapsed_Updated(context: context)
                || ApplicationAge_Updated(context: context)
                || ApplicationRequestInterval_Updated(context: context)
                || SessionAge_Updated(context: context)
                || SessionRequestInterval_Updated(context: context)
                || WorkingSet64_Updated(context: context)
                || VirtualMemorySize64_Updated(context: context)
                || ProcessId_Updated(context: context)
                || ProcessName_Updated(context: context)
                || BasePriority_Updated(context: context)
                || Url_Updated(context: context)
                || UrlReferer_Updated(context: context)
                || UserHostName_Updated(context: context)
                || UserHostAddress_Updated(context: context)
                || UserLanguage_Updated(context: context)
                || UserAgent_Updated(context: context)
                || SessionGuid_Updated(context: context)
                || ErrMessage_Updated(context: context)
                || ErrStackTrace_Updated(context: context)
                || InDebug_Updated(context: context)
                || AssemblyVersion_Updated(context: context)
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
        public void Update(Context context, bool writeSqlToDebugLog)
        {
            if (NotLoggingIp(UserHostAddress) != true)
            {
                Repository.ExecuteNonQuery(
                    context: context,
                    transactional: true,
                    writeSqlToDebugLog: writeSqlToDebugLog,
                    statements: Rds.UpdateSysLogs(
                        where: Rds.SysLogsWhereDefault(
                            context: context,
                            sysLogModel: this),
                        param: SysLogParam(context: context)));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public SysLogModel(Context context)
        {
            Class = context.Controller;
            Method = context.Action;
            WriteSysLog(
                context: context,
                sysLogType: SysLogTypes.Info);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public SysLogModel(
            Context context,
            Exception e,
            string extendedErrorMessage = null,
            Logs logs = null,
            SysLogTypes sysLogType = SysLogTypes.Execption)
        {
            Class = context.Controller;
            Method = context.Action;
            ErrMessage = e.Message
                + (extendedErrorMessage != null
                    ? "\n" + extendedErrorMessage
                    : string.Empty)
                + (logs?.Any() == true
                    ? "\n" + logs.Select(o => o.Name + ": " + o.Value).Join("\n")
                    : string.Empty);
            ErrStackTrace = e.StackTrace;
            WriteSysLog(
                context: context,
                sysLogType: sysLogType);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public SysLogModel(
            Context context,
            string method,
            string message,
            int sysLogsStatus = 200,
            string sysLogsDescription = null,
            string errStackTrace = null,
            SysLogTypes sysLogType = SysLogTypes.Info)
        {
            Class = context.Controller;
            Method = context.Action + (!method.IsNullOrEmpty()
                ? $":{method}"
                : string.Empty);
            switch (sysLogType)
            {
                case SysLogTypes.SystemError:
                case SysLogTypes.Execption:
                    ErrMessage = message;
                    break;
                default:
                    Comments = new Comments()
                    {
                        new Comment()
                        {
                            Body = message
                        }
                    };
                    break;
            }
            if (Parameters.Rds.SysLogsSchemaVersion >= 2)
            {
                Status = sysLogsStatus;
                Description = sysLogsDescription;
            }
            ErrStackTrace = errStackTrace;
            WriteSysLog(
                context: context,
                sysLogType: sysLogType);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public enum SysLogTypes : int
        {
            Info = 10,
            Warning = 50,
            UserError = 60,
            SystemError = 80,
            Execption = 90
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public void WriteSysLog(
            Context context,
            SysLogTypes sysLogType)
        {
            StartTime = DateTime.Now;
            SetProperties(
                context: context,
                sysLogType: sysLogType);
            if (NotLoggingIp(UserHostAddress) != true)
            {
                SysLogId = Repository.ExecuteScalar_response(
                    context: context,
                    selectIdentity: true,
                    statements: Rds.InsertSysLogs(
                        selectIdentity: true,
                        param: SysLogParam(context: context)))
                            .Id.ToLong();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private SqlParamCollection SysLogParam(Context context)
        {
            var param = Rds.SysLogsParamDefault(
                context: context,
                ss: null,
                sysLogModel: this);
            var comments = param.FirstOrDefault(o => o.Name == "Comments");
            if (comments != null && Comments.Count > 0)
            {
                comments.Value = Comments
                    .Select(o => o.Body)
                    .Join("\n");
            }
            return param;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetProperties(
            Context context,
            SysLogTypes sysLogType = SysLogTypes.Info)
        {
            SysLogType = sysLogType;
            OnAzure = Environments.RdsProvider == "Azure";
            MachineName = Environments.MachineName;
            ServiceName = Environments.ServiceName;
            Application = Environments.Application;
            if (context.Url != null)
            {
                RequestData = ProcessedRequestData(context: context);
                HttpMethod = context.HttpMethod;
                ApplicationAge = Applications.ApplicationAge();
                ApplicationRequestInterval = Applications.ApplicationRequestInterval();
                SessionAge = context.SessionAge();
                SessionRequestInterval = context.SessionRequestInterval();
                RequestSize = RequestData.Length;
                Url = context.Url;
                UrlReferer = context.UrlReferrer;
                UserHostName = context.UserHostName;
                UserHostAddress = context.UserHostAddress;
                UserLanguage = context.Language;
                UserAgent = context.UserAgent;
                SessionGuid = context.SessionGuid;
            }
            if (Parameters.Rds.SysLogsSchemaVersion >= 2)
            {
                ReferenceId = context.Id;
                switch (context.Controller)
                {
                    case "items":
                        var itemModel = new ItemModel(
                            context: context,
                            referenceId: context.Id);
                        if (itemModel.AccessStatus == Databases.AccessStatuses.Selected)
                        {
                            SiteId = itemModel.SiteId;
                            ReferenceType = itemModel.ReferenceType;
                        }
                        break;
                }
            }
            InDebug = Debugs.InDebug();
            AssemblyVersion = Environments.AssemblyVersion;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string ProcessedRequestData(Context context)
        {
            if (!context.ApiRequestBody.IsNullOrEmpty())
            {
                return MaskApiData(context.ApiRequestBody) ?? string.Empty;
            }
            else
            {
                return context.FormStringRaw.Split('&')
                    .Where(o => o.Contains('='))
                    .Select(o => MaskPassword(o))
                    .Join("&");
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private string MaskApiData(string requestData)
        {
            var data = requestData;
            var apiKey = data?.RegexFirst("\"ApiKey\":[ ]*\"[a-zA-Z0-9]+?\"");
            var base64 = data?.RegexFirst("\"Base64\":[ ]*\".+?\"");
            var password = data?.RegexFirst("\"Password\":[ ]*\".+?\"");
            if (!apiKey.IsNullOrEmpty()) data = data.Replace(apiKey, "\"ApiKey\": \"*\"");
            if (!base64.IsNullOrEmpty()) data = data.Replace(base64, "\"base64\": \"*\"");
            if (!password.IsNullOrEmpty()) data = data.Replace(password, "\"Password\": \"*\"");
            return data;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private string MaskPassword(string requestData)
        {
            switch (requestData.Substring(0, requestData.IndexOf("=")).ToLower())
            {
                case "users_password": return "Users_Password=*";
                case "users_oldpassword": return "Users_OldPassword=*";
                case "users_changedpassword": return "Users_ChangedPassword=*";
                case "users_afterresetpassword": return "Users_AfterResetPassword=*";
                default: return requestData;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public void Finish(Context context, int responseSize = 0)
        {
            if (Parameters.Rds.SysLogsSchemaVersion >= 2)
            {
                Api = context.Api;
                Status = context.SysLogsStatus;
                Description = context.SysLogsDescription;
            }
            if (responseSize > 0) { ResponseSize = responseSize; }
            Elapsed = (DateTime.Now - StartTime).TotalMilliseconds;
            var currentProcess = Debugs.CurrentProcess();
            WorkingSet64 = currentProcess.WorkingSet64;
            VirtualMemorySize64 = currentProcess.VirtualMemorySize64;
            ProcessId = currentProcess.Id;
            ProcessName = currentProcess.ProcessName;
            BasePriority = currentProcess.BasePriority;
            Update(
                context: context,
                writeSqlToDebugLog: false);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private bool NotLoggingIp(string ipAddress)
        {
            if (Parameters.SysLog.NotLoggingIp?.Any() != true)
            {
                return false;
            }
            return Parameters.SysLog.NotLoggingIp
                .Select(addr => IpRange.FromCidr(addr))
                .Any(range => range.InRange(ipAddress));
        }
    }
}
