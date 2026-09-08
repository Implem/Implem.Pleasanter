using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace Implem.Pleasanter.Libraries.AiConnect
{
    public static class AiConnectUtilities
    {
        private const string Prefix = "pleasanter";
        private const int ImmediateOperationCapacity = 100;
        private const int ImmediateOperationConcurrency = 4;
        private static readonly SemaphoreSlim ImmediateOperationSemaphore
            = new SemaphoreSlim(ImmediateOperationConcurrency);
        private static int immediateOperationCount;
        private static readonly Dictionary<string, Task> immediateOperationChains
            = new Dictionary<string, Task>();

        public static bool AllowedEndpoint(string endpoint)
        {
            if (Uri.TryCreate(endpoint, UriKind.Absolute, out var uri) == false)
            {
                return false;
            }
            if (uri.Scheme == Uri.UriSchemeHttps)
            {
                return true;
            }
            if (uri.Scheme == Uri.UriSchemeHttp)
            {
                return Parameters.AiConnect?.AllowInsecureLoopbackEndpoint == true
                    && uri.IsLoopback;
            }
            return false;
        }

        public static void Sync(
            Context context,
            SiteSettings ss,
            string referenceType,
            long referenceId,
            Func<string, string> replacedDisplayValues,
            AiConnectSyncOperation operation = AiConnectSyncOperation.Unknown)
        {
            if (Parameters.AiConnect?.Rag.Enabled != true)
            {
                return;
            }
            if (ss?.AiProviders?.Any() != true)
            {
                return;
            }
            if (ss.AiProvidersAllDisabled == true)
            {
                return;
            }
            foreach (var aiProvider in ss.AiProviders)
            {
                try
                {
                    if (aiProvider.Disabled == true)
                    {
                        continue;
                    }
                    if (AiConnectQueueUtilities.ShouldQueue())
                    {
                        AiConnectQueueUtilities.EnqueueSync(
                            context: context,
                            ss: ss,
                            referenceType: referenceType,
                            referenceId: referenceId,
                            aiProviderId: aiProvider.Id,
                            operation: operation);
                        continue;
                    }
                    var key = ImmediateOperationKey(
                        context: context,
                        ss: ss,
                        aiProvider: aiProvider,
                        referenceType: referenceType,
                        referenceId: referenceId);
                    RunImmediate(
                        context: context,
                        key: key,
                        action: () =>
                    {
                        try
                        {
                            if (IsDeletedRecord(
                                context: context,
                                ss: ss,
                                referenceType: referenceType,
                                referenceId: referenceId))
                            {
                                LogDeletedRecordSkipped(
                                    context: context,
                                    ss: ss,
                                    aiProvider: aiProvider,
                                    referenceType: referenceType,
                                    referenceId: referenceId);
                                return;
                            }
                            var errorData = Output(
                                context: context,
                                ss: ss,
                                aiProvider: aiProvider,
                                referenceType: referenceType,
                                referenceId: referenceId,
                                replacedDisplayValues: replacedDisplayValues);
                            if (errorData.Type != Error.Types.None)
                            {
                                return;
                            }
                            var provider = AiConnectProviderFactory.Get(
                                providerType: aiProvider.ProviderType);
                            if (provider == null)
                            {
                                LogInvalidProviderType(
                                    context: context,
                                    ss: ss,
                                    aiProvider: aiProvider,
                                    method: nameof(Sync));
                                return;
                            }
                            var filePath = FilePath(
                                context: context,
                                ss: ss,
                                aiProvider: aiProvider,
                                referenceType: referenceType,
                                referenceId: referenceId);
                            provider.Send(
                                context: context,
                                ss: ss,
                                aiProvider: aiProvider,
                                referenceType: referenceType,
                                referenceId: referenceId,
                                filePath: filePath,
                                forceCreate: ForceCreate(
                                    operation: operation,
                                    retryCount: 0,
                                    resendCount: 0));
                        }
                        catch (Exception e)
                        {
                            new SysLogModel(context, e);
                        }
                    });
                }
                catch (Exception e)
                {
                    new SysLogModel(context, e);
                }
            }
        }

        public static bool ForceCreate(
            AiConnectSyncOperation operation,
            int retryCount,
            int resendCount)
        {
            return operation == AiConnectSyncOperation.Create
                && retryCount == 0
                && resendCount == 0;
        }

        public static void Delete(
            Context context,
            SiteSettings ss,
            string referenceType,
            long referenceId)
        {
            if (Parameters.AiConnect?.Rag.Enabled != true)
            {
                return;
            }
            if (Parameters.AiConnect?.Rag.DeleteEnabled != true)
            {
                return;
            }
            if (ss?.AiProviders?.Any() != true)
            {
                return;
            }
            if (ss.AiProvidersAllDisabled == true)
            {
                return;
            }
            foreach (var aiProvider in ss.AiProviders)
            {
                try
                {
                    if (aiProvider.Disabled == true)
                    {
                        continue;
                    }
                    if (AiConnectQueueUtilities.ShouldQueue())
                    {
                        AiConnectQueueUtilities.EnqueueDelete(
                            context: context,
                            ss: ss,
                            referenceType: referenceType,
                            referenceId: referenceId,
                            aiProviderId: aiProvider.Id);
                        continue;
                    }
                    RunImmediate(
                        context: context,
                        key: ImmediateOperationKey(
                            context: context,
                            ss: ss,
                            aiProvider: aiProvider,
                            referenceType: referenceType,
                            referenceId: referenceId),
                        action: () =>
                    {
                        try
                        {
                            var errorData = DeleteCore(
                                context: context,
                                ss: ss,
                                aiProvider: aiProvider,
                                referenceType: referenceType,
                                referenceId: referenceId,
                                transientFailed: out _);
                            if (errorData.Type != Error.Types.None)
                            {
                                throw new InvalidDataException(
                                    errorData.Message(context: context)?.Text);
                            }
                        }
                        catch (Exception e)
                        {
                            new SysLogModel(context, e);
                        }
                    });
                }
                catch (Exception e)
                {
                    new SysLogModel(context, e);
                }
            }
        }

        public static ErrorData DeleteCore(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            string referenceType,
            long referenceId,
            out bool transientFailed)
        {
            transientFailed = false;
            if (Parameters.AiConnect?.Rag.OutputFilePath.IsNullOrEmpty() != false)
            {
                return Failed(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    referenceType: referenceType,
                    referenceId: referenceId,
                    filePath: null,
                    phase: Phases.OutputFilePath,
                    message: "OutputFilePath is not set.",
                    sysLogType: SysLogModel.SysLogTypes.UserError,
                    method: nameof(DeleteCore));
            }
            var provider = AiConnectProviderFactory.Get(
                providerType: aiProvider.ProviderType);
            if (provider == null)
            {
                LogInvalidProviderType(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    method: nameof(Delete));
                throw new InvalidDataException(Displays.Get(
                    context: context,
                    id: "InvalidAiProviderSetting",
                    data: aiProvider.ProviderType));
            }
            var fileName = FileName(
                context: context,
                ss: ss,
                aiProvider: aiProvider,
                referenceType: referenceType,
                referenceId: referenceId);
            var errorData = provider.DeleteDocument(
                context: context,
                ss: ss,
                aiProvider: aiProvider,
                fileName: fileName);
            if (errorData.Type != Error.Types.None)
            {
                return errorData;
            }
            var filePath = FilePath(
                context: context,
                ss: ss,
                aiProvider: aiProvider,
                referenceType: referenceType,
                referenceId: referenceId);
            var deleteFileResult = DeleteFile(
                context: context,
                filePath: filePath);
            if (deleteFileResult == AiConnectDeleteFileResults.TransientFailed)
            {
                transientFailed = true;
                return new ErrorData(
                    type: Error.Types.FailedWriteFile,
                    id: referenceId);
            }
            if (deleteFileResult == AiConnectDeleteFileResults.PermanentFailed)
            {
                return new ErrorData(
                    type: Error.Types.FailedWriteFile,
                    id: referenceId);
            }
            new SysLogModel(
                context: context,
                method: nameof(DeleteCore),
                message: LogMessage(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    referenceType: referenceType,
                    referenceId: referenceId,
                    filePath: filePath,
                    phase: Phases.Delete,
                    message: "The document has been deleted.",
                    e: null),
                sysLogType: SysLogModel.SysLogTypes.Info);
            return new ErrorData(type: Error.Types.None);
        }

        private static string ImmediateOperationKey(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            string referenceType,
            long referenceId)
        {
            return $"{context.TenantId}-{ss?.SiteId}-{aiProvider?.Id}"
                + $"-{referenceType}-{referenceId}";
        }

        private static bool IsDeletedRecord(
            Context context,
            SiteSettings ss,
            string referenceType,
            long referenceId)
        {
            switch (referenceType)
            {
                case "Issues":
                    return Rds.ExecuteScalar_int(
                        context: context,
                        statements: Rds.SelectIssues(
                            column: Rds.IssuesColumn().IssuesCount(),
                            where: Rds.IssuesWhere()
                                .SiteId(ss.SiteId)
                                .IssueId(referenceId))) == 0;
                case "Results":
                    return Rds.ExecuteScalar_int(
                        context: context,
                        statements: Rds.SelectResults(
                            column: Rds.ResultsColumn().ResultsCount(),
                            where: Rds.ResultsWhere()
                                .SiteId(ss.SiteId)
                                .ResultId(referenceId))) == 0;
                default:
                    return false;
            }
        }

        private static void LogDeletedRecordSkipped(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            string referenceType,
            long referenceId)
        {
            new SysLogModel(
                context: context,
                method: nameof(Sync),
                message: LogMessage(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    referenceType: referenceType,
                    referenceId: referenceId,
                    filePath: null,
                    phase: "Record",
                    message: "The record has been deleted; synchronization was skipped.",
                    e: null),
                sysLogType: SysLogModel.SysLogTypes.Info);
        }

        private static void RunImmediate(Context context, string key, Action action)
        {
            if (Interlocked.Increment(ref immediateOperationCount) > ImmediateOperationCapacity)
            {
                Interlocked.Decrement(ref immediateOperationCount);
                new SysLogModel(
                    context: context,
                    method: nameof(RunImmediate),
                    message: "Executed the immediate AI Connect operation synchronously "
                        + $"because the capacity ({ImmediateOperationCapacity}) was reached.",
                    sysLogType: SysLogModel.SysLogTypes.Warning);
                RunSynchronously(key: key, action: action);
                return;
            }
            ChainImmediateOperation(
                key: key,
                operation: async () =>
            {
                try
                {
                    await ImmediateOperationSemaphore.WaitAsync();
                    try
                    {
                        action();
                    }
                    finally
                    {
                        ImmediateOperationSemaphore.Release();
                    }
                }
                finally
                {
                    Interlocked.Decrement(ref immediateOperationCount);
                }
            });
        }

        private static void RunSynchronously(string key, Action action)
        {
            ChainImmediateOperation(
                key: key,
                operation: () =>
            {
                ImmediateOperationSemaphore.Wait();
                try
                {
                    action();
                }
                finally
                {
                    ImmediateOperationSemaphore.Release();
                }
                return Task.CompletedTask;
            }).Wait();
        }

        private static Task ChainImmediateOperation(string key, Func<Task> operation)
        {
            Task current;
            lock (immediateOperationChains)
            {
                var previous = immediateOperationChains.TryGetValue(key, out var chain)
                    ? chain
                    : Task.CompletedTask;
                current = previous.ContinueWith(
                    continuationFunction: _ => operation(),
                    scheduler: TaskScheduler.Default).Unwrap();
                immediateOperationChains[key] = current;
            }
            current.ContinueWith(
                continuationAction: completed => RemoveImmediateOperationChain(
                    key: key,
                    completed: completed),
                scheduler: TaskScheduler.Default);
            return current;
        }

        private static void RemoveImmediateOperationChain(string key, Task completed)
        {
            lock (immediateOperationChains)
            {
                if (immediateOperationChains.TryGetValue(key, out var chain)
                    && chain == completed)
                {
                    immediateOperationChains.Remove(key);
                }
            }
        }

        public static AiConnectDeleteFileResults DeleteFile(
            Context context,
            string filePath)
        {
            try
            {
                if (ValidFilePath(filePath: filePath) == false)
                {
                    LogDeleteFileFailed(
                        context: context,
                        filePath: filePath,
                        message: "The file path is not under the output root.");
                    return AiConnectDeleteFileResults.PermanentFailed;
                }
                System.IO.File.Delete(filePath);
                return AiConnectDeleteFileResults.Succeeded;
            }
            catch (FileNotFoundException)
            {
                return AiConnectDeleteFileResults.Succeeded;
            }
            catch (DirectoryNotFoundException)
            {
                return AiConnectDeleteFileResults.Succeeded;
            }
            catch (PathTooLongException e)
            {
                LogDeleteFileFailed(
                    context: context,
                    filePath: filePath,
                    message: "Failed to delete the file."
                        + $" Exception: {e.GetType().FullName}");
                return AiConnectDeleteFileResults.PermanentFailed;
            }
            catch (DriveNotFoundException e)
            {
                LogDeleteFileFailed(
                    context: context,
                    filePath: filePath,
                    message: "Failed to delete the file."
                        + $" Exception: {e.GetType().FullName}");
                return AiConnectDeleteFileResults.PermanentFailed;
            }
            catch (UnauthorizedAccessException e)
            {
                LogDeleteFileFailed(
                    context: context,
                    filePath: filePath,
                    message: "Failed to delete the file."
                        + $" Exception: {e.GetType().FullName}");
                return AiConnectDeleteFileResults.PermanentFailed;
            }
            catch (ArgumentException e)
            {
                LogDeleteFileFailed(
                    context: context,
                    filePath: filePath,
                    message: "Failed to delete the file."
                        + $" Exception: {e.GetType().FullName}");
                return AiConnectDeleteFileResults.PermanentFailed;
            }
            catch (NotSupportedException e)
            {
                LogDeleteFileFailed(
                    context: context,
                    filePath: filePath,
                    message: "Failed to delete the file."
                        + $" Exception: {e.GetType().FullName}");
                return AiConnectDeleteFileResults.PermanentFailed;
            }
            catch (InvalidOperationException e)
            {
                LogDeleteFileFailed(
                    context: context,
                    filePath: filePath,
                    message: "Failed to delete the file."
                        + $" Exception: {e.GetType().FullName}");
                return AiConnectDeleteFileResults.PermanentFailed;
            }
            catch (IOException e)
            {
                LogDeleteFileFailed(
                    context: context,
                    filePath: filePath,
                    message: "Failed to delete the file."
                        + $" Exception: {e.GetType().FullName}");
                return AiConnectDeleteFileResults.TransientFailed;
            }
        }

        public static ErrorData Output(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            string referenceType,
            long referenceId,
            Func<string, string> replacedDisplayValues)
        {
            if (Parameters.AiConnect?.Rag.Enabled != true)
            {
                return new ErrorData(type: Error.Types.None);
            }
            WarnIfChoiceHashNotSet(
                context: context,
                ss: ss,
                aiProvider: aiProvider,
                referenceType: referenceType,
                referenceId: referenceId);
            var filePath = (string)null;
            try
            {
                if (Parameters.AiConnect.Rag.OutputFilePath.IsNullOrEmpty())
                {
                    return Failed(
                        context: context,
                        ss: ss,
                        aiProvider: aiProvider,
                        referenceType: referenceType,
                        referenceId: referenceId,
                        filePath: filePath,
                        phase: Phases.OutputFilePath,
                        message: "OutputFilePath is not set.",
                        sysLogType: SysLogModel.SysLogTypes.UserError);
                }
                if (aiProvider.Format.IsNullOrEmpty())
                {
                    return Failed(
                        context: context,
                        ss: ss,
                        aiProvider: aiProvider,
                        referenceType: referenceType,
                        referenceId: referenceId,
                        filePath: filePath,
                        phase: Phases.Body,
                        message: "Format is not set.",
                        sysLogType: SysLogModel.SysLogTypes.UserError);
                }
                var body = Body(
                    format: aiProvider.Format,
                    replacedDisplayValues: replacedDisplayValues);
                filePath = FilePath(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    referenceType: referenceType,
                    referenceId: referenceId);
                if (ValidFilePath(filePath: filePath) == false)
                {
                    return Failed(
                        context: context,
                        ss: ss,
                        aiProvider: aiProvider,
                        referenceType: referenceType,
                        referenceId: referenceId,
                        filePath: filePath,
                        phase: Phases.FilePath,
                        message: "The file path is not under the output root.",
                        sysLogType: SysLogModel.SysLogTypes.UserError);
                }
                if (Write(
                    body: body,
                    filePath: filePath) == false)
                {
                    return Failed(
                        context: context,
                        ss: ss,
                        aiProvider: aiProvider,
                        referenceType: referenceType,
                        referenceId: referenceId,
                        filePath: filePath,
                        phase: Phases.Write,
                        message: "Failed to write the file.",
                        sysLogType: SysLogModel.SysLogTypes.SystemError);
                }
                return new ErrorData(type: Error.Types.None);
            }
            catch (Exception e)
            {
                return Failed(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    referenceType: referenceType,
                    referenceId: referenceId,
                    filePath: filePath,
                    phase: Phases.Exception,
                    message: "An exception occurred.",
                    sysLogType: SysLogModel.SysLogTypes.Exception,
                    e: e);
            }
        }

        public static string FilePath(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            string referenceType,
            long referenceId)
        {
            return Path.Combine(
                RootPath(),
                context.TenantId.ToString(),
                ss.SiteId.ToString(),
                FileName(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    referenceType: referenceType,
                    referenceId: referenceId));
        }

        public static string FileNamePrefix(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider)
        {
            return $"{Prefix}-{context.TenantId}-{ss.SiteId}-{aiProvider.Id}-";
        }

        public static ErrorData ValidateConnectionSetting(string connectionSetting)
        {
            if (connectionSetting.IsNullOrEmpty())
            {
                return new ErrorData(type: Error.Types.ValidationError);
            }
            JObject json = null;
            try
            {
                json = JToken.Parse(connectionSetting) as JObject;
            }
            catch (JsonReaderException)
            {
                return new ErrorData(type: Error.Types.ValidationError);
            }
            if (json == null)
            {
                return new ErrorData(type: Error.Types.ValidationError);
            }
            return new ErrorData(type: Error.Types.None);
        }

        private static void LogDeleteFileFailed(
            Context context,
            string filePath,
            string message)
        {
            new SysLogModel(
                context: context,
                method: nameof(DeleteFile),
                message: new List<string>
                {
                    $"Phase: {Phases.DeleteFile}",
                    $"TenantId: {context?.TenantId}",
                    $"FilePath: {filePath}",
                    $"Message: {message}"
                }.Join(", "),
                sysLogType: SysLogModel.SysLogTypes.Warning);
        }

        private static void LogInvalidProviderType(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            string method)
        {
            new SysLogModel(
                context: context,
                method: method,
                message: new List<string>
                {
                    $"TenantId: {context?.TenantId}",
                    $"SiteId: {ss?.SiteId}",
                    $"AiProviderId: {aiProvider?.Id}",
                    $"AiProviderTitle: {aiProvider?.Title}",
                    $"ProviderType: {aiProvider?.ProviderType}",
                    "Message: The provider type is invalid."
                }.Join(", "),
                sysLogType: SysLogModel.SysLogTypes.UserError);
        }

        private static string Body(
            string format,
            Func<string, string> replacedDisplayValues)
        {
            var body = replacedDisplayValues(format) ?? string.Empty;
            body = body
                .Replace("\r\n", "\n")
                .Replace("\r", "\n");
            return body.TrimEnd('\n') + "\n";
        }

        public static string FileName(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            string referenceType,
            long referenceId)
        {
            return FileNamePrefix(
                context: context,
                ss: ss,
                aiProvider: aiProvider)
                    + $"{ReferenceTypeName(referenceType: referenceType)}-{referenceId}.md";
        }

        private static string ReferenceTypeName(string referenceType)
        {
            switch (referenceType)
            {
                case "Issues":
                    return "issue";
                case "Results":
                    return "result";
                default:
                    return referenceType?.ToLower();
            }
        }

        private static string RootPath()
        {
            var path = Parameters.AiConnect?.Rag.OutputFilePath;
            if (path.IsNullOrEmpty())
            {
                throw new InvalidOperationException(
                    "Parameters.AiConnect.Rag.OutputFilePath is not configured.");
            }
            return Path.GetFullPath(Path.IsPathRooted(path)
                ? path
                : Path.Combine(Environments.CurrentDirectoryPath, path));
        }

        private static bool ValidFilePath(string filePath)
        {
            var root = RootPath();
            if (root.EndsWith(Path.DirectorySeparatorChar) == false
                && root.EndsWith(Path.AltDirectorySeparatorChar) == false)
            {
                root += Path.DirectorySeparatorChar;
            }
            return Path.GetFullPath(filePath).StartsWith(
                root,
                OperatingSystem.IsWindows()
                    ? StringComparison.OrdinalIgnoreCase
                    : StringComparison.Ordinal);
        }

        private static bool Write(string body, string filePath)
        {
            return body
                .ToBytes(new UTF8Encoding(encoderShouldEmitUTF8Identifier: false))
                .TryWrite(filePath: filePath);
        }

        private static ErrorData Failed(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            string referenceType,
            long referenceId,
            string filePath,
            string phase,
            string message,
            SysLogModel.SysLogTypes sysLogType,
            Exception e = null,
            string method = null)
        {
            new SysLogModel(
                context: context,
                method: method ?? nameof(Output),
                message: LogMessage(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    referenceType: referenceType,
                    referenceId: referenceId,
                    filePath: filePath,
                    phase: phase,
                    message: message,
                    e: e),
                errStackTrace: e?.StackTrace,
                sysLogType: sysLogType);
            return new ErrorData(
                type: Error.Types.FailedWriteFile,
                id: referenceId);
        }

        private static string LogMessage(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            string referenceType,
            long referenceId,
            string filePath,
            string phase,
            string message,
            Exception e)
        {
            var items = new List<string>
            {
                $"Phase: {phase}",
                $"TenantId: {context?.TenantId}",
                $"SiteId: {ss?.SiteId}",
                $"ReferenceType: {referenceType}",
                $"ReferenceId: {referenceId}",
                $"AiProviderId: {aiProvider?.Id}",
                $"AiProviderTitle: {aiProvider?.Title}"
            };
            if (filePath.IsNullOrEmpty() == false)
            {
                items.Add($"FilePath: {filePath}");
            }
            items.Add($"Message: {message}");
            if (e != null)
            {
                items.Add($"Exception: {e.GetType().FullName}: {e.Message}");
            }
            return items.Join(", ");
        }

        private static void WarnIfChoiceHashNotSet(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            string referenceType,
            long referenceId)
        {
            if (ss?.Columns?.Any(column =>
                column.HasChoices() && column.ChoiceHash == null) == true)
            {
                new SysLogModel(
                    context: context,
                    method: nameof(Output),
                    message: LogMessage(
                        context: context,
                        ss: ss,
                        aiProvider: aiProvider,
                        referenceType: referenceType,
                        referenceId: referenceId,
                        filePath: null,
                        phase: Phases.ChoiceHash,
                        message: "ss.SetChoiceHash() may not have been called.",
                        e: null),
                    sysLogType: SysLogModel.SysLogTypes.Warning);
            }
        }

        private static class Phases
        {
            public const string OutputFilePath = "OutputFilePath";
            public const string Body = "Body";
            public const string FilePath = "FilePath";
            public const string Write = "Write";
            public const string Exception = "Exception";
            public const string ChoiceHash = "ChoiceHash";
            public const string Delete = "Delete";
            public const string DeleteFile = "DeleteFile";
        }
    }
}
