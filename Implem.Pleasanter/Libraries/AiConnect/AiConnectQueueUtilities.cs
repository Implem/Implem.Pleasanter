using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.BackgroundServices;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.AiConnect
{
    public static class AiConnectQueueUtilities
    {
        public static bool ShouldQueue()
        {
            return BackgroundJobQueue.BackgroundQueueEnabled()
                && Parameters.BackgroundJobs?.OutputFilePath.IsNullOrEmpty() == false;
        }

        public static long EnqueueSync(
            Context context,
            SiteSettings ss,
            string referenceType,
            long referenceId,
            int aiProviderId,
            AiConnectSyncOperation operation = AiConnectSyncOperation.Unknown)
        {
            return EnqueueSyncCore(
                context: context,
                ss: ss,
                referenceType: referenceType,
                referenceId: referenceId,
                aiProviderId: aiProviderId,
                operation: operation,
                retryCount: 0,
                resendCount: 0,
                checkDuplicate: true);
        }

        public static long EnqueueSyncRetry(
            Context context,
            SiteSettings ss,
            AiConnectSyncJobParameters original)
        {
            return EnqueueSyncCore(
                context: context,
                ss: ss,
                referenceType: original.ReferenceType,
                referenceId: original.ReferenceId,
                aiProviderId: original.AiProviderId,
                operation: original.Operation,
                retryCount: original.RetryCount + 1,
                resendCount: original.ResendCount,
                checkDuplicate: false);
        }

        public static long EnqueueSyncResend(
            Context context,
            SiteSettings ss,
            string referenceType,
            long referenceId,
            int aiProviderId,
            AiConnectSyncOperation operation = AiConnectSyncOperation.Unknown)
        {
            return EnqueueSyncCore(
                context: context,
                ss: ss,
                referenceType: referenceType,
                referenceId: referenceId,
                aiProviderId: aiProviderId,
                operation: operation,
                retryCount: 0,
                resendCount: 1,
                checkDuplicate: false);
        }

        private static long EnqueueSyncCore(
            Context context,
            SiteSettings ss,
            string referenceType,
            long referenceId,
            int aiProviderId,
            AiConnectSyncOperation operation,
            int retryCount,
            int resendCount,
            bool checkDuplicate)
        {
            if (checkDuplicate)
            {
                var duplicateId = FindDuplicateSyncJob(
                    context: context,
                    ss: ss,
                    referenceType: referenceType,
                    referenceId: referenceId,
                    aiProviderId: aiProviderId);
                if (duplicateId != 0)
                {
                    new SysLogModel(
                        context: context,
                        method: nameof(EnqueueSync),
                        message: "Skipped enqueue (duplicate Pending): "
                            + $"BackgroundJobId={duplicateId}"
                            + $", ReferenceType={referenceType}"
                            + $", ReferenceId={referenceId}"
                            + $", AiProviderId={aiProviderId}",
                        sysLogType: SysLogModel.SysLogTypes.Info);
                    return 0;
                }
            }
            var jobParameters = new AiConnectSyncJobParameters
            {
                ReferenceType = referenceType,
                ReferenceId = referenceId,
                AiProviderId = aiProviderId,
                Operation = operation,
                Language = context.Language,
                RetryCount = retryCount,
                ResendCount = resendCount
            };
            return BackgroundJobQueue.Enqueue(
                context: context,
                siteId: ss.SiteId,
                jobType: BackgroundJobTypes.AiConnectSync,
                jobParameters: jobParameters.ToJson());
        }

        public static long EnqueueDelete(
            Context context,
            SiteSettings ss,
            string referenceType,
            long referenceId,
            int aiProviderId)
        {
            return EnqueueDeleteCore(
                context: context,
                ss: ss,
                referenceType: referenceType,
                referenceId: referenceId,
                aiProviderId: aiProviderId,
                retryCount: 0,
                checkDuplicate: true);
        }

        public static long EnqueueDeleteRetry(
            Context context,
            SiteSettings ss,
            AiConnectDeleteJobParameters original)
        {
            return EnqueueDeleteCore(
                context: context,
                ss: ss,
                referenceType: original.ReferenceType,
                referenceId: original.ReferenceId,
                aiProviderId: original.AiProviderId,
                retryCount: original.RetryCount + 1,
                checkDuplicate: false);
        }

        private static long EnqueueDeleteCore(
            Context context,
            SiteSettings ss,
            string referenceType,
            long referenceId,
            int aiProviderId,
            int retryCount,
            bool checkDuplicate)
        {
            if (checkDuplicate)
            {
                var duplicateId = FindDuplicateDeleteJob(
                    context: context,
                    ss: ss,
                    referenceType: referenceType,
                    referenceId: referenceId,
                    aiProviderId: aiProviderId);
                if (duplicateId != 0)
                {
                    new SysLogModel(
                        context: context,
                        method: nameof(EnqueueDelete),
                        message: "Skipped enqueue (duplicate Pending): "
                            + $"BackgroundJobId={duplicateId}"
                            + $", ReferenceType={referenceType}"
                            + $", ReferenceId={referenceId}"
                            + $", AiProviderId={aiProviderId}",
                        sysLogType: SysLogModel.SysLogTypes.Info);
                    return 0;
                }
            }
            var jobParameters = new AiConnectDeleteJobParameters
            {
                ReferenceType = referenceType,
                ReferenceId = referenceId,
                AiProviderId = aiProviderId,
                Language = context.Language,
                RetryCount = retryCount
            };
            return BackgroundJobQueue.Enqueue(
                context: context,
                siteId: ss.SiteId,
                jobType: BackgroundJobTypes.AiConnectDelete,
                jobParameters: jobParameters.ToJson());
        }

        public static long EnqueueResync(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider)
        {
            var duplicateId = FindDuplicateResyncJob(
                context: context,
                ss: ss,
                aiProviderId: aiProvider.Id);
            if (duplicateId != 0)
            {
                new SysLogModel(
                    context: context,
                    method: nameof(EnqueueResync),
                    message: $"Skipped enqueue (already registered): BackgroundJobId={duplicateId}"
                        + $", AiProviderId={aiProvider.Id}",
                    sysLogType: SysLogModel.SysLogTypes.Info);
                return 0;
            }
            var jobParameters = new AiConnectResyncJobParameters
            {
                AiProviderId = aiProvider.Id,
                Language = context.Language
            };
            return BackgroundJobQueue.Enqueue(
                context: context,
                siteId: ss.SiteId,
                jobType: BackgroundJobTypes.AiConnectResync,
                jobParameters: jobParameters.ToJson());
        }

        public static bool CanEnqueueIndexCheck()
        {
            return ShouldQueue()
                && (Parameters.AiConnect?.Rag.IndexCheckMaxAttempts ?? 10) > 0;
        }

        public static long EnqueueIndexCheck(
            Context context,
            SiteSettings ss,
            string referenceType,
            long referenceId,
            int aiProviderId,
            string fileId,
            int resendCount,
            List<string> oldFileIds = null)
        {
            if (CanEnqueueIndexCheck() == false)
            {
                return 0;
            }
            var jobParameters = new AiConnectIndexCheckJobParameters
            {
                ReferenceType = referenceType,
                ReferenceId = referenceId,
                AiProviderId = aiProviderId,
                FileId = fileId,
                AttemptCount = 0,
                ResendCount = resendCount,
                OldFileIds = oldFileIds,
                Language = context.Language
            };
            return BackgroundJobQueue.Enqueue(
                context: context,
                siteId: ss.SiteId,
                jobType: BackgroundJobTypes.AiConnectIndexCheck,
                jobParameters: jobParameters.ToJson());
        }

        public static long EnqueueIndexCheckNext(
            Context context,
            SiteSettings ss,
            AiConnectIndexCheckJobParameters original)
        {
            var jobParameters = new AiConnectIndexCheckJobParameters
            {
                ReferenceType = original.ReferenceType,
                ReferenceId = original.ReferenceId,
                AiProviderId = original.AiProviderId,
                FileId = original.FileId,
                AttemptCount = original.AttemptCount + 1,
                ResendCount = original.ResendCount,
                OldFileIds = original.OldFileIds,
                Language = context.Language
            };
            return BackgroundJobQueue.Enqueue(
                context: context,
                siteId: ss.SiteId,
                jobType: BackgroundJobTypes.AiConnectIndexCheck,
                jobParameters: jobParameters.ToJson());
        }

        private static long FindDuplicateSyncJob(
            Context context,
            SiteSettings ss,
            string referenceType,
            long referenceId,
            int aiProviderId)
        {
            var candidateTable = Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectBackgroundJobs(
                    column: Rds.BackgroundJobsColumn()
                        .BackgroundJobId()
                        .JobParameters(),
                    where: Rds.BackgroundJobsWhere()
                        .TenantId(context.TenantId)
                        .SiteId(ss.SiteId)
                        .JobType(BackgroundJobTypes.AiConnectSync)
                        .Status(BackgroundJobStatus.Pending)
                        .JobParameters(
                            value: $"%\"AiProviderId\":{aiProviderId}%",
                            _operator: " like ")));
            return candidateTable
                .AsEnumerable()
                .Select(row => new
                {
                    BackgroundJobId = row["BackgroundJobId"].ToLong(),
                    Parameters = row["JobParameters"].ToString()
                        .Deserialize<AiConnectSyncJobParameters>()
                })
                .Where(o => o.Parameters?.ReferenceType == referenceType
                    && o.Parameters.ReferenceId == referenceId
                    && o.Parameters.AiProviderId == aiProviderId
                    && o.Parameters.RetryCount == 0
                    && o.Parameters.ResendCount == 0)
                .Select(o => o.BackgroundJobId)
                .FirstOrDefault();
        }

        private static long FindDuplicateDeleteJob(
            Context context,
            SiteSettings ss,
            string referenceType,
            long referenceId,
            int aiProviderId)
        {
            var candidateTable = Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectBackgroundJobs(
                    column: Rds.BackgroundJobsColumn()
                        .BackgroundJobId()
                        .JobParameters(),
                    where: Rds.BackgroundJobsWhere()
                        .TenantId(context.TenantId)
                        .SiteId(ss.SiteId)
                        .JobType(BackgroundJobTypes.AiConnectDelete)
                        .Status(BackgroundJobStatus.Pending)
                        .JobParameters(
                            value: $"%\"AiProviderId\":{aiProviderId}%",
                            _operator: " like ")));
            return candidateTable
                .AsEnumerable()
                .Select(row => new
                {
                    BackgroundJobId = row["BackgroundJobId"].ToLong(),
                    Parameters = row["JobParameters"].ToString()
                        .Deserialize<AiConnectDeleteJobParameters>()
                })
                .Where(o => o.Parameters?.ReferenceType == referenceType
                    && o.Parameters.ReferenceId == referenceId
                    && o.Parameters.AiProviderId == aiProviderId
                    && o.Parameters.RetryCount == 0)
                .Select(o => o.BackgroundJobId)
                .FirstOrDefault();
        }

        private static long FindDuplicateResyncJob(
            Context context,
            SiteSettings ss,
            int aiProviderId)
        {
            var candidateTable = Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectBackgroundJobs(
                    column: Rds.BackgroundJobsColumn()
                        .BackgroundJobId()
                        .JobParameters(),
                    where: Rds.BackgroundJobsWhere()
                        .TenantId(context.TenantId)
                        .SiteId(ss.SiteId)
                        .JobType(BackgroundJobTypes.AiConnectResync)
                        .Status_In(value: new[]
                        {
                            BackgroundJobStatus.Pending,
                            BackgroundJobStatus.Running,
                            BackgroundJobStatus.RunningOverdue
                        })
                        .JobParameters(
                            value: $"%\"AiProviderId\":{aiProviderId}%",
                            _operator: " like ")));
            return candidateTable
                .AsEnumerable()
                .Select(row => new
                {
                    BackgroundJobId = row["BackgroundJobId"].ToLong(),
                    Parameters = row["JobParameters"].ToString()
                        .Deserialize<AiConnectResyncJobParameters>()
                })
                .Where(o => o.Parameters?.AiProviderId == aiProviderId)
                .Select(o => o.BackgroundJobId)
                .FirstOrDefault();
        }
    }
}
