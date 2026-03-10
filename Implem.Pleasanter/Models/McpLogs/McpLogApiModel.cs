using Implem.Pleasanter.Models.Shared;
using System;
using System.Collections.Generic;
namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class McpLogApiModel : _BaseApiModel
    {
        public long? McpLogId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string McpRequestId { get; set; }
        public string McpSessionId { get; set; }
        public string McpMethod { get; set; }
        public string TargetName { get; set; }
        public int? TenantId { get; set; }
        public int? UserId { get; set; }
        public string ApiKeyPrefix { get; set; }
        public string ClientName { get; set; }
        public string ClientVersion { get; set; }
        public string ProtocolVersion { get; set; }
        public double? Elapsed { get; set; }
        public int? Status { get; set; }
        public int? JsonRpcErrorCode { get; set; }
        public string ErrMessage { get; set; }
        public string RequestData { get; set; }
        public string ResponseData { get; set; }
        public string UserHostAddress { get; set; }
        public string UserAgent { get; set; }
        public int? Ver { get; set; }
        public string Comments { get; set; }
        public int? Updator { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public int? Creator { get; set; }
        public DateTime? CreatedTime { get; set; }

        public McpLogApiModel()
        {
        }

        public override object ObjectValue(string columnName)
        {
            switch (columnName)
            {
                case "McpLogId": return McpLogId;
                case "StartTime": return StartTime;
                case "EndTime": return EndTime;
                case "McpRequestId": return McpRequestId;
                case "McpSessionId": return McpSessionId;
                case "McpMethod": return McpMethod;
                case "TargetName": return TargetName;
                case "TenantId": return TenantId;
                case "UserId": return UserId;
                case "ApiKeyPrefix": return ApiKeyPrefix;
                case "ClientName": return ClientName;
                case "ClientVersion": return ClientVersion;
                case "ProtocolVersion": return ProtocolVersion;
                case "Elapsed": return Elapsed;
                case "Status": return Status;
                case "JsonRpcErrorCode": return JsonRpcErrorCode;
                case "ErrMessage": return ErrMessage;
                case "RequestData": return RequestData;
                case "ResponseData": return ResponseData;
                case "UserHostAddress": return UserHostAddress;
                case "UserAgent": return UserAgent;
                case "Ver": return Ver;
                case "Comments": return Comments;
                case "Updator": return Updator;
                case "UpdatedTime": return UpdatedTime;
                case "Creator": return Creator;
                case "CreatedTime": return CreatedTime;
                default: return base.ObjectValue(columnName: columnName);
            }
        }
    }
}
