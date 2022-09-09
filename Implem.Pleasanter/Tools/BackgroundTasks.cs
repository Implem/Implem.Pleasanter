using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using System;
using System.Threading;
namespace Implem.PleasanterTools
{
    public class BackgroundTasks
    {
        private Context Context;
        public static DateTime LatestTime;

        public BackgroundTasks(Context context)
        {
           Context = context;
        }

        public string Do()
        {
            var now = DateTime.Now;
            while ((DateTime.Now - now).Seconds <= Parameters.BackgroundTask.BackgroundTaskSpan)
            {
                SysLogUtilities.Maintain(context: Context);
                Thread.Sleep(Parameters.BackgroundTask.Interval);
                LatestTime = DateTime.Now;
            }
            return new ResponseCollection(context: Context).ToJson();
        }

        public string DeleteLog()
        {
            SysLogUtilities.Maintain(
                context: Context,
                force: true);
            return string.Empty;
        }
    }
}
