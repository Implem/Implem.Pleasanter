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
            var useUsersView = ss.SaveViewType == SiteSettings.SaveViewTypes.User;
            setSession = setSession && ss.SaveViewType != SiteSettings.SaveViewTypes.None;
            var view = !context.Ajax
                ? context.QueryStrings.Data("View")?.Deserialize<View>()
                : null;
            if (view != null)
            {
                SetSession(
                    context: context,
                    ss: ss,
                    view: view,
                    setSession: setSession,
                    useUsersView: useUsersView);
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
                    setSession: setSession,
                    useUsersView: useUsersView);
                return view;
            }
            var sessionData = useUsersView ? context.UserSessionData : context.SessionData;
            view = sessionData.Get("View")?.Deserialize<View>()
                ?? ss.Views?.Get(ss.GridView)
                ?? new View();
            view.SetByForm(
                context: context,
                ss: ss);
            SetSession(
                context: context,
                ss: ss,
                view: view,
                setSession: setSession,
                useUsersView: useUsersView);
            return view;
        }

        public static View GetBySession(
            Context context,
            SiteSettings ss,
            string dataTableName,
            bool setSession = false)
        {
            var key = "View_" + dataTableName;
            var useUsersView = ss.SaveViewType == SiteSettings.SaveViewTypes.User;
            var sessionData = useUsersView ? context.UserSessionData : context.SessionData;
            setSession = setSession && ss.SaveViewType != SiteSettings.SaveViewTypes.None;
            var view = sessionData.Get(key)?.Deserialize<View>();
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
                    key: key,
                    useUsersView: useUsersView);
            }
            return view;
        }

        private static void SetSession(
            Context context, SiteSettings ss, View view, bool setSession, string key = "View", bool useUsersView=false)
        {
            if (setSession)
            {
                SessionUtilities.Set(
                    context: context,
                    ss: ss,
                    key: key,
                    view: view,
                    sessionGuid: useUsersView
                        ? "@" + context.UserId 
                        : null);
            }
        }
    }
}