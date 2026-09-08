using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
namespace Implem.Pleasanter.Libraries.AiConnect
{
    public static class AiConnectResyncUtilities
    {
        private const int PageSize = 100;

        public static AiConnectResyncResult Resync(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            IAiConnectHttpClient client = null)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = new AiConnectResyncResult
            {
                TargetCount = TargetCount(
                    context: context,
                    ss: ss)
            };
            Log(
                context: context,
                ss: ss,
                aiProvider: aiProvider,
                phase: Phases.Start,
                message: $"TargetCount: {result.TargetCount}",
                sysLogType: SysLogModel.SysLogTypes.Info);
            var provider = AiConnectProviderFactory.Get(
                providerType: aiProvider.ProviderType);
            if (provider == null)
            {
                Log(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    phase: Phases.Provider,
                    message: "Message: The provider type is invalid.",
                    sysLogType: SysLogModel.SysLogTypes.UserError);
                result.ErrorData = new ErrorData(
                    type: Error.Types.ServerConnectionError);
                Completed(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    result: result,
                    stopwatch: stopwatch);
                return result;
            }
            ss.SetChoiceHash(context: context);
            var deleteAllResult = provider.DeleteAll(
                context: context,
                ss: ss,
                aiProvider: aiProvider,
                client: client);
            result.DeletedCount = deleteAllResult.DeletedCount;
            result.DeleteFailedCount = deleteAllResult.FailedCount;
            Log(
                context: context,
                ss: ss,
                aiProvider: aiProvider,
                phase: Phases.Delete,
                message: $"DeletedCount: {result.DeletedCount}"
                    + $", DeleteFailedCount: {result.DeleteFailedCount}",
                sysLogType: SysLogModel.SysLogTypes.Info);
            if (deleteAllResult.ErrorData.Type != Error.Types.None)
            {
                result.ErrorData = deleteAllResult.ErrorData;
                Completed(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    result: result,
                    stopwatch: stopwatch);
                return result;
            }
            switch (ss.ReferenceType)
            {
                case "Issues":
                    ResyncIssues(
                        context: context,
                        ss: ss,
                        aiProvider: aiProvider,
                        provider: provider,
                        client: client,
                        result: result);
                    break;
                case "Results":
                    ResyncResults(
                        context: context,
                        ss: ss,
                        aiProvider: aiProvider,
                        provider: provider,
                        client: client,
                        result: result);
                    break;
            }
            Completed(
                context: context,
                ss: ss,
                aiProvider: aiProvider,
                result: result,
                stopwatch: stopwatch);
            return result;
        }

        public static int TargetCount(Context context, SiteSettings ss)
        {
            switch (ss?.ReferenceType)
            {
                case "Issues":
                    return Rds.ExecuteScalar_int(
                        context: context,
                        statements: Rds.SelectIssues(
                            column: Rds.IssuesColumn().IssuesCount(),
                            where: Rds.IssuesWhere().SiteId(ss.SiteId)));
                case "Results":
                    return Rds.ExecuteScalar_int(
                        context: context,
                        statements: Rds.SelectResults(
                            column: Rds.ResultsColumn().ResultsCount(),
                            where: Rds.ResultsWhere().SiteId(ss.SiteId)));
                default:
                    return 0;
            }
        }

        private static void ResyncIssues(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            IAiConnectProvider provider,
            IAiConnectHttpClient client,
            AiConnectResyncResult result)
        {
            var offset = 0;
            while (true)
            {
                var issueCollection = new IssueCollection(
                    context: context,
                    ss: ss,
                    where: Rds.IssuesWhere().SiteId(ss.SiteId),
                    orderBy: Rds.IssuesOrderBy().IssueId(),
                    offset: offset,
                    pageSize: PageSize);
                if (issueCollection.Any() != true)
                {
                    break;
                }
                issueCollection.ForEach(issueModel => ResyncRecord(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    provider: provider,
                    referenceType: "Issues",
                    referenceId: issueModel.IssueId,
                    replacedDisplayValues: value => issueModel.ReplacedDisplayValues(
                        context: context,
                        ss: ss,
                        value: value),
                    client: client,
                    result: result));
                offset += PageSize;
            }
        }

        private static void ResyncResults(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            IAiConnectProvider provider,
            IAiConnectHttpClient client,
            AiConnectResyncResult result)
        {
            var offset = 0;
            while (true)
            {
                var resultCollection = new ResultCollection(
                    context: context,
                    ss: ss,
                    where: Rds.ResultsWhere().SiteId(ss.SiteId),
                    orderBy: Rds.ResultsOrderBy().ResultId(),
                    offset: offset,
                    pageSize: PageSize);
                if (resultCollection.Any() != true)
                {
                    break;
                }
                resultCollection.ForEach(resultModel => ResyncRecord(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    provider: provider,
                    referenceType: "Results",
                    referenceId: resultModel.ResultId,
                    replacedDisplayValues: value => resultModel.ReplacedDisplayValues(
                        context: context,
                        ss: ss,
                        value: value),
                    client: client,
                    result: result));
                offset += PageSize;
            }
        }

        private static void ResyncRecord(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            IAiConnectProvider provider,
            string referenceType,
            long referenceId,
            Func<string, string> replacedDisplayValues,
            IAiConnectHttpClient client,
            AiConnectResyncResult result)
        {
            try
            {
                var errorData = AiConnectUtilities.Output(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    referenceType: referenceType,
                    referenceId: referenceId,
                    replacedDisplayValues: replacedDisplayValues);
                if (errorData.Type != Error.Types.None)
                {
                    result.FailedCount++;
                    return;
                }
                var sendResult = provider.Send(
                    context: context,
                    ss: ss,
                    aiProvider: aiProvider,
                    referenceType: referenceType,
                    referenceId: referenceId,
                    filePath: AiConnectUtilities.FilePath(
                        context: context,
                        ss: ss,
                        aiProvider: aiProvider,
                        referenceType: referenceType,
                        referenceId: referenceId),
                    client: client,
                    forceCreate: true);
                if (sendResult.ErrorData.Type != Error.Types.None)
                {
                    result.FailedCount++;
                    return;
                }
                result.SucceededCount++;
            }
            catch (Exception e)
            {
                result.FailedCount++;
                new SysLogModel(context, e);
            }
        }

        private static void Completed(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            AiConnectResyncResult result,
            Stopwatch stopwatch)
        {
            stopwatch.Stop();
            Log(
                context: context,
                ss: ss,
                aiProvider: aiProvider,
                phase: Phases.Completed,
                message: $"DeletedCount: {result.DeletedCount}"
                    + $", DeleteFailedCount: {result.DeleteFailedCount}"
                    + $", TargetCount: {result.TargetCount}"
                    + $", SucceededCount: {result.SucceededCount}"
                    + $", FailedCount: {result.FailedCount}"
                    + $", Elapsed: {stopwatch.ElapsedMilliseconds}ms",
                sysLogType: SysLogModel.SysLogTypes.Info);
        }

        private static void Log(
            Context context,
            SiteSettings ss,
            AiProvider aiProvider,
            string phase,
            string message,
            SysLogModel.SysLogTypes sysLogType)
        {
            var items = new List<string>
            {
                $"Phase: {phase}",
                $"TenantId: {context?.TenantId}",
                $"SiteId: {ss?.SiteId}",
                $"ReferenceType: {ss?.ReferenceType}",
                $"AiProviderId: {aiProvider?.Id}",
                $"AiProviderTitle: {aiProvider?.Title}",
                $"ProviderType: {aiProvider?.ProviderType}",
                message
            };
            new SysLogModel(
                context: context,
                method: nameof(Resync),
                message: items.Join(", "),
                sysLogType: sysLogType);
        }

        private static class Phases
        {
            public const string Start = "Start";
            public const string Provider = "Provider";
            public const string Delete = "Delete";
            public const string Completed = "Completed";
        }
    }

    public class AiConnectResyncResult
    {
        public int DeletedCount { get; set; }
        public int DeleteFailedCount { get; set; }
        public int TargetCount { get; set; }
        public int SucceededCount { get; set; }
        public int FailedCount { get; set; }
        public ErrorData ErrorData { get; set; } = new ErrorData(type: Error.Types.None);
    }
}
