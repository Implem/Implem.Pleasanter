namespace Implem.Pleasanter.Libraries.Migrators
{
    public static class Version1_004
    {
        public static void Migrate1_004(this Settings.SiteSettings ss)
        {
            ss.GridColumns = ss.GridColumnsOrder;
            ss.FilterColumns = ss.FilterColumnsOrder;
            ss.EditorColumns = ss.EditorColumnsOrder;
            ss.TitleColumns = ss.TitleColumnsOrder;
            ss.LinkColumns = ss.LinkColumnsOrder;
            ss.HistoryColumns = ss.HistoryColumnsOrder;
            ss.GridColumnsOrder = null;
            ss.FilterColumnsOrder = null;
            ss.EditorColumnsOrder = null;
            ss.TitleColumnsOrder = null;
            ss.LinkColumnsOrder = null;
            ss.HistoryColumnsOrder = null;
            ss.Migrated = true;
        }
    }
}