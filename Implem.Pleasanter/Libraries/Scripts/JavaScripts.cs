namespace Implem.Pleasanter.Libraries.Scripts
{
    public static class JavaScripts
    {
        public static string ViewMode(string viewMode)
        {
            switch (viewMode)
            {
                case "index": return "$p.paging('#Grid')";
                case "calendar": return "$p.setCalendar();";
                case "crosstab": return "$p.setCrosstab();";
                case "gantt": return "$p.drawGantt();";
                case "burndown": return "$p.drawBurnDown();";
                case "timeseries": return "$p.drawTimeSeries();";
                case "kamban": return "$p.setKamban();";
            }
            return string.Empty;
        }
    }
}