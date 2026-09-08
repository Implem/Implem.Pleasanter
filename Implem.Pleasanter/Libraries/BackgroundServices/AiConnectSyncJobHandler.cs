using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.AiConnect;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    public class AiConnectSyncJobHandler : IBackgroundJobHandler
    {
        public Task ExecuteAsync(
            Context context,
            BackgroundJobModel backgroundJobModel)
        {
            var jobParameters = backgroundJobModel.JobParameters
                .Deserialize<AiConnectSyncJobParameters>();
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
            ss.SetChoiceHash(context: jobContext);
            var aiProvider = AiConnectJobContext.GetAiProvider(
                ss: ss,
                aiProviderId: jobParameters.AiProviderId);
            if (aiProvider == null)
            {
                backgroundJobModel.ResultMessage = Displays.Get(
                    context: jobContext,
                    id: "AiConnectJobSkipped");
                return Task.CompletedTask;
            }
            var replacedDisplayValues = ReplacedDisplayValues(
                context: jobContext,
                ss: ss,
                jobParameters: jobParameters);
            if (replacedDisplayValues == null)
            {
                backgroundJobModel.ResultMessage = Displays.Get(
                    context: jobContext,
                    id: "AiConnectJobSkipped");
                return Task.CompletedTask;
            }
            var outputErrorData = AiConnectUtilities.Output(
                context: jobContext,
                ss: ss,
                aiProvider: aiProvider,
                referenceType: jobParameters.ReferenceType,
                referenceId: jobParameters.ReferenceId,
                replacedDisplayValues: replacedDisplayValues);
            if (outputErrorData.Type != Error.Types.None)
            {
                throw new InvalidDataException(
                    outputErrorData.Message(context: jobContext)?.Text);
            }
            var provider = AiConnectProviderFactory.Get(
                providerType: aiProvider.ProviderType);
            if (provider == null)
            {
                new SysLogModel(
                    context: jobContext,
                    method: nameof(ExecuteAsync),
                    message: $"TenantId: {jobContext.TenantId}"
                        + $", SiteId: {ss.SiteId}"
                        + $", AiProviderId: {aiProvider.Id}"
                        + $", ProviderType: {aiProvider.ProviderType}"
                        + ", Message: The provider type is invalid.",
                    sysLogType: SysLogModel.SysLogTypes.UserError);
                throw new InvalidDataException(Displays.Get(
                    context: jobContext,
                    id: "InvalidAiProviderSetting"));
            }
            var filePath = AiConnectUtilities.FilePath(
                context: jobContext,
                ss: ss,
                aiProvider: aiProvider,
                referenceType: jobParameters.ReferenceType,
                referenceId: jobParameters.ReferenceId);
            var sendResult = provider.Send(
                context: jobContext,
                ss: ss,
                aiProvider: aiProvider,
                referenceType: jobParameters.ReferenceType,
                referenceId: jobParameters.ReferenceId,
                filePath: filePath,
                forceCreate: AiConnectUtilities.ForceCreate(
                    operation: jobParameters.Operation,
                    retryCount: jobParameters.RetryCount,
                    resendCount: jobParameters.ResendCount));
            if (sendResult.ErrorData.Type == Error.Types.ServerConnectionError)
            {
                HandleSendFailure(
                    context: jobContext,
                    ss: ss,
                    jobParameters: jobParameters);
                return Task.CompletedTask;
            }
            if (sendResult.ErrorData.Type != Error.Types.None)
            {
                throw new InvalidDataException(
                    sendResult.ErrorData.Message(context: jobContext)?.Text);
            }
            if (sendResult.Indexing != null
                && sendResult.Indexing.Status != "completed")
            {
                var indexCheckJobId = AiConnectQueueUtilities.EnqueueIndexCheck(
                    context: jobContext,
                    ss: ss,
                    referenceType: jobParameters.ReferenceType,
                    referenceId: jobParameters.ReferenceId,
                    aiProviderId: jobParameters.AiProviderId,
                    fileId: sendResult.Indexing.FileId,
                    resendCount: jobParameters.ResendCount,
                    oldFileIds: sendResult.Indexing.OldFileIds);
                if (indexCheckJobId == 0)
                {
                    LogOldFilesKept(
                        context: jobContext,
                        aiProvider: aiProvider,
                        oldFileIds: sendResult.Indexing.OldFileIds);
                }
            }
            if (sendResult.Indexing == null
                || sendResult.Indexing.Status == "completed")
            {
                backgroundJobModel.ResultMessage = Displays.Get(
                    context: jobContext,
                    id: "AiConnectJobCompleted");
            }
            return Task.CompletedTask;
        }

        private static void LogOldFilesKept(
            Context context,
            AiProvider aiProvider,
            List<string> oldFileIds)
        {
            if (oldFileIds?.Any() != true)
            {
                return;
            }
            new SysLogModel(
                context: context,
                method: nameof(LogOldFilesKept),
                message: $"AiProviderId: {aiProvider.Id}"
                    + $", OldFileIds: {oldFileIds.Join()}"
                    + ", Message: The old files are kept"
                        + " until the next update or synchronization"
                        + " because the index check job was not enqueued.",
                sysLogType: SysLogModel.SysLogTypes.Warning);
        }

        private static void HandleSendFailure(
            Context context,
            SiteSettings ss,
            AiConnectSyncJobParameters jobParameters)
        {
            var retryLimit = Parameters.AiConnect?.Rag.SyncRetryCount ?? 2;
            if (jobParameters.RetryCount >= retryLimit)
            {
                throw new InvalidDataException(Displays.Get(
                    context: context,
                    id: "AiConnectJobRetryExceeded"));
            }
            var retryJobId = AiConnectQueueUtilities.EnqueueSyncRetry(
                context: context,
                ss: ss,
                original: jobParameters);
            throw new InvalidDataException(Displays.Get(
                context: context,
                id: "AiConnectJobRetryEnqueued",
                data: retryJobId.ToString()));
        }

        private static Func<string, string> ReplacedDisplayValues(
            Context context,
            SiteSettings ss,
            AiConnectSyncJobParameters jobParameters)
        {
            switch (jobParameters.ReferenceType)
            {
                case "Issues":
                    var issueModel = new IssueModel(
                        context: context,
                        ss: ss,
                        issueId: jobParameters.ReferenceId);
                    return issueModel.AccessStatus == Databases.AccessStatuses.Selected
                        ? (Func<string, string>)(value => issueModel.ReplacedDisplayValues(
                            context: context,
                            ss: ss,
                            value: value))
                        : null;
                case "Results":
                    var resultModel = new ResultModel(
                        context: context,
                        ss: ss,
                        resultId: jobParameters.ReferenceId);
                    return resultModel.AccessStatus == Databases.AccessStatuses.Selected
                        ? (Func<string, string>)(value => resultModel.ReplacedDisplayValues(
                            context: context,
                            ss: ss,
                            value: value))
                        : null;
                default:
                    return null;
            }
        }
    }

    public class AiConnectSyncJobParameters
    {
        public string ReferenceType { get; set; }
        public long ReferenceId { get; set; }
        public int AiProviderId { get; set; }
        public AiConnectSyncOperation Operation { get; set; } = AiConnectSyncOperation.Unknown;
        public string Language { get; set; }
        public int RetryCount { get; set; }
        public int ResendCount { get; set; }
    }
}
