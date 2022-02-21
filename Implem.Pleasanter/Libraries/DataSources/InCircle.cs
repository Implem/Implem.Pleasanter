using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
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
                    new SysLogModel(
                        context: context,
                        method: nameof(Send),
                        message: Displays.InCircleInvalidToken(context),
                        sysLogType: SysLogModel.SysLogTypes.UserError);
                    return;
                }
                var ticketNo = tokenArray[0];
                var tokenId = tokenArray[1];
                var parameters = new Dictionary<string, string>()
                {
                    ["token_id"] = tokenId,
                    ["ticketno"] = ticketNo,
                    ["action"] = "1",
                    ["text"] = text
                };
                var client = new NotificationHttpClient();
                client.NotifyFormUrlencorded(url, parameters);
            });
        }
    }
}