using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.Interfaces;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.BackgroundServices;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Resources;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.Web;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Quartz;
using IoFile = System.IO.File;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;

namespace Implem.Pleasanter.Models
{
    public static partial class BackgroundJobQueue
    {
        private static readonly TimeSpan DownloadLockLease = TimeSpan.FromHours(24);
        private static readonly Dictionary<string, string> JobTypeDisplayIds =
            new Dictionary<string, string>
        {
            { "Export", "Export" }
        };
        private static readonly ConcurrentDictionary<long, DateTime> s_downloadLocks
            = new ConcurrentDictionary<long, DateTime>();

        public static bool TryAcquireDownloadLock(
            long backgroundJobId,
            out DateTime lockToken)
        {
            lockToken = DateTime.UtcNow;
            if (s_downloadLocks.TryAdd(
                backgroundJobId,
                lockToken))
            {
                return true;
            }
            if (s_downloadLocks.TryGetValue(
                backgroundJobId,
                out var currentLockToken)
                && IsDownloadLockExpired(
                    lockToken: currentLockToken,
                    now: lockToken)
                && ReleaseDownloadLock(
                    backgroundJobId: backgroundJobId,
                    lockToken: currentLockToken))
            {
                return s_downloadLocks.TryAdd(
                    backgroundJobId,
                    lockToken);
            }
            return false;
        }

        public static bool ReleaseDownloadLock(
            long backgroundJobId,
            DateTime lockToken)
        {
            return ((ICollection<KeyValuePair<long, DateTime>>)s_downloadLocks).Remove(
                new KeyValuePair<long, DateTime>(
                    backgroundJobId,
                    lockToken));
        }

        public static bool IsDownloading(long backgroundJobId)
        {
            var now = DateTime.UtcNow;
            if (s_downloadLocks.TryGetValue(
                backgroundJobId,
                out var lockToken) == false)
            {
                return false;
            }
            if (IsDownloadLockExpired(
                lockToken: lockToken,
                now: now))
            {
                ReleaseDownloadLock(
                    backgroundJobId: backgroundJobId,
                    lockToken: lockToken);
                return false;
            }
            return true;
        }

        private static bool IsDownloadLockExpired(
            long backgroundJobId,
            DateTime now)
        {
            return s_downloadLocks.TryGetValue(
                backgroundJobId,
                out var lockToken)
                && IsDownloadLockExpired(
                    lockToken: lockToken,
                    now: now);
        }

        private static bool IsDownloadLockExpired(
            DateTime lockToken,
            DateTime now)
        {
            return lockToken.Add(DownloadLockLease) < now;
        }

        internal static BackgroundJobDownloadResult PrepareDownload(
            Context context,
            long backgroundJobId)
        {
            var model = Get(
                context: context,
                backgroundJobId: backgroundJobId);
            if (model == null)
            {
                return BackgroundJobDownloadResult.NotFound();
            }
            var errorData = BackgroundJobAccessValidator.OnDownloading(
                context: context,
                model: model);
            if (errorData.Type != Error.Types.None
                || IoFile.Exists(model.File) == false)
            {
                return BackgroundJobDownloadResult.NotFound();
            }
            if (TryAcquireDownloadLock(
                backgroundJobId: backgroundJobId,
                lockToken: out var lockToken) == false)
            {
                return BackgroundJobDownloadResult.Conflict();
            }
            var fileInfo = new System.IO.FileInfo(model.File);
            return BackgroundJobDownloadResult.Success(
                backgroundJobId: backgroundJobId,
                lockToken: lockToken,
                fileInfo: fileInfo,
                contentType: Mime.Type(fileDownloadName: fileInfo.Name));
        }

        public static long Enqueue(
            Context context,
            long siteId,
            string jobType,
            string jobParameters = null,
            int priority = 100)
        {
            var response = Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: new SqlStatement[]
                {
                    Rds.InsertBackgroundJobs(
                        selectIdentity: true,
                        param: Rds.BackgroundJobsParam()
                            .TenantId(context.TenantId)
                            .SiteId(siteId)
                            .UserId(context.UserId)
                            .JobType(jobType.MaxLength(50))
                            .Status(BackgroundJobStatus.Pending)
                            .Priority(priority)
                            .JobParameters(jobParameters ?? string.Empty)
                            .JobEnqueuedTime(DateTime.UtcNow)
                            .Comments("[]"))
                });
            var backgroundJobId = (response.Id ?? 0L).ToLong();
            new SysLogModel(
                context: context,
                method: nameof(Enqueue),
                message: $"Enqueued: BackgroundJobId={backgroundJobId}"
                    + $", JobType={jobType}"
                    + $", TenantId={context.TenantId}"
                    + $", SiteId={siteId}"
                    + $", UserId={context.UserId}",
                sysLogType: SysLogModel.SysLogTypes.Info);
            var outputFilePathConfigured =
                Parameters.BackgroundJobs?.OutputFilePath.IsNullOrEmpty() == false;
            if (outputFilePathConfigured == false)
            {
                FailPendingOutputFilePathNotConfigured(
                    context: context,
                    backgroundJobId: backgroundJobId);
            }
            return backgroundJobId;
        }

        private static void FailPendingOutputFilePathNotConfigured(
            Context context,
            long backgroundJobId)
        {
            var now = DateTime.UtcNow;
            var resultMessage = SanitizeMessage(
                Displays.Get(
                    context: context,
                    id: "BackgroundJobOutputFilePathNotConfigured"));
            var count = Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.UpdateBackgroundJobs(
                    where: Rds.BackgroundJobsWhere()
                        .BackgroundJobId(backgroundJobId)
                        .Status(BackgroundJobStatus.Pending),
                    param: Rds.BackgroundJobsParam()
                        .Status(BackgroundJobStatus.Failed)
                        .JobFinishedTime(now)
                        .ResultMessage(resultMessage),
                    addUpdatorParam: false,
                    addUpdatedTimeParam: false));
            new SysLogModel(
                context: context,
                method: nameof(Enqueue),
                message: count == 1
                    ? $"Failed at enqueue (OutputFilePath not configured): BackgroundJobId={backgroundJobId}"
                    : $"Skipped fail at enqueue (unexpected state): BackgroundJobId={backgroundJobId}",
                sysLogType: SysLogModel.SysLogTypes.Info);
        }

        public static bool BackgroundQueueEnabled()
        {
            return Parameters.BackgroundJobs?.BackgroundQueue == true
                && Parameters.AllowQueue();
        }

        public static bool ShouldQueueExport(Context context, SiteSettings ss)
        {
            return BackgroundQueueEnabled()
                && CanQueueExportReferenceType(referenceType: ss?.ReferenceType);
        }

        private static bool CanQueueExportReferenceType(string referenceType)
        {
            return referenceType == "Issues"
                || referenceType == "Results";
        }

        public static ErrorData ValidateBeforeEnqueueExport(
            Context context,
            SiteSettings ss)
        {
            if (context.ContractSettings.Export == false)
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            return ss?.ReferenceType == "Issues"
                ? IssueValidators.OnExporting(
                    context: context,
                    ss: ss)
                : ResultValidators.OnExporting(
                    context: context,
                    ss: ss);
        }

        public static long EnqueueExport(
            Context context,
            long siteId)
        {
            var jobParameters = ExportJobParameters.FromContext(context: context);
            return Enqueue(
                context: context,
                siteId: siteId,
                jobType: "Export",
                jobParameters: jobParameters.ToJson());
        }

        public static string GetJobTypeLabel(
            Context context,
            string jobType)
        {
            return GetJobTypeLabel(
                jobType: jobType,
                language: context.Language);
        }

        public static string GetJobTypeLabel(
            string jobType,
            string language)
        {
            if (jobType.IsNullOrEmpty())
            {
                return string.Empty;
            }
            return JobTypeDisplayIds.ContainsKey(jobType)
                ? Displays.Get(
                    id: JobTypeDisplayIds[jobType],
                    language: ResolveLanguage(language: language))
                : jobType;
        }

        public static IEnumerable<string> JobTypes()
        {
            return JobTypeDisplayIds.Keys;
        }

        public static Context CreateSysLogContext(
            Context context,
            BackgroundJobModel model)
        {
            return new Context(
                tenantId: model.TenantId,
                userId: model.UserId,
                request: false,
                setAuthenticated: true,
                context: context)
            {
                Controller = nameof(BackgroundJobQueue),
                Action = model.JobType,
                SiteId = model.SiteId,
                Id = model.BackgroundJobId
            };
        }

        public static BackgroundJobModel Dequeue(
            Context context,
            bool ignoreRunningOverdueTenantLocks = false,
            int targetTenantId = 0)
        {
            var ignoreTargetRunningOverdue =
                targetTenantId > 0
                && ignoreRunningOverdueTenantLocks;
            var lockingStatuses = ignoreTargetRunningOverdue
                ? new[] { BackgroundJobStatus.Running }
                : new[]
                {
                    BackgroundJobStatus.Running,
                    BackgroundJobStatus.RunningOverdue
                };
            var lockingTenantIds = Rds.SelectBackgroundJobs(
                column: Rds.BackgroundJobsColumn().TenantId(),
                where: Rds.BackgroundJobsWhere()
                    .Status_In(value: lockingStatuses)
                    .TenantId(
                        value: targetTenantId,
                        _using: targetTenantId > 0));
            var candidateTable = Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectBackgroundJobs(
                    dataTableName: "Candidates",
                    column: Rds.BackgroundJobsColumn()
                        .BackgroundJobId()
                        .TenantId(),
                    where: Rds.BackgroundJobsWhere()
                        .Status(BackgroundJobStatus.Pending)
                        .TenantId(
                            value: targetTenantId,
                            _using: targetTenantId > 0)
                        .TenantId_In(
                            sub: lockingTenantIds,
                            negative: true),
                    orderBy: Rds.BackgroundJobsOrderBy()
                        .Priority()
                        .JobEnqueuedTime(),
                    top: 100));
            var candidateId = candidateTable
                .AsEnumerable()
                .Select(static row => row["BackgroundJobId"].ToLong())
                .FirstOrDefault();
            if (candidateId == 0) return null;
            var now = DateTime.UtcNow;
            var updateCount = Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.UpdateBackgroundJobs(
                    where: Rds.BackgroundJobsWhere()
                        .BackgroundJobId(candidateId)
                        .Status(BackgroundJobStatus.Pending),
                    param: Rds.BackgroundJobsParam()
                        .Status(BackgroundJobStatus.Running)
                        .JobStartedTime(now),
                    addUpdatorParam: false,
                    addUpdatedTimeParam: false));
            if (updateCount == 0) return null;
            var claimedTable = Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectBackgroundJobs(
                    dataTableName: "Claimed",
                    column: Rds.BackgroundJobsColumn()
                        .BackgroundJobId()
                        .TenantId()
                        .SiteId()
                        .UserId()
                        .JobType()
                        .Status()
                        .Priority()
                        .JobParameters()
                        .JobStartedTime(),
                    where: Rds.BackgroundJobsWhere()
                        .BackgroundJobId(candidateId)
                        .Status(BackgroundJobStatus.Running)));
            if (claimedTable.Rows.Count == 0) return null;
            var model = new BackgroundJobModel(
                context: context,
                dataRow: claimedTable.Rows[0]);
            new SysLogModel(
                context: CreateSysLogContext(
                    context: context,
                    model: model),
                method: nameof(Dequeue),
                message: $"Dequeued: BackgroundJobId={model.BackgroundJobId}"
                    + $", JobType={model.JobType}"
                    + $", TenantId={model.TenantId}"
                    + $", JobStartedTime={now:O}",
                sysLogType: SysLogModel.SysLogTypes.Info);
            return model;
        }

        public static void Complete(
            Context context,
            BackgroundJobModel model,
            string resultData = null,
            string resultMessage = null)
        {
            var now = DateTime.UtcNow;
            var message = resultMessage.IsNullOrEmpty()
                ? GetResultMessage(
                    displayId: "BackgroundJobResultCompleted",
                    language: GetJobLanguage(model: model),
                    jobType: model.JobType)
                : resultMessage;
            var count = Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.UpdateBackgroundJobs(
                    where: Rds.BackgroundJobsWhere()
                        .BackgroundJobId(model.BackgroundJobId)
                        .Status_In(value: new[]
                        {
                            BackgroundJobStatus.Running,
                            BackgroundJobStatus.RunningOverdue
                        }),
                    param: Rds.BackgroundJobsParam()
                        .Status(BackgroundJobStatus.Completed)
                        .JobFinishedTime(now)
                        .File(resultData ?? string.Empty)
                        .ResultMessage(message),
                    addUpdatorParam: false,
                    addUpdatedTimeParam: false));
            if (count == 0)
            {
                if (resultData.IsNullOrEmpty() == false
                    && IoFile.Exists(resultData))
                {
                    try
                    {
                        IoFile.Delete(resultData);
                    }
                    catch (Exception e)
                    {
                        new SysLogModel(
                            context: context,
                            e: e);
                    }
                }
                new SysLogModel(
                    context: CreateSysLogContext(
                        context: context,
                        model: model),
                    method: nameof(Complete),
                    message: $"Skipped Complete (already cancelled): BackgroundJobId={model.BackgroundJobId}"
                        + $", JobType={model.JobType}",
                    sysLogType: SysLogModel.SysLogTypes.Info);
                return;
            }
            new SysLogModel(
                context: CreateSysLogContext(
                    context: context,
                    model: model),
                method: nameof(Complete),
                message: $"Completed: BackgroundJobId={model.BackgroundJobId}"
                    + $", JobType={model.JobType}"
                    + $", JobFinishedTime={now:O}",
                sysLogType: SysLogModel.SysLogTypes.Info);
        }

        public static void Fail(
            Context context,
            BackgroundJobModel model,
            string errorMessage)
        {
            var now = DateTime.UtcNow;
            var sanitized = SanitizeMessage(errorMessage);
            var count = Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.UpdateBackgroundJobs(
                    where: Rds.BackgroundJobsWhere()
                        .BackgroundJobId(model.BackgroundJobId)
                        .Status_In(value: new[]
                        {
                            BackgroundJobStatus.Running,
                            BackgroundJobStatus.RunningOverdue
                        }),
                    param: Rds.BackgroundJobsParam()
                        .Status(BackgroundJobStatus.Failed)
                        .JobFinishedTime(now)
                        .ResultMessage(sanitized),
                    addUpdatorParam: false,
                    addUpdatedTimeParam: false));
            if (count == 0)
            {
                new SysLogModel(
                    context: CreateSysLogContext(
                        context: context,
                        model: model),
                    method: nameof(Fail),
                    message: $"Skipped Fail (already cancelled): BackgroundJobId={model.BackgroundJobId}"
                        + $", JobType={model.JobType}"
                        + $", ResultMessage={sanitized}",
                    sysLogType: SysLogModel.SysLogTypes.Info);
                return;
            }
            new SysLogModel(
                context: CreateSysLogContext(
                    context: context,
                    model: model),
                method: nameof(Fail),
                message: $"Failed: BackgroundJobId={model.BackgroundJobId}"
                    + $", JobType={model.JobType}"
                    + $", ResultMessage={sanitized}",
                sysLogType: SysLogModel.SysLogTypes.Info);
        }

        private static string SanitizeMessage(string message)
        {
            if (message.IsNullOrEmpty())
            {
                return string.Empty;
            }
            var sanitized = CredentialPattern().Replace(
                message,
                "$1=***");
            sanitized = WindowsPathPattern().Replace(
                sanitized,
                "***");
            sanitized = UnixPathPattern().Replace(
                sanitized,
                "***");
            return sanitized;
        }

        private static string GetJobLanguage(BackgroundJobModel model)
        {
            if (model.JobType == "Export")
            {
                return model.JobParameters
                    .Deserialize<ExportJobParameters>()
                    ?.Language;
            }
            return null;
        }

        private static string GetResultMessage(
            string displayId,
            string language,
            string jobType)
        {
            var resolvedLanguage = ResolveLanguage(language: language);
            return Displays.Get(
                id: displayId,
                language: resolvedLanguage,
                data: GetJobTypeLabel(
                    jobType: jobType,
                    language: resolvedLanguage));
        }

        private static string ResolveLanguage(string language)
        {
            return language.IsNullOrEmpty()
                ? Parameters.BackgroundJobs?.FallbackLanguage
                    ?? Parameters.Service.DefaultLanguage
                : language;
        }

        [GeneratedRegex(
            @"(Server|Data Source|Password|PWD|User ID|UID)=[^;]*",
            RegexOptions.IgnoreCase)]
        private static partial Regex CredentialPattern();

        [GeneratedRegex(@"[A-Za-z]:\\[^\s""']+")]
        private static partial Regex WindowsPathPattern();

        [GeneratedRegex(@"/(?:var|tmp|home|etc|usr|opt)/[^\s""']+")]
        private static partial Regex UnixPathPattern();

        public static ErrorData Cancel(
            Context context,
            long backgroundJobId)
        {
            var model = new BackgroundJobModel(
                context: context,
                backgroundJobId: backgroundJobId);
            if (model.AccessStatus == Databases.AccessStatuses.NotFound)
            {
                return new ErrorData(type: Error.Types.NotFound);
            }
            var errorData = BackgroundJobAccessValidator.OnCancelling(
                context: context,
                model: model);
            if (errorData.Type != Error.Types.None) return errorData;
            var resultMessage = GetResultMessage(
                displayId: "BackgroundJobResultCancelled",
                language: context.Language,
                jobType: model.JobType);
            var now = DateTime.UtcNow;
            var count = Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.UpdateBackgroundJobs(
                    where: Rds.BackgroundJobsWhere()
                        .BackgroundJobId(backgroundJobId)
                        .Status(BackgroundJobStatus.Pending),
                    param: Rds.BackgroundJobsParam()
                        .Status(BackgroundJobStatus.Cancelled)
                        .JobFinishedTime(now)
                        .ResultMessage(resultMessage),
                    addUpdatorParam: false,
                    addUpdatedTimeParam: false));
            if (count == 1)
            {
                new SysLogModel(
                    context: context,
                    method: nameof(Cancel),
                    message: $"Cancelled: BackgroundJobId={backgroundJobId}"
                        + $", PreviousStatus={model.Status}"
                        + $", OperatorUserId={context.UserId}"
                        + $", JobFinishedTime={now:O}",
                    sysLogType: SysLogModel.SysLogTypes.Info);
            }
            return new ErrorData(type: count == 1
                ? Error.Types.None
                : Error.Types.CanNotPerformed);
        }

        public static string BulkCancel(Context context)
        {
            var ids = (context.Forms.Data("Ids").Deserialize<List<string>>()
                ?? new List<string>())
                .Select(static value => value.ToLong())
                .Where(static id => id > 0)
                .ToList();
            if (ids.Any() == false)
            {
                return Messages.ResponseSelectTargets(context: context).ToJson();
            }
            var cancelledCount = 0;
            var failedResults = new List<string>();
            foreach (var id in ids)
            {
                var errorData = Cancel(
                    context: context,
                    backgroundJobId: id);
                if (errorData.Type == Error.Types.None)
                {
                    cancelledCount++;
                }
                else
                {
                    failedResults.Add($"{id}:{errorData.Type}");
                }
            }
            if (failedResults.Any())
            {
                new SysLogModel(
                    context: context,
                    method: nameof(BulkCancel),
                    message: $"BulkCancel partially failed: FailedCount={failedResults.Count}"
                        + $", Details={string.Join(",", failedResults)}"
                        + $", OperatorUserId={context.UserId}",
                    sysLogType: SysLogModel.SysLogTypes.Warning);
            }
            var responses = HtmlBackgroundJobs.BackgroundJobsGridRows(context: context)
                .Deserialize<ResponseCollection>()
                    ?? new ResponseCollection();
            if (cancelledCount > 0)
            {
                responses.Message(Messages.BackgroundJobsCancelled(
                    context: context,
                    Displays.BackgroundJobs(context: context),
                    cancelledCount.ToString()));
            }
            if (failedResults.Any())
            {
                responses.Message(Messages.BackgroundJobsCancelFailed(
                    context: context,
                    Displays.BackgroundJobs(context: context),
                    failedResults.Count.ToString()));
            }
            return responses.ToJson();
        }

        public static ErrorData Delete(
            Context context,
            long backgroundJobId)
        {
            var model = new BackgroundJobModel(
                context: context,
                backgroundJobId: backgroundJobId);
            if (model.AccessStatus == Databases.AccessStatuses.NotFound)
            {
                return new ErrorData(type: Error.Types.NotFound);
            }
            var errorData = BackgroundJobAccessValidator.OnDeleting(
                context: context,
                model: model);
            if (errorData.Type != Error.Types.None)
            {
                return errorData;
            }
            if (IsDownloading(backgroundJobId: backgroundJobId))
            {
                return new ErrorData(
                    context: context,
                    type: Error.Types.CustomError,
                    sysLogsStatus: StatusCodes.Status400BadRequest,
                    sysLogsDescription: Debugs.GetSysLogsDescription(),
                    data: Displays.Get(
                        context: context,
                        id: "BackgroundJobDownloadingCanNotDelete"));
            }
            if (model.File.IsNullOrEmpty() == false
                && IoFile.Exists(model.File))
            {
                try
                {
                    IoFile.Delete(model.File);
                }
                catch (Exception e)
                {
                    new SysLogModel(
                        context: context,
                        e: e);
                }
            }
            var where = Rds.BackgroundJobsWhere()
                .BackgroundJobId(backgroundJobId);
            if (model.Status == BackgroundJobStatus.Pending)
            {
                where.Status(BackgroundJobStatus.Pending);
            }
            var count = Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.PhysicalDeleteBackgroundJobs(where: where));
            new SysLogModel(
                context: context,
                method: nameof(Delete),
                message: $"Deleted: BackgroundJobId={backgroundJobId}"
                    + $", Status={model.Status}"
                    + $", TenantId={model.TenantId}"
                    + $", SiteId={model.SiteId}"
                    + $", UserId={model.UserId}"
                    + $", OperatorUserId={context.UserId}",
                sysLogType: SysLogModel.SysLogTypes.Info);
            return new ErrorData(type: count == 1
                ? Error.Types.None
                : Error.Types.CanNotDelete);
        }

        public static string BulkDelete(Context context)
        {
            var ids = (context.Forms.Data("Ids").Deserialize<List<string>>()
                ?? new List<string>())
                .Select(static value => value.ToLong())
                .Where(static id => id > 0)
                .ToList();
            if (ids.Any() == false)
            {
                return Messages.ResponseSelectTargets(context: context).ToJson();
            }
            var deletedCount = 0;
            var failedResults = new List<string>();
            foreach (var id in ids)
            {
                var errorData = Delete(
                    context: context,
                    backgroundJobId: id);
                if (errorData.Type == Error.Types.None)
                {
                    deletedCount++;
                }
                else
                {
                    failedResults.Add($"{id}:{errorData.Type}");
                }
            }
            if (failedResults.Any())
            {
                new SysLogModel(
                    context: context,
                    method: nameof(BulkDelete),
                    message: $"BulkDelete partially failed: FailedCount={failedResults.Count}"
                        + $", Details={string.Join(",", failedResults)}"
                        + $", OperatorUserId={context.UserId}",
                    sysLogType: SysLogModel.SysLogTypes.Warning);
            }
            var responses = HtmlBackgroundJobs.BackgroundJobsGridRows(context: context)
                .Deserialize<ResponseCollection>()
                    ?? new ResponseCollection();
            if (deletedCount > 0)
            {
                responses.Message(Messages.BulkDeleted(
                    context: context,
                    Displays.BackgroundJobs(context: context),
                    deletedCount.ToString()));
            }
            if (failedResults.Any())
            {
                responses.Message(Messages.BackgroundJobBulkDeleteFailed(
                    context: context,
                    Displays.BackgroundJobs(context: context),
                    failedResults.Count.ToString()));
            }
            return responses.ToJson();
        }

        public static async Task<ErrorData> RequestNextJob(
            Context context,
            long backgroundJobId)
        {
            var model = new BackgroundJobModel(
                context: context,
                backgroundJobId: backgroundJobId);
            if (model.AccessStatus == Databases.AccessStatuses.NotFound)
            {
                return new ErrorData(type: Error.Types.NotFound);
            }
            if (BackgroundJobAccessValidator.CanAccess(
                context: context,
                model: model) == false)
            {
                return new ErrorData(
                    context: context,
                    type: Error.Types.HasNotPermission,
                    sysLogsStatus: StatusCodes.Status403Forbidden,
                    sysLogsDescription: Debugs.GetSysLogsDescription());
            }
            if (model.Status != BackgroundJobStatus.RunningOverdue)
            {
                return new ErrorData(
                    context: context,
                    type: Error.Types.CanNotPerformed,
                    sysLogsStatus: StatusCodes.Status400BadRequest,
                    sysLogsDescription: Debugs.GetSysLogsDescription());
            }
            var scheduler = CustomQuartzHostedService.Scheduler;
            var nonce = Guid.NewGuid().ToString("N");
            var jobKey = BackgroundJobNextJob.JobKey(backgroundJobId: backgroundJobId, nonce: nonce);
            var triggerKey = BackgroundJobNextJob.TriggerKey(backgroundJobId: backgroundJobId, nonce: nonce);
            var job = JobBuilder.Create<BackgroundJobNextJob>()
                .WithIdentity(jobKey)
                .UsingJobData(
                    BackgroundJobNextJob.BackgroundJobIdKey,
                    backgroundJobId.ToString())
                .Build();
            var trigger = TriggerBuilder.Create()
                .WithIdentity(triggerKey)
                .ForJob(jobKey)
                .StartNow()
                .Build();
            try
            {
                await scheduler.ScheduleJob(
                    jobDetail: job,
                    trigger: trigger);
            }
            catch (ObjectAlreadyExistsException)
            {
                return new ErrorData(type: Error.Types.None);
            }
            new SysLogModel(
                context: CreateSysLogContext(
                    context: context,
                    model: model),
                method: nameof(RequestNextJob),
                message: $"Requested next job: BackgroundJobId={backgroundJobId}"
                    + $", TenantId={model.TenantId}"
                    + $", OperatorUserId={context.UserId}",
                sysLogType: SysLogModel.SysLogTypes.Info);
            return new ErrorData(type: Error.Types.None);
        }

        public static async Task<string> BulkNextJob(Context context)
        {
            var ids = (context.Forms.Data("Ids").Deserialize<List<string>>()
                ?? new List<string>())
                .Select(static value => value.ToLong())
                .Where(static id => id > 0)
                .ToList();
            if (ids.Any() == false)
            {
                return Messages.ResponseSelectTargets(context: context).ToJson();
            }
            var requestedCount = 0;
            var failedResults = new List<string>();
            foreach (var id in ids)
            {
                var errorData = await RequestNextJob(
                    context: context,
                    backgroundJobId: id);
                if (errorData.Type == Error.Types.None)
                {
                    requestedCount++;
                }
                else
                {
                    failedResults.Add($"{id}:{errorData.Type}");
                }
            }
            if (failedResults.Any())
            {
                new SysLogModel(
                    context: context,
                    method: nameof(BulkNextJob),
                    message: $"BulkNextJob partially failed: FailedCount={failedResults.Count}"
                        + $", Details={string.Join(",", failedResults)}"
                        + $", OperatorUserId={context.UserId}",
                    sysLogType: SysLogModel.SysLogTypes.Warning);
            }
            var responses = HtmlBackgroundJobs.BackgroundJobsGridRows(context: context)
                .Deserialize<ResponseCollection>()
                    ?? new ResponseCollection();
            if (requestedCount > 0)
            {
                responses.Message(Messages.BackgroundJobNextJobRequested(
                    context: context,
                    requestedCount.ToString()));
            }
            if (failedResults.Any())
            {
                responses.Message(Messages.BackgroundJobNextJobRequestFailed(
                    context: context,
                    failedResults.Count.ToString()));
            }
            return responses.ToJson();
        }

        public static async Task<string> NextJobJson(
            Context context,
            long backgroundJobId)
        {
            var errorData = await RequestNextJob(
                context: context,
                backgroundJobId: backgroundJobId);
            if (errorData.Type != Error.Types.None)
            {
                return new ResponseCollection(context: context)
                    .Message(errorData.Message(context: context))
                    .ToJson();
            }
            return new ResponseCollection(context: context)
                .ReplaceAll(
                    "#MainContainer",
                    HtmlBackgroundJobs.BackgroundJobsEditor(
                        context: context,
                        backgroundJobId: backgroundJobId))
                .Message(Messages.BackgroundJobNextJobRequested(
                    context: context,
                    "1"))
                .ToJson();
        }

        public static string CancelJson(
            Context context,
            long backgroundJobId)
        {
            var errorData = Cancel(
                context: context,
                backgroundJobId: backgroundJobId);
            if (errorData.Type != Error.Types.None)
            {
                return new ResponseCollection(context: context)
                    .Message(errorData.Message(context: context))
                    .ToJson();
            }
            return new ResponseCollection(context: context)
                .ReplaceAll(
                    "#MainContainer",
                    HtmlBackgroundJobs.BackgroundJobsEditor(
                        context: context,
                        backgroundJobId: backgroundJobId))
                .Message(Messages.BackgroundJobCancelled(
                    context: context))
                .ToJson();
        }

        public static string DeleteJson(
            Context context,
            long backgroundJobId)
        {
            var errorData = Delete(
                context: context,
                backgroundJobId: backgroundJobId);
            if (errorData.Type != Error.Types.None)
            {
                return new ResponseCollection(context: context)
                    .Message(errorData.Message(context: context))
                    .ToJson();
            }
            return new ResponseCollection(context: context)
                .Href(Locations.Get(
                    context: context,
                    parts: "BackgroundJobs"))
                .Message(Messages.BackgroundJobDeleted(
                    context: context,
                    backgroundJobId.ToString()))
                .ToJson();
        }

        public static void RecoverStuckJobs(Context context)
        {
            var now = DateTime.UtcNow;
            var clusteringEnabled = Parameters.Quartz?.Clustering?.Enabled ?? false;
            var timeout = Parameters.BackgroundJobs?.BackgroundJobTimeout ?? 0;
            if (clusteringEnabled && timeout <= 0)
            {
                new SysLogModel(
                    context: context,
                    method: nameof(RecoverStuckJobs),
                    message: "RecoverStuckJobs: Skipped"
                        + " (Clustering enabled and BackgroundJobTimeout disabled).",
                    sysLogType: SysLogModel.SysLogTypes.Info);
                return;
            }
            var where = Rds.BackgroundJobsWhere()
                .Status_In(value: new[]
                {
                    BackgroundJobStatus.Running,
                    BackgroundJobStatus.RunningOverdue
                });
            if (clusteringEnabled)
            {
                where.JobStartedTime(
                    now.AddSeconds(-timeout),
                    _operator: "<");
            }
            var recoverAction = BackgroundJobStatus.Parse(
                name: Parameters.BackgroundJobs?.RecoverAction);
            if (recoverAction == BackgroundJobStatus.Pending)
            {
                Repository.ExecuteNonQuery(
                    context: context,
                    statements: Rds.UpdateBackgroundJobs(
                        where: where,
                        param: Rds.BackgroundJobsParam()
                            .Status(BackgroundJobStatus.Pending)
                            .JobEnqueuedTime(now)
                            .JobStartedTime(raw: "null")
                            .JobFinishedTime(raw: "null")
                            .ResultMessage(raw: "null"),
                        addUpdatorParam: false,
                        addUpdatedTimeParam: false));
                new SysLogModel(
                    context: context,
                    method: nameof(RecoverStuckJobs),
                    message: "RecoverStuckJobs: Re-queued Running jobs as Pending.",
                    sysLogType: SysLogModel.SysLogTypes.Info);
            }
            else
            {
                var errorMessage = SanitizeMessage(
                    Displays.Get(
                        id: "BackgroundJobRecoverStuckFailed",
                        language: Parameters.BackgroundJobs?.FallbackLanguage
                        ?? Parameters.Service.DefaultLanguage));
                var targetJobs = new BackgroundJobCollection(
                    context: context,
                    where: where);
                Repository.ExecuteNonQuery(
                    context: context,
                    statements: Rds.UpdateBackgroundJobs(
                        where: where,
                        param: Rds.BackgroundJobsParam()
                            .Status(BackgroundJobStatus.Failed)
                            .JobFinishedTime(now)
                            .ResultMessage(errorMessage),
                        addUpdatorParam: false,
                        addUpdatedTimeParam: false));
                foreach (var job in targetJobs)
                {
                    new SysLogModel(
                        context: CreateSysLogContext(
                            context: context,
                            model: job),
                        method: nameof(RecoverStuckJobs),
                        message: $"RecoverStuckJobs: Marked Running job as Failed."
                            + $" BackgroundJobId={job.BackgroundJobId}"
                            + $", TenantId={job.TenantId}"
                            + $", JobStartedTime={job.JobStartedTime:O}",
                        sysLogType: SysLogModel.SysLogTypes.Info);
                }
            }
        }

        public static bool IsTimedOut(BackgroundJobModel model)
        {
            if (model.Status != BackgroundJobStatus.Running) return false;
            var timeout = Parameters.BackgroundJobs?.BackgroundJobTimeout ?? 0;
            if (timeout <= 0) return false;
            if (!model.JobStartedTime.InRange()) return false;
            return model.JobStartedTime.AddSeconds(timeout) < DateTime.UtcNow;
        }

        public static void DeleteCompletedJobs(Context context)
        {
            WarnTimedOutRunningJobs(context: context);
            var retentionPeriod = Parameters.BackgroundService.BackgroundJobsRetentionPeriod;
            if (retentionPeriod < 0) return;
            var threshold = DateTime.UtcNow.AddDays(-retentionPeriod);
            var finishedJobs = new BackgroundJobCollection(
                context: context,
                where: Rds.BackgroundJobsWhere()
                    .Status_In(value: new[]
                    {
                        BackgroundJobStatus.Completed,
                        BackgroundJobStatus.Failed
                    })
                    .JobFinishedTime(
                        threshold,
                        _operator: "<"));
            var cancelledJobs = new BackgroundJobCollection(
                context: context,
                where: Rds.BackgroundJobsWhere()
                    .Status(BackgroundJobStatus.Cancelled)
                    .JobFinishedTime(
                        threshold,
                        _operator: "<"));
            var jobs = finishedJobs
                .Concat(cancelledJobs)
                .ToList();
            foreach (var tenantJobs in jobs.GroupBy(j => j.TenantId))
            {
                try
                {
                    foreach (var job in tenantJobs)
                    {
                        if (job.File.IsNullOrEmpty() == false
                            && IoFile.Exists(job.File))
                        {
                            try
                            {
                                IoFile.Delete(job.File);
                            }
                            catch (Exception e)
                            {
                                new SysLogModel(
                                    context: context,
                                    e: e);
                            }
                        }
                        Repository.ExecuteNonQuery(
                            context: context,
                            statements: Rds.PhysicalDeleteBackgroundJobs(
                                where: Rds.BackgroundJobsWhere()
                                    .BackgroundJobId(job.BackgroundJobId)));
                    }
                }
                catch (Exception e)
                {
                    new SysLogModel(
                        context: context,
                        e: e);
                }
            }
        }

        internal static void WarnTimedOutRunningJobs(Context context)
        {
            var timeout = Parameters.BackgroundJobs?.BackgroundJobTimeout ?? 0;
            if (timeout <= 0) return;
            var threshold = DateTime.UtcNow.AddSeconds(-timeout);
            var timedOutJobs = new BackgroundJobCollection(
                context: context,
                where: Rds.BackgroundJobsWhere()
                    .Status(BackgroundJobStatus.Running)
                    .JobStartedTime(
                        threshold,
                        _operator: "<"));
            foreach (var job in timedOutJobs)
            {
                var message = GetResultMessage(
                    displayId: "BackgroundJobRunningTimedOut",
                    language: GetJobLanguage(model: job),
                    jobType: job.JobType);
                var count = Repository.ExecuteNonQuery(
                    context: context,
                    statements: Rds.UpdateBackgroundJobs(
                        where: Rds.BackgroundJobsWhere()
                            .BackgroundJobId(job.BackgroundJobId)
                            .Status(BackgroundJobStatus.Running),
                        param: Rds.BackgroundJobsParam()
                            .Status(BackgroundJobStatus.RunningOverdue)
                            .ResultMessage(message),
                        addUpdatorParam: false,
                        addUpdatedTimeParam: false));
                if (count > 0)
                {
                    new SysLogModel(
                        context: CreateSysLogContext(
                            context: context,
                            model: job),
                        method: nameof(WarnTimedOutRunningJobs),
                        message: $"Warning: Running job timed out (stop unconfirmed)."
                            + $" BackgroundJobId={job.BackgroundJobId}"
                            + $", TenantId={job.TenantId}"
                            + $", JobStartedTime={job.JobStartedTime:O}",
                        sysLogType: SysLogModel.SysLogTypes.Warning);
                }
            }
        }

        public static BackgroundJobModel Get(
            Context context,
            long backgroundJobId)
        {
            var model = new BackgroundJobModel(
                context: context,
                backgroundJobId: backgroundJobId);
            if (model.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return null;
            }
            if (BackgroundJobAccessValidator.CanAccess(
                context: context,
                model: model) == false)
            {
                return null;
            }
            return model;
        }

        public static BackgroundJobCollection GetList(
            Context context,
            long? siteId = null,
            int? status = null,
            BackgroundJobFilterParams filter = null,
            BackgroundJobSortState sortState = null)
        {
            var where = Rds.BackgroundJobsWhere()
                .SiteId(
                    siteId,
                    _using: siteId.HasValue)
                .Status(
                    status,
                    _using: status.HasValue);
            var accessScope = BackgroundJobAccessValidator.GetAccessScope(context: context);
            switch (accessScope)
            {
                case BackgroundJobAccessScope.Tenant:
                    where.TenantId(context.TenantId);
                    break;
                case BackgroundJobAccessScope.Own:
                    where
                        .TenantId(context.TenantId)
                        .UserId(context.UserId);
                    break;
            }
            if (filter != null)
            {
                ApplyFilter(
                    where: where,
                    context: context,
                    filter: filter,
                    accessScope: accessScope);
            }
            var orderBy = BuildOrderBy(sortState: sortState);
            var join = BuildJoin(sortState: sortState);
            return new BackgroundJobCollection(
                context: context,
                join: join,
                where: where,
                orderBy: orderBy);
        }

        private static SqlOrderByCollection BuildOrderBy(BackgroundJobSortState sortState)
        {
            var ob = Rds.BackgroundJobsOrderBy();
            if (sortState == null || !sortState.HasSorters)
            {
                return ob.JobEnqueuedTime(SqlOrderBy.Types.desc);
            }
            foreach (var (columnName, direction) in sortState.GetSorters())
            {
                var isDesc = direction == "desc";
                switch (columnName)
                {
                    case "BackgroundJobId":
                        ob.BackgroundJobId(isDesc ? SqlOrderBy.Types.desc : SqlOrderBy.Types.asc);
                        break;
                    case "JobType":
                        ob.JobType(isDesc ? SqlOrderBy.Types.desc : SqlOrderBy.Types.asc);
                        break;
                    case "Status":
                        ob.Status(isDesc ? SqlOrderBy.Types.desc : SqlOrderBy.Types.asc);
                        break;
                    case "SiteId":
                        ob.SiteId(isDesc ? SqlOrderBy.Types.desc : SqlOrderBy.Types.asc);
                        break;
                    case "SiteName":
                        ob.Add(
                            columnBracket: "\"Title\"",
                            orderType: isDesc
                                ? SqlOrderBy.Types.desc
                                : SqlOrderBy.Types.asc,
                            tableName: "Sites");
                        break;
                    case "UserId":
                        ob.Add(
                            columnBracket: "\"Name\"",
                            orderType: isDesc
                                ? SqlOrderBy.Types.desc
                                : SqlOrderBy.Types.asc,
                            tableName: "Users");
                        break;
                    case "JobEnqueuedTime":
                        ob.JobEnqueuedTime(isDesc ? SqlOrderBy.Types.desc : SqlOrderBy.Types.asc);
                        break;
                    case "ResultMessage":
                        ob.ResultMessage(isDesc ? SqlOrderBy.Types.desc : SqlOrderBy.Types.asc);
                        break;
                    case "File":
                        ob.File(isDesc ? SqlOrderBy.Types.desc : SqlOrderBy.Types.asc);
                        break;
                }
            }
            return ob;
        }

        private static SqlJoinCollection BuildJoin(
            BackgroundJobSortState sortState)
        {
            var join = Rds.BackgroundJobsJoinDefault();
            if (sortState?.GetSorters()
                .Any(s => s.Key == "UserId") == true)
            {
                join.Add(new SqlJoin(
                    tableBracket: "\"Users\"",
                    joinType: SqlJoin.JoinTypes.LeftOuter,
                    joinExpression: "\"BackgroundJobs\".\"UserId\"=\"Users\".\"UserId\""));
            }
            if (sortState?.GetSorters()
                .Any(s => s.Key == "SiteName") == true)
            {
                join.Add(new SqlJoin(
                    tableBracket: "\"Sites\"",
                    joinType: SqlJoin.JoinTypes.LeftOuter,
                    joinExpression: "\"BackgroundJobs\".\"SiteId\"=\"Sites\".\"SiteId\""));
            }
            return join;
        }

        private static void ApplyFilter(
            Rds.BackgroundJobsWhereCollection where,
            Context context,
            BackgroundJobFilterParams filter,
            BackgroundJobAccessScope accessScope)
        {
            if (filter.Statuses?.Any() == true)
            {
                where.Status_In(value: filter.Statuses);
            }
            if (filter.JobTypes?.Any() == true)
            {
                where.JobType(
                    value: filter.JobTypes,
                    multiParamOperator: " or ");
            }
            if (filter.BackgroundJobId.IsNullOrEmpty() == false)
            {
                if (long.TryParse(filter.BackgroundJobId, out var jobId))
                {
                    where.BackgroundJobId(value: jobId);
                }
            }
            if (filter.SiteId.IsNullOrEmpty() == false)
            {
                if (long.TryParse(filter.SiteId, out var siteId))
                {
                    where.SiteId(value: siteId);
                }
            }
            if (filter.SiteName.IsNullOrEmpty() == false)
            {
                var siteTable = Repository.ExecuteTable(
                    context: context,
                    statements: Rds.SelectSites(
                        column: Rds.SitesColumn().SiteId(),
                        where: Rds.SitesWhere()
                            .TenantId(
                                value: context.TenantId,
                                _using: accessScope != BackgroundJobAccessScope.All)
                            .Title(
                                value: $"%{filter.SiteName}%",
                                _operator: " like ")));
                var matchingSiteIds = siteTable
                    .AsEnumerable()
                    .Select(static row => row["SiteId"].ToLong())
                    .ToList();
                if (matchingSiteIds.Any())
                {
                    where.SiteId_In(value: matchingSiteIds);
                }
                else
                {
                    where.SiteId(value: -1);
                }
            }
            if (filter.UserIds?.Any() == true)
            {
                var userIds = filter.UserIds.Where(id => id > 0).ToList();
                if (userIds.Any())
                {
                    where.UserId_In(value: userIds);
                }
            }
            ApplyDateFilter(
                where: where,
                value: filter.JobEnqueuedTime,
                context: context,
                columnBracket: "\"JobEnqueuedTime\"",
                parameterName: "JobEnqueuedTime");
            if (filter.ResultMessage.IsNullOrEmpty() == false)
            {
                where.ResultMessage(
                    value: $"%{filter.ResultMessage}%",
                    _operator: " like ");
            }
            if (filter.File.IsNullOrEmpty() == false)
            {
                where.File(
                    value: $"%{filter.File}%",
                    _operator: " like ");
            }
        }

        private static void ApplyDateFilter(
            Rds.BackgroundJobsWhereCollection where,
            string value,
            Context context,
            string columnBracket,
            string parameterName)
        {
            var range = ParseFirstDateRange(
                value: value,
                context: context);
            ApplyDateFilter(
                where: where,
                range: range,
                context: context,
                columnBracket: columnBracket,
                parameterName: parameterName);
        }

        private static void ApplyDateFilter(
            Rds.BackgroundJobsWhereCollection where,
            (DateTime? from, DateTime? to) range,
            Context context,
            string columnBracket,
            string parameterName)
        {
            if (range.from.HasValue)
            {
                where.Add(
                    columnBrackets: new[] { columnBracket },
                    tableName: "BackgroundJobs",
                    name: $"{parameterName}From",
                    value: ToUniversalTime(value: range.from.Value, context: context),
                    _operator: ">=");
            }
            if (range.to.HasValue)
            {
                where.Add(
                    columnBrackets: new[] { columnBracket },
                    tableName: "BackgroundJobs",
                    name: $"{parameterName}To",
                    value: ToUniversalTime(value: range.to.Value, context: context),
                    _operator: "<=");
            }
        }

        private static DateTime ToUniversalTime(DateTime value, Context context)
        {
            if (value.ToOADate() == 0) return value;
            var timeZoneInfo = context.TimeZoneInfo ?? TimeZoneInfo.Utc;
            return TimeZoneInfo.ConvertTimeToUtc(
                DateTime.SpecifyKind(value, DateTimeKind.Unspecified),
                timeZoneInfo);
        }

        public static DateTime ToBackgroundJobLocalTime(this DateTime value, Context context)
        {
            if (value.ToOADate() == 0) return value;
            var timeZoneInfo = context.TimeZoneInfo ?? TimeZoneInfo.Utc;
            return TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.SpecifyKind(value, DateTimeKind.Unspecified),
                timeZoneInfo);
        }

        private static (DateTime? from, DateTime? to) ParseFirstDateRange(
            string value,
            Context context)
        {
            if (value.IsNullOrEmpty()) return (null, null);
            var items = value.Deserialize<List<string>>();
            if (items == null || items.Any() == false) return (null, null);
            return ParseDateRange(items[0], context);
        }

        internal static (DateTime? from, DateTime? to) ParseDateRange(
            string value,
            Context context)
        {
            if (value.IsNullOrEmpty()) return (null, null);
            var timeZoneInfo = context.TimeZoneInfo ?? TimeZoneInfo.Utc;
            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo);
            switch (value)
            {
                case "Today":
                    return (now.Date, now.Date.AddDays(1).AddMilliseconds(-1));
                case "ThisMonth":
                    return (
                        new DateTime(now.Year, now.Month, 1),
                        new DateTime(now.Year, now.Month, 1).AddMonths(1).AddMilliseconds(-1));
                case "ThisYear":
                    return (
                        new DateTime(now.Year, 1, 1),
                        new DateTime(now.Year, 12, 31, 23, 59, 59, 997));
                default:
                    var parts = value.Split(',');
                    if (parts.Length == 2)
                    {
                        var from = DateTime.TryParse(parts[0], out var f) ? f : (DateTime?)null;
                        var to = DateTime.TryParse(parts[1], out var t) ? t : (DateTime?)null;
                        return (from, to);
                    }
                    return (null, null);
            }
        }

    }

    public class BackgroundJobFilterParams
    {
        public List<int> Statuses { get; set; } = new List<int>();
        public List<string> JobTypes { get; set; } = new List<string>();
        public string BackgroundJobId { get; set; }
        public string SiteId { get; set; }
        public string SiteName { get; set; }
        public List<int> UserIds { get; set; } = new List<int>();
        public string JobEnqueuedTime { get; set; }
        public string File { get; set; }
        public string ResultMessage { get; set; }

        public static BackgroundJobFilterParams FromContext(Context context)
        {
            if (context.Forms.ControlId() == "ViewFilters_Reset")
            {
                return new BackgroundJobFilterParams();
            }
            var filter = new BackgroundJobFilterParams();
            ParseMultiSelectInt(
                context: context,
                key: "ViewFilters__Status",
                list: filter.Statuses);
            ParseMultiSelectString(
                context: context,
                key: "ViewFilters__JobType",
                list: filter.JobTypes);
            var backgroundJobIdForm = context.Forms.Data("ViewFilters__BackgroundJobId");
            filter.BackgroundJobId = backgroundJobIdForm.IsNullOrEmpty()
                ? context.QueryStrings.Data("ViewFilters__BackgroundJobId")
                : backgroundJobIdForm;
            var siteIdForm = context.Forms.Data("ViewFilters__SiteId");
            filter.SiteId = siteIdForm.IsNullOrEmpty()
                ? context.QueryStrings.Data("ViewFilters__SiteId")
                : siteIdForm;
            filter.SiteName = context.Forms.Data("ViewFilters__SiteName");
            ParseMultiSelectInt(
                context: context,
                key: "ViewFilters__UserId",
                list: filter.UserIds);
            filter.JobEnqueuedTime = context.Forms.Data("ViewFilters__JobEnqueuedTime");
            filter.File = context.Forms.Data("ViewFilters__File");
            filter.ResultMessage = context.Forms.Data("ViewFilters__ResultMessage");
            return filter;
        }

        private static void ParseMultiSelectInt(
            Context context,
            string key,
            List<int> list)
        {
            var value = context.Forms.Data(key);
            if (value.IsNullOrEmpty()) return;
            var items = value.Deserialize<List<string>>();
            if (items == null) return;
            foreach (var item in items)
            {
                if (int.TryParse(item, out var i) && i >= 0)
                {
                    list.Add(i);
                }
            }
        }

        private static void ParseMultiSelectString(
            Context context,
            string key,
            List<string> list)
        {
            var value = context.Forms.Data(key);
            if (value.IsNullOrEmpty()) return;
            var items = value.Deserialize<List<string>>();
            if (items == null) return;
            list.AddRange(items.Where(i => !i.IsNullOrEmpty()));
        }
    }

    internal enum BackgroundJobDownloadStatus
    {
        Success,
        NotFound,
        Conflict
    }

    internal class BackgroundJobDownloadResult
    {
        public BackgroundJobDownloadStatus Status;
        public long BackgroundJobId;
        public DateTime LockToken;
        public System.IO.FileInfo FileInfo;
        public string ContentType;

        public static BackgroundJobDownloadResult Success(
            long backgroundJobId,
            DateTime lockToken,
            System.IO.FileInfo fileInfo,
            string contentType)
        {
            return new BackgroundJobDownloadResult
            {
                Status = BackgroundJobDownloadStatus.Success,
                BackgroundJobId = backgroundJobId,
                LockToken = lockToken,
                FileInfo = fileInfo,
                ContentType = contentType
            };
        }

        public static BackgroundJobDownloadResult NotFound()
        {
            return new BackgroundJobDownloadResult
            {
                Status = BackgroundJobDownloadStatus.NotFound
            };
        }

        public static BackgroundJobDownloadResult Conflict()
        {
            return new BackgroundJobDownloadResult
            {
                Status = BackgroundJobDownloadStatus.Conflict
            };
        }

        public void ReleaseLock()
        {
            if (Status == BackgroundJobDownloadStatus.Success)
            {
                BackgroundJobQueue.ReleaseDownloadLock(
                    backgroundJobId: BackgroundJobId,
                    lockToken: LockToken);
            }
        }
    }
}
