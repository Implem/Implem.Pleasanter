using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
namespace Implem.Pleasanter.Libraries.AiConnect
{
    public class AiConnectHttpClient : IAiConnectHttpClient
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private static readonly HttpClient loopbackHttpClient = new HttpClient(
            new HttpClientHandler
            {
                UseProxy = false,
                AllowAutoRedirect = false
            });
        public static readonly IAiConnectHttpClient Default = new AiConnectHttpClient();

        public static bool IsLoopbackHttpRequest(HttpRequestMessage request)
        {
            if (request?.RequestUri?.IsAbsoluteUri != true)
            {
                return false;
            }
            return request.RequestUri.Scheme == Uri.UriSchemeHttp
                && request.RequestUri.IsLoopback;
        }

        public AiConnectHttpResponse Get(
            string url,
            IDictionary<string, string> headers)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                return Send(
                    request: request,
                    headers: headers);
            }
        }

        public AiConnectHttpResponse Delete(
            string url,
            IDictionary<string, string> headers)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Delete, url))
            {
                return Send(
                    request: request,
                    headers: headers);
            }
        }

        public AiConnectHttpResponse PostFile(
            string url,
            IDictionary<string, string> headers,
            string data,
            string filePath,
            string dataPartName = "data",
            string dataContentType = "application/json")
        {
            try
            {
                using (var request = new HttpRequestMessage(HttpMethod.Post, url))
                {
                    request.Content = new MultipartFormDataContent();
                    var content = (MultipartFormDataContent)request.Content;
                    content.Add(
                        new StringContent(
                            content: data,
                            encoding: Encoding.UTF8,
                            mediaType: dataContentType),
                        dataPartName);
                    var fileContent = new ByteArrayContent(File.ReadAllBytes(filePath));
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue("text/markdown");
                    content.Add(
                        fileContent,
                        "file",
                        Path.GetFileName(filePath));
                    return Send(
                        request: request,
                        headers: headers);
                }
            }
            catch (Exception e)
            {
                return Failed(e: e);
            }
        }

        public AiConnectHttpResponse PostJson(
            string url,
            IDictionary<string, string> headers,
            string json)
        {
            try
            {
                using (var request = new HttpRequestMessage(HttpMethod.Post, url))
                {
                    request.Content = new StringContent(
                        content: json,
                        encoding: Encoding.UTF8,
                        mediaType: "application/json");
                    return Send(
                        request: request,
                        headers: headers);
                }
            }
            catch (Exception e)
            {
                return Failed(e: e);
            }
        }

        private static AiConnectHttpResponse Send(
            HttpRequestMessage request,
            IDictionary<string, string> headers)
        {
            try
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }
                var client = IsLoopbackHttpRequest(request: request)
                    ? loopbackHttpClient
                    : httpClient;
                using (var response = client.Send(request))
                {
                    return new AiConnectHttpResponse
                    {
                        StatusCode = (int)response.StatusCode,
                        Body = ResponseBody(response: response),
                        RetryAfterSeconds = RetryAfterSeconds(response: response)
                    };
                }
            }
            catch (Exception e)
            {
                return Failed(e: e);
            }
        }

        private static AiConnectHttpResponse Failed(Exception e)
        {
            return new AiConnectHttpResponse
            {
                StatusCode = 0,
                Body = string.Empty,
                Exception = e
            };
        }

        private static string ResponseBody(HttpResponseMessage response)
        {
            try
            {
                using (var stream = response.Content.ReadAsStream())
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        private static int? RetryAfterSeconds(HttpResponseMessage response)
        {
            var delta = response.Headers.RetryAfter?.Delta;
            return delta != null
                ? (int)delta.Value.TotalSeconds
                : (int?)null;
        }
    }
}
