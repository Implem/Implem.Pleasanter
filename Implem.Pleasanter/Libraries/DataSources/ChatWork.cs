﻿using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using System;
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
                var postDataBytes = Encoding.UTF8.GetBytes("body=" + Uri.EscapeDataString(text));
                var req = WebRequest.Create(url);
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
                req.ContentLength = postDataBytes.Length;
                req.Headers.Add($"X-ChatWorkToken: {token}");
                using (var reqStream = req.GetRequestStream())
                {
                    try
                    {
                        reqStream.Write(postDataBytes, 0, postDataBytes.Length);
                        req.GetResponse();
                    }
                    catch(Exception e)
                    {
                        new SysLogModel(context, e);
                    }
                }
            });
        }
    }
}