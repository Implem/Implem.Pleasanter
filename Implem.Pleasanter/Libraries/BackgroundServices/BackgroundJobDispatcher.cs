using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using Quartz;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    public class BackgroundJobDispatcher : ClusterExecutionTimerBase
    {
        private static volatile bool s_recoveredOnce = false;
        private static readonly Dictionary<string, IBackgroundJobHandler> s_handlers
            = new Dictionary<string, IBackgroundJobHandler>();

        static BackgroundJobDispatcher()
        {
            RegisterHandler(
                jobType: "Export",
                handler: new ExportJobHandler());
        }

        public class Param : IExecutionTimerBaseParam
        {
            private static readonly JobKey s_jobKey
                = new JobKey(
                    "BackgroundJobDispatcher",
                    "ExecutionTimerBase");
            public Type JobType => typeof(BackgroundJobDispatcher);
            public IEnumerable<string> TimeList => null;
            public bool Enabled => BackgroundJobQueue.BackgroundQueueEnabled();
            public JobKey JobKey => s_jobKey;
            public string JobName => "BackgroundJobDispatcherService";

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

        public override async Task Execute(IJobExecutionContext quartzContext)
        {
            await Task.Run(
                async () =>
                {
                    var context = CreateContext();
                    try
                    {
                        if (s_recoveredOnce == false)
                        {
                            BackgroundJobQueue.RecoverStuckJobs(context: context);
                            s_recoveredOnce = true;
                        }
                        BackgroundJobQueue.WarnTimedOutRunningJobs(context: context);
                        await ExecuteOneJob(context: context);
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
                },
                quartzContext.CancellationToken);
        }

        internal static async Task ExecuteOneJob(
            Context context,
            bool ignoreRunningOverdueTenantLocks = false,
            int targetTenantId = 0)
        {
            var model = default(BackgroundJobModel);
            try
            {
                model = BackgroundJobQueue.Dequeue(
                    context: context,
                    ignoreRunningOverdueTenantLocks: ignoreRunningOverdueTenantLocks,
                    targetTenantId: targetTenantId);
                if (model == null) return;
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
                if (model != null)
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
                else
                {
                    new SysLogModel(
                        context: new Context(
                            tenantId: context.TenantId,
                            request: false,
                            context: context)
                        {
                            Controller = nameof(BackgroundJobDispatcher),
                            Action = nameof(ExecuteOneJob)
                        },
                        e: e,
                        extendedErrorMessage: "BackgroundJobDispatcher Exception");
                }
            }
        }

        internal static IExecutionTimerBaseParam GetParam()
        {
            return new Param();
        }
    }
}
