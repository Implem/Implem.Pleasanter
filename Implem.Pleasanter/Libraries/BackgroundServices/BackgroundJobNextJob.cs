using System;
using System.Threading.Tasks;
using Implem.Libraries.Classes;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using Quartz;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    public class BackgroundJobNextJob : ExecutionTimerBase
    {
        public const string GroupName = "BackgroundJobNextJob";
        public const string BackgroundJobIdKey = "backgroundJobId";

        public static JobKey JobKey(long backgroundJobId, string nonce)
        {
            return new JobKey(
                $"NextJob_{backgroundJobId}_{nonce}",
                GroupName);
        }

        public static TriggerKey TriggerKey(long backgroundJobId, string nonce)
        {
            return new TriggerKey(
                $"NextJob_{backgroundJobId}_{nonce}",
                GroupName);
        }

        public override async Task Execute(IJobExecutionContext jobContext)
        {
            var backgroundJobId = jobContext
                .MergedJobDataMap
                .GetString(BackgroundJobIdKey)
                .ToLong();
            if (backgroundJobId <= 0) return;
            var context = CreateContext();
            var model = new BackgroundJobModel(
                context: context,
                backgroundJobId: backgroundJobId);
            if (model.AccessStatus != Databases.AccessStatuses.Selected
                || model.Status != BackgroundJobStatus.RunningOverdue)
            {
                return;
            }
            if (BackgroundJobTargetTenants.ProcessingEnabled == false
                || BackgroundJobTargetTenants.IsInScope(tenantId: model.TenantId) == false)
            {
                var reason = BackgroundJobTargetTenants.ProcessingEnabled == false
                    ? "this node does not process background jobs"
                    : "out of target tenant scope";
                new SysLogModel(
                    context: new Context(
                        tenantId: model.TenantId,
                        request: false,
                        context: context)
                    {
                        Controller = nameof(BackgroundJobNextJob),
                        Action = nameof(Execute)
                    },
                    method: "",
                    message: $"Skipped: {reason}."
                        + $" BackgroundJobId={model.BackgroundJobId}"
                        + $", TenantId={model.TenantId}",
                sysLogType: SysLogModel.SysLogTypes.Info);
                return;
            }
            try
            {
                var claimed = BackgroundJobDispatcher.ClaimJob(
                    context: context,
                    ignoreRunningOverdueTenantLocks: true,
                    targetTenantId: model.TenantId);
                if (claimed == null) return;
                await BackgroundJobDispatcher.RunJob(
                    context: context,
                    model: claimed);
            }
            catch (Exception e)
            {
                new SysLogModel(
                    context: new Context(
                        tenantId: context.TenantId,
                        request: false,
                        context: context)
                    {
                        Controller = nameof(BackgroundJobNextJob),
                        Action = nameof(Execute)
                    },
                    e: e,
                    extendedErrorMessage: "BackgroundJobNextJob Exception");
            }
        }
    }
}
