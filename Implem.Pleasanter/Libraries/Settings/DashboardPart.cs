using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
using System.Linq;

namespace Implem.Pleasanter.Libraries.Settings
{

    public class DashboardPart : ISettingListItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool? ShowTitle { get; set; }
        public DashboardPartType Type { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public List<string> QuickAccessSites { get; set; }
        public QuickAccessLayout QuickAccessLayout { get; set; }
        public List<string> TimeLineSites { get; set; }
        public View View { get; set; }
        public string TimeLineTitle { get; set; }
        public string TimeLineBody { get; set; }
        public string Content { get; set; }
        public TimeLineDisplayType TimeLineDisplayType { get; set; } = Settings.TimeLineDisplayType.Standard;
        public long SiteId { get; set; }

        private static readonly int initialWidth = 2;
        private static readonly int initialHeight = 10;

        public DashboardPart GetRecordingData(Context context)
        {
            var dashboardPart = new DashboardPart();
            dashboardPart.Id = Id;
            dashboardPart.Title = Title;
            dashboardPart.ShowTitle = ShowTitle != true
                ? null
                : ShowTitle;
            dashboardPart.Type = Type;
            dashboardPart.X = X;
            dashboardPart.Y = Y;
            dashboardPart.Width = Width;
            dashboardPart.Height = Height;
            dashboardPart.QuickAccessSites = QuickAccessSites;
            dashboardPart.QuickAccessLayout = QuickAccessLayout;
            dashboardPart.TimeLineSites = TimeLineSites;
            dashboardPart.TimeLineTitle = TimeLineTitle;
            dashboardPart.TimeLineBody = TimeLineBody;
            dashboardPart.Content = Content;
            dashboardPart.SiteId = SiteId;
            dashboardPart.TimeLineDisplayType = TimeLineDisplayType;
            var ss = SiteSettingsUtilities.Get(
                context: context,
                siteId: SiteId);
            if (ss != null)
            {
                View = View.GetRecordingData(
                    context: context,
                    ss: ss);
            }
            dashboardPart.View = View;
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
            string content,
            TimeLineDisplayType timeLineDisplayType)
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
                content: content,
                timeLineDisplayType: timeLineDisplayType);
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
            string content,
            TimeLineDisplayType timeLineDisplayType)
        {
            Title = title;
            ShowTitle = showTitle;
            Type = type;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            QuickAccessSites = quickAccessSites
                .Split(",")
                .Select(o => o.Trim())
                .Where(o => !o.IsNullOrEmpty())
                .ToList();
            QuickAccessLayout = quickAccessLayout;
            TimeLineSites = timeLineSites
                .Split(",")
                .Select(o => o.Trim())
                .Where(o => !o.IsNullOrEmpty())
                .ToList();
            TimeLineTitle = timeLineTitle;
            TimeLineBody = timeLineBody;
            Content = content;
            TimeLineDisplayType = timeLineDisplayType;
            var currentSs = GetBaseSiteSettings(
                context: context,
                timeLineSites: TimeLineSites);
            SiteId = currentSs?.SiteId ?? 0;
            View = SiteModel.GetDashboardPartView(
                context: context,
                ss: currentSs,
                view: View);
            return this;
        }

        private static SiteSettings GetBaseSiteSettings(Context context, List<string> timeLineSites)
        {
            return GetDashboardPartTables(context: context, sites: timeLineSites)
                .Select(id => SiteSettingsUtilities.Get(context: context, siteId: id))
                .FirstOrDefault(ss => ss != null);
        }

        public static SiteSettings GetBaseSiteSettings(Context context, string timeLineSitesString)
        {
            if (timeLineSitesString.IsNullOrEmpty())
            {
                return null;
            }
            var timeLineSites = timeLineSitesString
                .Split(",")
                .Select(o => o.Trim())
                .Where(o => !o.IsNullOrEmpty())
                .ToList();
            return GetDashboardPartTables(context: context, sites: timeLineSites)
                .Select(id => SiteSettingsUtilities.Get(context: context, siteId: id))
                .FirstOrDefault(ss => ss != null);
        }

        public static IList<long> GetDashboardPartSites(Context context, List<string> sites)
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
    }
}
