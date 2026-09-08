using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using Quartz;
using Quartz.Impl.Matchers;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    public class TimerBackground
    {
        public async Task InitAsync()
        {
            BackgroundJobTargetTenants.Evaluate();
            if (Parameters.BackgroundService.TimerEnabled(
                deploymentEnvironment: Parameters.Service.DeploymentEnvironment,
                backgroundQueueEnabled: BackgroundJobQueue.BackgroundQueueEnabled()))
            {
                var expectedWorkerNames = ExpectedWorkerJobNames();
                List<IExecutionTimerBaseParam> timers =
                [
                    SyncByLdapExecutionTimer.GetParam(),
                    DeleteSysLogsTimer.GetParam(),
                    DeleteTemporaryFilesTimer.GetParam(),
                    DeleteTrashBoxTimer.GetParam(),
                    ReminderBackgroundTimer.GetParam(),
                    DeleteUnusedRecordTimer.GetParam(),
                    DeleteMcpLogsTimer.GetParam(),
                    DeleteTenantsTimer.GetParam(),
                    BackgroundJobDispatcher.GetParam(workerNumber: 1),
                    BackgroundJobTimeoutMonitor.GetParam(),
                    DeleteBackgroundJobsTimer.GetParam()
                ];
                for (var index = 0; index < expectedWorkerNames.Count; index++)
                {
                    timers.Add(BackgroundJobDispatcher.GetParam(workerNumber: index + 2));
                }
                foreach (var timer in timers)
                {
                    await AddTimer(timer);
                }
                await CleanupWorkerJobs(expectedWorkerNames: expectedWorkerNames);
                var threadPoolSize = await ResolveThreadPoolSizeAsync();
                BackgroundJobTargetTenants.WriteStartupSummary(
                    expectedWorkerNames: expectedWorkerNames,
                    threadPoolSize: threadPoolSize);
                BackgroundJobTargetTenants.WriteThreadPoolSharingNotice(
                    registeredTimers: timers,
                    threadPoolSize: threadPoolSize);
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
                var jobBuilder = JobBuilder.Create(timer.JobType)
                    .WithIdentity(timer.JobKey)
                    .StoreDurably();
                var jobData = timer.JobData;
                if (jobData != null)
                {
                    foreach (var data in jobData)
                    {
                        jobBuilder = jobBuilder.UsingJobData(
                            key: data.Key,
                            value: data.Value);
                    }
                }
                var job = jobBuilder.Build();
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

        internal static List<string> ExpectedWorkerJobNames()
        {
            var names = new List<string>();
            if (BackgroundJobQueue.BackgroundQueueEnabled() == false)
            {
                return names;
            }
            for (var workerNumber = 2;
                workerNumber <= BackgroundJobTargetTenants.EffectiveWorkerCount;
                workerNumber++)
            {
                names.Add(BackgroundJobDispatcher.GetParam(workerNumber: workerNumber).JobKey.Name);
            }
            return names;
        }

        private async Task CleanupWorkerJobs(List<string> expectedWorkerNames)
        {
            var context = new Context(
                request: false,
                sessionStatus: false,
                sessionData: false,
                user: false,
                item: false)
            {
                Controller = nameof(TimerBackground),
                Action = nameof(CleanupWorkerJobs)
            };
            try
            {
                var scheduler = CustomQuartzHostedService.Scheduler;
                var keys = await scheduler.GetJobKeys(
                    GroupMatcher<JobKey>.GroupEquals(BackgroundJobDispatcher.WorkerJobGroup));
                var targets = keys
                    .Where(key => expectedWorkerNames.Contains(key.Name) == false)
                    .ToList();
                if (targets.Any() == false)
                {
                    return;
                }
                await scheduler.DeleteJobs(targets);
                var deletedJobNames = targets.Select(key => key.Name).Join(", ");
                var clusteringEnabled = Parameters.Quartz?.Clustering?.Enabled == true;
                if (clusteringEnabled)
                {
                    _ = new SysLogModel(
                        context: context,
                        method: nameof(CleanupWorkerJobs),
                        message: $"Deleted {targets.Count} unexpected dispatcher worker job(s):"
                            + $" {deletedJobNames}."
                            + $" Clustering shares the job store,"
                            + $" so jobs of other nodes may be included."
                            + $" Align BackgroundJobs.WorkerCount across nodes"
                            + $" (this node: {BackgroundJobTargetTenants.EffectiveWorkerCount}).",
                        sysLogType: SysLogModel.SysLogTypes.Warning);
                }
                else
                {
                    _ = new SysLogModel(
                        context: context,
                        method: nameof(CleanupWorkerJobs),
                        message: $"Deleted {targets.Count} unexpected dispatcher worker job(s):"
                            + $" {deletedJobNames}",
                        sysLogType: SysLogModel.SysLogTypes.Info);
                }
            }
            catch (Exception e)
            {
                _ = new SysLogModel(
                    context: context,
                    e: e,
                    extendedErrorMessage: "Failed to clean up dispatcher worker jobs");
            }
        }

        private static async Task<int?> ResolveThreadPoolSizeAsync()
        {
            try
            {
                var metaData = await CustomQuartzHostedService.Scheduler.GetMetaData();
                return metaData.ThreadPoolSize;
            }
            catch (SchedulerException)
            {
                return null;
            }
        }
    }
}
