using System;
using System.Net.Http;
namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptModelHttpClient
    {
        public string RequestUri { get; set; }
        public string Content { get; set; }
        public string Encoding { get; set; } = "utf-8";
        public string MediaType { get; set; } = "application/json";

        public ServerScriptModelHttpClient()
        {
        }

        public string Get()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = client.GetAsync(requestUri: RequestUri).Result;
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    return responseContent;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string Post()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var content = new StringContent(
                        content: Content,
                        encoding: System.Text.Encoding.GetEncoding(Encoding),
                        mediaType: MediaType);
                    var response = httpClient.PostAsync(
                        requestUri: RequestUri,
                        content: content).Result;
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    return responseContent;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string Put()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var content = new StringContent(
                        content: Content,
                        encoding: System.Text.Encoding.GetEncoding(Encoding),
                        mediaType: MediaType);
                    var response = httpClient.PutAsync(
                        requestUri: RequestUri,
                        content: content).Result;
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    return responseContent;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string Delete()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = httpClient.DeleteAsync(requestUri: RequestUri).Result;
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    return responseContent;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}