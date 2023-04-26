using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using System.Collections.Generic;
using System.Linq;

namespace Implem.Pleasanter.Libraries.Settings
{

    public class Dashboard : ISettingListItem 
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DashboardType Type { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public IList<string> Sites { get; set; }
        public View View { get; set; }

        public Dashboard GetRecordingData()
        {
            var dashboard = new Dashboard();
            dashboard.Id = Id;
            dashboard.Title = Title;
            dashboard.Type = Type;
            dashboard.X = X;
            dashboard.Y = Y;
            dashboard.Width = Width;
            dashboard.Height = Height;
            dashboard.Sites = Sites;
            dashboard.View = View;
            return dashboard;
        }

        public Dashboard Update(string title, DashboardType type, int x, int y, int width, int height, string sites, View view)
        {
            Title = title;
            Type = type;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Sites = sites
                .Split(",")
                .Select(o => o.Trim())
                .Where(o => !o.IsNullOrEmpty())
                .ToList();
            View = view;
            return this;
        }

        public IList<long> GetDashboardSites(Context context)
        {
            return Sites.SelectMany(site =>
                long.TryParse(site,out var siteId)
                    ? new[] {siteId}
                    : SiteInfo.Sites(context: context).Values
                        .Where(row => row.String("SiteName") == site
                            || row.String("SiteGroupName") == site)
                        .Select(row=>row.Long("SiteId")))
                .ToList();
        }
    }
}
