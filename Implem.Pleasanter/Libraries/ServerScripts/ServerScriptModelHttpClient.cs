using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
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
        public int TimeOut { get; set; }
        public int StatusCode { get; private set; }
        public bool IsSuccess { get; private set; }

        static ServerScriptModelHttpClient()
        {
            _httpClient = new HttpClient();
        }

        public string Get()
        {
            try
            {
                var request = CreateHttpRequest(HttpMethod.Get);
                _httpClient.Timeout = GetTimeOut();
                var response = _httpClient.SendAsync(request).Result;
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

        public string Post()
        {
            try
            {
                var content = new StringContent(
                    content: Content,
                    encoding: System.Text.Encoding.GetEncoding(Encoding),
                    mediaType: MediaType);
                var request = CreateHttpRequest(HttpMethod.Post, content);
                _httpClient.Timeout = GetTimeOut();
                var response = _httpClient.SendAsync(request).Result;
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

        public string Put()
        {
            try
            {
                var content = new StringContent(
                    content: Content,
                    encoding: System.Text.Encoding.GetEncoding(Encoding),
                    mediaType: MediaType);
                var request = CreateHttpRequest(HttpMethod.Put, content);
                _httpClient.Timeout = GetTimeOut();
                var response = _httpClient.SendAsync(request).Result;
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

        public string Patch()
        {
            try
            {
                var content = new StringContent(
                    content: Content,
                    encoding: System.Text.Encoding.GetEncoding(Encoding),
                    mediaType: MediaType);
                var request = CreateHttpRequest(HttpMethod.Patch, content);
                _httpClient.Timeout = GetTimeOut();
                var response = _httpClient.SendAsync(request).Result;
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

        public string Delete()
        {
            try
            {
                var request = CreateHttpRequest(HttpMethod.Delete);
                _httpClient.Timeout = GetTimeOut();
                var response = _httpClient.SendAsync(request).Result;
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
            var request = new HttpRequestMessage();
            request.Method = method;
            request.RequestUri = new Uri(RequestUri);
            request.Content = content;
            foreach (var header in RequestHeaders)
            {
                request.Headers.Add(header.Key, header.Value);
            }
            return request;
        }

        private TimeSpan GetTimeOut()
        {
            var timeOut = TimeSpan.FromMilliseconds(
                Parameters.Script.ServerScriptHttpClientTimeOut == TimeOut
                ? Parameters.Script.ServerScriptHttpClientTimeOut
                : TimeOut
            );
            var timeOutMax = TimeSpan.FromMilliseconds(Parameters.Script.ServerScriptHttpClientTimeOutMax);
            var timeOutMin = TimeSpan.FromMilliseconds(Parameters.Script.ServerScriptHttpClientTimeOutMin);

            timeOut = timeOut.Between(timeOutMin, timeOutMax)
                ? timeOut
                : TimeSpan.FromSeconds(100);

            return timeOut == TimeSpan.Zero ? Timeout.InfiniteTimeSpan : timeOut;
        }
    }
}