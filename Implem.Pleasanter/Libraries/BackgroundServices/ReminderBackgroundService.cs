using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Implem.Pleasanter.Libraries.BackgroundServices
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
            new SysLogModel(
                context: context,
                method: nameof(ExecuteAsync),
                message: "ReminderBackgroundService ExecuteAsync() Started",
                sysLogType: SysLogModel.SysLogTypes.Info);
            var exceptionCount = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Run(() => ReminderScheduleUtilities.Remind(context: context));
                    exceptionCount = 0;
                }
                catch (OperationCanceledException e)
                {
                    new SysLogModel(
                        context: context,
                        e: e,
                        extendedErrorMessage: "ReminderBackgroundService Canceled");
                    break;
                }
                catch (Exception e)
                {
                    exceptionCount++;
                    new SysLogModel(
                        context: context,
                        e: e,
                        extendedErrorMessage:  $"ReminderBackgroundService Exception Count={exceptionCount}");
                    if (exceptionCount > Parameters.BackgroundService.ReminderIgnoreConsecutiveExceptionCount)
                    {
                        throw;
                    }
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
            new SysLogModel(
                context: context,
                method: nameof(ExecuteAsync),
                message: "ReminderBackgroundService ExecuteAsync() Stopped",
                sysLogType: SysLogModel.SysLogTypes.Info);
        }
    }
}
