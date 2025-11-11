using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    public class CustomQuartzHostedService : IHostedService
    {
        private static readonly IScheduler scheduler;
        private readonly ILogger<CustomQuartzHostedService> logger;

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

        public CustomQuartzHostedService(ILogger<CustomQuartzHostedService> logger)
        {
            this.logger = logger;
        }

        public static IScheduler Scheduler { get => scheduler; }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await scheduler?.Start(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await scheduler?.Shutdown(cancellationToken);
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
