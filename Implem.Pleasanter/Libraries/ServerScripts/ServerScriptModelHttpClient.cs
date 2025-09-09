using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptModelHttpClient
    {
        private static readonly HttpClient _httpClient;
        public string RequestUri { get; set; }
        public string Content { get; set; }
        public string Encoding { get; set; } = "utf-8";
        public string MediaType { get; set; } = "application/json";
        public Dictionary<string, string> RequestHeaders { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, IList<string>> ResponseHeaders { get; set; } = new Dictionary<string, IList<string>>();
        public int TimeOut { get; set; } = Parameters.Script.ServerScriptHttpClientTimeOut;
        public int StatusCode { get; private set; }
        public bool IsSuccess { get; private set; }
        public bool IsTimeOut { get; private set; }

        static ServerScriptModelHttpClient()
        {
            _httpClient = new HttpClient()
            {
                Timeout = Timeout.InfiniteTimeSpan
            };
        }

        public string Get() => Core(HttpMethod.Get);

        public string Post() => Core(HttpMethod.Post, CreateContent());

        public string Put() => Core(HttpMethod.Put, CreateContent());

        public string Patch() => Core(HttpMethod.Patch, CreateContent());

        public string Delete() => Core(HttpMethod.Delete);

        private string Core(HttpMethod method, HttpContent content = null)
        {
            try
            {
                ResponseHeaders.Clear();
                StatusCode = default;
                IsSuccess = false;
                IsTimeOut = false;
                using var cts = new CancellationTokenSource();
                cts.CancelAfter(GetTimeOut());
                var request = CreateHttpRequest(method, content);
                HttpResponseMessage response;
                try
                {
                    response = _httpClient.SendAsync(request, cts.Token).Result;
                }
                catch (OperationCanceledException ex) when (ex.CancellationToken == cts.Token)
                {
                    IsTimeOut = true;
                    return default;
                }
                StatusCode = (int)response.StatusCode;
                IsSuccess = response.IsSuccessStatusCode;
                foreach (var header in response.Headers)
                {
                    ResponseHeaders.Add(header.Key, header.Value.ToArray());
                }
                var responseContent = response.Content.ReadAsStringAsync().Result;
                return responseContent;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private HttpRequestMessage CreateHttpRequest(HttpMethod method, HttpContent content = null)
        {
            var request = new HttpRequestMessage()
            {
                Method = method,
                RequestUri = new Uri(RequestUri),
                Content = content
            };
            foreach (var header in RequestHeaders)
            {
                request.Headers.Add(header.Key, header.Value);
            }
            return request;
        }

        private HttpContent CreateContent()
        {
            if (Content.IsNullOrEmpty())
            {
                return null;
            }
            return new StringContent(
                content: Content,
                encoding: System.Text.Encoding.GetEncoding(Encoding),
                mediaType: MediaType);
        }

        private TimeSpan GetTimeOut()
        {
            var timeOut = TimeSpan.FromMilliseconds(TimeOut);
            var timeOutMax = TimeSpan.FromMilliseconds(Parameters.Script.ServerScriptHttpClientTimeOutMax);
            var timeOutMin = TimeSpan.FromMilliseconds(Parameters.Script.ServerScriptHttpClientTimeOutMin);

            timeOut = timeOut.Between(timeOutMin, timeOutMax)
                ? timeOut
                : TimeSpan.FromSeconds(100);

            return timeOut == TimeSpan.Zero ? Timeout.InfiniteTimeSpan : timeOut;
        }
    }
}