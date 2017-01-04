namespace Implem.Pleasanter.Libraries.Migrators
{
    public static class Version1_006
    {
        public static void Migrate1_006(this Settings.SiteSettings ss)
        {
            ss.ColumnCollection?.ForEach(column =>
            {
                column.EditorFormat = column.ControlFormat;
                column.ControlFormat = null;
            });
            ss.Migrated = true;
        }
    }
}