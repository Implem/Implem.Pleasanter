using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Models;
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
        public List<string> Sites { get; set; }
        public long SiteId { get; set; }
        public View View { get; set; }

        public Dashboard GetRecordingData(Context context)
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
            dashboard.SiteId = SiteId;
            if(dashboard.Type == DashboardType.TimeLine)
            {
                var siteModel = new SiteModel(
                    context: context,
                    siteId: SiteId);
                if(siteModel.AccessStatus == Databases.AccessStatuses.Selected)
                {
                    View = View.GetRecordingData(
                        context: context,
                        ss: siteModel.SiteSettings);
                }
            }
            dashboard.View = View;
            return dashboard;
        }

        public Dashboard Update(string title, DashboardType type, int x, int y, int width, int height, string sites, long siteId, View view)
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
            SiteId = siteId;
            View = view;
            return this;
        }

        public IList<long> GetDashboardSites(Context context)
        {
            return Sites?.SelectMany(site =>
                long.TryParse(site,out var siteId)
                    ? new[] {siteId}
                    : SiteInfo.Sites(context: context).Values
                        .Where(row => row.String("SiteName") == site
                            || row.String("SiteGroupName") == site)
                        .Select(row=>row.Long("SiteId")))
                .ToList()?? new List<long>();
        }

        public IList<long> GetDashboardTables(Context context,string referenceType)
        {
            var sites = GetDashboardSites(context: context)
                .Where(id => SiteInfo.Sites(context: context).ContainsKey(id))
                .Select(id => new KeyValuePair<long, System.Data.DataRow>(id, SiteInfo.Sites(context: context)[id]));
            return EnumerateDashboardTables(context: context, sites, referenceType)
                .Distinct()
                .ToList();
            
        }

        private static IEnumerable<long> EnumerateDashboardTables(Context context, IEnumerable<KeyValuePair<long, System.Data.DataRow>> sites, string referenceType)
        {
            foreach (var site in sites)
            {
                var refType = site.Value.String("ReferenceType");
                if (refType == referenceType)
                {
                    yield return site.Key;
                }
                else if(refType == "Sites")
                {
                    var children = SiteInfo.Sites(context: context)
                        .Where(o => o.Value.Long("ParentId") == site.Key);
                    var childrenIds = EnumerateDashboardTables(context: context, children, referenceType);
                    foreach(var id in childrenIds)
                    {
                        yield return id;
                    }
                }
            }
        }
    }
}
