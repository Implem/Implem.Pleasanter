using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.AiConnect
{
    public interface IAiConnectProvider
    {
        AiConnectSendResult Send(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            string referenceType,
            long referenceId,
            string filePath,
            IAiConnectHttpClient client = null,
            bool forceCreate = false);

        AiConnectDeleteAllResult DeleteAll(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            IAiConnectHttpClient client = null);

        ErrorData DeleteDocument(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            string fileName,
            IAiConnectHttpClient client = null);

        ErrorData ValidateConnectionSetting(
            Context context,
            AiProvider aiProvider);
    }

    public class AiConnectDeleteAllResult
    {
        public int DeletedCount { get; set; }
        public int FailedCount { get; set; }
        public ErrorData ErrorData { get; set; } = new ErrorData(type: Error.Types.None);
    }

    public class AiConnectSendResult
    {
        public ErrorData ErrorData { get; set; } = new ErrorData(type: Error.Types.None);
        public AiConnectIndexingInfo Indexing { get; set; }
    }

    public class AiConnectIndexingInfo
    {
        public string FileId { get; set; }
        public string Status { get; set; }

        public List<string> OldFileIds { get; set; } = new List<string>();
    }
}
