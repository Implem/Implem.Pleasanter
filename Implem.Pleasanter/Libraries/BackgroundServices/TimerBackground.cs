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
                    await AddTimer(timer: SyncByLdapExecutionTimer.GetParam());
                    await AddTimer(timer: DeleteSysLogsTimer.GetParam());
                    await AddTimer(timer: DeleteTemporaryFilesTimer.GetParam());
                    await AddTimer(timer: DeleteTrashBoxTimer.GetParam());
                });
            }
        }

        private async Task AddTimer(IExecutionTimerBaseParam timer)
        {
            try
            {
                if (!timer.Enabled) return;
                var scheduler = CustomQuartzHostedService.Scheduler;
                var job = JobBuilder.Create(timer.JobType)
                    .WithIdentity(timer.JobKey)
                    .StoreDurably()
                    .Build();
                await scheduler.AddJob(job, true);
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
            catch (Exception e)
            {
                new SysLogModel(
                    context: new Context(request: false),
                    e: e,
                    extendedErrorMessage: $" / Fail {timer.JobKey.Group}");
            }
        }
    }
}
