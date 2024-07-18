using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using Quartz;
using System;
using System.Threading.Tasks;

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
                    // 呼び出すExecutionTimer実装クラスをここに追加する。
                    await AddTimer(timer: SyncByLdapExecutionTimer.GetParam());
                    await AddTimer(timer: DeleteSysLogsTimer.GetParam());
                    await AddTimer(timer: DeleteTemporaryFilesTimer.GetParam());
                    await AddTimer(timer: DeleteTrashBoxTimer.GetParam());
                    await AddTimer(timer: ReminderBackgroundTimer.GetParam());
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
                item: false);
            context.Controller = timer.JobName;
            try
            {
                if (!timer.Enabled) return;
                new SysLogModel(
                    context: context,
                    method: "ExecuteAsync",
                    message: $"{timer.JobName} ExecuteAsync() Started",
                    sysLogType: SysLogModel.SysLogTypes.Info);
                var scheduler = CustomQuartzHostedService.Scheduler;
                var job = JobBuilder.Create(timer.JobType)
                    .WithIdentity(timer.JobKey)
                    .StoreDurably()
                    .Build();
                await scheduler.AddJob(job, true);
                if (timer.TimeList != null)
                {
                    var timeZone = TimeZoneInfo.FindSystemTimeZoneById(
                        !Parameters.Service.TimeZoneDefault.IsNullOrEmpty()
                            ? Parameters.Service.TimeZoneDefault
                            : TimeZoneInfo.Utc.Id);
                    foreach (var hhmm in timer.TimeList)
                    {
                        if (DateTime.TryParse($"2020-01-01T{hhmm}:00.00", out var date))
                        {
                            var trigger = TriggerBuilder.Create()
                                .ForJob(timer.JobKey)
                                .WithCronSchedule(
                                    $"0 {date.Minute} {date.Hour} * * ? *",
                                    (s) => s.InTimeZone(timeZone))
                                .Build();
                            await scheduler.ScheduleJob(trigger);
                        }
                    }
                }
                else
                {
                    await timer.SetCustomTimer(scheduler: scheduler);
                }
            }
            catch (Exception e)
            {
                new SysLogModel(
                    context: context,
                    e: e,
                    extendedErrorMessage: $"{timer.JobName} ExecuteAsync() Failed to start");
            }
        }
    }
}
