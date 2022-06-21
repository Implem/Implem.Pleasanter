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
                view = SetSessionView(
                    context: context,
                    ss: ss,
                    useUsersView: useUsersView);
                SetSession(
                    context: context,
                    ss: ss,
                    view: view,
                    setSession: setSession,
                    useUsersView: useUsersView);
                return view;
            }
            view = SetSessionView(
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

        private static View SetSessionView(
            Context context,
            SiteSettings ss,
            bool useUsersView)
        {
            View view;
            var sessionData = useUsersView ? context.UserSessionData : context.SessionData;
            var sessionView = sessionData.Get("View")?.Deserialize<View>();
            view = sessionView != null
                ? MergeView(
                    context: context,
                    selectedView: ss.Views
                        ?.Where(o => o.Accessable(context: context))
                        .FirstOrDefault(o => o.Id == context.Forms.Int("ViewSelector"))
                            ?? new View(
                                context: context,
                                ss: ss),
                    sessionView: sessionView)
                : ss.Views?.Get(ss.GridView) ?? new View();
            return view;
        }

        private static View MergeView(
            Context context,
            View selectedView,
            View sessionView)
        {
            View mergedView;
            if (context.Forms.ControlId() == "ViewSelector")
            {
                mergedView = selectedView;
                mergedView.Id = selectedView.Id;
                if (selectedView?.KeepFilterState == true)
                {
                    mergedView.ColumnFilterHash = sessionView.ColumnFilterHash;
                }
                if (selectedView?.KeepSorterState == true)
                {
                    mergedView.ColumnSorterHash = sessionView.ColumnSorterHash;
                }
            }
            else
            {
                mergedView = sessionView;
            }
            return mergedView;
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