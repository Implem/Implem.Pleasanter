using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Libraries.AiConnect
{
    public class AiConnectDifyProvider : IAiConnectProvider
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
            return new AiConnectSendResult
            {
                ErrorData = AiConnectDifyUtilities.SendCore(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    referenceType: referenceType,
                    referenceId: referenceId,
                    filePath: filePath,
                    client: client,
                    forceCreate: forceCreate),
                Indexing = null
            };
        }

        public AiConnectDeleteAllResult DeleteAll(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            IAiConnectHttpClient client = null)
        {
            var result = new AiConnectDeleteAllResult();
            var documentIds = AiConnectDifyUtilities.DocumentIds(
                context: context,
                ss: ss,
                aiProvider: aiProvider,
                client: client,
                fileNamePrefix: AiConnectUtilities.FileNamePrefix(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider));
            if (documentIds == null)
            {
                result.ErrorData = new ErrorData(type: Error.Types.ServerConnectionError);
                return result;
            }
            documentIds.ForEach(documentId =>
            {
                var response = AiConnectDifyUtilities.Delete(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    documentId: documentId,
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
            var documentIds = AiConnectDifyUtilities.DocumentIdsByName(
                context: context,
                ss: ss,
                aiProvider: aiProvider,
                fileName: fileName,
                client: client);
            if (documentIds == null)
            {
                return new ErrorData(type: Error.Types.ServerConnectionError);
            }
            var failed = false;
            documentIds.ForEach(documentId =>
            {
                var response = AiConnectDifyUtilities.Delete(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    documentId: documentId,
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
            return AiConnectDifyUtilities.ValidateConnectionSetting(
                context: context,
                aiProvider: aiProvider);
        }
    }
}
