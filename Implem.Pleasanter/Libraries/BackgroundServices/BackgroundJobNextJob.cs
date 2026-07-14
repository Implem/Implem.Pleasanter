using Implem.Libraries.Utilities;
using Implem.Libraries.Classes;
using Implem.Pleasanter.Models;
using Quartz;
using System.Threading.Tasks;

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
            await BackgroundJobDispatcher.ExecuteOneJob(
                context: context,
                ignoreRunningOverdueTenantLocks: true,
                targetTenantId: model.TenantId);
        }
    }
}
