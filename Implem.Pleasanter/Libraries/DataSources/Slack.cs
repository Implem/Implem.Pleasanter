using Implem.Libraries.Utilities;
using System.Net;
using System.Text;
using System.Threading.Tasks;
namespace Implem.Pleasanter.Libraries.DataSources
{
    public class Slack
    {
        public string text;
        public string username;
        public string icon_emoji;

        public Slack(string _text, string _username, string _icon_emoji = null)
        {
            text = _text;
            username = _username;
            icon_emoji = _icon_emoji;
        }

        public void Send(string url)
        {
            Task.Run(() =>
            {
                using (var client = new WebClient())
                {
                    client.Headers.Add(
                        HttpRequestHeader.ContentType, "application/json;charset=UTF-8");
                    client.Encoding = Encoding.UTF8;
                    client.UploadString(url, this.ToJson());
                }
            });
        }
    }
}