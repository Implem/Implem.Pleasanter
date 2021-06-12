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

        public ServerScriptModelNorificationModel New()
        {
            var norification = new ServerScriptModelNorificationModel(
                context: Context,
                ss: SiteSettings);
            return norification;
        }

        public ServerScriptModelNorificationModel Get(int Id)
        {
            var norification = new ServerScriptModelNorificationModel(
                context: Context,
                ss: SiteSettings,
                Id: Id);
            return norification;
        }
    }
}