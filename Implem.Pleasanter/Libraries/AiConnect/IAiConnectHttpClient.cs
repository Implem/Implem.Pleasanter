using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.AiConnect
{
    public interface IAiConnectHttpClient
    {
        AiConnectHttpResponse Get(
            string url,
            IDictionary<string, string> headers);

        AiConnectHttpResponse PostFile(
            string url,
            IDictionary<string, string> headers,
            string data,
            string filePath,
            string dataPartName = "data",
            string dataContentType = "application/json");

        AiConnectHttpResponse PostJson(
            string url,
            IDictionary<string, string> headers,
            string json);

        AiConnectHttpResponse Delete(
            string url,
            IDictionary<string, string> headers);
    }
}
