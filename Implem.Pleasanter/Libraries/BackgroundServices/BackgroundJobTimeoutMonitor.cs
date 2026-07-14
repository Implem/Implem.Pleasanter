using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using Quartz;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    public class BackgroundJobTimeoutMonitor : ClusterExecutionTimerBase
    {
        public class Param : IExecutionTimerBaseParam
        {
            private static readonly JobKey s_jobKey
                = new JobKey(
                    "BackgroundJobTimeoutMonitor",
                    "ExecutionTimerBase");

            public Type JobType => typeof(BackgroundJobTimeoutMonitor);
            public IEnumerable<string> TimeList => null;
            public bool Enabled => BackgroundJobQueue.BackgroundQueueEnabled()
                && (Parameters.BackgroundJobs?.BackgroundJobTimeout ?? 0) > 0;
            public JobKey JobKey => s_jobKey;
            public string JobName => "BackgroundJobTimeoutMonitorService";

            public async Task<bool> SetCustomTimer(IScheduler scheduler)
            {
                var triggerKey = TimerTriggerRegistrar.SimpleTriggerKey(JobKey);
                var trigger = TriggerBuilder.Create()
                    .WithIdentity(triggerKey)
                    .ForJob(JobKey)
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(
                            Parameters.BackgroundJobs?.BackgroundJobDispatcherInterval ?? 60)
                        .RepeatForever())
                    .Build();
                await TimerTriggerRegistrar.EnsureTriggerAsync(
                    scheduler: scheduler,
                    trigger: trigger);
                await TimerTriggerRegistrar.CleanupUnexpectedTriggersAsync(
                    scheduler: scheduler,
                    jobKey: JobKey,
                    expectedKeys: [triggerKey]);
                return true;
            }
        }

        public override async Task Execute(IJobExecutionContext quartzContext)
        {
            await Task.Run(
                () =>
                {
                    var context = CreateContext();
                    try
                    {
                        BackgroundJobQueue.WarnTimedOutRunningJobs(context: context);
                    }
                    catch (Exception e)
                    {
                        new SysLogModel(
                            context: new Context(
                                tenantId: context.TenantId,
                                request: false,
                                context: context)
                            {
                                Controller = nameof(BackgroundJobTimeoutMonitor),
                                Action = nameof(Execute)
                            },
                            e: e,
                            extendedErrorMessage: "BackgroundJobTimeoutMonitor Exception");
                    }
                },
                quartzContext.CancellationToken);
        }

        internal static IExecutionTimerBaseParam GetParam()
        {
            return new Param();
        }
    }
}
