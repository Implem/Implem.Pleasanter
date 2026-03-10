using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Models;
using System;
using System.Threading;

namespace Implem.Pleasanter.MCP.McpContext
{
    public static class McpLogEntryHolder
    {
        private static readonly AsyncLocal<McpLogModel> _logEntry = new();

        public static McpLogModel Current
        {
            get => _logEntry.Value;
            set => _logEntry.Value = value;
        }

        public static void Clear()
        {
            _logEntry.Value = null;
        }

        public static McpLogModel StartNew()
        {
            var entry = new McpLogModel
            {
                StartTime = DateTime.UtcNow
            };
            _logEntry.Value = entry;
            return entry;
        }

        public static void Finish()
        {
            var entry = _logEntry.Value;
            if (entry != null)
            {
                entry.EndTime = DateTime.UtcNow;
                entry.Elapsed = (entry.EndTime - entry.StartTime).TotalMilliseconds;
            }
        }

        public static void SetHttpInfo(
            string userHostAddress,
            string userAgent)
        {
            var entry = _logEntry.Value;
            if (entry != null)
            {
                entry.UserHostAddress = userHostAddress ?? string.Empty;
                entry.UserAgent = userAgent ?? string.Empty;
            }
        }

        public static void SetMcpRequestInfo(McpRequestInfo requestInfo)
        {
            var entry = _logEntry.Value;
            if (entry != null && requestInfo != null)
            {
                entry.McpRequestId = requestInfo.RequestId ?? string.Empty;
                entry.McpMethod = requestInfo.Method ?? string.Empty;
                entry.TargetName = requestInfo.ToolName ?? string.Empty;
                entry.RequestData = requestInfo.RawRequestBody ?? string.Empty;
            }
        }

        public static void SetAuthInfo(
            int tenantId,
            int userId,
            string apiKey)
        {
            var entry = _logEntry.Value;
            if (entry != null)
            {
                entry.TenantId = tenantId;
                entry.UserId = SiteInfo.User(context: null, userId: userId);
                entry.ApiKeyPrefix = !string.IsNullOrEmpty(apiKey) && apiKey.Length > 16
                    ? apiKey.Substring(0, 16)
                    : apiKey ?? string.Empty;
            }
        }

        public static void SetSessionInfo(string sessionId)
        {
            var entry = _logEntry.Value;
            if (entry != null)
            {
                entry.McpSessionId = sessionId ?? string.Empty;
            }
        }

        public static void SetClientInfo(
            string clientName,
            string clientVersion,
            string protocolVersion)
        {
            var entry = _logEntry.Value;
            if (entry != null)
            {
                entry.ClientName = clientName ?? string.Empty;
                entry.ClientVersion = clientVersion ?? string.Empty;
                entry.ProtocolVersion = protocolVersion ?? string.Empty;
            }
        }

        public static void SetResponseInfo(
            int status,
            string responseData,
            int? jsonRpcErrorCode = null,
            string errMessage = null)
        {
            var entry = _logEntry.Value;
            if (entry != null)
            {
                entry.Status = status;
                var maxLength = DefinitionAccessor.Parameters.McpServer?.Logging?.ResponseDataMaxLength ?? 65536;
                entry.ResponseData = maxLength > 0 && responseData?.Length > maxLength
                    ? responseData.Substring(0, maxLength) + "...(truncated)"
                    : responseData ?? string.Empty;
                if (jsonRpcErrorCode.HasValue)
                {
                    entry.JsonRpcErrorCode = jsonRpcErrorCode.Value;
                }
                if (!string.IsNullOrEmpty(errMessage))
                {
                    entry.ErrMessage = errMessage;
                }
            }
        }

        public static void SetError(
            int status,
            int? jsonRpcErrorCode,
            string errMessage)
        {
            var entry = _logEntry.Value;
            if (entry != null)
            {
                entry.Status = status;
                if (jsonRpcErrorCode.HasValue)
                {
                    entry.JsonRpcErrorCode = jsonRpcErrorCode.Value;
                }
                entry.ErrMessage = errMessage ?? string.Empty;
            }
        }
    }
}
