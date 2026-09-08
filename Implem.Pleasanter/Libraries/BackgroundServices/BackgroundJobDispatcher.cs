using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using Quartz;
using Context = Implem.Pleasanter.Libraries.Requests.Context;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    public class BackgroundJobDispatcher : ClusterExecutionTimerBase
    {
        internal const string WorkerJobGroup = "BackgroundJobDispatcherWorkers";

        internal const string WorkerNumberKey = "workerNumber";

        internal enum WorkerNumberNotices
        {
            None,
            NotNumeric,
            OutOfRange
        }

        internal enum RecoveryStates
        {
            NotStarted,
            Running,
            Completed
        }

        private static readonly Dictionary<string, IBackgroundJobHandler> s_handlers
            = new Dictionary<string, IBackgroundJobHandler>();
        private static readonly ConcurrentDictionary<Task, byte> s_runningJobs
            = new ConcurrentDictionary<Task, byte>();
        private static readonly ConcurrentDictionary<int, RecoveryStates> s_recoveryStateByWorker
            = new ConcurrentDictionary<int, RecoveryStates>();

        static BackgroundJobDispatcher()
        {
            RegisterHandler(
                jobType: BackgroundJobTypes.Export,
                handler: new ExportJobHandler());
            RegisterHandler(
                jobType: BackgroundJobTypes.Import,
                handler: new ImportJobHandler());
            RegisterHandler(
                jobType: BackgroundJobTypes.AiConnectSync,
                handler: new AiConnectSyncJobHandler());
            RegisterHandler(
                jobType: BackgroundJobTypes.AiConnectResync,
                handler: new AiConnectResyncJobHandler());
            RegisterHandler(
                jobType: BackgroundJobTypes.AiConnectIndexCheck,
                handler: new AiConnectIndexCheckJobHandler());
            RegisterHandler(
                jobType: BackgroundJobTypes.AiConnectDelete,
                handler: new AiConnectDeleteJobHandler());
        }

        public class Param : IExecutionTimerBaseParam
        {
            private readonly int WorkerNumber;
            private readonly JobKey Key;

            public Param(int workerNumber)
            {
                WorkerNumber = workerNumber;
                Key = workerNumber <= 1
                    ? new JobKey(
                        nameof(BackgroundJobDispatcher),
                        "ExecutionTimerBase")
                    : new JobKey(
                        $"{nameof(BackgroundJobDispatcher)}_{workerNumber - 1}",
                        WorkerJobGroup);
            }

            public Type JobType => typeof(BackgroundJobDispatcher);
            public IEnumerable<string> TimeList => null;
            public bool Enabled => BackgroundJobQueue.BackgroundQueueEnabled();
            public JobKey JobKey => Key;
            public string JobName => $"BackgroundJobDispatcherServiceWorker{WorkerNumber}";

            public IEnumerable<KeyValuePair<string, string>> JobData => WorkerNumber <= 1
                ? null
                : new[]
                {
                    new KeyValuePair<string, string>(
                        WorkerNumberKey,
                        WorkerNumber.ToString())
                };

            public async Task<bool> SetCustomTimer(IScheduler scheduler)
            {
                var triggerKey = TimerTriggerRegistrar.SimpleTriggerKey(JobKey);
                var trigger = TriggerBuilder.Create()
                    .WithIdentity(triggerKey)
                    .ForJob(JobKey)
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(
                            Parameters.BackgroundJobs?.BackgroundJobDispatcherInterval ?? 60)
                        .RepeatForever())
                    .Build();
                await TimerTriggerRegistrar.EnsureTriggerAsync(
                    scheduler: scheduler,
                    trigger: trigger);
                await TimerTriggerRegistrar.CleanupUnexpectedTriggersAsync(
                    scheduler: scheduler,
                    jobKey: JobKey,
                    expectedKeys: [triggerKey]);
                return true;
            }
        }

        public static void RegisterHandler(
            string jobType,
            IBackgroundJobHandler handler)
        {
            s_handlers[jobType] = handler;
        }

        public override Task Execute(IJobExecutionContext quartzContext)
        {
            if (BackgroundJobTargetTenants.ProcessingEnabled == false) return Task.CompletedTask;
            var effectiveWorkerCount = BackgroundJobTargetTenants.EffectiveWorkerCount;
            var workerNumber = ResolveWorkerNumber(
                quartzContext: quartzContext,
                effectiveWorkerCount: effectiveWorkerCount);
            if (workerNumber <= 0)
            {
                return Task.CompletedTask;
            }
            Dispatch(
                workerNumber: workerNumber,
                effectiveWorkerCount: effectiveWorkerCount);
            return Task.CompletedTask;
        }

        internal static BackgroundJobModel ClaimJob(
            Context context,
            bool ignoreRunningOverdueTenantLocks = false,
            int targetTenantId = 0)
        {
            return BackgroundJobQueue.Dequeue(
                context: context,
                ignoreRunningOverdueTenantLocks: ignoreRunningOverdueTenantLocks,
                targetTenantId: targetTenantId);
        }

        internal static async Task RunJob(
            Context context,
            BackgroundJobModel model)
        {
            try
            {
                if (s_handlers.TryGetValue(
                    model.JobType,
                    out var handler) == false)
                {
                    BackgroundJobQueue.Fail(
                        context: context,
                        model: model,
                        errorMessage: Displays.Get(
                            id: "BackgroundJobNoHandler",
                            language: Parameters.BackgroundJobs?.FallbackLanguage
                                ?? Parameters.Service.DefaultLanguage,
                            data: model.JobType));
                    return;
                }
                await handler.ExecuteAsync(
                    context: context,
                    backgroundJobModel: model);
                BackgroundJobQueue.Complete(
                    context: context,
                    model: model,
                    resultData: model.File,
                    resultMessage: model.ResultMessage);
            }
            catch (Exception e)
            {
                BackgroundJobQueue.Fail(
                    context: context,
                    model: model,
                    errorMessage: e.Message);
                new SysLogModel(
                    context: BackgroundJobQueue.CreateSysLogContext(
                        context: context,
                        model: model),
                    e: e,
                    extendedErrorMessage: "BackgroundJobDispatcher Exception"
                        + $": BackgroundJobId={model.BackgroundJobId}"
                        + $", JobType={model.JobType}");
            }
        }

        internal static IExecutionTimerBaseParam GetParam(int workerNumber)
        {
            return new Param(workerNumber: workerNumber);
        }

        private void Dispatch(
            int workerNumber,
            int effectiveWorkerCount)
        {
            if (BackgroundJobTargetTenants.StopRequested) return;
            var context = CreateContext();
            var startedCount = 0;
            var scope = new BackgroundJobQueue.TenantScope
            {
                TargetTenantId = BackgroundJobTargetTenants.ScopedTenantId,
                WorkerCount = effectiveWorkerCount,
                WorkerNumber = workerNumber
            };
            try
            {
                if (BackgroundJobTargetTenants.StopRequested == false
                    && TryBeginRecovery(workerNumber: workerNumber))
                {
                    try
                    {
                        BackgroundJobQueue.RecoverStuckJobs(
                            context: context,
                            scope: scope);
                        CompleteRecovery(workerNumber: workerNumber);
                    }
                    catch
                    {
                        AbortRecovery(workerNumber: workerNumber);
                        throw;
                    }
                }
                if (BackgroundJobTargetTenants.StopRequested == false)
                {
                    BackgroundJobQueue.FailStalePreparingJobs(
                        context: context,
                        scope: scope);
                }
                var tenantIds = ResolveTargetTenantIds(
                    context: context,
                    scope: scope);
                for (var index = 0; index < tenantIds.Count; index++)
                {
                    if (BackgroundJobTargetTenants.StopRequested)
                    {
                        new SysLogModel(
                            context: context,
                            method: nameof(Dispatch),
                            message: $"BackgroundJobDispatcher Stopped:"
                                + $" StartedJobsCount={startedCount},"                      // 起動したジョブ件数
                                + $" UnprocessedTenantsCount={tenantIds.Count - index},"    // 未処理テナント件数
                                + $" WorkerNumber={workerNumber}",                          // ワーカー番号
                            sysLogType: SysLogModel.SysLogTypes.Info);
                        return;
                    }
                    var tenantId = tenantIds[index];
                    try
                    {
                        var tenantContext = new Context(
                            tenantId: tenantId,
                            request: false,
                            context: context);
                        var model = ClaimJob(
                            context: tenantContext,
                            ignoreRunningOverdueTenantLocks: false,
                            targetTenantId: tenantId);
                        if (model == null) continue;
                        StartJobTask(
                            context: tenantContext,
                            model: model);
                        startedCount++;
                    }
                    catch (Exception e)
                    {
                        new SysLogModel(
                            context: new Context(
                                tenantId: tenantId,
                                request: false,
                                context: context)
                            {
                                Controller = nameof(BackgroundJobDispatcher),
                                Action = nameof(Dispatch)
                            },
                            e: e,
                            extendedErrorMessage: $"BackgroundJobDispatcher Exception: TenantId={tenantId}");
                    }
                }
                new SysLogModel(
                    context: context,
                    method: nameof(Dispatch),
                    message: $"BackgroundJobDispatcher Completed:"
                        + $" TargetTenantIdsCount={tenantIds.Count},"           // 対象テナント一覧件数
                        + $" StartedJobsCount={startedCount},"                  // 起動したジョブ件数
                        + $" RunningJobsCount={s_runningJobs.Count},"           // 実行中ジョブ件数
                        + $" WorkerNumber={workerNumber}",                      // ワーカー番号
                    sysLogType: SysLogModel.SysLogTypes.Info);
            }
            catch (Exception e)
            {
                new SysLogModel(
                    context: new Context(
                        tenantId: context.TenantId,
                        request: false,
                        context: context)
                    {
                        Controller = nameof(BackgroundJobDispatcher),
                        Action = nameof(Execute)
                    },
                    e: e,
                    extendedErrorMessage: "BackgroundJobDispatcher Exception");
            }
        }

        private static void StartJobTask(
            Context context,
            BackgroundJobModel model)
        {
            var task = Task.Run(async () => await RunJob(
                context: context,
                model: model));
            s_runningJobs.TryAdd(task, 0);
            task.ContinueWith(t =>
            {
                s_runningJobs.TryRemove(t, out _);
                if (t.Exception != null)
                {
                    new SysLogModel(
                        context: new Context(
                            tenantId: context.TenantId,
                            request: false,
                            context: context)
                        {
                            Controller = nameof(BackgroundJobDispatcher),
                            Action = nameof(StartJobTask)
                        },
                        e: t.Exception,
                        extendedErrorMessage: "BackgroundJobDispatcher Exception");
                }
            });
        }

        private static List<int> ResolveTargetTenantIds(
            Context context,
            BackgroundJobQueue.TenantScope scope)
        {
            return BackgroundJobTargetTenants.MultiTenant
                ? BackgroundJobQueue.SelectTargetTenantIds(context: context, scope: scope)
                : new List<int> { BackgroundJobTargetTenants.DefaultTenantId };
        }

        internal static int ResolveWorkerNumber(
            string workerNumberText,
            int effectiveWorkerCount,
            out WorkerNumberNotices notice)
        {
            notice = WorkerNumberNotices.None;
            if (workerNumberText.IsNullOrEmpty())
            {
                return 1;
            }
            if (int.TryParse(workerNumberText, out var workerNumber) == false)
            {
                notice = WorkerNumberNotices.NotNumeric;
                return 1;
            }
            if (workerNumber < 1 || workerNumber > effectiveWorkerCount)
            {
                notice = WorkerNumberNotices.OutOfRange;
                return 0;
            }
            return workerNumber;
        }

        private int ResolveWorkerNumber(
            IJobExecutionContext quartzContext,
            int effectiveWorkerCount)
        {
            quartzContext.MergedJobDataMap.TryGetString(
                key: WorkerNumberKey,
                value: out var workerNumberText);
            var workerNumber = ResolveWorkerNumber(
                workerNumberText: workerNumberText,
                effectiveWorkerCount: effectiveWorkerCount,
                notice: out var notice);
            switch (notice)
            {
                case WorkerNumberNotices.NotNumeric:
                    new SysLogModel(
                        context: CreateContext(),
                        method: nameof(Execute),
                        message: "BackgroundJobDispatcher worker number is not numeric."
                            + " Treated as worker 1:"
                            + $" WorkerNumber={workerNumberText}",
                        sysLogType: SysLogModel.SysLogTypes.Warning);
                    break;
                case WorkerNumberNotices.OutOfRange:
                    new SysLogModel(
                        context: CreateContext(),
                        method: nameof(Execute),
                        message: "BackgroundJobDispatcher worker number is out of range."
                            + " This trigger does nothing:"
                            + $" WorkerNumber={workerNumberText},"
                            + $" EffectiveWorkerCount={effectiveWorkerCount}",
                        sysLogType: SysLogModel.SysLogTypes.Warning);
                    break;
                case WorkerNumberNotices.None:
                default:
                    break;
            }
            return workerNumber;
        }

        internal static bool TryBeginRecovery(int workerNumber)
        {
            return s_recoveryStateByWorker.TryAdd(
                key: workerNumber,
                value: RecoveryStates.Running);
        }

        internal static void CompleteRecovery(int workerNumber)
        {
            s_recoveryStateByWorker[workerNumber] = RecoveryStates.Completed;
        }

        internal static void AbortRecovery(int workerNumber)
        {
            s_recoveryStateByWorker.TryRemove(
                key: workerNumber,
                value: out _);
        }

        internal static RecoveryStates RecoveryState(int workerNumber)
        {
            return s_recoveryStateByWorker.TryGetValue(
                key: workerNumber,
                value: out var state)
                    ? state
                    : RecoveryStates.NotStarted;
        }

        internal static void ResetRecoveryStates()
        {
            s_recoveryStateByWorker.Clear();
        }
    }
}
