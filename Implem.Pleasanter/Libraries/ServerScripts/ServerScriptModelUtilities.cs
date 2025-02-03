using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Reflection.Metadata.Ecma335;
using System.Text;

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

        public bool InRange(object dt)
        {
            if (dt?.GetType().FullName != "System.DateTime")
            {
                return false;
            }
            var inRange = dt.ToDateTime().InRange();
            return inRange;
        }

        public string ConvertToBase64String(string s, string encoding = "utf-8")
        {
            var bytes = Encoding.GetEncoding(encoding).GetBytes(s);
            return Convert.ToBase64String(bytes);
        }

        public DateTime MinTime() => Parameters.General.MinTime.ToLocal(context: Context).Date.ToUniversal(context: Context);

        public DateTime MaxTime() => Parameters.General.MaxTime.ToLocal(context: Context).Date.ToUniversal(context: Context);

        public DateTime EmptyTime() => MinTime().AddDays(-1);
    }
}