using System;
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

        static ServerScriptModelHttpClient()
        {
            _httpClient = new HttpClient();
        }

        public string Get()
        {
            try
            {
                var response = _httpClient.GetAsync(requestUri: RequestUri).Result;
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
                var response = _httpClient.PostAsync(
                    requestUri: RequestUri,
                    content: content).Result;
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
                var response = _httpClient.PutAsync(
                    requestUri: RequestUri,
                    content: content).Result;
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
                var response = _httpClient.DeleteAsync(requestUri: RequestUri).Result;
                var responseContent = response.Content.ReadAsStringAsync().Result;
                return responseContent;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}