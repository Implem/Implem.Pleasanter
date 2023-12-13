using Implem.DefinitionAccessor;
using Implem.Pleasanter.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    public class DeleteSysLogsTimer : ExecutionTimerBase
    {
        public class Param : IExecutionTimerBaseParam
        {
            public static readonly JobKey jobKey = new JobKey("DeleteSysLogsTimer", "ExecutionTimerBase");
            public Type JobType => typeof(DeleteSysLogsTimer);
            public IEnumerable<string> TimeList => Parameters.BackgroundService.DeleteSysLogsTime;
            public bool Enabled => Parameters.BackgroundService.DeleteSysLogs;
            public JobKey JobKey => jobKey;
        }

        public override async Task Execute(IJobExecutionContext context)
        {
            await Task.Run(() =>
            {
                var context = CreateContext();
                var log = CreateSysLogModel(
                    context: context,
                    message: "Delete SysLogs.");
                if (Parameters.SysLog.RetentionPeriod > 0)
                {
                    SysLogUtilities.PhysicalDelete(context);
                }
                log.Finish(context: context);
            }, context.CancellationToken);
        }

        internal static IExecutionTimerBaseParam GetParam()
        {
            return new Param();
        }
    }
}
