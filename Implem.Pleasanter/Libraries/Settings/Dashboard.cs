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

        public Dashboard GetRecordingData()
        {
            var dashboard = new Dashboard();
            dashboard.Id = Id;
            dashboard.Title = Title;
            dashboard.Type = Type;
            dashboard.Width = Width;
            dashboard.Height = Height;
            dashboard.X = X;
            dashboard.Y = Y;
            dashboard.Sites = Sites;
            return dashboard;
        }

        public Dashboard Update(string title, DashboardType type, int x, int y, int width, int height, string sites)
        {
            Title = title;
            Type = type;
            Width = width;
            Height = height;
            X = x;
            Y = y;
            Sites = sites
                .Split(",")
                .Select(o => o.Trim())
                .Where(o => !o.IsNullOrEmpty())
                .ToList();
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
