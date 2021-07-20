using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptModelUtilities
    {
        private readonly Context Context;
        private readonly SiteSettings SiteSettings;

        public ServerScriptModelUtilities(Context context, SiteSettings ss)
        {
            Context = context;
            SiteSettings = ss;
        }

        public DateTime Today()
        {
            return DateTime.Now.ToLocal(context: Context).Date.ToUniversal(context: Context);
        }

        public bool InRange(DateTime dt)
        {
            return dt.InRange();
        }
    }
}