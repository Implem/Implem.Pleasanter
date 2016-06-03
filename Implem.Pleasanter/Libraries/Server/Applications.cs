using Implem.Libraries.Utilities;
using System;
using System.Web;
namespace Implem.Pleasanter.Libraries.Server
{
    public static class Applications
    {
        public static DateTime LogMaintenanceDate;

        public static double ApplicationAge()
        {
            return (DateTime.Now - HttpContext.Current.Application["StartTime"].ToDateTime())
                .TotalMilliseconds;
        }

        public static double ApplicationRequestInterval()
        {
            var ret = (DateTime.Now - HttpContext.Current.Application["LastAccessTime"].ToDateTime())
                .TotalMilliseconds;
            HttpContext.Current.Application["LastAccessTime"] = DateTime.Now;
            return ret;
        }
    }
}