using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
namespace Implem.Pleasanter.Libraries.DataSources
{
    public class Teams
    {
        public Context context;
        public string text;

        public Teams(Context _context, string _text)
        {
            text = WebUtility.HtmlEncode(_text);
            text = text
                .Replace("\r\n", "<br>")
                .Replace("\n", "<br>");
            context = _context;
        }

        public void Send(string address)
        {
            foreach (var url in address.Split(',').Select(x => x.Trim()))
            {
                Task.Run(() =>
                {
                    try
                    {
                        var client = new NotificationHttpClient();
                        string json = Newtonsoft.Json.JsonConvert.SerializeObject(new { text });
                        client.NotifyString(url, json);
                    }
                    catch (Exception e)
                    {
                        new SysLogModel(context, e);
                    }
                });
            }
        }
    }
}