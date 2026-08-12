using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Initializers;
using Implem.Pleasanter.Libraries.Migrators;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    public enum WarmupStatus
    {
        NotStarted,
        InProgress,
        Completed,
        Failed,
        Canceled,
        TimedOut
    }

    public class ApplicationWarmupHostedService(
        ILogger<ApplicationWarmupHostedService> logger,
        IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
    {
        private static int s_currentStatus = (int)WarmupStatus.NotStarted;
        private static string s_currentStepName = string.Empty;
        private static readonly TaskCompletionSource s_completionSource =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public static WarmupStatus CurrentStatus
        {
            get => (WarmupStatus)Volatile.Read(ref s_currentStatus);
            private set => Volatile.Write(ref s_currentStatus, (int)value);
        }

        public static Task WaitForCompletionAsync(CancellationToken cancellationToken = default)
        {
            return s_completionSource.Task.WaitAsync(cancellationToken);
        }

        public static string CurrentStepName
        {
            get => Volatile.Read(ref s_currentStepName) ?? string.Empty;
            private set => Volatile.Write(ref s_currentStepName, value ?? string.Empty);
        }

        private const string StartupMigrationsStepName = "StartupMigrations";
        private const string StartupMigrationsLockName = "startup-migrations";

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            CurrentStatus = WarmupStatus.InProgress;
            CurrentStepName = string.Empty;
            logger.LogInformation("Application Warmup Started");
            var context = CreateWarmupContext();
            var startedAtUtc = DateTime.UtcNow;
            var totalTimeout = GetWarmupTimeout();
            SysLogModel log = null;
            try
            {
                log = new SysLogModel(
                    context: context,
                    method: null,
                    message: Parameters.GetLicenseInfo().ToJson());
                var warmupSteps = new List<(string Name, Action Step)>
                {
                    ("TenantInitializer.Initialize", () => TenantInitializer.Initialize()),
                    ("ExtensionInitializer.Initialize", () => ExtensionInitializer.Initialize(context: context)),
                    ("UsersInitializer.Initialize", () => UsersInitializer.Initialize(context: context)),
                    ("ItemsInitializer.Initialize", () => ItemsInitializer.Initialize(context: context)),
                    (StartupMigrationsStepName, () => { /* StartupMigrations は別処理で実行するため、ここでは使用しない */ }),
                    ("StatusesInitializer.Initialize", () => StatusesInitializer.Initialize(context: context)),
                    ("NotificationInitializer.Initialize", () => NotificationInitializer.Initialize()),
                    ("SiteInfo.Refresh", () => SiteInfo.Refresh(context: context))
                };
                foreach (var (name, step) in warmupSteps)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    CurrentStepName = name;
                    if (name == StartupMigrationsStepName)
                    {
                        startedAtUtc = await RunStartupMigrationsAsync(
                            context: context,
                            startedAtUtc: startedAtUtc,
                            totalTimeout: totalTimeout,
                            cancellationToken: cancellationToken);
                    }
                    else
                    {
                        await RunStepAsync(
                            name: name,
                            step: step,
                            startedAtUtc: startedAtUtc,
                            totalTimeout: totalTimeout,
                            waitForCompletionAfterTimeout: false,
                            cancellationToken: cancellationToken);
                    }
                }
                CurrentStepName = string.Empty;
                CurrentStatus = WarmupStatus.Completed;
                s_completionSource.TrySetResult();
                logger.LogInformation("Application Warmup Finished");
            }
            catch (OperationCanceledException)
            {
                await StopApplicationAfterWarmupStoppedAsync(
                    status: WarmupStatus.Canceled,
                    context: context,
                    delayBeforeStop: false);
                throw;
            }
            catch (TimeoutException ex)
            {
                await StopApplicationAfterWarmupStoppedAsync(
                    status: WarmupStatus.TimedOut,
                    context: context,
                    exception: ex);
                throw;
            }
            catch (Exception ex)
            {
                await StopApplicationAfterWarmupStoppedAsync(
                    status: WarmupStatus.Failed,
                    context: context,
                    exception: ex);
                throw;
            }
            finally
            {
                try
                {
                    log?.Finish(context: context);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Application Warmup Logging Failed");
                }
            }
        }

        private async Task<DateTime> RunStartupMigrationsAsync(
            Context context,
            DateTime startedAtUtc,
            TimeSpan totalTimeout,
            CancellationToken cancellationToken)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation(
                    "Application Warmup Migration: acquiring '{LockName}' lock",
                    StartupMigrationsLockName);
            }
            var lockWaitStartedAtUtc = DateTime.UtcNow;
            using var startupLock = await Task.Run(
                () => DistributedLock.Acquire(
                    context: context,
                    name: StartupMigrationsLockName,
                    timeoutSeconds: Parameters.BackgroundService?.WarmupTimeoutSeconds ?? 0),
                cancellationToken);
            startedAtUtc = startedAtUtc.Add(DateTime.UtcNow - lockWaitStartedAtUtc);
            if (!startupLock.Acquired)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                {
                    logger.LogWarning(
                        "Application Warmup Migration: could not acquire '{LockName}' lock within timeout",
                        StartupMigrationsLockName);
                }
                throw new TimeoutException(
                    $"Application warmup could not acquire the '{StartupMigrationsLockName}' lock.");
            }
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation(
                    "Application Warmup Migration: acquired '{LockName}' lock, running migrators",
                    StartupMigrationsLockName);
            }
            await RunStepAsync(
                name: StartupMigrationsStepName,
                step: () =>
                {
                    StatusesMigrator.Migrate(context: context);
                    SiteSettingsMigrator.Migrate(context: context);
                },
                startedAtUtc: startedAtUtc,
                totalTimeout: totalTimeout,
                waitForCompletionAfterTimeout: true,
                cancellationToken: cancellationToken);
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation(
                    "Application Warmup Migration: completed migrators, releasing '{LockName}' lock",
                    StartupMigrationsLockName);
            }
            return startedAtUtc;
        }

        private async Task RunStepAsync(
            string name,
            Action step,
            DateTime startedAtUtc,
            TimeSpan totalTimeout,
            bool waitForCompletionAfterTimeout,
            CancellationToken cancellationToken)
        {
            var remaining = GetRemainingTimeout(startedAtUtc, totalTimeout);
            if (remaining != Timeout.InfiniteTimeSpan && remaining <= TimeSpan.Zero)
            {
                throw new TimeoutException(
                    $"Application warmup timed out before step '{name}' started. " +
                    $"TimeoutSeconds={Parameters.BackgroundService.WarmupTimeoutSeconds}");
            }
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation(
                    "Application Warmup Step Started: {WarmupStep}, Remaining={Remaining}",
                    name,
                    remaining == Timeout.InfiniteTimeSpan ? "Infinite" : remaining.ToString());
            }
            var task = Task.Run(step, cancellationToken);
            try
            {
                if (remaining == Timeout.InfiniteTimeSpan)
                {
                    await task;
                }
                else
                {
                    await task.WaitAsync(remaining, cancellationToken);
                }
                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation(
                        "Application Warmup Step Finished: {WarmupStep}",
                        name);
                }
            }
            catch (TimeoutException ex)
            {
                if (waitForCompletionAfterTimeout)
                {
                    await WaitStepCompletionAsync(
                        task: task,
                        name: name,
                        reason: "timed out");
                }
                throw new TimeoutException(
                    $"Application warmup timed out at step '{name}'. " +
                    $"Elapsed={(DateTime.UtcNow - startedAtUtc).TotalSeconds:F0}s, " +
                    $"TimeoutSeconds={Parameters.BackgroundService.WarmupTimeoutSeconds}",
                    ex);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                if (waitForCompletionAfterTimeout)
                {
                    await WaitStepCompletionAsync(
                        task: task,
                        name: name,
                        reason: "canceled");
                }
                throw;
            }
        }

        private async Task WaitStepCompletionAsync(Task task, string name, string reason)
        {
            if (!task.IsCompleted && logger.IsEnabled(LogLevel.Warning))
            {
                logger.LogWarning(
                    "Application Warmup Step Waiting For Completion: {WarmupStep}, Reason={Reason}",
                    name,
                    reason);
            }
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Application Warmup Step Failed While Waiting For Completion: {WarmupStep}",
                    name);
                throw;
            }
        }

        private static TimeSpan GetWarmupTimeout()
        {
            var seconds = Parameters.BackgroundService?.WarmupTimeoutSeconds ?? 0;
            return seconds <= 0
                ? Timeout.InfiniteTimeSpan
                : TimeSpan.FromSeconds(seconds);
        }

        private async Task StopApplicationAfterWarmupStoppedAsync(
            WarmupStatus status,
            Context context,
            Exception exception = null,
            bool delayBeforeStop = true)
        {
            CurrentStatus = status;
            s_completionSource.TrySetResult();
            LogWarmupStopped(status: status, exception: exception);
            if (exception != null)
            {
                TryWriteFailureLog(context, exception);
            }
            if (delayBeforeStop)
            {
                await DelayBeforeStopApplicationAsync(status: status);
            }
            hostApplicationLifetime.StopApplication();
        }

        private void LogWarmupStopped(WarmupStatus status, Exception exception)
        {
            switch (status)
            {
                case WarmupStatus.Canceled:
                    logger.LogWarning(
                        "Application Warmup Canceled at step {WarmupStep}",
                        CurrentStepName);
                    break;
                case WarmupStatus.TimedOut:
                    logger.LogError(
                        exception,
                        "Application Warmup Timed Out at step {WarmupStep}",
                        CurrentStepName);
                    break;
                default:
                    logger.LogError(
                        exception: exception,
                        message: "Application Warmup Failed");
                    break;
            }
        }

        private async Task DelayBeforeStopApplicationAsync(WarmupStatus status)
        {
            var seconds = Parameters.BackgroundService?.WarmupFailureShutdownDelaySeconds ?? 0;
            if (seconds <= 0)
            {
                return;
            }
            logger.LogWarning(
                "Application Warmup Stop Delayed: Status={WarmupStatus}, Step={WarmupStep}, DelaySeconds={DelaySeconds}",
                status,
                CurrentStepName,
                seconds);
            await Task.Delay(
                delay: TimeSpan.FromSeconds(seconds),
                cancellationToken: CancellationToken.None);
        }

        private static TimeSpan GetRemainingTimeout(
            DateTime startedAtUtc,
            TimeSpan totalTimeout)
        {
            if (totalTimeout == Timeout.InfiniteTimeSpan)
            {
                return Timeout.InfiniteTimeSpan;
            }
            var elapsed = DateTime.UtcNow - startedAtUtc;
            var remaining = totalTimeout - elapsed;
            return remaining > TimeSpan.Zero
                ? remaining
                : TimeSpan.Zero;
        }

        private void TryWriteFailureLog(Context context, Exception ex)
        {
            try
            {
                var log = new SysLogModel(context: context, e: ex);
                log.Finish(context: context);
            }
            catch (Exception innerEx)
            {
                logger.LogError(innerEx, "Application Warmup Logging Failed");
            }
        }

        private static Context CreateWarmupContext()
        {
            return new Context(
                request: false,
                sessionStatus: false,
                sessionData: false,
                user: false,
                item: false)
            {
                Controller = "ApplicationWarmup",
                Action = "Application_Start",
                Id = 0,
                TenantId = 0
            };
        }
    }
}
