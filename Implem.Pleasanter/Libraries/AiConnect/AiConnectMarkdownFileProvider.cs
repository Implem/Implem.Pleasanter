using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Libraries.AiConnect
{
    public class AiConnectMarkdownFileProvider : IAiConnectProvider
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
                ErrorData = new ErrorData(type: Error.Types.None),
                Indexing = null
            };
        }

        public AiConnectDeleteAllResult DeleteAll(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            IAiConnectHttpClient client = null)
        {
            return new AiConnectDeleteAllResult();
        }

        public ErrorData DeleteDocument(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            string fileName,
            IAiConnectHttpClient client = null)
        {
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData ValidateConnectionSetting(
            Context context,
            AiProvider aiProvider)
        {
            return new ErrorData(type: Error.Types.None);
        }
    }
}
