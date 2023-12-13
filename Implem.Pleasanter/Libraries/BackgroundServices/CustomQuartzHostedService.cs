using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using System.Threading;
using System.Threading.Tasks;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    public class CustomQuartzHostedService : IHostedService
    {
        private static readonly IScheduler scheduler;
        private readonly ILogger<CustomQuartzHostedService> logger;

        static CustomQuartzHostedService()
        {
            scheduler = StdSchedulerFactory.GetDefaultScheduler().GetAwaiter().GetResult();
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
    }
}
