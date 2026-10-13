using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Models;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    public class BulkDeleteJobHandler : IBackgroundJobHandler
    {
        public Task ExecuteAsync(
            Context context,
            BackgroundJobModel backgroundJobModel)
        {
            var jobParameters = backgroundJobModel.JobParameters
                .Deserialize<BulkDeleteJobParameters>();
            if (jobParameters == null)
            {
                throw new InvalidDataException(Displays.Get(
                    context: context,
                    id: "BackgroundJobInvalidParameters"));
            }
            var bulkDeleteContext = CreateContext(
                backgroundJobModel: backgroundJobModel,
                jobParameters: jobParameters);
            var resultData = BulkDeleteCore(
                context: bulkDeleteContext,
                siteId: backgroundJobModel.SiteId);
            if (resultData.HasError())
            {
                throw new InvalidDataException(ErrorMessage(
                    context: bulkDeleteContext,
                    resultData: resultData,
                    jobType: backgroundJobModel.JobType));
            }
            backgroundJobModel.ResultMessage = Displays.BackgroundJobResultBulkDeleteCompleted(
                context: bulkDeleteContext,
                data: new string[]
                {
                    BackgroundJobQueue.GetJobTypeLabel(
                        context: bulkDeleteContext,
                        jobType: backgroundJobModel.JobType),
                    resultData.Count.ToString()
                });
            return Task.CompletedTask;
        }

        private static Context CreateContext(
            BackgroundJobModel backgroundJobModel,
            BulkDeleteJobParameters jobParameters)
        {
            var lookupContext = new Context(
                tenantId: backgroundJobModel.TenantId,
                request: false);
            var user = SiteInfo.User(
                context: lookupContext,
                userId: backgroundJobModel.UserId);
            var context = new Context(
                tenantId: backgroundJobModel.TenantId,
                deptId: user.DeptId,
                userId: backgroundJobModel.UserId,
                language: jobParameters.Language,
                request: false,
                setAuthenticated: true)
            {
                TimeZoneInfo = BackgroundJobQueue.ResolveTimeZoneInfo(
                    timeZoneId: jobParameters.TimeZoneId,
                    fallback: Environments.TimeZoneInfoDefault),
                ContentType = jobParameters.ContentType,
                Forms = new Forms(),
                SessionData = jobParameters.SessionData ?? new Dictionary<string, string>(),
                UserSessionData = jobParameters.UserSessionData
                    ?? new Dictionary<string, string>(),
                AbsoluteUri = Parameters.Service.AbsoluteUri ?? jobParameters.AbsoluteUri,
                AbsolutePath = jobParameters.AbsolutePath,
                ApplicationPath = jobParameters.ApplicationPath,
                SiteId = backgroundJobModel.SiteId,
                Controller = jobParameters.Controller,
                Action = jobParameters.Action,
                Id = backgroundJobModel.SiteId,
                LoginId = user.LoginId

            };
            foreach (var item in jobParameters.Forms ?? new Dictionary<string, string>())
            {
                context.Forms[item.Key] = item.Value;
            }
            context.SetTenantProperties(force: true);
            return context;
        }

        private static BulkDeleteResultData BulkDeleteCore(
            Context context,
            long siteId)
        {
            var itemModel = new ItemModel(
                context: context,
                referenceId: siteId);
            itemModel.SetSite(
                context: context,
                initSiteSettings: true,
                setSiteIntegration: true);
            var siteModel = itemModel.Site;
            switch (siteModel.ReferenceType)
            {
                case "Issues":
                    return IssueUtilities.BulkDeleteCore(
                        context: context,
                        ss: siteModel.SiteSettings);
                case "Results":
                    return ResultUtilities.BulkDeleteCore(
                        context: context,
                        ss: siteModel.SiteSettings);
                default:
                    throw new InvalidDataException(Displays.Get(
                        context: context,
                        id: "BackgroundJobInvalidParameters"));
            }
        }

        private static string ErrorMessage(
            Context context,
            BulkDeleteResultData resultData,
            string jobType)
        {
            return resultData.ErrorData?.Message(context: context)?.Text
                ?? Displays.Get(
                    context: context,
                    id: "BackgroundJobExecutionFailed",
                    data: BackgroundJobQueue.GetJobTypeLabel(
                        context: context,
                        jobType: jobType));
        }
    }

    public class BulkDeleteJobParameters
    {
        public string ContentType { get; set; }
        public string Language { get; set; }
        public string TimeZoneId { get; set; }
        public Dictionary<string, string> Forms { get; set; }
        public Dictionary<string, string> SessionData { get; set; }
        public Dictionary<string, string> UserSessionData { get; set; }
        public string ApplicationPath { get; set; }
        public string AbsoluteUri { get; set; }
        public string AbsolutePath { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }

        public static BulkDeleteJobParameters FromContext(Context context)
        {
            return new BulkDeleteJobParameters
            {
                ContentType = context.ContentType,
                Language = context.Language,
                TimeZoneId = context.TimeZoneInfo?.Id,
                Forms = new Dictionary<string, string>(context.Forms),
                SessionData = new Dictionary<string, string>(context.SessionData),
                UserSessionData = new Dictionary<string, string>(context.UserSessionData),
                ApplicationPath = context.ApplicationPath,
                AbsoluteUri = context.AbsoluteUri,
                AbsolutePath = context.AbsolutePath,
                Controller = context.Controller,
                Action = context.Action
            };
        }
    }
}
