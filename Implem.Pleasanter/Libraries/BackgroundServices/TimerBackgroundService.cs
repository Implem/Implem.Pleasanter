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
    /// <summary>
    /// ExecutionTimerBase(のサブクラス)を指定された時間に毎日呼び出すクラス
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
            AddTimer(timer: new DeleteSysLogsTimer());
            AddTimer(timer: new DeleteTemporaryFilesTimer());
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

        // UnitTestで現在時間を置き換えるためvirtual
        virtual protected DateTime DateTimeNow()
        {
            return DateTime.Now;
        }

        /// <summary>
        /// 引数の時間をサーバOSの時間に変換して返す。既に時間を過ぎていたら次の日の時間を返す。
        /// </summary>
        private DateTime GetTimerDateTime(string timeString)
        {
            var context = CreateContext();
            var timeOfDay = DateTime.ParseExact(timeString, "HH:mm", CultureInfo.InvariantCulture).TimeOfDay;
            var now = DateTimeNow();
            // SyncByLdapTime.GetTimeList()の時間はService.jsonのTimeZoneDefaultのタイムゾーンの時間として扱うのでToLocal()する。
            var localNow = now.ToLocal(context);
            var localTimerDateTime = localNow.Date + timeOfDay;
            /// ToUniversal()は実行しているOSのローカル時間に変換して返す(名前に反してUTCを返すわけではないので注意)
            var timerDateTime = localTimerDateTime.ToUniversal(context);
            // 時間が過ぎているのは次の日に回す
            if (timerDateTime < now)
            {
                timerDateTime = timerDateTime.AddDays(1);
            }
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

        // UnitTestでDalay()を置き換えるためvirtual
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
            var context = CreateContext();
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
