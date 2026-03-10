using System;
namespace Implem.Pleasanter.Models.McpLogs;

[Serializable]
public readonly record struct McpLogLogModel(
    DateTime CreatedTime,
    long? McpLogId,
    int? Ver,
    DateTime? StartTime,
    DateTime? EndTime,
    string McpRequestId,
    string McpSessionId,
    string McpMethod,
    string TargetName,
    int? TenantId,
    int? UserId,
    string ApiKeyPrefix,
    string ClientName,
    string ClientVersion,
    string ProtocolVersion,
    double? Elapsed,
    int? Status,
    int? JsonRpcErrorCode,
    string ErrMessage,
    string RequestData,
    string ResponseData,
    string UserHostAddress,
    string UserAgent,
    string Comments,
    int? Creator,
    int? Updator,
    DateTime UpdatedTime
    );
