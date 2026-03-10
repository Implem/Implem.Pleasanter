using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.MCP.McpContext;
using Implem.Pleasanter.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Implem.Pleasanter.MCP.Infrastructure
{
    public class McpContextMiddleware
    {
        private readonly RequestDelegate _next;
        private const string McpSessionIdHeader = "Mcp-Session-Id";

        public McpContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Path.StartsWithSegments(McpConstants.BasePath))
            {
                await _next(context);
                return;
            }

            var loggingEnabled = Parameters.McpServer?.Logging?.EnableLoggingToDatabase == true
                || Parameters.McpServer?.Logging?.EnableLoggingToFile == true;

            var apiKey = McpApiKeyHelper.TryGetMcpApiKey(context.Request);
            if (!string.IsNullOrEmpty(apiKey))
            {
                ApiKeyHolder.CurrentApiKey = apiKey;
            }

            HttpContextAccessorHolder.Current = context;

            var httpSessionId = context.Request.Headers[McpSessionIdHeader].ToString();

            var startTime = DateTime.UtcNow;
            var stopwatch = Stopwatch.StartNew();
            string requestBody = null;
            string responseBody = null;
            McpRequestInfo mcpRequestInfo = null;

            if (context.Request.Method == "POST")
            {
                mcpRequestInfo = await TryExtractMcpRequestInfo(context);

                if (mcpRequestInfo != null)
                {
                    McpRequestContextHolder.Current = mcpRequestInfo;
                    requestBody = mcpRequestInfo.RawRequestBody;
                }
            }

            Stream originalBodyStream = null;
            MemoryStream responseBodyStream = null;

            if (loggingEnabled && context.Request.Method == "POST")
            {
                originalBodyStream = context.Response.Body;
                responseBodyStream = new MemoryStream();
                context.Response.Body = responseBodyStream;
            }

            try
            {
                await _next(context);

                if (responseBodyStream != null)
                {
                    responseBodyStream.Seek(0, SeekOrigin.Begin);
                    responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();
                    responseBodyStream.Seek(0, SeekOrigin.Begin);
                    await responseBodyStream.CopyToAsync(originalBodyStream);
                }

                if (mcpRequestInfo?.Method == "initialize" && string.IsNullOrEmpty(httpSessionId))
                {
                    httpSessionId = context.Response.Headers[McpSessionIdHeader].ToString();
                }
            }
            catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
            {
                if (loggingEnabled && context.Request.Method == "POST" && mcpRequestInfo != null)
                {
                    var canceledPayload = JsonSerializer.Serialize(new
                    {
                        result = new
                        {
                            content = new[]
                            {
                                new { type = "text", text = "Operation was canceled." }
                            },
                            isError = true
                        },
                        id = mcpRequestInfo.RequestId,
                        jsonrpc = "2.0"
                    });
                    responseBody = $"event: message\ndata: {canceledPayload}\n\n";
                    context.Response.StatusCode = StatusCodes.Status499ClientClosedRequest;
                }
            }
            finally
            {
                stopwatch.Stop();
                
                if (loggingEnabled && context.Request.Method == "POST" && mcpRequestInfo != null)
                {
                    var endTime = DateTime.UtcNow;
                    var elapsed = stopwatch.Elapsed.TotalMilliseconds;

                    var statusCode = context.Response.StatusCode;
                    var userHostAddress = context.Connection?.RemoteIpAddress?.ToString();
                    var userAgent = context.Request.Headers["User-Agent"].ToString();


                    var tenantId = McpInfoHolder.GetTenantIdFromHttpContext(context);
                    var userId = McpInfoHolder.GetUserIdFromHttpContext(context);

                    var logClientName = McpInfoHolder.GetClientNameFromHttpContext(context);
                    var logClientVersion = McpInfoHolder.GetClientVersionFromHttpContext(context);
                    var logProtocolVersion = McpInfoHolder.GetProtocolVersionFromHttpContext(context);

                    _ = Task.Run(() => SaveMcpLog(
                        statusCode: statusCode,
                        userHostAddress: userHostAddress,
                        userAgent: userAgent,
                        mcpRequestInfo: mcpRequestInfo,
                        sessionId: httpSessionId,
                        clientName: logClientName,
                        clientVersion: logClientVersion,
                        protocolVersion: logProtocolVersion,
                        tenantId: tenantId,
                        userId: userId,
                        apiKey: apiKey,
                        startTime: startTime,
                        endTime: endTime,
                        elapsed: elapsed,
                        requestBody: requestBody,
                        responseBody: responseBody));
                }

                if (responseBodyStream != null)
                {
                    context.Response.Body = originalBodyStream;
                    await responseBodyStream.DisposeAsync();
                }

                McpRequestContextHolder.Clear();
                ApiKeyHolder.Clear();
                McpInfoHolder.ClearAll();
                HttpContextAccessorHolder.Clear();
            }
        }

        private static void SaveMcpLog(
            int statusCode,
            string userHostAddress,
            string userAgent,
            McpRequestInfo mcpRequestInfo,
            string sessionId,
            string clientName,
            string clientVersion,
            string protocolVersion,
            int tenantId,
            int userId,
            string apiKey,
            DateTime startTime,
            DateTime endTime,
            double elapsed,
            string requestBody,
            string responseBody)
        {
            try
            {
                var context = new Context(
                    request: false,
                    sessionStatus: false,
                    sessionData: false,
                    user: false,
                    item: false);

                var mcpLog = new McpLogModel
                {
                    StartTime = startTime,
                    EndTime = endTime,
                    McpRequestId = mcpRequestInfo.RequestId,
                    McpSessionId = sessionId,
                    McpMethod = mcpRequestInfo.Method ?? string.Empty,
                    TargetName = GetTargetName(mcpRequestInfo),
                    TenantId = tenantId,
                    UserId = Libraries.Server.SiteInfo.User(context: context, userId: userId),
                    ApiKeyPrefix = GetApiKeyPrefix(apiKey),
                    ClientName = clientName,
                    ClientVersion = clientVersion,
                    ProtocolVersion = protocolVersion,
                    Elapsed = elapsed,
                    Status = statusCode,
                    JsonRpcErrorCode = ExtractJsonRpcErrorCode(responseBody),
                    ErrMessage = ExtractErrorMessage(responseBody),
                    RequestData = requestBody,
                    ResponseData = TruncateResponseData(responseBody),
                    UserHostAddress = userHostAddress,
                    UserAgent = userAgent
                };

                mcpLog.Save(context);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"[MCP] SaveMcpLog failed: {ex.Message}");
            }
        }

        private static string GetTargetName(McpRequestInfo mcpRequestInfo)
        {
            if (mcpRequestInfo == null) return null;

            return mcpRequestInfo.Method switch
            {
                "tools/call" => mcpRequestInfo.ToolName,
                "prompts/get" => ExtractPromptName(mcpRequestInfo.Arguments),
                _ => null
            };
        }

        private static string ExtractPromptName(string arguments)
        {
            if (string.IsNullOrEmpty(arguments)) return null;

            try
            {
                using var doc = JsonDocument.Parse(arguments);
                if (doc.RootElement.TryGetProperty("name", out var nameElement))
                {
                    return nameElement.GetString();
                }
            }
            catch { }

            return null;
        }

        private static string GetApiKeyPrefix(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey)) return null;
            return apiKey.Length > 16 ? apiKey[..16] : apiKey;
        }

        private static string ExtractJsonRpcPayload(string responseBody)
        {
            if (string.IsNullOrEmpty(responseBody)) return null;

            var lastDataIndex = responseBody.LastIndexOf("data: ");
            if (lastDataIndex >= 0)
            {
                var jsonStart = lastDataIndex + "data: ".Length;
                var lineEnd = responseBody.IndexOf('\n', jsonStart);
                return lineEnd >= 0
                    ? responseBody[jsonStart..lineEnd].TrimEnd('\r')
                    : responseBody[jsonStart..].Trim();
            }

            return responseBody.Trim();
        }

        private static int ExtractJsonRpcErrorCode(string responseBody)
        {
            var json = ExtractJsonRpcPayload(responseBody);
            if (string.IsNullOrEmpty(json)) return 0;

            try
            {
                using var doc = JsonDocument.Parse(json);

                if (doc.RootElement.TryGetProperty("error", out var errorElement)
                    && errorElement.TryGetProperty("code", out var codeElement))
                {
                    return codeElement.GetInt32();
                }

                if (doc.RootElement.TryGetProperty("result", out var resultElement)
                    && resultElement.TryGetProperty("isError", out var isErrorElement)
                    && isErrorElement.ValueKind == JsonValueKind.True)
                {
                    return -1;
                }
            }
            catch { }

            return 0;
        }

        private static string ExtractErrorMessage(string responseBody)
        {
            var json = ExtractJsonRpcPayload(responseBody);
            if (string.IsNullOrEmpty(json)) return null;

            try
            {
                using var doc = JsonDocument.Parse(json);

                if (doc.RootElement.TryGetProperty("error", out var errorElement)
                    && errorElement.TryGetProperty("message", out var messageElement))
                {
                    return messageElement.GetString();
                }

                if (doc.RootElement.TryGetProperty("result", out var resultElement)
                    && resultElement.TryGetProperty("isError", out var isErrorElement)
                    && isErrorElement.ValueKind == JsonValueKind.True
                    && resultElement.TryGetProperty("content", out var contentElement)
                    && contentElement.ValueKind == JsonValueKind.Array
                    && contentElement.GetArrayLength() > 0)
                {
                    var firstContent = contentElement[0];
                    if (firstContent.TryGetProperty("text", out var textElement))
                    {
                        return textElement.GetString();
                    }
                }
            }
            catch { }

            return null;
        }

        private static string TruncateResponseData(string responseBody)
        {
            if (string.IsNullOrEmpty(responseBody)) return null;

            var maxLength = Parameters.McpServer?.Logging?.ResponseDataMaxLength ?? 65536;
            if (maxLength <= 0) return responseBody;

            return responseBody.Length > maxLength
                ? responseBody[..maxLength] + "...(truncated)"
                : responseBody;
        }

        private async Task<McpRequestInfo> TryExtractMcpRequestInfo(HttpContext context)
        {
            try
            {
                context.Request.EnableBuffering();

                using var reader = new StreamReader(
                    context.Request.Body,
                    encoding: System.Text.Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    bufferSize: 1024,
                    leaveOpen: true);

                var requestBody = await reader.ReadToEndAsync();

                context.Request.Body.Position = 0;

                if (string.IsNullOrWhiteSpace(requestBody))
                {
                    return null;
                }

                return ParseMcpRequestBody(context, requestBody);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private McpRequestInfo ParseMcpRequestBody(HttpContext httpContext, string requestBody)
        {
            try
            {
                using var document = JsonDocument.Parse(requestBody);
                var root = document.RootElement;

                var requestId = root.TryGetProperty("id", out var idElement)
                    ? idElement.ToString()
                    : null;

                var method = root.TryGetProperty("method", out var methodElement)
                    ? methodElement.GetString()
                    : null;

                string toolName = null;
                string arguments = null;

                if (method == "tools/call" && root.TryGetProperty("params", out var paramsElement))
                {
                    if (paramsElement.TryGetProperty("name", out var nameElement))
                    {
                        toolName = nameElement.GetString();
                    }

                    if (paramsElement.TryGetProperty("arguments", out var argsElement))
                    {
                        arguments = argsElement.ToString();
                    }
                }

                if (method == "initialize" && root.TryGetProperty("params", out var initParams))
                {
                    ExtractAndSetClientInfo(httpContext, initParams);
                }

                return new McpRequestInfo
                {
                    RequestId = requestId,
                    Method = method,
                    ToolName = toolName,
                    Arguments = arguments,
                    RawRequestBody = requestBody,
                    ReceivedAt = DateTime.UtcNow
                };
            }
            catch
            {
                return null;
            }
        }

        private void ExtractAndSetClientInfo(HttpContext httpContext, JsonElement initParams)
        {
            try
            {
                string clientName = null;
                string clientVersion = null;
                string protocolVersion = null;

                if (initParams.TryGetProperty("protocolVersion", out var pvElement))
                {
                    protocolVersion = pvElement.GetString();
                }

                if (initParams.TryGetProperty("clientInfo", out var clientInfoElement))
                {
                    if (clientInfoElement.TryGetProperty("name", out var nameElement))
                    {
                        clientName = nameElement.GetString();
                    }
                    if (clientInfoElement.TryGetProperty("version", out var versionElement))
                    {
                        clientVersion = versionElement.GetString();
                    }
                }

                McpInfoHolder.SetClientInfoToHttpContext(
                    httpContext: httpContext,
                    clientName: clientName,
                    clientVersion: clientVersion,
                    protocolVersion: protocolVersion);
            }
            catch
            {
            }
        }
    }

    public static class McpContextMiddlewareExtensions
    {
        public static IApplicationBuilder UseMcpContextMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<McpContextMiddleware>();
        }
    }
}
