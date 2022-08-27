using Implem.Pleasanter.Libraries.BackgroundServices;
using Implem.Libraries.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Implem.PleasanterTest.Tests.BackgroundService
{
    /// <summary>
    /// テスト対象TimerBackgroundServiceのMockクラス
    /// </summary>
    public class MockTimerBackgroundService : TimerBackgroundService
    {
        // ExecuteAsync()を呼んだ順番を覚えておくリスト
        readonly public List<string> ExecuteAsyncCalledList = new ();
        readonly public List<TimeSpan> TaskDelayCalledList = new ();
        // 現在時間を固定する用
        public DateTime DateTimeNow_ = DateTime.Parse("2022-08-12 00:00");

        public List<TimeExecuteTimerPair> GetTimerList() { return TimerList; }

        public MockTimerBackgroundService()
        {
            //CI/CDサーバでタイムゾーンが違う影響を避けるため
            Environments.TimeZoneInfoDefault = null;
            //TimerBackgroundServiceにはコンストラクタで事前にリストに入っているのでテスト用に消しておく
            TimerList.Clear();
        }

        override protected DateTime DateTimeNow()
        {
            return DateTimeNow_;
        }

        override protected async Task TaskDelay(TimeSpan waitTimeSpan, CancellationToken stoppingToken)
        {
            //後でチェックのため呼び出された事を教えておく。
            TaskDelayCalledList.Add(waitTimeSpan);
            await Task.CompletedTask;
        }

        /// <summary>
        /// テスト対象クラスのprotectedなメソッドをテスト用に呼び出すため
        /// </summary>
        public void AddTimer_(ExecutionTimerBase timer)
        {
            AddTimer(timer);
        }
    }

    /// <summary>
    /// MockTimerBackgroundServiceの実行に必要なスタブクラス。
    /// MockTimerBackgroundServiceクラスから呼び出される。
    /// </summary>
    public class StubTimer : ExecutionTimerBase
    {
        public readonly string Name; 
        public MockTimerBackgroundService Parent;
        public List<string> TimeList = new List<string>();

        public StubTimer(string name, MockTimerBackgroundService parent)
        {
            Name = name;
            Parent = parent;
        }

        public override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //後でチェックのため呼び出された事を親クラスに教えておく。
            Parent.ExecuteAsyncCalledList.Add(Name);
            await Task.CompletedTask;
        }

        public override IList<string> GetTimeList()
        {
            return TimeList;
        }
    }

    public class TimerBackgroundServiceTest
    {

        [Fact]
        public async void WaitNextTimerThenExecuteAsync_TimerEmpty_DoesNotThrowException()
        {
            //Arrange
            var targetMock = new MockTimerBackgroundService();
            //Act
            var exception = await Record.ExceptionAsync(() => targetMock.WaitNextTimerThenExecuteAsync(CancellationToken.None));
            //Assert
            Assert.Null(exception);
        }

        [Fact]
        public void AddTimerTest()
        {
            //Arrange
            var targetMock = new MockTimerBackgroundService();
            // DateTime.Nowを固定
            targetMock.DateTimeNow_ = DateTime.Parse("2022-08-12 02:00");
            var timer1 = new StubTimer(name: "Timer1", parent: targetMock);
            timer1.TimeList.Add("03:00");
            timer1.TimeList.Add("01:00");
            //同じ時間も追加
            timer1.TimeList.Add("01:00");
            //Act
            targetMock.AddTimer_(timer: timer1);
            //Assert
            var expectedScheduledTimeList = new DateTime[]
            {
                //既に過ぎている時間は次の日8/13にスケジュールされているはず。
                DateTime.Parse("2022-08-12 03:00"),
                DateTime.Parse("2022-08-13 01:00"),
                DateTime.Parse("2022-08-13 01:00"),
            };
            Assert.Equal(expectedScheduledTimeList, targetMock.GetTimerList().Select(t => t.DateTime));
        }

        [Fact]
        public async void WaitNextTimerThenExecuteAsyncTest()
        {
            // Arrange
            var targetMock = new MockTimerBackgroundService();
            // DateTime.Nowを固定
            targetMock.DateTimeNow_ = DateTime.Parse("2022-08-12 00:00");
            // Timer1を２回(3:00と1:00)、Timer2は2:00で、StubTimer登録しておく。3つが下の順で登録される。
            // "2022-08-12 01:00", Timer1
            // "2022-08-12 02:00", Timer2
            // "2022-08-12 03:00", Timer1
            var timer1 = new StubTimer(name: "Timer1", parent: targetMock);
            timer1.TimeList.Add("03:00");
            timer1.TimeList.Add("01:00");
            targetMock.AddTimer_(timer: timer1);
            var timer2 = new StubTimer(name: "Timer2", parent: targetMock);
            timer2.TimeList.Add("02:00");
            targetMock.AddTimer_(timer: timer2);
            // Act
            // TimerBackgroundService:ExecuteAsync()内で2回ループを回った想定
            await targetMock.WaitNextTimerThenExecuteAsync(CancellationToken.None);
            await targetMock.WaitNextTimerThenExecuteAsync(CancellationToken.None);
            // Assert
            // StubTimerインスタンスはこの順番で呼ばれたはず。
            var executeAsyncCalledList = new List<string>() { "Timer1", "Timer2" };
            Assert.Equal(executeAsyncCalledList, targetMock.ExecuteAsyncCalledList);
            // StubTimerインスタンスが呼び出された待ち時間は、それぞれ下の待ち時間後だったはず。
            // (MockTimerBackgroundService内で呼び出し待ち時間(Task.Delay()の時間を記録して確認している。)
            var taskDelayCalledList = new List<TimeSpan>()
            {
                TimeSpan.Parse("01:00"),
                TimeSpan.Parse("02:00")
            };
            Assert.Equal(taskDelayCalledList, targetMock.TaskDelayCalledList);
            // タイマー予約時間が進んで現状下のようになっているはず。
            var expectedScheduledTimeList = new DateTime[]
            {
                DateTime.Parse("2022-08-12 03:00"),
                DateTime.Parse("2022-08-13 01:00"),
                DateTime.Parse("2022-08-13 02:00")
            };
            Assert.Equal(expectedScheduledTimeList, targetMock.GetTimerList().Select(t => t.DateTime));
            // 現状のタイマー待ち順番は下のようになっているはず。
            var expectedTimerBaseList = new object[]
            {
                timer1,
                timer1,
                timer2
            };
            Assert.Equal(expectedTimerBaseList, targetMock.GetTimerList().Select(t => t.ExecutionTimer));
        }
    }
}
