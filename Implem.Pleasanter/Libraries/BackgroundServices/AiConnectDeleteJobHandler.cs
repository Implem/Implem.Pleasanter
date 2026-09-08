using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.AiConnect;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.IO;
using System.Threading.Tasks;
namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    public class AiConnectDeleteJobHandler : IBackgroundJobHandler
    {
        public Task ExecuteAsync(
            Context context,
            BackgroundJobModel backgroundJobModel)
        {
            var jobParameters = backgroundJobModel.JobParameters
                .Deserialize<AiConnectDeleteJobParameters>();
            if (jobParameters == null)
            {
                throw new InvalidDataException(Displays.Get(
                    context: context,
                    id: "BackgroundJobInvalidParameters"));
            }
            var jobContext = AiConnectJobContext.CreateContext(
                backgroundJobModel: backgroundJobModel,
                language: jobParameters.Language);
            if (Parameters.AiConnect?.Rag.Enabled != true
                || Parameters.AiConnect?.Rag.DeleteEnabled != true)
            {
                backgroundJobModel.ResultMessage = Displays.Get(
                    context: jobContext,
                    id: "AiConnectDeleteJobSkipped");
                return Task.CompletedTask;
            }
            var ss = AiConnectJobContext.GetSiteSettings(
                context: jobContext,
                siteId: backgroundJobModel.SiteId);
            var aiProvider = AiConnectJobContext.GetAiProvider(
                ss: ss,
                aiProviderId: jobParameters.AiProviderId);
            if (aiProvider == null)
            {
                backgroundJobModel.ResultMessage = Displays.Get(
                    context: jobContext,
                    id: "AiConnectDeleteJobSkipped");
                return Task.CompletedTask;
            }
            var errorData = AiConnectUtilities.DeleteCore(
                context: jobContext,
                ss: ss,
                aiProvider: aiProvider,
                referenceType: jobParameters.ReferenceType,
                referenceId: jobParameters.ReferenceId,
                transientFailed: out var transientFailed);
            if (errorData.Type == Error.Types.ServerConnectionError
                || transientFailed)
            {
                HandleDeleteFailure(
                    context: jobContext,
                    ss: ss,
                    jobParameters: jobParameters);
            }
            if (errorData.Type != Error.Types.None)
            {
                throw new InvalidDataException(
                    errorData.Message(context: jobContext)?.Text);
            }
            backgroundJobModel.ResultMessage = Displays.Get(
                context: jobContext,
                id: "AiConnectDeleteJobCompleted");
            return Task.CompletedTask;
        }

        private static void HandleDeleteFailure(
            Context context,
            SiteSettings ss,
            AiConnectDeleteJobParameters jobParameters)
        {
            var retryLimit = Parameters.AiConnect?.Rag.DeleteRetryCount ?? 2;
            if (jobParameters.RetryCount >= retryLimit)
            {
                throw new InvalidDataException(Displays.Get(
                    context: context,
                    id: "AiConnectDeleteJobRetryExceeded"));
            }
            var retryJobId = AiConnectQueueUtilities.EnqueueDeleteRetry(
                context: context,
                ss: ss,
                original: jobParameters);
            throw new InvalidDataException(Displays.Get(
                context: context,
                id: "AiConnectDeleteJobRetryEnqueued",
                data: retryJobId.ToString()));
        }
    }

    public class AiConnectDeleteJobParameters
    {
        public string ReferenceType { get; set; }
        public long ReferenceId { get; set; }
        public int AiProviderId { get; set; }
        public string Language { get; set; }
        public int RetryCount { get; set; }
    }
}
