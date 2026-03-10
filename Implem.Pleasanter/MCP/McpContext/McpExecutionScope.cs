using System;
using System.Threading;

namespace Implem.Pleasanter.MCP.McpContext
{
    public sealed class McpExecutionScope : IDisposable
    {
        private static readonly AsyncLocal<McpExecutionContext> _current = new();

        private readonly McpExecutionContext _previous;
        private bool _disposed;

        public McpExecutionScope(string mcpClass, string mcpMethod)
        {
            _previous = _current.Value;

            _current.Value = new McpExecutionContext(
                mcpClass: mcpClass,
                mcpMethod: mcpMethod);
        }

        public static McpExecutionContext Current => _current.Value;

        public static string CurrentMcpClass => _current.Value?.McpClass ?? string.Empty;

        public static string CurrentMcpMethod => _current.Value?.McpMethod ?? string.Empty;

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _current.Value = _previous;
        }
    }

    public sealed class McpExecutionContext
    {
        public string McpClass { get; }

        public string McpMethod { get; }

        public DateTime CreatedAt { get; }

        public McpExecutionContext(string mcpClass, string mcpMethod)
        {
            McpClass = mcpClass ?? string.Empty;
            McpMethod = mcpMethod ?? string.Empty;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
