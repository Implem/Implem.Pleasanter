using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Responses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
namespace Implem.Pleasanter.Controllers.Api
{
    public class LineBotController : ApiController
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public async Task<HttpResponseMessage> Post()
        {
            var content = await Request.Content.ReadAsStringAsync();
            var xLineSignature = Request.Headers.GetValues("X-Line-Signature").FirstOrDefault();
            if (string.IsNullOrEmpty(xLineSignature)
                || !VerifySignature(
                    Parameters.Notification.LineChannelSecret,
                    xLineSignature,
                    content))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden);
            }
            dynamic jsonObject = JsonConvert.DeserializeObject(content);
            var ev = jsonObject.events[0];
            var replyToken = (string)ev.replyToken;
            var messageType = (string)ev.type;
            var sourceType = (string)ev.source.type;
            await ReplyMessageAsync(replyToken, new[]
            {
                Message(messageType, sourceType),
                (string)SourceId(ev, sourceType)
            });
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        private static string SourceId(dynamic ev, string sourceType)
        {
            switch (sourceType)
            {
                case "user":
                    return (string)ev.source.userId;
                case "room":
                    return (string)ev.source.roomId;
                case "group":
                    return (string)ev.source.groupId;
                default:
                    return string.Empty;
            }
        }

        private static string Message(string messageType, string sourceType)
        {
            switch (messageType)
            {
                case "follow":
                    return Displays.LineMessageFollow()
                        + Environment.NewLine
                        + Displays.LineMessageUserId();
                case "join":
                    switch (sourceType)
                    {
                        case "group":
                            return Displays.LineMessageJoinGroup()
                                + Environment.NewLine
                                + Displays.LineMessageGroupId();
                        default:
                            return Displays.LineMessageJoinRoom()
                                + Environment.NewLine
                                + Displays.LineMessageRoomId();
                    }
                default:
                    switch (sourceType)
                    {
                        case "group":
                            return Displays.LineMessageGroupId();
                        case "user":
                            return Displays.LineMessageUserId();
                        default:
                            return Displays.LineMessageRoomId();
                    }
            }
        }

        private async Task ReplyMessageAsync(string replyToken, IList<string> messageList)
        {
            var messages = messageList.Select(x => new { type = "text", text = x });
            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"https://api.line.me/v2/bot/message/reply");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer", Parameters.Notification.LineChannelAccessToken);
            var content = JsonConvert.SerializeObject(new { replyToken, messages });
            request.Content = new StringContent(content, Encoding.UTF8, "application/json");
            var response = await httpClient.SendAsync(request).ConfigureAwait(false);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                var error = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Trace.WriteLine(error);
            }
        }

        private static bool VerifySignature(
            string channelSecret, string xLineSignature, string requestBody)
        {
            try
            {
                var key = Encoding.UTF8.GetBytes(channelSecret);
                var body = Encoding.UTF8.GetBytes(requestBody);
                using (HMACSHA256 hmac = new HMACSHA256(key))
                {
                    var hash = hmac.ComputeHash(body, 0, body.Length);
                    var xLineBytes = Convert.FromBase64String(xLineSignature);
                    return SlowEquals(xLineBytes, hash);
                }
            }
            catch
            {
                return false;
            }
        }

        private static bool SlowEquals(byte[] a, byte[] b)
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
                diff |= (uint)(a[i] ^ b[i]);
            return diff == 0;
        }
    }
}
