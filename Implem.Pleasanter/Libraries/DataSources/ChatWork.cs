using Implem.Libraries.Utilities;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
namespace Implem.Pleasanter.Libraries.DataSources
{
    public class ChatWork
    {
        public string text;
        public string username;
        public string token;

        public ChatWork(string _text, string _username, string _token)
        {
            text = _text;
            username = _username;
            token = _token;
        }

        public void Send(string url)
        {
 
            //文字コードを指定する
            var enc = Encoding.GetEncoding("UTF-8");

            // パラメタのエンコード・構築
            var postData = "body=" + Uri.EscapeDataString(this.text);
            var postDataBytes = System.Text.Encoding.ASCII.GetBytes(postData);
            var req = WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = postDataBytes.Length;
            req.Headers.Add(string.Format("X-ChatWorkToken: {0}", token));

            // データをPOST送信するためのStreamを取得
            var reqStream = req.GetRequestStream();
            reqStream.Write(postDataBytes, 0, postDataBytes.Length);
            reqStream.Close();

            //非同期でPOST
            req.GetResponseAsync();
        }
    }
}