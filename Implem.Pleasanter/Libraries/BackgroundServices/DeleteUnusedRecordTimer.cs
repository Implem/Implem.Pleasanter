using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    public class DeleteUnusedRecordTimer : ClusterExecutionTimerBase
    {
        public class Param : IExecutionTimerBaseParam
        {
            public static readonly JobKey jobKey = new JobKey("DeleteUnusedRecordTimer", "ExecutionTimerBase");
            public Type JobType => typeof(DeleteUnusedRecordTimer);
            public IEnumerable<string> TimeList => Parameters.BackgroundService.DeleteUnusedRecordTime;
            public bool Enabled => Parameters.BackgroundService.DeleteUnusedRecord;
            public JobKey JobKey => jobKey;
            public string JobName => "DeleteUnusedRecordService";
            public Task<bool> SetCustomTimer(IScheduler scheduler) => Task.FromResult(false);
        }

        public override async Task Execute(IJobExecutionContext context)
        {
            await Task.Run(() =>
            {
                var context = CreateContext();
                var log = CreateSysLogModel(
                    context: context,
                    message: "Delete Unused Records.");
                try
                {
                    DeleteFromTables(context);
                }
                catch (Exception e)
                {
                    _ = new SysLogModel(
                        context: context,
                        e: e,
                        extendedErrorMessage: "DeleteUnusedRecordService Exception");
                }
                finally
                {
                    log.Finish(context: context);
                }
            }, context.CancellationToken);
        }

        internal static IExecutionTimerBaseParam GetParam()
        {
            return new Param();
        }

        private static void DeleteFromTables(Context context)
        {
            ExecuteDeleteFromTable(context, "Links", "DestinationId");
            ExecuteDeleteFromTable(context, "Links", "SourceId");
            ExecuteDeleteFromTable(context, "Permissions", "ReferenceId");
        }

        private static void ExecuteDeleteFromTable(Context context, string tableName, string columnName)
        {
            var sqlTemplate = Def.Sql.DeleteByUnusedReferenceId
                .Replace("#TableName#", tableName)
                .Replace("#ColumnName#", columnName);
            var configuredTopSize = Parameters.BackgroundService?.DeleteUnusedRecordChunkSize;
            var topSize = configuredTopSize.HasValue && configuredTopSize.Value > 0
                ? configuredTopSize.Value
                : 1000;
            var totalDeletedCount = 0;
            var iterations = 0;
            const int maxIterations = 10000; // 無限ループ防止用の最大繰り返し回数
            int deletedCount;
            do
            {
                deletedCount = Repository.ExecuteNonQuery(
                    context: context,
                    statements:
                    [
                        new SqlStatement(
                            commandText: sqlTemplate,
                            param: new SqlParamCollection
                            {
                                { "TopSize", topSize }
                            })
                    ]);
                totalDeletedCount += deletedCount;
                iterations++;
                if (iterations % 10 == 0 && deletedCount > 0)
                {
                    _ = new SysLogModel(
                        context: context,
                        method: nameof(ExecuteDeleteFromTable),
                        message: $"{tableName}.{columnName} cumulative deleted: {totalDeletedCount}",
                        sysLogType: SysLogModel.SysLogTypes.Info);
                }
            }
            while (deletedCount > 0 && iterations < maxIterations);
            _ = new SysLogModel(
                context: context,
                method: nameof(ExecuteDeleteFromTable),
                message: $"{tableName}.{columnName} total deleted: {totalDeletedCount}, iterations: {iterations}",
                sysLogType: SysLogModel.SysLogTypes.Info);
        }
    }
}
