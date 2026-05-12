using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Implem.DefinitionAccessor;
using Implem.Pleasanter.Models;
using Quartz;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    class ReminderBackgroundTimer : ClusterExecutionTimerBase
    {
        static private bool IsRunning = false;
        public class Param : IExecutionTimerBaseParam
        {
            public static readonly JobKey jobKey = new JobKey("ReminderBackgroundTimer", "ExecutionTimerBase");
            public Type JobType => typeof(ReminderBackgroundTimer);
            public IEnumerable<string> TimeList => null;
            public bool Enabled => Parameters.BackgroundService.Reminder;
            public JobKey JobKey => jobKey;
            public string JobName => "ReminderBackgroundService";
            public async Task<bool> SetCustomTimer(IScheduler scheduler)
            {
                var triggerKey = TimerTriggerRegistrar.SimpleTriggerKey(JobKey);
                var trigger = TriggerBuilder.Create()
                    .WithIdentity(triggerKey)
                    .ForJob(JobKey)
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(60)
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

        public override async Task Execute(IJobExecutionContext context)
        {
            if (IsRunning) return;
            await Task.Run(() =>
            {
                if (IsRunning) return;
                var context = CreateContext();
                try
                {
                    IsRunning = true;
                    ReminderScheduleUtilities.Remind(context: context);
                }
                catch (OperationCanceledException e)
                {
                    _ = new SysLogModel(
                        context: context,
                        e: e,
                        extendedErrorMessage: "ReminderBackgroundService Canceled");
                }
                catch (Exception e)
                {
                    _ = new SysLogModel(
                        context: context,
                        e: e,
                        extendedErrorMessage: "ReminderBackgroundService Exception");
                }
                finally
                {
                    IsRunning = false;
                }
            }, context.CancellationToken);
        }

        internal static IExecutionTimerBaseParam GetParam()
        {
            return new Param();
        }
    }
}
