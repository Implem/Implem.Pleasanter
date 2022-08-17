using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    /// <summary>
    /// 毎日定時にExecutionTimerBase(のサブクラス)を呼び出すクラス
    /// </summary>
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
            // 呼び出すExecutionTimer実装クラスをここに追加する。
            AddTimer(timer: new SyncByLdapExecutionTimer());
        }

        private void SortTimerList()
        {
            TimerList.Sort((x, y) => x.DateTime.CompareTo(y.DateTime));
        }

        protected void AddTimer(ExecutionTimerBase timer)
        {
            foreach (var timeString in timer.GetTimeList())
            {
                TimerList.Add(new TimeExecuteTimerPair()
                {
                    DateTime = GetTimerDateTime(timeString: timeString),
                    ExecutionTimer = timer
                });
            }
            SortTimerList();
        }

        virtual protected DateTime DateTimeNow()
        {
            return DateTime.Now;
        }

        private DateTime GetTimerDateTime(string timeString)
        {
            //TODO Parse()の例外対応どうする //TODO どうなるか確認。
            //TODO TimeZoneは考慮する必要があるか //TODO toLocalTime使う
            var timeOfDay = DateTime.Parse(timeString).TimeOfDay; //TODO JSONにエラーがあったらどうなるか。
            var now = DateTimeNow();
            var timerDateTime = now.Date + timeOfDay;
            // 時間が過ぎているのは次の日に回す
            if (timerDateTime < now)
            {
                timerDateTime = timerDateTime.AddDays(1);
            }
            SortTimerList();
            return timerDateTime;
        }

        private void ScheduleToNextDay(TimeExecuteTimerPair timerPair)
        {
            //再スケジュールするのは先頭要素だけのはず
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
            var waitTimeSpan = TimerList.First().DateTime - DateTimeNow();
            waitTimeSpan = TimeSpan.Zero < waitTimeSpan ? waitTimeSpan : TimeSpan.Zero; 
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
            //ExecuteAsync()呼び出し前に先頭のタイマー時間は次の日に進めておく。ExecuteAsync()が例外の場合に連続呼び出しを避けるため。
            ScheduleToNextDay(nextTiemr);
            await nextTiemr.ExecutionTimer.ExecuteAsync(stoppingToken: stoppingToken);
        }

        /// <summary>
        /// 内部でループして定期的にTimerBaseのサブクラスのExecuteAsync()を呼び出す。
        /// Pleasanter起動時にGeneric Hostにより自動的に呼ばれる。
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var context = new Context(
                request: false,
                sessionStatus: false,
                sessionData: false,
                user: false,
                item: false);
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
                message: "ExecuteAsync() Canceled",
                sysLogType: SysLogModel.SysLogTypes.Info);
        }
    }
}
