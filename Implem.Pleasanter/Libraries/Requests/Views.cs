using Implem.DefinitionAccessor;
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
                SetGridColumnsDefault(
                    context: context,
                    view: view);
                return view;
            }
            var processId = context.Forms.Int("BulkProcessingItems");
            if (processId > 0)
            {
                var process = ss.GetProcess(
                    context: context,
                    id: processId);
                if (process != null)
                {
                    view = GetView(
                        context: context,
                        ss: ss,
                        useUsersView: useUsersView);
                    process.View?.CopyViewFilters(view: view);
                    switch (process.CurrentStatus)
                    {
                        case -1:
                            break;
                        case 0:
                            view.ColumnFilterHash.AddOrUpdate("Status", $"[\"\t\"]");
                            break;
                        default:
                            view.ColumnFilterHash.AddOrUpdate("Status", $"[\"{process.CurrentStatus}\"]");
                            break;
                    }
                    return view;
                }
            }
            if (context.Forms.ControlId() == "ViewSelector"
                || context.QueryStrings.ContainsKey("ViewSelector"))
            {
                var viewId = context.RequestData(
                    name: "ViewSelector",
                    either: true).ToInt();
                view = ss.Views
                    ?.Where(o => o.Accessable(context: context))
                    .FirstOrDefault(o => o.Id == viewId)
                        ?? new View(
                            context: context,
                            ss: ss);
                if (view.KeepFilterState == true || view.KeepSorterState == true)
                {
                    var prevView = GetView(
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
                        view.Search = prevView.Search;
                        view.ColumnFilterNegatives = prevView.ColumnFilterNegatives;
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
                SetGridColumnsDefault(
                    context: context,
                    view: view);
                return view;
            }
            view = GetView(
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
            SetGridColumnsDefault(
                context: context,
                view: view);
            return view;
        }

        private static void SetGridColumnsDefault(Context context, View view)
        {
            if (context.RequestData(
                name: "SetGridColumnsDefault",
                either: true).ToBool())
            {
                view.GridColumns = null;
            }
        }

        private static View GetView(Context context, SiteSettings ss, bool useUsersView)
        {
            View view;
            var sessionData = useUsersView
                ? context.UserSessionData
                : context.SessionData;
            view = sessionData.Get("View")?.Deserialize<View>()
                // ビューの保存種別で保存しない場合には、ViewSelectorがalways-sendで送信される
                ?? ss.Views
                    ?.Where(o => o.Accessable(context: context))
                    .FirstOrDefault(o => o.Id == context.Forms.Int("ViewSelector"))
                ?? ss.Views
                    ?.Where(o => o.Accessable(context: context))
                    .FirstOrDefault(o => o.Id == ss.GridView)
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
                view = ss.Views.Get(ss.LinkPageSize);
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