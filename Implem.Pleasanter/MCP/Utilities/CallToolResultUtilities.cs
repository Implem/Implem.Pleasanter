using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.MCP.Translator;
using ModelContextProtocol.Protocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net;

namespace Implem.Pleasanter.MCP.Utilities
{
    public static class CallToolResultUtilities
    {
        public static CallToolResult CreateCallToolResult(
            string text,
            bool isError = false)
        {
            return new CallToolResult
            {
                Content =
                [
                    new TextContentBlock
                    {
                        Text = text
                    }
                ],
                IsError = isError,
            };
        }

        public static string GetResultTextContent(
            CallToolResult result,
            string propertyPath)
        {
            try
            {
                if (result?.IsError == true)
                {
                    return string.Empty;
                }
                var textContentBlock = result.Content.FirstOrDefault() as TextContentBlock;
                var json = textContentBlock?.Text;
                if (string.IsNullOrWhiteSpace(json))
                {
                    return string.Empty;
                }
                var rootToken = JToken.Parse(json);
                var token = rootToken.SelectToken(propertyPath);
                return token?.ToString() ?? string.Empty;
            }
            catch (JsonReaderException)
            {
                return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static CallToolResult ToCallToolResult(
            Context context,
            ContentResultInheritance result)
        {
            var isError = IsError(result.Content);
            var text = result.Content;
            return CreateCallToolResult(
                text: text,
                isError: isError);
        }

        public static CallToolResult ToCallToolResult(
            Context context,
            ContentResultInheritance result,
            SiteSettings ss)
        {
            var isError = IsError(result.Content);
            var text = TranslateToDisplayValue(
                context: context,
                ss: ss,
                json: result.Content);
            return CreateCallToolResult(
                text: text,
                isError: isError);
        }

        public static CallToolResult ToError(
            string errorMessage,
            Exception ex = null)
        {
            var text = ex != null
                ? $"{errorMessage}: {ex.Message}"
                : errorMessage;
            return CreateCallToolResult(
                text: text,
                isError: true);
        }

        public static CallToolResult ToError(
            Context context,
            string errorMessage,
            Exception ex = null)
        {
            var text = ex != null
                ? $"{errorMessage}: {ex.Message}"
                : errorMessage;
            return CreateCallToolResult(
                text: text,
                isError: true);
        }

        public static CallToolResult ToError(
            Context context,
            Error.Types type,
            params string[] data)
        {
            var errorData = new ErrorData(type: type);
            return ToError(
                context: context,
                errorData: errorData,
                data: data);
        }

        public static CallToolResult ToError(
            Context context,
            ErrorData errorData,
            params string[] data)
        {
            var apiResponse = ApiResponses.Error(
                context: context,
                errorData: errorData,
                data: data);
            var text = ApiResults.Get(apiResponse: apiResponse).Content;
            return CreateCallToolResult(
                text: text,
                isError: true);
        }

        private static bool IsError(string json)
        {
            try
            {
                const string propertyName = "StatusCode";
                var jObject = JObject.Parse(json);
                var statusToken = jObject[propertyName];
                if (statusToken == null)
                {
                    return true;
                }
                int statusCode = statusToken.Value<int>();
                return statusCode != (int)HttpStatusCode.OK;
            }
            catch
            {
                return true;
            }
        }

        private static string TranslateToDisplayValue(
            Context context,
            SiteSettings ss,
            string json)
        {
            if (string.IsNullOrEmpty(json) ||
                ss == null)
            {
                return json;
            }
            try
            {
                var translator = new DisplayTranslator(
                    context: context,
                    ss: ss);
                return translator.TranslateApiResponse(apiResponseJson: json);
            }
            catch
            {
                throw;
            }
        }
    }
}
