using Microsoft.AspNetCore.Http;
using System.Threading;

namespace Implem.Pleasanter.MCP.McpContext
{
    public static class HttpContextAccessorHolder
    {
        private static readonly AsyncLocal<HttpContext> _httpContext = new();

        public static HttpContext Current
        {
            get => _httpContext.Value;
            set => _httpContext.Value = value;
        }

        public static void Clear()
        {
            _httpContext.Value = null;
        }
    }
}
