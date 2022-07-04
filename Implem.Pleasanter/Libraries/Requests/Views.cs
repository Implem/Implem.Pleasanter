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
            var viewId = context.QueryStrings.Int("ViewSelector");
            if (context.Forms.ControlId() == "ViewSelector" || viewId > 0)
            {
                viewId = (viewId > 0) ? viewId : context.Forms.Int("ViewSelector");
                view = ss.Views
                    ?.Where(o => o.Accessable(context: context))
                    .FirstOrDefault(o => o.Id == viewId)
                        ?? new View(
                            context: context,
                            ss: ss);
                if (view.KeepFilterState == true || view.KeepSorterState == true)
                {
                    var prevView = GetSessionView(
                        context: context,
                        ss: ss,
                        useUsersView: useUsersView);
                    if (view.KeepFilterState == true)
                    {
                        view.FilterColumns = prevView.FilterColumns;
                        view.Incomplete = prevView.Incomplete;
                        view.Own = prevView.Own;
                        view.NearCompletionTime = prevView.NearCompletionTime;
                        view.Delay = prevView.Delay;
                        view.Overdue = prevView.Overdue;
                        view.ColumnFilterHash = prevView.ColumnFilterHash;
                        view.ColumnFilterSearchTypes = prevView.ColumnFilterSearchTypes;
                    }
                    if (view.KeepSorterState == true)
                    {
                        view.ColumnSorterHash = prevView.ColumnSorterHash;
                    }
                }
                SetSession(
                    context: context,
                    ss: ss,
                    view: view,
                    setSession: setSession,
                    useUsersView: useUsersView);
                return view;
            }
            view = GetSessionView(
                context: context,
                ss: ss,
                useUsersView: useUsersView);
            view.SetByForm(
                context: context,
                ss: ss);
            SetSession(
                context: context,
                ss: ss,
                view: view,
                setSession: setSession,
                useUsersView: useUsersView);
            switch (ss.ReferenceType)
            {
                case "Groups":
                    view.AdditionalWhere = GroupUtilities.AdditionalWhere(
                        context: context,
                        view: view);
                    break;
            }
            return view;
        }

        private static View GetSessionView(Context context, SiteSettings ss, bool useUsersView)
        {
            View view;
            var sessionData = useUsersView ? context.UserSessionData : context.SessionData;
            view = sessionData.Get("View")?.Deserialize<View>()
                ?? ss.Views?.Get(ss.GridView)
                ?? new View();
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
            }
            if (view == null)
            {
                view = new View();
            }
            if (ss.LinkTableView != null && view.GridColumns?.Any() != true)
            {
                view.GridColumns = ss.GridColumns;
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

        public static void SetSession(
            Context context,
            SiteSettings ss,
            View view,
            bool setSession,
            string key = "View",
            bool useUsersView = false)
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