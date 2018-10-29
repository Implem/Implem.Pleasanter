using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
namespace Implem.Pleasanter.Libraries.Requests
{
    public static class Views
    {
        public static View GetBySession(Context context, SiteSettings ss)
        {
            var view = !Request.IsAjax()
                ? QueryStrings.Data("View")?.Deserialize<View>()
                : null;
            var key = Key(context, ss);
            if (view != null)
            {
                SessionUtilities.Set(
                    context: context,
                    key: key,
                    view: view);
                return view;
            }
            if (Forms.ControlId() == "ViewSelector")
            {
                view = ss.Views?.Get(Forms.Int("ViewSelector"))
                    ?? new View(context: context, ss: ss);
                SessionUtilities.Set(
                    context: context,
                    key: key,
                    view: view);
                return view;
            }
            view = SessionUtilities.View(
                context: context,
                key: key)
                    ?? ss.Views?.Get(ss.GridView)
                    ?? new View(
                        context: context,
                        ss: ss);
            view.SetByForm(context: context, ss: ss);
            SessionUtilities.Set(
                context: context,
                key: key,
                view: view);
            return view;
        }

        private static string Key(Context context, SiteSettings ss)
        {
            return "View" + (ss.SiteId == 0
                ? Pages.Key(context: context)
                : ss.SiteId.ToString());
        }
    }
}