using Microsoft.AspNetCore.Http;
using System.Threading;

namespace Implem.Pleasanter.MCP.McpContext
{
    public static class McpInfoHolder
    {
        private static readonly AsyncLocal<string> _mcpClass = new();
        private static readonly AsyncLocal<string> _mcpMethod = new();
        private static readonly AsyncLocal<string> _sessionId = new();
        private static readonly AsyncLocal<int> _tenantId = new();
        private static readonly AsyncLocal<int> _userId = new();
        private static readonly AsyncLocal<string> _clientName = new();
        private static readonly AsyncLocal<string> _clientVersion = new();
        private static readonly AsyncLocal<string> _protocolVersion = new();

        private const string HttpContextKey_TenantId = "Mcp_TenantId";
        private const string HttpContextKey_UserId = "Mcp_UserId";
        private const string HttpContextKey_ClientName = "Mcp_ClientName";
        private const string HttpContextKey_ClientVersion = "Mcp_ClientVersion";
        private const string HttpContextKey_ProtocolVersion = "Mcp_ProtocolVersion";

        public static string CurrentMcpClass
        {
            get => _mcpClass.Value ?? string.Empty;
            set => _mcpClass.Value = value;
        }

        public static string CurrentMcpMethod
        {
            get => _mcpMethod.Value ?? string.Empty;
            set => _mcpMethod.Value = value;
        }

        public static string SessionId
        {
            get => _sessionId.Value;
            set => _sessionId.Value = value;
        }

        public static int TenantId
        {
            get => _tenantId.Value;
            set => _tenantId.Value = value;
        }

        public static int UserId
        {
            get => _userId.Value;
            set => _userId.Value = value;
        }

        public static string ClientName
        {
            get => _clientName.Value;
            set => _clientName.Value = value;
        }

        public static string ClientVersion
        {
            get => _clientVersion.Value;
            set => _clientVersion.Value = value;
        }

        public static string ProtocolVersion
        {
            get => _protocolVersion.Value;
            set => _protocolVersion.Value = value;
        }

        public static void Set(string mcpClass, string mcpMethod)
        {
            CurrentMcpClass = mcpClass;
            CurrentMcpMethod = mcpMethod;
        }

        public static void SetAuthInfo(int tenantId, int userId)
        {
            TenantId = tenantId;
            UserId = userId;

            var httpContext = HttpContextAccessorHolder.Current;
            if (httpContext != null)
            {
                httpContext.Items[HttpContextKey_TenantId] = tenantId;
                httpContext.Items[HttpContextKey_UserId] = userId;
            }
        }

        public static void SetClientInfoToHttpContext(
            HttpContext httpContext,
            string clientName,
            string clientVersion,
            string protocolVersion)
        {
            if (httpContext == null) return;

            httpContext.Items[HttpContextKey_ClientName] = clientName;
            httpContext.Items[HttpContextKey_ClientVersion] = clientVersion;
            httpContext.Items[HttpContextKey_ProtocolVersion] = protocolVersion;

            ClientName = clientName;
            ClientVersion = clientVersion;
            ProtocolVersion = protocolVersion;
        }

        public static int GetTenantIdFromHttpContext(HttpContext httpContext)
        {
            if (httpContext?.Items == null) return 0;

            if (httpContext.Items.TryGetValue(HttpContextKey_TenantId, out var value) && value is int tenantId)
            {
                return tenantId;
            }

            return 0;
        }

        public static int GetUserIdFromHttpContext(HttpContext httpContext)
        {
            if (httpContext?.Items == null) return 0;

            if (httpContext.Items.TryGetValue(HttpContextKey_UserId, out var value) && value is int userId)
            {
                return userId;
            }

            return 0;
        }

        public static string GetClientNameFromHttpContext(HttpContext httpContext)
        {
            if (httpContext?.Items == null) return null;

            if (httpContext.Items.TryGetValue(HttpContextKey_ClientName, out var value) && value is string clientName)
            {
                return clientName;
            }

            return null;
        }

        public static string GetClientVersionFromHttpContext(HttpContext httpContext)
        {
            if (httpContext?.Items == null) return null;

            if (httpContext.Items.TryGetValue(HttpContextKey_ClientVersion, out var value) && value is string clientVersion)
            {
                return clientVersion;
            }

            return null;
        }

        public static string GetProtocolVersionFromHttpContext(HttpContext httpContext)
        {
            if (httpContext?.Items == null) return null;

            if (httpContext.Items.TryGetValue(HttpContextKey_ProtocolVersion, out var value) && value is string protocolVersion)
            {
                return protocolVersion;
            }

            return null;
        }

        public static void SetClientInfo(
            string sessionId,
            string clientName,
            string clientVersion,
            string protocolVersion)
        {
            SessionId = sessionId;
            ClientName = clientName;
            ClientVersion = clientVersion;
            ProtocolVersion = protocolVersion;
        }

        public static void Clear()
        {
            _mcpClass.Value = null;
            _mcpMethod.Value = null;
        }

        public static void ClearAll()
        {
            _mcpClass.Value = null;
            _mcpMethod.Value = null;
            _sessionId.Value = null;
            _tenantId.Value = 0;
            _userId.Value = 0;
            _clientName.Value = null;
            _clientVersion.Value = null;
            _protocolVersion.Value = null;
        }
    }
}
