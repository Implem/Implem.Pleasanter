namespace Implem.Pleasanter.Libraries.Migrators
{
    public static class Version1_002
    {
        public static void Migrate1_002(this Settings.SiteSettings siteSettings)
        {
            siteSettings.ColumnCollection.ForEach(column =>
            {
                column.GridFormat = column.GridDateTime;
                column.ControlFormat = column.ControlDateTime;
            });
            siteSettings.Migrated = true;
        }
    }
}