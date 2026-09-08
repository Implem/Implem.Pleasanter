using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Libraries.AiConnect
{
    public class AiConnectOpenAiProvider : IAiConnectProvider
    {
        public AiConnectSendResult Send(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            string referenceType,
            long referenceId,
            string filePath,
            IAiConnectHttpClient client = null,
            bool forceCreate = false)
        {
            return AiConnectOpenAiUtilities.SendCore(
                context: context,
                ss: ss,
                aiProvider: aiProvider,
                referenceType: referenceType,
                referenceId: referenceId,
                filePath: filePath,
                client: client,
                forceCreate: forceCreate);
        }

        public AiConnectDeleteAllResult DeleteAll(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            IAiConnectHttpClient client = null)
        {
            var result = new AiConnectDeleteAllResult();
            var fileIds = AiConnectOpenAiUtilities.FileIdsByPrefix(
                context: context,
                ss: ss,
                aiProvider: aiProvider,
                client: client,
                errorType: out var errorType);
            if (fileIds == null)
            {
                result.ErrorData = new ErrorData(type: errorType);
                return result;
            }
            fileIds.ForEach(fileId =>
            {
                var response = AiConnectOpenAiUtilities.DeleteFile(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    fileId: fileId,
                    client: client);
                if (response.Succeeded)
                {
                    result.DeletedCount++;
                }
                else
                {
                    result.FailedCount++;
                }
            });
            return result;
        }

        public ErrorData DeleteDocument(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            string fileName,
            IAiConnectHttpClient client = null)
        {
            var fileIds = AiConnectOpenAiUtilities.FileIds(
                context: context,
                ss: ss,
                aiProvider: aiProvider,
                fileName: fileName,
                client: client,
                errorType: out var errorType);
            if (fileIds == null)
            {
                return new ErrorData(type: errorType);
            }
            var failed = false;
            fileIds.ForEach(fileId =>
            {
                var response = AiConnectOpenAiUtilities.DeleteFile(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    fileId: fileId,
                    client: client);
                if (response.Succeeded == false)
                {
                    failed = true;
                }
            });
            return failed
                ? new ErrorData(type: Error.Types.ServerConnectionError)
                : new ErrorData(type: Error.Types.None);
        }

        public ErrorData ValidateConnectionSetting(
            Context context,
            AiProvider aiProvider)
        {
            return AiConnectOpenAiUtilities.ValidateConnectionSetting(
                context: context,
                aiProvider: aiProvider);
        }
    }
}
