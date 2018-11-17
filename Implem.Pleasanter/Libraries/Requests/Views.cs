using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
namespace Implem.Pleasanter.Libraries.Requests
{
    public static class Views
    {
        public static View GetBySession(Context context, SiteSettings ss, bool setSession = true)
        {
            var view = !context.Ajax
                ? context.QueryStrings.Data("View")?.Deserialize<View>()
                : null;
            if (view != null)
            {
                SetSession(
                    context: context,
                    view: view,
                    setSession: setSession);
                return view;
            }
            if (context.Forms.ControlId() == "ViewSelector")
            {
                view = ss.Views?.Get(context.Forms.Int("ViewSelector"))
                    ?? new View(context: context, ss: ss);
                SetSession(
                    context: context,
                    view: view,
                    setSession: setSession);
                return view;
            }
            view = context.SessionData.Get("View")?.Deserialize<View>()
                ?? ss.Views?.Get(ss.GridView)
                ?? new View(
                    context: context,
                    ss: ss);
            view.SetByForm(context: context, ss: ss);
            SetSession(
                context: context,
                view: view,
                setSession: setSession);
            return view;
        }

        private static void SetSession(Context context, View view, bool setSession)
        {
            if (setSession)
            {
                SessionUtilities.Set(
                    context: context,
                    key: "View",
                    view: view);
            }
        }
    }
}