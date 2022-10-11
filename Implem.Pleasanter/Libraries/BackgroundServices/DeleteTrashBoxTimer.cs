using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    /// <summary>
    /// ごみ箱の削除を毎日定時に呼び出すクラス
    /// </summary>
    public class DeleteTrashBoxTimer : ExecutionTimerBase
    {
        override public async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Run(() =>
            {
                var context = CreateContext();
                var log = CreateSysLogModel(
                    context: context,
                    message: "delete TrashBox.");
                PhysicalDelete(context);
                log.Finish(context: context);
            }, stoppingToken);
        }

        override public IList<string> GetTimeList()
        {
            return Parameters.BackgroundService.DeleteTrashBoxTime;
        }

        public override bool Enabled()
        {
            return Parameters.BackgroundService.DeleteTrashBox;
        }

        private void PhysicalDelete(Context context)
        {
            if (Parameters.BackgroundService.DeleteTrashBoxRetentionPeriod <= 0)
            {
                return;
            }
            var deleteOlderThan = DateTime.Now.Date.AddDays(
                Parameters.BackgroundService.DeleteTrashBoxRetentionPeriod * -1);
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.PhysicalDeleteItems(
                    tableType: Sqls.TableTypes.Deleted,
                    where: Rds.ItemsWhere().UpdatedTime(
                        deleteOlderThan,
                        _operator: "<")));
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.PhysicalDeleteIssues(
                    tableType: Sqls.TableTypes.Deleted,
                    where: Rds.IssuesWhere().UpdatedTime(
                        deleteOlderThan,
                        _operator: "<")));
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.PhysicalDeleteResults(
                    tableType: Sqls.TableTypes.Deleted,
                    where: Rds.ResultsWhere().UpdatedTime(
                        deleteOlderThan,
                        _operator: "<")));
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.PhysicalDeleteSites(
                    tableType: Sqls.TableTypes.Deleted,
                    where: Rds.SitesWhere().UpdatedTime(
                        deleteOlderThan,
                        _operator: "<")));
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.PhysicalDeleteWikis(
                    tableType: Sqls.TableTypes.Deleted,
                    where: Rds.WikisWhere().UpdatedTime(
                        deleteOlderThan,
                        _operator: "<")));
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.PhysicalDeleteBinaries(
                    tableType: Sqls.TableTypes.Deleted,
                    where: Rds.BinariesWhere().UpdatedTime(
                        deleteOlderThan,
                        _operator: "<")));
        }
    }
}
