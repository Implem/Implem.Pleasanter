using Quartz;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    [DisallowConcurrentExecution]
    abstract public class ClusterExecutionTimerBase : ExecutionTimerBase
    {
    }
}
