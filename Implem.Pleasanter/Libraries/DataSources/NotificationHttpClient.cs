using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Implem.Pleasanter.Libraries.DataSources
{
    public class NotificationHttpClient
    {
        private static readonly HttpClient _httpClient;
        public string ContentType { get; set; } = "application/json";
        public Encoding Encoding { get; set; } = Encoding.UTF8;
        
        static NotificationHttpClient()
        {
            _httpClient = new HttpClient();
        }

        public NotificationHttpClient()
        {
        }

        public void NotifyString(string url, string content, IDictionary<string, string> headers = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }
            request.Content = new StringContent(
                content: content,
                encoding: Encoding,
                mediaType: ContentType);
            var response = _httpClient.Send(request);
            response.EnsureSuccessStatusCode();
        }

        public void NotifyFormUrlencorded(string url,IDictionary<string,string> parameters, IDictionary<string, string> headers = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }
            request.Content = new FormUrlEncodedContent(parameters);
            var response = _httpClient.Send(request);
            response.EnsureSuccessStatusCode();
        }
    }
}
