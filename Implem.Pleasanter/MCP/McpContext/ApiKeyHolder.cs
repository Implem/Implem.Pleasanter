using System.Threading;

namespace Implem.Pleasanter.MCP.McpContext
{
    public static class ApiKeyHolder
    {
        private static readonly AsyncLocal<string> ApiKey = new();

        public static string CurrentApiKey
        {
            get => ApiKey.Value ?? string.Empty;
            set => ApiKey.Value = value;
        }

        public static void Clear()
        {
            ApiKey.Value = null;
        }
    }
}