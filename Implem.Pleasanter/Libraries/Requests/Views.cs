using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System.Web;
namespace Implem.Pleasanter.Libraries.Requests
{
    public static class Views
    {
        public static View GetBySession(Context context, SiteSettings ss)
        {
            var view = !Request.IsAjax()
                ? QueryStrings.Data("View")?.Deserialize<View>()
                : null;
            var key = "View" + (ss.SiteId == 0
                ? Pages.Key()
                : ss.SiteId.ToString());
            if (view != null)
            {
                HttpContext.Current.Session[key] = view;
                return view;
            }
            else if (Forms.ControlId() == "ViewSelector")
            {
                view = ss.Views?.Get(Forms.Int("ViewSelector"))
                    ?? new View(context: context, ss: ss);
                HttpContext.Current.Session[key] = view;
                return view;
            }
            else if (HttpContext.Current.Session[key] != null)
            {
                view = (HttpContext.Current.Session[key] as View);
                view.SetByForm(context: context, ss: ss);
                return view;
            }
            else
            {
                view = ss.Views?.Get(ss.GridView)
                    ?? new View(context: context, ss: ss);
                HttpContext.Current.Session[key] = view;
                return view;
            }
        }
    }
}