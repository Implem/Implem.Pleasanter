using Implem.DefinitionAccessor;
using Implem.Pleasanter.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    public class DeleteMcpLogsTimer : ClusterExecutionTimerBase
    {
        public class Param : IExecutionTimerBaseParam
        {
            public static readonly JobKey jobKey = new JobKey("DeleteMcpLogsTimer", "ExecutionTimerBase");
            public Type JobType => typeof(DeleteMcpLogsTimer);
            public IEnumerable<string> TimeList => Parameters.BackgroundService.DeleteMcpLogsTime;
            public bool Enabled => Parameters.BackgroundService.DeleteMcpLogs;
            public JobKey JobKey => jobKey;
            public string JobName => "DeleteMcpLogsService";
            public Task<bool> SetCustomTimer(IScheduler scheduler) => Task.FromResult(false);
        }

        public override async Task Execute(IJobExecutionContext context)
        {
            await Task.Run(() =>
            {
                var context = CreateContext();
                var log = CreateSysLogModel(
                    context: context,
                    message: "Delete McpLogs.");
                if (Parameters.BackgroundService.McpLogsRetentionPeriod >= 0)
                {
                    McpLogUtilities.PhysicalDelete(context);
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
