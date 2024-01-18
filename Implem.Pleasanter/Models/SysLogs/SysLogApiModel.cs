using Implem.Pleasanter.Models.Shared;
using System;
using System.Collections.Generic;
namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class SysLogApiModel : _BaseApiModel
    {
        public DateTime? CreatedTime { get; set; }
        public long? SysLogId { get; set; }
        public int? Ver { get; set; }
        public int? SysLogType { get; set; }
        public bool? OnAzure { get; set; }
        public string MachineName { get; set; }
        public string ServiceName { get; set; }
        public string TenantName { get; set; }
        public string Application { get; set; }
        public new string Class { get; set; }
        public string Method { get; set; }
        public bool? Api { get; set; }
        public long? SiteId { get; set; }
        public long? ReferenceId { get; set; }
        public string ReferenceType { get; set; }
        public long? Status { get; set; }
        public new string Description { get; set; }
        public string RequestData { get; set; }
        public string HttpMethod { get; set; }
        public int? RequestSize { get; set; }
        public int? ResponseSize { get; set; }
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

        public SysLogApiModel()
        {
        }

        public override object ObjectValue(string columnName)
        {
            switch (columnName)
            {
                case "CreatedTime": return CreatedTime;
                case "SysLogId": return SysLogId;
                case "Ver": return Ver;
                case "SysLogType": return SysLogType;
                case "OnAzure": return OnAzure;
                case "MachineName": return MachineName;
                case "ServiceName": return ServiceName;
                case "TenantName": return TenantName;
                case "Application": return Application;
                case "Class": return Class;
                case "Method": return Method;
                case "Api": return Api;
                case "SiteId": return SiteId;
                case "ReferenceId": return ReferenceId;
                case "ReferenceType": return ReferenceType;
                case "Status": return Status;
                case "Description": return Description;
                case "RequestData": return RequestData;
                case "HttpMethod": return HttpMethod;
                case "RequestSize": return RequestSize;
                case "ResponseSize": return ResponseSize;
                case "Elapsed": return Elapsed;
                case "ApplicationAge": return ApplicationAge;
                case "ApplicationRequestInterval": return ApplicationRequestInterval;
                case "SessionAge": return SessionAge;
                case "SessionRequestInterval": return SessionRequestInterval;
                case "WorkingSet64": return WorkingSet64;
                case "VirtualMemorySize64": return VirtualMemorySize64;
                case "ProcessId": return ProcessId;
                case "ProcessName": return ProcessName;
                case "BasePriority": return BasePriority;
                case "Url": return Url;
                case "UrlReferer": return UrlReferer;
                case "UserHostName": return UserHostName;
                case "UserHostAddress": return UserHostAddress;
                case "UserLanguage": return UserLanguage;
                case "UserAgent": return UserAgent;
                case "SessionGuid": return SessionGuid;
                case "ErrMessage": return ErrMessage;
                case "ErrStackTrace": return ErrStackTrace;
                case "InDebug": return InDebug;
                case "AssemblyVersion": return AssemblyVersion;
                case "Comments": return Comments;
                case "Creator": return Creator;
                case "Updator": return Updator;
                case "UpdatedTime": return UpdatedTime;
                default: return base.ObjectValue(columnName: columnName);
            }
        }
    }
}
