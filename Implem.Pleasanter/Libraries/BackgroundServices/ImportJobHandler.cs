using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using IoFile = System.IO.File;

namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    public class ImportJobHandler : IBackgroundJobHandler
    {
        public Task ExecuteAsync(
            Context context,
            BackgroundJobModel backgroundJobModel)
        {
            try
            {
                var jobParameters = backgroundJobModel.JobParameters
                    .Deserialize<ImportJobParameters>();
                if (jobParameters == null)
                {
                    throw new InvalidDataException(Displays.Get(
                        context: context,
                        id: "BackgroundJobInvalidParameters"));
                }
                var importContext = CreateContext(
                    backgroundJobModel: backgroundJobModel,
                    jobParameters: jobParameters);
                if (!IoFile.Exists(backgroundJobModel.File))
                {
                    throw new InvalidDataException(Displays.Get(
                        context: importContext,
                        id: "BackgroundJobInvalidParameters"));
                }
                var csvBytes = IoFile.ReadAllBytes(backgroundJobModel.File);
                var resultData = ImportCore(
                    context: importContext,
                    siteId: backgroundJobModel.SiteId,
                    csvBytes: csvBytes);
                if (resultData.HasError())
                {
                    throw new InvalidDataException(ErrorMessage(
                        context: importContext,
                        resultData: resultData,
                        jobType: backgroundJobModel.JobType));
                }
                backgroundJobModel.ResultMessage = Displays.BackgroundJobResultImportCompleted(
                    context: importContext,
                    data: new string[]
                    {
                        BackgroundJobQueue.GetJobTypeLabel(
                            context: importContext,
                            jobType: backgroundJobModel.JobType),
                        resultData.InsertCount.ToString(),
                        resultData.UpdateCount.ToString()
                    });
                return Task.CompletedTask;
            }
            finally
            {
                DeleteInputFile(
                    context: context,
                    backgroundJobModel: backgroundJobModel);
            }
        }

        private static Context CreateContext(
            BackgroundJobModel backgroundJobModel,
            ImportJobParameters jobParameters)
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
                ContentType = jobParameters.ContentType,
                Forms = new Forms(),
                SessionData = jobParameters.SessionData ?? new Dictionary<string, string>(),
                UserSessionData = jobParameters.UserSessionData
                    ?? new Dictionary<string, string>(),
                AbsoluteUri = Parameters.Service.AbsoluteUri,
                SiteId = backgroundJobModel.SiteId
            };
            foreach (var item in jobParameters.Forms ?? new Dictionary<string, string>())
            {
                context.Forms[item.Key] = item.Value;
            }
            context.SetTenantProperties(force: true);
            return context;
        }

        private static ImportResultData ImportCore(
            Context context,
            long siteId,
            byte[] csvBytes)
        {
            var itemModel = new ItemModel(
                context: context,
                referenceId: siteId);
            itemModel.SetSite(context: context);
            var siteModel = itemModel.Site;
            switch (siteModel.ReferenceType)
            {
                case "Issues":
                {
                    var ss = siteModel.IssuesSiteSettings(
                        context: context,
                        referenceId: siteModel.SiteId,
                        setAllChoices: true);
                    return IssueUtilities.ImportCore(
                        context: context,
                        ss: ss,
                        csvBytes: csvBytes,
                        encoding: context.Forms.Data("Encoding"),
                        updatableImport: context.Forms.Bool("UpdatableImport"),
                        key: context.Forms.Data("Key"),
                        migrationMode: ss.AllowMigrationMode == true
                            && context.Forms.Bool("MigrationMode"));
                }
                case "Results":
                {
                    var ss = siteModel.ResultsSiteSettings(
                        context: context,
                        referenceId: siteModel.SiteId,
                        setAllChoices: true);
                    return ResultUtilities.ImportCore(
                        context: context,
                        ss: ss,
                        csvBytes: csvBytes,
                        encoding: context.Forms.Data("Encoding"),
                        updatableImport: context.Forms.Bool("UpdatableImport"),
                        key: context.Forms.Data("Key"),
                        migrationMode: ss.AllowMigrationMode == true
                            && context.Forms.Bool("MigrationMode"));
                }
                default:
                    throw new InvalidDataException(Displays.Get(
                        context: context,
                        id: "BackgroundJobInvalidParameters"));
            }
        }

        private static string ErrorMessage(
            Context context,
            ImportResultData resultData,
            string jobType)
        {
            return resultData.CustomMessage?.Text
                ?? resultData.ErrorData?.Message(context: context)?.Text
                ?? Displays.Get(
                    context: context,
                    id: "BackgroundJobExecutionFailed",
                    data: BackgroundJobQueue.GetJobTypeLabel(
                        context: context,
                        jobType: jobType));
        }

        private static void DeleteInputFile(
            Context context,
            BackgroundJobModel backgroundJobModel)
        {
            var path = backgroundJobModel.File;
            if (path.IsNullOrEmpty()
                || IoFile.Exists(path) == false)
            {
                return;
            }
            try
            {
                IoFile.Delete(path);
            }
            catch (Exception e)
            {
                new SysLogModel(
                    context: context,
                    e: e);
            }
        }
    }

    public class ImportJobParameters
    {
        public string ContentType { get; set; }
        public string Language { get; set; }
        public Dictionary<string, string> Forms { get; set; }
        public Dictionary<string, string> SessionData { get; set; }
        public Dictionary<string, string> UserSessionData { get; set; }

        public static ImportJobParameters FromContext(Context context)
        {
            return new ImportJobParameters
            {
                ContentType = context.ContentType,
                Language = context.Language,
                Forms = new Dictionary<string, string>(context.Forms),
                SessionData = new Dictionary<string, string>(context.SessionData),
                UserSessionData = new Dictionary<string, string>(context.UserSessionData)
            };
        }
    }
}
