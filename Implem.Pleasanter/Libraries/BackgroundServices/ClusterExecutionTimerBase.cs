using Quartz;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    /// <summary>
    /// 毎日定時に呼び出されるクラスのクラスター対応版ベースクラス。
    /// 各種タイマー毎にこのクラスのサブクラスとして実装する。
    /// TimerBackgroundServiceクラスによってこのクラスのメソッドExecute()が毎日指定された時間に実行される。
    /// </summary>
    [DisallowConcurrentExecution]
    abstract public class ClusterExecutionTimerBase : ExecutionTimerBase
    {
    }
}
