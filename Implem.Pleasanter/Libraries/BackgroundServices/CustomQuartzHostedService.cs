using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Settings;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    public class CustomQuartzHostedService(
        ILogger<CustomQuartzHostedService> logger,
        IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
    {
        private static readonly IScheduler scheduler;

        static CustomQuartzHostedService()
        {
            var enableClustering = Parameters.Quartz?.Clustering?.Enabled ?? false;
            if (enableClustering)
            {
                scheduler = CreateScheduler().GetAwaiter().GetResult();
            }
            else
            {
                scheduler = StdSchedulerFactory.GetDefaultScheduler().GetAwaiter().GetResult();
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!await WaitForWarmupAsync(stoppingToken: stoppingToken))
            {
                return;
            }
            try
            {
                await new TimerBackground().InitAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Timer Schedule Registration Failed");
            }
            try
            {
                await BackgroundServerScriptUtilities.InitScheduleAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Background Server Script Schedule Registration Failed");
            }
            try
            {
                await scheduler.Start(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                logger.LogWarning("Quartz Scheduler Start Canceled");
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Quartz Scheduler Failed To Start");
                hostApplicationLifetime.StopApplication();
            }
        }

        public static IScheduler Scheduler { get => scheduler; }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            BackgroundJobTargetTenants.RequestStop();
            await base.StopAsync(cancellationToken);
            await scheduler.Shutdown(cancellationToken);
        }

        private async Task<bool> WaitForWarmupAsync(CancellationToken stoppingToken)
        {
            try
            {
                await ApplicationWarmupHostedService.WaitForCompletionAsync(
                    cancellationToken: stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                logger.LogWarning("Quartz Scheduler Not Started: Warmup Canceled");
                return false;
            }
            if (ApplicationWarmupHostedService.CurrentStatus != WarmupStatus.Completed)
            {
                logger.LogWarning(
                    "Quartz Scheduler Not Started: WarmupStatus={WarmupStatus}",
                    ApplicationWarmupHostedService.CurrentStatus);
                return false;
            }
            return true;
        }

        private static async Task<IScheduler> CreateScheduler()
        {
            var properties = new NameValueCollection();

            var quartzConfig = Parameters.Quartz ?? new ParameterAccessor.Parts.Quartz();
            var clusteringConfig = quartzConfig.Clustering ?? new ParameterAccessor.Parts.QuartzClustering();

            properties["quartz.scheduler.instanceName"] = clusteringConfig.SchedulerName ?? "PleasanterScheduler";
            properties["quartz.scheduler.instanceId"] = clusteringConfig.InstanceId ?? "AUTO";

            properties["quartz.threadPool.type"] = "Quartz.Simpl.DefaultThreadPool, Quartz";
            properties["quartz.threadPool.maxConcurrency"] = (clusteringConfig.MaxConcurrency > 0 ? clusteringConfig.MaxConcurrency : 10).ToString();
            properties["quartz.threadPool.threadPriority"] = clusteringConfig.ThreadPriority ?? "Normal";

            if (clusteringConfig.Enabled)
            {
                SetupClusteredJobStore(properties, clusteringConfig);
            }
            else
            {
                SetupSingleNodeJobStore(properties);
            }

            var factory = new StdSchedulerFactory(properties);
            return await factory.GetScheduler();
        }

        private static void SetupSingleNodeJobStore(NameValueCollection properties)
        {
            properties["quartz.jobStore.type"] = "Quartz.Simpl.RAMJobStore, Quartz";
        }

        private static void SetupClusteredJobStore(NameValueCollection properties, ParameterAccessor.Parts.QuartzClustering config)
        {
            properties["quartz.jobStore.type"] = "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz";
            properties["quartz.jobStore.driverDelegateType"] = GetDriverDelegateType();
            properties["quartz.jobStore.tablePrefix"] = config.TablePrefix ?? "QRTZ_";
            properties["quartz.jobStore.dataSource"] = "default";
            properties["quartz.jobStore.useProperties"] = "true";

            properties["quartz.jobStore.clustered"] = "true";
            properties["quartz.jobStore.clusterCheckinInterval"] = (config.CheckinInterval > 0 ? config.CheckinInterval : 15000).ToString();
            properties["quartz.jobStore.clusterCheckinMisfireThreshold"] = (config.MaxMisfireThreshold > 0 ? config.MaxMisfireThreshold : 60000).ToString();

            properties["quartz.jobStore.misfireThreshold"] = (config.MaxMisfireThreshold > 0 ? config.MaxMisfireThreshold : 60000).ToString();

            properties["quartz.serializer.type"] = config.Serializer ?? "json";

            properties["quartz.dataSource.default.connectionString"] = Parameters.Rds.UserConnectionString;
            properties["quartz.dataSource.default.provider"] = GetQuartzDataSourceProvider();
            properties["quartz.dataSource.default.maxConnections"] = "20";
        }

        private static string GetQuartzDataSourceProvider()
        {
            var dbms = Parameters.Rds.Dbms;
            return dbms switch
            {
                "SQLServer" => "SqlServer",
                "PostgreSQL" => "Npgsql",
                "MySQL" => "MySqlConnector",
                _ => "SqlServer"
            };
        }

        private static string GetDriverDelegateType()
        {
            var dbms = Parameters.Rds.Dbms;
            return dbms switch
            {
                "SQLServer" => "Quartz.Impl.AdoJobStore.SqlServerDelegate, Quartz",
                "PostgreSQL" => "Quartz.Impl.AdoJobStore.PostgreSQLDelegate, Quartz",
                "MySQL" => "Quartz.Impl.AdoJobStore.MySQLDelegate, Quartz",
                _ => "Quartz.Impl.AdoJobStore.SqlServerDelegate, Quartz",
            };
        }
    }
}
