using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace Implem.Pleasanter.Libraries.DataSources
{
    public class LineWorks
    {
        private readonly Context context;
        private readonly string title;
        private readonly string text;

        public LineWorks(Context _context, string _title, string _text)
        {
            context = _context;
            title = _title;
            text = _text.Replace("\t", "    ");
        }

        public void Send(string address)
        {
            var regex = new Regex(@"^(https?:/\/\S+)");
            var buttonUrl = regex.Match(text)?.Groups?[1]?.Value ?? "";
            var json = buttonUrl.IsNullOrEmpty() ?
                new
                {
                    title,
                    body = new { text }
                }.ToJson() :
                new
                {
                    title,
                    body = new { text },
                    button = new
                    {
                        label = Displays.ViewDetails(context),
                        url = buttonUrl
                    }
                }.ToJson();
            foreach (var url in address.Split(',').Select(x => x.Trim()))
            {
                Task.Run(() =>
                {
                    try
                    {
                        var client = new NotificationHttpClient();
                        client.NotifyString(url, json);
                    }
                    catch (Exception e)
                    {
                        _ = new SysLogModel(context, e);
                    }
                });
            }
        }
    }
}
