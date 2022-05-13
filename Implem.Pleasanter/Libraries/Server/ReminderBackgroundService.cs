using Implem.DefinitionAccessor;
using Implem.Pleasanter.Controllers;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Implem.Pleasanter.Libraries.Server
{
    class ReminderBackgroundService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!Parameters.Reminder.Enabled)
            {
                return;
            }
            var context = new Context(
                request: false,
                sessionStatus: false,
                sessionData: false,
                user: false,
                item: false);
            var exceptionCount = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                    ReminderScheduleUtilities.Remind(context: context);
                    exceptionCount = 0;
                }
                catch (OperationCanceledException e)
                {
                    new SysLogModel(context, e, "Reminder Canceled");
                    break;
                }
                catch (Exception e)
                {
                    const int MAX_INTERVAL_MINUTES = 30;
                    exceptionCount++;
                    new SysLogModel(context, e, $"Reminder Exception Count={exceptionCount}");
                    var waitMinutes = Math.Min(exceptionCount, MAX_INTERVAL_MINUTES);
                    await Task.Delay(waitMinutes, stoppingToken);
                }
            }
        }
    }
}
