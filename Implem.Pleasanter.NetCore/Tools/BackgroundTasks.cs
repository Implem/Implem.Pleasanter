﻿using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using Implem.Pleasanter.NetCore.Libraries.Requests;
using Implem.Pleasanter.Tools;
using System;
using System.Threading;

namespace Implem.Pleasanter.NetCore.Tools
{
    public class BackgroundTasks : IBackgroundTasks
    {
        private ContextImplement Context;
        public static DateTime LatestTime;

        public BackgroundTasks(ContextImplement context)
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
            return new ResponseCollection().ToJson();
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
