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
    public static class AiConnectDifyUtilities
    {
        private const int ListLimit = 100;
        private const int BodyLogLength = 1000;

        public static ErrorData SendCore(
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
                    documentId: null,
                    message: "The connection setting is invalid.",
                    sysLogType: SysLogModel.SysLogTypes.UserError);
            }
            var documentId = (string)null;
            if (forceCreate == false)
            {
                documentId = DocumentId(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    referenceType: referenceType,
                    referenceId: referenceId,
                    setting: setting,
                    fileName: fileName,
                    client: httpClient,
                    failed: out var listFailed);
                if (listFailed != null)
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
                        documentId: null,
                        message: "Failed to get the document list.",
                        sysLogType: SysLogType(response: listFailed));
                }
            }
            var create = documentId.IsNullOrEmpty();
            var phase = create
                ? Phases.Create
                : Phases.Update;
            Func<AiConnectHttpResponse> send;
            if (create)
            {
                send = () => Create(
                    setting: setting,
                    fileName: fileName,
                    filePath: filePath,
                    client: httpClient);
            }
            else
            {
                send = () => Update(
                    setting: setting,
                    documentId: documentId,
                    fileName: fileName,
                    filePath: filePath,
                    client: httpClient);
            }
            var response = forceCreate
                ? Retry429(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    referenceType: referenceType,
                    referenceId: referenceId,
                    fileName: fileName,
                    phase: phase,
                    documentId: documentId,
                    send: send)
                : send();
            if (response.Succeeded == false)
            {
                return Failed(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    referenceType: referenceType,
                    referenceId: referenceId,
                    fileName: fileName,
                    phase: phase,
                    response: response,
                    documentId: documentId,
                    message: "Failed to send the document.",
                    sysLogType: SysLogType(response: response));
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
                    phase: phase,
                    response: response,
                    documentId: documentId,
                    batch: Batch(response: response),
                    message: "The document has been sent.",
                    sysLogType: SysLogModel.SysLogTypes.Info);
            }
            return new ErrorData(type: Error.Types.None);
        }

        public static List<string> DocumentIds(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            IAiConnectHttpClient client = null,
            string fileNamePrefix = null)
        {
            var httpClient = client ?? AiConnectHttpClient.Default;
            var setting = Setting(connectionSetting: aiProvider?.ConnectionSetting);
            if (setting == null)
            {
                LogDataset(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    phase: Phases.Setting,
                    response: null,
                    documentId: null,
                    message: "The connection setting is invalid.",
                    sysLogType: SysLogModel.SysLogTypes.UserError);
                return null;
            }
            var documentIds = new List<string>();
            var page = 1;
            while (true)
            {
                var currentPage = page;
                var response = Retry429(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    referenceType: null,
                    referenceId: 0,
                    fileName: null,
                    phase: Phases.List,
                    documentId: null,
                    send: () => httpClient.Get(
                        url: Url(
                            setting: setting,
                            path: $"datasets/{setting.DatasetId}/documents"
                                + $"?limit={ListLimit}&page={currentPage}"),
                        headers: Headers(setting: setting)));
                var json = response.Succeeded
                    ? Json(body: response.Body)
                    : null;
                if (json == null)
                {
                    LogDataset(
                        context: context,
                        ss: ss,
                        aiProvider: aiProvider,
                        phase: Phases.List,
                        response: response,
                        documentId: null,
                        message: "Failed to get the document list.",
                        sysLogType: SysLogType(response: response));
                    return null;
                }
                var documents = Documents(json: json);
                if (fileNamePrefix != null)
                {
                    documents = documents.Where(o =>
                        Value(json: o, key: "name")?.StartsWith(
                            fileNamePrefix,
                            StringComparison.Ordinal) == true);
                }
                documentIds.AddRange(documents
                    .Select(o => Value(json: o, key: "id"))
                    .Where(o => o.IsNullOrEmpty() == false));
                if (json.Property("has_more")?.Value?.Value<bool>() != true)
                {
                    break;
                }
                page++;
            }
            return documentIds;
        }

        public static AiConnectHttpResponse Delete(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            string documentId,
            IAiConnectHttpClient client = null)
        {
            var httpClient = client ?? AiConnectHttpClient.Default;
            var setting = Setting(connectionSetting: aiProvider?.ConnectionSetting);
            if (setting == null)
            {
                LogDataset(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    phase: Phases.Setting,
                    response: null,
                    documentId: documentId,
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
                documentId: documentId,
                send: () => httpClient.Delete(
                    url: Url(
                        setting: setting,
                        path: $"datasets/{setting.DatasetId}/documents/{documentId}"),
                    headers: Headers(setting: setting)));
            if (response.Succeeded == false)
            {
                LogDataset(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    phase: Phases.Delete,
                    response: response,
                    documentId: documentId,
                    message: "Failed to delete the document.",
                    sysLogType: SysLogType(response: response));
            }
            return response;
        }

        public static List<string> DocumentIdsByName(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            string fileName,
            IAiConnectHttpClient client = null)
        {
            var httpClient = client ?? AiConnectHttpClient.Default;
            var setting = Setting(connectionSetting: aiProvider?.ConnectionSetting);
            if (setting == null)
            {
                LogDataset(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    phase: Phases.Setting,
                    response: null,
                    documentId: null,
                    message: "The connection setting is invalid.",
                    sysLogType: SysLogModel.SysLogTypes.UserError);
                return null;
            }
            var documentIds = DocumentIdsByName(
                context: context,
                ss: ss,
                aiProvider: aiProvider,
                referenceType: null,
                referenceId: 0,
                setting: setting,
                fileName: fileName,
                client: httpClient,
                failed: out var listFailed);
            if (listFailed != null)
            {
                LogDataset(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    phase: Phases.List,
                    response: listFailed,
                    documentId: null,
                    message: "Failed to get the document list.",
                    sysLogType: SysLogType(response: listFailed));
                return null;
            }
            return documentIds;
        }

        public static ErrorData ValidateConnectionSetting(
            Context context,
            AiProvider aiProvider)
        {
            return AiConnectUtilities.ValidateConnectionSetting(
                connectionSetting: aiProvider?.ConnectionSetting);
        }

        private static DifyConnectionSetting Setting(string connectionSetting)
        {
            if (connectionSetting.IsNullOrEmpty())
            {
                return null;
            }
            var setting = connectionSetting.Deserialize<DifyConnectionSetting>();
            if (setting == null
                || setting.DatasetId.IsNullOrEmpty()
                || setting.ApiKey.IsNullOrEmpty())
            {
                return null;
            }
            if (setting.Endpoint.IsNullOrEmpty())
            {
                setting.Endpoint = AiProviderUtilities.ConnectionSettingEndpoint(
                    providerType: AiProvider.Types.Dify);
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

        private static string DocumentId(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            string referenceType,
            long referenceId,
            DifyConnectionSetting setting,
            string fileName,
            IAiConnectHttpClient client,
            out AiConnectHttpResponse failed)
        {
            return DocumentIdsByName(
                context: context,
                ss: ss,
                aiProvider: aiProvider,
                referenceType: referenceType,
                referenceId: referenceId,
                setting: setting,
                fileName: fileName,
                client: client,
                failed: out failed)
                    ?.FirstOrDefault();
        }

        private static List<string> DocumentIdsByName(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            string referenceType,
            long referenceId,
            DifyConnectionSetting setting,
            string fileName,
            IAiConnectHttpClient client,
            out AiConnectHttpResponse failed)
        {
            failed = null;
            var matched = new List<JObject>();
            var page = 1;
            while (true)
            {
                var currentPage = page;
                var response = Retry429(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    referenceType: referenceType,
                    referenceId: referenceId,
                    fileName: fileName,
                    phase: Phases.List,
                    documentId: null,
                    send: () => client.Get(
                        url: Url(
                            setting: setting,
                            path: $"datasets/{setting.DatasetId}/documents"
                                + $"?keyword={Uri.EscapeDataString(fileName)}"
                                + $"&limit={ListLimit}&page={currentPage}"),
                        headers: Headers(setting: setting)));
                var json = response.Succeeded
                    ? Json(body: response.Body)
                    : null;
                if (json == null)
                {
                    failed = response;
                    return null;
                }
                matched.AddRange(Documents(json: json)
                    .Where(o => string.Equals(
                        Value(json: o, key: "name"),
                        fileName,
                        StringComparison.Ordinal)));
                if (json.Property("has_more")?.Value?.Value<bool>() != true)
                {
                    break;
                }
                page++;
            }
            if (matched.Count > 1)
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
                    documentId: null,
                    batch: null,
                    message: $"{matched.Count} documents have the same name.",
                    sysLogType: SysLogModel.SysLogTypes.Warning);
            }
            return matched
                .Select(o => Value(json: o, key: "id"))
                .Where(o => o.IsNullOrEmpty() == false)
                .ToList();
        }

        private static AiConnectHttpResponse Update(
            DifyConnectionSetting setting,
            string documentId,
            string fileName,
            string filePath,
            IAiConnectHttpClient client)
        {
            return client.PostFile(
                url: Url(
                    setting: setting,
                    path: $"datasets/{setting.DatasetId}/documents/{documentId}"
                        + "/update-by-file"),
                headers: Headers(setting: setting),
                data: new JObject
                {
                    ["name"] = fileName,
                    ["process_rule"] = new JObject
                    {
                        ["mode"] = "automatic"
                    }
                }.ToString(Formatting.None),
                filePath: filePath);
        }

        private static AiConnectHttpResponse Create(
            DifyConnectionSetting setting,
            string fileName,
            string filePath,
            IAiConnectHttpClient client)
        {
            return client.PostFile(
                url: Url(
                    setting: setting,
                    path: $"datasets/{setting.DatasetId}/document/create-by-file"),
                headers: Headers(setting: setting),
                data: new JObject
                {
                    ["indexing_technique"] = "high_quality",
                    ["process_rule"] = new JObject
                    {
                        ["mode"] = "automatic"
                    }
                }.ToString(Formatting.None),
                filePath: filePath);
        }

        private static AiConnectHttpResponse Retry429(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            string referenceType,
            long referenceId,
            string fileName,
            string phase,
            string documentId,
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
                    documentId: documentId,
                    batch: null,
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
                    documentId: documentId,
                    batch: null,
                    message: $"Did not retry because the requested wait time"
                        + $" ({seconds} seconds) is out of the allowed range.",
                    sysLogType: SysLogModel.SysLogTypes.Warning));
        }

        private static string Url(DifyConnectionSetting setting, string path)
        {
            return $"{setting.Endpoint.TrimEnd('/')}/{path.TrimStart('/')}";
        }

        private static Dictionary<string, string> Headers(DifyConnectionSetting setting)
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

        private static IEnumerable<JObject> Documents(JObject json)
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

        private static string Batch(AiConnectHttpResponse response)
        {
            return Value(
                json: Json(body: response?.Body),
                key: "batch");
        }

        private static SysLogModel.SysLogTypes SysLogType(AiConnectHttpResponse response)
        {
            return response?.Exception != null
                ? SysLogModel.SysLogTypes.Exception
                : SysLogModel.SysLogTypes.SystemError;
        }

        private static ErrorData Failed(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            string referenceType,
            long referenceId,
            string fileName,
            string phase,
            AiConnectHttpResponse response,
            string documentId,
            string message,
            SysLogModel.SysLogTypes sysLogType)
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
                documentId: documentId,
                batch: null,
                message: message,
                sysLogType: sysLogType);
            return new ErrorData(
                type: Error.Types.ServerConnectionError,
                id: referenceId);
        }

        private static void LogDataset(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            string phase,
            AiConnectHttpResponse response,
            string documentId,
            string message,
            SysLogModel.SysLogTypes sysLogType)
        {
            Log(
                context: context,
                ss: ss,
                aiProvider: aiProvider,
                referenceType: null,
                referenceId: 0,
                fileName: null,
                phase: phase,
                response: response,
                documentId: documentId,
                batch: null,
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
            string documentId,
            string batch,
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
            if (documentId.IsNullOrEmpty() == false)
            {
                items.Add($"DocumentId: {documentId}");
            }
            if (batch.IsNullOrEmpty() == false)
            {
                items.Add($"Batch: {batch}");
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
            public const string Update = "Update";
            public const string Create = "Create";
            public const string Delete = "Delete";
        }
    }
}
