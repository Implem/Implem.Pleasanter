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
    public class SyncByLdapTimer : TimerBase
    {
        override public async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = await Task.Run(() => UserUtilities.SyncByLdap(context: context), stoppingToken);
            log.Finish(context: context, responseSize: json.Length);
        }

        override public IList<string> GetTimeList()
        {
            return Parameters.BackgroundService.SyncByLdapTimerTime;
        }
    }
}
