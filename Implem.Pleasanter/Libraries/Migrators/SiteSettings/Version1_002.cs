namespace Implem.Pleasanter.Libraries.Migrators
{
    public static class Version1_002
    {
        public static void Migrate1_002(this Settings.SiteSettings ss)
        {
            ss.ColumnCollection.ForEach(column =>
            {
                column.GridFormat = column.GridDateTime;
                column.EditorFormat = column.ControlDateTime;
            });
            ss.Migrated = true;
        }
    }
}