using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.AiConnect;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    public class AiConnectIndexCheckJobHandler : IBackgroundJobHandler
    {
        public Task ExecuteAsync(
            Context context,
            BackgroundJobModel backgroundJobModel)
        {
            var jobParameters = backgroundJobModel.JobParameters
                .Deserialize<AiConnectIndexCheckJobParameters>();
            if (jobParameters == null)
            {
                throw new InvalidDataException(Displays.Get(
                    context: context,
                    id: "BackgroundJobInvalidParameters"));
            }
            var jobContext = AiConnectJobContext.CreateContext(
                backgroundJobModel: backgroundJobModel,
                language: jobParameters.Language);
            if (Parameters.AiConnect?.Rag.Enabled != true)
            {
                backgroundJobModel.ResultMessage = Displays.Get(
                    context: jobContext,
                    id: "AiConnectJobSkipped");
                return Task.CompletedTask;
            }
            var ss = AiConnectJobContext.GetSiteSettings(
                context: jobContext,
                siteId: backgroundJobModel.SiteId);
            var aiProvider = AiConnectJobContext.GetAiProvider(
                ss: ss,
                aiProviderId: jobParameters.AiProviderId);
            if (aiProvider == null
                || aiProvider.ProviderType != AiProvider.Types.OpenAi)
            {
                backgroundJobModel.ResultMessage = Displays.Get(
                    context: jobContext,
                    id: "AiConnectJobSkipped");
                return Task.CompletedTask;
            }
            var maxAttempts = Parameters.AiConnect?.Rag.IndexCheckMaxAttempts ?? 10;
            if (jobParameters.AttemptCount >= maxAttempts)
            {
                new SysLogModel(
                    context: jobContext,
                    method: nameof(ExecuteAsync),
                    message: $"AttemptCount: {jobParameters.AttemptCount}"
                        + $", FileId: {jobParameters.FileId}"
                        + KeptOldFileIds(jobParameters: jobParameters)
                        + ", Message: The indexing completion could not be confirmed.",
                    sysLogType: SysLogModel.SysLogTypes.SystemError);
                throw new InvalidDataException(Displays.Get(
                    context: jobContext,
                    id: "AiConnectIndexCheckIncomplete"));
            }
            var response = AiConnectOpenAiUtilities.VectorStoreFile(
                aiProvider: aiProvider,
                fileId: jobParameters.FileId);
            if (response == null
                || response.Exception != null
                || response.Succeeded == false)
            {
                var nextJobId = AiConnectQueueUtilities.EnqueueIndexCheckNext(
                    context: jobContext,
                    ss: ss,
                    original: jobParameters);
                new SysLogModel(
                    context: jobContext,
                    method: nameof(ExecuteAsync),
                    message: $"AttemptCount: {jobParameters.AttemptCount}"
                        + $", FileId: {jobParameters.FileId}"
                        + $", NextBackgroundJobId: {nextJobId}"
                        + ", Message: Failed to check the indexing status.",
                    sysLogType: SysLogModel.SysLogTypes.Warning);
                backgroundJobModel.ResultMessage = Displays.Get(
                    context: jobContext,
                    id: "AiConnectJobSkipped");
                return Task.CompletedTask;
            }
            var status = Status(body: response.Body);
            new SysLogModel(
                context: jobContext,
                method: nameof(ExecuteAsync),
                message: $"Status: {status}"
                    + $", AttemptCount: {jobParameters.AttemptCount}"
                    + $", FileId: {jobParameters.FileId}",
                sysLogType: SysLogModel.SysLogTypes.Info);
            switch (status)
            {
                case "completed":
                    DeleteOldFiles(
                        context: jobContext,
                        ss: ss,
                        aiProvider: aiProvider,
                        jobParameters: jobParameters);
                    backgroundJobModel.ResultMessage = Displays.Get(
                        context: jobContext,
                        id: "AiConnectJobCompleted");
                    return Task.CompletedTask;
                case "in_progress":
                    AiConnectQueueUtilities.EnqueueIndexCheckNext(
                        context: jobContext,
                        ss: ss,
                        original: jobParameters);
                    return Task.CompletedTask;
                case "failed":
                case "cancelled":
                    HandleFailedOrCancelled(
                        context: jobContext,
                        ss: ss,
                        jobParameters: jobParameters,
                        status: status);
                    return Task.CompletedTask;
                default:
                    var nextJobId = AiConnectQueueUtilities.EnqueueIndexCheckNext(
                        context: jobContext,
                        ss: ss,
                        original: jobParameters);
                    new SysLogModel(
                        context: jobContext,
                        method: nameof(ExecuteAsync),
                        message: $"Status: {status}"
                            + $", AttemptCount: {jobParameters.AttemptCount}"
                            + $", FileId: {jobParameters.FileId}"
                            + $", NextBackgroundJobId: {nextJobId}"
                            + ", Message: Unknown indexing status.",
                        sysLogType: SysLogModel.SysLogTypes.Warning);
                    backgroundJobModel.ResultMessage = Displays.Get(
                        context: jobContext,
                        id: "AiConnectJobSkipped");
                    return Task.CompletedTask;
            }
        }

        private static void DeleteOldFiles(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            AiConnectIndexCheckJobParameters jobParameters)
        {
            if (jobParameters.OldFileIds?.Any() != true)
            {
                return;
            }
            new SysLogModel(
                context: context,
                method: nameof(DeleteOldFiles),
                message: $"FileId: {jobParameters.FileId}"
                    + $", OldFileIds: {jobParameters.OldFileIds.Join()}"
                    + ", Message: Deletes the old files"
                        + " because the indexing has been completed.",
                sysLogType: SysLogModel.SysLogTypes.Info);
            jobParameters.OldFileIds.ForEach(fileId => AiConnectOpenAiUtilities.DeleteFile(
                context: context,
                ss: ss,
                aiProvider: aiProvider,
                fileId: fileId));
        }

        private static string KeptOldFileIds(AiConnectIndexCheckJobParameters jobParameters)
        {
            return jobParameters.OldFileIds?.Any() == true
                ? $", KeptOldFileIds: {jobParameters.OldFileIds.Join()}"
                : string.Empty;
        }

        private static void HandleFailedOrCancelled(
            Context context,
            SiteSettings ss,
            AiConnectIndexCheckJobParameters jobParameters,
            string status)
        {
            if (jobParameters.ResendCount >= 1)
            {
                new SysLogModel(
                    context: context,
                    method: nameof(HandleFailedOrCancelled),
                    message: $"Status: {status}"
                        + $", FileId: {jobParameters.FileId}"
                        + $", ResendCount: {jobParameters.ResendCount}"
                        + KeptOldFileIds(jobParameters: jobParameters)
                        + ", Message: The resend has already been used. No further resend.",
                    sysLogType: SysLogModel.SysLogTypes.Warning);
                throw new InvalidDataException(Displays.Get(
                    context: context,
                    id: "AiConnectIndexResendExceeded"));
            }
            var resendJobId = AiConnectQueueUtilities.EnqueueSyncResend(
                context: context,
                ss: ss,
                referenceType: jobParameters.ReferenceType,
                referenceId: jobParameters.ReferenceId,
                aiProviderId: jobParameters.AiProviderId);
            new SysLogModel(
                context: context,
                method: nameof(HandleFailedOrCancelled),
                message: $"Status: {status}"
                    + $", FileId: {jobParameters.FileId}"
                    + $", ResendBackgroundJobId: {resendJobId}"
                    + KeptOldFileIds(jobParameters: jobParameters),
                sysLogType: SysLogModel.SysLogTypes.Warning);
            throw new InvalidDataException(Displays.Get(
                context: context,
                id: "AiConnectIndexCheckFailed",
                data: resendJobId.ToString()));
        }

        private static string Status(string body)
        {
            if (body.IsNullOrEmpty())
            {
                return null;
            }
            try
            {
                return (JToken.Parse(body) as JObject)
                    ?.Property("status")
                    ?.Value
                    ?.Value<string>();
            }
            catch (JsonException)
            {
                return null;
            }
        }
    }

    public class AiConnectIndexCheckJobParameters
    {
        public string ReferenceType { get; set; }
        public long ReferenceId { get; set; }
        public int AiProviderId { get; set; }
        public string FileId { get; set; }
        public int AttemptCount { get; set; }
        public int ResendCount { get; set; }

        public List<string> OldFileIds { get; set; }

        public string Language { get; set; }
    }
}
