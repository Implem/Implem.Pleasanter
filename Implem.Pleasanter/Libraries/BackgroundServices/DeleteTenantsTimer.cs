using Implem.DefinitionAccessor;
using Implem.Pleasanter.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    public class DeleteTenantsTimer : ClusterExecutionTimerBase
    {
        public class Param : IExecutionTimerBaseParam
        {
            public static readonly JobKey jobKey = new JobKey("DeleteTenantsTimer", "ExecutionTimerBase");
            public Type JobType => typeof(DeleteTenantsTimer);
            public IEnumerable<string> TimeList => Parameters.BackgroundService.DeleteTenantTime;
            public bool Enabled => Parameters.BackgroundService.DeleteTenant;
            public JobKey JobKey => jobKey;
            public string JobName => "DeleteTenantsService";
            public Task<bool> SetCustomTimer(IScheduler scheduler) => Task.FromResult(false);
        }

        public override async Task Execute(IJobExecutionContext context)
        {
            await Task.Run(() =>
            {
                var context = CreateContext();
                var log = CreateSysLogModel(
                    context: context,
                    message: "Delete Tenant.");
                TenantDeleteService.Execute(context: context);
                log.Finish(context: context);
            }, context.CancellationToken);
        }

        internal static IExecutionTimerBaseParam GetParam()
        {
            return new Param();
        }
    }
}
