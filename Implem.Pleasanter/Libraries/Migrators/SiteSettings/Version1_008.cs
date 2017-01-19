using System.Linq;
namespace Implem.Pleasanter.Libraries.Migrators
{
    public static class Version1_008
    {
        public static void Migrate1_008(this Settings.SiteSettings ss)
        {
            ss.Notifications?
                .Select((o, i) => new { Notification = o, Index = i + 1 })
                .ForEach(data =>
                    data.Notification.Id = data.Index);
            ss.Migrated = true;
        }
    }
}