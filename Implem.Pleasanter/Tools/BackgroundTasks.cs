using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using System;
using System.Threading;
namespace Implem.Pleasanter.Tools
{
    public static class BackgroundTasks
    {
        public static DateTime LatestTime;

        public static string Do(Context context)
        {
            var now = DateTime.Now;
            while ((DateTime.Now - now).Seconds <= Parameters.BackgroundTask.BackgroundTaskSpan)
            {
                SysLogUtilities.Maintain(context: context);
                SearchIndexUtilities.Maintain(context: context);
                SearchIndexUtilities.RebuildSearchIndexes(context: context);
                Thread.Sleep(Parameters.BackgroundTask.Interval);
                LatestTime = DateTime.Now;
            }
            return new ResponseCollection().ToJson();
        }
    }
}
