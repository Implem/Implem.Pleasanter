using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using Quartz;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    abstract public class ExecutionTimerBase : IJob
    {
        virtual public async Task Execute(IJobExecutionContext context) { await Task.CompletedTask; }

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
                method: $"{nameof(ExecutionTimerBase)}.{nameof(CreateSysLogModel)}",
                message: $"{GetType().FullName}.{callerMemberName} , {message}",
                sysLogType: SysLogModel.SysLogTypes.Info);
        }
    }
}
