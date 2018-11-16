using System;
namespace Implem.SupportTools.SysLogViewer.Model
{
    public enum SysLogTypes : int
    {
        Info = 10,
        Warning = 50,
        UserError = 60,
        SystemError = 80,
        Execption = 90
    }

    public class SysLogModel
    {
        public DateTime CreatedTime { get; set; }   
        public long? SysLogId { get; set; }
        public int? Ver { get; set; }
        public SysLogTypes SysLogType { get; set; }
        public bool OnAzure { get; set; }
        public string MachineName { get; set; }
        public string ServiceName { get; set; }
        public string TenantName { get; set; }
        public string Application { get; set; }
        public string Class { get; set; }
        public string Method { get; set; }
        public string RequestData { get; set; }
        public string HttpMethod { get; set; }
        public int RequestSize { get; set; }
        public int ResponseSize { get; set; }
        public double? Elapsed { get; set; }
        public double? ApplicationAge { get; set; }
        public double? ApplicationRequestInterval { get; set; }
        public double? SessionAge { get; set; }
        public double? SessionRequestInterval { get; set; }
        public long? WorkingSet64 { get; set; }
        public long? VirtualMemorySize64 { get; set; }
        public int? ProcessId { get; set; }
        public string ProcessName { get; set; }
        public int? BasePriority { get; set; }
        public string Url { get; set; }
        public string UrlReferer { get; set; }
        public string UserHostName { get; set; }
        public string UserHostAddress { get; set; }
        public string UserLanguage { get; set; }
        public string UserAgent { get; set; }
        public string SessionGuid { get; set; }
        public string ErrMessage { get; set; }
        public string ErrStackTrace { get; set; }
        public bool? InDebug { get; set; }
        public string AssemblyVersion { get; set; }
        public string Comments { get; set; }
        public int? Creator { get; set; }
        public int? Updator { get; set; }
        public DateTime? UpdatedTime { get; set; }

        public SysLogModel()
        {

        }
    }
}
