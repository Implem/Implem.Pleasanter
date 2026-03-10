using Implem.DefinitionAccessor;
using Implem.Pleasanter.MCP.Utilities;
using ModelContextProtocol.Protocol;
using System;
using System.Collections.Generic;

namespace Implem.Pleasanter.MCP.Models
{
    public class ToolPermission
    {
        private const string ReadOnlyModeDisabledMessage =
            "ツール {0} は ReadOnlyMode では利用できません。利用可能なツール: {1}";

        private static readonly HashSet<string> ReadOnlyModeAllowedTools = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "CreateUpdateItemJson",
            "GetItem",
            "GetItems",
            "GetSite",
            "GetSiteIdByTitle",
            "GetUserIdByName",
            "GetUsers",
            "CreateViewJson",
            "GetView",
            "GetViewIdByViewName"
        };

        private string AllowedToolsText => string.Join(", ", ReadOnlyModeAllowedTools);

        private readonly string _toolName;

        public ToolPermission(string toolName)
        {
            _toolName = toolName;
        }

        public bool IsDenied()
        {
            bool isReadOnlyDenied = Parameters.McpServer.ReadOnlyMode
                && !ReadOnlyModeAllowedTools.Contains(_toolName);

            return isReadOnlyDenied;
        }

        public CallToolResult CreateDeniedResult()
        {
            var message = string.Format(
                ReadOnlyModeDisabledMessage,
                _toolName,
                AllowedToolsText);
            return CallToolResultUtilities.CreateCallToolResult(
                text: message);
        }
    }
}
