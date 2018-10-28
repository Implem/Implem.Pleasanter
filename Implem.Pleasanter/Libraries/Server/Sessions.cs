using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using System;
using System.Web;
namespace Implem.Pleasanter.Libraries.Server
{
    public static class Sessions
    {
        public static void Set(string name, object data)
        {
            HttpContext.Current.Session[name] = data;
        }

        public static void Abandon()
        {
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
        }

        public static void Clear(string name)
        {
            HttpContext.Current.Session[name] = null;
        }

        public static bool Created()
        {
            return HttpContext.Current?.Session != null;
        }

        public static double SessionAge()
        {
            return Created()
                ? (DateTime.Now - HttpContext.Current.Session["StartTime"].ToDateTime())
                    .TotalMilliseconds
                : 0;
        }

        public static double SessionRequestInterval()
        {
            if (Created())
            {
                var ret = (DateTime.Now - HttpContext.Current.Session["LastAccessTime"].ToDateTime())
                    .TotalMilliseconds;
                HttpContext.Current.Session["LastAccessTime"] = DateTime.Now;
                return ret;
            }
            else
            {
                return 0;
            }
        }

        public static string SessionGuid()
        {
            return HttpContext.Current.Session?["SessionGuid"].ToString();
        }

        public static Message Message()
        {
            var message = HttpContext.Current.Session["Message"] as Message;
            if (message != null) Clear("Message");
            return message;
        }

        public static object PageSession(this BaseModel baseModel, Context context, string name)
        {
            return HttpContext.Current.Session[Pages.Key(
                context: context,
                baseModel: baseModel,
                name: name)];
        }

        public static void PageSession(
            this BaseModel baseModel, Context context, string name, object value)
        {
            HttpContext.Current.Session[Pages.Key(
                context: context,
                baseModel: baseModel,
                name: name)] = value;
        }
    }
}