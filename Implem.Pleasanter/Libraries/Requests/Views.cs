using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
namespace Implem.Pleasanter.Libraries.Requests
{
    public static class Views
    {
        public static View GetBySession(Context context, SiteSettings ss)
        {
            var view = !Request.IsAjax()
                ? context.QueryStrings.Data("View")?.Deserialize<View>()
                : null;
            var key = "View";
            if (view != null)
            {
                SessionUtilities.Set(
                    context: context,
                    key: key,
                    view: view);
                return view;
            }
            if (context.Forms.ControlId() == "ViewSelector")
            {
                view = ss.Views?.Get(context.Forms.Int("ViewSelector"))
                    ?? new View(context: context, ss: ss);
                SessionUtilities.Set(
                    context: context,
                    key: key,
                    view: view);
                return view;
            }
            view = context.SessionData.Get("View")?.Deserialize<View>()
                ?? ss.Views?.Get(ss.GridView)
                ?? new View(
                    context: context,
                    ss: ss);
            view.SetByForm(context: context, ss: ss);
            SessionUtilities.Set(
                context: context,
                key: key,
                view: view);
            return view;
        }
    }
}