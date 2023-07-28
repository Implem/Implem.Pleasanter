using Implem.Pleasanter.Libraries.DataTypes;
using System.Security.Permissions;

namespace Implem.Pleasanter.Libraries.Settings
{
    public class DashboardTimeLineItem
    {
        public long Id { get; set; }
        public long SiteId { get; set; }
        public SiteTitle SiteTitle { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public Time CreatedTime { get; set; }
        public Time UpdatedTime { get; set; }
        public User Creator { get; set; }
        public User Updator { get; set; }
    }
}
