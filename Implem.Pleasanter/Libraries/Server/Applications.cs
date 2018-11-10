using System;
namespace Implem.Pleasanter.Libraries.Server
{
    public static class Applications
    {
        public static DateTime StartTime = DateTime.Now;
        public static DateTime LastAccessTime = DateTime.Now;
        public static DateTime SysLogsMaintenanceDate = DateTime.Now;
        public static DateTime SearchIndexesMaintenanceDate = DateTime.Now;

        public static double ApplicationAge()
        {
            return (DateTime.Now - StartTime).TotalMilliseconds;
        }

        public static double ApplicationRequestInterval()
        {
            var ret = (DateTime.Now - LastAccessTime).TotalMilliseconds;
            LastAccessTime = DateTime.Now;
            return ret;
        }
    }
}