﻿using Implem.DefinitionAccessor;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    /// <summary>
    /// SysLog削除を毎日定時に呼び出すクラス
    /// </summary>
    public class SysLogsDeleteTimer : ExecutionTimerBase
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
                    SysLogUtilities.PhysicalDelete(context);
                }
                log.Finish(context: context);
            }, stoppingToken);
        }

        override public IList<string> GetTimeList()
        {
            return Parameters.BackgroundService.DeleteSysLogsTime;
        }

        public override bool Enabled()
        {
            return Parameters.BackgroundService.DeleteSysLogs;
        }
    }
}
