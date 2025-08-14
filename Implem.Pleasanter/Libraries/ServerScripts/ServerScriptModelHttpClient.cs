using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
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

        static ServerScriptModelHttpClient()
        {
            _httpClient = new HttpClient()
            {
                //キャンセルトークンでキャンセル処理をおこなうためHttpClientでのデフォルトでのタイムアウト制御はおこなわない
                Timeout = Timeout.InfiniteTimeSpan
            };
        }

        public string Get() => Core(HttpMethod.Get);

        public string Post() => Core(HttpMethod.Post);

        public string Put() => Core(HttpMethod.Put);

        public string Patch() => Core(HttpMethod.Patch);

        public string Delete() => Core(HttpMethod.Delete);

        private string Core(HttpMethod method)
        {
            try
            {
                using var cts = new CancellationTokenSource(GetTimeOut());
                var request = CreateHttpRequest(method);
                var response = _httpClient.SendAsync(request, cts.Token).Result;
                StatusCode = (int)response.StatusCode;
                IsSuccess = response.IsSuccessStatusCode;
                ResponseHeaders.Clear();
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

        private HttpRequestMessage CreateHttpRequest(HttpMethod method)
        {
            var request = new HttpRequestMessage();
            request.Method = method;
            request.RequestUri = new Uri(RequestUri);
            if(method == HttpMethod.Post
                || method == HttpMethod.Put
                || method == HttpMethod.Patch)
            {
                request.Content = new StringContent(
                    content: Content,
                    encoding: System.Text.Encoding.GetEncoding(Encoding),
                    mediaType: MediaType);
            }
            else
            {
                request.Content = null;
            }
            foreach (var header in RequestHeaders)
            {
                request.Headers.Add(header.Key, header.Value);
            }
            return request;
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