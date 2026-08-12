using Implem.DefinitionAccessor;
using Implem.Pleasanter.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    public class DeleteBackgroundJobsTimer : ClusterExecutionTimerBase
    {
        public class Param : IExecutionTimerBaseParam
        {
            private static readonly JobKey s_jobKey
                = new JobKey("DeleteBackgroundJobsTimer", "ExecutionTimerBase");
            public Type JobType => typeof(DeleteBackgroundJobsTimer);
            public IEnumerable<string> TimeList
                => Parameters.BackgroundService.DeleteBackgroundJobsTime
                    ?? new List<string>();
            public bool Enabled => Parameters.BackgroundService.DeleteBackgroundJobs;
            public JobKey JobKey => s_jobKey;
            public string JobName => "DeleteBackgroundJobsService";
            public Task<bool> SetCustomTimer(IScheduler scheduler)
                => Task.FromResult(false);
        }

        public override async Task Execute(IJobExecutionContext quartzContext)
        {
            await Task.Run(() =>
            {
                var context = CreateContext();
                var log = CreateSysLogModel(
                    context: context,
                    message: "Delete BackgroundJobs.");
                BackgroundJobQueue.DeleteCompletedJobs(context: context);
                log.Finish(context: context);
            }, quartzContext.CancellationToken);
        }

        internal static IExecutionTimerBaseParam GetParam()
        {
            return new Param();
        }
    }
}
