using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    /// <summary>
    /// 毎日定時に呼び出すBackgroundServiceクラス
    /// </summary>
    public class TimerBackgroundService : BackgroundService
    {
        readonly protected IList<TimerBase> Timers = new List<TimerBase>();
        readonly protected SortedList<DateTime, TimerBase> ScheduledTimerList = new SortedList<DateTime, TimerBase>();

        public TimerBackgroundService()
        {
            AddTimer(new SyncByLdapTimer());
        }

        protected void AddTimer(TimerBase timer)
        {
            Timers.Add(timer);
            foreach(var t in GetTimerDateTimeList(timer))
            {
                ScheduledTimerList.Add(t, timer);
            }
        }

        virtual protected DateTime DateTimeNow()
        {
            return DateTime.Now;
        }

        private IEnumerable<DateTime> GetTimerDateTimeList(TimerBase timer)
        {
            return timer.GetTimeList().Select(timeString =>
            {
                // timeStringに日付があっても無視してTimeOfDayだけ取り出す
                var timeOfDay = DateTime.Parse(timeString).TimeOfDay;
                var now = DateTimeNow();
                var timerDateTime = now.Date + timeOfDay;
                // 時間が過ぎているのは次の日に回す
                if ((timerDateTime - now) <= TimeSpan.Zero)
                {
                    timerDateTime = timerDateTime.AddDays(1);
                }
                return timerDateTime;
            });
        }

        private (DateTime DateTime, TimerBase Timer) GetFirstTimer()
        {
            return (ScheduledTimerList.First().Key, ScheduledTimerList.First().Value);
        }

        private void ScheduleFirstTimerToNextDay()
        {
            var firstTimer = GetFirstTimer();
            ScheduledTimerList.RemoveAt(0);
            var nextDayTimer = firstTimer.DateTime.AddDays(1);
            ScheduledTimerList.Add(nextDayTimer, firstTimer.Timer);
        }

        private Context CreateEmptyContext()
        {
            return new Context(
                request: false,
                sessionStatus: false,
                sessionData: false,
                user: false,
                item: false);
        }

        virtual protected async Task TaskDelay(TimeSpan waitTimeSpan, CancellationToken stoppingToken)
        {
            await Task.Delay(waitTimeSpan, stoppingToken);
        }

        private async Task WaitNextTimer(CancellationToken stoppingToken)
        {
            var waitTimeSpan = GetFirstTimer().DateTime - DateTimeNow();
            if (waitTimeSpan <= TimeSpan.Zero)
            {
                return;
            }
            await TaskDelay(waitTimeSpan, stoppingToken);
        }

        public async Task WaitNextTimerThenExecuteAsync(CancellationToken stoppingToken)
        {
            if (!ScheduledTimerList.Any())
            {
                return;
            }
            await WaitNextTimer(stoppingToken);
            var nextTiemr = GetFirstTimer().Timer;
            //ExecuteAsync()呼び出し前に先頭のタイマー時間は次の日に進めておく。ExecuteAsync()が例外の場合に連続呼び出しを避けるため。
            //ExecuteAsync()処理リトライを考慮したいなら各TimerBase側でリトライの実装をすること。
            ScheduleFirstTimerToNextDay();
            await nextTiemr.ExecuteAsync(stoppingToken);
        }

        /// <summary>
        /// 内部でループして定期的にTimerBaseのサブクラスのExecuteAsync()を呼び出す。
        /// Pleasanter起動時にGeneric Hostにより自動的に呼ばれる。
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!Parameters.BackgroundService.SyncByLdapTimer)
            {
                return;
            }
            var context = CreateEmptyContext();
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await WaitNextTimerThenExecuteAsync(stoppingToken);
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
                message: "ExecuteAsync() Canceled",
                sysLogType: SysLogModel.SysLogTypes.Info);
        }
    }
}
