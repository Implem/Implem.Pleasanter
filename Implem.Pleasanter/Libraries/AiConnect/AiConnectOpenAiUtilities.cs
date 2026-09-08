using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace Implem.Pleasanter.Libraries.AiConnect
{
    public static class AiConnectOpenAiUtilities
    {
        private const int BodyLogLength = 1000;
        private const string CompletedStatus = "completed";
        private const string Purpose = "assistants";
        private const string PurposePartName = "purpose";
        private const string PurposeContentType = "text/plain";

        public static AiConnectSendResult SendCore(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            string referenceType,
            long referenceId,
            string filePath,
            IAiConnectHttpClient client = null,
            bool forceCreate = false)
        {
            var httpClient = client ?? AiConnectHttpClient.Default;
            var fileName = Path.GetFileName(filePath);
            var setting = Setting(connectionSetting: aiProvider?.ConnectionSetting);
            if (setting == null)
            {
                return Failed(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    referenceType: referenceType,
                    referenceId: referenceId,
                    fileName: fileName,
                    phase: Phases.Setting,
                    response: null,
                    fileId: null,
                    message: "The connection setting is invalid.",
                    sysLogType: SysLogModel.SysLogTypes.UserError,
                    type: Error.Types.InvalidAiProviderSetting);
            }
            var oldFileIds = new List<string>();
            if (forceCreate == false)
            {
                oldFileIds = FileIds(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    referenceType: referenceType,
                    referenceId: referenceId,
                    setting: setting,
                    fileName: fileName,
                    client: httpClient,
                    failed: out var listFailed);
                if (oldFileIds == null)
                {
                    return Failed(
                        context: context,
                        ss: ss,
                        aiProvider: aiProvider,
                        referenceType: referenceType,
                        referenceId: referenceId,
                        fileName: fileName,
                        phase: Phases.List,
                        response: listFailed,
                        fileId: null,
                        message: "Failed to get the file list.",
                        sysLogType: SysLogType(response: listFailed));
                }
            }
            var uploaded = Retry429(
                context: context,
                ss: ss,
                aiProvider: aiProvider,
                referenceType: referenceType,
                referenceId: referenceId,
                fileName: fileName,
                phase: Phases.Upload,
                fileId: null,
                send: () => Upload(
                    setting: setting,
                    filePath: filePath,
                    client: httpClient));
            if (uploaded.Succeeded == false)
            {
                return Failed(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    referenceType: referenceType,
                    referenceId: referenceId,
                    fileName: fileName,
                    phase: Phases.Upload,
                    response: uploaded,
                    fileId: null,
                    message: "Failed to upload the file.",
                    sysLogType: SysLogType(response: uploaded));
            }
            var fileId = Value(
                json: Json(body: uploaded.Body),
                key: "id");
            if (fileId.IsNullOrEmpty())
            {
                return Failed(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    referenceType: referenceType,
                    referenceId: referenceId,
                    fileName: fileName,
                    phase: Phases.Upload,
                    response: uploaded,
                    fileId: null,
                    message: "The file id is not returned.",
                    sysLogType: SysLogModel.SysLogTypes.SystemError);
            }
            var attached = Retry429(
                context: context,
                ss: ss,
                aiProvider: aiProvider,
                referenceType: referenceType,
                referenceId: referenceId,
                fileName: fileName,
                phase: Phases.Attach,
                fileId: fileId,
                send: () => Attach(
                    setting: setting,
                    fileId: fileId,
                    client: httpClient));
            if (attached.Succeeded == false)
            {
                Cleanup(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    referenceType: referenceType,
                    referenceId: referenceId,
                    fileName: fileName,
                    setting: setting,
                    fileId: fileId,
                    client: httpClient);
                return Failed(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    referenceType: referenceType,
                    referenceId: referenceId,
                    fileName: fileName,
                    phase: Phases.Attach,
                    response: attached,
                    fileId: fileId,
                    message: "Failed to attach the file to the vector store.",
                    sysLogType: SysLogType(response: attached));
            }
            var status = Value(
                json: Json(body: attached.Body),
                key: "status");
            var deferred = DeferOldFileDeletion(status: status);
            if (deferred == false)
            {
                oldFileIds.ForEach(oldFileId => DeleteOldFile(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    referenceType: referenceType,
                    referenceId: referenceId,
                    fileName: fileName,
                    setting: setting,
                    fileId: oldFileId,
                    client: httpClient));
            }
            if (Parameters.AiConnect?.Rag.LogSucceeded != false)
            {
                Log(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    referenceType: referenceType,
                    referenceId: referenceId,
                    fileName: fileName,
                    phase: Phases.Attach,
                    response: attached,
                    fileId: fileId,
                    status: status,
                    message: deferred && oldFileIds.Any()
                        ? AiConnectQueueUtilities.CanEnqueueIndexCheck()
                            ? "The document has been sent."
                                + " The old files are kept until the indexing is completed."
                            : "The document has been sent."
                                + " The old files are kept until the next update or synchronization"
                                + " because the indexing check is not available."
                        : "The document has been sent.",
                    sysLogType: SysLogModel.SysLogTypes.Info);
            }
            return new AiConnectSendResult
            {
                ErrorData = new ErrorData(type: Error.Types.None),
                Indexing = new AiConnectIndexingInfo
                {
                    FileId = fileId,
                    Status = status,
                    OldFileIds = deferred
                        ? oldFileIds
                        : new List<string>()
                }
            };
        }

        public static AiConnectHttpResponse VectorStoreFile(
            AiProvider aiProvider,
            string fileId,
            IAiConnectHttpClient client = null)
        {
            var setting = Setting(connectionSetting: aiProvider?.ConnectionSetting);
            if (setting == null)
            {
                return null;
            }
            var httpClient = client ?? AiConnectHttpClient.Default;
            return httpClient.Get(
                url: Url(
                    setting: setting,
                    path: $"vector_stores/{setting.VectorStoreId}/files/{fileId}"),
                headers: Headers(setting: setting));
        }

        public static List<string> FileIds(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            string fileName,
            IAiConnectHttpClient client = null)
        {
            return FileIds(
                context: context,
                ss: ss,
                aiProvider: aiProvider,
                fileName: fileName,
                client: client,
                errorType: out _);
        }

        internal static List<string> FileIds(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            string fileName,
            IAiConnectHttpClient client,
            out Error.Types errorType)
        {
            errorType = Error.Types.None;
            var httpClient = client ?? AiConnectHttpClient.Default;
            var setting = Setting(connectionSetting: aiProvider?.ConnectionSetting);
            if (setting == null)
            {
                errorType = Error.Types.InvalidAiProviderSetting;
                LogFiles(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    fileName: fileName,
                    phase: Phases.Setting,
                    response: null,
                    fileId: null,
                    message: "The connection setting is invalid.",
                    sysLogType: SysLogModel.SysLogTypes.UserError);
                return null;
            }
            var fileIds = FileIds(
                context: context,
                ss: ss,
                aiProvider: aiProvider,
                referenceType: null,
                referenceId: 0,
                setting: setting,
                fileName: fileName,
                client: httpClient,
                failed: out var listFailed);
            if (fileIds == null)
            {
                errorType = Error.Types.ServerConnectionError;
                LogFiles(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    fileName: fileName,
                    phase: Phases.List,
                    response: listFailed,
                    fileId: null,
                    message: "Failed to get the file list.",
                    sysLogType: SysLogType(response: listFailed));
            }
            return fileIds;
        }

        public static List<string> FileIdsByPrefix(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            IAiConnectHttpClient client = null)
        {
            return FileIdsByPrefix(
                context: context,
                ss: ss,
                aiProvider: aiProvider,
                client: client,
                errorType: out _);
        }

        internal static List<string> FileIdsByPrefix(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            IAiConnectHttpClient client,
            out Error.Types errorType)
        {
            errorType = Error.Types.None;
            var httpClient = client ?? AiConnectHttpClient.Default;
            var setting = Setting(connectionSetting: aiProvider?.ConnectionSetting);
            if (setting == null)
            {
                errorType = Error.Types.InvalidAiProviderSetting;
                LogFiles(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    fileName: null,
                    phase: Phases.Setting,
                    response: null,
                    fileId: null,
                    message: "The connection setting is invalid.",
                    sysLogType: SysLogModel.SysLogTypes.UserError);
                return null;
            }
            var files = Files(
                context: context,
                ss: ss,
                aiProvider: aiProvider,
                referenceType: null,
                referenceId: 0,
                fileName: null,
                setting: setting,
                client: httpClient,
                failed: out var listFailed);
            if (files == null)
            {
                errorType = Error.Types.ServerConnectionError;
                LogFiles(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    fileName: null,
                    phase: Phases.List,
                    response: listFailed,
                    fileId: null,
                    message: "Failed to get the file list.",
                    sysLogType: SysLogType(response: listFailed));
                return null;
            }
            var prefix = AiConnectUtilities.FileNamePrefix(
                context: context,
                ss: ss,
                aiProvider: aiProvider);
            var fileIds = files
                .Where(o => o.FileName?.StartsWith(prefix, StringComparison.Ordinal) == true)
                .Select(o => o.Id)
                .Where(o => o.IsNullOrEmpty() == false)
                .ToList();
            LogFiles(
                context: context,
                ss: ss,
                aiProvider: aiProvider,
                fileName: null,
                phase: Phases.List,
                response: null,
                fileId: null,
                message: $"MatchedCount: {fileIds.Count}"
                    + $", UnmatchedCount: {files.Count - fileIds.Count}",
                sysLogType: SysLogModel.SysLogTypes.Info);
            return fileIds;
        }

        public static AiConnectHttpResponse DeleteFile(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            string fileId,
            IAiConnectHttpClient client = null)
        {
            var httpClient = client ?? AiConnectHttpClient.Default;
            var setting = Setting(connectionSetting: aiProvider?.ConnectionSetting);
            if (setting == null)
            {
                LogFiles(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    fileName: null,
                    phase: Phases.Setting,
                    response: null,
                    fileId: fileId,
                    message: "The connection setting is invalid.",
                    sysLogType: SysLogModel.SysLogTypes.UserError);
                return new AiConnectHttpResponse();
            }
            var response = Retry429(
                context: context,
                ss: ss,
                aiProvider: aiProvider,
                referenceType: null,
                referenceId: 0,
                fileName: null,
                phase: Phases.Delete,
                fileId: fileId,
                send: () => Delete(
                    setting: setting,
                    fileId: fileId,
                    client: httpClient));
            if (response.Succeeded == false)
            {
                LogFiles(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    fileName: null,
                    phase: Phases.Delete,
                    response: response,
                    fileId: fileId,
                    message: "Failed to delete the file.",
                    sysLogType: SysLogType(response: response));
            }
            return response;
        }

        public static ErrorData ValidateConnectionSetting(
            Context context,
            AiProvider aiProvider)
        {
            return AiConnectUtilities.ValidateConnectionSetting(
                connectionSetting: aiProvider?.ConnectionSetting);
        }

        private static OpenAiConnectionSetting Setting(string connectionSetting)
        {
            if (connectionSetting.IsNullOrEmpty())
            {
                return null;
            }
            var setting = connectionSetting.Deserialize<OpenAiConnectionSetting>();
            if (setting == null
                || setting.VectorStoreId.IsNullOrEmpty()
                || setting.ApiKey.IsNullOrEmpty())
            {
                return null;
            }
            if (setting.Endpoint.IsNullOrEmpty())
            {
                setting.Endpoint = AiProviderUtilities.ConnectionSettingEndpoint(
                    providerType: AiProvider.Types.OpenAi);
            }
            if (setting.Endpoint.IsNullOrEmpty())
            {
                return null;
            }
            if (AiConnectUtilities.AllowedEndpoint(endpoint: setting.Endpoint) == false)
            {
                return null;
            }
            return setting;
        }

        private static List<string> FileIds(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            string referenceType,
            long referenceId,
            OpenAiConnectionSetting setting,
            string fileName,
            IAiConnectHttpClient client,
            out AiConnectHttpResponse failed)
        {
            var files = Files(
                context: context,
                ss: ss,
                aiProvider: aiProvider,
                referenceType: referenceType,
                referenceId: referenceId,
                fileName: fileName,
                setting: setting,
                client: client,
                failed: out failed);
            if (files == null)
            {
                return null;
            }
            var fileIds = files
                .Where(o => string.Equals(
                    o.FileName,
                    fileName,
                    StringComparison.Ordinal))
                .Select(o => o.Id)
                .Where(o => o.IsNullOrEmpty() == false)
                .ToList();
            if (fileIds.Count > 1)
            {
                Log(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    referenceType: referenceType,
                    referenceId: referenceId,
                    fileName: fileName,
                    phase: Phases.List,
                    response: null,
                    fileId: null,
                    status: null,
                    message: $"{fileIds.Count} files have the same name.",
                    sysLogType: SysLogModel.SysLogTypes.Warning);
            }
            return fileIds;
        }

        private static List<OpenAiFile> Files(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            string referenceType,
            long referenceId,
            string fileName,
            OpenAiConnectionSetting setting,
            IAiConnectHttpClient client,
            out AiConnectHttpResponse failed)
        {
            failed = null;
            var files = new List<OpenAiFile>();
            var after = (string)null;
            while (true)
            {
                var path = $"files?purpose={Purpose}&limit=100";
                if (after.IsNullOrEmpty() == false)
                {
                    path += $"&after={Uri.EscapeDataString(after)}";
                }
                var response = Retry429(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    referenceType: referenceType,
                    referenceId: referenceId,
                    fileName: fileName,
                    phase: Phases.List,
                    fileId: null,
                    send: () => client.Get(
                        url: Url(
                            setting: setting,
                            path: path),
                        headers: Headers(setting: setting)));
                var json = response.Succeeded
                    ? Json(body: response.Body)
                    : null;
                if (json == null)
                {
                    failed = response;
                    return null;
                }
                files.AddRange(Data(json: json).Select(o => new OpenAiFile
                {
                    Id = Value(
                        json: o,
                        key: "id"),
                    FileName = Value(
                        json: o,
                        key: "filename")
                }));
                if (json.Property("has_more")?.Value?.Value<bool>() != true)
                {
                    break;
                }
                after = Value(
                    json: json,
                    key: "last_id");
                if (after.IsNullOrEmpty())
                {
                    break;
                }
            }
            return files;
        }

        private static AiConnectHttpResponse Upload(
            OpenAiConnectionSetting setting,
            string filePath,
            IAiConnectHttpClient client)
        {
            return client.PostFile(
                url: Url(
                    setting: setting,
                    path: "files"),
                headers: Headers(setting: setting),
                data: Purpose,
                filePath: filePath,
                dataPartName: PurposePartName,
                dataContentType: PurposeContentType);
        }

        private static AiConnectHttpResponse Attach(
            OpenAiConnectionSetting setting,
            string fileId,
            IAiConnectHttpClient client)
        {
            return client.PostJson(
                url: Url(
                    setting: setting,
                    path: $"vector_stores/{setting.VectorStoreId}/files"),
                headers: Headers(setting: setting),
                json: new JObject
                {
                    ["file_id"] = fileId
                }.ToString(Formatting.None));
        }

        private static AiConnectHttpResponse Delete(
            OpenAiConnectionSetting setting,
            string fileId,
            IAiConnectHttpClient client)
        {
            return client.Delete(
                url: Url(
                    setting: setting,
                    path: $"files/{fileId}"),
                headers: Headers(setting: setting));
        }

        private static void Cleanup(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            string referenceType,
            long referenceId,
            string fileName,
            OpenAiConnectionSetting setting,
            string fileId,
            IAiConnectHttpClient client)
        {
            var response = Retry429(
                context: context,
                ss: ss,
                aiProvider: aiProvider,
                referenceType: referenceType,
                referenceId: referenceId,
                fileName: fileName,
                phase: Phases.Cleanup,
                fileId: fileId,
                send: () => Delete(
                    setting: setting,
                    fileId: fileId,
                    client: client));
            if (response.Succeeded == false)
            {
                Log(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    referenceType: referenceType,
                    referenceId: referenceId,
                    fileName: fileName,
                    phase: Phases.Cleanup,
                    response: response,
                    fileId: fileId,
                    status: null,
                    message: "Failed to delete the uploaded file.",
                    sysLogType: SysLogModel.SysLogTypes.Warning);
            }
        }

        private static bool DeferOldFileDeletion(string status)
        {
            return status != CompletedStatus;
        }

        private static void DeleteOldFile(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            string referenceType,
            long referenceId,
            string fileName,
            OpenAiConnectionSetting setting,
            string fileId,
            IAiConnectHttpClient client)
        {
            var response = Retry429(
                context: context,
                ss: ss,
                aiProvider: aiProvider,
                referenceType: referenceType,
                referenceId: referenceId,
                fileName: fileName,
                phase: Phases.Delete,
                fileId: fileId,
                send: () => Delete(
                    setting: setting,
                    fileId: fileId,
                    client: client));
            if (response.Succeeded == false)
            {
                Log(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    referenceType: referenceType,
                    referenceId: referenceId,
                    fileName: fileName,
                    phase: Phases.Delete,
                    response: response,
                    fileId: fileId,
                    status: null,
                    message: "Failed to delete the old file.",
                    sysLogType: SysLogModel.SysLogTypes.Warning);
            }
        }

        private static AiConnectHttpResponse Retry429(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            string referenceType,
            long referenceId,
            string fileName,
            string phase,
            string fileId,
            Func<AiConnectHttpResponse> send)
        {
            return AiConnectHttpUtilities.Retry429(
                send: send,
                onRetry: (response, seconds) => Log(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    referenceType: referenceType,
                    referenceId: referenceId,
                    fileName: fileName,
                    phase: phase,
                    response: response,
                    fileId: fileId,
                    status: null,
                    message: $"Waiting {seconds} seconds before retrying.",
                    sysLogType: SysLogModel.SysLogTypes.Warning),
                onExceeded: (response, seconds) => Log(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    referenceType: referenceType,
                    referenceId: referenceId,
                    fileName: fileName,
                    phase: phase,
                    response: response,
                    fileId: fileId,
                    status: null,
                    message: $"Did not retry because the requested wait time"
                        + $" ({seconds} seconds) is out of the allowed range.",
                    sysLogType: SysLogModel.SysLogTypes.Warning));
        }

        private static string Url(OpenAiConnectionSetting setting, string path)
        {
            return $"{setting.Endpoint.TrimEnd('/')}/{path.TrimStart('/')}";
        }

        private static Dictionary<string, string> Headers(OpenAiConnectionSetting setting)
        {
            return new Dictionary<string, string>
            {
                { "Authorization", $"Bearer {setting.ApiKey}" }
            };
        }

        private static JObject Json(string body)
        {
            if (body.IsNullOrEmpty())
            {
                return null;
            }
            try
            {
                return JToken.Parse(body) as JObject;
            }
            catch (JsonException)
            {
                return null;
            }
        }

        private static IEnumerable<JObject> Data(JObject json)
        {
            return (json.Property("data")?.Value as JArray)
                ?.OfType<JObject>()
                    ?? Enumerable.Empty<JObject>();
        }

        private static string Value(JObject json, string key)
        {
            return json
                ?.Property(key)
                ?.Value
                ?.Value<string>();
        }

        private static SysLogModel.SysLogTypes SysLogType(AiConnectHttpResponse response)
        {
            return response?.Exception != null
                ? SysLogModel.SysLogTypes.Exception
                : SysLogModel.SysLogTypes.SystemError;
        }

        private static AiConnectSendResult Failed(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            string referenceType,
            long referenceId,
            string fileName,
            string phase,
            AiConnectHttpResponse response,
            string fileId,
            string message,
            SysLogModel.SysLogTypes sysLogType,
            Error.Types type = Error.Types.ServerConnectionError)
        {
            Log(
                context: context,
                ss: ss,
                aiProvider: aiProvider,
                referenceType: referenceType,
                referenceId: referenceId,
                fileName: fileName,
                phase: phase,
                response: response,
                fileId: fileId,
                status: null,
                message: message,
                sysLogType: sysLogType);
            return new AiConnectSendResult
            {
                ErrorData = new ErrorData(
                    type: type,
                    id: referenceId),
                Indexing = null
            };
        }

        private static void LogFiles(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            string fileName,
            string phase,
            AiConnectHttpResponse response,
            string fileId,
            string message,
            SysLogModel.SysLogTypes sysLogType)
        {
            Log(
                context: context,
                ss: ss,
                aiProvider: aiProvider,
                referenceType: null,
                referenceId: 0,
                fileName: fileName,
                phase: phase,
                response: response,
                fileId: fileId,
                status: null,
                message: message,
                sysLogType: sysLogType);
        }

        private static void Log(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            string referenceType,
            long referenceId,
            string fileName,
            string phase,
            AiConnectHttpResponse response,
            string fileId,
            string status,
            string message,
            SysLogModel.SysLogTypes sysLogType)
        {
            var items = new List<string>
            {
                $"Phase: {phase}",
                $"TenantId: {context?.TenantId}",
                $"SiteId: {ss?.SiteId}"
            };
            if (referenceType.IsNullOrEmpty() == false)
            {
                items.Add($"ReferenceType: {referenceType}");
                items.Add($"ReferenceId: {referenceId}");
            }
            items.Add($"AiProviderId: {aiProvider?.Id}");
            items.Add($"AiProviderTitle: {aiProvider?.Title}");
            items.Add($"ProviderType: {aiProvider?.ProviderType}");
            if (fileName.IsNullOrEmpty() == false)
            {
                items.Add($"FileName: {fileName}");
            }
            if (response != null && response.StatusCode != 0)
            {
                items.Add($"StatusCode: {response.StatusCode}");
            }
            if (fileId.IsNullOrEmpty() == false)
            {
                items.Add($"FileId: {fileId}");
            }
            if (status.IsNullOrEmpty() == false)
            {
                items.Add($"Status: {status}");
            }
            if (sysLogType != SysLogModel.SysLogTypes.Info
                && response?.Body.IsNullOrEmpty() == false)
            {
                items.Add($"Body: {Truncated(value: response.Body)}");
            }
            items.Add($"Message: {message}");
            if (response?.Exception != null)
            {
                items.Add($"Exception: {response.Exception.GetType().FullName}"
                    + $": {response.Exception.Message}");
            }
            new SysLogModel(
                context: context,
                method: nameof(SendCore),
                message: items.Join(", "),
                errStackTrace: response?.Exception?.StackTrace,
                sysLogType: sysLogType);
        }

        private static string Truncated(string value)
        {
            return value.Length > BodyLogLength
                ? value.Substring(0, BodyLogLength)
                : value;
        }

        private static class Phases
        {
            public const string Setting = "Setting";
            public const string List = "List";
            public const string Upload = "Upload";
            public const string Attach = "Attach";
            public const string Cleanup = "Cleanup";
            public const string Delete = "Delete";
        }
    }

    public class OpenAiConnectionSetting
    {
        public string Endpoint { get; set; }
        public string VectorStoreId { get; set; }
        public string ApiKey { get; set; }
    }

    public class OpenAiFile
    {
        public string Id { get; set; }
        public string FileName { get; set; }
    }
}
