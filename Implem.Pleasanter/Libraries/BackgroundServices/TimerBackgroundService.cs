using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Implem.Pleasanter.Libraries.Server;
using System.Globalization;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    public class TimerBackgroundService : BackgroundService
    {
        public class TimeExecuteTimerPair
        {
            public DateTime DateTime { get; set; }
            public ExecutionTimerBase ExecutionTimer { get; set; }
        }

        readonly protected List<TimeExecuteTimerPair> TimerList = new List<TimeExecuteTimerPair>();

        public TimerBackgroundService()
        {
            AddTimer(timer: new SyncByLdapExecutionTimer());
            AddTimer(timer: new DeleteSysLogsTimer());
            AddTimer(timer: new DeleteTemporaryFilesTimer());
            AddTimer(timer: new DeleteTrashBoxTimer());
        }

        private void SortTimerList()
        {
            TimerList.Sort((x, y) => x.DateTime.CompareTo(y.DateTime));
        }

        private Context CreateContext()
        {
            return new Context(
                request: false,
                sessionStatus: false,
                sessionData: false,
                user: false,
                item: false);
        }

        protected void AddTimer(ExecutionTimerBase timer)
        {
            if (!timer.Enabled())
            {
                return;
            }
            try
            {
                foreach (var timeString in timer.GetTimeList())
                {
                    TimerList.Add(new TimeExecuteTimerPair()
                    {
                        DateTime = GetTimerDateTime(timeString: timeString),
                        ExecutionTimer = timer
                    });
                }
            }
            catch (Exception e)
            {
                var context = CreateContext();
                new SysLogModel(
                    context: context,
                    e: e,
                    extendedErrorMessage: "AddTimer() Exception");
            }
            SortTimerList();
        }

        virtual protected DateTime DateTimeNow()
        {
            return DateTime.Now;
        }

        private DateTime GetTimerDateTime(string timeString)
        {
            var context = CreateContext();
            var timeOfDay = DateTime.ParseExact(timeString, "HH:mm", CultureInfo.InvariantCulture).TimeOfDay;
            var now = DateTimeNow();
            var localNow = now.ToLocal(context);
            var localTimerDateTime = localNow.Date + timeOfDay;
            var timerDateTime = localTimerDateTime.ToUniversal(context);
            if (timerDateTime < now)
            {
                timerDateTime = timerDateTime.AddDays(1);
            }
            return timerDateTime;
        }

        private void ScheduleToNextDay(TimeExecuteTimerPair timerPair)
        {
            Debug.Assert(TimerList.First() == timerPair);
            TimerList.Remove(timerPair);
            var nextDayTime = timerPair.DateTime.AddDays(1);
            TimerList.Add(new TimeExecuteTimerPair()
            {
                DateTime = nextDayTime,
                ExecutionTimer = timerPair.ExecutionTimer
            });
            SortTimerList();
        }

        virtual protected async Task TaskDelay(TimeSpan waitTimeSpan, CancellationToken stoppingToken)
        {
            await Task.Delay(
                waitTimeSpan,
                stoppingToken);
        }

        private async Task WaitNextTimer(CancellationToken stoppingToken)
        {
            var now = DateTimeNow();
            var waitTimeSpan = TimerList.First().DateTime - now;
            waitTimeSpan = TimeSpan.Zero < waitTimeSpan
                ? waitTimeSpan
                : TimeSpan.Zero;
            await TaskDelay(
                waitTimeSpan: waitTimeSpan,
                stoppingToken: stoppingToken);
        }

        public async Task WaitNextTimerThenExecuteAsync(CancellationToken stoppingToken)
        {
            if (!TimerList.Any())
            {
                return;
            }
            await WaitNextTimer(stoppingToken: stoppingToken);
            var nextTiemr = TimerList.First();
            ScheduleToNextDay(nextTiemr);
            await nextTiemr.ExecutionTimer.ExecuteAsync(stoppingToken: stoppingToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var context = CreateContext();
            new SysLogModel(
                context: context,
                method: nameof(ExecuteAsync),
                message: "TimerBackgroundService ExecuteAsync() Started",
                sysLogType: SysLogModel.SysLogTypes.Info);
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await WaitNextTimerThenExecuteAsync(stoppingToken: stoppingToken);
                }
                catch (OperationCanceledException e)
                {
                    new SysLogModel(
                        context: context,
                        e: e,
                        extendedErrorMessage: "TimerBackgroundService Canceled");
                    break;
                }
                catch (Exception e)
                {
                    new SysLogModel(
                        context: context,
                        e: e,
                        extendedErrorMessage: "TimerBackgroundService Exception");
                }
            }
            new SysLogModel(
                context: context,
                method: nameof(ExecuteAsync),
                message: "TimerBackgroundService ExecuteAsync() Stopped",
                sysLogType: SysLogModel.SysLogTypes.Info);
        }
    }
}
