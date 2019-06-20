using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using Implem.Pleasanter.NetFramework.Libraries.Requests;
using Implem.Pleasanter.Tools;
using System;
using System.Threading;
namespace Implem.Pleasanter.NetFramework.Tools
{
    public class BackgroundTasks : IBackgroundTasks
    {
        ContextImplement _context { get; }
        public BackgroundTasks(ContextImplement context)
        {
            _context = context;
        }

        public static DateTime LatestTime;

        public string Do()
        {
            var now = DateTime.Now;
            while ((DateTime.Now - now).Seconds <= Parameters.BackgroundTask.BackgroundTaskSpan)
            {
                SysLogUtilities.Maintain(context: _context);
                SearchIndexUtilities.Maintain(context: _context);
                SearchIndexUtilities.RebuildSearchIndexes(context: _context);
                Thread.Sleep(Parameters.BackgroundTask.Interval);
                LatestTime = DateTime.Now;
            }
            return new ResponseCollection().ToJson();
        }
    }
}
