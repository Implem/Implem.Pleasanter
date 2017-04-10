namespace Implem.Pleasanter.Libraries.Migrators
{
    public static class Version1_010
    {
        public static void Migrate1_010(this Settings.SiteSettings ss)
        {
            if (ss.EditorColumns?.Contains("Comments") == false)
            {
                ss.EditorColumns.Add("Comments");
            }
            ss.Migrated = true;
        }
    }
}