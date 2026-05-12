using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using Quartz;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    internal static class TimerTriggerRegistrar
    {
        internal static TriggerKey CronTriggerKey(JobKey jobKey, string hhmm)
        {
            var slot = hhmm.Replace(":", string.Empty);
            return new TriggerKey($"{jobKey.Name}_cron_{slot}", jobKey.Group);
        }

        internal static TriggerKey SimpleTriggerKey(
            JobKey jobKey,
            string suffix = "default")
        {
            return new TriggerKey(
                $"{jobKey.Name}_simple_{suffix}",
                jobKey.Group);
        }

        internal static async Task EnsureTriggerAsync(
            IScheduler scheduler,
            ITrigger trigger)
        {
            try
            {
                if (await scheduler.CheckExists(trigger.Key))
                {
                    await scheduler.RescheduleJob(trigger.Key, trigger);
                }
                else
                {
                    await scheduler.ScheduleJob(trigger);
                }
            }
            catch (ObjectAlreadyExistsException)
            {
                await scheduler.RescheduleJob(trigger.Key, trigger);
            }
        }

        internal static async Task CleanupUnexpectedTriggersAsync(
            IScheduler scheduler,
            JobKey jobKey,
            IEnumerable<TriggerKey> expectedKeys)
        {
            var expected = expectedKeys.ToHashSet();
            var currentTriggers = await scheduler.GetTriggersOfJob(jobKey);
            var staleKeys = currentTriggers
                .Select(trigger => trigger.Key)
                .Where(key => !expected.Contains(key));
            if (!staleKeys.Any()) return;
            foreach (var staleKey in staleKeys)
            {
                await scheduler.UnscheduleJob(staleKey);
            }
            var context = new Context(
                request: false,
                sessionStatus: false,
                sessionData: false,
                user: false,
                item: false);
            _ = new SysLogModel(
                context: context,
                method: $"{nameof(TimerTriggerRegistrar)}.{nameof(CleanupUnexpectedTriggersAsync)}",
                message: $"Cleaned up unexpected triggers for job {jobKey}. Removed triggers: {string.Join(", ", staleKeys)}");
        }
    }
}
