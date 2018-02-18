using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Libraries.Migrators
{
    public static class Version1_013
    {
        public static void Migrate1_013(this SiteSettings ss)
        {
            ss.Views?.ForEach(view =>
            {
                view.CalendarFromTo = view.CalendarColumn;
                view.CalendarColumn = null;
            });
            ss.Migrated = true;
        }
    }
}