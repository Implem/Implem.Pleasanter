using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Listener;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    internal sealed class QuartzSchedulerErrorListener(
        ILogger<CustomQuartzHostedService> logger) : SchedulerListenerSupport
    {
        public override Task SchedulerError(
            string message,
            SchedulerException cause,
            CancellationToken cancellationToken = default)
        {
            TryWriteSysLog(
                method: nameof(SchedulerError),
                message: message,
                exception: cause);
            return Task.CompletedTask;
        }

        internal void TryWriteSysLog(string method, string message, Exception exception)
        {
            try
            {
                logger.LogError(exception, "{message}", message);
                var context = new Context(
                    request: false,
                    sessionStatus: false,
                    sessionData: false,
                    user: false,
                    item: false,
                    setPermissions: false)
                {
                    Controller = nameof(CustomQuartzHostedService),
                    Action = method,
                    TenantId = 0,
                    Id = 0
                };
                _ = new SysLogModel(
                    context: context,
                    method: null,
                    message: message + Environment.NewLine + exception.Message,
                    errStackTrace: Parameters.SysLog.OutputErrorDetails
                        ? exception.ToString()
                        : exception.StackTrace,
                    sysLogType: SysLogModel.SysLogTypes.Exception);
            }
            catch (Exception loggingException)
            {
                logger.LogError(
                    exception: loggingException,
                    message: "Quartz SysLog Recording Failed: {Message}",
                    args: [message]);
            }
        }
    }
}
