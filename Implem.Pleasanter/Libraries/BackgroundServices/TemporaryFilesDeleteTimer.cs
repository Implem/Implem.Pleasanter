using Implem.DefinitionAccessor;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    /// <summary>
    /// テンポラリファイル削除を毎日定時に呼び出すクラス
    /// </summary>
    public class TemporaryFilesDeleteTimer : ExecutionTimerBase
    {
        override public async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Run(() =>
            {
                var context = CreateContext();
                var log = CreateSysLogModel(
                    context: context,
                    message: "delete temporary files.");
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
