using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using System;
using System.Threading;
namespace Implem.Pleasanter.Tools
{
    public static class BackgroundTasks
    {
        public static DateTime LatestTime;

        public static string Do()
        {
            var now = DateTime.Now;
            while ((DateTime.Now - now).Seconds <= Parameters.General.BackgroundTaskSpan)
            {
                SysLogsUtility.Maintenance();
                ItemsUtility.Maintenance();
                Thread.Sleep(Parameters.BackgroundTask.Interval);
                LatestTime = DateTime.Now;
            }
            return new ResponseCollection().ToJson();
        }

        public static bool Disabled()
        {
            return
                (DateTime.Now - BackgroundTasks.LatestTime).Milliseconds >=
                Parameters.BackgroundTask.DisableInterval;
        }
    }
}
