using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Server
{
    public class Pages
    {
        public static string Key(Context context, BaseModel baseModel, string name)
        {
            return Key(context: context) + "_" + name.ToLower();
        }

        public static string Key(Context context)
        {
            var callerOfMethod = context.Action;
            if (Sessions.Created())
            {
                var path = Url.AbsolutePath().ToLower()
                    .Split('/').Where(o => o != string.Empty).ToList();
                var methodIndex = path.IndexOf(callerOfMethod.ToLower());
                return methodIndex != -1
                    ? path.Take(methodIndex).Join("/")
                    : path.Join("/");
            }
            return string.Empty;
        }
    }
}