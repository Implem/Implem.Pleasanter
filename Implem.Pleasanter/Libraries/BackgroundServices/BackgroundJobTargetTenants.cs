using System.Collections.Generic;
using System.Linq;
using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    public static class BackgroundJobTargetTenants
    {
        public enum StartupNotices
        {
            None,
            SingleTenantScope,
            InvalidDefaultTenantId
        }

        private const int DefaultThreadPoolSize = 10;

        private static volatile bool s_multiTenant;
        private static volatile int s_defaultTenantId;
        private static volatile bool s_queueEnabled;
        private static volatile bool s_timerEnabled;
        private static volatile bool s_stopRequested;
        private static volatile int s_effectiveWorkerCount;
        private static volatile int s_verifiedWorkerCount;
        private static volatile int s_workerCountLimit;
        private static volatile int s_configuredWorkerCount;
        private static volatile bool s_workerCountSpecified;
        private static volatile bool s_workerCountCorrected;
        private static volatile bool s_workerCountIgnored;

        public static bool MultiTenant => s_multiTenant;

        public static int DefaultTenantId => s_defaultTenantId;

        public static bool ProcessingEnabled =>
            s_timerEnabled
            && s_queueEnabled
            && (s_multiTenant || s_defaultTenantId > 0);

        public static int ScopedTenantId => s_multiTenant ? 0 : s_defaultTenantId;

        public static bool StopRequested => s_stopRequested;

        public static StartupNotices StartupNotice
        {
            get
            {
                if (s_queueEnabled == false || s_multiTenant) return StartupNotices.None;
                return s_defaultTenantId > 0
                    ? StartupNotices.SingleTenantScope
                    : StartupNotices.InvalidDefaultTenantId;
            }
        }

        public static int EffectiveWorkerCount => s_effectiveWorkerCount;

        public static int WorkerCountLimit => s_workerCountLimit;

        public static int? ConfiguredWorkerCount => s_workerCountSpecified
            ? s_configuredWorkerCount
            : null;

        public static bool WorkerCountCorrectedNotice => s_queueEnabled && s_workerCountCorrected;

        public static bool WorkerCountIgnoredNotice => s_queueEnabled && s_workerCountIgnored;

        public static void RequestStop()
        {
            s_stopRequested = true;
        }

        public static bool IsInScope(int tenantId)
        {
            return s_multiTenant || tenantId == s_defaultTenantId;
        }

        public static void Evaluate()
        {
            s_multiTenant = Parameters.AllowMultiTenants();
            s_defaultTenantId = Parameters.MultiTenant?.DefaultTenantId ?? 0;
            s_queueEnabled = BackgroundJobQueue.BackgroundQueueEnabled();
            s_timerEnabled = Parameters.BackgroundService?.TimerEnabled(
                deploymentEnvironment: Parameters.Service?.DeploymentEnvironment,
                backgroundQueueEnabled: s_queueEnabled) == true;
            EvaluateWorkerCount();
            WriteStartupNotice();
            WriteWorkerCountNotice();
        }

        private static Context NoticeContext(string action)
        {
            return new Context(
                request: false,
                sessionStatus: false,
                sessionData: false,
                user: false,
                item: false)
            {
                Controller = nameof(BackgroundJobTargetTenants),
                Action = action
            };
        }

        private static void WriteStartupNotice()
        {
            var context = NoticeContext(nameof(Evaluate));
            switch (StartupNotice)
            {
                case StartupNotices.SingleTenantScope:
                    new SysLogModel(
                        context: context,
                        method: "",
                        message: $"BackgroundJob processes only default tenant."
                            + $" It does not process background jobs for other tenants."
                            + $" MultiTenant.DefaultTenantId={s_defaultTenantId}.",
                        sysLogType: SysLogModel.SysLogTypes.Info);
                    break;
                case StartupNotices.InvalidDefaultTenantId:
                    new SysLogModel(
                        context: context,
                        method: "",
                        message: $"BackgroundJobs are not processed:"
                            + $" MultiTenant is unavailable and the default tenant is invalid"
                            + $" (must be 1 or greater)."
                            + $" MultiTenant.DefaultTenantId={s_defaultTenantId}.",
                        sysLogType: SysLogModel.SysLogTypes.Warning);
                    break;
                case StartupNotices.None:
                default:
                    break;
            }
        }

        private static void WriteWorkerCountNotice()
        {
            var context = NoticeContext(nameof(Evaluate));
            if (WorkerCountCorrectedNotice)
            {
                new SysLogModel(
                    context: context,
                    method: "",
                    message: $"BackgroundJobs.WorkerCount={s_configuredWorkerCount} is out of range."
                        + $" Corrected to {s_verifiedWorkerCount}."
                        + $" The safe limit is {s_workerCountLimit},"
                        + $" which equals the effective thread pool size of this node.",
                    sysLogType: SysLogModel.SysLogTypes.Warning);
            }
            if (WorkerCountIgnoredNotice)
            {
                new SysLogModel(
                    context: context,
                    method: "",
                    message: $"WorkerCount is set to {s_configuredWorkerCount},"
                        + $" but ignored because the license is single-tenant."
                        + $" Effective worker count is {s_effectiveWorkerCount}.",
                    sysLogType: SysLogModel.SysLogTypes.Info);
            }
        }

        internal static void WriteStartupSummary(
            IEnumerable<string> expectedWorkerNames,
            int? threadPoolSize)
        {
            if (s_queueEnabled == false)
            {
                return;
            }
            var context = NoticeContext(nameof(WriteStartupSummary));
            var workerNames = new[]
                {
                    BackgroundJobDispatcher.GetParam(workerNumber: 1).JobKey.Name
                }
                .Concat(expectedWorkerNames ?? Enumerable.Empty<string>())
                .ToList();
            var clusteringEnabled = Parameters.Quartz?.Clustering?.Enabled ?? false;
            var threadPoolNote = clusteringEnabled
                ? "from Quartz.Clustering.MaxConcurrency"
                : "fixed; cannot be raised by configuration while clustering is disabled";
            new SysLogModel(
                context: context,
                method: "",
                message: $"BackgroundJobs worker configuration:"
                    + $" ConfiguredWorkerCount={ConfiguredWorkerCount?.ToString() ?? "(not set)"},"
                    + $" EffectiveWorkerCount={s_effectiveWorkerCount},"
                    + $" MultiTenant={s_multiTenant},"
                    + $" DefaultTenantId={s_defaultTenantId},"
                    + $" ExpectedWorkerCount={workerNames.Count},"
                    + $" ExpectedWorkers=[{workerNames.Join(", ")}],"
                    + $" Clustering={clusteringEnabled},"
                    + $" ThreadPoolSize={threadPoolSize?.ToString() ?? "(unknown)"} ({threadPoolNote}),"
                    + $" WorkerCountLimit={s_workerCountLimit},"
                    + $" SchedulerName={CustomQuartzHostedService.Scheduler.SchedulerName}",
                sysLogType: SysLogModel.SysLogTypes.Info);
        }

        internal static void WriteThreadPoolSharingNotice(
            IEnumerable<IExecutionTimerBaseParam> registeredTimers,
            int? threadPoolSize)
        {
            if (s_queueEnabled == false)
            {
                return;
            }
            var timers = registeredTimers ?? Enumerable.Empty<IExecutionTimerBaseParam>();
            var otherIntervalTimerCount = timers
                .Count(timer => timer.TimeList == null
                    && timer.JobType != typeof(BackgroundJobDispatcher)
                    && timer.Enabled);
            if (s_effectiveWorkerCount + otherIntervalTimerCount <= s_workerCountLimit)
            {
                return;
            }
            var context = NoticeContext(nameof(WriteThreadPoolSharingNotice));
            var clusteringNote = Parameters.Quartz?.Clustering?.Enabled == true
                ? string.Empty
                : " The thread pool size cannot be raised by configuration"
                    + " while clustering is disabled.";
            new SysLogModel(
                context: context,
                method: "",
                message: $"BackgroundJob workers share the thread pool with other timers,"
                    + $" so firings may be delayed."
                    + $" EffectiveWorkerCount={s_effectiveWorkerCount},"
                    + $" OtherIntervalTimerCount={otherIntervalTimerCount},"
                    + $" ThreadPoolSize={threadPoolSize?.ToString() ?? "(unknown)"},"
                    + $" WorkerCountLimit={s_workerCountLimit}."
                    + clusteringNote,
                sysLogType: SysLogModel.SysLogTypes.Warning);
        }

        internal static int VerifyWorkerCount(
            int? configured,
            int limit,
            out bool corrected)
        {
            corrected = false;
            var verified = configured ?? 1;
            if (verified <= 0)
            {
                verified = 1;
                corrected = true;
            }
            else if (verified > limit)
            {
                verified = limit;
                corrected = true;
            }
            return verified;
        }

        private static int ResolveWorkerCountLimit()
        {
            var clustering = Parameters.Quartz?.Clustering;
            if (clustering?.Enabled != true)
            {
                return DefaultThreadPoolSize;
            }
            return clustering.MaxConcurrency > 0
                ? clustering.MaxConcurrency
                : DefaultThreadPoolSize;
        }

        private static void EvaluateWorkerCount()
        {
            var configured = Parameters.BackgroundJobs?.WorkerCount;
            s_workerCountSpecified = configured.HasValue;
            s_configuredWorkerCount = configured ?? 0;
            s_workerCountLimit = ResolveWorkerCountLimit();
            var verified = VerifyWorkerCount(
                configured: configured,
                limit: s_workerCountLimit,
                corrected: out var corrected);
            s_workerCountCorrected = corrected;
            s_verifiedWorkerCount = verified;
            s_workerCountIgnored = s_multiTenant == false && verified >= 2;
            s_effectiveWorkerCount = s_multiTenant
                ? verified
                : 1;
        }
    }
}
