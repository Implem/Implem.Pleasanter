using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Linq;
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
                    ss: ss,
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
                    ss: ss,
                    view: view,
                    setSession: setSession);
                return view;
            }
            view = context.SessionData.Get("View")?.Deserialize<View>()
                ?? ss.Views?.Get(ss.GridView)
                ?? new View();
            view.SetByForm(
                context: context,
                ss: ss);
            SetSession(
                context: context,
                ss: ss,
                view: view,
                setSession: setSession);
            return view;
        }

        public static View GetBySession(
            Context context,
            SiteSettings ss,
            string dataTableName,
            bool setSession = false)
        {
            var key = "View_" + dataTableName;
            var view = context.SessionData.Get(key)?.Deserialize<View>();
            if (view == null)
            {
                view = ss.Views.Get(ss.LinkTableView);
                if (view != null && view.GridColumns?.Any() != true)
                {
                    view.GridColumns = ss.GridColumns;
                }
            }
            if (view == null)
            {
                view = new View();
            }
            if (setSession)
            {
                view.SetByForm(
                    context: context,
                    ss: ss);
                SetSession(
                    context: context,
                    ss: ss,
                    view: view,
                    setSession: true,
                    key: key);
            }
            return view;
        }

        private static void SetSession(
            Context context, SiteSettings ss, View view, bool setSession, string key = "View")
        {
            if (setSession)
            {
                SessionUtilities.Set(
                    context: context,
                    ss: ss,
                    key: key,
                    view: view);
            }
        }
    }
}