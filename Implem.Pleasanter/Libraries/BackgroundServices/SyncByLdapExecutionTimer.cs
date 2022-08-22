using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    /// <summary>
    /// SyncByLdapを毎日定時に呼び出すクラス
    /// </summary>
    public class SyncByLdapExecutionTimer : ExecutionTimerBase
    {
        override public async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Run(() =>
            {
                var context = new Context(
                    request: false,
                    sessionStatus: false,
                    sessionData: false,
                    user: false,
                    item: false);
                var log = new SysLogModel(
                    context: context,
                    method: nameof(ExecuteAsync),
                    message: "ExecuteAsync() Called.",
                    sysLogType: SysLogModel.SysLogTypes.Info);
                var json = UserUtilities.SyncByLdap(context: context);
                log.Finish(
                    context: context,
                    responseSize: json.Length);
            }, stoppingToken);
        }

        override public IList<string> GetTimeList()
        {
            return Parameters.BackgroundService.SyncByLdapTime;
        }
    }
}
