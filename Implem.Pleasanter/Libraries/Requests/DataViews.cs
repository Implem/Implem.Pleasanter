using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System.Linq;
using System.Web;
namespace Implem.Pleasanter.Libraries.Requests
{
    public static class DataViews
    {
        public static DataView GetBySession(SiteSettings ss)
        {
            var key = "DataView_" + (ss.SiteId == 0
                ? Pages.Key()
                : ss.SiteId.ToString());
            if (Forms.ControlId() == "DataViewSelector")
            {
                var dataView = ss.DataViews.FirstOrDefault(o =>
                    o.Id == Forms.Int("DataViewSelector")) ?? new DataView(ss);
                HttpContext.Current.Session[key] = dataView;
                return dataView;
            }
            else if (HttpContext.Current.Session[key] != null)
            {
                var dataView = (HttpContext.Current.Session[key] as DataView);
                dataView.SetByForm(ss);
                return dataView;
            }
            else
            {
                var dataView = new DataView(ss);
                HttpContext.Current.Session[key] = dataView;
                return dataView;
            }
        }
    }
}