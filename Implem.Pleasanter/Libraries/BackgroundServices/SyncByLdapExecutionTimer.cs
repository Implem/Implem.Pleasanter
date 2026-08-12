using Implem.DefinitionAccessor;
using Implem.Pleasanter.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    public class SyncByLdapExecutionTimer : ClusterExecutionTimerBase
    {
        public class Param : IExecutionTimerBaseParam
        {
            public static readonly JobKey jobKey = new JobKey("SyncByLdapExecutionTimer", "ExecutionTimerBase");
            public Type JobType => typeof(SyncByLdapExecutionTimer);
            public IEnumerable<string> TimeList => Parameters.BackgroundService.SyncByLdapTime;
            public bool Enabled => Parameters.BackgroundService.SyncByLdap;
            public JobKey JobKey => jobKey;
            public string JobName => "SyncByLdapExecutionService";
            public Task<bool> SetCustomTimer(IScheduler scheduler) => Task.FromResult(false);
        }

        public override async Task Execute(IJobExecutionContext context)
        {
            await Task.Run(() =>
            {
                var context = CreateContext();
                var log = CreateSysLogModel(
                    context: context,
                    message: "Execute SyncByLdap().");
                string json = null;
                try
                {
                    json = UserUtilities.SyncByLdap(context: context);
                }
                finally
                {
                    log.Finish(
                        context: context,
                        responseSize: json?.Length ?? 0);
                }
            }, context.CancellationToken);
        }

        internal static IExecutionTimerBaseParam GetParam()
        {
            return new Param();
        }
    }
}
