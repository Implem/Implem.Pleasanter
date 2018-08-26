using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using System;
using System.Threading;
using System.Web;
namespace Implem.Pleasanter.Tools
{
    public static class BackgroundTasks
    {
        public static DateTime LatestTime;

        public static string Do(Context context)
        {
            var now = DateTime.Now;
            HealthUtilities.Maintain(context: context);
            while ((DateTime.Now - now).Seconds <= Parameters.BackgroundTask.BackgroundTaskSpan)
            {
                SysLogUtilities.Maintain(context: context);
                SearchIndexUtilities.Maintain(context: context);
                SearchIndexUtilities.CreateInBackground(context: context);
                Thread.Sleep(Parameters.BackgroundTask.Interval);
                LatestTime = DateTime.Now;
            }
            HttpContext.Current.Session.Abandon();
            return new ResponseCollection().ToJson();
        }
    }
}
