using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    /// <summary>
    /// 毎日定時に呼び出す処理
    /// </summary>
    abstract public class ExecutionTimerBase
    {
        virtual public async Task ExecuteAsync(CancellationToken stoppingToken) { await Task.CompletedTask; }

        abstract public IList<string> GetTimeList();
    }
}
