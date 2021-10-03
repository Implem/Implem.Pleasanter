using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
namespace Implem.Pleasanter.Libraries.DataSources
{
    public class InCircle
    {
        public Context context;
        public string text;
        public string username;
        public string token;
        public string ticketNo;

        public InCircle(Context _context, string _text, string _username, string _token)
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
                var tokenArray = token.Split(':');
                if (tokenArray.Length < 2)
                {
                    new SysLogModel(context, Displays.InCircleInvalidToken(context));
                    return;
                }
                var ticketNo = tokenArray[0];
                var tokenId = tokenArray[1];
                var postDataBytes = Encoding.UTF8.GetBytes(
                    "token_id=" + tokenId +
                    "&ticketno=" + ticketNo +
                    "&action=1" +
                    "&text=" + text);
                var req = WebRequest.Create(url);
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
                req.ContentLength = postDataBytes.Length;
                using (var reqStream = req.GetRequestStream())
                {
                    try
                    {
                        reqStream.Write(postDataBytes, 0, postDataBytes.Length);
                        req.GetResponse();
                    }
                    catch (Exception e)
                    {
                        new SysLogModel(context, e);
                    }
                }
            });
        }
    }
}