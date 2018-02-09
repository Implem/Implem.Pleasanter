using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
namespace Implem.Pleasanter.Libraries.DataSources
{
    public class ChatWork
    {
        public string text;
        public string username;
        public string token;

        public ChatWork(string _text, string _username, string _token)
        {
            text = _text;
            username = _username;
            token = _token;
        }

        public void Send(string url)
        {
            Task.Run(() =>
            {
                var postDataBytes = Encoding.ASCII.GetBytes("body=" + Uri.EscapeDataString(text));
                var req = WebRequest.Create(url);
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                req.ContentLength = postDataBytes.Length;
                req.Headers.Add($"X-ChatWorkToken: {token}");
                using (var reqStream = req.GetRequestStream())
                {
                    try
                    {
                        reqStream.Write(postDataBytes, 0, postDataBytes.Length);
                    }
                    catch
                    {
                        throw;
                    }
                }
            });
        }
    }
}