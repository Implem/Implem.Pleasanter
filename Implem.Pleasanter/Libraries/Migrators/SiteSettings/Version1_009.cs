namespace Implem.Pleasanter.Libraries.Migrators
{
    public static class Version1_009
    {
        public static void Migrate1_009(this Settings.SiteSettings ss)
        {
            ss.Views?.ForEach(data =>
            {
                data.KambanGroupByX = data.KambanGroupBy;
                data.KambanGroupBy = null;
            });
            ss.Migrated = true;
        }
    }
}