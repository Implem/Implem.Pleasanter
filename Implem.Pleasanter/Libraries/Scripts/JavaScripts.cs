namespace Implem.Pleasanter.Libraries.Scripts
{
    public static class JavaScripts
    {
        public static string ViewMode(string viewMode)
        {
            switch (viewMode)
            {
                case "burndown": return "$p.drawBurnDown();";
                case "gantt": return "$p.drawGantt();";
                case "timeseries": return "$p.drawTimeSeries();";
                case "kamban": return "$p.setKamban();";
            }
            return string.Empty;
        }
    }
}