using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.MCP.McpContext;
using Implem.Pleasanter.MCP.Translator;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace Implem.Pleasanter.MCP.Utilities
{
    public static class CommonUtilities
    {
        public const string ContentTypeJson = "application/json";

        public const string Generated = "{0} を生成しました。";
        public const string ParseFailed = "{0} のパースに失敗しました。有効な JSON 形式で指定してください。";
        public const string ToolError = "{0}でエラーが発生しました";
        public const string ViewNotFound = "指定したビューID {0} のビューが見つかりませんでした。";

        private static readonly decimal ApiVersion = 1.1m;

        public enum ActionType
        {
            Copy,
            Create,
            Delete,
            Export,
            Get,
            GetByName,
            Import,
            Update
        }

        public static Context CreateContext(
            string apiRequestJson = "",
            string viewJson = "",
            long siteId = 0,
            long referenceId = 0)
        {
            var invalidApiRequestJson = IsInvalidJson(apiRequestJson);
            var invalidViewJson = IsInvalidJson(viewJson);

            if (invalidApiRequestJson ||
                invalidViewJson)
            {
                return new Context
                {
                    InvalidJsonData = true,
                    Controller = McpExecutionScope.CurrentMcpClass,
                    Action = McpExecutionScope.CurrentMcpMethod
                };
            }

            var apiRequestBody = CreateApiRequestBody(
                apiRequestJson: apiRequestJson,
                viewJson: viewJson);

            var context = new Context(
                sessionStatus: true,
                sessionData: true,
                apiRequestBody: apiRequestBody,
                contentType: ContentTypeJson,
                api: true);

            context.SiteId = siteId;
            context.Id = (referenceId != 0)
                ? referenceId
                : siteId;

            context.Controller = McpExecutionScope.CurrentMcpClass;
            context.Action = McpExecutionScope.CurrentMcpMethod;

            if (context.Authenticated)
            {
                McpInfoHolder.SetAuthInfo(
                    tenantId: context.TenantId,
                    userId: context.UserId);
            }

            return context;
        }

        public static CodeTranslator Translator(long siteId)
        {
            var context = CreateContext();
            var ss = SiteSettingsUtilities.Get(
                context: context,
                siteId: siteId);
            return new CodeTranslator(
                context: context,
                ss: ss);
        }

        private static string CreateApiRequestBody(
            string apiRequestJson = "",
            string viewJson = "")
        {
            var apiKey = GetApiKey();
            var jObject = new JObject
            {
                ["ApiVersion"] = ApiVersion,
                ["ApiKey"] = apiKey
            };

            if (!string.IsNullOrEmpty(viewJson))
            {
                jObject["View"] = JObject.Parse(viewJson);
            }

            if (!string.IsNullOrEmpty(apiRequestJson))
            {
                jObject.Merge(JObject.Parse(apiRequestJson));
            }

            return jObject.ToString(Formatting.None);
        }

        private static string GetApiKey(HttpContext httpContext = null)
        {
            return ApiKeyHolder.CurrentApiKey
                ?? GetApiKeyFromHeader(httpContext)
                ?? string.Empty;
        }

        private static string GetApiKeyFromHeader(HttpContext httpContext)
        {
            var headers = httpContext?.Request?.Headers;
            if (headers == null)
            {
                return string.Empty;
            }
            if (headers.TryGetValue("Authorization", out var authHeader))
            {
                var authValue = authHeader.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(authValue))
                {
                    return authValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                        ? authValue[7..]
                        : authValue;
                }
            }
            if (headers.TryGetValue("X-API-Key", out var apiKeyHeader))
            {
                return apiKeyHeader.FirstOrDefault() ?? string.Empty;
            }
            return string.Empty;
        }

        private static bool IsInvalidJson(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return false;
            }

            try
            {
                JObject.Parse(json);
                return false;
            }
            catch (JsonReaderException)
            {
                return true;
            }
        }
    }
}