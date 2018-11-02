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
            text = _text;
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
                        using (var client = new WebClient())
                        {

                            string json = Newtonsoft.Json.JsonConvert.SerializeObject(new { text });
                            client.Headers[HttpRequestHeader.ContentType] = "application/json;charset=UTF-8";
                            client.Headers[HttpRequestHeader.Accept] = "application/json";
                            client.Encoding = Encoding.UTF8;
                            client.UploadString(url, "POST", json);
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
}