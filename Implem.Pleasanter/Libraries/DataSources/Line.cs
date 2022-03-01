using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
namespace Implem.Pleasanter.Libraries.DataSources
{
    public class Line
    {
        public Context context;
        public string text;
        public string username;
        public string token;

        public Line(Context _context, string _text, string _username, string _token)
        {
            text = _text;
            username = _username;
            token = _token;
            context = _context;
        }

        public void Send(string toUsers, bool isGroup)
        {
            Task.Run(() =>
            {
                try
                {
                    var client = new NotificationHttpClient();
                    if (!isGroup)
                    {
                        var users = toUsers.Split(',')
                            .Where(s => !string.IsNullOrEmpty(s))
                            .Select(s => s.Trim()).ToArray();
                        if (users.Length > 0)
                        {
                            string json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                            {
                                to = users,
                                messages = new[] { new { type = "text", text } }
                            });
                            client.NotifyString(
                                url: "https://api.line.me/v2/bot/message/multicast",
                                content: json,
                                headers: new Dictionary<string, string>()
                                {
                                    ["Authorization"] = "Bearer " + token
                                });
                        }
                    }
                    else
                    {
                        var group = toUsers.Split(',').FirstOrDefault()?.Trim();
                        if (!string.IsNullOrEmpty(group))
                        {
                            string json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                            {
                                to = group,
                                messages = new[] { new { type = "text", text } }
                            });
                            client.NotifyString(
                                url: "https://api.line.me/v2/bot/message/push",
                                content: json,
                                headers: new Dictionary<string, string>()
                                {
                                    ["Authorization"] = "Bearer " + token
                                });
                        }
                    }
                }
                catch (Exception e)
                {
                    new SysLogModel(context, e);
                }
            });
        }
    }
}