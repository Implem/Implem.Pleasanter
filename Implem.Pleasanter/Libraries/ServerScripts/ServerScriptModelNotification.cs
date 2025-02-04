using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptModelNotification
    {
        private readonly Context Context;
        private readonly SiteSettings SiteSettings;

        public ServerScriptModelNotification(Context context, SiteSettings ss)
        {
            Context = context;
            SiteSettings = ss;
        }

        public ServerScriptModelNotificationModel New()
        {
            var notification = new ServerScriptModelNotificationModel(
                context: Context,
                ss: SiteSettings);
            return notification;
        }

        public ServerScriptModelNotificationModel Get(int id)
        {
            var notification = new ServerScriptModelNotificationModel(
                context: Context,
                ss: SiteSettings,
                id: id);
            return notification;
        }
    }
}