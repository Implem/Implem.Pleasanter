using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    /// <summary>
    /// Reminderを定期的に呼び出すBackgroundServiceクラス
    /// </summary>
    class ReminderBackgroundService : BackgroundService
    {
        /// <summary>
        /// 内部でループして定期的にReminderScheduleUtilities.Remind()を呼び出す。
        /// Pleasanter起動時にGeneric Hostにより自動的に呼ばれる。
        /// </summary>
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
                    // Remind()を直接呼ぶとASP.NETのWebリクエストのスレッドが待たされるのでRun()を使う。
                    await Task.Run(() => ReminderScheduleUtilities.Remind(context: context));
                    exceptionCount = 0;
                }
                catch (OperationCanceledException e)
                {
                    new SysLogModel(
                        context: context,
                        e: e,
                        extendedErrorMessage: "Reminder Canceled");
                    break;
                }
                catch (Exception e)
                {
                    exceptionCount++;
                    new SysLogModel(
                        context: context,
                        e: e,
                        extendedErrorMessage:  $"Reminder Exception Count={exceptionCount}");
                    if (exceptionCount > Parameters.BackgroundService.ReminderIgnoreConsecutiveExceptionCount)
                    {
                        throw;
                    }
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
        }
    }
}
