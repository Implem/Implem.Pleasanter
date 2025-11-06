using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Quartz;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    /// <summary>
    /// ごみ箱(Xxx_deletedテーブル)の削除を毎日定時に呼び出すクラス。
    /// Users,Groups,Deptsの_deletedテーブルも削除対象。
    /// </summary>
    public class DeleteTrashBoxTimer : ClusterExecutionTimerBase
    {
        public class Param : IExecutionTimerBaseParam
        {
            public static readonly JobKey jobKey = new JobKey("DeleteTrashBoxTimer", "ExecutionTimerBase");
            public Type JobType => typeof(DeleteTrashBoxTimer);
            public IEnumerable<string> TimeList => Parameters.BackgroundService.DeleteTrashBoxTime;
            public bool Enabled => Parameters.BackgroundService.DeleteTrashBox;
            public JobKey JobKey => jobKey;
            public string JobName => "DeleteTrashBoxService";
            public Task<bool> SetCustomTimer(IScheduler scheduler) => Task.FromResult(false);
        }

        public override async Task Execute(IJobExecutionContext context)
        {
            await Task.Run(() =>
            {
                var context = CreateContext();
                var log = CreateSysLogModel(
                    context: context,
                    message: "Delete TrashBox.");
                PhysicalDelete(context);
                log.Finish(context: context);
            }, context.CancellationToken);
        }

        internal static IExecutionTimerBaseParam GetParam()
        {
            return new Param();
        }

        private void PhysicalDelete(Context context)
        {
            if (Parameters.BackgroundService.DeleteTrashBoxRetentionPeriod <= 0)
            {
                return;
            }
            var deleteOlderThan = DateTime.Now.AddDays(
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
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.PhysicalDeleteUsers(
                    tableType: Sqls.TableTypes.Deleted,
                    where: Rds.UsersWhere().UpdatedTime(
                        deleteOlderThan,
                        _operator: "<")));
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.PhysicalDeleteGroups(
                    tableType: Sqls.TableTypes.Deleted,
                    where: Rds.GroupsWhere().UpdatedTime(
                        deleteOlderThan,
                        _operator: "<")));
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.PhysicalDeleteDepts(
                    tableType: Sqls.TableTypes.Deleted,
                    where: Rds.DeptsWhere().UpdatedTime(
                        deleteOlderThan,
                        _operator: "<")));
        }
    }
}
