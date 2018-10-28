using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
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