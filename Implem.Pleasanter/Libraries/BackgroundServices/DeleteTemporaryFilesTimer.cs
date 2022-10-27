using Implem.DefinitionAccessor;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    public class DeleteTemporaryFilesTimer : ExecutionTimerBase
    {
        override public async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Run(() =>
            {
                var context = CreateContext();
                var log = CreateSysLogModel(
                    context: context,
                    message: "Delete Temporary Files.");
                Initializer.DeleteTemporaryFiles();
                log.Finish(context: context);
            }, stoppingToken);
        }

        override public IList<string> GetTimeList()
        {
            return Parameters.BackgroundService.DeleteTemporaryFilesTime;
        }

        public override bool Enabled()
        {
            return Parameters.BackgroundService.DeleteTemporaryFiles;
        }
    }
}
