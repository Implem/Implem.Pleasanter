using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
using System.Linq;

namespace Implem.Pleasanter.Libraries.Settings
{
    public class QuickAccessSite
    {
        public string Id { get; set; }
        public string Icon { get; set; }
        public string Css { get; set; }
    }
    public class QuickAccessSiteModel
    {
        public SiteModel Model { get; set; }
        public string Icon { get; set; }
        public string Css { get; set; }
    }

    public class DashboardPart : ISettingListItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool? ShowTitle { get; set; }
        public bool DisableAsynchronousLoading { get; set; }
        public DashboardPartType Type { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string QuickAccessSites { get; set; }
        public List<QuickAccessSite> QuickAccessSitesData { get; set; }
        public QuickAccessLayout? QuickAccessLayout { get; set; }
        public string TimeLineSites { get; set; }
        public List<string> TimeLineSitesData { get; set; }
        public View View { get; set; }
        public string TimeLineTitle { get; set; }
        public string TimeLineBody { get; set; }
        public int TimeLineItemCount { get; set; }
        public string Content { get; set; }
        public string HtmlContent { get; set; }
        public TimeLineDisplayType? TimeLineDisplayType { get; set; }
        public SiteSettings.CalendarTypes? CalendarType { get; set; }
        public string CalendarSites { get; set; }
        public List<string> CalendarSitesData { get; set; }
        public string CalendarGroupBy { get; set; }
        public string CalendarTimePeriod { get; set; }
        public string CalendarFromTo { get; set; }
        public bool CalendarShowStatus { get; set; }
        public string IndexSites { get; set; }
        public List<string> IndexSitesData { get; set; }
        public string KambanSites { get; set; }
        public List<string> KambanSitesData { get; set; }
        public string KambanGroupByX { get; set; }
        public string KambanGroupByY { get; set; }
        public string KambanAggregateType { get; set; }
        public string KambanValue { get; set; }
        public string KambanColumns { get; set; }
        public bool KambanAggregationView { get; set; }
        public bool KambanShowStatus { get; set; }


        public long SiteId { get; set; }
        public string ExtendedCss { get; set; }
        public List<int> Depts { get; set; }
        public List<int> Groups { get; set; }
        public List<int> Users { get; set; }
        private static readonly int initialWidth = 2;
        private static readonly int initialHeight = 10;

        public DashboardPart()
        {
            TimeLineDisplayType = Settings.TimeLineDisplayType.Standard;
        }

        public DashboardPart GetRecordingData(Context context)
        {
            var dashboardPart = new DashboardPart();
            var ss = SiteSettingsUtilities.Get(
                context: context,
                siteId: SiteId);
            dashboardPart.Id = Id;
            dashboardPart.Title = Title;
            if (ShowTitle == true) dashboardPart.ShowTitle = true;
            dashboardPart.Type = Type;
            dashboardPart.X = X;
            dashboardPart.Y = Y;
            dashboardPart.Width = Width;
            dashboardPart.Height = Height;
            dashboardPart.ExtendedCss = ExtendedCss;
            if (DisableAsynchronousLoading == true) dashboardPart.DisableAsynchronousLoading = true;
            if (Depts?.Any() == true)
            {
                dashboardPart.Depts = Depts;
            }
            if (Groups?.Any() == true)
            {
                dashboardPart.Groups = Groups;
            }
            if (Users?.Any() == true)
            {
                dashboardPart.Users = Users;
            }
            dashboardPart.TimeLineDisplayType = null;
            switch (Type)
            {
                case DashboardPartType.QuickAccess:
                    dashboardPart.QuickAccessSites = QuickAccessSites;
                    if (QuickAccessLayout != Settings.QuickAccessLayout.Horizontal)
                    {
                        dashboardPart.QuickAccessLayout = QuickAccessLayout;
                    }
                    break;
                case DashboardPartType.TimeLine:
                    dashboardPart.TimeLineSites = TimeLineSites;
                    dashboardPart.TimeLineTitle = TimeLineTitle;
                    dashboardPart.TimeLineBody = TimeLineBody;
                    dashboardPart.TimeLineItemCount = TimeLineItemCount;
                    dashboardPart.SiteId = SiteId;
                    if (ss != null)
                    {
                        View = View.GetRecordingData(
                            context: context,
                            ss: ss);
                    }
                    dashboardPart.View = View;
                    dashboardPart.TimeLineDisplayType = (TimeLineDisplayType != Settings.TimeLineDisplayType.Standard)
                        ? TimeLineDisplayType
                        : null;
                    break;
                case DashboardPartType.Custom:
                    dashboardPart.Content = Content;
                    break;
                case DashboardPartType.CustomHtml:
                    dashboardPart.HtmlContent = HtmlContent;
                    break;
                case DashboardPartType.Calendar:
                    dashboardPart.CalendarSites = CalendarSites;
                    dashboardPart.CalendarSitesData = CalendarSitesData;
                    dashboardPart.CalendarType = CalendarType;
                    dashboardPart.CalendarGroupBy = CalendarGroupBy;
                    dashboardPart.CalendarTimePeriod = CalendarTimePeriod;
                    dashboardPart.CalendarFromTo = CalendarFromTo;
                    dashboardPart.SiteId = SiteId;
                    if (ss != null)
                    {
                        View = View.GetRecordingData(
                            context: context,
                            ss: ss);
                    }
                    dashboardPart.View = View;
                    if (CalendarShowStatus == true) dashboardPart.CalendarShowStatus = true;
                    break;
                case DashboardPartType.Kamban:
                    dashboardPart.KambanSites = KambanSites;
                    dashboardPart.KambanSitesData = KambanSitesData;
                    dashboardPart.KambanGroupByX = KambanGroupByX;
                    dashboardPart.KambanGroupByY = KambanGroupByY;
                    dashboardPart.KambanAggregateType = KambanAggregateType;
                    dashboardPart.KambanValue = KambanValue;
                    dashboardPart.KambanColumns = KambanColumns;
                    dashboardPart.KambanAggregationView = KambanAggregationView;
                    dashboardPart.KambanShowStatus = KambanShowStatus;
                    dashboardPart.SiteId = SiteId;
                    if (ss != null)
                    {
                        View = View.GetRecordingData(
                            context: context,
                            ss: ss);
                    }
                    dashboardPart.View = View;
                    break;
                case DashboardPartType.Index:
                    dashboardPart.IndexSites = IndexSites;
                    dashboardPart.IndexSitesData = IndexSitesData;
                    dashboardPart.SiteId = SiteId;
                    if (ss != null)
                    {
                        View = View.GetRecordingData(
                            context: context,
                            ss: ss);
                    }
                    dashboardPart.View = View;
                    break;
            }
            return dashboardPart;
        }

        public static DashboardPart Create(
            Context context,
            int id,
            string title,
            bool showTitle,
            DashboardPartType type,
            string quickAccessSites,
            QuickAccessLayout quickAccessLayout,
            string timeLineSites,
            string timeLineTitle,
            string timeLineBody,
            int timeLineItemCount,
            string content,
            string htmlContent,
            TimeLineDisplayType timeLineDisplayType,
            SiteSettings.CalendarTypes calendarType,
            string calendarSites,
            List<string> calendarSitesData,
            string calendarGroupBy,
            string calendarTimePeriod,
            string calendarFromTo,
            bool calendarShowStatus,
            string indexSites,
            List<string> indexSitesData,
            string kambanSites,
            List<string> kambanSitesData,
            string kambanGroupByX,
            string kambanGroupByY,
            string kambanAggregateType,
            string kambanValue,
            string kambanColumns,
            bool kambanAggregationView,
            bool kambanShowStatus,
            string extendedCss,
            bool disableAsynchronousLoading,
            List<Permission> permissions)
        {
            var dashboardPart = new DashboardPart() { Id = id };
            return dashboardPart.Update(
                context: context,
                title: title,
                showTitle: showTitle,
                type: type,
                x: 0,
                y: 0,
                width: initialWidth,
                height: initialHeight,
                quickAccessSites: quickAccessSites,
                quickAccessLayout: quickAccessLayout,
                timeLineSites: timeLineSites,
                timeLineTitle: timeLineTitle,
                timeLineBody: timeLineBody,
                timeLineItemCount: timeLineItemCount,
                content: content,
                htmlContent: htmlContent,
                timeLineDisplayType: timeLineDisplayType,
                calendarType: calendarType,
                calendarSites: calendarSites,
                calendarSitesData: calendarSitesData,
                calendarGroupBy: calendarGroupBy,
                calendarTimePeriod: calendarTimePeriod,
                calendarFromTo: calendarFromTo,
                calendarShowStatus: calendarShowStatus,
                indexSites: indexSites,
                indexSitesData: indexSitesData,
                kambanSites : kambanSites,
                kambanSitesData: kambanSitesData,
                kambanGroupByX : kambanGroupByX,
                kambanGroupByY : kambanGroupByY,
                kambanAggregateType : kambanAggregateType,
                kambanValue: kambanValue,
                kambanColumns: kambanColumns,
                kambanAggregationView: kambanAggregationView,
                kambanShowStatus: kambanShowStatus,
                extendedCss: extendedCss,
                disableAsynchronousLoading: disableAsynchronousLoading,
                permissions: permissions);
        }

        public DashboardPart Update(
            Context context,
            string title,
            bool showTitle,
            DashboardPartType type,
            int x,
            int y,
            int width,
            int height,
            string quickAccessSites,
            QuickAccessLayout quickAccessLayout,
            string timeLineSites,
            string timeLineTitle,
            string timeLineBody,
            int timeLineItemCount,
            string content,
            string htmlContent,
            TimeLineDisplayType timeLineDisplayType,
            SiteSettings.CalendarTypes calendarType,
            string calendarSites,
            List<string> calendarSitesData,
            string calendarGroupBy,
            string calendarTimePeriod,
            string calendarFromTo,
            bool calendarShowStatus,
            string indexSites,
            List<string> indexSitesData,
            string kambanSites,
            List<string> kambanSitesData,
            string kambanGroupByX,
            string kambanGroupByY,
            string kambanAggregateType,
            string kambanValue,
            string kambanColumns,
            bool kambanAggregationView,
            bool kambanShowStatus,
            string extendedCss,
            bool disableAsynchronousLoading,
            List<Permission> permissions)
        {
            Title = title;
            ShowTitle = showTitle;
            Type = type;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            QuickAccessSites = quickAccessSites;
            QuickAccessLayout = quickAccessLayout;
            TimeLineSites = timeLineSites;
            TimeLineTitle = timeLineTitle;
            TimeLineBody = timeLineBody;
            TimeLineItemCount = timeLineItemCount;
            Content = content;
            HtmlContent = htmlContent;
            TimeLineDisplayType = timeLineDisplayType;
            CalendarType = calendarType;
            CalendarSites = calendarSites;
            CalendarSitesData = calendarSitesData;
            CalendarGroupBy = calendarGroupBy;
            CalendarTimePeriod = calendarTimePeriod;
            CalendarFromTo = calendarFromTo;
            CalendarShowStatus = calendarShowStatus;
            IndexSites = indexSites;
            IndexSitesData = indexSitesData;
            KambanSites = kambanSites;
            KambanSitesData = kambanSitesData;
            KambanGroupByX = kambanGroupByX;
            KambanGroupByY = kambanGroupByY;
            KambanAggregateType = kambanAggregateType;
            KambanValue = kambanValue;
            KambanColumns = kambanColumns;
            KambanAggregationView = kambanAggregationView;
            KambanShowStatus = kambanShowStatus;
            ExtendedCss = extendedCss;
            DisableAsynchronousLoading = disableAsynchronousLoading;
            SetSitesData();
            SetPermissions(permissions);
            SetBaseSiteData(context: context);

            return this;
        }

        public void SetSitesData()
        {
            SetQuickAccessSitesData();
            SetTimeLineSitesData();
            SetCalendarSitesData();
            SetIndexSitesData();
            SetKambanSitesData();
        }

        private void SetBaseSiteData(Context context)
        { 
            var currentSs = GetBaseSiteSettings(
                context: context,
                sites: GetSiteTypeData());
            SiteId = currentSs?.SiteId ?? 0;
            View = SiteModel.GetDashboardPartView(
                context: context,
                ss: currentSs,
                view: View);
        }

        private List<string> GetSiteTypeData()
        {
            switch (Type)
            {
                case DashboardPartType.TimeLine:
                    return TimeLineSitesData;
                case DashboardPartType.Calendar:
                    return CalendarSitesData;
                case DashboardPartType.Index:
                    return IndexSitesData;
                case DashboardPartType.Kamban:
                    return KambanSitesData;
                default:
                    return null;
            }
        }

        private void SetQuickAccessSitesData()
        {
            if (QuickAccessSites == null)
            {
                QuickAccessSitesData = new List<QuickAccessSite>();
                return;
            }
            QuickAccessSitesData = QuickAccessSites.Deserialize<List<QuickAccessSite>>()
                ?? QuickAccessSites
                    ?.Split(new[] { ",", "\n", "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries)
                    .Select(o => o.Trim())
                    .Where(o => !o.IsNullOrEmpty())
                    .Select(o => new QuickAccessSite()
                    {
                        Id = o,
                        Icon = null,
                        Css = null
                    })
                    .ToList();
        }
                
        private void SetTimeLineSitesData()
        {
            if(TimeLineSites == null)
            {
                TimeLineSitesData = new List<string>();
                return;
            }
            TimeLineSitesData = TimeLineSites
                .Split(",")
                .Select(o => o.Trim())
                .Where(o => !o.IsNullOrEmpty())
                .ToList();
        }

        private void SetCalendarSitesData()
        {
            if (CalendarSites == null)
            {
                CalendarSitesData = new List<string>();
                return;
            }
            CalendarSitesData = CalendarSites
                .Split(",")
                .Select(o => o.Trim())
                .Where(o => !o.IsNullOrEmpty())
                .ToList();
        }

        private void SetIndexSitesData()
        {
            if (IndexSites == null)
            {
                IndexSitesData = new List<string>();
                return;
            }
            IndexSitesData = IndexSites
                .Split(",")
                .Select(o => o.Trim())
                .Where(o => !o.IsNullOrEmpty())
                .ToList();
        }
        private void SetKambanSitesData()
        {
            if (KambanSites == null)
            {
                KambanSitesData = new List<string>();
                return;
            }
            KambanSitesData = KambanSites
                .Split(",")
                .Select(o => o.Trim())
                .Where(o => !o.IsNullOrEmpty())
                .ToList();
        }

        private static SiteSettings GetBaseSiteSettings(Context context, List<string> sites)
        {
            return GetDashboardPartTables(context: context, sites: sites)
                .Select(id => SiteSettingsUtilities.Get(context: context, siteId: id))
                .FirstOrDefault(ss => ss != null);
        }

        public static SiteSettings GetBaseSiteSettings(Context context, string sitesString)
        {
            if (sitesString.IsNullOrEmpty())
            {
                return null;
            }
            var sites = sitesString
                .Split(",")
                .Select(o => o.Trim())
                .Where(o => !o.IsNullOrEmpty())
                .ToList();
            return GetDashboardPartTables(context: context, sites: sites)
                .Select(id => SiteSettingsUtilities.Get(context: context, siteId: id))
                .FirstOrDefault(ss => ss != null);
        }

        private static IList<long> GetDashboardPartSites(Context context, IEnumerable<string> sites)
        {
            return sites?.SelectMany(site =>
                long.TryParse(site, out var siteId)
                    ? new[] { siteId }
                    : SiteInfo.Sites(context: context).Values
                        .Where(row => row.String("SiteName") == site
                            || row.String("SiteGroupName") == site)
                        .Select(row => row.Long("SiteId")))
                .ToList() ?? new List<long>();
        }

        public static IList<(long Id, string Icon, string Css)> GetQuickAccessSites(Context context, IEnumerable<QuickAccessSite> sites)
        {
            return sites?.SelectMany(site =>
                long.TryParse(site.Id, out var siteId)
                    ? new[] { (siteId, site.Icon, site.Css) }
                    : SiteInfo.Sites(context: context).Values
                        .Where(row => row.String("SiteName") == site.Id
                            || row.String("SiteGroupName") == site.Id)
                        .Select(row => (row.Long("SiteId"), site.Icon, site.Css)))
                .ToList()
                    ?? new List<(long, string, string)>();
        }

        public static IEnumerable<long> GetDashboardPartTables(Context context, List<string> sites)
        {
            var keyValues = GetDashboardPartSites(context: context, sites: sites)
                .Where(id => SiteInfo.Sites(context: context).ContainsKey(id))
                .Select(id => new KeyValuePair<long, System.Data.DataRow>(id, SiteInfo.Sites(context: context)[id]));
            return EnumerateDashboardPartTables(sites: keyValues)
                .Distinct();
        }

        private static IEnumerable<long> EnumerateDashboardPartTables(IEnumerable<KeyValuePair<long, System.Data.DataRow>> sites)
        {
            foreach (var site in sites)
            {
                var refType = site.Value.String("ReferenceType");
                if (refType == "Results" || refType == "Issues")
                {
                    yield return site.Key;
                }
            }
        }

        private void SetPermissions(List<Permission> permissions)
        {
            Depts?.Clear();
            Groups?.Clear();
            Users?.Clear();
            foreach (var permission in permissions)
            {
                switch (permission.Name)
                {
                    case "Dept":
                        if (Depts == null)
                        {
                            Depts = new List<int>();
                        }
                        if (!Depts.Contains(permission.Id))
                        {
                            Depts.Add(permission.Id);
                        }
                        break;
                    case "Group":
                        if (Groups == null)
                        {
                            Groups = new List<int>();
                        }
                        if (!Groups.Contains(permission.Id))
                        {
                            Groups.Add(permission.Id);
                        }
                        break;
                    case "User":
                        if (Users == null)
                        {
                            Users = new List<int>();
                        }
                        if (!Users.Contains(permission.Id))
                        {
                            Users.Add(permission.Id);
                        }
                        break;
                }
            }
        }

        public List<Permission> GetPermissions(SiteSettings ss)
        {
            var permissions = new List<Permission>();
            Depts?.ForEach(deptId => permissions.Add(new Permission(
                ss: ss,
                name: "Dept",
                id: deptId)));
            Groups?.ForEach(groupId => permissions.Add(new Permission(
                ss: ss,
                name: "Group",
                id: groupId)));
            Users?.ForEach(userId => permissions.Add(new Permission(
                ss: ss,
                name: "User",
                id: userId)));
            return permissions;
        }

        public bool Accessable(Context context, SiteSettings ss)
        {
            if (!context.CanRead(ss: ss))
            {
                return false;
            }
            if (context.HasPrivilege)
            {
                return true;
            }
            if (Depts?.Any() != true
                && Groups?.Any() != true
                && Users?.Any() != true)
            {
                return true;
            }
            if (Depts?.Contains(context.DeptId) == true)
            {
                return true;
            }
            if (Groups?.Any(groupId => context.Groups.Contains(groupId)) == true)
            {
                return true;
            }
            if (Users?.Contains(context.UserId) == true)
            {
                return true;
            }
            return false;
        }

        public string PartTypeString(Context context)
        {
            switch (Type)
            {
                case DashboardPartType.QuickAccess:
                    return Displays.QuickAccess(context: context);

                case DashboardPartType.TimeLine:
                    return Displays.TimeLine(context: context);

                case DashboardPartType.Custom:
                    return Displays.DashboardCustom(context: context);

                case DashboardPartType.CustomHtml:
                    return Displays.DashboardCustomHtml(context: context);
                case DashboardPartType.Calendar:
                    return Displays.Calendar(context: context);
                case DashboardPartType.Index:
                    return Displays.Index(context: context);
                case DashboardPartType.Kamban:
                    return Displays.Kamban(context: context);
                default:
                    return Displays.QuickAccess(context: context);
            }
        }

        public void SetQuickAccessSites()
        {
            if (QuickAccessSitesData == null)
            {
                QuickAccessSites = string.Empty;
            }
            else if (QuickAccessSitesData.Any(o => o.Icon != null || o.Css != null))
            {
                QuickAccessSites = QuickAccessSitesData.ToJson(formatting: Newtonsoft.Json.Formatting.Indented);
            }
            else
            {
                QuickAccessSites = QuickAccessSitesData.Select(o => o.Id).Join("\n");
            }
        }

        public void SetTimeLineSites()
        {
            if (TimeLineSitesData == null)
            {
                TimeLineSites = string.Empty;
            }
            else
            {
                TimeLineSites = TimeLineSitesData.Join(",");
            }
        }

        public void SetCalendarSites()
        {
            if (CalendarSitesData == null)
            {
                CalendarSites = string.Empty;
            }
            else
            {
                CalendarSites = CalendarSitesData.Join(",");
            }
        }

        public void SetIndexSites()
        {
            if (IndexSitesData == null)
            {
                IndexSites = string.Empty;
            }
            else
            {
                IndexSites = IndexSitesData.Join(",");
            }
        }
        public void SetKambanSites()
        {
            if (KambanSitesData == null)
            {
                KambanSites = string.Empty;
            }
            else
            {
                KambanSites = KambanSitesData.Join(",");
            }
        }
    }
}
