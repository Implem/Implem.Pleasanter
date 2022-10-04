using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    /// <summary>
    /// 毎日定時に呼び出されるクラスのベースクラス。
    /// 各種タイマー毎にこのクラスのサブクラスとして実装する。
    /// TimerBackgroundServiceクラスによってこのクラスのメソッドExecuteAsync()が毎日指定された時間に実行される。
    /// </summary>
    abstract public class ExecutionTimerBase
    {
        virtual public async Task ExecuteAsync(CancellationToken stoppingToken) { await Task.CompletedTask; }

        /// <summary>
        /// ExecuteAsync()を呼び出す時間を"HH:MM"形式の文字列リストで返す。BackgroundService.jsonの設定を読み込んで返す。
        /// </summary>
        abstract public IList<string> GetTimeList();

        /// <summary>
        /// このタイマーのExecuteAsync()を呼び出すかを返す。BackgroundService.jsonの設定でこのタイマーが有効化されているかどうか。
        /// </summary>
        abstract public bool Enabled();

        protected Context CreateContext()
        {
            return new Context(
                request: false,
                sessionStatus: false,
                sessionData: false,
                user: false,
                item: false);
        }

        protected SysLogModel CreateSysLogModel(Context context, string message, [CallerMemberName] string callerMemberName = "")
        {
            return new SysLogModel(
                context: context,
                method: nameof(CreateSysLogModel),
                message: $"{GetType().FullName}.{callerMemberName} , {message}",
                sysLogType: SysLogModel.SysLogTypes.Info);
        }
    }
}
