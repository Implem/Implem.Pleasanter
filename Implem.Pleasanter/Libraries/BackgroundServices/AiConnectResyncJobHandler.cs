using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.AiConnect;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using System.IO;
using System.Threading.Tasks;
namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    public class AiConnectResyncJobHandler : IBackgroundJobHandler
    {
        public Task ExecuteAsync(
            Context context,
            BackgroundJobModel backgroundJobModel)
        {
            var jobParameters = backgroundJobModel.JobParameters
                .Deserialize<AiConnectResyncJobParameters>();
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
            if (aiProvider == null)
            {
                backgroundJobModel.ResultMessage = Displays.Get(
                    context: jobContext,
                    id: "AiConnectJobSkipped");
                return Task.CompletedTask;
            }
            var result = AiConnectResyncUtilities.Resync(
                context: jobContext,
                ss: ss,
                aiProvider: aiProvider);
            var resultMessage = Displays.Get(
                context: jobContext,
                id: "SyncAiProvidersCompleted",
                data: new string[]
                {
                    result.DeletedCount.ToString(),
                    result.SucceededCount.ToString(),
                    (result.FailedCount + result.DeleteFailedCount).ToString()
                });
            if (result.ErrorData.Type != Error.Types.None)
            {
                backgroundJobModel.ResultMessage = resultMessage
                    + " "
                    + result.ErrorData.Message(context: jobContext)?.Text;
                throw new InvalidDataException(backgroundJobModel.ResultMessage);
            }
            backgroundJobModel.ResultMessage = resultMessage;
            return Task.CompletedTask;
        }
    }

    public class AiConnectResyncJobParameters
    {
        public int AiProviderId { get; set; }
        public string Language { get; set; }
    }
}
