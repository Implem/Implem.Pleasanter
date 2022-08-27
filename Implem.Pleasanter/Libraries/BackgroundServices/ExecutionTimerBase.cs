using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    /// <summary>
    /// 毎日定時に呼び出されるクラス。
    /// ExecuteAsync()が毎日指定した時間に実行される。
    /// メソッドGetTimerList()で実行される時間を返す。
    /// </summary>
    abstract public class ExecutionTimerBase
    {
        virtual public async Task ExecuteAsync(CancellationToken stoppingToken) { await Task.CompletedTask; }

        /// <summary>
        /// ExecuteAsync()を呼び出す時間を返す。時間を"HH:MM"形式のリストで返す。
        /// </summary>
        abstract public IList<string> GetTimeList();
    }
}
