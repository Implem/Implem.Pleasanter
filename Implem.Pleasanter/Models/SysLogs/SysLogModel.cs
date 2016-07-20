using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
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
        public Title Title { get { return new Title(SysLogId, SysLogId.ToString()); } }
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
        public bool SysLogId_Updated { get { return SysLogId != SavedSysLogId; } }
        public bool SysLogType_Updated { get { return SysLogType.ToInt() != SavedSysLogType; } }
        public bool OnAzure_Updated { get { return OnAzure != SavedOnAzure; } }
        public bool MachineName_Updated { get { return MachineName != SavedMachineName && MachineName != null; } }
        public bool ServiceName_Updated { get { return ServiceName != SavedServiceName && ServiceName != null; } }
        public bool TenantName_Updated { get { return TenantName != SavedTenantName && TenantName != null; } }
        public bool Application_Updated { get { return Application != SavedApplication && Application != null; } }
        public bool Class_Updated { get { return Class != SavedClass && Class != null; } }
        public bool Method_Updated { get { return Method != SavedMethod && Method != null; } }
        public bool RequestData_Updated { get { return RequestData != SavedRequestData && RequestData != null; } }
        public bool HttpMethod_Updated { get { return HttpMethod != SavedHttpMethod && HttpMethod != null; } }
        public bool RequestSize_Updated { get { return RequestSize != SavedRequestSize; } }
        public bool ResponseSize_Updated { get { return ResponseSize != SavedResponseSize; } }
        public bool Elapsed_Updated { get { return Elapsed != SavedElapsed; } }
        public bool ApplicationAge_Updated { get { return ApplicationAge != SavedApplicationAge; } }
        public bool ApplicationRequestInterval_Updated { get { return ApplicationRequestInterval != SavedApplicationRequestInterval; } }
        public bool SessionAge_Updated { get { return SessionAge != SavedSessionAge; } }
        public bool SessionRequestInterval_Updated { get { return SessionRequestInterval != SavedSessionRequestInterval; } }
        public bool WorkingSet64_Updated { get { return WorkingSet64 != SavedWorkingSet64; } }
        public bool VirtualMemorySize64_Updated { get { return VirtualMemorySize64 != SavedVirtualMemorySize64; } }
        public bool ProcessId_Updated { get { return ProcessId != SavedProcessId; } }
        public bool ProcessName_Updated { get { return ProcessName != SavedProcessName && ProcessName != null; } }
        public bool BasePriority_Updated { get { return BasePriority != SavedBasePriority; } }
        public bool Url_Updated { get { return Url != SavedUrl && Url != null; } }
        public bool UrlReferer_Updated { get { return UrlReferer != SavedUrlReferer && UrlReferer != null; } }
        public bool UserHostName_Updated { get { return UserHostName != SavedUserHostName && UserHostName != null; } }
        public bool UserHostAddress_Updated { get { return UserHostAddress != SavedUserHostAddress && UserHostAddress != null; } }
        public bool UserLanguage_Updated { get { return UserLanguage != SavedUserLanguage && UserLanguage != null; } }
        public bool UserAgent_Updated { get { return UserAgent != SavedUserAgent && UserAgent != null; } }
        public bool SessionGuid_Updated { get { return SessionGuid != SavedSessionGuid && SessionGuid != null; } }
        public bool ErrMessage_Updated { get { return ErrMessage != SavedErrMessage && ErrMessage != null; } }
        public bool ErrStackTrace_Updated { get { return ErrStackTrace != SavedErrStackTrace && ErrStackTrace != null; } }
        public bool InDebug_Updated { get { return InDebug != SavedInDebug; } }
        public bool AssemblyVersion_Updated { get { return AssemblyVersion != SavedAssemblyVersion && AssemblyVersion != null; } }
        public List<long> SwitchTargets;

        public SysLogModel(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            DataRow dataRow)
        {
            OnConstructing();
            SiteSettings = siteSettings;
            Set(dataRow);
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
                orderBy: orderBy ?? null,
                param: param ?? null,
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

        private void Set(DataRow dataRow)
        {
            AccessStatus = Databases.AccessStatuses.Selected;
            foreach(DataColumn dataColumn in dataRow.Table.Columns)
            {
                var name = dataColumn.ColumnName;
                switch(name)
                {
                    case "CreatedTime": if (dataRow[name] != DBNull.Value) { CreatedTime = new Time(dataRow, "CreatedTime"); SavedCreatedTime = CreatedTime.Value; } break;
                    case "SysLogId": if (dataRow[name] != DBNull.Value) { SysLogId = dataRow[name].ToLong(); SavedSysLogId = SysLogId; } break;
                    case "Ver": Ver = dataRow[name].ToInt(); SavedVer = Ver; break;
                    case "SysLogType": SysLogType = (SysLogTypes)dataRow[name].ToInt(); SavedSysLogType = SysLogType.ToInt(); break;
                    case "OnAzure": OnAzure = dataRow[name].ToBool(); SavedOnAzure = OnAzure; break;
                    case "MachineName": MachineName = dataRow[name].ToString(); SavedMachineName = MachineName; break;
                    case "ServiceName": ServiceName = dataRow[name].ToString(); SavedServiceName = ServiceName; break;
                    case "TenantName": TenantName = dataRow[name].ToString(); SavedTenantName = TenantName; break;
                    case "Application": Application = dataRow[name].ToString(); SavedApplication = Application; break;
                    case "Class": Class = dataRow[name].ToString(); SavedClass = Class; break;
                    case "Method": Method = dataRow[name].ToString(); SavedMethod = Method; break;
                    case "RequestData": RequestData = dataRow[name].ToString(); SavedRequestData = RequestData; break;
                    case "HttpMethod": HttpMethod = dataRow[name].ToString(); SavedHttpMethod = HttpMethod; break;
                    case "RequestSize": RequestSize = dataRow[name].ToInt(); SavedRequestSize = RequestSize; break;
                    case "ResponseSize": ResponseSize = dataRow[name].ToInt(); SavedResponseSize = ResponseSize; break;
                    case "Elapsed": Elapsed = dataRow[name].ToDouble(); SavedElapsed = Elapsed; break;
                    case "ApplicationAge": ApplicationAge = dataRow[name].ToDouble(); SavedApplicationAge = ApplicationAge; break;
                    case "ApplicationRequestInterval": ApplicationRequestInterval = dataRow[name].ToDouble(); SavedApplicationRequestInterval = ApplicationRequestInterval; break;
                    case "SessionAge": SessionAge = dataRow[name].ToDouble(); SavedSessionAge = SessionAge; break;
                    case "SessionRequestInterval": SessionRequestInterval = dataRow[name].ToDouble(); SavedSessionRequestInterval = SessionRequestInterval; break;
                    case "WorkingSet64": WorkingSet64 = dataRow[name].ToLong(); SavedWorkingSet64 = WorkingSet64; break;
                    case "VirtualMemorySize64": VirtualMemorySize64 = dataRow[name].ToLong(); SavedVirtualMemorySize64 = VirtualMemorySize64; break;
                    case "ProcessId": ProcessId = dataRow[name].ToInt(); SavedProcessId = ProcessId; break;
                    case "ProcessName": ProcessName = dataRow[name].ToString(); SavedProcessName = ProcessName; break;
                    case "BasePriority": BasePriority = dataRow[name].ToInt(); SavedBasePriority = BasePriority; break;
                    case "Url": Url = dataRow[name].ToString(); SavedUrl = Url; break;
                    case "UrlReferer": UrlReferer = dataRow[name].ToString(); SavedUrlReferer = UrlReferer; break;
                    case "UserHostName": UserHostName = dataRow[name].ToString(); SavedUserHostName = UserHostName; break;
                    case "UserHostAddress": UserHostAddress = dataRow[name].ToString(); SavedUserHostAddress = UserHostAddress; break;
                    case "UserLanguage": UserLanguage = dataRow[name].ToString(); SavedUserLanguage = UserLanguage; break;
                    case "UserAgent": UserAgent = dataRow[name].ToString(); SavedUserAgent = UserAgent; break;
                    case "SessionGuid": SessionGuid = dataRow[name].ToString(); SavedSessionGuid = SessionGuid; break;
                    case "ErrMessage": ErrMessage = dataRow[name].ToString(); SavedErrMessage = ErrMessage; break;
                    case "ErrStackTrace": ErrStackTrace = dataRow[name].ToString(); SavedErrStackTrace = ErrStackTrace; break;
                    case "InDebug": InDebug = dataRow[name].ToBool(); SavedInDebug = InDebug; break;
                    case "AssemblyVersion": AssemblyVersion = dataRow[name].ToString(); SavedAssemblyVersion = AssemblyVersion; break;
                    case "Comments": Comments = dataRow["Comments"].ToString().Deserialize<Comments>() ?? new Comments(); SavedComments = Comments.ToJson(); break;
                    case "Creator": Creator = SiteInfo.User(dataRow.Int(name)); SavedCreator = Creator.Id; break;
                    case "Updator": Updator = SiteInfo.User(dataRow.Int(name)); SavedUpdator = Updator.Id; break;
                    case "UpdatedTime": UpdatedTime = new Time(dataRow, "UpdatedTime"); Timestamp = dataRow.Field<DateTime>("UpdatedTime").ToString("yyyy/M/d H:m:s.fff"); SavedUpdatedTime = UpdatedTime.Value; break;
                    case "IsHistory": VerType = dataRow[name].ToBool() ? Versions.VerTypes.History : Versions.VerTypes.Latest; break;
                }
            }
        }

        private string ResponseConflicts()
        {
            Get();
            return AccessStatus == Databases.AccessStatuses.Selected
                ? Messages.ResponseUpdateConflicts(Updator.FullName).ToJson()
                : Messages.ResponseDeleteConflicts().ToJson();
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
        public SysLogModel(Exception e)
        {
            Class = Routes.Controller();
            Method = Routes.Action();
            ErrMessage = e.Message;
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
            var request = new Request(HttpContext);
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
