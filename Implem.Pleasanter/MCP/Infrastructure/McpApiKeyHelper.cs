using Microsoft.AspNetCore.Http;
using System;

namespace Implem.Pleasanter.MCP.Infrastructure
{
    public static class McpApiKeyHelper
    {
        public static string TryGetMcpApiKey(HttpRequest request)
        {
            var headers = request.Headers;

            if (headers.TryGetValue("X-API-Key", out var xApiKey) &&
                !string.IsNullOrWhiteSpace(xApiKey))
            {
                return xApiKey.ToString();
            }

            if (headers.TryGetValue("Authorization", out var authHeader))
            {
                var value = authHeader.ToString();
                const string bearerPrefix = "Bearer ";

                if (value.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    return value[bearerPrefix.Length..];
                }
            }

            return null;
        }
    }
}
