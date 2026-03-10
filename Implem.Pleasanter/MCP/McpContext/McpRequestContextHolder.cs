using System;
using System.Threading;

namespace Implem.Pleasanter.MCP.McpContext
{
    public static class McpRequestContextHolder
    {
        private static readonly AsyncLocal<McpRequestInfo> _requestInfo = new();

        public static McpRequestInfo Current
        {
            get => _requestInfo.Value;
            set => _requestInfo.Value = value;
        }

        public static void Clear()
        {
            _requestInfo.Value = null;
        }
    }

    public sealed class McpRequestInfo
    {
        public string RequestId { get; init; }

        public string ToolName { get; init; }

        public string Arguments { get; init; }

        public string Method { get; init; }

        public DateTime ReceivedAt { get; init; }

        public string RawRequestBody { get; init; }

        public McpRequestInfo()
        {
            ReceivedAt = DateTime.UtcNow;
        }

        public override string ToString()
        {
            return $"[MCP Request] Id={RequestId}, Tool={ToolName}, ReceivedAt={ReceivedAt:O}";
        }

        public string ToDetailedString()
        {
            return $"[MCP Request Detail] Id={RequestId}, Method={Method}, Tool={ToolName}, Arguments={Arguments}, ReceivedAt={ReceivedAt:O}";
        }
    }
}
