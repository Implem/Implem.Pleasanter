using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System.Linq;
using System.Web;
namespace Implem.Pleasanter.Libraries.Requests
{
    public static class Views
    {
        public static View GetBySession(SiteSettings ss)
        {
            var key = "View" + (ss.SiteId == 0
                ? Pages.Key()
                : ss.SiteId.ToString());
            if (Forms.ControlId() == "ViewSelector")
            {
                var view = ss.Views.FirstOrDefault(o =>
                    o.Id == Forms.Int("ViewSelector")) ?? new View(ss);
                HttpContext.Current.Session[key] = view;
                return view;
            }
            else if (HttpContext.Current.Session[key] != null)
            {
                var view = (HttpContext.Current.Session[key] as View);
                view.SetByForm(ss);
                return view;
            }
            else
            {
                var view = ss.Views?.FirstOrDefault(o => o.Id == ss.GridView) ?? new View(ss);
                HttpContext.Current.Session[key] = view;
                return view;
            }
        }
    }
}