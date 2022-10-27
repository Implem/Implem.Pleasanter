using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    abstract public class ExecutionTimerBase
    {
        virtual public async Task ExecuteAsync(CancellationToken stoppingToken) { await Task.CompletedTask; }

        abstract public IList<string> GetTimeList();

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
                method: $"{nameof(ExecutionTimerBase)}.{nameof(CreateSysLogModel)}",
                message: $"{GetType().FullName}.{callerMemberName} , {message}",
                sysLogType: SysLogModel.SysLogTypes.Info);
        }
    }
}
