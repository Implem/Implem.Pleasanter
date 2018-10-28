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

        [NonSerialized] public long SavedSysLogId = 0;
        [NonSerialized] public DateTime SavedStartTime = 0.ToDateTime();
        [NonSerialized] public DateTime SavedEndTime = 0.ToDateTime();
        [NonSerialized] public int SavedSysLogType = 10;
        [NonSerialized] public bool SavedOnAzure = false;
        [NonSerialized] public string SavedMachineName = string.Empty;
        [NonSerialized] public string SavedServiceName = string.Empty;
        [NonSerialized] public string SavedTenantName = string.Empty;
        [NonSerialized] public string SavedApplication = string.Empty;
        [NonSerialized] public string SavedClass = string.Empty;
        [NonSerialized] public string SavedMethod = string.Empty;
        [NonSerialized] public string SavedRequestData = string.Empty;
        [NonSerialized] public string SavedHttpMethod = string.Empty;
        [NonSerialized] public int SavedRequestSize = 0;
        [NonSerialized] public int SavedResponseSize = 0;
        [NonSerialized] public double SavedElapsed = 0;
        [NonSerialized] public double SavedApplicationAge = 0;
        [NonSerialized] public double SavedApplicationRequestInterval = 0;
        [NonSerialized] public double SavedSessionAge = 0;
        [NonSerialized] public double SavedSessionRequestInterval = 0;
        [NonSerialized] public long SavedWorkingSet64 = 0;
        [NonSerialized] public long SavedVirtualMemorySize64 = 0;
        [NonSerialized] public int SavedProcessId = 0;
        [NonSerialized] public string SavedProcessName = string.Empty;
        [NonSerialized] public int SavedBasePriority = 0;
        [NonSerialized] public string SavedUrl = string.Empty;
        [NonSerialized] public string SavedUrlReferer = string.Empty;
        [NonSerialized] public string SavedUserHostName = string.Empty;
        [NonSerialized] public string SavedUserHostAddress = string.Empty;
        [NonSerialized] public string SavedUserLanguage = string.Empty;
        [NonSerialized] public string SavedUserAgent = string.Empty;
        [NonSerialized] public string SavedSessionGuid = string.Empty;
        [NonSerialized] public string SavedErrMessage = string.Empty;
        [NonSerialized] public string SavedErrStackTrace = string.Empty;
        [NonSerialized] public bool SavedInDebug = false;
        [NonSerialized] public string SavedAssemblyVersion = string.Empty;

        public bool SysLogId_Updated(Context context, Column column = null)
        {
            return SysLogId != SavedSysLogId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToLong() != SysLogId);
        }

        public bool SysLogType_Updated(Context context, Column column = null)
        {
            return SysLogType.ToInt() != SavedSysLogType &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != SysLogType.ToInt());
        }

        public bool OnAzure_Updated(Context context, Column column = null)
        {
            return OnAzure != SavedOnAzure &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToBool() != OnAzure);
        }

        public bool MachineName_Updated(Context context, Column column = null)
        {
            return MachineName != SavedMachineName && MachineName != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != MachineName);
        }

        public bool ServiceName_Updated(Context context, Column column = null)
        {
            return ServiceName != SavedServiceName && ServiceName != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != ServiceName);
        }

        public bool TenantName_Updated(Context context, Column column = null)
        {
            return TenantName != SavedTenantName && TenantName != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != TenantName);
        }

        public bool Application_Updated(Context context, Column column = null)
        {
            return Application != SavedApplication && Application != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != Application);
        }

        public bool Class_Updated(Context context, Column column = null)
        {
            return Class != SavedClass && Class != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != Class);
        }

        public bool Method_Updated(Context context, Column column = null)
        {
            return Method != SavedMethod && Method != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != Method);
        }

        public bool RequestData_Updated(Context context, Column column = null)
        {
            return RequestData != SavedRequestData && RequestData != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != RequestData);
        }

        public bool HttpMethod_Updated(Context context, Column column = null)
        {
            return HttpMethod != SavedHttpMethod && HttpMethod != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != HttpMethod);
        }

        public bool RequestSize_Updated(Context context, Column column = null)
        {
            return RequestSize != SavedRequestSize &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != RequestSize);
        }

        public bool ResponseSize_Updated(Context context, Column column = null)
        {
            return ResponseSize != SavedResponseSize &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != ResponseSize);
        }

        public bool Elapsed_Updated(Context context, Column column = null)
        {
            return Elapsed != SavedElapsed &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToDouble() != Elapsed);
        }

        public bool ApplicationAge_Updated(Context context, Column column = null)
        {
            return ApplicationAge != SavedApplicationAge &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToDouble() != ApplicationAge);
        }

        public bool ApplicationRequestInterval_Updated(Context context, Column column = null)
        {
            return ApplicationRequestInterval != SavedApplicationRequestInterval &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToDouble() != ApplicationRequestInterval);
        }

        public bool SessionAge_Updated(Context context, Column column = null)
        {
            return SessionAge != SavedSessionAge &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToDouble() != SessionAge);
        }

        public bool SessionRequestInterval_Updated(Context context, Column column = null)
        {
            return SessionRequestInterval != SavedSessionRequestInterval &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToDouble() != SessionRequestInterval);
        }

        public bool WorkingSet64_Updated(Context context, Column column = null)
        {
            return WorkingSet64 != SavedWorkingSet64 &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToLong() != WorkingSet64);
        }

        public bool VirtualMemorySize64_Updated(Context context, Column column = null)
        {
            return VirtualMemorySize64 != SavedVirtualMemorySize64 &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToLong() != VirtualMemorySize64);
        }

        public bool ProcessId_Updated(Context context, Column column = null)
        {
            return ProcessId != SavedProcessId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != ProcessId);
        }

        public bool ProcessName_Updated(Context context, Column column = null)
        {
            return ProcessName != SavedProcessName && ProcessName != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != ProcessName);
        }

        public bool BasePriority_Updated(Context context, Column column = null)
        {
            return BasePriority != SavedBasePriority &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != BasePriority);
        }

        public bool Url_Updated(Context context, Column column = null)
        {
            return Url != SavedUrl && Url != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != Url);
        }

        public bool UrlReferer_Updated(Context context, Column column = null)
        {
            return UrlReferer != SavedUrlReferer && UrlReferer != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != UrlReferer);
        }

        public bool UserHostName_Updated(Context context, Column column = null)
        {
            return UserHostName != SavedUserHostName && UserHostName != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != UserHostName);
        }

        public bool UserHostAddress_Updated(Context context, Column column = null)
        {
            return UserHostAddress != SavedUserHostAddress && UserHostAddress != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != UserHostAddress);
        }

        public bool UserLanguage_Updated(Context context, Column column = null)
        {
            return UserLanguage != SavedUserLanguage && UserLanguage != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != UserLanguage);
        }

        public bool UserAgent_Updated(Context context, Column column = null)
        {
            return UserAgent != SavedUserAgent && UserAgent != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != UserAgent);
        }

        public bool SessionGuid_Updated(Context context, Column column = null)
        {
            return SessionGuid != SavedSessionGuid && SessionGuid != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != SessionGuid);
        }

        public bool ErrMessage_Updated(Context context, Column column = null)
        {
            return ErrMessage != SavedErrMessage && ErrMessage != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != ErrMessage);
        }

        public bool ErrStackTrace_Updated(Context context, Column column = null)
        {
            return ErrStackTrace != SavedErrStackTrace && ErrStackTrace != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != ErrStackTrace);
        }

        public bool InDebug_Updated(Context context, Column column = null)
        {
            return InDebug != SavedInDebug &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToBool() != InDebug);
        }

        public bool AssemblyVersion_Updated(Context context, Column column = null)
        {
            return AssemblyVersion != SavedAssemblyVersion && AssemblyVersion != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != AssemblyVersion);
        }

        public SysLogModel(Context context, DataRow dataRow, string tableAlias = null)
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

        public SysLogModel Get(
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
                statements: Rds.SelectSysLogs(
                    tableType: tableType,
                    column: column ?? Rds.SysLogsDefaultColumns(),
                    join: join ??  Rds.SysLogsJoinDefault(),
                    where: where ?? Rds.SysLogsWhereDefault(this),
                    orderBy: orderBy,
                    param: param,
                    distinct: distinct,
                    top: top)));
            return this;
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
                        case "IsHistory": VerType = dataRow[column.ColumnName].ToBool() ? Versions.VerTypes.History : Versions.VerTypes.Latest; break;
                    }
                }
            }
        }

        public bool Updated(Context context)
        {
            return
                SysLogId_Updated(context: context) ||
                Ver_Updated(context: context) ||
                SysLogType_Updated(context: context) ||
                OnAzure_Updated(context: context) ||
                MachineName_Updated(context: context) ||
                ServiceName_Updated(context: context) ||
                TenantName_Updated(context: context) ||
                Application_Updated(context: context) ||
                Class_Updated(context: context) ||
                Method_Updated(context: context) ||
                RequestData_Updated(context: context) ||
                HttpMethod_Updated(context: context) ||
                RequestSize_Updated(context: context) ||
                ResponseSize_Updated(context: context) ||
                Elapsed_Updated(context: context) ||
                ApplicationAge_Updated(context: context) ||
                ApplicationRequestInterval_Updated(context: context) ||
                SessionAge_Updated(context: context) ||
                SessionRequestInterval_Updated(context: context) ||
                WorkingSet64_Updated(context: context) ||
                VirtualMemorySize64_Updated(context: context) ||
                ProcessId_Updated(context: context) ||
                ProcessName_Updated(context: context) ||
                BasePriority_Updated(context: context) ||
                Url_Updated(context: context) ||
                UrlReferer_Updated(context: context) ||
                UserHostName_Updated(context: context) ||
                UserHostAddress_Updated(context: context) ||
                UserLanguage_Updated(context: context) ||
                UserAgent_Updated(context: context) ||
                SessionGuid_Updated(context: context) ||
                ErrMessage_Updated(context: context) ||
                ErrStackTrace_Updated(context: context) ||
                InDebug_Updated(context: context) ||
                AssemblyVersion_Updated(context: context) ||
                Comments_Updated(context: context) ||
                Creator_Updated(context: context) ||
                Updator_Updated(context: context);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public void Update(Context context, bool writeSqlToDebugLog)
        {
            Rds.ExecuteNonQuery(
                context: context,
                transactional: true,
                writeSqlToDebugLog: writeSqlToDebugLog,
                statements: Rds.UpdateSysLogs(
                    where: Rds.SysLogsWhereDefault(this),
                    param: Rds.SysLogsParamDefault(
                        context: context,
                        sysLogModel: this)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public SysLogModel(Context context)
        {
            Class = context.Controller;
            Method = context.Action;
            WriteSysLog(context: context);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public SysLogModel(Context context, Exception e, Logs logs = null)
        {
            Class = context.Controller;
            Method = context.Action;
            ErrMessage = e.Message + (logs?.Any() == true
                ? "\n" + logs.Select(o => o.Name + ": " + o.Value).Join("\n")
                : string.Empty);
            ErrStackTrace = e.StackTrace;
            WriteSysLog(context: context);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public SysLogModel(Context context, string errorMessage)
        {
            Class = context.Controller;
            Method = context.Action;
            ErrMessage = errorMessage;
            WriteSysLog(context: context);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public SysLogModel(Context context, System.Web.Mvc.ExceptionContext filterContext)
        {
            Class = context.Controller;
            Method = context.Action;
            WriteSysLog(context: context);
            SysLogType = SysLogTypes.Execption;
            ErrMessage = filterContext.Exception.Message;
            ErrStackTrace = filterContext.Exception.StackTrace;
            Finish(context: context);
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
        public void WriteSysLog(Context context)
        {
            StartTime = DateTime.Now;
            SetProperties();
            SysLogId = Rds.ExecuteScalar_response(
                context: context,
                selectIdentity: true,
                statements: Rds.InsertSysLogs(
                    setIdentity: true,
                    param: Rds.SysLogsParamDefault(
                        context: context,
                        sysLogModel: this)))
                            .Identity.ToLong();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetProperties()
        {
            SysLogType = SysLogTypes.Info;
            OnAzure = Environments.RdsProvider == "Azure";
            MachineName = Environments.MachineName;
            ServiceName = Environments.ServiceName;
            Application = Environments.Application;
            var request = new Request();
            if (request.HttpRequest != null)
            {
                RequestData = request.ProcessedRequestData();
                HttpMethod = request.HttpMethod();
                ApplicationAge = Applications.ApplicationAge();
                ApplicationRequestInterval = Applications.ApplicationRequestInterval();
                SessionAge = Sessions.SessionAge();
                SessionRequestInterval = Sessions.SessionRequestInterval();
                RequestSize = RequestData.Length;
                Url = request.Url();
                UrlReferer = request.UrlReferrer();
                UserHostName = request.UserHostName();
                UserHostAddress = request.UserHostAddress();
                UserLanguage = request.UserLanguage();
                UserAgent = request.UserAgent();
                SessionGuid = Sessions.SessionGuid();
            }
            InDebug = Debugs.InDebug();
            AssemblyVersion = Environments.AssemblyVersion;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public void Finish(Context context, int responseSize = 0)
        {
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
    }
}
