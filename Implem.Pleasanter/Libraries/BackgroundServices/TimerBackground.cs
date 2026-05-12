using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using Quartz;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    public class TimerBackground
    {
        public void Init()
        {
            if (Parameters.BackgroundService.TimerEnabled(
                deploymentEnvironment: Parameters.Service.DeploymentEnvironment))
            {
                Task.Run(async () =>
                {
                    await AddTimer(timer: SyncByLdapExecutionTimer.GetParam());
                    await AddTimer(timer: DeleteSysLogsTimer.GetParam());
                    await AddTimer(timer: DeleteTemporaryFilesTimer.GetParam());
                    await AddTimer(timer: DeleteTrashBoxTimer.GetParam());
                    await AddTimer(timer: ReminderBackgroundTimer.GetParam());
                    await AddTimer(timer: DeleteUnusedRecordTimer.GetParam());
                    await AddTimer(timer: DeleteMcpLogsTimer.GetParam());
                });
            }
        }

        private async Task AddTimer(IExecutionTimerBaseParam timer)
        {
            var context = new Context(
                request: false,
                sessionStatus: false,
                sessionData: false,
                user: false,
                item: false)
            {
                Controller = timer.JobName
            };
            try
            {
                var scheduler = CustomQuartzHostedService.Scheduler;
                if (!timer.Enabled)
                {
                    var deleted = await scheduler.DeleteJob(timer.JobKey);
                    if (deleted)
                    {
                        _ = new SysLogModel(
                            context: context,
                            method: "ExecuteAsync",
                            message: $"{timer.JobName} disabled. Deleted persisted job.",
                            sysLogType: SysLogModel.SysLogTypes.Info);
                    }
                    return;
                }
                _ = new SysLogModel(
                    context: context,
                    method: "ExecuteAsync",
                    message: $"{timer.JobName} ExecuteAsync() Started",
                    sysLogType: SysLogModel.SysLogTypes.Info);
                var job = JobBuilder.Create(timer.JobType)
                    .WithIdentity(timer.JobKey)
                    .StoreDurably()
                    .Build();
                await scheduler.AddJob(job, true);
                if (timer.TimeList != null)
                {
                    var expectedKeys = new List<TriggerKey>();
                    var timeZone = TimeZoneInfo.FindSystemTimeZoneById(
                        !Parameters.Service.TimeZoneDefault.IsNullOrEmpty()
                            ? Parameters.Service.TimeZoneDefault
                            : TimeZoneInfo.Utc.Id);
                    foreach (var hhmm in timer.TimeList
                        .Where(hhmm => !hhmm.IsNullOrEmpty())
                        .Distinct())
                    {
                        if (DateTime.TryParse($"2020-01-01T{hhmm}:00.00", out var date))
                        {
                            var triggerKey = TimerTriggerRegistrar.CronTriggerKey(
                                jobKey: timer.JobKey,
                                hhmm: hhmm);
                            expectedKeys.Add(triggerKey);
                            var trigger = TriggerBuilder.Create()
                                .WithIdentity(triggerKey)
                                .ForJob(timer.JobKey)
                                .WithCronSchedule(
                                    $"0 {date.Minute} {date.Hour} * * ? *",
                                    (s) => s.InTimeZone(timeZone))
                                .Build();
                            await TimerTriggerRegistrar.EnsureTriggerAsync(
                                scheduler: scheduler,
                                trigger: trigger);
                        }
                    }
                    await TimerTriggerRegistrar.CleanupUnexpectedTriggersAsync(
                        scheduler: scheduler,
                        jobKey: timer.JobKey,
                        expectedKeys: expectedKeys);
                }
                else
                {
                    await timer.SetCustomTimer(scheduler: scheduler);
                }
            }
            catch (Exception e)
            {
                _ = new SysLogModel(
                    context: context,
                    e: e,
                    extendedErrorMessage: $"{timer.JobName} ExecuteAsync() Failed to start");
            }
        }
    }
}
