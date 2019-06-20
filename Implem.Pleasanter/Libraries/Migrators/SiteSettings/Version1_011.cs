using System.Linq;
namespace Implem.Pleasanter.Libraries.Migrators
{
    public static class Version1_011
    {
        public static void Migrate1_011(this Settings.SiteSettings ss)
        {
            ss.ViewLatestId = ss.Views?.Max(o => o.Id) ?? 0;
            ss.Migrated = true;
        }
    }
}