using Implem.DefinitionAccessor;
using Implem.Pleasanter.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    /// <summary>
    /// Reminderを定期的に呼び出すBackgroundServiceクラス
    /// </summary>
    class ReminderBackgroundTimer : ExecutionTimerBase
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
                var trigger = TriggerBuilder.Create()
                    .ForJob(JobKey)
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(60)
                        .RepeatForever())
                    .Build();
                await scheduler.ScheduleJob(trigger);
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
                    new SysLogModel(
                        context: context,
                        e: e,
                        extendedErrorMessage: "ReminderBackgroundService Canceled");
                }
                catch (Exception e)
                {
                    new SysLogModel(
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
