using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using static Implem.Pleasanter.Libraries.Settings.Notification;

namespace Implem.Pleasanter.Libraries.DataSources
{
    public class HttpClient
    {
        public Context context;
        public string text;
        public MethodTypes? MethodType;
        public string Encoding;
        public string MediaType;
        public string Headers;

        public HttpClient(Context _context, string _text)
        {
            context = _context;
            text = _text;
        }

        public void Send(string url)
        {
            System.Text.Encoding encoding = System.Text.Encoding.UTF8;
            Dictionary<string, string> headers;
            HttpMethod method;
            try
            {
                encoding = System.Text.Encoding.GetEncoding(Encoding);
                headers = Headers?.Deserialize<Dictionary<string, string>>();
                method = ConvertMethodType(MethodType);
            }
            catch (ArgumentException e)
            {
                _ = new SysLogModel(context, e);
                throw;
            }
            catch (JsonException e)
            {
                _ = new SysLogModel(context, e);
                throw;
            }
            catch (NotSupportedException e)
            {
                _ = new SysLogModel(context, e);
                throw;
            }
            Task.Run(() =>
            {
                try
                {
                    var client = new NotificationHttpClient
                    {
                        Encoding = encoding,
                        ContentType = MediaType
                    };
                    client.NotifyString(
                        url: url,
                        content: text,
                        method: method,
                        headers: headers);
                }
                catch (Exception e)
                {
                    _ = new SysLogModel(context, e);
                }
            });
        }

        private static HttpMethod ConvertMethodType(MethodTypes? methodType)
        {
            switch(methodType)
            {
                case null:
                    return HttpMethod.Post;
                case MethodTypes.Get:
                    return HttpMethod.Get;
                case MethodTypes.Post:
                    return HttpMethod.Post;
                case MethodTypes.Put:
                    return HttpMethod.Put;
                case MethodTypes.Delete:
                    return HttpMethod.Delete;
                default:
                    throw new NotSupportedException($"Unknown method type: {methodType}");
            }
        }
    }
}
