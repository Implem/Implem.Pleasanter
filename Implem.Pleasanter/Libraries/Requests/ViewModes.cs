using Implem.Libraries.Utilities;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.Requests
{
    public static class ViewModes
    {
        public static string GetSessionData(Context context, long siteId)
        {
            return SessionData(context: context).Get(siteId) ?? "index";
        }

        public static void Set(Context context, long siteId)
        {
            var data = SessionData(context: context);
            if (data.ContainsKey(siteId))
            {
                data[siteId] = context.Action;
            }
            else
            {
                data.Add(siteId, context.Action);
            }
            SessionUtilities.Set(
                context: context,
                key: "ViewMode",
                value: data.ToJson());
        }

        private static Dictionary<long, string> SessionData(Context context)
        {
            return context.SessionData
                .Get("ViewMode")?
                .Deserialize<Dictionary<long, string>>() ?? new Dictionary<long, string>();
        }
    }
}