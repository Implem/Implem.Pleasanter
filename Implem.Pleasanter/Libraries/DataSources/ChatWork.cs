using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
namespace Implem.Pleasanter.Libraries.DataSources
{
    public class ChatWork
    {
        public Context context;
        public string text;
        public string username;
        public string token;
       
        public ChatWork(Context _context, string _text, string _username, string _token)
        {
            context = _context;
            text = _text;
            username = _username;
            token = _token;
        }

        public void Send(string url)
        {
            Task.Run(() =>
            {
                var client = new NotificationHttpClient();
                client.NotifyFormUrlencorded(
                    url: url,
                    parameters: new Dictionary<string, string>()
                    {
                        ["body"] = text
                    },
                    headers: new Dictionary<string, string>()
                    {
                        ["X-ChatWorkToken"] = token
                    });
            });
        }
    }
}