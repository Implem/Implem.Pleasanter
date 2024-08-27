using System;
using System.Collections.Generic;
using System.Net.Http;
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
                var response = _httpClient.SendAsync(request).Result;
                StatusCode = (int)response.StatusCode;
                IsSuccess = response.IsSuccessStatusCode;
                foreach (var header in response.Headers)
                {
                    ResponseHeaders.Add(header.Key, header.Value);
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
                var response = _httpClient.SendAsync(request).Result;
                StatusCode = (int)response.StatusCode;
                IsSuccess = response.IsSuccessStatusCode;
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
                var response = _httpClient.SendAsync(request).Result;
                StatusCode = (int)response.StatusCode;
                IsSuccess = response.IsSuccessStatusCode;
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
                var response = _httpClient.SendAsync(request).Result;
                StatusCode = (int)response.StatusCode;
                IsSuccess = response.IsSuccessStatusCode;
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
                var response = _httpClient.SendAsync(request).Result;
                StatusCode = (int)response.StatusCode;
                IsSuccess = response.IsSuccessStatusCode;
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
    }
}