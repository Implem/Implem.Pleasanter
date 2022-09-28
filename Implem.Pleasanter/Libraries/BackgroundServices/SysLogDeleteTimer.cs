using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.DataSources;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    /// <summary>
    /// SysLog削除を毎日定時に呼び出すクラス
    /// </summary>
    public class SysLogDeleteTimer : ExecutionTimerBase
    {
        override public async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Run(() =>
            {
                var context = CreateContext();
                var log = CreateSysLogModel(
                    context: context,
                    message: "delete SysLog.");
                if (Parameters.SysLog.RetentionPeriod > 0)
                {
                    Repository.ExecuteNonQuery(
                        context: context,
                        statements: Rds.PhysicalDeleteSysLogs(
                            where: Rds.SysLogsWhere().CreatedTime(
                                DateTime.Now.Date.AddDays(
                                    Parameters.SysLog.RetentionPeriod * -1),
                                _operator: "<")));
                }
                log.Finish(context: context);
            }, stoppingToken);
        }

        override public IList<string> GetTimeList()
        {
            return Parameters.BackgroundService.DeleteSysLogTime;
        }

        public override bool Enabled()
        {
            return Parameters.BackgroundService.DeleteSysLog;
        }
    }
}
