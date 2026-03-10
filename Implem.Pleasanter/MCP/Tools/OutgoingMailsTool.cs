using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.MCP.McpContext;
using Implem.Pleasanter.MCP.Models;
using Implem.Pleasanter.MCP.Utilities;
using Implem.Pleasanter.Models;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static Implem.Pleasanter.MCP.Utilities.CommonUtilities;

namespace Implem.Pleasanter.MCP.Tools
{
    [McpServerToolType]
    [Description(@"
Pleasanter のメール送信を行うツール群です。

【送信】SendEmail でアイテムに関連するメールを送信")]
    public class OutgoingMailsTool
    {
        private const string ClassName = nameof(OutgoingMailsTool);

        [McpServerTool(Name = "SendEmail")]
        [Description("指定アイテムに関連するメールを送信します。To, Cc, Bcc, Title, Bodyを指定可能。")]
        public static async Task<CallToolResult> SendEmail(
            [Description("サイトの種類(issues, results等)")]
                string reference,
            [Description("アイテムのID")]
                long id,
            [Description("送信先メールアドレス(複数の場合はカンマ区切り)")]
                string to = null,
            [Description("Ccメールアドレス(複数の場合はカンマ区切り)")]
                string cc = null,
            [Description("Bccメールアドレス(複数の場合はカンマ区切り)")]
                string bcc = null,
            [Description("メールのタイトル")]
                string title = null,
            [Description("メールの本文")]
                string body = null)
        {
            var toolPermission = new ToolPermission(nameof(SendEmail));
            if (toolPermission.IsDenied())
            {
                return toolPermission.CreateDeniedResult();
            }
            using var scope = new McpExecutionScope(
                mcpClass: ClassName,
                mcpMethod: nameof(SendEmail));

            List<AttachmentData> attachments = new List<AttachmentData>();

            var apiRequestJson = CreateApiRequestJson(
                to: to,
                cc: cc,
                bcc: bcc,
                title: title,
                body: body,
                attachments: attachments);

            var context = CreateContext(apiRequestJson: apiRequestJson);

            try
            {
                if (!context.Authenticated)
                {
                    return CallToolResultUtilities.ToError(
                        context: context,
                        type: Error.Types.Unauthorized);
                }

                if (!TenantQuotaUsagesUtilities.TryWithinQuotaKeyLimit(
                        context: context,
                        quotaKey: QuotaKeys.McpRequests,
                        errorType: out var errorType,
                        errorData: out var errorData))
                {
                    return CallToolResultUtilities.ToError(
                        context: context,
                        type: errorType,
                        data: errorData ?? Array.Empty<string>());
                }

                var result = OutgoingMailUtilities.SendByApi(
                    context: context,
                    reference: reference,
                    id: id);

                return CallToolResultUtilities.ToCallToolResult(
                    context: context,
                    result: result);
            }
            catch (Exception ex)
            {
                return CallToolResultUtilities.ToError(
                    context: context,
                    type: Error.Types.InternalServerError,
                    data: ex.Message);
            }
        }

        private static async Task<List<AttachmentData>> ProcessAttachmentsAsync(string[] attachmentPaths)
        {
            if (attachmentPaths == null || attachmentPaths.Length == 0)
            {
                return null;
            }

            var attachments = new List<AttachmentData>();

            foreach (var path in attachmentPaths)
            {
                if (string.IsNullOrWhiteSpace(path))
                {
                    continue;
                }

                try
                {
                    AttachmentData attachment;

                    if (IsUrl(path))
                    {
                        attachment = await DownloadFromUrlAsync(path);
                    }
                    else
                    {
                        attachment = ReadFromLocalFile(path);
                    }

                    if (attachment != null)
                    {
                        attachments.Add(attachment);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"添付ファイルの処理に失敗しました ({path}): {ex.Message}", ex);
                }
            }

            return attachments.Count > 0 ? attachments : null;
        }

        private static bool IsUrl(string path)
        {
            return Uri.TryCreate(path, UriKind.Absolute, out var uri) &&
                   (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }

        private static async Task<AttachmentData> DownloadFromUrlAsync(string url)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromSeconds(30);

                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var fileBytes = await response.Content.ReadAsByteArrayAsync();
                var base64Content = Convert.ToBase64String(fileBytes);

                var fileName = GetFileNameFromResponse(response, url);

                var contentType = response.Content.Headers.ContentType?.MediaType;
                if (string.IsNullOrEmpty(contentType))
                {
                    contentType = GetContentTypeFromFileName(fileName);
                }

                return new AttachmentData
                {
                    Name = fileName,
                    Base64 = base64Content,
                    ContentType = contentType
                };
            }
        }

        private static string GetFileNameFromResponse(HttpResponseMessage response, string url)
        {
            if (response.Content.Headers.ContentDisposition?.FileName != null)
            {
                return response.Content.Headers.ContentDisposition.FileName.Trim('"');
            }

            var uri = new Uri(url);
            var fileName = Path.GetFileName(uri.LocalPath);

            if (string.IsNullOrEmpty(fileName) || fileName == "/")
            {
                var contentType = response.Content.Headers.ContentType?.MediaType;
                var extension = GetExtensionFromContentType(contentType);
                fileName = $"download{extension}";
            }

            return fileName;
        }

        private static string GetExtensionFromContentType(string contentType)
        {
            if (string.IsNullOrEmpty(contentType))
            {
                return ".bin";
            }

            return contentType.ToLowerInvariant() switch
            {
                "image/jpeg" => ".jpg",
                "image/png" => ".png",
                "image/gif" => ".gif",
                "image/webp" => ".webp",
                "application/pdf" => ".pdf",
                "text/plain" => ".txt",
                "text/html" => ".html",
                "application/json" => ".json",
                "application/xml" => ".xml",
                "application/zip" => ".zip",
                _ => ".bin"
            };
        }

        private static AttachmentData ReadFromLocalFile(string path)
        {
            var fullPath = Path.GetFullPath(path);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"ファイルが見つかりません: {fullPath}");
            }

            var fileBytes = File.ReadAllBytes(fullPath);
            var base64Content = Convert.ToBase64String(fileBytes);

            var fileName = Path.GetFileName(fullPath);

            var contentType = GetContentTypeFromFileName(fileName);

            return new AttachmentData
            {
                Name = fileName,
                Base64 = base64Content,
                ContentType = contentType
            };
        }

        private static string GetContentTypeFromFileName(string fileName)
        {
            var extension = Path.GetExtension(fileName)?.ToLowerInvariant();

            return extension switch
            {
                ".txt" => "text/plain",
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".ppt" => "application/vnd.ms-powerpoint",
                ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                ".bmp" => "image/bmp",
                ".svg" => "image/svg+xml",
                ".zip" => "application/zip",
                ".rar" => "application/x-rar-compressed",
                ".7z" => "application/x-7z-compressed",
                ".csv" => "text/csv",
                ".json" => "application/json",
                ".xml" => "application/xml",
                ".html" or ".htm" => "text/html",
                ".css" => "text/css",
                ".js" => "application/javascript",
                ".mp4" => "video/mp4",
                ".mp3" => "audio/mpeg",
                ".wav" => "audio/wav",
                _ => "application/octet-stream" // デフォルト
            };
        }

        private static string CreateApiRequestJson(
            string to,
            string cc,
            string bcc,
            string title,
            string body,
            List<AttachmentData> attachments)
        {
            var apiRequest = new
            {
                From = (string)null,
                To = to,
                Cc = cc,
                Bcc = bcc,
                Title = title,
                Body = body,
                Attachments = attachments?.Select(a => new
                {
                    Name = a.Name,
                    Base64 = a.Base64,
                    ContentType = a.ContentType
                }).ToArray()
            };

            return apiRequest.ToJson();
        }

        private class AttachmentData
        {
            public string Name { get; set; }
            public string Base64 { get; set; }
            public string ContentType { get; set; }
        }
    }
}
