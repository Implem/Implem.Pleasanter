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
        private static readonly System.Net.Http.HttpClient _httpClient;
        public string ContentType { get; set; } = "text/plain";
        public Encoding Encoding { get; set; } = Encoding.UTF8;
        
        static NotificationHttpClient()
        {
            _httpClient = new System.Net.Http.HttpClient();
        }

        public NotificationHttpClient()
        {
        }

        public void NotifyString(string url, string content, HttpMethod method, IDictionary<string, string> headers = null)
        {
            var request = new HttpRequestMessage(method, url);
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
            var msg = response.EnsureSuccessStatusCode();
            Console.WriteLine(msg);
        }

        public void NotifyString(string url, string content, IDictionary<string, string> headers = null)
        {
            NotifyString(url, content, HttpMethod.Post, headers);
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
