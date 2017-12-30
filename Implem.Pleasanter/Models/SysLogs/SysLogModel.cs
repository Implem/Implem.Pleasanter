using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
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

        public bool SysLogId_Updated
        {
            get
            {
                return SysLogId != SavedSysLogId;
            }
        }

        public bool SysLogType_Updated
        {
            get
            {
                return SysLogType.ToInt() != SavedSysLogType;
            }
        }

        public bool OnAzure_Updated
        {
            get
            {
                return OnAzure != SavedOnAzure;
            }
        }

        public bool MachineName_Updated
        {
            get
            {
                return MachineName != SavedMachineName && MachineName != null;
            }
        }

        public bool ServiceName_Updated
        {
            get
            {
                return ServiceName != SavedServiceName && ServiceName != null;
            }
        }

        public bool TenantName_Updated
        {
            get
            {
                return TenantName != SavedTenantName && TenantName != null;
            }
        }

        public bool Application_Updated
        {
            get
            {
                return Application != SavedApplication && Application != null;
            }
        }

        public bool Class_Updated
        {
            get
            {
                return Class != SavedClass && Class != null;
            }
        }

        public bool Method_Updated
        {
            get
            {
                return Method != SavedMethod && Method != null;
            }
        }

        public bool RequestData_Updated
        {
            get
            {
                return RequestData != SavedRequestData && RequestData != null;
            }
        }

        public bool HttpMethod_Updated
        {
            get
            {
                return HttpMethod != SavedHttpMethod && HttpMethod != null;
            }
        }

        public bool RequestSize_Updated
        {
            get
            {
                return RequestSize != SavedRequestSize;
            }
        }

        public bool ResponseSize_Updated
        {
            get
            {
                return ResponseSize != SavedResponseSize;
            }
        }

        public bool Elapsed_Updated
        {
            get
            {
                return Elapsed != SavedElapsed;
            }
        }

        public bool ApplicationAge_Updated
        {
            get
            {
                return ApplicationAge != SavedApplicationAge;
            }
        }

        public bool ApplicationRequestInterval_Updated
        {
            get
            {
                return ApplicationRequestInterval != SavedApplicationRequestInterval;
            }
        }

        public bool SessionAge_Updated
        {
            get
            {
                return SessionAge != SavedSessionAge;
            }
        }

        public bool SessionRequestInterval_Updated
        {
            get
            {
                return SessionRequestInterval != SavedSessionRequestInterval;
            }
        }

        public bool WorkingSet64_Updated
        {
            get
            {
                return WorkingSet64 != SavedWorkingSet64;
            }
        }

        public bool VirtualMemorySize64_Updated
        {
            get
            {
                return VirtualMemorySize64 != SavedVirtualMemorySize64;
            }
        }

        public bool ProcessId_Updated
        {
            get
            {
                return ProcessId != SavedProcessId;
            }
        }

        public bool ProcessName_Updated
        {
            get
            {
                return ProcessName != SavedProcessName && ProcessName != null;
            }
        }

        public bool BasePriority_Updated
        {
            get
            {
                return BasePriority != SavedBasePriority;
            }
        }

        public bool Url_Updated
        {
            get
            {
                return Url != SavedUrl && Url != null;
            }
        }

        public bool UrlReferer_Updated
        {
            get
            {
                return UrlReferer != SavedUrlReferer && UrlReferer != null;
            }
        }

        public bool UserHostName_Updated
        {
            get
            {
                return UserHostName != SavedUserHostName && UserHostName != null;
            }
        }

        public bool UserHostAddress_Updated
        {
            get
            {
                return UserHostAddress != SavedUserHostAddress && UserHostAddress != null;
            }
        }

        public bool UserLanguage_Updated
        {
            get
            {
                return UserLanguage != SavedUserLanguage && UserLanguage != null;
            }
        }

        public bool UserAgent_Updated
        {
            get
            {
                return UserAgent != SavedUserAgent && UserAgent != null;
            }
        }

        public bool SessionGuid_Updated
        {
            get
            {
                return SessionGuid != SavedSessionGuid && SessionGuid != null;
            }
        }

        public bool ErrMessage_Updated
        {
            get
            {
                return ErrMessage != SavedErrMessage && ErrMessage != null;
            }
        }

        public bool ErrStackTrace_Updated
        {
            get
            {
                return ErrStackTrace != SavedErrStackTrace && ErrStackTrace != null;
            }
        }

        public bool InDebug_Updated
        {
            get
            {
                return InDebug != SavedInDebug;
            }
        }

        public bool AssemblyVersion_Updated
        {
            get
            {
                return AssemblyVersion != SavedAssemblyVersion && AssemblyVersion != null;
            }
        }

        public SysLogModel(DataRow dataRow, string tableAlias = null)
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

        public SysLogModel Get(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            Set(Rds.ExecuteTable(statements: Rds.SelectSysLogs(
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
                        case "CreatedTime":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                CreatedTime = new Time(dataRow, column.ColumnName);
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
                            Creator = SiteInfo.User(dataRow[column.ColumnName].ToInt());
                            SavedCreator = Creator.Id;
                            break;
                        case "Updator":
                            Updator = SiteInfo.User(dataRow[column.ColumnName].ToInt());
                            SavedUpdator = Updator.Id;
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
                CreatedTime_Updated ||
                SysLogId_Updated ||
                Ver_Updated ||
                SysLogType_Updated ||
                OnAzure_Updated ||
                MachineName_Updated ||
                ServiceName_Updated ||
                TenantName_Updated ||
                Application_Updated ||
                Class_Updated ||
                Method_Updated ||
                RequestData_Updated ||
                HttpMethod_Updated ||
                RequestSize_Updated ||
                ResponseSize_Updated ||
                Elapsed_Updated ||
                ApplicationAge_Updated ||
                ApplicationRequestInterval_Updated ||
                SessionAge_Updated ||
                SessionRequestInterval_Updated ||
                WorkingSet64_Updated ||
                VirtualMemorySize64_Updated ||
                ProcessId_Updated ||
                ProcessName_Updated ||
                BasePriority_Updated ||
                Url_Updated ||
                UrlReferer_Updated ||
                UserHostName_Updated ||
                UserHostAddress_Updated ||
                UserLanguage_Updated ||
                UserAgent_Updated ||
                SessionGuid_Updated ||
                ErrMessage_Updated ||
                ErrStackTrace_Updated ||
                InDebug_Updated ||
                AssemblyVersion_Updated ||
                Comments_Updated ||
                Creator_Updated ||
                Updator_Updated ||
                UpdatedTime_Updated;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public void Update(bool writeSqlToDebugLog)
        {
            Rds.ExecuteNonQuery(
                transactional: true,
                writeSqlToDebugLog: writeSqlToDebugLog,
                statements: Rds.UpdateSysLogs(
                    verUp: VerUp,
                    where: Rds.SysLogsWhereDefault(this),
                    param: Rds.SysLogsParamDefault(this)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public SysLogModel()
        {
            Class = Routes.Controller();
            Method = Routes.Action();
            WriteSysLog();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public SysLogModel(Exception e, Logs logs = null)
        {
            Class = Routes.Controller();
            Method = Routes.Action();
            ErrMessage = e.Message + (logs?.Any() == true
                ? "\n" + logs.Select(o => o.Name + ": " + o.Value).Join("\n")
                : string.Empty);
            ErrStackTrace = e.StackTrace;
            WriteSysLog();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public SysLogModel(System.Web.Mvc.ExceptionContext filterContext)
        {
            Class = Routes.Controller();
            Method = Routes.Action();
            WriteSysLog();
            SysLogType = SysLogTypes.Execption;
            ErrMessage = filterContext.Exception.Message;
            ErrStackTrace = filterContext.Exception.StackTrace;
            Finish();
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
        public void WriteSysLog()
        {
            StartTime = DateTime.Now;
            SetProperties();
            SysLogId = Rds.ExecuteScalar_long(statements: Rds.InsertSysLogs(
                selectIdentity: true,
                param: Rds.SysLogsParamDefault(this)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetProperties()
        {
            SysLogType = SysLogModel.SysLogTypes.Info;
            OnAzure = Environments.RdsProvider == "Azure";
            MachineName = Environments.MachineName;
            ServiceName = Environments.ServiceName;
            Application = Environments.Application;
            var request = new Request(System.Web.HttpContext.Current);
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
        public void Finish(int responseSize = 0)
        {
            if (responseSize > 0) { ResponseSize = responseSize; }
            Elapsed = (DateTime.Now - StartTime).TotalMilliseconds;
            var currentProcess = Debugs.CurrentProcess();
            WorkingSet64 = currentProcess.WorkingSet64;
            VirtualMemorySize64 = currentProcess.VirtualMemorySize64;
            ProcessId = currentProcess.Id;
            ProcessName = currentProcess.ProcessName;
            BasePriority = currentProcess.BasePriority;
            Update(writeSqlToDebugLog: false);
        }
    }
}
