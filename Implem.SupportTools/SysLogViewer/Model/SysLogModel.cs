using System;
using System.Linq;

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

        public static string CsvHeader
        {
            get => $"{nameof(CreatedTime)},{nameof(SysLogId)},{nameof(Ver)},{nameof(SysLogType)},{nameof(OnAzure)},{nameof(MachineName)}," +
                $"{nameof(ServiceName)},{nameof(TenantName)},{nameof(Application)},{nameof(Class)},{nameof(Method)},{nameof(RequestData)},{nameof(HttpMethod)}," +
                $"{nameof(RequestSize)},{nameof(ResponseSize)},{nameof(Elapsed)},{nameof(ApplicationAge)},{nameof(ApplicationRequestInterval)},{nameof(SessionAge)},{nameof(SessionRequestInterval)}," +
                $"{nameof(WorkingSet64)},{nameof(VirtualMemorySize64)},{ nameof(ProcessId)},{ nameof(ProcessName)},{ nameof(BasePriority)},{ nameof(Url)}," +
                $"{ nameof(UrlReferer)},{ nameof(UserHostName)},{ nameof(UserHostAddress)},{ nameof(UserLanguage)},{ nameof(UserAgent)},{ nameof(SessionGuid)}," +
                $"{ nameof(ErrMessage)},{ nameof(ErrStackTrace)},{ nameof(InDebug)},{ nameof(AssemblyVersion)},{ nameof(Comments)},{ nameof(Creator)},{ nameof(Updator)},{ nameof(UpdatedTime)}";
        }

        public string ToCsv()
        {
            return $"{ToCsvField(CreatedTime)},{ToCsvField(SysLogId)},{ToCsvField(Ver)},{ToCsvField(SysLogType)},{ToCsvField(OnAzure)},{ToCsvField(MachineName)}," +
                $"{ToCsvField(ServiceName)},{ToCsvField(TenantName)},{ToCsvField(Application)},{ToCsvField(Class)},{ToCsvField(Method)},{ToCsvField(RequestData)},{ToCsvField(HttpMethod)}," +
                $"{ToCsvField(RequestSize)},{ToCsvField(ResponseSize)},{ToCsvField(Elapsed)},{ToCsvField(ApplicationAge)},{ToCsvField(ApplicationRequestInterval)},{ToCsvField(SessionAge)},{ToCsvField(SessionRequestInterval)}," +
                $"{ToCsvField(WorkingSet64)},{ToCsvField(VirtualMemorySize64)},{ ToCsvField(ProcessId)},{ ToCsvField(ProcessName)},{ ToCsvField(BasePriority)},{ ToCsvField(Url)}," +
                $"{ ToCsvField(UrlReferer)},{ ToCsvField(UserHostName)},{ ToCsvField(UserHostAddress)},{ ToCsvField(UserLanguage)},{ ToCsvField(UserAgent)},{ ToCsvField(SessionGuid)}," +
                $"{ ToCsvField(ErrMessage)},{ ToCsvField(ErrStackTrace)},{ ToCsvField(InDebug)},{ ToCsvField(AssemblyVersion)},{ ToCsvField(Comments)},{ ToCsvField(Creator)},{ ToCsvField(Updator)},{ ToCsvField(UpdatedTime)}";
        }

        private string ToCsvField(object value)
        {
            var field = value?.ToString() ?? "";
            if(field.Any(c=> new[] { '"', ',', '\n', '\r' }.Contains(c)))
            {
                return $"\"{field.Replace("\"", "\"\"")}\"";
            }
            return field;
        }
    }
}
